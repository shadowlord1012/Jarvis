using Common.Events.Interfaces;
using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jarvis
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddJarvis(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBus.EventBus>();
            //services.AddSingleton<ILoggerService, Logger>();
            //services.AddSingleton<IFileAccessor, FileAccessor>();
            services.AddSingleton<Common.Configuration.IConfigurationService, Common.Configuration.ConfigurationService>();
            return services;
        }
    }
}
