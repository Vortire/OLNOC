using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Open_Local_Network_Operation_Centre.Frontend
{
    /// <summary>
    /// Parts of this code have been taken from:
    /// https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7
    /// </summary>
    public static class Webserver
    {
        private static HttpListener? listener;
        private static bool _keepRunning = true;

        private static string ReadResource(string resourceName)
        {
            var _contents = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();

            // Get resource
            using (Stream stream = assembly.GetManifestResourceStream(
                assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(resourceName))
                ))

            // Get contents
            using (StreamReader reader = new StreamReader(stream))
            {
                _contents = reader.ReadToEnd();
            }

            // Get timezones
            var timeUtc = DateTime.UtcNow;
            TimeZoneInfo ESTZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo GMTZ = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
            DateTime EST = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, ESTZ);
            DateTime GMT = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, GMTZ);

            // Parse for inputs that need replacing
            _contents = _contents.
                Replace("%time%", DateTime.UtcNow.ToString()).
                Replace("%username%", $"{Environment.UserDomainName}:{Environment.UserName}").
                Replace("%service%", GlobalVariables.ServiceName).
                Replace("%gmt%", GMT.ToShortTimeString()).
                Replace("%est%", EST.ToShortTimeString());

            // Return parsed inputs
            return _contents;
        }

        private static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private static async Task _handleConnection()
        {
            while (_keepRunning)
            {
                try
                {
                    // Will wait here until we hear from a connection
                    HttpListenerContext ctx = await listener.GetContextAsync();
                    HttpListenerRequest req = ctx.Request;

                    // Peel out the requests and response objects
                    HttpListenerResponse resp = ctx.Response;

                    // Set data array to null ready to be filled
                    byte[] data = null;

                    // Sanitise requested input
                    string _req = req.RawUrl.Replace("/", "");

                    // Sanity check
                    if (_req.Length > 50)
                    {
                        resp.Close();
                        Console.WriteLine(
                            $"Closed connection from {req.RemoteEndPoint}" +
                            $" because raw request was larger than" +
                            $" 60 characters.");
                        continue;
                    }

                    // Make sure we have enough workers left
                    if(GlobalVariables.WorkersAlive > GlobalVariables.MaximumWorkersAlive)
                    {
                        // Woah! We're full at the moment, close request
                        resp.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        resp.Close();
                        Console.WriteLine(
                            $"Closed connection from {req.RemoteEndPoint}" +
                            $" because we ran out of worker space." +
                            $" Currently working: {GlobalVariables.WorkersAlive} | Maximum allows: {GlobalVariables.MaximumWorkersAlive}.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Allowed connection from {req.RemoteEndPoint}" +
                            $" because we have enough worker space." +
                            $" Currently working: {GlobalVariables.WorkersAlive} | Maximum allows: {GlobalVariables.MaximumWorkersAlive}.");
                    }

                    // Process this request in a new thread (worker)
                    new Thread(() =>
                    {
                        // Tell OLNOC we're a new worker
                        GlobalVariables.WorkersAlive++;

                        // Switch between possible URL types
                        Console.WriteLine("Processing: " + req.RawUrl);
                        switch (_req)
                        {
                            case "30secondsample.png":
                                // Write the latest 30 second sample
                                data = ImageToByte(
                                    Providers.NFO.Get30SecondSample(
                                        GlobalVariables.Email,
                                        GlobalVariables.Password,
                                        GlobalVariables.ServiceName,
                                        GlobalVariables.ServiceType)
                                    );
                                resp.ContentType = "image/png; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "5minutesample.png":
                                // Write the latest 5 minute sample
                                data = ImageToByte(
                                    Providers.NFO.Get5MinuteSample(
                                        GlobalVariables.Email,
                                        GlobalVariables.Password,
                                        GlobalVariables.ServiceName,
                                        GlobalVariables.ServiceType)
                                    );
                                resp.ContentType = "image/png; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "30minutesample.png":
                                // Write the latest 30 minute sample
                                data = ImageToByte(
                                    Providers.NFO.Get30MinuteSample(
                                        GlobalVariables.Email,
                                        GlobalVariables.Password,
                                        GlobalVariables.ServiceName,
                                        GlobalVariables.ServiceType)
                                    );
                                resp.ContentType = "image/png; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "2hoursample.png":
                                // Write the latest 2 hour sample
                                data = ImageToByte(
                                    Providers.NFO.Get2HourSample(
                                        GlobalVariables.Email,
                                        GlobalVariables.Password,
                                        GlobalVariables.ServiceName,
                                        GlobalVariables.ServiceType)
                                    );
                                resp.ContentType = "image/png; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "traffic.html":
                                // traffic page
                                data = Encoding.UTF8.GetBytes(
                                    ReadResource("Traffic.html")
                                    );
                                resp.ContentType = "text/html; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "themedtraffic.html":
                                // traffic page
                                data = Encoding.UTF8.GetBytes(
                                    ReadResource("ThemedTraff_ic.html")
                                    );
                                resp.ContentType = "text/html; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            case "dictionary.html":
                                // dictionary page
                                data = Encoding.UTF8.GetBytes(
                                    ReadResource("Dictionary.html")
                                    );
                                resp.ContentType = "text/html; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                            default:
                                // Index page
                                data = Encoding.UTF8.GetBytes(
                                    ReadResource("Index.html")
                                    );
                                resp.ContentType = "text/html; charset=utf-8";
                                resp.ContentEncoding = Encoding.UTF8;
                                resp.ContentLength64 = data.LongLength;
                                break;
                        }

                        // Write out to the response stream (asynchronously), then close it
                        resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();

                        // Tell OLNOC we're done as a worker
                        GlobalVariables.WorkersAlive--;
                    }).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception found! Details: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Switch _keepRunning to false which will exit the 
        /// while loop in the connection handler.
        /// </summary>
        public static void _shutDown()
        {
            _keepRunning = false;
        }

        private static void _mainInit(string host)
        {
            while (true)
            {
                try
                {
                    // Create a Http server and start listening for incoming connections
                    listener = new HttpListener();
                    listener.Prefixes.Add(host);
                    listener.Start();

                    // Handle requests
                    Task listenTask = _handleConnection();
                    listenTask.GetAwaiter().GetResult();

                    // Close the listener
                    listener.Close();
                }
                catch (Exception e) {
                    // We get this error since we're booting a new webserver on an existing prefix
                    if (e.Message.Contains("conflicts")) { continue; }
                    
                    // This isn't an error we're aware of, show the console
                    Console.WriteLine($"Exception found! Details: {e.Message}"); }
            }
        }

        public static void _initFrontend(string host = "http://localhost:8000/")
        {
            // Create new thread handler for the webserver
            new Thread(() =>
            {
                _mainInit(host);
            }).Start();

            // Notify console of the prefix
            Console.WriteLine("[+] Your OLNOC webserver is reachable using this address: " + host);
        }
    }
}
