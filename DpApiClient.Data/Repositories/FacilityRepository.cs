using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class FacilityRepository : IRepository<Facility>
    {
        private HospitalContext _db { get; set; }
        public FacilityRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public IEnumerable<DoctorFacility> GetDoctorFacilities(Facility facility)
        {
            return _db.DoctorFacilities.Where(df => df.FacilityId == facility.Id);
        }

        public IEnumerable<Doctor> GetDoctors(Facility facility)
        {
            return GetDoctors(facility.Id);
        }

        public IEnumerable<Doctor> GetDoctors(int id)
        {
            return _db.DoctorFacilities.Where(df => df.FacilityId == id).Select(df => df.Doctor);
        }

        public DoctorFacility AddDoctor(Facility facility, Doctor doctor)
        {
            return _db.DoctorFacilities.Add(new DoctorFacility()
            {
                Doctor = doctor,
                Facility = facility
            });

        }

        public void RemoveDoctor(Facility facility, Doctor doctor)
        {
            var doctorFacility = _db.DoctorFacilities.SingleOrDefault(df=> df.DoctorId == doctor.Id && df.FacilityId == facility.Id);

            _db.DoctorFacilities.Remove(doctorFacility);

        }

        public void SetDoctors(Facility facility, List<Doctor> doctors)
        {
            _db.DoctorFacilities.RemoveRange(GetDoctorFacilities(facility));

            doctors.ForEach(d =>
            {
                _db.DoctorFacilities.Add(new DoctorFacility
                {
                    Facility = facility,
                    Doctor = d
                });
            });
        }

        public bool IsDoctorExist(Facility facility, Doctor doctor)
        {
            return _db.DoctorFacilities.Any(df => df.DoctorId == doctor.Id && df.FacilityId == facility.Id);
        }

        public void Delete(int id)
        {
            _db.Facilities.Remove(_db.Facilities.Find(id));
        }

        public IEnumerable<Facility> GetAll()
        {
            return _db.Facilities.Include(f => f.ParentFacility);
        }

        public Facility GetById(int id)
        {
            return _db.Facilities
                .Include(f => f.Branches)
                .Include(f => f.DoctorFacilities)
                .Include(f => f.ParentFacility)
                .SingleOrDefault(d => d.Id == id);
        }

        public IEnumerable<Facility> GetByIds(params int[] ids)
        {
            return _db.Facilities
                .Include(f => f.Branches)
                .Include(f => f.DoctorFacilities)
                .Include(f => f.ParentFacility)
                .Where(d => ids.Contains(d.Id));
        }

        public void Insert(Facility entity)
        {
            _db.Facilities.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Facility entity)
        {
            _db.Facilities.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
