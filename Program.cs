using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using HotelReservationAgentChatBot.Configuration;
using HotelReservationAgentChatBot.Data;
using HotelReservationAgentChatBot.Models;
using HotelReservationAgentChatBot.Plugins;
using HotelReservationAgentChatBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;

WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder(args);

// Load configuration
AppConfiguration config = AppConfiguration.Load();

// Azure Search Services
var indexClient = new SearchIndexClient(
    new Uri(config.SearchEndpoint),
    new AzureKeyCredential(config.SearchKey)
);

var searchClient = new SearchClient(
    new Uri(config.SearchEndpoint),
    config.IndexName,
    new AzureKeyCredential(config.SearchKey)
);

// Get embedding service early for initialization
var embeddingService = new AzureOpenAITextEmbeddingGenerationService(
    config.EmbeddingDeploymentName,
    config.AzureOpenAIEndpoint,
    config.AzureOpenAIKey);

// Initialize search infrastructure automatically on startup
Console.WriteLine("Initializing search index...");
try
{
    var searchInfrastructure = new SeedToIndexDataConverter(indexClient, searchClient, embeddingService);
    await searchInfrastructure.InitializeAsync();
    Console.WriteLine("Search index initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not initialize search index: {ex.Message}");
    Console.WriteLine("Index may already exist. Continuing...");
}

// Initialize services
var searchService = new SearchService(searchClient, embeddingService);

// Build Semantic Kernel
IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: config.ChatDeploymentName,
        endpoint: config.AzureOpenAIEndpoint,
        apiKey: config.AzureOpenAIKey)
    .AddAzureOpenAITextEmbeddingGeneration(
        deploymentName: config.EmbeddingDeploymentName,
        endpoint: config.AzureOpenAIEndpoint,
        apiKey: config.AzureOpenAIKey);

// Create service instances
var reservationService = new ReservationService();
var personService = new PersonService(reservationService);
var hotelService = new HotelService(); // Yeni HotelService eklendi

// Add plugins to kernel
kernelBuilder.Plugins.AddFromObject(new HotelRagSearchPlugin(searchClient, searchService, embeddingService));
kernelBuilder.Plugins.AddFromObject(new HotelServicePlugin(hotelService)); // Yeni HotelServicePlugin eklendi
kernelBuilder.Plugins.AddFromObject(new ReservationPlugin(reservationService));
kernelBuilder.Plugins.AddFromObject(new PersonPlugin(personService));
kernelBuilder.Plugins.AddFromObject(new RoomPlugin(reservationService));
kernelBuilder.Plugins.AddFromObject(new DateRangePlugin(reservationService));

Kernel kernel = kernelBuilder.Build();

// Get required services from kernel
IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Dependency Injection
webAppBuilder.Services.AddSingleton(config);
webAppBuilder.Services.AddSingleton(searchClient);
webAppBuilder.Services.AddSingleton(kernel);
webAppBuilder.Services.AddSingleton(chatCompletionService);
webAppBuilder.Services.AddSingleton(searchService);
webAppBuilder.Services.AddSingleton<IReservationService>(reservationService);
webAppBuilder.Services.AddSingleton<IPersonService>(personService);
webAppBuilder.Services.AddSingleton<IHotelService>(hotelService); // Yeni HotelService DI eklendi

// Session storage
webAppBuilder.Services.AddSingleton<IChatSessionService, ChatSessionService>();

// Add CORS if needed
webAppBuilder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

webAppBuilder.Services.AddControllers();
webAppBuilder.Services.AddEndpointsApiExplorer();
webAppBuilder.Services.AddSwaggerGen();

WebApplication app = webAppBuilder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();