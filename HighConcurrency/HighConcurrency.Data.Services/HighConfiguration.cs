using HighConcurrency.Data.Core;
using HighConcurrency.Data.Services.Actions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HighConcurrency.Data.Services
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection HighConfiguration(this IServiceCollection service)
        {
            service.AddScoped<DapperRepository>();
            service.AddScoped<OrdersService>();
            service.AddScoped<AllSituationService>();
            return service;
        }

        public static IApplicationBuilder ApplicationBuilder(this IApplicationBuilder app)
        {
            return app;     
        }
    }
}
