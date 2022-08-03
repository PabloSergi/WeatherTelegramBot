using WeatherTelegramBot.BotHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace WeatherTelegramBot
{
    class Program
    {
        private static string TlgToken { get; set; } = "5552556277:AAGrYAKCRa9AEcwvGc-EX0C0QDbMiz_zOW8";
        private static TelegramBotClient? bot;

        static async Task Main(string[] args)
        {
            bot = new TelegramBotClient(TlgToken);

            Console.WriteLine($"Запущен бот "+bot.GetMeAsync().Result.FirstName);
            

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = { },
            };


            bot.StartReceiving(
                Handlers.HandleUpdateAsync,
                Handlers.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );


            await PrintAsync();   // вызов асинхронного метода
            Console.WriteLine("Некоторые действия в методе Main");

            void Print()
            {
                Thread.Sleep(30000000);     // имитация продолжительной работы
                Console.WriteLine("Hello METANIT.COM");
            }

            // определение асинхронного метода
            async Task PrintAsync()
            {
                Console.WriteLine("Начало метода PrintAsync"); // выполняется синхронно
                await Task.Run(() => Print());                // выполняется асинхронно
                Console.WriteLine("Конец метода PrintAsync");
            }

            Console.ReadLine();

        }

    }
}

