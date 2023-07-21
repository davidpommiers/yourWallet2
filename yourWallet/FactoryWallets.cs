using System.Text;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace NethereumSample
{
    
    public abstract class FactoryWallets
    {
        public static List<AccountWallet> InitAccount()
        {
            (Mnemonic,string) userSeedPassword = LogIn.Login();
            return FactoryCreateAllAccounts(userSeedPassword.Item1, userSeedPassword.Item2);
        }
        public static List<AccountWallet> FactoryCreateAllAccounts(Mnemonic seed, string password)
        {
            BTCAccount newAccountBTC = new BTCAccount(seed, password);
            AccountWallet newAccountETH = new ETHAccount(seed, password);
            List<AccountWallet> listAccount = new List<AccountWallet>();

            listAccount.Add(newAccountETH);
            listAccount.Add(newAccountBTC);
            
            return listAccount;
        }

        public static AccountWallet FactoryFindCorrectWallet(List<AccountWallet> allAccountWallet, Wallets id)
        {
            AccountWallet? walletWanted = null;

            for (int indexAccountWallet = 0; indexAccountWallet < allAccountWallet.Count; indexAccountWallet++)
            {
                if(id == allAccountWallet[indexAccountWallet].GetIDWallet())
                {
                    walletWanted = allAccountWallet[indexAccountWallet];
                }
            }

            return (walletWanted != null) ? walletWanted : throw new Exception("Wallet " + id+ "not found");
        }
    }
}