using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DpApiClient.Data;
using DpApiClient.Data.Repositories;

namespace DpApiClient.Controllers
{
    public class FacilitiesController : Controller
    {
        private FacilityRepository facilityRepo = new FacilityRepository();

        // GET: Facilities
        public ActionResult Index()
        {
            var facilities = facilityRepo.GetAll();
            return View(facilities.ToList());
        }

        [HttpGet]
        public ActionResult GetDoctors(int id)
        {
            var doctors = facilityRepo.GetDoctors(id).Select(f=> new {f.Id, f.Name }).ToList();


            return Json(doctors, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                facilityRepo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
