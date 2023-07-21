using System.Text;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    public enum Wallets
    {
        ETH,
        BTC
    }
    
    public interface AccountWallet
    {        
        public  string GetPublicAddress();
        public  decimal GetBalance();
        public  string Send(string To, decimal amount);
        public  Wallets GetIDWallet();   
        public  List<string> GetAllTransactions();
        public  List<decimal> GetAllHistoryAccount();
    }
}