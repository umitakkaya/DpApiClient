using DpApiClient.Data;
using DpApiClient.REST.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpApiClient.REST.DTO;
using System.Data.Entity;
using DpApiClient.Data.Repositories;
using DpApiClient.Core;

namespace DpApiClient.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientId = AppSettings.ClientId;
            string clientSecret = AppSettings.ClientSecret;
            string locale = AppSettings.Locale;
            bool shouldExit = false;


            HospitalContext myDb = new HospitalContext();

            var facilityRepo = new ForeignFacilityRepository(myDb);
            var doctorRepo = new ForeignDoctorRepository(myDb);
            var addressRepo = new ForeignAddressRepository(myDb);
            var specializationRepo = new ForeignSpecializationRepository(myDb);
            var doctorServiceRepo = new ForeignDoctorServiceRepository(myDb);

            var client = new DpApi(clientId, clientSecret, (Locale)locale);
            var facilities = client.GetFacilities();

            foreach (var facility in facilities)
            {

                Console.WriteLine($"IMPORTING FACILITY: {facility.Name}\n");
                var foreignFacility = new ForeignFacility()
                {
                    Id = facility.Id,
                    Name = facility.Name
                };

                facilityRepo.InsertOrUpdate(foreignFacility);

                var dpDoctors = new List<DPDoctor>(client.GetDoctors(facility.Id, true));

                Console.WriteLine("DOCTORS:");
                foreach (var dpDoctor in dpDoctors)
                {
                    Console.WriteLine($"-> {dpDoctor.Name} {dpDoctor.Surname} (ID:{dpDoctor.Id})");

                    var foreignDoctor = new ForeignDoctor()
                    {
                        Id = dpDoctor.Id,
                        Name = dpDoctor.Name,
                        Surname = dpDoctor.Surname
                    };

                    var tempDoctor = client.GetDoctor(facility.Id, dpDoctor.Id);

                    Console.WriteLine("---> IMPORTING SPECIZALIZATIONS");
                    ImportSpecializations(specializationRepo, tempDoctor, foreignDoctor);


                    var addresses = client.GetAddresses(facility.Id, dpDoctor.Id);
                    Console.WriteLine("---> IMPORTING ADDRESSES");
                    ImportAddresses(addressRepo, dpDoctor, foreignFacility, foreignDoctor, addresses);



                    var doctorServices = client.GetDoctorServices(facility.Id, dpDoctor.Id);
                    Console.WriteLine("---> IMPORTING DOCTOR SERVICES");
                    ImportDoctorServices(doctorServiceRepo, dpDoctor, foreignDoctor, doctorServices);


                    doctorRepo.InsertOrUpdate(foreignDoctor);
                    Console.WriteLine();
                }
            }

            doctorRepo.Save();

            Console.WriteLine("DONE");

            if (shouldExit == false)
            {
                Console.ReadLine();
            }

        }

        private static void ImportSpecializations(ForeignSpecializationRepository repo, DPDoctor dpDoctor, ForeignDoctor foreignDoctor)
        {
            if (dpDoctor.Specializations == null)
            {
                return;
            }

            dpDoctor.Specializations
                .Items
                .ForEach(ds =>
                {
                    if (ds.Id != null && ds.Name != null)
                    {
                        repo.InsertOrUpdate(new ForeignSpecialization()
                        {
                            ForeignDoctorId = foreignDoctor.Id,
                            Id = ds.Id,
                            Name = ds.Name
                        });
                    }
                });
        }

        private static void ImportAddresses(ForeignAddressRepository addressRepo, DPDoctor dpDoctor, ForeignFacility foreignFacility, ForeignDoctor foreignDoctor, List<Address> addresses)
        {
            if (addresses == null)
            {
                return;
            }

            foreach (var address in addresses)
            {
                var foreignAddress = new ForeignAddress()
                {
                    Id = address.Id,
                    Name = address.Name,
                    Street = address.Street + " " + address.PostCode,
                    BookingExtraFields = new Data.BookingExtraFields()
                    {
                        ForeignAddressId = address.Id,
                        IsBirthDate = address.BookingExtraFields.IsBirthDateEnabled,
                        IsGender = address.BookingExtraFields.IsGenderEnabled,
                        IsNin = address.BookingExtraFields.IsNinEnabled
                    },
                    ForeignDoctorId = foreignDoctor.Id,
                    ForeignFacilityId = foreignFacility.Id
                };

                addressRepo.InsertOrUpdate(foreignAddress);
            }
        }

        private static void ImportDoctorServices(ForeignDoctorServiceRepository doctorServiceRepo, DPDoctor dpDoctor, ForeignDoctor foreignDoctor, List<DoctorService> doctorServices)
        {
            if (doctorServices == null)
            {
                return;
            }

            foreach (var doctorService in doctorServices)
            {
                var foreignDoctorService = new ForeignDoctorService()
                {
                    Id = doctorService.Id,
                    Name = doctorService.Name,
                    PriceMax = doctorService.PriceMax,
                    PriceMin = doctorService.PriceMin,
                    ServiceId = doctorService.ServiceId,
                    ForeignDoctorId = foreignDoctor.Id
                };

                doctorServiceRepo.InsertOrUpdate(foreignDoctorService);
            }
        }
    }
}
