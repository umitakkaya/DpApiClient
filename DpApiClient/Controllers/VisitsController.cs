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
    public class VisitsController : Controller
    {
        private HospitalContext _db = new HospitalContext();
        private VisitRepository _repo;
        private DoctorFacilityRepository _doctorFacilityRepo;
        private ScheduleRepository _scheduleRepo;
        private VisitManager _visitManager;
        private ScheduleManager _scheduleManager;
        private DpApi _client;

        public VisitsController()
        {
            _repo = new VisitRepository(_db);
            _doctorFacilityRepo = new DoctorFacilityRepository(_db);
            _scheduleRepo = new ScheduleRepository(_db);


            _client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);
            _scheduleManager = new ScheduleManager(_db, _client);
            _visitManager = new VisitManager(_db, _client, _scheduleManager);
        }

        // GET: Visits
        public ActionResult Index()
        {
            var visits = _repo.GetAll();
            return View(visits.ToList());
        }

        // POST: Visits/Cancel/5
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            Visit visit = _repo.GetById(id);
            try
            {
                 _visitManager.CancelVisit(visit);
                return Json(new { status = true });
            }
            catch (ArgumentException ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
