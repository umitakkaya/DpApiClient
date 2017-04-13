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

        // GET: Facilities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Facility facility = facilityRepo.GetById(id.Value);
            if (facility == null)
            {
                return HttpNotFound();
            }
            return View(facility);
        }

        // GET: Facilities/Create
        public ActionResult Create()
        {
            ViewBag.ParentFacilityId = new SelectList(facilityRepo.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ParentFacilityId")] Facility facility)
        {
            if (ModelState.IsValid)
            {

                facilityRepo.Insert(facility);
                facilityRepo.Save();

                return RedirectToAction("Index");
            }

            ViewBag.ParentFacilityId = new SelectList(facilityRepo.GetAll(), "Id", "Name", facility.ParentFacilityId);
            return View(facility);
        }

        // GET: Facilities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Facility facility = facilityRepo.GetById(id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentFacilityId = new SelectList(facilityRepo.GetAll(), "Id", "Name", facility.ParentFacilityId);
            return View(facility);
        }

        // POST: Facilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ParentFacilityId")] Facility facility)
        {
            if (ModelState.IsValid)
            {

                facilityRepo.Update(facility);
                facilityRepo.Save();
                return RedirectToAction("Index");
            }
            ViewBag.ParentFacilityId = new SelectList(facilityRepo.GetAll(), "Id", "Name", facility.ParentFacilityId);
            return View(facility);
        }

        // GET: Facilities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Facility facility = facilityRepo.GetById(id.Value);

            if (facility == null)
            {
                return HttpNotFound();
            }
            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            facilityRepo.Delete(id);
            facilityRepo.Save();

            return RedirectToAction("Index");
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
