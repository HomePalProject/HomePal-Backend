using System.Globalization;
using System.Text.Json;
using HomePal.Application.Features.PantryManagement.DTOs;
using HomePal.Application.Features.PantryManagement.Interfaces;
using HomePal.Domain.Common;
using HomePal.Domain.Entities;
using HomePal.Infrastructure.AI.PantryManagement.Instructions;
using HomePal.Infrastructure.AI.PantryManagement.Options;
using HomePal.Shared.Results;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

using Microsoft.Extensions.DependencyInjection;

namespace HomePal.Infrastructure.AI.PantryManagement.Services;

public class PantryAgentScanner : IPantryScannerService
{
    private readonly AIAgent _agent;
    private readonly AgentOptions _options;
    private readonly ILogger<PantryAgentScanner> _logger;

    public PantryAgentScanner(
        [FromKeyedServices("PantryScannerAgent")] AIAgent agent,
        IOptions<AgentOptions> options,
        ILogger<PantryAgentScanner> logger)
    {
        _agent = agent;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Result<PantryScanResponse>> ScanPantryImageAsync(
        Stream imageStream,
        string contentType,
        IReadOnlyList<MeasuringUnit> availableUnits,
        IReadOnlyList<ProductCategory> availableCategories,
        CancellationToken cancellationToken = default)
    {
        if (imageStream == null || imageStream.Length == 0)
        {
            return Result<PantryScanResponse>.Fail(ErrorMessages.Auth.InvalidImageFile, ResultStatus.BadRequest);
        }

        // Read image bytes
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();

        try
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var unitsFormatted = string.Join("\n", availableUnits.Select(u => $"- ID: {u.Id}, Name: {u.Name.Get(culture)}"));
            var categoriesFormatted = string.Join("\n", availableCategories.Select(c => $"- ID: {c.Id}, Name: {c.Name.Get(culture)}"));

            var promptText = PantryAgentInstructions.BuildPrompt(unitsFormatted, categoriesFormatted, DateTime.UtcNow, culture);

            var mediaType = string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType;
            var userMessage = new ChatMessage(ChatRole.User, [
                new TextContent(promptText),
                new DataContent(imageBytes, mediaType)
            ]);

            var agentResponse = await _agent.RunAsync<PantryScanRawOutput>(userMessage, cancellationToken: cancellationToken);
            var parsedResult = agentResponse.Result;

            if (parsedResult?.Items != null && parsedResult.Items.Count > 0)
            {
                var responseItems = parsedResult.Items.Select(item => new PantryScanItemDto
                {
                    Name = item.Name,
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    MeasuringUnitId = item.MeasuringUnitId != Guid.Empty ? item.MeasuringUnitId : (availableUnits.FirstOrDefault()?.Id ?? Guid.NewGuid()),
                    MeasuringUnitName = item.MeasuringUnitName ?? availableUnits.FirstOrDefault(u => u.Id == item.MeasuringUnitId)?.Name.Get(culture),
                    CategoryId = item.CategoryId != Guid.Empty ? item.CategoryId : (availableCategories.FirstOrDefault()?.Id ?? Guid.NewGuid()),
                    CategoryName = item.CategoryName ?? availableCategories.FirstOrDefault(c => c.Id == item.CategoryId)?.Name.Get(culture),
                    SuggestedExpireDate = item.SuggestedExpireDate ?? DateTime.UtcNow.AddDays(7)
                }).ToList();

                return Result<PantryScanResponse>.Ok(new PantryScanResponse { Items = responseItems }, SuccessMessages.Pantry.Scan);
            }

            return Result<PantryScanResponse>.Fail(ErrorMessages.Pantry.ScanFailed, ResultStatus.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing Microsoft Agent Framework Pantry Scan.");
            return Result<PantryScanResponse>.Fail(ErrorMessages.Pantry.ScanFailed, ResultStatus.Failure);
        }
    }

    private class PantryScanRawOutput
    {
        public List<PantryScanRawItem>? Items { get; set; }
    }

    private class PantryScanRawItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public Guid MeasuringUnitId { get; set; }
        public string? MeasuringUnitName { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? SuggestedExpireDate { get; set; }
    }
}
