using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;

namespace api
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddCors(options =>
            {
               options.AddPolicy(MyAllowSpecificOrigins,
               builder =>
               {
                   builder.WithOrigins(this.Configuration["ALLOWED_ORIGIN"])
                                        .AllowAnyHeader();
               });
            });

            // services.AddAuthentication(options =>
            //     {
            //         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
            //         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            //     })
            //     .AddJwtBearer(options =>
            //     {
            //         options.Authority = this.Configuration["TOKENS_AUTHORITY"];
            //         options.RequireHttpsMetadata = false;
            //         options.Audience = this.Configuration["TOKENS_AUDIENCE"];
            //     });

            services
                .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = this.Configuration["TOKENS_AUTHORITY"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = this.Configuration["TOKENS_AUDIENCE"];
                });
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();
            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Values}/{action=Index}"
                );
            });
        }
    }
}
