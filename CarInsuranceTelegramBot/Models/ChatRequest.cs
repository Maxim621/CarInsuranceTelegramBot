using System.Collections.Generic;

namespace CarInsuranceTelegramBot.Models
{
    public class ChatRequest
    {
        public List<Message> Messages { get; set; } // Adds a setter for modification
        public string Model { get; set; } // Adds a setter for modification

        // Constructor without parameters
        public ChatRequest()
        {
            Messages = new List<Message>(); // Initializes an empty list of messages
            Model = string.Empty; // Initializes Model as an empty string
        }

        // Constructor for convenience
        public ChatRequest(List<Message> messages, string model)
        {
            Messages = messages; // Sets the Messages property
            Model = model; // Sets the Model property
        }

        // Method to add a message to the list
        public void AddMessage(Message message)
        {
            Messages.Add(message); // Adds the provided message to the list
        }

        // Method to change the model
        public void SetModel(string model)
        {
            Model = model; // Sets the new model
        }
    }
}
