using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DpApiClient.Data.Repositories
{
    public class ForeignAddressRepository : IForeignRepository<ForeignAddress>
    {
        private HospitalContext _db { get; set; }
        public ForeignAddressRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(string id)
        {
            _db.ForeignAddresses.Remove(GetById(id));
        }



        public IEnumerable<ForeignAddress> GetAll()
        {
            return _db.ForeignAddresses
                .Include(fa => fa.BookingExtraFields)
                .Include(fa => fa.DoctorMapping)
                .Include(fa => fa.ForeignDoctor)
                .Include(fa => fa.ForeignFacility);
        }

        public ForeignAddress GetById(string id)
        {
            return _db.ForeignAddresses
                .Include(fa => fa.BookingExtraFields)
                .Include(fa => fa.DoctorMapping)
                .Include(fa => fa.ForeignDoctor)
                .Include(fa => fa.ForeignFacility)
                .SingleOrDefault(fa => fa.Id == id);

        }

        public IEnumerable<ForeignAddress> GetByIds(params string[] ids)
        {
            return _db.ForeignAddresses
                .Include(fa => fa.BookingExtraFields)
                .Include(fa => fa.DoctorMapping)
                .Include(fa => fa.ForeignDoctor)
                .Include(fa => fa.ForeignFacility)
                .Where(fa => ids.Contains(fa.Id));
        }

        public void Insert(ForeignAddress entity)
        {
            _db.ForeignAddresses.Add(entity);
        }

        public void InsertOrUpdate(ForeignAddress entity)
        {
            ForeignAddress address = _db.ForeignAddresses.AsNoTracking().SingleOrDefault(fa=> fa.Id == entity.Id);
            if (address == null)
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

        public void Update(ForeignAddress entity)
        {
            _db.ForeignAddresses.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
