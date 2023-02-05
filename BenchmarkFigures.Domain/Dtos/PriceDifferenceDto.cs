using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Objects
{
    public class PriceDifferenceDto
    {
        public string InstrumentCode { get; set; }
        public DateTime PriceDate { get; set; }
        public double PriceDifferencePercentage { get; set; }

    }
}
