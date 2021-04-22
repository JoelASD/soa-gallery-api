using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SOAImageGalleryAPI.Configuration;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI
{
    public class Startup
    {
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            Console.WriteLine(CurrentEnvironment.EnvironmentName);
            Console.WriteLine(CurrentEnvironment.IsDevelopment());
            Console.WriteLine(CurrentEnvironment.IsProduction());
            Console.WriteLine(Environment.GetEnvironmentVariable("IMAGE_GALLERY_POSTGRESQL_CONNECTION_STRING"));
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql(Configuration["PostgreSqlConnectionString"]));
            } else if (CurrentEnvironment.IsProduction())
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql($"Server={Environment.GetEnvironmentVariable("PG_SERVER")}; port={Environment.GetEnvironmentVariable("PG_PORT")}; user id={Environment.GetEnvironmentVariable("PG_USER")}; password={Environment.GetEnvironmentVariable("PG_PASSWORD")}; database={Environment.GetEnvironmentVariable("PG_DB")};"));
            }
            else
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql($"Server={Environment.GetEnvironmentVariable("PG_SERVER")}; port={Environment.GetEnvironmentVariable("PG_PORT")}; user id={Environment.GetEnvironmentVariable("PG_USER")}; password={Environment.GetEnvironmentVariable("PG_PASSWORD")}; database={Environment.GetEnvironmentVariable("PG_DB")};"));
            }

            services.AddDbContext<DataContext>(p => p.UseNpgsql(Configuration["PostgreSqlConnectionString"]));

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie()
            .AddGoogle(options =>
            {
                if(CurrentEnvironment.IsDevelopment())
                {
                    IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                } else if (CurrentEnvironment.IsProduction())
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENT_ID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENT_SECRET");
                } else
                {
                    options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENT_ID");
                    options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENT_SECRET");
                }
                
                //options.Scope.Add("profile");
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SaveTokens = true;
                options.UserInformationEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given-name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Sid, "sid");
            })
            .AddJwtBearer(jwt => {
                var key = Encoding.ASCII.GetBytes("");
                if (CurrentEnvironment.IsDevelopment())
                {
                    key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                } else if (CurrentEnvironment.IsProduction())
                {
                    key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
                } else
                {
                    key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
                }


                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            });

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<DataContext>();

            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SOAImageGalleryAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SOAImageGalleryAPI v1"));
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SOAImageGalleryAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
