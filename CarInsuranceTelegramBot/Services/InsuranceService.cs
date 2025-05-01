using OpenAI;
using OpenAI.Chat;
using OpenAIRole = OpenAI.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarInsuranceTelegramBot.Services
{
    public class InsuranceService
    {
        private readonly OpenAIClient _openAiClient;
        private readonly bool _useFakeResponse = false; // Used for testing purposes

        // Constructor to initialize OpenAI client
        public InsuranceService(OpenAIClient openAiClient)
        {
            _openAiClient = openAiClient;
        }

        // Method to generate an insurance policy based on extracted data
        public async Task<string> GenerateInsurancePolicyAsync(string extractedData)
        {
            string policyText;

            // If fake response is enabled, use mock data
            if (_useFakeResponse)
            {
                policyText = $"Insurance Policy based on provided data:\n{extractedData}\n\n" +
                             $"Policy Number: POL-{Guid.NewGuid().ToString().Substring(0, 8)}\n" +
                             $"Insurance Company: Reliable Insurance\n" +
                             $"Insurance Cost: 100 USD\n" +
                             $"Start Date: {DateTime.Today:dd.MM.yyyy}\n" +
                             $"End Date: {DateTime.Today.AddYears(1):dd.MM.yyyy}";
            }
            else
            {
                // If not using a fake response, create a request for OpenAI to generate the policy text
                var messages = new List<Message>
                {
                    new Message(OpenAIRole.System, "You are an assistant that generates official car insurance policies based on user-provided data."),
                    new Message(OpenAIRole.User, $"Generate an official insurance policy based on the following data:\n{extractedData}")
                };

                // Create a chat request with the model and the messages
                var chatRequest = new ChatRequest(
                    messages: messages,
                    model: "gpt-3.5-turbo"
                );

                // Send the request to OpenAI's chat endpoint
                var result = await _openAiClient.ChatEndpoint.GetCompletionAsync(chatRequest);
                policyText = result.FirstChoice.Message.Content; // Get the generated policy text
            }

            // Output the generated policy to the console
            Console.WriteLine($"Generated Insurance Policy:\n{policyText}");

            return policyText;
        }
    }
}
