using System.Net;
using Newtonsoft.Json;

namespace WeatherTelegramBot.WeatherInfo
{
    public class WeatherCheck
    {
        public static WeatherResponse Deserialize(string cityName)
        {
            string lang = "ru";
            string units = "metric";
            string appid = "271988e019a63beb1e08c8550b0d872f";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&lang={lang}&units={units}&appid={appid}";

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            string answer = string.Empty;

            using (Stream stream = response.GetResponseStream())
            {
                using StreamReader streamReader = new StreamReader(stream);
                answer = streamReader.ReadToEnd();
            }

            response.Close();

            WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(answer);

            return weatherResponse;
        }
    }
}

