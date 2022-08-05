using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherTelegramBot.WeatherInfo;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace WeatherTelegramBot.BotHandlers
{
    public class Handlers
    {
        public static string? Lang { get; set; }
        public static bool ChatOpened { get; set; } = false;
        public static bool FirstRequest { get; set; } = true;
        public static int FirstRequestWeatherId { get; set; } = 0;
        public static int FirstRequestPhotoId { get; set; } = 0;
        public static int StartMsgId { get; set; } = 0;
        public static int MenuCurrentId { get; set; } = 0;
        //public static bool IsStarted { get; set; } = false;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message && update.Type != UpdateType.CallbackQuery) 
                return;
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;

                if (message.Text.ToLower() == "/start")
                {
                    ChatOpened = false;
                    StartMsgId = message.MessageId;

                    if (MenuCurrentId > 0)
                    {
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: MenuCurrentId);
                        MenuCurrentId = 0;
                    }
                    if (!FirstRequest)
                    {
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestWeatherId);
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestPhotoId);
                    }

                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    replyMarkup: MenuLanguage(),
                    text: "Выберите язык / Choose language");

                    MenuCurrentId = message.MessageId + 1;
                    FirstRequest = true;

                }
                else if (message.Text.ToLower() == "москва" && ChatOpened)
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: message.MessageId);
                    if (!FirstRequest)
                    {
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestWeatherId);
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestPhotoId);
                    }
                    
                    WeatherResponse weatherResponse = WeatherCheck.Deserialize(message.Text);
                    
                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text:
                    $"Температура в городе {weatherResponse.Name}: {weatherResponse.Main.Temp} °C\n" +
                    $"Влажность: {weatherResponse.Main.Humidity}%\n" +
                    $"Скорость ветра: {weatherResponse.Wind.Speed} м/с\n");
                    Thread.Sleep(800);

                    await botClient.SendPhotoAsync(
                    chatId: message.Chat,
                    photo: $"https://raw.githubusercontent.com/PabloSergi/NewGit/main/WeatherApp/icons/{weatherResponse.Weather[0].Icon}.png");

                    FirstRequest = false;
                    FirstRequestWeatherId = message.MessageId + 1;
                    FirstRequestPhotoId = FirstRequestWeatherId + 1;
                }

                else //удаление всех сообщений кроме запроса погоды  и /start.
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: message.MessageId);
                }

                if (StartMsgId > 0) //удаление /start, внутри оператора. if при старте бота начинает сбоить. 
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: StartMsgId);
                    StartMsgId = 0;
                }


            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuMain();
                string codeOfButton = update.CallbackQuery.Data;
                string str;
                switch (codeOfButton)
                {
                    case "ru":
                        ChatOpened = false;
                        Lang = "ru";
                        str = "Выбран русский язык";
                        break;
                    case "eng":
                        ChatOpened = false;
                        Lang = "eng";
                        str = "English language choosed";
                        break;
                    case "menu":
                        ChatOpened = false;
                        str = "Выберите следующее действие";
                        break;
                    case "language":
                        ChatOpened = false;
                        str = "Выберите следующее действие";
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuLanguage();
                        break;
                    case "weather":
                        ChatOpened = true;
                        str = "Выберите следующее действие";
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuButton();
                        break;
                    default:
                        throw new NotImplementedException("Unrecognized value.");
                        break;
                }
                await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, str, replyMarkup: inlineKeyboardMarkup);
                MenuCurrentId = update.CallbackQuery.Message.MessageId;



                /*
                if (codeOfButton == "ru")
                {
                    ChatOpened = false;
                    Lang = "ru";
                    string telegramMessage = "Выбран русский язык";
                    await botClient.EditMessageTextAsync( update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, telegramMessage, replyMarkup: (InlineKeyboardMarkup?)MenuMain());
                    MenuCurrentId = update.CallbackQuery.Message.MessageId;
                }
                if (codeOfButton == "eng")
                {
                    ChatOpened = false;
                    Lang = "eng";
                    string telegramMessage = "English language choosed";
                    await botClient.EditMessageTextAsync( update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, telegramMessage, replyMarkup: (InlineKeyboardMarkup?)MenuMain());
                    MenuCurrentId = update.CallbackQuery.Message.MessageId;
                }
                if (codeOfButton == "menu")
                {
                    ChatOpened = false;
                    string telegramMessage = "Выберите следующее действие";
                    await botClient.EditMessageTextAsync( update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, telegramMessage, replyMarkup: (InlineKeyboardMarkup?)MenuMain());
                    MenuCurrentId = update.CallbackQuery.Message.MessageId;
                }
                if (codeOfButton == "language")
                {
                    ChatOpened = false;
                    string telegramMessage = "Выберите язык / Choose language";
                    await botClient.EditMessageTextAsync( update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, telegramMessage, replyMarkup: (InlineKeyboardMarkup?)MenuLanguage());
                    MenuCurrentId = update.CallbackQuery.Message.MessageId;
                }
                if (codeOfButton == "weather")
                {
                    ChatOpened = true;
                    string telegramMessage = "Назад в меню";
                    await botClient.EditMessageTextAsync( update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, telegramMessage, replyMarkup: (InlineKeyboardMarkup?)MenuButton());
                    MenuCurrentId = update.CallbackQuery.Message.MessageId;
                } */
            
            } 

            /*async void WeatherAnswerDel(bool check, ChatId chatId, int messageId)
            {
                if (!check)
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestWeatherId);
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: FirstRequestPhotoId);
                }
            }*/
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private static IReplyMarkup? MenuLanguage()
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(text: "РУССКИЙ", callbackData: "ru"),
                        InlineKeyboardButton.WithCallbackData(text: "ENGLISH", callbackData: "eng"),
                    }
                }
            );
        }

        private static IReplyMarkup? MenuButton()
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(text: "MENU", callbackData: "menu"),
                    }
                }
            );
        }

        private static IReplyMarkup? MenuMain()
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                         InlineKeyboardButton.WithCallbackData(text: "ЯЗЫК", callbackData: "language"),
                         InlineKeyboardButton.WithCallbackData(text: "ПОГОДА", callbackData: "weather"),
                    }
                }
            );
        }
    }
}
