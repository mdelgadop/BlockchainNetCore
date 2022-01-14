using System.Security.Cryptography;
using System.Text;

namespace JumaCoin.Business.classes.Helpers
{
    public class HashHelper
    {
        public static string Calculate(string data)
        {
            string myHashCalculated = string.Empty;
            
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] encodedPassword = new UTF8Encoding().GetBytes(data);
                var hash = mySHA256.ComputeHash(encodedPassword);
                myHashCalculated = System.BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            return myHashCalculated;
        }

    }
}