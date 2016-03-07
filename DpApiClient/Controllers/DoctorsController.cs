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

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Doctor doctor = doctorRepo.GetById(id.Value);

            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {

            var doctorView = new DoctorView()
            {
                Facilities = facilityRepo.GetAll().ToList(),
                Specializations = specializationRepo.GetAll().ToList()
            };

            return View(doctorView);
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DoctorView doctorView)
        {
            if (ModelState.IsValid)
            {


                var facilities = facilityRepo.GetByIds(doctorView.SelectedFacilityIds);
                var specializations = specializationRepo.GetByIds(doctorView.SelectedSpecializationIds);

                doctorRepo.Insert(doctorView.Doctor);
                doctorRepo.SetFacilities(doctorView.Doctor, facilities.ToList());
                doctorRepo.SetSpecializations(doctorView.Doctor, specializations.ToList());
                doctorRepo.Save();

                return RedirectToAction("Index");
            }

            return View(doctorView);
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Doctor doctor = doctorRepo.GetById(id.Value);

            if (doctor == null)
            {
                return HttpNotFound();
            }

            var doctorView = new DoctorView()
            {
                Facilities = db.Facilities.ToList(),
                Specializations = specializationRepo.GetAll().ToList(),
                Doctor = doctor
            };
            return View(doctorView);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DoctorView doctorView)
        {
            if (ModelState.IsValid)
            {
                var facilities = facilityRepo.GetByIds(doctorView.SelectedFacilityIds);
                var specializations = specializationRepo.GetByIds(doctorView.SelectedSpecializationIds);

                doctorRepo.Update(doctorView.Doctor);
                doctorRepo.SetFacilities(doctorView.Doctor, facilities.ToList());
                doctorRepo.SetSpecializations(doctorView.Doctor, specializations.ToList());
                doctorRepo.Save();

                return RedirectToAction("Index");
            }
            return View(doctorView);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DoctorRepository doctorRepo = new DoctorRepository();
            Doctor doctor = doctorRepo.GetById(id.Value);

            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorRepository doctorRepo = new DoctorRepository();
            doctorRepo.Delete(id);
            doctorRepo.Save();

            return RedirectToAction("Index");
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
