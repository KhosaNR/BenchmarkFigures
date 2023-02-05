using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BenchmarkPerformance
    {
        public string BenchmarkCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime  DateTo { get; set; }
        public double Performance { get; set; }
    }
}
