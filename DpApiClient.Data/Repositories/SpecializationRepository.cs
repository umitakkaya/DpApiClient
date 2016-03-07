using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class SpecializationRepository : IRepository<Specialization>
    {
        private HospitalContext _db;

        public SpecializationRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(int id)
        {
            _db.Specializations.Remove(GetById(id));
        }

        public List<Doctor> GetDoctors(Specialization specialization)
        {
            return specialization.Doctors;
        }

        public IEnumerable<Specialization> GetAll()
        {
            return _db.Specializations;
        }

        public Specialization GetById(int id)
        {
            return _db.Specializations.Find(id);
        }

        public IEnumerable<Specialization> GetByIds(params int[] ids)
        {
            return _db.Specializations.Where(s => ids.Contains(s.Id));
        }

        public void Insert(Specialization entity)
        {
            _db.Specializations.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Specialization entity)
        {
            _db.Specializations.Attach(entity);
            _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
