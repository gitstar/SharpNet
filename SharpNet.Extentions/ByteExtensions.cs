using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ByteExtensions
{
    public static byte[] Compress(this byte[] data)
    {
        byte[] compressed;
        using (var outStream = new MemoryStream())
        {
            using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
            using (var mStream = new MemoryStream(data))
                mStream.CopyTo(tinyStream);

            compressed = outStream.ToArray();

            return compressed;
        }
    }

    public static byte[] Decompress(this byte[] data)
    {
        try
        {
            using (var inStream = new MemoryStream(data))
            using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);

                return bigStreamOut.ToArray();
                // output = Encoding.UTF8.GetString(bigStreamOut.ToArray());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
       
    }

    public static string ToEucKRString(this byte[] data)
    {
        int euckrCodepage = 51949;
        return Encoding.GetEncoding(euckrCodepage).GetString(data);
    }

    public static async Task<byte[]> ReadAllBytes(this BinaryReader reader)
    {
        const int bufferSize = 4096;
        using (var ms = new MemoryStream())
        {
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                await   ms.WriteAsync(buffer, 0, count);

            return ms.ToArray();
        }
    }
}