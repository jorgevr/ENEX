using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace ENEXWinService.Quartz
{
    public class ENEXJob
    {
        public string Name { get; set; }
        public ITrigger Trigger { get; set; }
        public IJobDetail JobDetail { get; set; }
    }
}
