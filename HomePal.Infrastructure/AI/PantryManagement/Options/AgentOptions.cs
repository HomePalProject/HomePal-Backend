namespace HomePal.Infrastructure.AI.PantryManagement.Options;

public class AgentOptions
{
    public const string SectionName = "AgentOptions";

    public string Provider { get; set; } = "OpenAI"; // "OpenAI", "AzureOpenAI", "Mock"
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ModelId { get; set; } = "gpt-4o";
    public string EmbeddingModelId { get; set; } = "gemini-embedding-001";
    public int Dimensions { get; set; } = 1536;

    public LangfuseOptions Langfuse { get; set; } = new();
}

public class LangfuseOptions
{
    public bool Enabled { get; set; } = false;
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://cloud.langfuse.com/api/public/otel/v1/traces";
    public string ServiceName { get; set; } = "HomePal.AI";
}

