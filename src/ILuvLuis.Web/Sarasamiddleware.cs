using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ILuvLuis.Web
{
    public class Sarasamiddleware
    {
        private readonly RequestDelegate _next;

        public Sarasamiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("this");

            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                var inner = e;

                while (inner != null)
                {
                    logger.LogCritical(inner.Message, inner.StackTrace);

                    inner = e.InnerException;
                }
            }
        }
    }
}
