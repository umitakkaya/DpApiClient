using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class ForeignDoctorRepository : IForeignRepository<ForeignDoctor>
    {
        private HospitalContext _db { get; set; }
        public ForeignDoctorRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(string id)
        {
            _db.ForeignDoctors.Remove(GetById(id));
        }

        public void SetSpecializations(ForeignDoctor doctor, List<ForeignSpecialization> specializations)
        {
            doctor.ForeignSpecializations.RemoveAll(fs => true);
            doctor.ForeignSpecializations.AddRange(specializations);
        }

        public IEnumerable<ForeignDoctor> GetAll()
        {
            return _db.ForeignDoctors;
        }

        public ForeignDoctor GetById(string id)
        {
            return _db.ForeignDoctors
                .Include(fd => fd.ForeignAddresses)
                .Include(fd => fd.ForeignDoctorServices)
                .Include(fd => fd.ForeignSpecializations)
                .SingleOrDefault(fd => fd.Id == id);
        }

        public IEnumerable<ForeignDoctor> GetByIds(params string[] ids)
        {
            return _db.ForeignDoctors
                .Include(fd => fd.ForeignAddresses)
                .Include(fd => fd.ForeignDoctorServices)
                .Include(fd => fd.ForeignSpecializations)
                .Where(fds => ids.Contains(fds.Id));
        }

        public void Insert(ForeignDoctor entity)
        {
            _db.ForeignDoctors.Add(entity);
        }

        public void InsertOrUpdate(ForeignDoctor entity)
        {
            ForeignDoctor doctor = _db.ForeignDoctors.AsNoTracking().SingleOrDefault(fd=>fd.Id == entity.Id);

            if (doctor == null)
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

        public void Update(ForeignDoctor entity)
        {
            _db.ForeignDoctors.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
