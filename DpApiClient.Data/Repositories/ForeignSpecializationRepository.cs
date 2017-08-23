using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DpApiClient.Data.Repositories
{
    public class ForeignSpecializationRepository : IForeignRepository<ForeignSpecialization>
    {
        private HospitalContext _db { get; set; }
        public ForeignSpecializationRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(string id)
        {
            _db.ForeignSpecializations.Remove(GetById(id));
        }



        public IEnumerable<ForeignSpecialization> GetAll()
        {
            return _db.ForeignSpecializations.Include(fs => fs.ForeignDoctor);
        }

        public ForeignSpecialization GetById(string id)
        {
            return _db.ForeignSpecializations
                .Include(fs => fs.ForeignDoctor)
                .SingleOrDefault(fs => fs.Id == id);
        }

        public ForeignSpecialization GetByIdAndDoctorId(string id, string doctorId)
        {
            return _db.ForeignSpecializations
                .Include(fs => fs.ForeignDoctor)
                .SingleOrDefault(fs => fs.Id == id && fs.ForeignDoctorId == doctorId);
        }

        public IEnumerable<ForeignSpecialization> GetByIds(params string[] ids)
        {
            return _db.ForeignSpecializations
                .Include(fs => fs.ForeignDoctor)
                .Where(fs => ids.Contains(fs.Id));
        }

        public void Insert(ForeignSpecialization entity)
        {
            _db.ForeignSpecializations.Add(entity);
        }

        public void InsertOrUpdate(ForeignSpecialization entity)
        {
            var specialization = _db.ForeignSpecializations.AsNoTracking()
                .SingleOrDefault(fs=>fs.Id == entity.Id && fs.ForeignDoctorId == entity.ForeignDoctorId);

            if (specialization == null)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(ForeignSpecialization entity)
        {
            _db.ForeignSpecializations.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
