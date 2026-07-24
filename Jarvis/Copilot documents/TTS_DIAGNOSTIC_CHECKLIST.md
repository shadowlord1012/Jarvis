# TTS Diagnostic Checklist

Run your app and check the logs for these specific messages:

## Startup Phase
```
[Info] Starting JARVIS...
[Success/Error] Voice pipeline initialized   ← CRITICAL: Is this present?
[Info] Loading AI services and plugins...
[Success] AI service ready
[Success] JARVIS ready
```

## If "Voice pipeline initialized" is MISSING:

Look for this instead:
```
[Error] Failed to initialize voice pipeline: <error message>
```

The console window should also show:
```
Voice pipeline error details: <full exception>
```

**Common Errors:**

### 1. Missing AIInputController
```
Error: Unable to resolve service for type 'Jarvis.UI.Controls.HUD.Models.AIInputController'
```
**Fix:** Verify BootStrapper.cs registers AIInputController before VoicePipelineBridge

### 2. Missing IVoicePipelineService
```
Error: Unable to resolve service for type 'Jarvis.Interfaces.IVoicePipelineService'
```
**Fix:** Verify Speech services are registered in BootStrapper.cs

### 3. Missing VoicePipelineOptions
```
Error: Unable to resolve service for type 'IOptions<VoicePipelineOptions>'
```
**Fix:** Verify appsettings.json has "VoicePipeline" section

## Input Phase (after typing "hello")

### If bridge initialized successfully, you should see:
```
[Info] [Summary] [Input] hello
[Info] Voice pipeline processing input: hello
[Info] VoicePipeline - Sending request to AI: hello
[Info] AI State changed: Offline ->Thinking
[Info] LLM response received...
[Info] TTS-EdgeTTS: <sentence>
[Info] AudioService - PlayAsync called with XXXX bytes
[Info] AudioService - Starting audio playback...
[Info] Voice pipeline completed processing
```

### If you DON'T see "Voice pipeline processing input":

**Check:**
1. Is `VoicePipeline.Enabled = true` in appsettings.json?
2. Is the InputSubmittedEvent being fired?
3. Is the bridge's OnInputSubmitted handler being called?

## Quick Test Commands

### Test if bridge is registered:
Open your app, then check logs for "Voice pipeline initialized"

### Test if event is firing:
Add this temporary logging to see if event fires (not required, just for diagnostics):
- Open `UI\Controls\HUD\Models\AIInputController.cs`
- Find the line: `InputSubmittedEvent?.Invoke(this, input);`
- Add before it: `Console.WriteLine($"EVENT: Firing InputSubmittedEvent with: {input}");`

### Test VoicePipeline directly:
You can test the voice pipeline in isolation using the TtsTestRunner we created earlier:
```powershell
# From the Jarvis directory
dotnet run -- test-tts "hello world"
```

## What to Report

If TTS still doesn't work, copy and paste:

1. **Startup logs** (from "Starting JARVIS" to "JARVIS ready")
2. **Any error messages** about voice pipeline
3. **Input logs** (from when you type "hello" until AI response completes)
4. **The result of:** grep "Voice pipeline" in your log file

This will help identify exactly where the pipeline is breaking!
