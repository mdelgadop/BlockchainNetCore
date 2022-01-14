using System.Security.Cryptography;
using System.Text;

namespace JumaCoin.Business.classes.Helpers
{
    public class RSAHelper
    {
        #region Generation of keys
        public static string[] GenerateKeys()
        {
            string[] keys = new string[2];
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                keys[0] = SerializeRSAParameters(RSA.ExportParameters(false));//public key
                keys[1] = SerializeRSAParameters(RSA.ExportParameters(true));//private key
            }
            return keys;
        }

        #endregion Generation of keys

        #region Encryption and Descryption
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, string publicKey, bool DoOAEPPadding = false)
        {
            try
            {
                RSAParameters key = DeserializeRSAParameters(publicKey);
                byte[] encryptedData;

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(key);
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                System.Console.WriteLine(e.Message);

                return null;
            }
        }

        public static byte[] RSADecrypt(byte[] DataToDecrypt, string privateKey, bool DoOAEPPadding = false)
        {
            try
            {
                RSAParameters key = DeserializeRSAParameters(privateKey);
                byte[] decryptedData;

                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(key);
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                System.Console.WriteLine(e.ToString());

                return null;
            }
        }

        #endregion Encryption and Descryption

        #region Signature
        public static byte[] Sign(string data, string privateKey)
        {
            RSAParameters key = DeserializeRSAParameters(privateKey);
            byte[] signature;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(data);

                try
                {
                    rsa.ImportParameters(key);

                    signature = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return signature;
        }
        public static bool Verify(string data, byte[] signature, string privateKey)
        {
            RSAParameters key = DeserializeRSAParameters(privateKey);
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] dataBytes = encoder.GetBytes(data);
                try
                {
                    rsa.ImportParameters(key);

                    success = rsa.VerifyData(dataBytes, CryptoConfig.MapNameToOID("SHA512"), signature);
                }
                catch (CryptographicException e)
                {
                    System.Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }
        #endregion Signature

        #region Internal serialization of keys
        private static string SerializeRSAParameters(RSAParameters parameters)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, parameters);
            string pubKeyString = sw.ToString();
            
            return pubKeyString;
        }

        private static RSAParameters DeserializeRSAParameters(string data)
        {
            var sr = new System.IO.StringReader(data);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            RSAParameters pubKey = (RSAParameters)xs.Deserialize(sr);

            return pubKey;
        }
        #endregion Internal serialization of keys
    }
}