using ECommerceAPI.Application.Services;
using ECommerceAPI.Infastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Infastructure
{
    public static class ServiceRagistration
    {
        public static void AddInfastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
        }
    }
}
