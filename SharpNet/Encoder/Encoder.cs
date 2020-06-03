using System.Text;

namespace SharpNet.Encoder
{
    public class Encoder
    {
        static public Encoding EUCKR()
        {
            return Encoding.GetEncoding(51949);
        }

        static public Encoding UTF8()
        {
            return Encoding.UTF8;
        }
    }
}
