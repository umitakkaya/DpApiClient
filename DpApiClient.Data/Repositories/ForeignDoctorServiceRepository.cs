using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DpApiClient.Data.Repositories
{
    public class ForeignDoctorServiceRepository : IForeignRepository<ForeignDoctorService>
    {
        private HospitalContext _db { get; set; }
        public ForeignDoctorServiceRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(string id)
        {
            _db.ForeignDoctorServices.Remove(GetById(id));
        }

        public IEnumerable<ForeignDoctorService> GetAll()
        {
            return _db.ForeignDoctorServices.Include(fds => fds.ForeignDoctor);
        }

        public ForeignDoctorService GetById(string id)
        {
            return _db.ForeignDoctorServices
                .Include(fds => fds.ForeignDoctor)
                .Include(fds => fds.DoctorSchedules)
                .Include(fds => fds.Visits)
                .SingleOrDefault(fds => fds.Id == id);
        }

        public IEnumerable<ForeignDoctorService> GetByIds(params string[] ids)
        {
            return _db.ForeignDoctorServices
                .Include(fds => fds.ForeignDoctor)
                .Include(fds => fds.DoctorSchedules)
                .Include(fds => fds.Visits)
                .Where(fds => ids.Contains(fds.Id));
        }

        public IEnumerable<ForeignDoctorService> GetByForeignDoctorId(string foreignDoctorId)
        {
            return _db.ForeignDoctorServices
                .Include(fds => fds.ForeignDoctor)
                .Include(fds => fds.DoctorSchedules)
                .Include(fds => fds.Visits)
                .Where(fds => fds.ForeignDoctorId == foreignDoctorId);
        }

        public void InsertOrUpdate(ForeignDoctorService entity)
        {
            ForeignDoctorService doctorService = _db.ForeignDoctorServices.AsNoTracking().SingleOrDefault(fds=> fds.Id == entity.Id);

            if (doctorService == null)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }


        public void Insert(ForeignDoctorService entity)
        {
            _db.ForeignDoctorServices.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(ForeignDoctorService entity)
        {
            _db.ForeignDoctorServices.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }


    }
}
