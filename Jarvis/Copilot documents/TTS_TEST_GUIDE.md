# Quick TTS Test Guide

## What Was Fixed

The TTS system wasn't being called at runtime. We added:
1. **VoicePipelineBridge** - connects UI input to voice pipeline
2. Event hook in **AIInputController** - fires when user submits input
3. Registration in **BootStrapper** and **App.xaml.cs**

## Before Running

### Install Edge-TTS (Required for EdgeTTS provider)

```powershell
pip install edge-tts
```

Test it:
```powershell
edge-tts --text "hello world" --write-media test.mp3
```
If you hear audio, it's working!

## What to Look For in Logs

### ✅ SUCCESS - You Should See:
```
[Info] [Voice pipeline initialized]
[Info] [Voice pipeline processing input: hello]
[Info] [VoicePipeline] Sending request to AI: hello
[Info] [AI Service] Streaming iteration 1.
[Info] [TTS-EdgeTTS] <sentence being spoken>
[Info] [AudioService] PlayAsync called with XXXX bytes
[Info] [AudioService] Starting audio playback...
[Info] [AudioService] Playback completed successfully
[Info] [Voice pipeline completed processing]
```

### ❌ MISSING TTS (before fix):
```
[Info] [Summary] [Input] hello
[Info] [AI State changed:] Offline ->Thinking
[Info] [Ollama] Response parsed successfully.
[Info] [AI State changed:] Thinking ->Idle
```
(No TTS logs = pipeline never called)

## Test Steps

1. **Start the application**
2. **Wait for "JARVIS ready"** message
3. **Type:** `hello`
4. **Press Enter**

### Expected Result:
- UI shows AI response text
- **You hear the response spoken aloud**
- Logs show TTS and AudioService entries

## Troubleshooting

### No Audio but TTS Logs Present

Check:
```powershell
# Test system audio
edge-tts --text "test" --write-media test.mp3
.\test.mp3
```

### No TTS Logs at All

Check `appsettings.json`:
```json
"VoicePipeline": {
  "Enabled": true,      // ← Must be true
  "EnableTts": true     // ← Must be true
}
```

### "edge-tts not found" Error

```powershell
pip install edge-tts
# or
pip3 install edge-tts
```

### Switch to ElevenLabs (if Edge fails)

In `appsettings.json`:
```json
"VoicePipeline": {
  "TtsProvider": "ElevenLabs"
},
"TTS": {
  "Provider": "ElevenLabs",
  "ElevenLabs": {
	"ApiKey": "your_api_key_here",
	"VoiceId": "21m00Tcm4TlvDq8ikWAM"
  }
}
```

## Configuration Reference

### Disable TTS (Text Only Mode)
```json
"VoicePipeline": {
  "EnableTts": false
}
```

### Change Voice (EdgeTTS)
```json
"TTS": {
  "EdgeTTS": {
	"Voice": "en-US-GuyNeural"  // Male
	// or
	"Voice": "en-US-JennyNeural" // Female
  }
}
```

### List Available Voices
```powershell
edge-tts --list-voices
```

## Success Criteria

✅ You see "Voice pipeline initialized" in logs  
✅ You see "Voice pipeline processing input" when you type  
✅ You see TTS-EdgeTTS or TTS-ElevenLabs entries  
✅ You see AudioService PlayAsync messages  
✅ **You hear audio output from your speakers**

If all these are true = TTS is working! 🎉
