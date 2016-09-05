using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DpApiClient.Data;

namespace DpApiClient.Controllers
{
    public class SpecializationsController : Controller
    {
        private HospitalContext db = new HospitalContext();

        // GET: Specializations
        public ActionResult Index()
        {
            return View(db.Specializations.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
