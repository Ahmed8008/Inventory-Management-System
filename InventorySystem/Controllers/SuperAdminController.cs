using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventorySystem.Controllers
{
    public class SuperAdminController : Controller
    {
        //
        // GET: /SuperAdmin/

        public ActionResult SuperAdminDashboard()
        {
            return View();
        }

    }
}
