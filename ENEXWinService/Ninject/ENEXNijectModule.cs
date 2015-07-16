using ENEXWinService.FileExtracter;
using ENEXWinService.Ftp;
using ENEXWinService.Quartz;
using ENEXWinService.WebApi;
using Ninject.Modules;
using Ninject;
using ENEXWinService.Processers;
using ENEXWinService.Configuration;
using ENEXWinService.Services;
using Renci.SshNet;

namespace ENEXWinService.Ninject
{
    class ENEXNijectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICronDispatcher>().To<Dispatcher>();
            Bind<IENEXJobFactory>().To<ENEXJobFactory>();
            Bind<IConfigurationProvider>().To<ConfigurationProvider>();
            Bind<IMeasureService>().To<MeasureService>();
            Bind<IPlantService>().To<PlantService>();
            Bind<IMeasureProcesser>().To<MeasureProcesser>();
            Bind<IMeasureFileExtracter>().To<MeasureFileExtracter>();
            Bind<IPlantPowerFileExtracter>().To<PlantPowerFileExtracter>();
            Bind<IPlantPowerService>().To<PlantPowerService>();
            Bind<IFtpInfo>().To<FtpInfo>();
            Bind<IFtpClient>().To<Sftp>();
            Bind<IWebApiInfo>().To<WebApiInfo>();
            Bind<IWebApiClient>().To<WebApiClient>();
        }

        public static T GetDependency<T>()
        {
            return new StandardKernel(new ENEXNijectModule()).Get<T>();
        }
    }
}
