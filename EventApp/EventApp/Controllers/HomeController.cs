using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace EventApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult index()
        {
            return View();
        }
    }


}