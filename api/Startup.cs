using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;  

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
            services.AddMvc();

            services.AddCors(options =>
            {
               options.AddPolicy(MyAllowSpecificOrigins,
               builder =>
               {
                   builder.WithOrigins(this.Configuration["ALLOWED_ORIGIN"])
                                        .AllowAnyHeader();
               });
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
                })
                .AddCookie()
                .AddJwtBearer(config =>  
                {
                    // get the public key from config
                    var keyPath = string.IsNullOrWhiteSpace(this.Configuration["TOKENS_PUBLIC_KEY_PATH"]) ? "/certs/public.pem" : this.Configuration["TOKENS_PUBLIC_KEY_PATH"];
                    PemReader pemParser = new PemReader(new StreamReader(keyPath));
                    var certObj = pemParser.ReadPemObject();
                    
                    // setup the signing key to be used to validate the jwt token
                    AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(certObj.Content);
                    RsaKeyParameters rsaKeyParameters = (RsaKeyParameters) asymmetricKeyParameter;
                    RSAParameters rsaParameters = new RSAParameters();
                    rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
                    rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
                    var signingKey = new RsaSecurityKey(rsaParameters);

                    // random config
                    config.RequireHttpsMetadata = false;  
                    config.SaveToken = true;
                    
                    // setup the token validation config
                    config.TokenValidationParameters = new TokenValidationParameters()  
                    {  
                        IssuerSigningKey = signingKey,  
                        ValidateAudience = true,  
                        ValidAudience = this.Configuration["TOKENS_AUDIENCE"],  
                        ValidateIssuer = true,  
                        ValidIssuer = this.Configuration["TOKENS_ISSUER"],  
                        ValidateLifetime = true,  
                        ValidateIssuerSigningKey = true  
                    };  
                    
                    config.Events = new JwtBearerEvents {
                        // perform any custom mappings on valid token
                        OnTokenValidated = async context => {
                            // implement any custom mappings here
                        }
                    };
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
