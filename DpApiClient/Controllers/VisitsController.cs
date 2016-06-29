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


        // GET: Visits/Create
        public ActionResult Create()
        {
            ViewBag.FacilityId = new SelectList(_doctorFacilityRepo.GetAllFacilities(), "Id", "Name");
            return View();
        }

        // POST: Visits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(
            [Bind(Include = "StartAt,EndAt,DoctorId,FacilityId,DoctorScheduleId")] Visit visit,
            [Bind(Include = "Name,Surname,Email,Phone,NIN,Gender,Birthdate")] VisitPatient patient)
        {
            if (ModelState.IsValid)
            {
                visit.VisitStatus = VisitStatus.Booked;
                visit.DoctorFacility = _doctorFacilityRepo.GetByIds(visit.DoctorId, visit.FacilityId);
                visit.DoctorSchedule = _scheduleRepo.GetById(visit.DoctorScheduleId);
                visit.VisitPatient = patient;
                visit.StartAt = visit.DoctorSchedule.Date.Add(visit.StartAt.TimeOfDay);
                visit.EndAt = visit.DoctorSchedule.Date.Add(visit.EndAt.TimeOfDay);

                try
                {
                    _visitManager.BookVisit(visit);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("form", ex.Message);
                }
            }

            ViewBag.FacilityId = new SelectList(_doctorFacilityRepo.GetAllFacilities(), "Id", "Name");
            return View(visit);
        }

        [HttpPost]
        public ActionResult Schedule(int doctorId, int facilityId)
        {
            var doctorFacility = _doctorFacilityRepo.GetByIds(doctorId, facilityId);
            var schedules = _scheduleRepo.GetByDoctorFacility(doctorFacility);
            var dates = schedules.GroupBy(ds => ds.Date).Select(ds => ds.Key.ToString("yyyy-MM-dd"));

            return Json(new { dates = dates });
        }

        [HttpPost]
        public ActionResult Slots(string date, int doctorId, int facilityId)
        {
            DateTime scheduleDate = DateTime.Parse(date);

            var doctorFacility = _doctorFacilityRepo.GetByIds(doctorId, facilityId);
            var slots = _scheduleManager.GetSlots(scheduleDate, doctorFacility);

            var slotsObject = slots.Select(s=> new
            {
                DoctorScheduleId = s.DoctorSchedule.Id,
                StartAt = s.StartTime.ToString(),
                EndAt = s.EndTime.ToString(),
                Duration = s.Duration,
                DoctorService = s.DoctorService == null ? null : s.DoctorService.Name,
                DoctorServiceId = s.DoctorService == null ? null : s.DoctorService.Id
            }).ToList();
            return Json(new { slots = slotsObject });
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
