namespace SharpNet.Core.Security
{
    public interface IEncryption
    {
        string key { get; set; }
        string EncryptMessage(byte[] bytes);

        string EncryptMessage(string text);
        string DecryptMessage(string text);
    }
}
