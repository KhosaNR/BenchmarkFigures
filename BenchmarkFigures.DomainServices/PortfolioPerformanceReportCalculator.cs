using BenchmarkFigures.DomainServices.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkFigures.DomainServices
{
    public static class PortfolioPerformanceReportCalculator //: IPortfolioPerformanceReportCalculator
    {
        static Dictionary<int, string>  numberOfMonthsToReport = new() {
            {1,"1 Month" },
            {3,"3 Months" },
            {6,"6 Months" },
            {12,"1 Year" },
            {24,"2 Years" },
            {36,"3 Years" },
            {60,"5 Years" },
            {84,"7 Years" },
            {120,"10 Years" } };
        public static PortfolioReport GeneratePortfolioReport(DateTime endDate, List<BenchmarkPerformance> benchmarkPerformances)
        {
            PortfolioReport PortfolioReport = new();
            foreach (var numberOfMonths in numberOfMonthsToReport.Keys)
            {
                PortfolioReport.Add(numberOfMonthsToReport[numberOfMonths],
                    GetPerformanceForMonths(endDate, benchmarkPerformances, numberOfMonths));
            }
            return PortfolioReport;

        }

        public static double GetPerformanceForMonths(DateTime endDate, List<BenchmarkPerformance> benchmarkPerformances, int numberOfMonths)
        {

            var dataWithinRange = benchmarkPerformances
                .Where(x => DateTime.Compare(x.DateTo,endDate.AddMonths(-numberOfMonths+1)) >= 0
                    &&
                    DateTime.Compare(x.DateTo, endDate) <= 0
                ).ToList();
            double average = dataWithinRange.Count>0?  1 : 0;
            foreach (var performance in dataWithinRange)
            {
                average = ((double)performance.Performance / 100 + 1) * average;
            }

            average = (average - 1) * 100;

            return average;
        }
    }
}
