using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.HdWallet;
using NBitcoin;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;

namespace NethereumSample
{
    public enum NetworkETH
    {
        TestNet,
        Main
    }
    /// <summary>
    /// The EthereumWallet class is a C# class that provides an interface to the Ethereum blockchain. 
    /// It allows users to manage Ethereum accounts and interact with the Ethereum network.
    /// </summary>
    public class ETHAccount : AccountWallet
    {
        private string publicAddress;
        private List<string> allTransactions = new List<string>();
        private List<decimal> accountHistory = new List<decimal>();
        private Network network = Network.TestNet;
        private Wallets id = Wallets.ETH;

        /// <summary>
        /// This constructor creates a new Ethereum wallet using a given seed phrase and password. 
        /// The seedPhrase parameter is a Mnemonic object that represents the seed phrase for the wallet, and the password parameter is a string that represents the password for the wallet.
        /// </summary>
        /// <param name="seedPhrase"></param>
        /// <param name="password"></param>
        public ETHAccount(Mnemonic seed, string password)
        {
            (string,string) addressAndPrivateKey = ETHBlockchain.CreateAddressAndPrivateKey(seed, password);
            this.publicAddress = addressAndPrivateKey.Item2.ToString();
            decimal valueBalance = ETHBlockchain.GetBalance(addressAndPrivateKey.Item2).Result;
            this.accountHistory.Add(valueBalance);
            Console.WriteLine("Public Address ETH :" + this.publicAddress);
            Console.WriteLine("Private Address ETH :" + addressAndPrivateKey.Item1);
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
            string privateKey = ETHBlockchain.RecoverPrivateKey(seed.Item1, seed.Item2);
            string hashTransaction = ETHBlockchain.Send(this.publicAddress, privateKey, To, amount).Result;
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