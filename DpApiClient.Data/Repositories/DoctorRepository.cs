using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private HospitalContext _db { get; set; }
        public DoctorRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public IEnumerable<DoctorFacility> GetDoctorFacilities(Doctor doctor)
        {
            return _db.DoctorFacilities.Where(df => df.DoctorId == doctor.Id);
        }

        public IEnumerable<Facility> GetFacilities(Doctor doctor)
        {
            return _db.DoctorFacilities.Where(df => df.DoctorId == doctor.Id).Select(df => df.Facility);
        }

        public DoctorFacility AddFacility(Doctor doctor, Facility facility)
        {

            return _db.DoctorFacilities.Add(new DoctorFacility()
            {
                Doctor = doctor,
                Facility = facility
            });
        }

        public void RemoveFacility(Doctor doctor, Facility facility)
        {
            var doctorFacility = _db.DoctorFacilities.SingleOrDefault(df => df.DoctorId == doctor.Id && df.FacilityId == facility.Id);

            _db.DoctorFacilities.Remove(doctorFacility);
        }

        /// <summary>
        /// Replace all facilities
        /// </summary>
        /// <param name="doctor"></param>
        /// <param name="facilities"></param>
        public void SetFacilities(Doctor doctor, List<Facility> facilities)
        {
            _db.DoctorFacilities.RemoveRange(GetDoctorFacilities(doctor));

            facilities.ForEach(f =>
            {
                _db.DoctorFacilities.Add(new DoctorFacility
                {
                    Doctor = doctor,
                    Facility = f
                });
            });
        }

        public Specialization RemoveSpecialization(Doctor doctor, int SpecializationId)
        {
            return doctor.Specializations.Find(s => s.Id == SpecializationId);
        }

        public void AddSpecialization(Doctor doctor, Specialization specialization)
        {
            doctor.Specializations.Add(specialization);
        }

        public IEnumerable<Specialization> GetSpecializations(Doctor doctor)
        {
            return doctor.Specializations;
        }

        public void SetSpecializations(Doctor doctor, List<Specialization> specializations)
        {
            if(doctor.Id != 0)
            {
                doctor = GetById(doctor.Id);
            }
            doctor.Specializations.RemoveAll(s => true);
            doctor.Specializations = specializations;
        }

        public bool IsFacilityExist(Doctor doctor, Facility facility)
        {
            return _db.DoctorFacilities.Any(df => df.DoctorId == doctor.Id && df.FacilityId == facility.Id);
        }

        public void Delete(int id)
        {
            Doctor doctor = _db.Doctors.Find(id);
            _db.Doctors.Remove(doctor);
        }

        public IEnumerable<Doctor> GetAll()
        {
            return _db.Doctors
                .Include(d => d.DoctorFacilities)
                .Include(d => d.Specializations);
        }

        public Doctor GetById(int id)
        {
            return _db.Doctors
                .Include(d => d.DoctorFacilities)
                .Include(d => d.Specializations)
                .SingleOrDefault(d => d.Id == id);
        }

        public IEnumerable<Doctor> GetByIds(params int[] ids)
        {
            return _db.Doctors
                .Include(d => d.DoctorFacilities)
                .Include(d => d.Specializations)
                .Where(d => ids.Contains(d.Id));
        }

        public void Insert(Doctor entity)
        {
            _db.Doctors.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Doctor entity)
        {
            _db.Doctors.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
