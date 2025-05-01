# Car Insurance Telegram Bot

This is a C#/.NET 8 Telegram bot designed to help users purchase car insurance through an AI-powered, document-driven process. The bot integrates with the [Mindee API](https://mindee.com) for document parsing and [OpenAI](https://platform.openai.com/) for AI-based policy generation. Built using the ASP.NET Web API and deployed via Docker.

## Features

- Document scanning with Mindee API
- AI-based interaction using OpenAI (ChatGPT)
- Telegram bot integration
- Session handling and multi-step user flow
- Multilingual support
- Ready for cloud deployment (e.g., Render)

---

## Technologies Used

- [.NET 8](https://dotnet.microsoft.com/)
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)
- [OpenAI .NET SDK](https://github.com/betalgo/openai)
- [Mindee .NET SDK](https://mindee.com)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- Docker

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- [Telegram Bot Token](https://core.telegram.org/bots#creating-a-new-bot)
- [Mindee API Key](https://mindee.com/)
- [OpenAI API Key](https://platform.openai.com/account/api-keys)

---

### Configuration

Create a file named `appsettings.json` in the root directory:

```json
{
  "TelegramBotToken": "<your-telegram-bot-token>",
  "OpenAIKey": "<your-openai-key>",
  "MindeeApiKey": "<your-mindee-api-key>"
}
```

---

### Run Locally

dotnet restore
dotnet run

The app will start a web server to handle Telegram webhook updates.

### Run with Docker
1. Build the image:

- docker build -t car-insurance-bot .

2. Run the container:

- docker run -d -p 80:80 --name insurance-bot \
-  -e TelegramBotToken=<your-token> \
-  -e OpenAIKey=<your-openai-key> \
-  -e MindeeApiKey=<your-mindee-api-key> \
-  car-insurance-bot
  
### Deployment (e.g., Render)
- Set the environment variables TelegramBotToken, OpenAIKey, and MindeeApiKey in the Render dashboard.

- Specify the Docker build context and Dockerfile path.

- Enable automatic deploy from your GitHub repo.

### Project Structure

CarInsuranceTelegramBot/
- Controllers/           # Web API controller for webhook
- Handlers/              # BotHandler: main bot logic
- Models/                # DTOs and user session
- Services/              # MindeeService, InsuranceService
- appsettings.json       # Secret keys (ignored in .gitignore)
- Dockerfile             # Docker build config
- Program.cs             # App startup configuration
