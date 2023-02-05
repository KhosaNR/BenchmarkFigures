using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BenchmarkFigures.DomainServices
{
    public class PortfolioReport: Dictionary<string, double>
    {
        //Key is numberOfMonths, Value is performance for that portfolio
        public string GenerateCsvString(string delimeter = ",")
        {
            var csv = new StringBuilder();
            var newLine = "";
            foreach (var item in this.Keys)
            {
                newLine += String.IsNullOrEmpty(newLine) ? item.ToString() : $"{delimeter}{item.ToString()}";
            }
            if(!String.IsNullOrEmpty(newLine))
                csv.AppendLine(newLine);

            newLine = "";
            foreach (var item in this.Values)
            {
                newLine += String.IsNullOrEmpty(newLine) ? item.ToString() : $"{delimeter}{item.ToString()}";
            }
            if (!String.IsNullOrEmpty(newLine))
                csv.AppendLine(newLine);

            return csv.ToString();
        }

    }
}
