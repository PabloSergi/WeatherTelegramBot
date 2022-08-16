using System;
namespace WeatherTelegramBot.BotHandlers
{
    class User
    {
        public User(long userId)
        {
            this.UserId = userId;
            ChatOpened = false;
            FirstRequest = true;
            FirstRequestWeatherId = 0;
            FirstRequestPhotoId = 0;
            StartMsgId = 0;
            CurrentMenuId = 0;
            Lang = "ru";
            messageStorage = new List<int>();
            text = new Localization(Lang);
        }

        public long ChatId { get; set; }
        public bool ChatOpened { get; set; }
        public bool FirstRequest { get; set; }
        public int FirstRequestWeatherId { get; set; }
        public int FirstRequestPhotoId { get; set; }
        public int StartMsgId { get; set; }
        public int CurrentMenuId { get; set; }
        public string Lang { get; set; } 
        public long UserId { get; set; }
        public List<int> messageStorage;
        public Localization text;
    }
}

