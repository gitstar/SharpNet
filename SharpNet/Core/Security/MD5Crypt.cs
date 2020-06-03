using System;
using System.Security.Cryptography;

namespace SharpNet.Core.Security
{
    public class MD5Crypt:IEncryption
    {
        public string key { get; set; }
        public string EncryptMessage(byte[] asciiBytes)
        {
            //byte[] data = Convert.FromBase64String(val);
            //// This is one implementation of the abstract class MD5.
            //MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] result = md5.ComputeHash(data);
            //return Convert.ToBase64String(result);
            ///////////////

           // byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(val);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedString;
        }

        public string EncryptMessage(string text)
        {
            return EncryptMessage(text.GetByteDefault());
        }

        public string DecryptMessage(string text)
        {
            return null;
        }

    }
}
