using System.IO;
using System.Text;

namespace Phone.Common.Extensions.System.IO_
{
    public static class StreamExtensions
    {
        public static string ReadToEnd(this Stream s)
        {
            using (var sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

        public static void Write(this Stream s, string str)
        {
            using (var sr = new StreamWriter(s))
            {
                sr.Write(str);
            }
        }

        public static Stream ToStream(this string str)
        {
            byte[] byteArray = Encoding.Unicode.GetBytes(str);
            var stream = new MemoryStream(byteArray);
            return stream;
        }
    }
}
