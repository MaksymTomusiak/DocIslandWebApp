using Application.Common.Interfaces.Services.Emails;
using Application.Common.Interfaces.Services.Views;
using Infrastructure.Services.Emails;
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

        //ToDo: Add file storage
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