using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Storage;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Infastructure.Enums;
using ECommerceAPI.Infastructure.Services;
using ECommerceAPI.Infastructure.Services.Storage;
using ECommerceAPI.Infastructure.Services.Storage.Azure;
using ECommerceAPI.Infastructure.Services.Storage.Local;
using ECommerceAPI.Infastructure.Services.Token;
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
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IMailService, MailService>();
        }
        public static void AddStorage<T>(this IServiceCollection services) where T : Storage, IStorage
        {
            services.AddScoped<IStorage, T>();
        }
        public static void AddStorage(this IServiceCollection services, StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    services.AddScoped<IStorage, LocalStorage>();
                    break;
                case StorageType.Azure:
                    services.AddScoped<IStorage, AzureStorage>();
                    break;
                case StorageType.AWS:
                    break;
                default:
                    break;
            }
        }
    }
}
