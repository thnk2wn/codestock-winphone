using System;
using System.Text;
using Newtonsoft.Json;

namespace CodeStock.Data
{
    public static class JsonUtility
    {
        public static T Deserialize<T>(string result)
        {
            try
            {
                var item = JsonConvert.DeserializeObject<T>(result);
                return item;
            }
            catch (JsonReaderException readerEx)
            {
                var sb = new StringBuilder();
                sb.Append("An error occurred reading JSON data. Most likely an HTML response was returned when ");
                sb.Append("JSON was expected; this might happen when not fully connected to a public WiFi network ");
                sb.Append("with a login page. ");
                sb.AppendLine();
                sb.AppendLine();
                sb.Append("You may need to open a web browser, supply any logon credentials, verify WiFi connection, ");
                sb.Append("restart the app, and try again (or connect via cellular).");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("Error was: {0}", readerEx.Message);
                throw new DataFormatReadException(sb.ToString(), readerEx);
            }
        }
    }

    public class DataFormatReadException : Exception
    {
        public DataFormatReadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
