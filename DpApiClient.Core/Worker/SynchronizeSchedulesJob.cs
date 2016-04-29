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

                Console.WriteLine("Cleaning slot result: {0}", clearResult);
                Console.WriteLine("Push slot result: {0}", pushResult);
            }

            repo.Dispose();
        }
    }
}
