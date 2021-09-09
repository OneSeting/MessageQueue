using HighTransmit.Data.MessageQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HighTransmit.Data
{
    public static class HighConfiguration
    {
        public static IServiceCollection ServiceCollection(this IServiceCollection services)
        {
            services.AddScoped<MseeageProduction>();
            //services.AddScoped<ServiceLocator>();
            
            return services;
        }
        public static IApplicationBuilder ApplicationBuilder(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
