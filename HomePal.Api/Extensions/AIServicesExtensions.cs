using System.ClientModel;
using HomePal.Application.Common.Interfaces;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Infrastructure.AI.PantryManagement.Instructions;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Infrastructure.AI.PantryManagement.Services;
using HomePal.Infrastructure.AI.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;

namespace HomePal.Api.Extensions;

public static class AIServicesExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AgentOptions>()
            .Bind(configuration.GetSection(AgentOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IChatClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
            var modelId = string.IsNullOrWhiteSpace(options.ModelId) ? "gpt-4o" : options.ModelId;
            var apiKey = string.IsNullOrWhiteSpace(options.ApiKey) ? "mock-key" : options.ApiKey;
            var endpoint = options.Endpoint;

            OpenAIClientOptions? clientOptions = null;
            if (!string.IsNullOrWhiteSpace(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            {
                clientOptions = new OpenAIClientOptions
                {
                    Endpoint = endpointUri
                };
            }

            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
            var openAiChatClient = openAiClient.GetChatClient(modelId);
            return openAiChatClient.AsIChatClient();
        });

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
            var embeddingModelId = options.EmbeddingModelId;
            var apiKey = options.ApiKey;
            var endpoint = options.Endpoint;

            OpenAIClientOptions? clientOptions = null;
            if (!string.IsNullOrWhiteSpace(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            {
                clientOptions = new OpenAIClientOptions
                {
                    Endpoint = endpointUri
                };
            }

            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), clientOptions);
            var embeddingClient = openAiClient.GetEmbeddingClient(embeddingModelId);
            return embeddingClient.AsIEmbeddingGenerator();
        });

        services.AddAIAgent("PantryScannerAgent", instructions: PantryAgentInstructions.SystemInstructions);

        services.AddSingleton<AIAgent>(sp =>
        {
            var chatClient = sp.GetRequiredService<IChatClient>();
            return chatClient.AsAIAgent(instructions: PantryAgentInstructions.SystemInstructions, name: "PantryScannerAgent");
        });

        services.AddScoped<IPantryScannerService, PantryAgentScanner>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();

        return services;
    }
}
