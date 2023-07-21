using System.Text;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    public static class BTCBlockchain
    {
        private const string BlockChainAPI = "1ef88390eda34881bb460ed1ec58c256";
        private static Network network = Network.TestNet; 


        private static async Task<JArray> GetUtxo(BitcoinAddress pubAddress)
        {
            // Récupérer les UTXOs du sender
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage utxoResponse = await httpClient.GetAsync("https://api.blockcypher.com/v1/btc/test3/addrs/"+pubAddress);
            string utxoJson = await utxoResponse.Content.ReadAsStringAsync();
            JObject utxos = JObject.Parse(utxoJson);
            string txrefs= string.Empty;
            try
            {
                txrefs = utxos.GetValue("txrefs").ToString();
            }
            catch(System.NullReferenceException)
            {
                Console.WriteLine("You do not have token in your wallet balance = " + utxos.GetValue("txrefs").ToString());
                throw new InvalidOperationException("Une erreur s'est produite.");
            }
            JArray utxosArray = JArray.Parse(txrefs);

            return utxosArray;
        }

        private static Transaction CreateTransaction(BitcoinAddress pubAddress, BitcoinSecret privateKey, 
                                                        BitcoinAddress receiverBTCAddress, decimal BtcToTransfert, 
                                                        Speed speed, Network network)
        {
            // Créer et signer la transaction
            Transaction transaction = network.CreateTransaction();
            Money toTransfer = new Money(BtcToTransfert, MoneyUnit.BTC);
            Money totalInputUTXO = new Money(0, MoneyUnit.BTC);
            List<Coin> coinArray = new List<Coin>();
            JArray utxosArray = GetUtxo(pubAddress).Result;

            foreach (JToken utxo in utxosArray)
            {
                if(toTransfer>totalInputUTXO)
                {
                    try
                    {
                        Console.WriteLine(utxo["spent"].ToString());
                        if(utxo["spent"].ToString() == "False")
                        {
                            OutPoint outpoint = new OutPoint(uint256.Parse(utxo["tx_hash"].ToString()), utxo["tx_output_n"].Value<int>());
                            TxIn txIn = new TxIn(outpoint);
                            TxOut txout = new TxOut(new Money(utxo["value"].Value<decimal>(), MoneyUnit.Satoshi), pubAddress);
                            Coin coin = new Coin(outpoint, txout);
                            transaction.Inputs.Add(txIn);
                            coinArray.Add(coin);
                            totalInputUTXO += coin.Amount;
                        }
                    }
                    catch(System.Exception)
                    {
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }

            //Estimate the gaz fee 
            Money fee = Fee(transaction);

            transaction.Outputs.Add(new TxOut(toTransfer, receiverBTCAddress));
            Money change = totalInputUTXO - toTransfer - fee - new Money((decimal)speed, MoneyUnit.Satoshi);
            Money zero = new Money(0, MoneyUnit.BTC);
            Console.WriteLine(change);

            if (change > zero)
            {
                Console.WriteLine("change = " + change.ToString());
                transaction.Outputs.Add(new TxOut(change, pubAddress));
            }
            else
            {
                throw new InvalidOperationException("Not Enough BTC Token");
            }

            transaction.Sign(privateKey, coinArray.ToArray());

            return transaction;
        }

        private  static async Task<string> SendTransaction(Transaction transaction,  string APIKey)
        {
            string transactionSigned = transaction.ToHex();
            HttpClient httpClient2 = new HttpClient();
            string blockCypherApiUrl = "https://api.blockcypher.com/v1/btc/test3/txs/push?token=" + APIKey;
            string jsonContent = "{\"tx\": \"" + transactionSigned + "\"}";
            HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient2.PostAsync(blockCypherApiUrl, httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Transaction diffusée avec succès !");
                return transaction.GetHash().ToString();
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erreur lors de la diffusion de la transaction : " + responseBody);
                return "";
            }
        }

        public static Money Fee(Transaction transaction)
        {
            long fees = transaction.Inputs.Count*180+(transaction.Outputs.Count+2)*24+10;
            Money fee = new Money(fees);
            return fee;
        }
        public  static  string Send(BitcoinAddress publicAddress, BitcoinSecret privatekey, BitcoinAddress destination, 
                                                decimal amount)
        {
            
            Transaction transaction =  CreateTransaction(publicAddress, privatekey, destination, amount, Speed.MEDIUM, network);
            string hashTransaction = SendTransaction(transaction, BlockChainAPI).Result;

            return hashTransaction;
        }

        public static async Task<decimal> GetBalance(BitcoinAddress publicAddress)
        {
            using (var httpClient = new HttpClient())
            {
                string url = "https://api.blockcypher.com/v1/btc/test3/addrs/"+publicAddress+"/balance";
                var response = await httpClient.GetAsync(url);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(jsonResponse);
                Console.WriteLine(json);

                decimal balanceSatoshi;
                try
                {
                    balanceSatoshi = json["balance"].Value<decimal>();
                }
                catch(System.AggregateException)
                {
                    balanceSatoshi = 0;
                }
                // Convertir le solde en BTC
                decimal balanceBTC = balanceSatoshi / 1_000_000_000m;
                Console.WriteLine("BTC balance :" + balanceBTC);
                return balanceSatoshi;
            }
        }

        public static (BitcoinSecret, BitcoinAddress) CreateAddressAndPrivateKey(Mnemonic seed,string password)
        {
            ExtKey hdroot = seed.DeriveExtKey(password);
            ExtKey pkey = hdroot.Derive(new NBitcoin.KeyPath("m/84'/0'/0'/0/0'"));
            BitcoinSecret privateKey = pkey.PrivateKey.GetBitcoinSecret(network);
            BitcoinAddress publicAddress = pkey.GetPublicKey().GetAddress(ScriptPubKeyType.Legacy, network);

            return(privateKey, publicAddress);
        }
        public static BitcoinSecret RecoverPrivateKey(Mnemonic seed,string password)
        {
            ExtKey hdroot = seed.DeriveExtKey(password);
            ExtKey pkey = hdroot.Derive(new NBitcoin.KeyPath("m/84'/0'/0'/0/0'"));
            BitcoinSecret privateKey = pkey.PrivateKey.GetBitcoinSecret(network);
            BitcoinAddress publicAddress = pkey.GetPublicKey().GetAddress(ScriptPubKeyType.Legacy, network);

            return privateKey;
        }
    }
}