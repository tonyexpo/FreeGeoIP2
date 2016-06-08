using FreeGeoIP2Web2.Models;
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
    public class HomeController : Controller
    {
        [HttpGet, OutputCache(Duration = 3600, VaryByParam = "*", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View();
        }

        GeoIPResult InnerResolve(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = Request.UserHostAddress;
            else
                id = id.Replace('-', '.');

            var ip = id.IPAddressAsLong();
            if (ip != 0)
            {
                var block = Database.Blocks
                    .Where(x => ip >= x.network_start && ip <= x.network_end)
                    .FirstOrDefault();

                if (block == null)
                    return null;

                var location = Database.Locations.AsParallel().Where(x => x.geoname_id == block.geoname_id).FirstOrDefault();

                if (location != null)
                    return new GeoIPResult
                    {
                        Block = block,
                        Location = location,
                    };
                else
                    return null;
            }
            else
                return null;
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
                return new HttpStatusCodeResult(501);
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
                return new HttpStatusCodeResult(501);
            }
        }
    }
}