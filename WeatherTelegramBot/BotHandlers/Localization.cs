using System;
namespace WeatherTelegramBot.BotHandlers
{
    class Localization
    {
        public Localization(string lang)
        {
            ChangeLanguage(lang);
        }

        public string weather;
        public string weatherDiscription;
        public string menu;
        public string menuDiscription;
        public string language;
        public string languageDiscription = "Выберите язык / Choose language.";
        public string russian = "РУССКИЙ";
        public string english = "ENGLISH";
        public string temperatureAnswer;
        public string humidAnswer;
        public string windSpdAnswer;
        public string spdType;
        public string idontknow;

        public void ChangeLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    weather = "ПОГОДА";
                    weatherDiscription = "Введите название города, в котором хотите узнать погоду. Для возврата в меню нажмите \"НАЗАД В МЕНЮ\".";
                    menu = "НАЗАД В МЕНЮ";
                    menuDiscription = "Выберите \"ПОГОДА\", что бы узнать погоду, или \"ЯЗЫК\" для смены языка интерфейса.";
                    language = "ЯЗЫК";
                    temperatureAnswer = "Температура в городе";
                    humidAnswer = "Влажность";
                    windSpdAnswer = "Скорость ветра";
                    spdType = "м/с";
                    idontknow = "Я не знаю такой город. Попробуйте написать иначе.";
                    break;
                case "eng":
                    weather = "WEATHER";
                    weatherDiscription = "Enter the name of the city, where you want to know the weather.To return to the menu press \"BACK TO MENU\".";
                    menu = "BACK TO MENU";
                    menuDiscription = "Select \"WEATHER\" to check the weather, or \"LANGUAGE\" to change the interface language.";
                    language = "LANGUAGE";
                    temperatureAnswer = "Temepature in";
                    humidAnswer = "Humidity";
                    windSpdAnswer = "Wind speed";
                    spdType = "m/s";
                    idontknow = "I do not know such a city. Try to write differently.";
                    break;
                default:
                    throw new NotImplementedException("Unrecognized value.");
            }
        }
    }
}

