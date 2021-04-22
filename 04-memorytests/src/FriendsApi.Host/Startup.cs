using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendsApi.Host.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace FriendsApi.Host
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

            services
                .AddDabaseConnection(Configuration.GetConnectionString("db"))
                .AddApplicationServices()

                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FriendsApi", Version = "v1" });

                    c.DescribeAllParametersInCamelCase();

                    //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FriendsApi.Host.xml");
                    //c.IncludeXmlComments(filePath);
                    //var filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FriendsApi.Types.xml");
                    //c.IncludeXmlComments(filePath2);
                })

                .AddControllers(o =>
                {
                    o.Filters.Add(new ProducesAttribute("application/json"));
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Env:" + env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FriendsApi v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
