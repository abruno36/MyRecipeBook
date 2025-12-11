using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Middleware;
using MyRecipeBook.API.Token;
using MyRecipeBook.Application;
using MyRecipeBook.Application.Services;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Infrastructure;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Migrations;
using MyRecipeBook.Infrastructure.Services.ServiceBus;
using MyRecipeBook.Infrastructure.Services.Storage;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("=== Starting API ===");
Console.WriteLine("Environment -> " + builder.Environment.EnvironmentName);

// =====================================
// CONFIG LOAD
// =====================================
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

Console.WriteLine("DB -> " + builder.Configuration["ConnectionStrings:ConnectionSQLServer"]);
Console.WriteLine("JWT Loaded -> " + (builder.Configuration["Settings:Jwt:SigningKey"] != null));

// =====================================
// SERVICES REGISTRATION
// =====================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CultureActionFilter>();
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITokenProvider, HttpContextTokenValue>();
builder.Services.AddScoped<IDeleteUserQueue, FakeDeleteUserQueue>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();

// =====================================
// STORAGE SERVICE (Fake em Dev ou Docker)
// =====================================
var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (builder.Environment.IsDevelopment()
 || builder.Environment.EnvironmentName == "Test"
 || runningInContainer)
{
    builder.Services.AddScoped<IBlobStorageService, FakeBlobStorageService>();
}
else
{
    builder.Services.AddSingleton(_ =>
        new BlobServiceClient(builder.Configuration["Settings:BlobStorage:Azure"]));

    builder.Services.AddScoped<IBlobStorageService, AzureStorageService>();
}

// =====================================
// JWT AUTH
// =====================================
var jwtKey = builder.Configuration["Settings:Jwt:SigningKey"]
             ?? throw new Exception("JWT missing in appsettings.Production.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// =====================================
// SWAGGER
// =====================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyRecipeBook API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu_token_jwt}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// =====================================
// HEALTH
// =====================================
builder.Services.AddHealthChecks().AddDbContextCheck<MyRecipeBookDbContext>();

// =====================================
// BUILD APP
// =====================================
var app = builder.Build();

if (app.Environment.IsDevelopment() || runningInContainer)
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.RoutePrefix = "swagger");
}

app.UseMiddleware<CultureMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// =====================================
// MIGRATIONS (com retry para Docker)
// =====================================
MigrateDatabase(app);

Console.WriteLine("🚀 API RUNNING @ http://localhost:8080/swagger");
await app.RunAsync();

// =====================================
// MIGRATION METHOD
// =====================================
void MigrateDatabase(WebApplication appInstance)
{
    if (builder.Configuration.IsUnitTestEnviroment())
        return;

    var databaseType = builder.Configuration.DatabaseType();
    var connectionString = builder.Configuration.ConnetionString();

    using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    DatabaseMigration.Migrate(databaseType, connectionString, scope.ServiceProvider);
}

// Necessário para testes
public partial class Program { }
