using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Core.Security
{
    public class Security
    {
        // Create the DES encryption provider:
        private DES des;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public Security()
        {
            des = new DESCryptoServiceProvider();
            //ReadDes();
        }

        public void CreateDes(string folderPath = null)
        {
            // Serialize the DES provider's key and IV to disk for decryption later:
            using (StreamWriter sw = new StreamWriter(folderPath + "DES.bin"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                byte[][] stuff = new byte[2][];
                stuff[0] = des.Key;
                stuff[1] = des.IV;
                bf.Serialize(sw.BaseStream, stuff);
            }
        }

        public void ReadDes()
        {

            try
            {
                // Deserialize the DES provider's key and IV from disk:
                using (StreamReader sr = new StreamReader("DES.bin"))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    byte[][] stuff = (byte[][])bf.Deserialize(sr.BaseStream);
                    des.Key = stuff[0];
                    des.IV = stuff[1];
                }
            }
            catch (System.Exception)
            {

            }

        }

        public bool ReadDesStream(byte[] bytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        byte[][] stuff = (byte[][])bf.Deserialize(sr.BaseStream);
                        des.Key = stuff[0];
                        des.IV = stuff[1];
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                logger.Error("GetTableDataList : ", ex.ToString(), "DapperFactoryBase");
            }

            return false;
        }

        public DataSet EncryptDataSet(DataSet ds, string fileName, string folderPath = null)
        {
            if (!Directory.Exists(folderPath + "data"))
                Directory.CreateDirectory(folderPath + "data");

            using (FileStream fs = new FileStream(fileName + ".bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (CryptoStream cs = new CryptoStream(fs, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // Encrypt the DataSet to the file:
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(ds.GetXml());
                    }
                }
            }

            // Now write the DataSet schema to disk:
            if (fileName.ToLower().Contains("pattern"))
                ds.WriteXmlSchema(folderPath + "data\\main.xsd");
            else
            {
                if (folderPath == null)
                    ds.WriteXmlSchema("data\\sub.xsd");
                else
                    ds.WriteXmlSchema(folderPath + "data\\sub.xsd");
            }

            return ds;
        }

        public DataSet DecryptDataSet(string fileName)
        {
            DataSet ds = new DataSet();

            // Decrypt the Encrypted DataSet:
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                // Decrypt the DataSet and store it into an instance:
                using (CryptoStream cs = new CryptoStream(fs, des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    ds.ReadXml(cs);
                }
            }

            // Now write the DataSet schema to disk:
            if (fileName.ToLower().Contains("pattern"))
                ds.ReadXmlSchema("data\\main.xsd");
            else
            {
                ds.ReadXmlSchema("data\\sub.xsd");
            }

            return ds;
        }



    }
}
