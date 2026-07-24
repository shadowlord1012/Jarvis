using Jarvis.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic tool to verify VoicePipelineBridge registration
    /// </summary>
    public static class BridgeDiagnostic
    {
        public static void TestRegistration(IHost host)
        {
            Console.WriteLine("=== BRIDGE DIAGNOSTIC START ===");

            try
            {
                Console.WriteLine("Testing if VoicePipelineBridge is registered...");
                var bridge = host.Services.GetService<VoicePipelineBridge>();

                if (bridge == null)
                {
                    Console.WriteLine("❌ FAILED: VoicePipelineBridge is NOT registered in DI container!");
                }
                else
                {
                    Console.WriteLine("✅ SUCCESS: VoicePipelineBridge instance obtained from DI");
                    Console.WriteLine($"   Instance type: {bridge.GetType().FullName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR resolving VoicePipelineBridge:");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Type: {ex.GetType().Name}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
            }

            // Test dependencies
            Console.WriteLine("\nTesting VoicePipelineBridge dependencies:");

            try
            {
                var controller = host.Services.GetService<UI.Controls.HUD.Models.AIInputController>();
                Console.WriteLine(controller == null 
                    ? "❌ AIInputController: NOT FOUND" 
                    : "✅ AIInputController: OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ AIInputController: ERROR - {ex.Message}");
            }

            try
            {
                var pipeline = host.Services.GetService<Interfaces.IVoicePipelineService>();
                Console.WriteLine(pipeline == null 
                    ? "❌ IVoicePipelineService: NOT FOUND" 
                    : "✅ IVoicePipelineService: OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ IVoicePipelineService: ERROR - {ex.Message}");
            }

            Console.WriteLine("=== BRIDGE DIAGNOSTIC END ===\n");
        }
    }
}
