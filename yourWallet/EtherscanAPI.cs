using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    /// <summary>
    /// Retrieve the ABI of a collection, needed to find the smart contract in the blockchain
    /// </summary>
    static class  EtherScan
    {
        private const string  APIKEYETHSCAN = "D6YB9S2A9QESQD6JJ7YH2Z8YTHDGQZ1YAX" ;

        /// <summary>
        /// Retrieves all the transactions hash of a address
        /// </summary>
        /// <param name="address">The Ethereum address.</param>
        /// <returns>List of transaction hash</returns>        
        public static List<string> RetrieveAllTransactions(string address)
        {
            string result = new WebClient().DownloadString("https://api.etherscan.io/api?module=account&action=txlist&address="+
                                                            address + "&startblock=0&endblock=99999999&page=1&offset=10&sort=asc"+
                                                            "&apikey=" + APIKEYETHSCAN);
            List<string> allTransactions = new List<string>();
            JObject jsonTransactions = JObject.Parse(result);
            if(jsonTransactions.ContainsKey("status"))
            {
                string statusString = jsonTransactions.GetValue("status").ToString();
                if(statusString == "0")
                {
                    throw new InvalidOperationException("Invalid API Key of etherscan");
                }
                JArray? transationsArray = JArray.Parse(jsonTransactions.GetValue("result").ToString());

                for(int indexTransaction = 0; indexTransaction < transationsArray.Count; indexTransaction++)
                {
                    JObject transaction = JObject.Parse(transationsArray[indexTransaction].ToString());

                    allTransactions.Add(transaction.GetValue("hash").ToString());
                }

                return allTransactions;
            }
            
            throw new InvalidOperationException("Error in the API etherscan, something may have changed,"+
                                                    " follow : https://docs.etherscan.io/api-endpoints/contracts");
        }

        /// <summary>
        /// Retrieves all the transactions hash of a address
        /// </summary>
        /// <param name="hash">The hash of a transaction</param>
        /// <returns>Status of the transaction, DONE or WIP </returns>  
        public static bool RetrieveStatusTransaction(string hash)
        {
            string result = new WebClient().DownloadString("https://api.etherscan.io/api?module=transaction&action=gettxreceiptstatus"+
                                                            "&txhash="+hash+"&apikey="+APIKEYETHSCAN);
            JObject statusTransaction = JObject.Parse(result);
            if(statusTransaction.ContainsKey("status"))
            {
                string statusString = statusTransaction.GetValue("status").ToString();
                if(statusString == "0")
                {
                    throw new InvalidOperationException("Invalid API Key of etherscan");
                }
                string resultMessage = statusTransaction.GetValue("message").ToString();
                return resultMessage.Equals("OK");
                
            }
            
            throw new InvalidOperationException("Error in the API etherscan, something may have changed,"+
                                                    " follow : https://docs.etherscan.io/api-endpoints/contracts");
        }
    }
}