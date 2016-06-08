using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace FreeGeoIP2Web2
{
    public static class NetworkExtensions
    {
        public static long IPAddressAsLong(this string ip)
        {
            IPAddress address = null;
            if (IPAddress.TryParse(ip, out address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var mask = address.ToString().Split('.').Select(x => long.Parse(x)).ToArray();
                return (mask[0] * (long)Math.Pow(256, 3)) + (mask[1] * (long)Math.Pow(256, 2)) + (mask[2] * 256L) + mask[3];
            }
            else
                return 0;
        }

        public static long IPAddressAsLong(this IPAddress address)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                var mask = address.ToString().Split('.').Select(x => long.Parse(x)).ToArray();
                return (mask[0] * (long)Math.Pow(256, 3)) + (mask[1] * (long)Math.Pow(256, 2)) + (mask[2] * 256L) + mask[3];
            }
            else
                return 0;
        }
    }
}