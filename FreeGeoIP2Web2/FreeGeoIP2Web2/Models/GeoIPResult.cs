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
}