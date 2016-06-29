using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeGeoIP2Web2.Models
{
    [Serializable]
    public sealed class GeoIPResult
    {
        public Location Location { get; set; }
        public Block Block { get; set; }
    }

    [Serializable]
    public sealed class Block
    {
        public string isp { get; set; }
        public string geoname_id { get; set; }
        public string registered_country_geoname_id { get; set; }
        public string represented_country_geoname_id { get; set; }
        public bool is_anonymous_proxy { get; set; }
        public bool is_satellite_provider { get; set; }
        public string postal_code { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
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
    }
}