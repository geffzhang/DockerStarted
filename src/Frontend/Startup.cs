using Frontend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Frontend
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            setupDataProtection(services);

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "SampleInstance";
                options.Configuration = Configuration.GetSection("cache").GetValue<string>("redis");
            });

            services.AddSingleton<IPeopleService>(x => new PeopleService(
                Configuration.GetSection("services").GetValue<string>("people"),
                x.GetService<IDistributedCache>()));
        }

        private void setupDataProtection(IServiceCollection services)
        {
            if (!Configuration.GetValue<bool>("isInCluster"))
                return;

            var redis = ConnectionMultiplexer.Connect(Configuration.GetSection("cache").GetValue<string>("redis"));

            services.AddDataProtection()
                .SetApplicationName("frontend-test")
                .PersistKeysToRedis(redis);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
