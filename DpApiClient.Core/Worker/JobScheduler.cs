using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core.Worker
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail importJob = JobBuilder.Create<ImportResourcesJob>().Build();
            ITrigger importTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(3)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(6, 0))
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 0))
                  )
                .Build();
            scheduler.ScheduleJob(importJob, importTrigger);

            IJobDetail syncJob = JobBuilder.Create<SynchronizeSchedulesJob>().Build();
            ITrigger syncTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(5)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 59))
                  )
                .Build();
            scheduler.ScheduleJob(syncJob, syncTrigger);

            IJobDetail pullJob = JobBuilder.Create<PullNotificationJob>().Build();
            ITrigger pullTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInMinutes(5)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 59))
                  )
                .Build();

            scheduler.ScheduleJob(pullJob, pullTrigger);
        }
    }
}
