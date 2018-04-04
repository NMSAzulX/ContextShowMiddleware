using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContextShow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddContextShow((option) => {            
                
                option.ShowInConsole = true;
                option.IsMergeInfo = true;
                option.AddEnter(".*");
                option.AddIgnore("/favicon.ico");

                //If you want to use the default setting. You can remove this method.

            }).AddRequestShow((option)=> {

                //If you want to use the default setting. You can remove this method.

            }).AddResponseShow((option) => {

                //If you want to use the default setting. You can remove this method.

            }).RegisterContextShow();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseContextShow();
            app.UseMvc();
        }
    }
}
