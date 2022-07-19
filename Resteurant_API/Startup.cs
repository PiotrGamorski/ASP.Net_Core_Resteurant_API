using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Resteurant_API.Authentication;
using Resteurant_API.Authorization;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Interfaces;
using Resteurant_API.Middleware;
using Resteurant_API.Services;
using Resteurant_API.Services.ContextServices;
using Resteurant_API.Validators;
using System.Text;

namespace Resteurant_API
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
            var authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection("Authentication").Bind(authenticationSettings);
            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", configurePolicy => 
                    configurePolicy.RequireClaim("Nationality", "Polish"));
                options.AddPolicy("AtLeast18", configurePolicy =>
                    configurePolicy.AddRequirements(new MinimumAgeRequirement(18)));
                options.AddPolicy("CreatedAtLest2Resteurants", configurePolicy =>
                    configurePolicy.AddRequirements(new MinimumResteurantsCount(2)));
            });
            services.AddControllers().AddFluentValidation();
            services.AddDbContext<ResteurantDbContext>();
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, MinimumResteurantsCountHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddScoped<ISeeder, EntitiesSeeder>();
            services.AddScoped<IResteurantService, ResteurantService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<CreateDishDto>, CreateDishDtoValidator>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddHttpClient();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeeder seeder)
        {
            seeder.Seed();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resteurant_API");
            });

            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
