using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open_Local_Network_Operation_Centre.Frontend
{
    public static class FriendlyMessages
    {
        public static Dictionary<string, string> Messages = new Dictionary<string, string>
        {
            { "disclaimer", "! WARNING !\nThis file/project/library is not endorsed, made or distributed by NFO.\nThis binary and the libraries contained within it are provided as is.\nYou are solely responsible for any damages caused by proper or improper use of this project.\n\nType 'y' to confirm and proceed" },
            { "elevated", "[+] You are running OLNOC as Administrator. The webserver will be reachable via this devices network IPv4 address instead of localhost. This means other network devices will be able to reach your NOC. Remember you will need to either disable your FireWall or add an exception to it to allow HTTP traffic inbound and outbound." },
            { "notelevated", "[-] You are running OLNOC as a standard process. The webserver will be reachable via 127.0.0.1/localhost. This means no one but this device can reach your NOC." },
            { "credits", "[-] Open Local NOC was created, designed and written by Adam H." },
            { "sentget", "[-] Sent a request to a remote host outside of your network" }
        };
    }
}
