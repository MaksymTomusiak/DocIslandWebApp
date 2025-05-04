using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Emails;
using Application.Common.Interfaces.Services.Files;
using Application.Common.Interfaces.Services.LLM;
using Domain.Conversations;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Tests.Common;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("events_manager_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
        
        builder.ConfigureTestServices(services =>
        {
            RegisterDatabase(services);

            // Remove and mock IEmailService
            services.RemoveServiceByType(typeof(IEmailService));
            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock
                .Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            services.AddScoped(_ => emailServiceMock.Object);

            // Remove and mock IFileStorageService
            services.RemoveServiceByType(typeof(IFileStorageService));
            var fileStorageMock = new Mock<IFileStorageService>();
            fileStorageMock
                .Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://mocked-storage-uri.com/file.jpg");
            fileStorageMock
                .Setup(x => x.DeleteFileAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://mocked-storage-uri.com/file.jpg");
            services.AddScoped(_ => fileStorageMock.Object);

            // Remove and mock ILlmService
            services.RemoveServiceByType(typeof(ILlmService));
            var llmServiceMock = new Mock<ILlmService>();
            llmServiceMock
                .Setup(x => x.AskQuestionAsync(It.IsAny<ConversationId>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("Mocked LLM response");
            llmServiceMock
                .Setup(x => x.CreateConversation(It.IsAny<ConversationId>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            services.AddScoped(_ => llmServiceMock.Object);
        });

    }

    private void RegisterDatabase(IServiceCollection services)
    {
        services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString());
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.DisposeAsync().AsTask();
    }
}

public static class TestFactoryExtensions
{
    public static void RemoveServiceByType(this IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == serviceType);
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
}