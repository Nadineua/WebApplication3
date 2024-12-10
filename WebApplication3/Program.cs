using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using WebApplication3.Data;
using WebApplication3.Model;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using WebApplication3.Models.WebApplication3.Model;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register services with the DI container (before building the app)
        builder.Services.AddControllersWithViews(); // Register MVC services
        builder.Services.AddRazorPages(); // Register Razor Pages services

        // Key Vault and BlobServiceClient setup
        var keyVaultUri = "https://finalkeyvault.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

        // Retrieve the function key and storage connection string from Key Vault
        KeyVaultSecret functionSecret = secretClient.GetSecret("Http");
        string functionKey = functionSecret.Value;
        string functionUrl = $"https://funcFINAL.azurewebsites.net/api/HttpTrigger1?code={functionKey}";

        KeyVaultSecret storageSecret = secretClient.GetSecret("storage2");
        string storageConnectionString = storageSecret.Value;

        // Retrieve the SQL Database connection string from Key Vault
        var secret = secretClient.GetSecret("sqldatabase2");
        var connectionString = " Data Source=serverfinal.database.windows.net; Initial Catalog = Test; User ID = Nadine; Password = N@dine123; Connect Timeout = 30; Encrypt = True; Trust Server Certificate=False; Application Intent = ReadWrite; Multi Subnet Failover=False";

        // Register BlobServiceClient and AzureFunctionCaller in DI container
        builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString)); // Singleton for BlobServiceClient
        builder.Services.AddScoped<BlobStorageService>(); // Scoped for BlobStorageService
        builder.Services.AddScoped(provider => new AzureFunctionCaller(functionUrl, functionKey)); // Scoped for AzureFunctionCaller
        builder.Services.AddScoped(provider => new AzureFunctionCaller(functionUrl, functionKey)); // Scoped for AzureFunctionCaller

        // Register DbContext for MySQL Server
        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(connectionString));


        // Register FileService if it is required for dependency injection
        builder.Services.AddScoped<FileService>();

        // Build the application after registering all services
        var app = builder.Build();
        app.UseHttpsRedirection();


        // Ensure static files middleware is enabled (if required for your app)
        app.UseStaticFiles();  // Enable static files (optional based on your needs)

        // Configure the HTTP request pipeline
        app.UseRouting(); // Setup routing for controllers and pages
        app.UseAuthorization(); // Enable authorization middleware

        // Map the endpoints
        app.MapControllers();  // This maps the controllers
        app.MapRazorPages();  // This maps Razor Pages routes
                              // Run the application
        app.Run();
    }
}