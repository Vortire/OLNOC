using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Open_Local_Network_Operation_Centre.Providers
{
    public static class NFO
    {
        /// <summary>
        /// Contains the ability to send a specific GET request to
        /// NFOServers with specific attributes applied.
        /// 
        /// Should only be called from other API functions that
        /// are public.
        /// </summary>
        /// <param name="Host">NFOServers URI</param>
        /// <param name="header_email">NFOServers account email</param>
        /// <param name="header_pswd">NFOSevers account password</param>
        /// <param name="data_serviceName">The name of the service to modify</param>
        /// <param name="data_serviceType">The type of the service to modify</param>
        /// <returns>Response body</returns>
        private static Bitmap NFOHGET_Call_GET(string Host, string header_email, string header_pswd, string data_serviceName, string data_serviceType)
        {
            try
            {
                // Declare our handler and form request
                HttpWebRequest hwr_Req = (HttpWebRequest)WebRequest.Create(Host);

                // Sanitise user input for protection
                header_email = header_email.Replace(";", "");
                header_pswd = header_pswd.Replace(";", "");
                header_email = header_email.Replace(@"""", "");
                header_pswd = header_pswd.Replace(@"""", "");
                header_email = header_email.Replace("=", "");
                header_pswd = header_pswd.Replace("=", "");

                // Set our UserAgent
                hwr_Req.UserAgent =
                    $"Open Local Network Operations Centre. Time is {DateTime.UtcNow}";

                // Notify console
                Console.WriteLine(Frontend.FriendlyMessages.Messages["sentget"]);

                // Set request headers
                hwr_Req.Headers.Add("cookie", $"email={header_email};password={header_pswd};cookietoken=a");
                hwr_Req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                // Return the response to the requesting object
                using (HttpWebResponse response = (HttpWebResponse)hwr_Req.GetResponse())
                {
                    return new Bitmap(response.GetResponseStream());
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// Get the last 30 second traffic sample from NFO
        /// </summary>
        /// <param name="email">NFO Account Email Address</param>
        /// <param name="password">NFO Account Password</param>
        /// <param name="sname">NFO Account Service Name</param>
        /// <param name="stype">NFO Account Service Type</param>
        /// <returns>Bitmap image of the graph requested</returns>
        public static Bitmap Get30SecondSample(string email, string password, string sname, string stype = "virtual")
        {
            return NFOHGET_Call_GET(
                "https://www.nfoservers.com/control/getdgraph.pl?graphtype=h",
                email,
                password,
                sname,
                stype);
        }

        /// <summary>
        /// Get the last 5 minute traffic sample from NFO
        /// </summary>
        /// <param name="email">NFO Account Email Address</param>
        /// <param name="password">NFO Account Password</param>
        /// <param name="sname">NFO Account Service Name</param>
        /// <param name="stype">NFO Account Service Type</param>
        /// <returns>Bitmap image of the graph requested</returns>
        public static Bitmap Get5MinuteSample(string email, string password, string sname, string stype = "virtual")
        {
            return NFOHGET_Call_GET(
                "https://www.nfoservers.com/control/getdgraph.pl?graphtype=d",
                email,
                password,
                sname,
                stype);
        }

        /// <summary>
        /// Get the last 30 minute traffic sample from NFO
        /// </summary>
        /// <param name="email">NFO Account Email Address</param>
        /// <param name="password">NFO Account Password</param>
        /// <param name="sname">NFO Account Service Name</param>
        /// <param name="stype">NFO Account Service Type</param>
        /// <returns>Bitmap image of the graph requested</returns>
        public static Bitmap Get30MinuteSample(string email, string password, string sname, string stype = "virtual")
        {
            return NFOHGET_Call_GET(
                "https://www.nfoservers.com/control/getdgraph.pl?graphtype=w",
                email,
                password,
                sname,
                stype);
        }

        /// <summary>
        /// Get the last 2 hour traffic sample from NFO
        /// </summary>
        /// <param name="email">NFO Account Email Address</param>
        /// <param name="password">NFO Account Password</param>
        /// <param name="sname">NFO Account Service Name</param>
        /// <param name="stype">NFO Account Service Type</param>
        /// <returns>Bitmap image of the graph requested</returns>
        public static Bitmap Get2HourSample(string email, string password, string sname, string stype = "virtual")
        {
            return NFOHGET_Call_GET("https://www.nfoservers.com/control/getdgraph.pl?graphtype=m",
                email,
                password,
                sname,
                stype);
        }
    }
}
