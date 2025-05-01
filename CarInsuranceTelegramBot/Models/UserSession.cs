namespace CarInsuranceTelegramBot.Models
{
    public class UserSession
    {
        // The name of the user
        public string Name { get; set; }

        // The car model and make
        public string Car { get; set; }

        // The city where the user lives
        public string City { get; set; }

        // The current step of the process (default is 0)
        public int Step { get; set; } = 0; // Step of the process

        // The file path to the document (if uploaded)
        public string DocumentFilePath { get; set; }

        // Flag indicating if the user's data has been confirmed
        public bool IsDataConfirmed { get; set; } = false; // User data confirmation

        // Flag indicating if the user has accepted the price
        public bool IsPriceAccepted { get; set; } = false; // Price acceptance

        // Additional fields can be added as necessary for specific steps in the process
    }
}
