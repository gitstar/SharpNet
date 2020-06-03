using System;
using System.Security.Cryptography;
using System.Text;

namespace SharpNet.Core.Security
{
    public class Encryption: IEncryption
    {
        public string key { get; set; }

        public Encryption()
        {
            key = "aed323423dafasdf23422tsd";
        }
        public string EncryptMessage(byte[] text)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 256;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            aes.Key = Encoding.Default.GetBytes(key);
            aes.GenerateIV();

            string IV = ("-[--IV-[-" + Encoding.Default.GetString(aes.IV));

            ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] buffer = text;

            return Convert.ToBase64String(Encoding.Default.GetBytes(Encoding.Default.GetString(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length)) + IV));

        }

        public string EncryptMessage(string text)
        {
            return EncryptMessage(text.GetByteDefault());
        }

        public string DecryptMessage(string text)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 256;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;

            aes.Key = Encoding.Default.GetBytes(key);

            text = Encoding.Default.GetString(Convert.FromBase64String(text));

            string IV = text;
            IV = IV.Substring(IV.IndexOf("-[--IV-[-") + 9);
            text = text.Replace("-[--IV-[-" + IV, "");

            text = Convert.ToBase64String(Encoding.Default.GetBytes(text));
            aes.IV = Encoding.Default.GetBytes(IV);

            ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] buffer = Convert.FromBase64String(text);

            return Encoding.Default.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
    }
}
