using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENEXWinService.Configuration;
using ENEXWinService.Ninject;

namespace ENEXWinService.Quartz
{
    public interface IENEXJobFactory
    {
        IList<ENEXJob> GetENEXJobs();
    }

    class ENEXJobFactory : IENEXJobFactory
    {
        private static IConfigurationProvider _configProvider;

        public ENEXJobFactory(IConfigurationProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public IList<ENEXJob> GetENEXJobs()
        {
            var result = new List<ENEXJob>();

            foreach (var jobHelper in getJobHelpers())
            {
                var job = jobHelper.GetJob();
                result.Add(job);
            }

            return result;
        }

        private IList<IENEXJobHelper> getJobHelpers()
        {
            var result = new List<IENEXJobHelper>();
            result.Add(ENEXNijectModule.GetDependency<MeasuresJobHelper>());
            return result;
        }
    }
}
