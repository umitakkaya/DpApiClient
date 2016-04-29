using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class DoctorFacilityRepository : IDisposable
    {
        private HospitalContext _db { get; set; }

        public DoctorFacilityRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(int doctorId, int facilityId)
        {
            _db.DoctorFacilities.Remove(GetByIds(doctorId, facilityId));
        }

        public DoctorFacility GetByIds(int doctorId, int facilityId)
        {
            return _db.DoctorFacilities.Find(doctorId, facilityId);
        }

        public IEnumerable<DoctorFacility> GetAll()
        {
            return _db.DoctorFacilities
                .Include(df => df.Doctor)
                .Include(df => df.Facility)
                .Include(df => df.DoctorMapping);
        }

        public IEnumerable<DoctorFacility> GetMapped()
        {
            return _db.DoctorFacilities
                .Include(df => df.Doctor)
                .Include(df => df.Facility)
                .Include(df => df.DoctorMapping)
                .Where(df => df.DoctorMapping != null);
        }

        public IEnumerable<DoctorFacility> GetNotMapped()
        {
            return _db.DoctorFacilities
                .Include(df => df.Doctor)
                .Include(df => df.Facility)
                .Include(df => df.DoctorMapping)
                .Where(df=>df.DoctorMapping == null);
        }

        public IEnumerable<DoctorFacility> GetByDoctorId(int doctorId)
        {
            return _db.DoctorFacilities
                .Include(df => df.Doctor)
                .Include(df => df.Facility)
                .Include(df => df.DoctorMapping)
                .Where(df => df.DoctorId == doctorId);
        }

        public IEnumerable<DoctorFacility> GetByFacilityId(int facilityId)
        {
            return _db.DoctorFacilities
                .Include(df => df.Doctor)
                .Include(df => df.Facility)
                .Include(df => df.DoctorMapping)
                .Where(df => df.FacilityId == facilityId);
        }

        public IEnumerable<Doctor> GetAllDoctors()
        {
            return _db.DoctorFacilities.Select(df => df.Doctor).Distinct();
        }

        public IEnumerable<Facility> GetAllFacilities()
        {
            return _db.DoctorFacilities.Select(df => df.Facility).Distinct();
        }

        public void Insert(DoctorFacility entity)
        {
            _db.DoctorFacilities.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(DoctorFacility entity)
        {
            _db.DoctorFacilities.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
