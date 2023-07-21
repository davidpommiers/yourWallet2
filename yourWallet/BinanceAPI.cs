using System.Text;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    public enum Interval
    {
        SMALL,//2min
        MEDIUM,//15min
        LONG,//1h
        EXTRALONG//12h

    }
    public enum Symbol
    {
        ETHUSDT,
        ETHEUR,
        BTCUSDT,
        BTCEUR
    }
    public class BinanceAPI
    {
        private static string GetInterval(Interval interval)
        {
            string intervalResult = string.Empty;
            switch(interval)
            {
                case Interval.SMALL:
                    intervalResult = "3m";
                    break;
                case Interval.MEDIUM:
                    intervalResult = "15m";
                    break;
                case Interval.LONG:
                    intervalResult = "1h";
                    break;
                case Interval.EXTRALONG:
                    intervalResult = "12h";
                    break;   
            }

            return intervalResult;
        }

        private static string GetSymbol(Symbol symbol)
        {
            string symbolResult = string.Empty;
            switch(symbol)
            {
                case Symbol.BTCEUR:
                    symbolResult = "BTCEUR";
                    break;
                case Symbol.BTCUSDT:
                    symbolResult = "BTCUSDT";
                    break;
                case Symbol.ETHEUR:
                    symbolResult = "ETHEUR";
                    break;
                case Symbol.ETHUSDT:
                    symbolResult = "ETHUSDT";
                    break;   
            }

            return symbolResult;
        }

        public static async Task<List<List<object>>> RetrieveBinanceVariationCoinMarket(Interval interval, Symbol symbol, int limit = 720)
        {
            string intervalString = GetInterval(interval);
            string symbolString = GetSymbol(symbol);

            // The objectif was to stay at 720 points however because of
            // the absence of 2min interval, we need only for the SMALL intervall 
            // take 480 points 
            if(interval == Interval.SMALL)
            {
                limit = 480;
            }
            string apiUrl = "https://api.binance.com/api/v3/uiKlines?symbol="+symbolString+"&interval="+intervalString+"&limit="+limit;

            using (HttpClient? client = new HttpClient())
            {
                HttpResponseMessage? response = await client.GetAsync(apiUrl);
                string responseContent = await response.Content.ReadAsStringAsync();  
                List<List<object>>? priceHistory = JsonConvert.DeserializeObject<List<List<object>>>(responseContent);     

                return priceHistory;
            }
        }
        public static async Task<string> RetrieveAllInfoCoin24H(Symbol symbol)
        {
            string symbolString = GetSymbol(symbol);
            string apiUrl = "https://api.binance.com/api/v3/ticker/24hr?symbol="+symbolString;

            using (HttpClient? client = new HttpClient())
            {
                HttpResponseMessage? response = await client.GetAsync(apiUrl);
                string responseContent = await response.Content.ReadAsStringAsync();
                
                return responseContent;
            }
        }
    }
}