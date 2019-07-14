using Application.Identity.Api.Data;
using Application.Identity.Api.Options;
using Application.Identity.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Application.Identity.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Entity Framework 
            //services.AddDbContext<IdentityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityContext")));
            services.AddDbContext<IdentityContext>(options => options.UseNpgsql(Configuration.GetConnectionString("IdentityContext")));

            // Identity 
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            // JWT
            var appSettingsSection = Configuration.GetSection("Identity");
            services.Configure<IdentitySettings>(appSettingsSection);

            var identitySettings = appSettingsSection.Get<IdentitySettings>();
            var key = Encoding.ASCII.GetBytes(identitySettings.Secret);

            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(b =>
            {
                b.RequireHttpsMetadata = true;
                b.SaveToken = true;
                b.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = identitySettings.Issuer,
                    ValidAudience = identitySettings.Audience
                };
            });

            // Only useful if have more complex required roles     
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
            // });

            // Swagger
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Application Auth Api", Version = "v1" });

                s.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                s.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                {
                        "Bearer", Enumerable.Empty<string>() },
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
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

            var swaggerSettings = new SwaggerSettings();
            Configuration.GetSection("Swagger").Bind(swaggerSettings);

            app.UseSwagger(option => { option.RouteTemplate = swaggerSettings.JsonRoute; });
            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerSettings.UIEndpoint, swaggerSettings.Description));

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
