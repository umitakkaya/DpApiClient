using DpApiClient.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core
{
    public class CalendarGenerator
    {
        public static List<CalendarBlock> MergeSchedules(IEnumerable<DoctorSchedule> schedules)
        {
            List<CalendarBlock> blocks = new List<CalendarBlock>();
            List<DoctorSchedule> processedSchedules = new List<DoctorSchedule>();
            CalendarBlock block = null;

            schedules = schedules.OrderBy(s => s.Date)
                .ThenBy(s=>s.Start);

            while(schedules.Except(processedSchedules).Count() != 0)
            {
                var schedule = schedules.Except(processedSchedules).First();
                processedSchedules.Add(schedule);

                var start = schedule.Date.Add(schedule.Start);
                var end = schedule.Date.Add(schedule.End);

                if (block == null)
                {
                    block = new CalendarBlock()
                    {
                        Start = start,
                        End = end,
                        Duration  = schedule.Duration,
                        DoctorService = schedule.ForeignDoctorService
                    };
                    blocks.Add(block);
                }

                else if (block.End == start && block.Duration == schedule.Duration && block.DoctorService == schedule.ForeignDoctorService)
                {
                    block.End = end;
                }
                else
                {
                    if(false == blocks.Contains(block))
                    {
                        blocks.Add(block);
                    }

                    block = new CalendarBlock()
                    {
                        Start = start,
                        End = end,
                        Duration = schedule.Duration,
                        DoctorService = schedule.ForeignDoctorService
                    };
                }

                if (false == blocks.Contains(block))
                {
                    blocks.Add(block);
                }
            }

            return blocks;
        }
    }
}
