using BYSDN.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BYSDN.Controllers
{
    public class PageController : Controller
    {
        BYSDNEntities entities = new BYSDNEntities();
        //
        // GET: /Page/
        public ActionResult Index()
        {
            var person = (from a in entities.Table_User
                          where a.Name == Request.LogonUserIdentity.Name
                          select a.ID).FirstOrDefault();

            if(person == Guid.Empty)
            {
                return RedirectToAction("Registe", "User");                
            }

            return View();
        }
	}
}