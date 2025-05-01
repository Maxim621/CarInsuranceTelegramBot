using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot;
using CarInsuranceTelegramBot.Handlers;

// The BotController class is responsible for handling incoming HTTP requests from Telegram
// to process updates (messages, commands, etc.) sent to the bot.
namespace CarInsuranceTelegramBot.Controllers
{
    // This attribute marks the class as an API controller and defines the route for requests
    [ApiController]
    [Route("[controller]")]
    public class BotController : ControllerBase
    {
        // Private field for BotHandler, injected via constructor
        private readonly BotHandler _botHandler;

        // Constructor to inject the BotHandler service
        public BotController(BotHandler botHandler)
        {
            _botHandler = botHandler;  // Storing the injected BotHandler instance
        }

        // HTTP POST endpoint that handles incoming updates from Telegram
        // The [FromBody] attribute specifies that the data for the 'update' parameter 
        // should be deserialized from the body of the HTTP request.
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            // If the update is null, return a BadRequest response
            if (update == null) return BadRequest();

            // Pass the update to the BotHandler to process it (e.g., handle messages, commands)
            await _botHandler.HandleUpdateAsync(update);

            // Return an OK response once the update is successfully processed
            return Ok();
        }
    }
}
