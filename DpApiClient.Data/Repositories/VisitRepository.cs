using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DpApiClient.Data.Repositories
{
    public class VisitRepository : IRepository<Visit>
    {
        private HospitalContext _db { get; set; }
        public VisitRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(int id)
        {
            _db.Visits.Remove(GetById(id));
        }

        

        public IEnumerable<Visit> GetAll()
        {
            return _db.Visits
                .Include(v => v.DoctorFacility)
                .Include(v => v.DoctorSchedule)
                .Include(v => v.VisitPatient);
        }

        public Visit GetById(int id)
        {
            return _db.Visits
                .Include(v => v.DoctorFacility)
                .Include(v => v.DoctorSchedule)
                .Include(v => v.VisitPatient)
                .SingleOrDefault(v=>v.Id == id);
        }

        public Visit GetByForeignId(string id)
        {
            return _db.Visits
                .Include(v => v.DoctorFacility)
                .Include(v => v.DoctorSchedule)
                .Include(v => v.VisitPatient)
                .SingleOrDefault(v => v.ForeignVisitId == id);
        }

        public IEnumerable<Visit> GetByIds(params int[] ids)
        {
            return _db.Visits
                .Include(v => v.DoctorFacility)
                .Include(v => v.DoctorSchedule)
                .Include(v => v.VisitPatient)
                .Where(v=> ids.Contains(v.Id));
        }

        public void Insert(Visit entity)
        {
            _db.Visits.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Visit entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
