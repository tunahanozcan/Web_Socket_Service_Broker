using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using northwindq.Hubs;
using northwindq.Models;
using northwindq.Subscription;
using northwindq.Subscription.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace northwindq
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddDefaultPolicy(policy =>
                                        policy.AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials()
                                        .SetIsOriginAllowed(origin => true)
            ));
            services.AddSignalR();
         
            services.AddSingleton<DatabaseSubscription<Order>>();



            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseCors();
            app.UseDatabaseSubscription<DatabaseSubscription<Order>>("Orders");
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<siparisHub>("/siparishub");
            });
        }
    }
}
