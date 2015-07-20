using ENEXWinService.Configuration;
using log4net;
using Quartz;
using ENEXWinService.Services;
using ENEXWinService.Ftp;
using ENEXWinService.FileExtracter;

namespace ENEXWinService.Quartz
{
    class MeasuresJobHelper : IENEXJobHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MeasuresJobHelper));

        private static IConfigurationProvider _configProvider;
        private static IMeasureService _service;
       // private static IFtpClient _ftpClient;
        private static IMeasureFileExtracter _measureFileExtracter;
        private static IPlantPowerFileExtracter _plantPowerFileExtracter;
        private static IPlantPowerService _plantPowerService;
        private static IPlantService _plantService;

        
        public MeasuresJobHelper(IConfigurationProvider configProvider, IMeasureService service, IMeasureFileExtracter measureFileExtracter, IPlantPowerService plantPowerService, IPlantPowerFileExtracter plantPowerFileExtracter, IPlantService plantService)
        {
            _configProvider = configProvider;
            _service = service;
            //_ftpClient = ftpClient;
            _measureFileExtracter = measureFileExtracter;
            _plantPowerService = plantPowerService;
            _plantPowerFileExtracter = plantPowerFileExtracter;
            _plantService = plantService;
        }

         
        public ENEXJob GetJob()
        {
            var result = new ENEXJob();

            result.Name = "Measures ENEX Loader Job";
            result.JobDetail = createJobDetail();
            result.Trigger = createTrigger();

            return result;
        }

        private ITrigger createTrigger()
        {
            _logger.Info("Preparing  Measures ENEX Loader Trigger");
            var jobDataMap = getJobData();
            var cronExpression = _configProvider.GetHourlyCronExpression();
            var result = getTrigger(jobDataMap, cronExpression);
            _logger.Info("File  Measures ENEX Loader Trigger prepared");
            return result;
        }

        private JobDataMap getJobData()
        {
            _logger.Info("Preparing Measures ENEX Loader Job Data");

            JobDataMap result = new JobDataMap();
            result.Put("MeasureService", _service);
            result.Put("ConfigurationProvider", _configProvider);
            // result.Put("FtpClient", _ftpClient);
            result.Put("MeasureFileExtracter", _measureFileExtracter);
            result.Put("PlantPowerService", _plantPowerService);
            result.Put("PlantPowerFileExtracter", _plantPowerFileExtracter);
            result.Put("PlantService", _plantService);

            _logger.Info("Done preparing Measures ENEX Loader Job Data");
            return result;
        }

        private ITrigger getTrigger(JobDataMap jobDataMap, string cronExpression)
        {
            return TriggerBuilder.Create()
                .WithCronSchedule(cronExpression, x => x.WithMisfireHandlingInstructionDoNothing())
                .UsingJobData(jobDataMap)
                .Build();
        }

        private IJobDetail createJobDetail()
        {
            _logger.Info("Preparing Measures ENEX Loader JobDetail");

            var result = JobBuilder.Create()
                .OfType(typeof(MeasuresLoaderJob))
                .Build();

            _logger.Info("JobDetail Measures Loader prepared");
            return result;
        }
    }
}
