using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IdentityServer4;

namespace TokenService
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddTestUsers(Config.Users);

            services.AddAuthentication(options => 
            {
                options.DefaultScheme = "myCookie";
            })
                .AddCookie("myCookie", options =>
                {
                    options.Cookie.Name = "example_cookie";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Account/Login";
                })
                .AddGoogle("Google", options => 
                {
                    options.ClientId = Configuration["SocialLogins:Google:ClientId"];
                    options.ClientSecret = Configuration["SocialLogins:Google:ClientSecret"];
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                })
                .AddFacebook("Facebook", options =>
                {
                    options.ClientId = Configuration["SocialLogins:Facebook:ClientId"];
                    options.ClientSecret = Configuration["SocialLogins:Facebook:ClientSecret"];
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                })
                .AddTwitter("Twitter", options =>
                {
                    options.ConsumerKey = Configuration["SocialLogins:Twitter:ConsumerKey"];
                    options.ConsumerSecret = Configuration["SocialLogins:Twitter:ConsumerSecret"];
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
