using ConsoleApp.PostgreSQL;
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

            //services.AddDbContext<DataContext>(p => p.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
<<<<<<< HEAD
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
=======
>>>>>>> 72eb9c66fc68c580e7fbc033ad0e6af8f6501d44
            Console.WriteLine(CurrentEnvironment.EnvironmentName);
            Console.WriteLine(CurrentEnvironment.IsDevelopment());
            Console.WriteLine(CurrentEnvironment.IsProduction());
            Console.WriteLine(Environment.GetEnvironmentVariable("IMAGE_GALLERY_POSTGRESQL_CONNECTION_STRING"));
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql(Configuration["PostgreSqlConnectionString"]));
            } else if (CurrentEnvironment.IsProduction())
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql(Environment.GetEnvironmentVariable("IMAGE_GALLERY_POSTGRESQL_CONNECTION_STRING")));
            } else
            {
                services.AddDbContext<DataContext>(p => p.UseNpgsql(Environment.GetEnvironmentVariable("IMAGE_GALLERY_POSTGRESQL_CONNECTION_STRING")));
            }

            services.AddDbContext<DataContext>(p => p.UseNpgsql(Configuration["PostgreSqlConnectionString"]));

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt => {
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);

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
