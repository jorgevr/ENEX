using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENEXWinService.Model
{
    public class Measure
    {
        public string Plant { get; set; }
        public string Source { get; set; }
        public string DataVariable { get; set; }
        public string Resolution { get; set; }
        public string UtcDate { get; set; }
        public int UtcHour { get; set; }
        public int UtcMinute { get; set; }
        public int UtcSecond { get; set; }
        public double Value { get; set; }
        public double MeasurePercentage { get; set; }
        public int ReliabilityType { get; set; }

        public Measure(string plant, string source, string datavariable, string utcdate
            , int utchour, int utcminute, int utcsecond, double value, double percentage
            , int reliability, string resolution)
        {
            Plant = plant;
            Source = source;
            DataVariable = datavariable;
            UtcDate = utcdate;
            UtcHour = utchour;
            UtcMinute = utcminute;
            UtcSecond = utcsecond;
            Value = value;
            MeasurePercentage = percentage;
            ReliabilityType = reliability;
            Resolution = resolution;
        }
    }
}
