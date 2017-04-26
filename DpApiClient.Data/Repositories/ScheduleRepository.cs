using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Data.Repositories
{
    public class ScheduleRepository : IRepository<DoctorSchedule>
    {
        private HospitalContext _db { get; set; }

        public ScheduleRepository(HospitalContext db = null)
        {
            _db = db ?? new HospitalContext();
        }

        public void Delete(int id)
        {
            _db.DoctorSchedules.Remove(_db.DoctorSchedules.Find(id));
        }

        public IEnumerable<DoctorSchedule> GetByDoctorFacility(DoctorFacility doctorFacility)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds => ds.DoctorId == doctorFacility.DoctorId && ds.FacilityId == doctorFacility.FacilityId);
        }

        public IEnumerable<DoctorSchedule> GetByRangeAndDoctorFacility(DateTime startDate, DateTime endDate, int doctorId, int facilityId)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds =>
                    ds.DoctorId == doctorId &&
                    ds.FacilityId == facilityId &&
                    ds.Date >= DbFunctions.TruncateTime(startDate)
                    &&
                    ds.Date <= DbFunctions.TruncateTime(endDate)
                );
        }

        public IEnumerable<DoctorSchedule> GetByDateAndDoctorFacility(DateTime date, int doctorId, int facilityId)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds =>
                    ds.DoctorId == doctorId && ds.FacilityId == facilityId
                    &&
                    ds.Date == DbFunctions.TruncateTime(date)
                );
        }
        
        
        public IEnumerable<DoctorSchedule> GetByDateAndDoctorFacilityAsNoTracking(DateTime date, int doctorId, int facilityId)
        {
            return _db.DoctorSchedules
                .AsNoTracking()
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds =>
                    ds.DoctorId == doctorId && ds.FacilityId == facilityId
                    &&
                    ds.Date == DbFunctions.TruncateTime(date)
                );
        }

        public IEnumerable<DoctorSchedule> GetByDate(DateTime date)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds =>
                    ds.Date == DbFunctions.TruncateTime(date)
                );
        }

        public IEnumerable<DoctorSchedule> GetAll()
        {
            return _db.DoctorSchedules
                .Include(d => d.DoctorFacility).
                Include(d => d.ForeignDoctorService);
        }

        public DoctorSchedule GetById(int id)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .SingleOrDefault(ds => ds.Id == id);
        }

        public DoctorSchedule GetByIdAsNonTracking(int id)
        {
            return _db.DoctorSchedules
                .AsNoTracking()
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .SingleOrDefault(ds => ds.Id == id);
        }

        public IEnumerable<DoctorSchedule> GetByIds(params int[] ids)
        {
            return _db.DoctorSchedules
                .Include(ds => ds.DoctorFacility)
                .Include(ds => ds.ForeignDoctorService)
                .Where(ds => ids.Contains(ds.Id));
        }


        public void Insert(DoctorSchedule entity)
        {
            _db.DoctorSchedules.Add(entity);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(DoctorSchedule entity)
        {
            if (_db.Entry(entity).State != EntityState.Modified)
            {
                _db.DoctorSchedules.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Attach(DoctorSchedule entity)
        {
            _db.DoctorSchedules.Attach(entity);
        }


        public DoctorSchedule Clone(DoctorSchedule entity)
        {
            var schedule = new DoctorSchedule()
            {
                Date = entity.Date,
                End = entity.End,
                FacilityId = entity.FacilityId,
                DoctorId = entity.DoctorId,
                Duration = entity.Duration,
                ForeignDoctorServiceId = entity.ForeignDoctorServiceId,
                IsFullfilled = false,
                Start = entity.Start
            };
            return schedule;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
