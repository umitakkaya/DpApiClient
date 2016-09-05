using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DpApiClient.Data;
using DpApiClient.ViewModels;
using DpApiClient.Data.Repositories;

namespace DpApiClient.Controllers
{
    public class DoctorsController : Controller
    {
        private DoctorRepository doctorRepo;
        private FacilityRepository facilityRepo;
        private SpecializationRepository specializationRepo;

        private HospitalContext db = new HospitalContext();

        public DoctorsController()
        {
            doctorRepo = new DoctorRepository(db);
            facilityRepo = new FacilityRepository(db);
            specializationRepo = new SpecializationRepository(db);
        }

        // GET: Doctors
        public ActionResult Index()
        {
            return View(doctorRepo.GetAll());
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
