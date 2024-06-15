using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DemoEmployeeDb.Data;
using DemoEmployeeDb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;
using DemoEmployeeDb.Services;

namespace DemoEmployeeDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private string cosmosDbUri;
        private string cosmosDbKey;
        private string cosmosDbDatabaseName;
        private string cosmosDbContainerName;

        public void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext and Identity services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add Controllers and Views
            services.AddControllersWithViews();
            services.AddRazorPages();

            // Register Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoEmployeeDbAPI", Version = "v1" });
            });

            // Cosmos DB Configuration
            cosmosDbUri = Configuration["CosmosDb:Account"];
            cosmosDbKey = Configuration["CosmosDb:Key"];
            cosmosDbDatabaseName = Configuration["CosmosDb:DatabaseName"];
            cosmosDbContainerName = Configuration["CosmosDb:ContainerName"];

            // Register CosmosClient
            services.AddSingleton(s => new CosmosClient(cosmosDbUri, cosmosDbKey));

            // Register IEmployeeService with required parameters
            services.AddScoped<IEmployeeService>(serviceProvider =>
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                return new EmployeeService(
                    cosmosClient,
                    cosmosDbDatabaseName,
                    cosmosDbContainerName
                );
            });
        }

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoEmployeeDbAPI v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Initialize Cosmos DB container
            var cosmosClient = app.ApplicationServices.GetRequiredService<CosmosClient>();
            InitializeCosmosDbAsync(cosmosClient, cosmosDbDatabaseName, cosmosDbContainerName).Wait();
        }

        private static async Task InitializeCosmosDbAsync(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/dtype");
        }
    }
}
