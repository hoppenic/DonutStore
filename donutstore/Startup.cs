using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using donutstore.Models;
using Newtonsoft.Json.Serialization;



namespace donutstore
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

            string DonutStoreConnectionString = Configuration.GetConnectionString("DonutStore");

            services.AddDbContext<DonutStoreDbContext>(opt => opt.UseSqlServer(DonutStoreConnectionString));

            services.AddIdentity<DonutStoreUser, IdentityRole>()
                .AddEntityFrameworkStores<DonutStoreDbContext>()
                .AddDefaultTokenProviders();


            services.AddMvc();

            services.AddTransient((x) => { return new EmailService(Configuration["SendGridKey"]); });
            services.AddTransient((x) => {
                return new Braintree.BraintreeGateway(
                Configuration["BraintreeEnvironment"],
                Configuration["BraintreeMerchantId"],
                Configuration["BraintreePublicKey"],
                Configuration["BraintreePrivateKey"]);

            });




            //smartystreets
            services.AddTransient((x) =>
            {
                SmartyStreets.ClientBuilder builder = new SmartyStreets.ClientBuilder(Configuration["SmartyStreetsAuthId"], Configuration["SmartyStreetsAuthToken"]);
                return builder.BuildUsStreetApiClient();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,DonutStoreDbContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //this instructs app to use cookies for sign in/out
            app.UseStaticFiles();
            app.UseAuthentication();

            //seeding database
            DbInitializer.Initialize(db);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
