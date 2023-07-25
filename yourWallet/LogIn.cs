using NBitcoin;
using System.Security.Cryptography;

//cV2VrGWeyPp5H5ePtkJeG7FjW1Vmnv2rGEvChBJnmKFAZKB5sVWZ alice 20 tBTC
namespace NethereumSample
{
    public class LogIn
    {       
        public static (Mnemonic,string) Login()
        {
            Console.WriteLine("[C3P0] : Hy I'm C3P0 and I will help you to manage or create your wallet");
            Console.Write("[C3P0] : Enter your password = ");
            string? myPassword = Console.WriteLine();
            while(myPassword == "")
            {
                myPassword = Console.ReadLine();
            }
            bool login = false;
            try
            {
                int infoUserLine = 0;
                string[] result = InformationUser.FindIVAndHashInLines(InformationUser.USERPATH, infoUserLine);
                string genesisAccountPivateKey = string.Empty;
                int WrongPassWord = 5;
                while(WrongPassWord > 1 && !login)
                {
                    try
                    {
                        Console.WriteLine("[C3P0] : Connexion...");
                        genesisAccountPivateKey = AES.Decrypt(result[0], myPassword, result[1]);
                        login = true;
                    }
                    catch(CryptographicException)
                    {
                        WrongPassWord--;

                        Console.WriteLine("[C3P0] : /!\\ WrongPassWord /!\\");
                        Console.WriteLine("[C3P0] : /!\\ "+ WrongPassWord+ " chances last /!\\");

                        Console.Write("[C3P0] : Enter your password master = ");
                        myPassword = Console.ReadLine();
                        Console.WriteLine();
                    }
                }
                if(WrongPassWord == 0)
                {
                    throw new Exception("[C3P0] : Are you really my master ? ");
                }

                return (new Mnemonic(genesisAccountPivateKey), myPassword);
            }
            catch(System.IndexOutOfRangeException)
            {
                Console.WriteLine("[C3P0] : It seems like you do not have a account");
                return SingUp(myPassword);
            }
        }
        public static (Mnemonic, string) SingUp(string myPassword)
        {
            string[] lines = File.ReadAllLines("User.txt");
            Console.Write("[C3P0] : Confirm your password ");
            myPassword = Console.ReadLine();
            Console.WriteLine("[C3P0] : Perfect your password is " + myPassword);
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.TwentyFour);

            (string, string) encryptMessageAndIV = AES.Encypt(mnemo.ToString(), myPassword);
            InformationUser.SaveInfo(encryptMessageAndIV.Item1+" "+encryptMessageAndIV.Item2, InformationUser.USERPATH);
            return (mnemo, myPassword);
        }
    }
}