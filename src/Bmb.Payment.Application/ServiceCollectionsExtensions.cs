using System.Diagnostics.CodeAnalysis;
using Bmb.Payment.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.Application;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddUseCases(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICreatePaymentUseCase, CreatePaymentUseCase>()
            .AddScoped<IUpdatePaymentStatusUseCase, UpdatePaymentStatusUseCase>();
    }
}