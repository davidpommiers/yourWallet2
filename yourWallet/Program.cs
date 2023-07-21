using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace NethereumSample
{

    class Program
    {
        static async Task Main(string[] args)
        {

            List<AccountWallet> account = FactoryWallets.InitAccount();
            AccountWallet btc = FactoryWallets.FactoryFindCorrectWallet(account, Wallets.BTC);

            btc.Send("muETSWT5tf2W1XZVSLivatFX8mgR8Pmsre", 0.001m);            
        }
    }
}
