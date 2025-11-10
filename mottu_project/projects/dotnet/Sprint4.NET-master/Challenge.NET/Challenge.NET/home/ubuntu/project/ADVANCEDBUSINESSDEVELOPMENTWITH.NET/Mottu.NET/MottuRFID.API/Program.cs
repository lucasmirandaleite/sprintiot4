using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MottuRFID.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Reflection;
using System.IO;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Asp.Versioning.ApiExplorer;
using MottuRFID.API.Auth;
using MottuRFID.Application.Services; // Importar o novo serviço

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adicionar o serviço de status de moto como Singleton
builder.Services.AddSingleton<MotoStatusService>();

// Configurar o DbContext com Oracle
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-api-version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configurar autenticação com API Key
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyPolicy", policy =>
        policy.Requirements.Add(new ApiKeyRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

// Configurar o Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mottu RFID API",
        Version = "v1",
        Description = "API para rastreamento de motos via RFID - Versão 1.0",
        Contact = new OpenApiContact
        {
            Name = "Equipe Mottu",
            Email = "contato@mottu.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Mottu RFID API",
        Version = "v2",
        Description = "API para rastreamento de motos via RFID - Versão 2.0 (Futura)",
        Contact = new OpenApiContact
        {
            Name = "Equipe Mottu",
            Email = "contato@mottu.com"
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar autenticação API Key no Swagger
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key necessária para acessar os endpoints. Adicione 'X-API-Key' no header.",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "ApiKey",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mottu RFID API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Mottu RFID API v2");
    c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
