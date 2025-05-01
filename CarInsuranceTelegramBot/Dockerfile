# Вказуємо базовий образ з .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Встановлюємо робочу директорію
WORKDIR /app

# Копіюємо всі файли проекту в контейнер
COPY . ./

# Встановлюємо залежності та публікуємо додаток
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Вказуємо на запуск програми після публікації
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Вказуємо порт, на якому буде працювати додаток
EXPOSE 80

# Запускаємо додаток
ENTRYPOINT ["dotnet", "CarInsuranceTelegramBot.dll"]
