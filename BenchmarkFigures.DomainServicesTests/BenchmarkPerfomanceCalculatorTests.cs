using Microsoft.VisualStudio.TestTools.UnitTesting;
using BenchmarkFigures.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Objects;

namespace BenchmarkFigures.DomainServices.Tests
{
    [TestClass()]
    public class BenchmarkPerformanceCalculatorTests
    {
        [TestMethod()]
        public void CalculatePriceDifferencesTest()
        {
            List<Price> priceList= new List<Price>();
            priceList.Add(new Price() { InstrumentCode = "JB01", PriceClose = 21.89, PriceDate = DateTime.Today.AddDays(-3) });
            priceList.Add( new Price() { InstrumentCode = "JB01", PriceClose = 21.89, PriceDate = DateTime.Today.AddDays(-2) });
            priceList.Add(new Price() { InstrumentCode = "JB01", PriceClose = 89.13, PriceDate = DateTime.Today.AddDays(-1) });
            priceList.Add(new Price() { InstrumentCode = "JB01", PriceClose = 74.23, PriceDate = DateTime.Today });

            var priceDifferenceList = BenchmarkPerformanceCalculator.CalculatePriceDifferences(priceList);
            Assert.AreEqual(0, priceDifferenceList[0].PriceDifferencePercentage);
            Assert.AreEqual(307.1722247602, Math.Round(priceDifferenceList[1].PriceDifferencePercentage,10));
            Assert.AreEqual(-16.7171547178, Math.Round(priceDifferenceList[2].PriceDifferencePercentage,10));
        }

        [TestMethod()]
        public void GenerateBenchamarkPerformanceForMonthAndCodeTest()
        {
            DateTime firstDay = DateTime.Parse("2022/01/01");
            var priceDifferences = new List<PriceDifferenceDto>();
            priceDifferences.Add(new PriceDifferenceDto() { InstrumentCode = "JB02", PriceDifferencePercentage = 307.1722247602, PriceDate = firstDay.AddDays(-1) });
            priceDifferences.Add(new PriceDifferenceDto() { InstrumentCode = "JB01", PriceDifferencePercentage = 307.1722247602, PriceDate = firstDay });
            priceDifferences.Add(new PriceDifferenceDto() { InstrumentCode = "JB01", PriceDifferencePercentage = -16.7171547178, PriceDate = firstDay.AddDays(1) });
            priceDifferences.Add(new PriceDifferenceDto() { InstrumentCode = "JB01", PriceDifferencePercentage = -86.3262831739, PriceDate = firstDay.AddDays(2) });
            priceDifferences.Add(new PriceDifferenceDto() { InstrumentCode = "JB01", PriceDifferencePercentage = 19.7044334975,  PriceDate = firstDay.AddDays(3) });

            var benchmarkPerformance = BenchmarkPerformanceCalculator
                                .GenerateBenchamarkPerformanceForMonthAndCode(priceDifferences, firstDay.Year, firstDay.AddDays(1).Month, "JB01");

            Assert.AreEqual(-44.4952032891, Math.Round(benchmarkPerformance.Performance,10));
        }
    }
}