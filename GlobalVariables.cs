using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open_Local_Network_Operation_Centre
{
    public static class GlobalVariables
    {
        public static string Email = string.Empty;
        public static string Password = string.Empty;
        public static string ServiceName = string.Empty;
        public static string ServiceType = string.Empty;

        /// <summary>
        /// Contains the amount of workers currently processing a 
        /// request.
        /// </summary>
        public static Int64 WorkersAlive = 0;

        /// <summary>
        /// If more workers are alive than the maximum allows,
        /// new requests will be closed until the workers alive dips below
        /// the maximum.
        /// </summary>
        public static Int64 MaximumWorkersAlive = 1000;
    }
}
