# Whisper Model Auto-Download

## What Was Added

The system will now **automatically download** the Whisper model if it's missing!

### New Component: WhisperModelDownloader

- **Automatically downloads** Whisper models from Hugging Face
- **Supports all model sizes:** tiny, base, small, medium, large-v1/v2/v3
- **Progress tracking** and logging
- **Smart caching:** Only downloads once, then reuses the model

### How It Works

1. **On first startup** (or when model is missing):
   - WhisperService checks if `Models/ggml-small.bin` exists
   - If not found, WhisperModelDownloader starts downloading
   - Progress is logged every 10MB and every 10%
   - Model is saved to the configured path

2. **On subsequent startups:**
   - Model exists, loads instantly
   - No download needed

## Configuration

In your `appsettings.json`:

```json
"Whisper": {
  "ModelPath": "Models/ggml-small.bin",  // Where to save the model
  "ModelName": "small"                    // Which model to download
}
```

### Available Models

| Model       | Size    | RAM Needed | Speed       | Accuracy |
|-------------|---------|------------|-------------|----------|
| `tiny`      | ~75 MB  | < 4GB      | Very Fast   | Basic    |
| `tiny.en`   | ~75 MB  | < 4GB      | Very Fast   | Basic    |
| `base`      | ~142 MB | 4-8GB      | Fast        | Good     |
| `base.en`   | ~142 MB | 4-8GB      | Fast        | Good     |
| `small`     | ~466 MB | 8-16GB     | Moderate    | Better   |
| `small.en`  | ~466 MB | 8-16GB     | Moderate    | Better   |
| `medium`    | ~1.5 GB | 16-32GB    | Slower      | Great    |
| `medium.en` | ~1.5 GB | 16-32GB    | Slower      | Great    |
| `large-v3`  | ~3 GB   | 32GB+      | Slowest     | Best     |

**Recommendation:** Use `small` for most applications (good balance of speed/accuracy)

### Models with `.en` suffix
- English-only models
- Slightly faster and more accurate for English
- Don't use if you need multi-language support

## First Run Experience

When you run the app for the first time:

### Expected Log Output:

```
[Info] Whisper model not found at Models/ggml-small.bin
[Info] Created directory: Models
[Info] Downloading Whisper model 'small' from https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-small.bin
[Info] Downloading Whisper model: 10%
[Info] Download progress: 10485760 / 104857600 bytes (10.0%)
[Info] Downloading Whisper model: 20%
[Info] Download progress: 20971520 / 104857600 bytes (20.0%)
...
[Info] Downloading Whisper model: 100%
[Info] Successfully downloaded Whisper model to Models/ggml-small.bin (466033152 bytes)
[Info] Whisper initialized using model Models/ggml-small.bin
```

### Download Times (approximate):

- **tiny:** ~10 seconds (75 MB)
- **base:** ~20 seconds (142 MB)
- **small:** ~1-2 minutes (466 MB) ← **Your current config**
- **medium:** ~3-5 minutes (1.5 GB)
- **large-v3:** ~10-15 minutes (3 GB)

*(Times vary based on internet speed)*

## Changing Models

To use a different model, update `appsettings.json`:

```json
"Whisper": {
  "ModelPath": "Models/ggml-base.bin",  // Change path
  "ModelName": "base"                    // Change name
}
```

Delete the old model file if you want to save space:
```powershell
Remove-Item Models\ggml-small.bin
```

## Troubleshooting

### Download Fails

**Symptom:**
```
[Error] Failed to download Whisper model 'small' to Models/ggml-small.bin
```

**Causes:**
1. No internet connection
2. Hugging Face is down
3. Firewall blocking downloads
4. Disk full

**Solution:**
- Check internet connectivity
- Try again later
- Download manually from Hugging Face and place in `Models/` folder

### Manual Download

If auto-download fails, you can download manually:

1. Go to: https://huggingface.co/ggerganov/whisper.cpp/tree/main
2. Download `ggml-small.bin` (or your preferred model)
3. Create `Models` folder in your app directory
4. Place the downloaded file as `Models/ggml-small.bin`

### Wrong Model Name

**Symptom:**
```
[Error] Unknown Whisper model name: invalid. Available models: tiny, base, small, medium, large-v3
```

**Solution:**
Update `appsettings.json` with a valid model name from the list above.

### Out of Memory

If the model is too large for your system:
```
[Error] Failed to initialize Whisper
```

**Solution:**
Use a smaller model:
- 4GB RAM → use `tiny`
- 8GB RAM → use `base`
- 16GB RAM → use `small`

## TTS Configuration Note

**IMPORTANT:** TTS (Text-to-Speech) does NOT require Whisper!

- **Whisper** = Speech-to-Text (STT) - converts audio to text
- **TTS** = Text-to-Speech - converts text to audio

If you're only using **TTS** (like EdgeTTS or ElevenLabs), you can:
1. Set `VoicePipeline.Enabled = false` to skip Whisper entirely
2. Or just ignore Whisper initialization (it won't block TTS)

## Status

✅ Model downloader implemented  
✅ Auto-download on first run  
✅ Progress logging  
✅ Smart caching  
✅ All model sizes supported  

**The model will download automatically on the next run!** 🎉

No manual intervention needed - just start the app and wait for the download to complete.
