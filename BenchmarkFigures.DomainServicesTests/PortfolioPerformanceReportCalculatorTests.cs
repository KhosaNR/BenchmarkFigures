using Microsoft.VisualStudio.TestTools.UnitTesting;
using BenchmarkFigures.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace BenchmarkFigures.DomainServices.Tests
{
    [TestClass()]
    public class PortfolioPerformanceReportCalculatorTests
    {
        [TestMethod()]
        public void GetPerformanceForMonthsTest()
        {
            var performancePercentages = new List<double>() { -0.1158075183, -0.1159420000, -3.7579796750, 0.6331973540, 0, 0.8689135733, 0, 0.1336693657, 0.4894687316, 0.2509228567, 2.3262662202, 0.1151076979 };
            int year = 2018;
            int numberOfMonths = 12;
            DateTime endDate = DateTime.Parse("2018/12/31");
            List<BenchmarkPerformance> benchmarkPerformances = new List<BenchmarkPerformance>();
            for(int i = 1; i<=12; i++)
            {
                BenchmarkPerformance benchmarkPerformance = new BenchmarkPerformance()
                {
                    BenchmarkCode = "JBO1",
                    DateFrom = DateTime.ParseExact($"{year}{i.ToString().PadLeft(2, '0')}01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    DateTo = DateTime.ParseExact($"{year}{i.ToString().PadLeft(2, '0')}{DateTime.DaysInMonth(year, i)}", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    Performance = performancePercentages[i-1],
                };
                benchmarkPerformances.Add(benchmarkPerformance);
            }

            var performanceForMonths = PortfolioPerformanceReportCalculator.GetPerformanceForMonths(endDate, benchmarkPerformances, numberOfMonths);
            
            Assert.AreEqual(0.7237980732, Math.Round(performanceForMonths,10));
        }
    }
}