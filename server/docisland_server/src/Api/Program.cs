using Api.Authorization;
using Api.Middleware;
using Api.Modules;
using Application;
using Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>(optional: true) // Local dev only
    .AddEnvironmentVariables();

// Configure application services
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.SetupServices();
builder.Services.AddMemoryCache();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin",
        options => options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Clerk";
    options.DefaultChallengeScheme = "Clerk";
})
.AddJwtBearer("Clerk", options =>
{
    options.Authority = $"https://{builder.Configuration["Clerk:Issuer"]}";
    options.Audience = builder.Configuration["Clerk:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

// Add authorization services
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.Requirements.Add(new AdminRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");

app.UseAuthentication();
app.UseAuthorization();

// Add Clerk user sync middleware
app.UseClerkUserSync();

await app.InitializeDb();
app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;