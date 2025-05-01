namespace CarInsuranceTelegramBot.Models
{
    // Enum representing the role of a participant in the conversation
    public enum Role
    {
        System, // Represents the system (bot)
        User    // Represents the user (person interacting with the bot)
    }

    public class Message
    {
        // The role of the participant (System or User)
        public Role Role { get; set; }

        // The content of the message (text)
        public string Content { get; set; }

        // Constructor to initialize the role and content of the message
        public Message(Role role, string content)
        {
            Role = role;       // Sets the Role of the message (either System or User)
            Content = content; // Sets the content of the message (the text)
        }
    }
}
