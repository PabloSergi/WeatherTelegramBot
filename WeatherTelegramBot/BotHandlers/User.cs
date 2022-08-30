using System;
namespace WeatherTelegramBot.BotHandlers
{
    public class User
    {
        public User(long userId)
        {
            this.UserId = userId;
            ChatOpened = false;
            Lang = "ru";
            messageStorage = new List<int>();
           // text = new Localization(Lang);
        }

        public long ChatId { get; set; }
        public bool ChatOpened { get; set; }
        public string Lang { get; set; } 
        public long UserId { get; set; }
        public List<int> messageStorage;
       // public Localization text;
    }
}

