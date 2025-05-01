using Mindee;
using Mindee.Input;
using Mindee.Product.Invoice; // Or another product if needed
using System.Threading.Tasks;
using System.IO;

namespace CarInsuranceTelegramBot.Services
{
    public class MindeeService
    {
        private readonly MindeeClient _mindeeClient;

        // Constructor to initialize the Mindee client with an API key
        public MindeeService(string mindeeApiKey)
        {
            _mindeeClient = new MindeeClient(mindeeApiKey);
        }

        // Method to process a document (PDF or other formats)
        public async Task<string> ProcessDocumentAsync(string filePath)
        {
            try
            {
                // Load the file
                var inputSource = new LocalInputSource(filePath);

                // Use ParseAsync to process the document
                var response = await _mindeeClient.ParseAsync<InvoiceV4>(inputSource);

                // Return the result or a default message if no data
                return response?.Document.ToString() ?? "Document not processed.";
            }
            catch (Exception ex)
            {
                // Return an error message if processing fails
                return $"An error occurred while processing the document: {ex.Message}";
            }
        }

        // Method to process a photo (image file)
        public async Task<string> ProcessPhotoAsync(string filePath)
        {
            try
            {
                // Load the photo
                var inputSource = new LocalInputSource(filePath);

                // Use ParseAsync to process the photo
                var response = await _mindeeClient.ParseAsync<InvoiceV4>(inputSource);

                // Return the result or a default message if no data
                return response?.Document.ToString() ?? "Photo not processed.";
            }
            catch (Exception ex)
            {
                // Return an error message if processing fails
                return $"An error occurred while processing the photo: {ex.Message}";
            }
        }
    }
}
