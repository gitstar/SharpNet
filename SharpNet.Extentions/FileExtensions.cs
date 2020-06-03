using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;


public static class FileExtensions
{
    public static string ExistsFileName(this string file)
    {
        return new FileInfo(file).ExistsFileName();
    }
    public static string ExistsFileName(this FileInfo file)
    {
        var filename = file.Name.Replace(file.Extension, string.Empty);
        var dir = file.Directory.FullName;
        var ext = file.Extension;

        if (file.Exists)
        {
            int count = 0;
            string added;

            do
            {
                count++;
                added = "_" + count;
            } while (File.Exists(dir + "\\" + filename + added + ext));

            filename += added;
        }

        return (dir + "\\" + filename + ext);
    }

    public static List<string> GetFilesList(this String dirPath, string filterPattern = null)
    {
        if (!Directory.Exists(dirPath))
            return null;

        string[] tempFiles;

        if(string.IsNullOrEmpty(filterPattern))
            tempFiles = Directory.GetFiles(dirPath);
        else
            tempFiles = filterPattern.Split('|').SelectMany(d => Directory.GetFiles(dirPath, d)).ToArray();

        List<string> spFiles = new List<string>();

        foreach (string file in tempFiles)
        {
            string[] tmps = file.Split('\\');


            spFiles.Add(tmps[tmps.Length - 1]);
        }

        return spFiles;
    }
       
    //public static string FileToBase64(this FileInfo fInfo)
    //{
    //    return Exyll.Base64Encoder.ToBase64String(fInfo.FileToByte());
    //}

    public static byte[] FileToCompressByte(this FileInfo fInfo)
    {
        using (FileStream fs = new FileStream(fInfo.FullName, FileMode.Open, FileAccess.Read))
        {
            byte[] filebytes = new byte[fs.Length];
            fs.Read(filebytes, 0, (int)fs.Length);
            return filebytes.Compress();
        }
    }

    //public static string FileToBase64(this string filePath)
    //{
    //    if (!string.IsNullOrEmpty(filePath))
    //    {
    //        filePath = filePath.Trim(Path.GetInvalidFileNameChars());
    //        filePath = filePath.Trim(Path.GetInvalidPathChars());

    //        return new FileInfo(filePath).FileToBase64();
    //    }
    //    return null;
    //}

    public static byte[] FileToCompressByte(this string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            filePath = filePath.Trim(Path.GetInvalidFileNameChars());
            filePath = filePath.Trim(Path.GetInvalidPathChars());

            return new FileInfo(filePath).FileToCompressByte();
        }
        return null;
    }

    //public static Image Base64ToImage(this string base64String)
    //{
    //    byte[] imageBytes = base64String.Base64ToByte();
    //    using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
    //    {
    //        ms.Write(imageBytes, 0, imageBytes.Length);
    //        Image image = Image.FromStream(ms, true);
    //        return image;
    //    }
    //}

    public static Image ByteToImage(this byte[] bytes)
    {
        using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
        {
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }

    //public static byte[] Base64ToByte(this string base64String)
    //{
    //    return Exyll.Base64Encoder.FromBase64String(base64String).Decompress();
    //}

    public static string RandomFullFileName(this string directory, string fileExtension)
    {
        return Path.Combine(directory, fileExtension.RandomFileName());
    }

    public static string RandomFileName(this string fileExtension)
    {
        return Path.ChangeExtension(Path.GetRandomFileName(), fileExtension);
    }
}