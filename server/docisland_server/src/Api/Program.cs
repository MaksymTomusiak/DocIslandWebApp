using Api.Middleware;
using Api.Modules;
using Application;
using Infrastructure;
using Microsoft.IdentityModel.Tokens;

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

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowOrigin");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Add Clerk user sync middleware
app.UseClerkUserSync();

await app.InitializeDb();
app.MapControllers();

app.Run();

public partial class Program;