using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DpApiClient.Data.Repositories
{
    public class ForeignFacilityRepository : IForeignRepository<ForeignFacility>
    {
        private HospitalContext _db { get; set; }
        public ForeignFacilityRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(string id)
        {
            _db.ForeignFacilities.Remove(GetById(id));
        }

        public IEnumerable<ForeignFacility> GetAll()
        {
            return _db.ForeignFacilities
                .Include(ff => ff.ForeignAddresses);
        }

        public ForeignFacility GetById(string id)
        {
            return _db.ForeignFacilities
                .Include(ff => ff.ForeignAddresses)
                .SingleOrDefault(ff => ff.Id == id);
        }

        public IEnumerable<ForeignFacility> GetByIds(params string[] ids)
        {
            return _db.ForeignFacilities
                .Include(ff => ff.ForeignAddresses)
                .Where(ff => ids.Contains(ff.Id));
        }

        public void InsertOrUpdate(ForeignFacility entity)
        {
            ForeignFacility facility = _db.ForeignFacilities.AsNoTracking().SingleOrDefault(ff=> ff.Id == entity.Id);

            if (facility == null)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public void Insert(ForeignFacility entity)
        {
            _db.ForeignFacilities.Add(entity);
        }

        public void Update(ForeignFacility entity)
        {
            _db.ForeignFacilities.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
