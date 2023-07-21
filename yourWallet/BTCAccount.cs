using System.Text;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    public enum Speed
    {
        LOW = 74,
        MEDIUM = 148,
        HIGH = 256,
        FAST = 300
    }
    public class BTCAccount : AccountWallet
    {
        private string publicAddress;
        private Wallets id = Wallets.BTC;
        private List<string> allTransactions = new List<string>();
        private List<decimal> accountHistory = new List<decimal>();
        private Network network = Network.TestNet;

        public BTCAccount(Mnemonic seed, string password)
        {
            (BitcoinSecret,BitcoinAddress) addressAndPrivateKey = BTCBlockchain.CreateAddressAndPrivateKey(seed, password);
            this.publicAddress = addressAndPrivateKey.Item2.ToString();
            decimal valueBalance = BTCBlockchain.GetBalance(addressAndPrivateKey.Item2).Result;
            this.accountHistory.Add(valueBalance);
            
            Console.WriteLine("Public Address BTC :" + this.publicAddress);
            Console.WriteLine("Private Address BTC :" + addressAndPrivateKey.Item1);
        }

        public  string GetPublicAddress()
        {
            return this.publicAddress.ToString();
        }
        public  Wallets GetIDWallet()
        {
            return this.id;
        }

        public  List<string> GetAllTransactions()
        {
            return this.allTransactions;
        }
        public  List<decimal> GetAllHistoryAccount()
        {
            return this.accountHistory;
        }

        public string Send(string To, decimal amount)
        {
            (Mnemonic, string) seed = LogIn.Login(); 
            BitcoinSecret privateKey = BTCBlockchain.RecoverPrivateKey(seed.Item1, seed.Item2);
            BitcoinAddress address = privateKey.GetAddress(ScriptPubKeyType.Legacy);
            BitcoinAddress addressDestination = BitcoinAddress.Create(To, network);
            string hashTransaction = BTCBlockchain.Send(address, privateKey, addressDestination, amount);
            this.allTransactions.Add(hashTransaction);

            return hashTransaction;
        }

        public decimal GetBalance()
        {
            BitcoinAddress address = BitcoinAddress.Create(this.publicAddress, this.network);
            decimal balance = BTCBlockchain.GetBalance(address).Result;
            this.accountHistory.Add(balance);

            return balance;
        }

    }

}