using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using ENEXWinService.Model;

namespace ENEXWinService.Processers
{
    public interface IMeasureProcesser
    {
        Measure GetHourlyEnergyMeasure(IList<Measure> sourceMeasures, string hourMeasureSource, string hourDataVariable);
    }

    public class MeasureProcesser : IMeasureProcesser
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MeasureProcesser));

        public Measure GetHourlyEnergyMeasure(IList<Measure> sourceMeasures, string hourMeasureSource, string hourDataVariable)
        {
            var dataVariable = hourDataVariable;
            var measurePercentage = 1;
            var plant = sourceMeasures.First().Plant;
            var reliability = 0;
            var resolution = getHourResolution();
            var source = hourMeasureSource;
            var utcDate = sourceMeasures.First().UtcDate;
            var utcHour = sourceMeasures.First().UtcHour;
            var utcMinute = 00;
            var utcSecond = 00;
            var value = getValue(sourceMeasures);
            return new Measure(plant, source, dataVariable, utcDate, utcHour, utcMinute, utcSecond, value, measurePercentage, reliability, resolution);
        }

        private string getHourResolution()
        {
            return "1H";
        }

        private double getValue(IList<Measure> sourceMeasures)
        {
            var orderedMeasures = sourceMeasures.OrderBy(x => x.UtcMinute).ToList();

            int cont = 0;
            double value = 0;
            while (cont < orderedMeasures.Count)
            {
                var currentMeasure = orderedMeasures[cont];

                var minute = currentMeasure.UtcMinute;
                var nextMinute = 60;
                if (orderedMeasures.Count > cont + 1)
                    nextMinute = orderedMeasures[cont + 1].UtcMinute;

                value += currentMeasure.Value * (nextMinute - minute);

                cont++;
            }

            value = value / 60;

            return value;
        }
    }
}
