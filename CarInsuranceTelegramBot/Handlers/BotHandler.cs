using CarInsuranceTelegramBot.Models;
using CarInsuranceTelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;
using System.Globalization;

namespace CarInsuranceTelegramBot.Handlers
{
    public class BotHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InsuranceService _insuranceService;
        private readonly MindeeService _mindeeService;
        private readonly Dictionary<long, UserSession> _userSessions = new();

        // Constructor initializes the Telegram client and necessary services
        public BotHandler(ITelegramBotClient botClient, InsuranceService insuranceService, MindeeService mindeeService)
        {
            _botClient = botClient;
            _insuranceService = insuranceService;
            _mindeeService = mindeeService;
        }

        // Handle general bot errors
        public async Task HandleErrorAsync(Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }

        // Validate that the name consists of letters, spaces, or hyphens and has an acceptable length
        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();
            return name.Length >= 2 && name.Length <= 50 &&
                   name.All(c => char.IsLetter(c) || c == ' ' || c == '-');
        }

        // Validate that the city name is correct (letters, spaces, or hyphens, acceptable length)
        private bool IsValidCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return false;

            city = city.Trim();
            return city.Length >= 2 && city.Length <= 50 &&
                   city.All(c => char.IsLetter(c) || c == ' ' || c == '-');
        }

        // Capitalize the first letter of each word (e.g., john doe -> John Doe)
        private string CapitalizeEachWord(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        // Main method to handle incoming updates (messages) from the user
        public async Task HandleUpdateAsync(Update update)
        {
            if (update.Message == null)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text?.Trim();

            // Create a new session if one does not already exist for this user
            if (!_userSessions.ContainsKey(chatId))
            {
                _userSessions[chatId] = new UserSession();
                Console.WriteLine($"New session created for {chatId}");
            }

            var session = _userSessions[chatId];

            // Start the onboarding process when user sends /start
            if (messageText == "/start")
            {
                session.Step = 1;
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "Hello!\n\n" +
                    "I am your assistant for car insurance registration.\n" +
                    "I will help you quickly create an insurance policy by processing your documents.\n\n" +
                    "Let's get started!"
                );
                await _botClient.SendTextMessageAsync(chatId, "What is your full name?");
                return;
            }

            // Handling user interaction based on the current step
            switch (session.Step)
            {
                case 1:
                    // Step 1: Get and validate the user's name
                    if (IsValidName(messageText))
                    {
                        session.Name = CapitalizeEachWord(messageText);
                        session.Step = 2;
                        await _botClient.SendTextMessageAsync(chatId, "Great! Now enter your car's make and model:");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chatId, "Please enter a valid name (2-50 letters, only letters, spaces, or hyphens).");
                    }
                    break;

                case 2:
                    // Step 2: Save car information
                    session.Car = messageText;
                    session.Step = 3;
                    await _botClient.SendTextMessageAsync(chatId, "Which city do you live in?");
                    break;

                case 3:
                    // Step 3: Get and validate the user's city
                    if (IsValidCity(messageText))
                    {
                        session.City = CapitalizeEachWord(messageText);
                        session.Step = 4;
                        await _botClient.SendTextMessageAsync(chatId, "Thank you! Please upload a photo of your car registration, driver's license, or a PDF document.");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chatId, "Please enter a valid city name (2-50 letters, only letters, spaces, or hyphens).");
                    }
                    break;

                case 4:
                    // Step 4: Process uploaded photo or document
                    if (update.Message.Photo != null)
                    {
                        var fileId = update.Message.Photo.Last().FileId;
                        var telegramFile = await _botClient.GetFileAsync(fileId);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.jpg");

                        // Save photo locally
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await _botClient.DownloadFileAsync(telegramFile.FilePath, fileStream);
                        }
                        Console.WriteLine($"Received photo for {chatId}: {filePath}");

                        // Send photo for OCR processing
                        var result = await _mindeeService.ProcessPhotoAsync(filePath);
                        Console.WriteLine($"Photo processing result: {result}");

                        System.IO.File.Delete(filePath);

                        await _botClient.SendTextMessageAsync(chatId, "Document processed successfully. Shall we continue? (yes/no)");
                        session.Step = 5;
                    }
                    else if (update.Message.Document != null)
                    {
                        var fileId = update.Message.Document.FileId;
                        var file = await _botClient.GetFileAsync(fileId);
                        var filePath = file.FilePath;

                        await _botClient.SendTextMessageAsync(chatId, "Document received, processing...");

                        // Send document for OCR processing
                        var result = await _mindeeService.ProcessDocumentAsync(filePath);
                        Console.WriteLine($"Document processing result: {result}");

                        await _botClient.SendTextMessageAsync(chatId, "Document processed successfully. Shall we continue? (yes/no)");
                        session.Step = 5;
                    }
                    else if (update.Message.Text != null)
                    {
                        // In case user sends only text instead of a document
                        await _botClient.SendTextMessageAsync(chatId, "Please upload a photo or a PDF document.");
                    }
                    break;

                case 5:
                    // Step 5: Confirm if the user wants to proceed
                    if (messageText?.ToLower() == "yes")
                    {
                        session.IsDataConfirmed = true;
                        session.Step = 6;
                        await _botClient.SendTextMessageAsync(chatId, "The insurance price is 100 USD. Do you want to proceed? (yes/no)");
                    }
                    else
                    {
                        session.Step = 3;
                        await _botClient.SendTextMessageAsync(chatId, "Please upload another document.");
                    }
                    break;

                case 6:
                    // Step 6: Confirm price acceptance and proceed with insurance policy generation
                    if (messageText?.ToLower() == "yes")
                    {
                        session.IsPriceAccepted = true;
                        session.Step = 7;
                        await _botClient.SendTextMessageAsync(chatId, "Generating your insurance policy...");

                        // Create a prompt for the AI to generate the insurance policy
                        var prompt = $"Create a car insurance policy for:\n" +
                                     $"- Name: {session.Name}\n" +
                                     $"- Car: {session.Car}\n" +
                                     $"- City: {session.City}";

                        var policy = await _insuranceService.GenerateInsurancePolicyAsync(prompt);
                        Console.WriteLine($"Generated insurance policy: {policy}");

                        // Send the generated policy to the user
                        await _botClient.SendTextMessageAsync(chatId, "Your insurance policy has been generated:");
                        await _botClient.SendTextMessageAsync(chatId, policy);

                        // End the session after completion
                        _userSessions.Remove(chatId);
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chatId, "The price of 100 USD is fixed, so we cannot proceed.");
                        _userSessions.Remove(chatId);
                    }
                    break;

                default:
                    // If unknown step, suggest starting over
                    await _botClient.SendTextMessageAsync(chatId, "Please press /start to begin the insurance policy creation.");
                    break;
            }
        }
    }
}
