using WeatherTelegramBot.BotHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using NpgsqlDb;


namespace WeatherTelegramBot
{
    class Program
    {

        private static string TlgToken { get; set; } = "5552556277:AAGrYAKCRa9AEcwvGc-EX0C0QDbMiz_zOW8";
        private static TelegramBotClient? bot;

        static async Task Main(string[] args)
        {
            bool dmg = DBCheck.CheckCity("Moscow");

            bot = new TelegramBotClient(TlgToken);

            Console.WriteLine($"Запущен бот "+bot.GetMeAsync().Result.FirstName);
            

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = { },
            };
            

            bot.StartReceiving
            (
                Handlers.HandleUpdateAsync,
                Handlers.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            //Console.ReadKey();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}

