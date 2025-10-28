using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OMFlags.API.Client;
using OMFlags.Domain.Contracts;
using OMFlags.Infrastructure.Services;

namespace OMFlags.API
{
    public class Startup
    {
        private const string CorsPolicyName = "FrontendPolicy"; 

        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // CORS: allow Blazor dev origin(s)
            services.AddCors(opt =>
            {
                opt.AddPolicy(CorsPolicyName, p => p
                    .WithOrigins("https://localhost:7245", "http://localhost:7245")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                // .AllowCredentials() // only if send cookies
                );
            });

            services.AddHttpClient<ApiClientAsync>(client =>
            {
                var baseUrl = Configuration["Backend:BaseUrl"] ?? "https://localhost:7020/";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddTransient<IApiClientAsync>(sp => sp.GetRequiredService<ApiClientAsync>());

            services.AddHttpClient<ICountryService, CountryService>(client =>
            {
                client.BaseAddress = new Uri("https://restcountries.com/v3.1/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(OMFlags.Application.Countries.GetCountriesQuery).Assembly) 
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OMFlags.API", Version = "v1" });
                c.CustomSchemaIds(x => x.FullName!.Substring(x.Namespace!.Length + 1).Replace("+", "_"));
                c.MapType<FileContentResult>(() => new OpenApiSchema { Type = "string", Format = "binary" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OMFlags.API v1"));

            app.UseHttpsRedirection();
            app.UseRouting();

            // Use the policy by NAME (string), not the type
            app.UseCors(CorsPolicyName);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(CorsPolicyName);
            });
        }
    }
}
