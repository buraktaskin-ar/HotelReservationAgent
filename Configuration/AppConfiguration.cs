namespace HotelReservationAgentChatBot.Configuration;

public class AppConfiguration
{
    public string SearchEndpoint { get; set; }
    public string SearchKey { get; set; }
    public string AzureOpenAIEndpoint { get; set; }
    public string AzureOpenAIKey { get; set; }
    public string IndexName { get; set; } = "hotels-hybrid-index";
    public string EmbeddingDeploymentName { get; set; } = "text-embedding-ada-002";
    public string ChatDeploymentName { get; set; } = "gpt-4o-mini";

    public static AppConfiguration Load()
    {
        return new AppConfiguration
        {
            SearchEndpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_ENDPOINT")
                ?? throw new InvalidOperationException("AZURE_SEARCH_ENDPOINT is not set"),
            SearchKey = Environment.GetEnvironmentVariable("AZURE_SEARCH_ADMIN_KEY")
                ?? throw new InvalidOperationException("AZURE_SEARCH_ADMIN_KEY is not set"),
            AzureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")
                ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set"),
            AzureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
                ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY is not set")
        };
    }
}