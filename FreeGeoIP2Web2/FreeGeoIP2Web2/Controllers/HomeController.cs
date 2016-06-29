using FreeGeoIP2Web2.Models;
using MaxMind.GeoIP2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace FreeGeoIP2Web2.Controllers
{
    //must add NuGet package: MaxMind.GeoIP2
    public class HomeController : Controller
    {
        [HttpGet, OutputCache(Duration = 3600, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        static DatabaseReader database = null;

        //https://dev.maxmind.com/geoip/geoip2/geolite2/
        GeoIPResult InnerResolve(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = Request.UserHostAddress;

            if (database == null)
                database = new DatabaseReader(Server.MapPath(@"~/App_Data/GeoLite2-City.mmdb"));

            var result = database.City(id.Replace("-", "."));

            if (result == null)
                return null;
            else
                return new GeoIPResult
                {
                    Block = new Block()
                    {
                        isp = result.Traits.Isp,
                        latitude = result.Location.Latitude,
                        longitude = result.Location.Longitude,
                        postal_code = result.Postal.Code,
                        is_anonymous_proxy = result.Traits.IsAnonymousProxy,
                        geoname_id = string.Format("{0}", result.City.GeoNameId),
                        is_satellite_provider = result.Traits.IsSatelliteProvider,
                        registered_country_geoname_id = string.Format("{0}", result.RegisteredCountry.GeoNameId),
                        represented_country_geoname_id = string.Format("{0}", result.RepresentedCountry.GeoNameId),
                    },
                    Location = new Location()
                    {
                        city_name = result.City.Name,
                        continent_name = result.Continent.Name,
                        country_name = result.Country.Name,
                        continent_code = result.Continent.Code,
                        country_iso_code = result.Country.IsoCode,
                        time_zone = result.Location.TimeZone,
                        metro_code = string.Format("{0}", result.Location.MetroCode),
                        geoname_id = string.Format("{0}", result.City.GeoNameId),
                        locale_code = null,
                        subdivision_1_iso_code = result.Subdivisions.Select(x => x.IsoCode).FirstOrDefault(),
                        subdivision_1_name = result.Subdivisions.Select(x => x.Name).FirstOrDefault(),
                        subdivision_2_iso_code = result.Subdivisions.Skip(1).Select(x => x.IsoCode).FirstOrDefault(),
                        subdivision_2_name = result.Subdivisions.Skip(1).Select(x => x.Name).FirstOrDefault(),
                    },
                };
        }

        [HttpGet, OutputCache(Duration = 3600, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Resolve(string id = null)
        {
            try
            {
                var result = InnerResolve(id);
                if (result != null)
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                    return HttpNotFound();
            }
            catch (Exception)
            {
                return HttpNotFound();
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, OutputCache(Duration = 3600, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult ResolveXml(string id = null)
        {
            try
            {
                var result = InnerResolve(id);
                if (result != null)
                {
                    var xs = new XmlSerializer(result.GetType());
                    using (var m = new MemoryStream())
                    {
                        xs.Serialize(m, result);
                        return new FileContentResult(m.ToArray(), "text/xml");
                    }
                }
                else
                    return HttpNotFound();
            }
            catch (Exception)
            {
                return HttpNotFound();
                //return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}