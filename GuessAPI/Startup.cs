using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GuessAPI.Model;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using GuessAPI.CentralHub;

namespace GuessAPI
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
            services.AddDbContext<GuessContext>(options => options.UseSqlServer(Configuration.GetConnectionString("GuessDatabase")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(
            options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddDbContext<GuessContext>();
            services.AddAutoMapper(typeof(Startup));

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Guess API",
                    Version = "v1",
                    Description = "A web API for making a quiz game",
                    Contact = new Contact
                    {
                        Name = "Sakyawira Nanda Ruslim",
                        Email = "sakyawira@gmail.com",
                        Url = "https://github.com/Sakyawira/GuessSongAPI"
                    },
                });
            });

            //Registering Azure SignalR service
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Make sure the CORS middleware is ahead of SignalR.
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()//("https://sakyafrontend.azurewebsites.net/")
               
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            // SignalR
            app.UseFileServer();
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/hub");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GuessAPI V1");
                c.RoutePrefix = string.Empty; // launch swagger from root
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            
        }
    }
}
