# ✅ EdgeTTS Command Fix

## The Problem
```
System.ComponentModel.Win32Exception (2): An error occurred trying to start process 'edge-tts'
The system cannot find the file specified.
```

The `edge-tts` command wasn't in the system PATH, even though the package was installed via pip.

## The Solution

Changed `EdgeTtsProvider.cs` to use Python module invocation instead of direct command:

### Before:
```csharp
FileName = "edge-tts",
Arguments = $"--voice \"{_options.Voice}\" " +
		   $"--text \"{text}\" " +
		   $"--write-media \"{tempFile}\""
```

### After:
```csharp
FileName = "python",
Arguments = $"-m edge_tts " +
		   $"--voice \"{_options.Voice}\" " +
		   $"--text \"{text}\" " +
		   $"--write-media \"{tempFile}\""
```

## Why This Works

**Python module invocation** (`python -m edge_tts`) works reliably because:
- ✅ Python is in PATH
- ✅ Finds the module regardless of Scripts folder PATH issues
- ✅ Works on Windows, Linux, and macOS
- ✅ No PATH configuration needed

## Verification

Tested successfully:
```powershell
python -m edge_tts --text "Test" --write-media test.mp3
# ✅ File created successfully
```

## Build Status
✅ **Build Successful**

---

## **Ready to Test TTS!**

Now when you run the app and type "hello", you should:

1. ✅ See "Voice pipeline connected"
2. ✅ See "Voice pipeline processing input: hello"
3. ✅ See "[TTS-EdgeTTS] Synthesizing: <response>"
4. ✅ See "[AudioService] PlayAsync called..."
5. ✅ **HEAR AUDIO!** 🔊

### Expected Log Flow:
```
[Success] Voice pipeline connected
[Info] [VoicePipelineBridge] Voice pipeline processing input: hello
[Info] [VoicePipeline] Sending request to AI: hello
[Info] [AI State changed:] Offline ->Thinking
[Debug] [Ollama] Response parsed successfully
[Info] [TTS-EdgeTTS] Synthesizing: Hello! How can I help you?
[Info] [AudioService] PlayAsync called with 45678 bytes
[Info] [AudioService] Starting audio playback...
[Info] [AudioService] Playback completed successfully
[Info] [VoicePipelineBridge] Voice pipeline completed processing
```

## If You Still Want to Use Direct Command

To fix PATH issues and use `edge-tts` directly:

### Find Python Scripts folder:
```powershell
python -c "import site; print(site.USER_BASE + '\\Scripts')"
```

### Add to PATH:
```powershell
$scriptsPath = python -c "import site; print(site.USER_BASE + '\\Scripts')"
[Environment]::SetEnvironmentVariable("Path", $env:Path + ";$scriptsPath", "User")
```

But **using `python -m edge_tts` is the better solution** - no PATH configuration needed!

---

## Alternative: Switch to ElevenLabs

If EdgeTTS continues to have issues, you already have ElevenLabs configured in `appsettings.json`:

```json
"VoicePipeline": {
  "TtsProvider": "ElevenLabs"  // ← Change from "EdgeTTS"
}
```

Your API key is already configured, so this would work immediately!

---

**The EdgeTTS command issue is SOLVED!** 🎉

TTS should now work perfectly via `python -m edge_tts`.
