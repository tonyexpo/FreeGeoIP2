using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace FreeGeoIP2Web2
{
    /// <summary>
    /// http://dev.maxmind.com/geoip/geoip2/geolite2/
    /// </summary>
    public static class Database
    {
        public static IEnumerable<Block> Blocks { get; private set; }
        public static IEnumerable<Location> Locations { get; private set; }

#warning EXTRACE DATA FILES TO APP_DATA FOLDER

        internal static void Init()
        {
            var cityFname = HttpContext.Current.Server.MapPath("~/App_Data/GeoLite2-City-Blocks-IPv4.csv");
            var locFname = HttpContext.Current.Server.MapPath("~/App_Data/GeoLite2-City-Locations-en.csv");

            var task1 = Task.Factory.StartNew(() => Block.Read(cityFname).ToArray())
                  .ContinueWith(t => Blocks = t.Result);

            var task2 = Task.Factory.StartNew(() => Location.Read(locFname).ToArray())
                .ContinueWith(t => Locations = t.Result);

            Task.WaitAll(task1, task2);
        }
    }

    [Serializable]
    public sealed class Block
    {
        internal IPNetwork network { get; set; }
        public long network_start { get; set; }
        public long network_end { get; set; }
        public string geoname_id { get; set; }
        public string registered_country_geoname_id { get; set; }
        public string represented_country_geoname_id { get; set; }
        public bool is_anonymous_proxy { get; set; }
        public bool is_satellite_provider { get; set; }
        public string postal_code { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }

        static IEnumerable<string> ReadRows(string fname)
        {
            using (var f = File.OpenRead(fname))
            using (var r = new StreamReader(f))
                while (!r.EndOfStream)
                    yield return r.ReadLine();
        }

        public static IEnumerable<Block> Read(string fname)
        {
            var enus = new CultureInfo("en-US");
            return ReadRows(fname)
                .AsParallel()
                .Skip(1)
                .Select(r =>
                {
                    var s = r.Replace("\"", "").Split(',');
                    double lati;
                    double longi;
                    var i = 0;
                    var network = IPNetwork.Parse(s[i++]);
                    return new Block
                    {
                        network = network,
                        network_start = network.FirstUsable.IPAddressAsLong(),
                        network_end = network.LastUsable.IPAddressAsLong(),
                        geoname_id = s[i++],
                        registered_country_geoname_id = s[i++],
                        represented_country_geoname_id = s[i++],
                        is_anonymous_proxy = s[i++] == "1",
                        is_satellite_provider = s[i++] == "1",
                        postal_code = s[i++],
                        latitude = double.TryParse(s[i++], NumberStyles.Any, enus, out lati) ? lati : (double?)null,
                        longitude = double.TryParse(s[i++], NumberStyles.Any, enus, out longi) ? longi : (double?)null,
                    };
                });
        }
    }

    [Serializable]
    public sealed class Location
    {
        public string geoname_id { get; set; }
        public string locale_code { get; set; }
        public string continent_code { get; set; }
        public string continent_name { get; set; }
        public string country_iso_code { get; set; }
        public string country_name { get; set; }
        public string subdivision_1_iso_code { get; set; }
        public string subdivision_1_name { get; set; }
        public string subdivision_2_iso_code { get; set; }
        public string subdivision_2_name { get; set; }
        public string city_name { get; set; }
        public string metro_code { get; set; }
        public string time_zone { get; set; }

        static IEnumerable<string> ReadRows(string fname)
        {
            using (var f = File.OpenRead(fname))
            using (var r = new StreamReader(f))
                while (!r.EndOfStream)
                    yield return r.ReadLine();
        }

        public static IEnumerable<Location> Read(string fname)
        {
            var enus = new CultureInfo("en-US");
            return ReadRows(fname)
                .AsParallel()
                .Skip(1)
                .Select(r => r.Replace("\"", "").Split(','))
                .Select(s =>
                {
                    var i = 0;
                    return new Location
                    {
                        geoname_id = s[i++],
                        locale_code = s[i++],
                        continent_code = s[i++],
                        continent_name = s[i++],
                        country_iso_code = s[i++],
                        country_name = s[i++],
                        subdivision_1_iso_code = s[i++],
                        subdivision_1_name = s[i++],
                        subdivision_2_iso_code = s[i++],
                        subdivision_2_name = s[i++],
                        city_name = s[i++],
                        metro_code = s[i++],
                        time_zone = s[i++]
                    };
                });
        }
    }
}