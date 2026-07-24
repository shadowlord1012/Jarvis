using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace UI.Controls.HUD.Models
{
    public sealed class SystemStatus : ISystemStatusService
    {
        public double CpuUsage { get; set; }

        public double MemoryLeft { get; set; }

        public double TotalMemoryMB { get; set; }

        public double GpuUsage { get; set; }

        public double FramesPerSecond { get; set; }

        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        public SystemStatus GetStatus()
        {

            this.CpuUsage = cpuCounter.NextValue();
            this.MemoryLeft = ramCounter.NextValue();
            long totalMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            this.TotalMemoryMB = (double)totalMemoryBytes / (1024 * 1024);

            return this;
        }
    }
}
