using System.Text;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomePal.Api.Extensions.OpenTelemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddAIOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var agentOptions = configuration.GetSection(AgentOptions.SectionName).Get<AgentOptions>() ?? new AgentOptions();
        if (!agentOptions.Langfuse.Enabled)
        {
            return services;
        }

        var langfuse = agentOptions.Langfuse;
        var authBytes = Encoding.UTF8.GetBytes($"{langfuse.PublicKey}:{langfuse.SecretKey}");
        var base64Auth = Convert.ToBase64String(authBytes);

        var rawEndpoint = string.IsNullOrWhiteSpace(langfuse.Endpoint)
            ? "https://cloud.langfuse.com"
            : langfuse.Endpoint.TrimEnd('/');

        var endpoint = rawEndpoint.EndsWith("/api/public/otel/v1/traces", StringComparison.OrdinalIgnoreCase)
            ? rawEndpoint
            : $"{rawEndpoint}/api/public/otel/v1/traces";

        var serviceName = string.IsNullOrWhiteSpace(langfuse.ServiceName)
            ? "HomePal.AI"
            : langfuse.ServiceName;

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddSource("Experimental.Microsoft.Extensions.AI")
                    .AddSource("Microsoft.Extensions.AI")
                    .AddSource("Microsoft.Agents.AI")
                    .AddSource("*")
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(endpoint);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        options.Headers = $"Authorization=Basic {base64Auth},x-langfuse-ingestion-version=4";
                    });
            });

        return services;
    }
}
