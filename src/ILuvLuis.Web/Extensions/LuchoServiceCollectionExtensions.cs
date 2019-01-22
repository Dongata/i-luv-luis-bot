using ILuvLuis.Models.Stores;
using ILuvLuis.Qavant;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LuchoServiceCollectionExtensions
    {
        public static IServiceCollection AddLuchoServices(this IServiceCollection services, string baseUri)
        {
            services.AddHttpClient<IPersonStore, PersonStore>(s => s.BaseAddress = new Uri(baseUri));

            return services;
        }
    }
}
