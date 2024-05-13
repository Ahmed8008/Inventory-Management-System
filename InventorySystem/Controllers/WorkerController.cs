using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventorySystem.Models;
using System.Data.SqlClient;
namespace InventorySystem.Controllers
{
    public class WorkerController : Controller
    {
        //
        // GET: /Worker/


        DataBase db = new DataBase();

        public ActionResult WorkerDashBoard()
        {
            return View();
        }

      
    
    
    
    
    
    }
}
