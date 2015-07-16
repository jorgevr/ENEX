using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using log4net;
using ENEXWinService.Services;
using ENEXWinService.WebApi;


namespace ENEXWinService.Quartz
{
    public interface ICronDispatcher : IDisposable
    {
        void Init();
    }

    public class Dispatcher : ICronDispatcher
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Dispatcher));

        private static IScheduler _scheduler;
        private static IENEXJobFactory _jobFactory;

        public Dispatcher(IENEXJobFactory jobFactory)
        {
            _scheduler = new StdSchedulerFactory().GetScheduler();
            _jobFactory = jobFactory;
        }

        public void Init()
        {
            try
            {
                _logger.Info("Starting Dispatcher");
                init();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception while preparing jobs: {0}\nStackTrace: \n{1}\n", e.Message, e.StackTrace);
            }
        }

        private void init()
        {
            foreach (var ENEXJob in _jobFactory.GetENEXJobs())
            {
                scheduleENEXJob(ENEXJob);
            }
            _scheduler.Start();
        }

        private void scheduleENEXJob(ENEXJob ENEXJob)
        {
            _logger.InfoFormat("Scheduling job {0}", ENEXJob.Name);
            _scheduler.ScheduleJob(ENEXJob.JobDetail, ENEXJob.Trigger);
        }

        public void Dispose()
        {
            _logger.Info("Disposing Dispatcher");
            if (_scheduler != null)
            {
                _scheduler.Shutdown(false);
                _scheduler = null;
            }
        }
    }
}
