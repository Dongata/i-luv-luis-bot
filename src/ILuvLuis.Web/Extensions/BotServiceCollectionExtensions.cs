using ILuvLuis.Web.Bots;
using ILuvLuis.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BotServiceCollectionExtensions
    {
        public static IServiceCollection AddMainBot(this IServiceCollection services, IConfiguration cofiguration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Get Bot file configs
            var secretKey = cofiguration.GetSection("Bot:botFileSecret")?.Value;
            var botFilePath = cofiguration.GetSection("Bot:botFilePath")?.Value;

            // Load the configuration
            var botConfig = BotConfiguration.Load(botFilePath, secretKey);

            var botServices = new BotServices(botConfig);

            services.AddSingleton(botServices);
            services.AddSingleton(botConfig);

            IStorage dataStore = new MemoryStorage();
            services.AddSingleton(dataStore);
            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState);

            services.AddSingleton(new BotStateSet(conversationState));

            services.AddBot<MainBot>(o =>
            {
                var service = botConfig.Services.Where(a => a.Type == "endpoint" && a.Name.ToLower().Trim() == env.EnvironmentName.ToLower().Trim())
                    .First();

                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"There's no endpoint with '{env.EnvironmentName}' as environment on the bot configuration");
                }

                o.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                var logger = loggerFactory.CreateLogger<MainBot>();

                o.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught on a turn context");
                    var inner = exception;
                    while (inner != null)
                    {
                        logger.LogError(inner.Message);
                        logger.LogError(inner.StackTrace);
                        inner = inner.InnerException;
                    }

                    await context.SendActivityAsync("Perdón pero parece que mi cerebro está roto, por favor revisá mas tarde");
                };

                o.Middleware.Add(new AutoSaveStateMiddleware(conversationState));
            });

            return services;
        }
    }
}
