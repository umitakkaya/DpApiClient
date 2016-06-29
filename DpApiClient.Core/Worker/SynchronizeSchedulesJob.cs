using Common.Logging;
using DpApiClient.Data;
using DpApiClient.Data.Repositories;
using DpApiClient.REST.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core.Worker
{
    public class SynchronizeSchedulesJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SynchronizeSchedulesJob));

        public void Execute(IJobExecutionContext context)
        {
            var db = new HospitalContext();
            var client = new DpApi(AppSettings.ClientId, AppSettings.ClientSecret, (Locale)AppSettings.Locale);

            var scheduleManager = new ScheduleManager(db, client);
            var repo = new DoctorFacilityRepository(db);

            var mappedDoctors = repo.GetMapped();

            foreach (var item in mappedDoctors)
            {
                bool clearResult = scheduleManager.ClearDPCalendar(item);
                bool pushResult = scheduleManager.PushSlots(item);

                log.Warn($"Cleaning slot result: {clearResult}");
                log.Warn($"Push slot result: {pushResult}");
            }

            repo.Dispose();
        }
    }
}
