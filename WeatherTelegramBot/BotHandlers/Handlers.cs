using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherTelegramBot.WeatherInfo;
using Telegram.Bot.Types.ReplyMarkups;
using NpgsqlDb;


namespace WeatherTelegramBot.BotHandlers
{
    public class Handlers
    {
        static List<User> usersBase = new List<User>();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Функция для удаления участка сообщений.
            static async void Delete (User user, int startElement, int leftElements, ITelegramBotClient botClient)
            {
                while (user.messageStorage.Count > leftElements)
                {
                    try
                    {
                        await botClient.DeleteMessageAsync(chatId: user.ChatId, messageId: user.messageStorage[startElement]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Какой-то пользователь удалил чат и решил заного написать /start");
                    }
                    user.messageStorage.RemoveAt(startElement);

                }
            } 

            if (update.Type != UpdateType.Message && update.Type != UpdateType.CallbackQuery)
            {
                return;
            }
            if (update.Type == UpdateType.Message && update.Message!.Type != MessageType.Text)
            {
                var message = update.Message;
                try
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: message.MessageId);
                }
                catch (Exception ex) { Console.WriteLine("Проблема с обработкой эмодзи"); }
                return;
            }

            long userId = 0;
            long chatId = 0;
            User newUser;

            if (update.Type == UpdateType.Message)
            {
                userId = update.Message.From.Id;
                chatId = update.Message.Chat.Id;
            }   
            else // if (update.Type == UpdateType.CallbackQuery)
            {
                userId = update.CallbackQuery.From.Id;
                chatId = update.CallbackQuery.Message.Chat.Id;
            }

            if (usersBase.Any(user => user.UserId == userId))
            {
                newUser = usersBase.Find(user => user.UserId == userId);
            }
            else
            {
                newUser = new User(userId);
                newUser.ChatId = chatId;
                usersBase.Add(newUser);
            }

            Localization text = new Localization(newUser.Lang);
                        
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                newUser.messageStorage.Add(update.Message.MessageId);

                if (message.Text.ToLower() == "/start")
                {
                    newUser.ChatOpened = false;

                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    replyMarkup: MenuLanguage(newUser),
                    text: "___");

                    newUser.messageStorage.Add(update.Message.MessageId + 1);

                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: newUser.text.languageDiscription);

                    newUser.messageStorage.Add(update.Message.MessageId + 2);

                    Delete(newUser, 0, 2, botClient);

                }
                else if (DBCheck.CheckCity(message.Text) && newUser.ChatOpened)
                {
                
                    WeatherResponse weatherResponse = WeatherCheck.Deserialize(message.Text, newUser.Lang);

                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text:
                    $"{newUser.text.temperatureAnswer} {weatherResponse.Name}: {weatherResponse.Main.Temp} °C\n" +
                    $"{newUser.text.humidAnswer}: {weatherResponse.Main.Humidity}%\n" +
                    $"{newUser.text.windSpdAnswer}: {weatherResponse.Wind.Speed} {newUser.text.spdType}\n");
                    newUser.messageStorage.Add(update.Message.MessageId + 1);

                    await botClient.SendPhotoAsync(
                    chatId: message.Chat,
                    photo: $"https://raw.githubusercontent.com/PabloSergi/NewGit/main/WeatherApp/icons/{weatherResponse.Weather[0].Icon}.png");
                    newUser.messageStorage.Add(update.Message.MessageId + 2);

                    Delete(newUser, 2, 4, botClient);
                }
                else if (!DBCheck.CheckCity(message.Text) && newUser.ChatOpened)
                {
                    await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: message.MessageId);

                    await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text:$"\"{message.Text}\"? {newUser.text.idontknow}\n");
                    newUser.messageStorage.Add(update.Message.MessageId + 1);

                    Delete(newUser, 2, 4, botClient);
                }
                else
                {
                    try
                    {
                        await botClient.DeleteMessageAsync(chatId: message.Chat, messageId: message.MessageId);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Искать баг при удалении любых сообщений кроме городов и старт");
                    }

                } //Удаление всех случайных сообщений.
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                InlineKeyboardMarkup inlineKeyboardMarkup;
                string codeOfButton = update.CallbackQuery.Data;
                string str;
                //Кнопки.
                switch (codeOfButton)
                {
                    case "ru":
                        newUser.ChatOpened = false;
                       // newUser.Lang = "ru";
                        newUser.text.ChangeLanguage(newUser.Lang = "ru");
                        str = newUser.text.menuDiscription;
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuMain(newUser);
                        break;
                    case "eng":
                        newUser.ChatOpened = false;
                        //newUser.Lang = "eng";
                        newUser.text.ChangeLanguage(newUser.Lang = "eng");
                        str = newUser.text.menuDiscription;
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuMain(newUser);
                        break;
                    case "menu":
                        newUser.ChatOpened = false;
                        str = newUser.text.menuDiscription;
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuMain(newUser);
                        break;
                    case "language":
                        newUser.ChatOpened = false;
                        str = newUser.text.languageDiscription;
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuLanguage(newUser);
                        break;
                    case "weather":
                        newUser.ChatOpened = true;
                        str = newUser.text.weatherDiscription;
                        inlineKeyboardMarkup = (InlineKeyboardMarkup?)MenuButton(newUser);
                        break;
                    default:
                        throw new NotImplementedException("Unrecognized value.");

                }

                //     await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                //         update.CallbackQuery.Message.MessageId, str, replyMarkup: inlineKeyboardMarkup);

                try
                {
                    await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                                        update.CallbackQuery.Message.MessageId, replyMarkup: inlineKeyboardMarkup);
                    await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId + 1, str);
                    if (!newUser.messageStorage.Contains(update.CallbackQuery.Message.MessageId))
                        newUser.messageStorage.Add(update.CallbackQuery.Message.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка редактирования");
                } 
            } 
        }

        static public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        private static IReplyMarkup? MenuLanguage(User user)
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(text: user.text.russian, callbackData: "ru"),
                        InlineKeyboardButton.WithCallbackData(text: user.text.english, callbackData: "eng"),
                    }
                }
            );
        }

        private static IReplyMarkup? MenuButton(User user)
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(text: user.text.menu, callbackData: "menu"),
                    }
                }
            );
        }

        private static IReplyMarkup? MenuMain(User user)
        {
            return new InlineKeyboardMarkup
            (
                inlineKeyboard: new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                         InlineKeyboardButton.WithCallbackData(text: user.text.language, callbackData: "language"),
                         InlineKeyboardButton.WithCallbackData(text: user.text.weather, callbackData: "weather"),
                    }
                }
            ); 
        } 
    } 
}
