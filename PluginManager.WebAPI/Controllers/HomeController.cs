using PluginManager.WebAPI.Models;
using PluginManager.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PluginManager.WebAPI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            PluginService pluginSvc = new PluginService();
            List<PluginLibInfo> pli = pluginSvc.GetPluginsInfo();
            return View(pli);
        }
    }
}