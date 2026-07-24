# TTS Audio Output Troubleshooting Guide

## Possible Issues and Solutions

### 1. Edge-TTS Not Installed
If you're using `EdgeTTS` provider (default), you need to install the edge-tts Python package:

```powershell
# Install edge-tts via pip
pip install edge-tts

# Verify installation
edge-tts --help
```

If you don't have Python/pip installed:
1. Install Python from https://www.python.org/downloads/
2. Make sure "Add Python to PATH" is checked during installation
3. Run the pip install command above

### 2. Switch to ElevenLabs (if Edge-TTS doesn't work)
Edit `appsettings.json`:

```json
"VoicePipeline": {
	"TtsProvider": "ElevenLabs"  // Changed from "EdgeTTS"
}
```

**Note**: ElevenLabs requires a valid API key in the configuration.

### 3. Check Audio Device
Make sure your system has a working audio output device:
- Check Windows Sound settings
- Verify default playback device is set
- Test with other applications (e.g., play a YouTube video)

### 4. Enable Detailed Logging
The code now includes extensive logging. Check the console output or log files for:
- "PlayAsync called with X bytes of audio data"
- "Starting audio playback..."
- Any error messages

### 5. Test TTS Directly

Add this code to your main application startup to test TTS:

```csharp
using Jarvis.Testing;

// After building the host/service provider:
await TtsTestRunner.TestTtsAsync(serviceProvider);
```

### 6. Manual Test with Edge-TTS

Test edge-tts manually from command line:

```powershell
edge-tts --voice "en-US-GuyNeural" --text "Hello, this is a test" --write-media test.mp3
```

Then play the `test.mp3` file. If this doesn't work, edge-tts isn't properly installed.

### 7. Check Configuration

Verify your `appsettings.json` settings:

```json
{
  "VoicePipeline": {
	"Enabled": true,
	"EnableTts": true,        // Must be true
	"TtsProvider": "EdgeTTS"   // or "ElevenLabs"
  },
  "TTS": {
	"Provider": "EdgeTTS",     // Should match VoicePipeline.TtsProvider
	"EdgeTTS": {
	  "Voice": "en-US-GuyNeural"
	}
  }
}
```

### 8. Common Error Messages

**"edge-tts is not recognized"**
- Edge-TTS not installed or not in PATH
- Solution: Install edge-tts via pip

**"Audio file was not found"**
- Edge-TTS failed to generate the file
- Check edge-tts installation
- Check temp directory permissions

**"No audio data generated"**
- TTS provider returned empty array
- Check provider configuration
- For ElevenLabs: verify API key is valid

**No error but no sound**
- Audio device issue
- Volume muted
- Wrong output device selected in Windows

### 9. Diagnostic Commands

Run these in PowerShell to diagnose:

```powershell
# Check if edge-tts is installed
where.exe edge-tts

# Check Python version
python --version

# Check pip
pip --version

# List installed Python packages
pip list | Select-String "edge-tts"

# Test edge-tts directly
edge-tts --voice "en-US-GuyNeural" --text "test" --write-media test.mp3
```

### 10. Alternative: Use System.Speech (Windows only)

If both Edge-TTS and ElevenLabs don't work, you could create a Windows SAPI provider:

```csharp
// This would use Windows built-in TTS (lower quality but always available)
using System.Speech.Synthesis;

var synth = new SpeechSynthesizer();
synth.Speak("Hello World");
```

## Next Steps

1. **First**, verify edge-tts is installed (or switch to ElevenLabs)
2. **Then**, run the TTS test (`TtsTestRunner.TestTtsAsync`)
3. **Check**, logs for detailed error messages
4. **Report**, what you see in the console/logs

The extensive logging I added will show exactly where the process is failing.
