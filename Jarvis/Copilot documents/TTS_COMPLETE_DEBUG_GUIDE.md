# TTS Debugging Guide - Complete Flow

## What We Just Fixed

1. **Added error handling** in App.xaml.cs to catch bridge initialization failures
2. **Added diagnostic logging** in VoicePipelineBridge constructor
3. **Fixed registration order** in BootStrapper.cs (though DI handles this automatically)

## Expected Log Flow (COMPLETE)

### Phase 1: Startup
```
[2026-XX-XX XX:XX:XX] [Info] Starting JARVIS...
[2026-XX-XX XX:XX:XX] [Info] VoicePipelineBridge constructor called. Enabled=True, EnableTts=True
[2026-XX-XX XX:XX:XX] [Info] VoicePipelineBridge subscribed to InputSubmittedEvent
[2026-XX-XX XX:XX:XX] [Success] Voice pipeline initialized  ← KEY SUCCESS INDICATOR
[2026-XX-XX XX:XX:XX] [Info] Loading AI services and plugins...
[2026-XX-XX XX:XX:XX] [Success] AI service ready
[2026-XX-XX XX:XX:XX] [Success] JARVIS ready
```

### Phase 2: User Input "hello"
```
[2026-XX-XX XX:XX:XX] [Summary] [Input] hello
[2026-XX-XX XX:XX:XX] [Info] Voice pipeline processing input: hello
[2026-XX-XX XX:XX:XX] [Info] [VoicePipeline] Sending request to AI: hello
[2026-XX-XX XX:XX:XX] [Info] [AI State changed:] Offline ->Thinking
[2026-XX-XX XX:XX:XX] [Debug] [Ollama] Sending request to Ollama...
[2026-XX-XX XX:XX:XX] [Debug] [Ollama] Response parsed successfully.
[2026-XX-XX XX:XX:XX] [Info] [TTS-EdgeTTS] Synthesizing: Hello! How can I help you today?
[2026-XX-XX XX:XX:XX] [Info] [AudioService] PlayAsync called with 45678 bytes
[2026-XX-XX XX:XX:XX] [Info] [AudioService] Temp file created: C:\Users\...\tmp12345.mp3
[2026-XX-XX XX:XX:XX] [Info] [AudioService] Starting audio playback...
[2026-XX-XX XX:XX:XX] [Info] [AudioService] Playback completed successfully
[2026-XX-XX XX:XX:XX] [Info] Voice pipeline completed processing
[2026-XX-XX XX:XX:XX] [Info] [AI State changed:] Thinking ->Idle
```

## What to Look For

### ✅ SUCCESS Indicators:
1. "VoicePipelineBridge constructor called"
2. "VoicePipelineBridge subscribed to InputSubmittedEvent"
3. "Voice pipeline initialized" (green/success)
4. "Voice pipeline processing input: hello"
5. "TTS-EdgeTTS" or "TTS-ElevenLabs" entries
6. "AudioService" playback entries
7. **YOU HEAR AUDIO** 🔊

### ❌ FAILURE Indicators:

#### Initialization Failure:
```
[Error] Failed to initialize voice pipeline: <message>
```
Followed by console output:
```
Voice pipeline error details: <full stack trace>
```

**Action:** Copy the full error and check:
- Is `AIInputController` registered in BootStrapper?
- Is `IVoicePipelineService` registered?
- Is `VoicePipelineOptions` configured in appsettings.json?

#### Event Not Firing:
If you see:
```
[Summary] [Input] hello
[Info] [AI State changed:] Offline ->Thinking
```
But NO "Voice pipeline processing input" message:

**Possible causes:**
1. VoicePipelineBridge didn't initialize
2. InputSubmittedEvent isn't being invoked by AIInputController
3. VoicePipeline.Enabled = false in config

#### TTS Not Executing:
If you see:
```
[Info] Voice pipeline processing input: hello
[Info] [VoicePipeline] Sending request to AI: hello
```
But NO "TTS-EdgeTTS" or "TTS-ElevenLabs":

**Check:**
1. `VoicePipeline.EnableTts = true` in appsettings.json
2. AI response is being received (check for "LLM response received")
3. Sentences are being detected by the buffer

## Test Procedure

### Step 1: Run the app
```powershell
# From Visual Studio: Press F5
# Or from terminal:
cd C:\Users\missi\source\repos\Jarvis
dotnet run
```

### Step 2: Wait for "JARVIS ready"
Watch the logs/console for the startup sequence

### Step 3: Check for bridge initialization
Look for:
- "VoicePipelineBridge constructor called"
- "Voice pipeline initialized"

**If missing:** Check console for error details

### Step 4: Type "hello" and press Enter

### Step 5: Observe logs
You should see the complete Phase 2 flow above

### Step 6: Listen for audio
If all logs appear but no sound:
- Check Windows volume mixer
- Test: `edge-tts --text "test" --write-media test.mp3` then play test.mp3
- Check your default audio device

## Common Issues & Fixes

### Issue 1: Bridge constructor called but Enabled=False
**Log:**
```
VoicePipelineBridge constructor called. Enabled=False, EnableTts=True
```

**Fix:**
Edit `appsettings.json`:
```json
"VoicePipeline": {
  "Enabled": true,  // ← Change to true
  "EnableTts": true
}
```

### Issue 2: Constructor called but EnableTts=False
**Log:**
```
VoicePipelineBridge constructor called. Enabled=True, EnableTts=False
```

**Fix:**
Edit `appsettings.json`:
```json
"VoicePipeline": {
  "Enabled": true,
  "EnableTts": true  // ← Change to true
}
```

### Issue 3: TTS runs but "edge-tts not found"
**Log:**
```
[Error] TTS synthesis failed: edge-tts not found
```

**Fix:**
```powershell
pip install edge-tts
edge-tts --version  # Verify
```

### Issue 4: Constructor never called
**Action:**
Check if DI registration exists:
```csharp
// In BootStrapper.cs, should have:
builder.Services.AddSingleton<VoicePipelineBridge>();
```

And App.xaml.cs should have:
```csharp
var voicePipelineBridge = _host.Services.GetRequiredService<VoicePipelineBridge>();
```

## Emergency Bypass Test

To test TTS in isolation without the full pipeline:

1. Create a test file:
```csharp
// TestTts.cs
var edge = new EdgeTtsProvider(options, logger);
var audio = await edge.SynthesizeAsync("hello world");
File.WriteAllBytes("test.mp3", audio);
```

2. Or use command line:
```powershell
edge-tts --text "hello world" --write-media test.mp3
.\test.mp3
```

If this works, TTS is fine and the issue is in the pipeline wiring.

## Next Steps After This Run

1. **Run the app**
2. **Capture the full log** from startup to after typing "hello"
3. **Report back:**
   - Did you see "Voice pipeline initialized"?
   - Did you see "Voice pipeline processing input"?
   - Did you see TTS logs?
   - Did you hear audio?
   - If any failures, paste the error messages

This will tell us exactly where the pipeline is breaking!
