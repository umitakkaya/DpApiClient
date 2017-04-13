using DpApiClient.Core;
using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using DpApiClient.ViewModels;
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
        private ScheduleManager scheduleManager;
        private ForeignDoctorServiceRepository doctorServiceRepo;
        private DpApi client;

        public MappingController()
        {
            var db = new HospitalContext();

            client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);

            scheduleManager = new ScheduleManager(db, client);
            repo = new DoctorMappingRepository(db);
            addressRepo = new ForeignAddressRepository(db);
            doctorFacilityRepo = new DoctorFacilityRepository(db);
            doctorServiceRepo = new ForeignDoctorServiceRepository(db);
        }

        // GET: Mapping
        public ActionResult Index()
        {

            var model = new MappingViewModel()
            {
                ForeignAddresses = addressRepo.GetAll(),
                DoctorFacilities = doctorFacilityRepo.GetNotMapped(),
                ForeignDoctorServices = doctorServiceRepo.GetAll()
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult Map(string id, [Bind(Include = "DoctorId, FacilityId")] DoctorFacility doctorFacility, string foreignDoctorServiceId)
        {
            var address = addressRepo.GetById(id);
            var foreignDoctorService = doctorServiceRepo.GetById(foreignDoctorServiceId);

            try
            {
                repo.Map(address, doctorFacility, foreignDoctorService);
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
                bool clearResult = scheduleManager.ClearDPCalendar(mapping.DoctorFacility);
                bool pushResult = scheduleManager.PushSlots(mapping.DoctorFacility);
                return Json(new { status = pushResult && clearResult });
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
