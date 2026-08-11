using Jarvis.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic tool to verify VoicePipelineBridge registration
    /// </summary>
    public static class BridgeDiagnostic
    {
        public static void TestRegistration(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILogService>();

            logger.LogInfo("BridgeDiagnostic", "=== BRIDGE DIAGNOSTIC START ===");

            try
            {
                logger.LogInfo("BridgeDiagnostic", "Testing if VoicePipelineBridge is registered...");
                var bridge = host.Services.GetService<VoicePipelineBridge>();

                if (bridge == null)
                {
                    logger.LogError("BridgeDiagnostic", "❌ FAILED: VoicePipelineBridge is NOT registered in DI container!");
                }
                else
                {
                    logger.LogSuccess("BridgeDiagnostic", "✅ SUCCESS: VoicePipelineBridge instance obtained from DI");
                    logger.LogInfo("BridgeDiagnostic", $"   Instance type: {bridge.GetType().FullName}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("BridgeDiagnostic", "❌ ERROR resolving VoicePipelineBridge:");
                logger.LogError("BridgeDiagnostic", $"   Message: {ex.Message}");
                logger.LogError("BridgeDiagnostic", $"   Type: {ex.GetType().Name}");

                if (ex.InnerException != null)
                {
                    logger.LogError("BridgeDiagnostic", $"   Inner: {ex.InnerException.Message}");
                }
            }

            // Test dependencies
            logger.LogInfo("BridgeDiagnostic", "Testing VoicePipelineBridge dependencies:");

            try
            {
                var controller = host.Services.GetService<UI.Controls.HUD.Models.AIInputController>();
                if (controller == null)
                {
                    logger.LogError("BridgeDiagnostic", "❌ AIInputController: NOT FOUND");
                }
                else
                {
                    logger.LogSuccess("BridgeDiagnostic", "✅ AIInputController: OK");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("BridgeDiagnostic", $"❌ AIInputController: ERROR - {ex.Message}");
            }

            try
            {
                var pipeline = host.Services.GetService<Interfaces.IVoicePipelineService>();
                if (pipeline == null)
                {
                    logger.LogError("BridgeDiagnostic", "❌ IVoicePipelineService: NOT FOUND");
                }
                else
                {
                    logger.LogSuccess("BridgeDiagnostic", "✅ IVoicePipelineService: OK");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("BridgeDiagnostic", $"❌ IVoicePipelineService: ERROR - {ex.Message}");
            }

            logger.LogInfo("BridgeDiagnostic", "=== BRIDGE DIAGNOSTIC END ===");
        }
    }
}
