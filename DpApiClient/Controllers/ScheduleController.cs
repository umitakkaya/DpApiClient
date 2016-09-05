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
using DpApiClient.Core;
using DpApiClient.REST.Client;

namespace DpApiClient.Controllers
{
    public class ScheduleController : Controller
    {
        private HospitalContext context = new HospitalContext();
        private ScheduleRepository repo;
        private DoctorFacilityRepository doctorFacilityRepo;
        private DoctorRepository doctorRepo;
        private FacilityRepository facilityRepo;
        private ForeignDoctorServiceRepository doctorServiceRepo;
        private DpApi client;
        private ScheduleManager scheduleManager;

        public ScheduleController()
        {

            //Let's be sure that every repo is using the same DbContext
            repo = new ScheduleRepository(context);
            doctorFacilityRepo = new DoctorFacilityRepository(context);
            doctorRepo = new DoctorRepository(context);
            facilityRepo = new FacilityRepository(context);
            doctorServiceRepo = new ForeignDoctorServiceRepository(context);
            client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);
            scheduleManager = new ScheduleManager(context, client);
        }

        // GET: Schedule
        public ActionResult Index()
        {
            var doctorSchedules = repo.GetAll().Where(ds=> ds.Date.Add(ds.Start) >= DateTime.Now);
            return View(doctorSchedules.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                facilityRepo.Dispose();
                doctorRepo.Dispose();
                doctorFacilityRepo.Dispose();
                doctorServiceRepo.Dispose();
                repo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
