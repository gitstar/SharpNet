//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml.Serialization;
//using static System.Windows.Forms.ListView;

//public static class ListViewExtentions
//{
//    public static bool SaveToXml(this ListView liveView, string myXmlFilePath)
//    {
//        try
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(ListViewItemCollection));

//            using (FileStream stream = File.OpenWrite(myXmlFilePath))
//            {
//                serializer.Serialize(stream, liveView.Items);
//            }

//            return true;
//        }
//        catch
//        {
//            return false;
//        }

      
//    }
//    public static void LoadFromXML(this string myXmlFilePath, ListView liveView)
//    {
//        if (File.Exists(myXmlFilePath))
//        {
//            XmlSerializer serializer = new XmlSerializer(typeof(ListViewItemCollection));

//            using (FileStream stream = File.OpenRead(myXmlFilePath))
//            {
//                liveView.Items.AddRange((ListViewItemCollection)serializer.Deserialize(stream));
//            };
//        };
//    }


//}