using System.Globalization;
using System.Text;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    public struct Data
    {
        public DateTime date;
        public float open {get;}
        public float high {get;}
        public float low {get;}
        public float close {get;}
        public Interval interval {get;}

        public Data(Interval interval, float close, float low, float high, float open, DateTime date)
        {
            this.interval = interval;
            this.close = close;
            this.low = low;
            this.high = high;
            this.open = open;
            this.date = date;
        }
    }
    public struct DataDailyInfo
    {
        public float highPrice {get;}
        public float lowPrice {get;}
        public float priceChange {get;}
        public float priceChangePercent {get;}
        public float volume {get; set;}

        public DataDailyInfo(float volume, float priceChangePercent, float priceChange, float lowPrice, float highPrice)
        {
            this.volume = volume;
            this.priceChangePercent = priceChangePercent;
            this.priceChange = priceChange;
            this.lowPrice = lowPrice;
            this.highPrice = highPrice;
        }
    }

    public class BinanceAllDatasCoin 
    {
        List<Data> CoinDailyMovement = new List<Data>();
        List<Data> CoinWeeklyMovement = new List<Data>();
        List<Data> CoinMonthlyMovement = new List<Data>();
        List<Data> CoinAnnuallyMovement = new List<Data>();
        Symbol symbole;
        DataDailyInfo allDatasOfDay;
        
        public BinanceAllDatasCoin(Symbol symbol)
        {
            this.symbole = symbol;

            this.CoinDailyMovement = LoadDatas(Interval.SMALL);
            this.CoinWeeklyMovement = LoadDatas(Interval.MEDIUM);
            this.CoinMonthlyMovement = LoadDatas(Interval.LONG);
            this.CoinAnnuallyMovement = LoadDatas(Interval.EXTRALONG);

            this.allDatasOfDay = LoadDatas24h(symbol);            
        }

        private List<Data> LoadDatas(Interval interval)
        {
            List<Data> resulData = new List<Data>();
            List<List<object>> resultBinanceAPI = BinanceAPI.RetrieveBinanceVariationCoinMarket(interval, this.symbole).Result;
            for (int indexDatas = 0; indexDatas < resultBinanceAPI.Count; indexDatas++)
            {
                DateTimeOffset openTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds((long)resultBinanceAPI[indexDatas][0]);
                DateTime openTimeLocal = openTimeUtc.ToOffset(TimeSpan.FromHours(2)).DateTime;

                float open = float.Parse(resultBinanceAPI[indexDatas][1].ToString(),  CultureInfo.InvariantCulture);
                float high =  float.Parse(resultBinanceAPI[indexDatas][2].ToString(),  CultureInfo.InvariantCulture);
                float low = float.Parse(resultBinanceAPI[indexDatas][3].ToString(),  CultureInfo.InvariantCulture);
                float close =  float.Parse(resultBinanceAPI[indexDatas][4].ToString(),  CultureInfo.InvariantCulture);

                Data newData = new Data(interval, close, low, high, open, openTimeLocal);
                resulData.Add(newData);
            }
            return resulData;
        } 

        private DataDailyInfo LoadDatas24h(Symbol symbol)
        {
            string  allDatas24h = BinanceAPI.RetrieveAllInfoCoin24H(symbol).Result;
            JObject jsonInfo24HInfo = JObject.Parse(allDatas24h);
            float highPrice = 0.0f;
            float lowPrice  = 0.0f;
            float priceChange  = 0.0f;
            float priceChangePercent  = 0.0f;
            float volume = 0.0f;
            if(jsonInfo24HInfo.ContainsKey("priceChange"))
            {
                priceChange = float.Parse(jsonInfo24HInfo.GetValue("priceChange").ToString(),  CultureInfo.InvariantCulture);
            }
            if(jsonInfo24HInfo.ContainsKey("highPrice"))
            {
                highPrice = float.Parse(jsonInfo24HInfo.GetValue("highPrice").ToString(),  CultureInfo.InvariantCulture);
            }
            if(jsonInfo24HInfo.ContainsKey("priceChangePercent"))
            {
                priceChangePercent = float.Parse(jsonInfo24HInfo.GetValue("priceChangePercent").ToString(),  CultureInfo.InvariantCulture);
            }
            if(jsonInfo24HInfo.ContainsKey("lowPrice"))
            {
                lowPrice = float.Parse(jsonInfo24HInfo.GetValue("lowPrice").ToString(),  CultureInfo.InvariantCulture);
            }
            if(jsonInfo24HInfo.ContainsKey("quoteVolume"))
            {
                volume = float.Parse(jsonInfo24HInfo.GetValue("quoteVolume").ToString(),  CultureInfo.InvariantCulture);
            }
            DataDailyInfo newDailyInfo = new DataDailyInfo(volume, priceChangePercent, priceChange, lowPrice, highPrice);
            return newDailyInfo;
        }

        public void PrintDatasCoin()
        {
            for (int indexDatas = 0; indexDatas < this.CoinDailyMovement.Count; indexDatas++)
            {
                DateTime date = this.CoinDailyMovement[indexDatas].date;
                double open = this.CoinDailyMovement[indexDatas].open;
                double high = this.CoinDailyMovement[indexDatas].high;
                double low = this.CoinDailyMovement[indexDatas].low;
                double close = this.CoinDailyMovement[indexDatas].close;
                Console.WriteLine($"Heure d'ouverture: {date}, Ouverture: {open}, Haut: {high}, Bas: {low}, Fermeture: {close}");
            }
            Console.WriteLine(this.CoinDailyMovement.Count);
        }

        public void PrintDatas24h()
        {
            float volume = this.allDatasOfDay.volume;
            float priceChangePercent = this.allDatasOfDay.priceChangePercent;
            float priceChange = this.allDatasOfDay.priceChange;
            float lowPrice = this.allDatasOfDay.lowPrice;
            float highPrice = this.allDatasOfDay.highPrice;

            Console.WriteLine("volume " + volume);
            Console.WriteLine("priceChangePercent "+ priceChangePercent);
            Console.WriteLine("priceChange "+ priceChange);
            Console.WriteLine("lowPrice "+ lowPrice );
            Console.WriteLine("highPrice "+ highPrice);
        }
    }
}