using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using DpApiClient.REST.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core
{
    public class ImportManager
    {
        private static HospitalContext db = new HospitalContext();

        public static void Import()
        {
            string clientId = AppSettings.ClientId;
            string clientSecret = AppSettings.ClientSecret;
            string locale = AppSettings.Locale;

            var client = new DpApi(clientId, clientSecret, (Locale)locale);
            var facilities = client.GetFacilities();

            foreach (var facility in facilities)
            {
                Console.WriteLine($"IMPORTING FACILITY: {facility.Name}\n");
                var foreignFacility = CreateOrUpdateFacility(facility);

                var dpDoctors = client.GetDoctors(facility.Id, true);

                foreach (var doctor in dpDoctors)
                {
                    Console.WriteLine($"-> {doctor.Name} {doctor.Surname} (ID:{doctor.Id})");
                    var foreignDoctor = CreateOrUpdateDoctor(doctor);

                    var specializations = client.GetDoctor(facility.Id, doctor.Id).Specializations;
                    Console.WriteLine("---> IMPORTING SPECIZALIZATIONS");
                    ImportSpecializations(specializations, foreignDoctor);

                    var addresses = client.GetAddresses(facility.Id, doctor.Id);
                    Console.WriteLine("---> IMPORTING ADDRESSES");
                    ImportAddresses(foreignFacility, foreignDoctor, addresses);

                    var doctorServices = client.GetDoctorServices(facility.Id, doctor.Id);
                    Console.WriteLine("---> IMPORTING DOCTOR SERVICES");
                    ImportDoctorServices(foreignDoctor, doctorServices);
                    ClearLeftOverDoctorServices(foreignDoctor, doctorServices);

                    Console.WriteLine("");
                }
            }

            db.SaveChanges();
        }

        private static void ClearLeftOverDoctorServices(ForeignDoctor foreignDoctor, List<DoctorService> doctorServices)
        {
            string[] validDoctorServiceIDs = doctorServices.Select(ds => ds.Id).ToArray();
            var invalidDoctorServices = foreignDoctor.ForeignDoctorServices
                .Where(fds => 
                    false == validDoctorServiceIDs.Contains(fds.Id) 
                    && fds.ForeignDoctorId == foreignDoctor.Id
                );

            string[] invalidIds = invalidDoctorServices
                .Select(ds => ds.Id)
                .ToArray();

            var invalidSchedules = db.DoctorSchedules.Where(ds => invalidIds.Contains(ds.ForeignDoctorServiceId));

            foreach (var schedule in invalidSchedules)
            {
                schedule.ForeignDoctorServiceId = null;
                schedule.ForeignDoctorService = null;
            }

            db.DoctorMappings.RemoveRange(db.DoctorMappings.Where(dm => invalidIds.Contains(dm.ForeignDoctorServiceId)));
            db.ForeignDoctorServices.RemoveRange(invalidDoctorServices);
        }

        private static ForeignDoctor CheckDoctorContext(string doctorId)
        {
            return db.ForeignDoctors.SingleOrDefault(fd => fd.Id == doctorId) ??
                db.ForeignDoctors.Local.SingleOrDefault(fd => fd.Id == doctorId) ??
                new ForeignDoctor();
        }


        private static ForeignDoctor CreateOrUpdateDoctor(DPDoctor doctor)
        {
            var foreignDoctor = CheckDoctorContext(doctor.Id);

            foreignDoctor.Id = doctor.Id;
            foreignDoctor.Name = doctor.Name;
            foreignDoctor.Surname = doctor.Surname;

            if (db.Entry(foreignDoctor).State == EntityState.Detached)
            {
                db.ForeignDoctors.Add(foreignDoctor);
            }

            return foreignDoctor;
        }


        private static ForeignFacility CheckFacilityContext(string facilityId)
        {
            return db.ForeignFacilities.SingleOrDefault(ff => ff.Id == facilityId) ??
                db.ForeignFacilities.Local.SingleOrDefault(ff => ff.Id == facilityId) ??
                new ForeignFacility();
        }

        private static ForeignFacility CreateOrUpdateFacility(DPFacility facility)
        {
            var foreignFacility = CheckFacilityContext(facility.Id);

            foreignFacility.Id = facility.Id;
            foreignFacility.Name = facility.Name;

            if (db.Entry(foreignFacility).State == EntityState.Detached)
            {
                db.ForeignFacilities.Add(foreignFacility);
            }

            return foreignFacility;
        }

        private static void ImportSpecializations(DPCollection<REST.DTO.Specialization> specializations, ForeignDoctor foreignDoctor)
        {
            if (specializations == null)
            {
                return;
            }

            specializations
                .Items
                .Where(s => s.Id != null && s.Name != null)
                .ToList()
                .ForEach(s =>
                {
                    var foreignSpecialization = foreignDoctor.ForeignSpecializations
                        .SingleOrDefault(fs => fs.Id == s.Id && fs.ForeignDoctorId == foreignDoctor.Id)
                        ?? new ForeignSpecialization();

                    foreignSpecialization.ForeignDoctor = foreignDoctor;
                    foreignSpecialization.Id = s.Id;
                    foreignSpecialization.Name = s.Name;

                    if (false == foreignDoctor.ForeignSpecializations.Exists(fs => fs.Id == s.Id))
                    {
                        foreignDoctor.ForeignSpecializations.Add(foreignSpecialization);
                    }
                });
        }

        private static void ImportAddresses(ForeignFacility foreignFacility, ForeignDoctor foreignDoctor, List<Address> addresses)
        {
            if (addresses == null)
            {
                return;
            }

            foreach (var address in addresses)
            {
                var foreignAddress = foreignFacility.ForeignAddresses
                    .SingleOrDefault(fa => fa.Id == address.Id)
                    ?? new ForeignAddress();

                foreignAddress.Id = address.Id;
                foreignAddress.Name = address.Name;
                foreignAddress.Street = address.Street + " " + address.PostCode;
                foreignAddress.ForeignDoctor = foreignDoctor;
                foreignAddress.ForeignFacility = foreignFacility;

                var extraFields = foreignAddress.BookingExtraFields ?? new Data.BookingExtraFields() { ForeignAddressId = address.Id };

                extraFields.IsBirthDate = address.BookingExtraFields.IsBirthDateEnabled;
                extraFields.IsGender = address.BookingExtraFields.IsGenderEnabled;
                extraFields.IsNin = address.BookingExtraFields.IsNinEnabled;

                foreignAddress.BookingExtraFields = extraFields;

                if (false == foreignFacility.ForeignAddresses.Exists(fa => fa.Id == address.Id))
                {
                    foreignFacility.ForeignAddresses.Add(foreignAddress);
                }
            }
        }

        private static void ImportDoctorServices(ForeignDoctor foreignDoctor, List<DoctorService> doctorServices)
        {
            if (doctorServices == null)
            {
                return;
            }

            foreach (var doctorService in doctorServices)
            {
                var foreignDoctorService = foreignDoctor.ForeignDoctorServices
                    .SingleOrDefault(ds => ds != null && ds.Id == doctorService.Id)
                    ?? new ForeignDoctorService();

                foreignDoctorService.Id = doctorService.Id;
                foreignDoctorService.Name = doctorService.Name;
                foreignDoctorService.PriceMax = doctorService.PriceMax;
                foreignDoctorService.PriceMin = doctorService.PriceMin;
                foreignDoctorService.ServiceId = doctorService.ServiceId;
                foreignDoctorService.ForeignDoctorId = foreignDoctor.Id;

                if (false == foreignDoctor.ForeignDoctorServices.Exists(fd => fd.Id == doctorService.Id))
                {
                    foreignDoctor.ForeignDoctorServices.Add(foreignDoctorService);
                }
            }
        }
    }
}
