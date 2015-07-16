using System;
using System.ServiceProcess;
using log4net;
using ENEXWinService.Quartz;
using System.Configuration;
using ENEXWinService.Configuration;

namespace ENEXWinService
{
    public partial class ENEXService : ServiceBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ENEXService));

        private static ICronDispatcher _dispatcher;

        public ENEXService(ICronDispatcher dispatcher)
        {
            InitializeComponent();
            _dispatcher = dispatcher;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Info("STARTED");
                initDispatcher();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception while launching dispatcher: {0}\nStackTrace: \n{1}\n", e.Message, e.StackTrace);
            }
        }

        protected override void OnStop()
        {
            _logger.Info("STOPPED");
            if (_dispatcher != null)
                _dispatcher.Dispose();

            base.OnStop();
        }

        private void initDispatcher()
        {
            _dispatcher.Init();
        }
    }
}
