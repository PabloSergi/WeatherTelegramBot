using WeatherTelegramBot.BotHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace WeatherTelegramBot
{
    class Program
    {
        private static string TlgToken { get; set; } = "5552556277:AAGrYAKCRa9AEcwvGc-EX0C0QDbMiz_zOW8";
        private static TelegramBotClient? bot;

        static void Main(string[] args)
        {
            bot = new TelegramBotClient(TlgToken);

            Console.WriteLine($"Запущен бот "+bot.GetMeAsync().Result.FirstName);

            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                Handlers.HandleUpdateAsync,
                Handlers.HandleErrorAsync,
                receiverOptions,
                cancellationToken : cts.Token
            );

            Console.ReadLine();
            cts.Cancel();
        }
    }
}

