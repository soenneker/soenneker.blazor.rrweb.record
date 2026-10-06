using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;
using Soenneker.Blazor.Rrweb.Record.Abstract;

namespace Soenneker.Blazor.Rrweb.Record.Registrars;

/// <summary>Registers rrweb record interop and its resource dependencies.</summary>
public static class RrwebRecordInteropRegistrar
{
    /// <summary>Adds <see cref="IRrwebRecordInterop"/> as a scoped service.</summary>
    public static IServiceCollection AddRrwebRecordInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped();
        services.TryAddScoped<IRrwebRecordInterop, RrwebRecordInterop>();
        return services;
    }
}
