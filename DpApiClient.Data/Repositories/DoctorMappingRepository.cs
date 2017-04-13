using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class DoctorMappingRepository : IDisposable
    {
        private HospitalContext _db { get; set; }

        public DoctorMappingRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public IEnumerable<DoctorMapping> GetAll()
        {
            return _db.DoctorMappings
                .Include(dm=>dm.DoctorFacility)
                .Include(dm=>dm.ForeignAddress);
        }

        public DoctorMapping GetByForeignAddress(string foreignAddressId)
        {
            return _db.DoctorMappings
                .Include(dm => dm.DoctorFacility)
                .Include(dm => dm.ForeignAddress)
                .SingleOrDefault(dm => dm.ForeignAddress.Id == foreignAddressId);
        }

        public IEnumerable<DoctorMapping> GetByFacility(int facilityId)
        {
            return _db.DoctorMappings
                .Include(dm => dm.DoctorFacility)
                .Include(dm => dm.ForeignAddress)
                .Where(dm => dm.FacilityId == facilityId);
        }

        public IEnumerable<DoctorMapping> GetByDoctor(int doctorId)
        {
            return _db.DoctorMappings
                .Include(dm => dm.DoctorFacility)
                .Include(dm => dm.ForeignAddress)
                .Where(dm => dm.DoctorId == doctorId);
        }

        public DoctorMapping GetByAddressAndDoctorFacility(ForeignAddress address, DoctorFacility doctorFacility)
        {
            return _db.DoctorMappings.SingleOrDefault(dm =>
                dm.ForeignAddress.Id == address.Id
                &&
                dm.DoctorId == doctorFacility.DoctorId
                &&
                dm.FacilityId == doctorFacility.FacilityId
            );
        }

        public DoctorMapping Map(ForeignAddress address, DoctorFacility doctorFacility, ForeignDoctorService foreignDoctorService)
        {
            var mapping = GetByAddressAndDoctorFacility(address, doctorFacility);

            if (mapping == null)
            {
                return _db.DoctorMappings.Add(new DoctorMapping()
                {
                    DoctorId = doctorFacility.DoctorId,
                    FacilityId = doctorFacility.FacilityId,
                    ForeignAddress = address,
                    ForeignDoctorService = foreignDoctorService
                });
            }

            return mapping;
        }

        public void RemoveMapping(ForeignAddress address, DoctorFacility doctorFacility)
        {
            var mapping = GetByAddressAndDoctorFacility(address, doctorFacility);
            if (mapping == null)
            {
                return;
            }

            _db.DoctorMappings.Remove(mapping);
        }

        public void Insert(DoctorMapping mapping)
        {
            _db.DoctorMappings.Add(mapping);
        }

        public void Update(DoctorMapping mapping)
        {
            _db.DoctorMappings.Attach(mapping);
            _db.Entry(mapping).State = EntityState.Modified;
        }

        public void Delete(DoctorMapping mapping)
        {
            _db.DoctorMappings.Remove(mapping);
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
