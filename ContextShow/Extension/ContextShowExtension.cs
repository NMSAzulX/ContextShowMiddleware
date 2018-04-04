using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContextShow
{
    public static class ContextShowExtension
    {
        public static AllContextShowOptions Options;
        public static IApplicationBuilder UseContextShow(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContextShowMiddleware>();
        }

        public static IServiceCollection RegisterContextShow(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            services.AddSingleton(((provider) =>
            {
                try
                {
                    if (Options==null)
                    {
                        Options = new AllContextShowOptions();
                    }
                    if (Options.Request==null)
                    {
                        Options.Request = new ContextShowRequestOption();
                    }
                    if (Options.Response == null)
                    {
                        Options.Response = new ContextShowResponseOption();
                    }
                    return new ContextShowMiddleware(Options);
                }
                finally
                {
                    Options = null;
                }
            }));
            return services;
        }

        public static IServiceCollection AddRequestShow(this IServiceCollection services, Action<ContextShowRequestOption> optionsAction=null)
        {
            if (Options==null)
            {
                Options = new AllContextShowOptions();
            }
            ContextShowRequestOption request = new ContextShowRequestOption();
            optionsAction?.Invoke(request);
            Options.Request = request;
            return services;
        }
        public static IServiceCollection AddResponseShow(this IServiceCollection services, Action<ContextShowResponseOption> optionsAction=null)
        {
            if (Options == null)
            {
                Options = new AllContextShowOptions();
            }
            ContextShowResponseOption response = new ContextShowResponseOption();
            optionsAction?.Invoke(response);
            Options.Response = response;
            return services;
        }
        public static IServiceCollection AddContextShow(this IServiceCollection services, Action<ContextShowOption> optionsAction=null)
        {
            if (Options == null)
            {
                Options = new AllContextShowOptions();
            }
            ContextShowOption main = new ContextShowOption();
            optionsAction?.Invoke(main);
            Options.Main = main;
            return services;
        }
    }

}
