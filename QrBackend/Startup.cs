using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models; // Add this using directive
using QrBackend.Models;
namespace QrBackend
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
            services.AddControllersWithViews();
            services.AddScoped<DataConversion>(); // Use AddSingleton<DataConversion>() if it should be a singleton
            services.AddTransient<DataAccessLayer>();

            // Define CORS policies
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", // Change to AllowAllOrigins
                    builder => builder.AllowAnyOrigin() // Allow any origin
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });


            // Add Swagger services with basic configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            // Enable Swagger middleware
            app.UseSwagger();

            // Enable Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Use MapControllers() to register all API controllers
            });
        }
    }
}
