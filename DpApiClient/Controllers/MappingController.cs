using DpApiClient.Core;
using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DpApiClient.Controllers
{
    public class MappingController : Controller
    {
        private DoctorMappingRepository repo;
        private ForeignAddressRepository addressRepo;
        private DoctorFacilityRepository doctorFacilityRepo;
        private ScheduleManager _scheduleManager;
        private DpApi _client;

        public MappingController()
        {
            var db = new HospitalContext();

            _client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);

            _scheduleManager = new ScheduleManager(db, _client);
            repo = new DoctorMappingRepository(db);
            addressRepo = new ForeignAddressRepository(db);
            doctorFacilityRepo = new DoctorFacilityRepository(db);
        }

        // GET: Mapping
        public ActionResult Index()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(AppSettings.Locale);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(AppSettings.Locale);


            var addresses = addressRepo.GetAll();
            ViewBag.DoctorFacilities = doctorFacilityRepo.GetNotMapped();
            return View(addresses);
        }


        [HttpPost]
        public ActionResult Map(string id, [Bind(Include = "DoctorId, FacilityId")] DoctorFacility doctorFacility)
        {
            var address = addressRepo.GetById(id);
            try
            {


                repo.Map(address, doctorFacility);
                repo.Save();
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }

        }


        [HttpPost]
        public ActionResult Sync(string id)
        {
            var address = addressRepo.GetById(id);
            var mapping = repo.GetByForeignAddress(id);
            try
            {
                bool result = _scheduleManager.PushSlots(mapping.DoctorFacility);
                return Json(new { status = result });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }

        }


        // POST: Mapping/Delete/5
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                var address = addressRepo.GetById(id);

                if (address == null)
                {
                    return HttpNotFound();
                }

                var mapping = repo.GetByForeignAddress(id);

                if(mapping == null)
                {
                    return HttpNotFound();
                }

                repo.Delete(mapping);
                repo.Save();

                return Json(new { status = true });
            }
            catch(Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
    }
}
