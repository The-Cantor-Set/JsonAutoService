using System.Collections.Generic;
using JsonAutoService.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace JsonAutoServiceExamples
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
            services.AddJsonAutoService(options =>
            {
                options.ConnectionString = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetValue<string>(nameof(options.ConnectionString));
                options.Mode = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetValue<string>(nameof(options.Mode));
                options.ErrorThreshold = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetValue<int>(nameof(options.ErrorThreshold));
                options.DefaultErrorMessages = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetSection(nameof(options.DefaultErrorMessages)).Get<Dictionary<string, string>>();
                options.RequiredHeaders = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetSection(nameof(options.RequiredHeaders)).Get<Dictionary<string, string>>();
                options.IdentityClaims = Configuration.GetSection(nameof(JsonAutoServiceOptions)).GetSection(nameof(options.IdentityClaims)).Get<Dictionary<string, string>>();
            });

            services.AddControllers();

            // Swagger UI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JsonAutoService Example API", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JsonAutoService Example V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
