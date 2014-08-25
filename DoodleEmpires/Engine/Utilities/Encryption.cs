using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Utilities
{
    internal static class Encryption
    {
        internal static string _privateKey;
        internal static string _publicKey;
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        internal static void Initialize()
        {
            var rsa = new RSACryptoServiceProvider();
            _privateKey = rsa.ToXmlString(true);
            _publicKey = rsa.ToXmlString(false);
        }

        internal static void Initialize(string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            _privateKey = rsa.ToXmlString(true);
            rsa.FromXmlString(publicKey);
        }

        internal static string Decrypt(string data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string[] dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(_privateKey);
            byte[] decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        internal static string Encrypt(string data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            byte[] dataToEncrypt = _encoder.GetBytes(data);
            byte[] encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            int length = encryptedByteArray.Count();
            int item = 0;
            StringBuilder sb = new StringBuilder();
            foreach (byte x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }
    }
}
