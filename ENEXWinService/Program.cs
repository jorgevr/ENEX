using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Ninject;
using ENEXWinService.Ninject;
using Common.Logging;

namespace ENEXWinService
{
    static class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            initLogger();

            try
            {
                var kernel = new StandardKernel(new ENEXNijectModule());
                var service = kernel.Get<ENEXService>();
                ServiceBase.Run(service);
            }
            catch (Exception e)
            {
                _logger.Error("Exception while launching ENEXService", e);
            }
        }

        private static void initLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
