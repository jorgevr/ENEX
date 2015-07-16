using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENEXWinService.Model
{
    public class PlantPower
    {
        public double Power { get; set; }
        public string UtcInsertionDateTime { get; set; }
        public string Plant { get; set; }
        public string UtcUpdatedDateTime { get; set; }

        public PlantPower(string plant, string utcInsertionDateTime, string utcUpdatedDateTime, double power)
        {
            Plant = plant;
            UtcInsertionDateTime = utcInsertionDateTime;
            UtcUpdatedDateTime = utcUpdatedDateTime;
            Power = power;
        }
    }
}
