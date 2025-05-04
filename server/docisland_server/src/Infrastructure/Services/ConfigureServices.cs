using Application.Common.Interfaces.Services.Emails;
using Application.Common.Interfaces.Services.Files;
using Application.Common.Interfaces.Services.LLM;
using Application.Common.Interfaces.Services.Views;
using Azure.Storage.Blobs;
using Infrastructure.Services.Emails;
using Infrastructure.Services.Files;
using Infrastructure.Services.Files.FileTextExtractors;
using Infrastructure.Services.LLM;
using Infrastructure.Services.Views;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddNotifications(services);

        AddFilesServices(services, configuration);
        
        AddLlmServices(services);
    }

    private static void AddLlmServices(IServiceCollection services)
    {
        services.AddScoped<HttpClient>();
        services.AddScoped<ILlmService, LlmService>();
    }

    private static void AddFilesServices(IServiceCollection services, IConfiguration configuration)
    {
        // Retrieve Blob Storage configuration
        var blobStorageConfig = configuration.GetSection("Azure:BlobStorage");
        var connectionString = blobStorageConfig["ConnectionString"];

        // Register BlobServiceClient with connection string
        services.AddScoped(_ => new BlobServiceClient(connectionString));
    
        // Register AzureFileStorageService and optionally pass containerName
        services.AddScoped<AzureFileStorageService>();
    
        // Register IFileStorageService
        services.AddScoped<IFileStorageService>(provider => provider.GetRequiredService<AzureFileStorageService>());

        services.AddScoped<IFileTextExtractor, TxtFileTextExtractor>();
        services.AddScoped<IFileTextExtractor, DocxFileTextExtractor>();
        services.AddScoped<IFileTextExtractor, PdfFileTextExtractor>();
    }

    private static void AddNotifications(IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddRazorRuntimeCompilation(); 

        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new InfrastructureViewLocationExpander());
        });

        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        services.AddSingleton<ICompositeViewEngine, CompositeViewEngine>();
        services.AddScoped<IViewRenderer, ViewRenderer>();
        services.AddScoped<IEmailService, EmailService>();
    }
}