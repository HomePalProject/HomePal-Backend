namespace HomePal.Infrastructure.AI.PantryManagement.Options;

public class AgentOptions
{
    public const string SectionName = "AgentOptions";

    public string Provider { get; set; } = "OpenAI"; // "OpenAI", "AzureOpenAI", "Mock"
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ModelId { get; set; } = "gpt-4o";
}
