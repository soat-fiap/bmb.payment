using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
using Bmb.Payment.Api.Auth;
using Bmb.Payment.DI;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Bmb.Payment.Api;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ILogger<Program>? logger = null;
        try
        {
            builder.Host.UseSerilog((context, configuration) =>
                configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

            // Add services to the container.
            builder.Services.ConfigureJwt(builder.Configuration);
            builder.Services.AddAuthorization();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            // Add CORS services to the DI container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BMB Payment API", Version = "v1", Extensions =
                    {
                        {
                            "x-logo",
                            new OpenApiObject
                            {
                                {
                                    "url",
                                    new OpenApiString(
                                        "https://avatars.githubusercontent.com/u/165858718?s=384")
                                },
                                {
                                    "background",
                                    new OpenApiString(
                                        "#FF0000")
                                }
                            }
                        }
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var jwtOptions = builder.Configuration
                .GetSection("JwtOptions")
                .Get<JwtOptions>();
            builder.Services.AddSingleton(jwtOptions);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.IoCSetup(builder.Configuration);
            builder.Services.AddExceptionHandler<DomainExceptionHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            AddHealthChecks(builder, builder.Configuration);

            var app = builder.Build();
            logger = app.Services.GetService<ILogger<Program>>();
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            logger?.LogCritical(ex, "Application start-up failed");
            Console.WriteLine(ex);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void AddHealthChecks(WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddHealthChecks()
            .AddMySql(configuration.GetConnectionString("MySql")!);
    }
}
