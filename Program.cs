using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

// We want to catch every exception
AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

// Reload whenever we find an exception
static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
{
    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
    Environment.Exit(0);
}

// Print the disclaimer
/*
 * IF IN RELEASE VERSION
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine(Open_Local_Network_Operation_Centre.Frontend.FriendlyMessages.Messages["disclaimer"]);
if(Console.ReadLine().ToLower() != "y") { Environment.Exit(0); }
Console.ResetColor();
Console.Clear();
*/

// Read config
string[] Configuration = File.ReadAllLines("config.txt");

// Get account username
Open_Local_Network_Operation_Centre.GlobalVariables.Email =
    Configuration[0];
Console.Clear();

// Get account password
Open_Local_Network_Operation_Centre.GlobalVariables.Password =
    Configuration[1];
Console.Clear();

// Get service name
Open_Local_Network_Operation_Centre.GlobalVariables.ServiceName =
    Configuration[2];
Console.Clear();

// Get service type
Open_Local_Network_Operation_Centre.GlobalVariables.ServiceType =
    Configuration[3];
Console.Clear();

// Are we running with elevated privileges or standard privileges?
if (WindowsIdentity.GetCurrent().Owner
          .IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
{
    // Start webserver instance on local IP
    Open_Local_Network_Operation_Centre.Frontend.Webserver._initFrontend(
        $"http://{GetLocalIPAddress()}:80/");
    Console.WriteLine(Open_Local_Network_Operation_Centre.Frontend.FriendlyMessages.Messages["elevated"]);
}
else
{
    // Start webserver instance on localhost
    Open_Local_Network_Operation_Centre.Frontend.Webserver._initFrontend(
        $"http://localhost:8000/");
    Console.WriteLine(Open_Local_Network_Operation_Centre.Frontend.FriendlyMessages.Messages["notelevated"]);
}

// Stop our program auto-exiting
Console.ReadLine();

/// <summary>
/// Get our local IP address which can be used by the 
/// webserver to give us network-wide access.
/// </summary>
string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    throw new Exception("This device doesn't support IPv4");
}

string GetSensitiveInput()
{
    var pass = string.Empty;
    ConsoleKey key;
    do
    {
        var keyInfo = Console.ReadKey(intercept: true);
        key = keyInfo.Key;

        if (key == ConsoleKey.Backspace && pass.Length > 0)
        {
            Console.Write("\b \b");
            pass = pass[0..^1];
        }
        else if (!char.IsControl(keyInfo.KeyChar))
        {
            Console.Write("*");
            pass += keyInfo.KeyChar;
        }
    } while (key != ConsoleKey.Enter);
    return pass;
}