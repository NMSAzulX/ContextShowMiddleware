using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContextShow
{
    public static class ContextShowExtension
    {
        public static IApplicationBuilder UseContextShow(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContextShowMiddleware>();
        }

        public static IServiceCollection AddContextShow(this IServiceCollection services,Action<ContextShowRequestOption, ContextShowResponseOption> optionsAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            services.AddSingleton(((provider) =>
            {
                ContextShowRequestOption requestOption = new ContextShowRequestOption();
                ContextShowResponseOption responseOption = new ContextShowResponseOption();
                optionsAction?.Invoke(requestOption, responseOption);
                return new ContextShowMiddleware(requestOption, responseOption);
            }));
            return services;
        }
    }

    
}
