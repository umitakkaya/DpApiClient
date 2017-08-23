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
            var doctorSchedules = repo.GetAll()
                .Where(ds => ds.Date.Add(ds.Start) >= DateTime.Now)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.Start);

            return View(doctorSchedules.ToList());
        }

        // GET: Schedule/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorSchedule doctorSchedule = repo.GetById(id.Value);
            if (doctorSchedule == null)
            {
                return HttpNotFound();
            }
            return View(doctorSchedule);
        }

        // GET: Schedule/Create
        public ActionResult Create()
        {
            var doctorFacilities = doctorFacilityRepo.GetAll();

            //ViewBag.DoctorId = new SelectList(doctorFacilityRepo.GetAllDoctors(), "Id", "Name");
            ViewBag.FacilityId = new SelectList(doctorFacilityRepo.GetAllFacilities(), "Id", "Name");
            ViewBag.ForeignDoctorServiceId = doctorServiceRepo.GetAll();
            return View();
        }

        // POST: Schedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Duration,Start,End,Date,DoctorId,FacilityId,ForeignDoctorServiceId")] DoctorSchedule doctorSchedule)
        {
            if (ModelState.IsValid)
            {
                repo.Insert(doctorSchedule);
                repo.Save();

                var doctorFacility = doctorFacilityRepo.GetByIds(doctorSchedule.DoctorId, doctorSchedule.FacilityId);

                scheduleManager.PushSlots(doctorFacility);

                return RedirectToAction("Index");
            }

            var doctorFacilities = doctorFacilityRepo.GetAll();

            ViewBag.DoctorId = new SelectList(doctorFacilityRepo.GetAllDoctors(), "Id", "Name", doctorSchedule.DoctorId);
            ViewBag.FacilityId = new SelectList(doctorFacilityRepo.GetAllFacilities(), "Id", "Name", doctorSchedule.FacilityId);
            ViewBag.ForeignDoctorServiceId = doctorServiceRepo.GetAll();
            return View(doctorSchedule);
        }

        // GET: Schedule/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorSchedule doctorSchedule = repo.GetById(id.Value);

            if (doctorSchedule == null)
            {
                return HttpNotFound();
            }

            var doctorFacilities = doctorFacilityRepo.GetAll();

            ViewBag.DoctorId = new SelectList(facilityRepo.GetDoctors(doctorSchedule.FacilityId), "Id", "Name", doctorSchedule.DoctorId);
            ViewBag.FacilityId = new SelectList(doctorFacilityRepo.GetAllFacilities(), "Id", "Name", doctorSchedule.FacilityId);
            ViewBag.ForeignDoctorServiceId = doctorServiceRepo.GetAll();

            return View(doctorSchedule);
        }

        // POST: Schedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Duration,Start,End,Date,DoctorId,FacilityId,ForeignDoctorServiceId")] DoctorSchedule doctorSchedule)
        {
            if (ModelState.IsValid)
            {

                var schedule = repo.GetByIdAsNonTracking(doctorSchedule.Id);
                scheduleManager.DeleteSlots(schedule, schedule.DoctorFacility);

                repo.Update(doctorSchedule);
                repo.Save();

                

                scheduleManager.PushSlots(repo.GetById(doctorSchedule.Id).DoctorFacility);

                return RedirectToAction("Index");
            }

            var doctorFacilities = doctorFacilityRepo.GetAll();

            ViewBag.DoctorId = new SelectList(facilityRepo.GetDoctors(doctorSchedule.FacilityId), "Id", "Name", doctorSchedule.DoctorId);
            ViewBag.FacilityId = new SelectList(doctorFacilityRepo.GetAllFacilities(), "Id", "Name", doctorSchedule.FacilityId);
            ViewBag.ForeignDoctorServiceId = doctorServiceRepo.GetAll();

            return View(doctorSchedule);
        }

        // GET: Schedule/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorSchedule doctorSchedule = repo.GetById(id.Value);

            if (doctorSchedule == null)
            {
                return HttpNotFound();
            }
            return View(doctorSchedule);
        }

        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var schedule = repo.GetById(id);
            var doctorFacility = doctorFacilityRepo.GetByIds(schedule.DoctorFacility.DoctorId, schedule.DoctorFacility.FacilityId);

            scheduleManager.DeleteSlots(schedule, schedule.DoctorFacility);

            //mark as fulfilled instead of deleting
            schedule.IsFullfilled = true;
            repo.Save();

            scheduleManager.PushSlots(doctorFacility);

            return RedirectToAction("Index");
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
