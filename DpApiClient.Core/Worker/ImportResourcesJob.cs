using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core.Worker
{
    public class ImportResourcesJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ImportManager.Import();
        }
    }
}
