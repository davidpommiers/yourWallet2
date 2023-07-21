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
    public static class ETHBlockchain
    {
        private static Web3 network =new Web3("https://sepolia.infura.io/v3/969eda60bf5242dfb2c46f0fa053e7a6"); 

        public static async Task<string> Send(string publicAddress, string privateKey, string To, decimal amount, string data = "")
        {
            decimal chainId = Convert.ToDecimal(network.Net.Version.SendRequestAsync().Result);
            BigInteger amountToSend = Web3.Convert.ToWei(amount); // in Wei, 1 Ether for example

            HexBigInteger nonce = await network.Eth.Transactions.GetTransactionCount.SendRequestAsync(publicAddress);
            BigInteger maxPriorityFeePerGas = Web3.Convert.ToWei(2, UnitConversion.EthUnit.Gwei); // 2 Gwei
            BigInteger maxFeePerGas = Web3.Convert.ToWei(30, UnitConversion.EthUnit.Gwei); // 30 Gwei
            BigInteger gasLimit = 21000;
            BigInteger chain = new BigInteger((decimal)chainId);
            var balance = await network.Eth.GetBalance.SendRequestAsync(publicAddress);

            Nethereum.Model.Transaction1559 transaction = new Nethereum.Model.Transaction1559(chain, nonce, maxPriorityFeePerGas, maxFeePerGas, gasLimit, To, amountToSend, data, null);
            Nethereum.Signer.Transaction1559Signer? signer = new Transaction1559Signer();

            string signedTransaction = signer.SignTransaction(privateKey, transaction);
            Console.WriteLine($"Transaction sent with ID: {signedTransaction}");

            string txId = await network.Eth.Transactions.SendRawTransaction.SendRequestAsync("0x" + signedTransaction);
            Console.WriteLine($"Transaction sent with ID: {txId}");
            return txId;
        }

        public static async Task<Decimal> GetBalance(string address)
        {
            HexBigInteger balance = await network.Eth.GetBalance.SendRequestAsync(address);

            decimal etherAmount = Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");

            return etherAmount;
        }
        public static (string, string) CreateAddressAndPrivateKey(Mnemonic seed,string password)
        {
            Wallet myWalletEth = new Wallet(seed.ToString(), password);
            Account accountEth = myWalletEth.GetAccount(0);
            Web3 network = new Web3(accountEth,"https://sepolia.infura.io/v3/969eda60bf5242dfb2c46f0fa053e7a6");

            return(accountEth.PrivateKey, accountEth.Address);
        }
        public static string RecoverPrivateKey(Mnemonic seed,string password)
        {
            Wallet myWalletEth = new Wallet(seed.ToString(), password);
            Account accountEth = myWalletEth.GetAccount(0);
            Web3 network = new Web3(accountEth,"https://sepolia.infura.io/v3/969eda60bf5242dfb2c46f0fa053e7a6");

            return accountEth.PrivateKey;
        }
    }
}