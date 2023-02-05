using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Price
    {
        public string InstrumentCode { get; set; }
        public DateTime PriceDate { get; set; }
        public double PriceClose { get; set; } 
    }
}
