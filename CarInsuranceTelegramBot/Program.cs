using CarInsuranceTelegramBot.Handlers;
using CarInsuranceTelegramBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;
using Telegram.Bot;

// Create a web application builder
var builder = WebApplication.CreateBuilder(args);

// Loading configuration settings
var config = builder.Configuration;

// Registering services
builder.Services.AddControllers();  // Adds services for handling HTTP requests (e.g., Web API)

// Registering the Telegram Bot service as a singleton to be shared across the app
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
var token = config["TelegramBotToken"];  // Fetching the Telegram bot token from configuration
return new TelegramBotClient(token);  // Returning a new TelegramBotClient instance
});

// Registering OpenAI service as a singleton for AI-related operations
builder.Services.AddSingleton<OpenAIClient>(sp =>
{
var apiKey = config["OpenAIKey"];  // Fetching the OpenAI API key from configuration
return new OpenAIClient(new OpenAIAuthentication(apiKey));  // Returning a new OpenAIClient instance with the provided key
});

// Registering Mindee service as a singleton for document processing
builder.Services.AddSingleton<MindeeService>(sp =>
{
var mindeeKey = config["MindeeApiKey"];  // Fetching the Mindee API key from configuration
return new MindeeService(mindeeKey);  // Returning a new MindeeService instance with the provided key
});

// Registering additional services related to the business logic (insurance and bot handling)
builder.Services.AddSingleton<InsuranceService>();  // Service for handling insurance logic
builder.Services.AddSingleton<BotHandler>();  // Service for handling Telegram bot updates and events

// Adding a background service to run the Telegram bot
builder.Services.AddHostedService<BotBackgroundService>();

// Building the web application
var app = builder.Build();

// Defining HTTP routes for controllers (e.g., API endpoints)
app.MapControllers();

// Running the application (starting the web server)
app.Run();

// Background service class that runs the Telegram bot
public class BotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;  // Telegram bot client
    private readonly BotHandler _botHandler;  // Bot handler to manage updates and errors

    // Constructor to inject dependencies
    public BotBackgroundService(ITelegramBotClient botClient, BotHandler botHandler)
    {
        _botClient = botClient;  // Storing the bot client
        _botHandler = botHandler;  // Storing the bot handler
    }

    // Asynchronous method to execute background tasks (e.g., start receiving updates from the bot)
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start receiving updates from the Telegram bot
        _botClient.StartReceiving(
            updateHandler: async (bot, update, token) => await _botHandler.HandleUpdateAsync(update),  // Handling incoming updates
            pollingErrorHandler: async (bot, exception, token) => await _botHandler.HandleErrorAsync(exception),  // Handling errors in polling
            cancellationToken: stoppingToken  // Passing cancellation token to support graceful shutdown
        );
    }
}
