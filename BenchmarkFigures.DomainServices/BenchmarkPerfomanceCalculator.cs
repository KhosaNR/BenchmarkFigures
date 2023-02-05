using BenchmarkFigures.DomainServices.Interfaces;
using Domain.Models;
using Domain.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkFigures.DomainServices
{
    public static class BenchmarkPerformanceCalculator //: IBenchmarkPerformanceCalcultor
    {

        public static List<BenchmarkPerformance> GenerateBenchmarkPerformances(List<PriceDifferenceDto> priceListDifferences)
        {
            List<BenchmarkPerformance> benchmarkPerformances = new List<BenchmarkPerformance>();
            var yearsAndMonth = priceListDifferences
                //.Where(price => price.PriceDate.Year == 2018 && price.PriceDate.Month == 12)
                .OrderBy(y => y.PriceDate)
                .GroupBy(x => x.PriceDate).Select(p => p.Key.ToString("yyyy/MM")).Distinct().ToList();
            foreach (var date in yearsAndMonth)
            {
                var benchmarksPerformancesForMonth = GenerateBenchmarksForMonth(priceListDifferences, date);
                benchmarkPerformances.AddRange(benchmarksPerformancesForMonth);

            }
            return benchmarkPerformances.OrderBy(x=>x.DateTo).ToList();
        }

        private static List<BenchmarkPerformance> GenerateBenchmarksForMonth(List<PriceDifferenceDto> priceListDifferences, string? date)
        {
            List<BenchmarkPerformance> benchmarkPerformances = new();
            var yearAndMonth = DateTime.ParseExact(date, "yyyy/MM",
                    System.Globalization.CultureInfo.InvariantCulture);
            var firstDayOfCurrentMonth = yearAndMonth;

            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
            var priceDifferenceListForMonthAndYear = priceListDifferences
                                                        .Where(x => x.PriceDate.Year == yearAndMonth.Year
                                                                   && x.PriceDate.Month == yearAndMonth.Month).ToList();

            var benchmarkCodesForMonth = priceDifferenceListForMonthAndYear.GroupBy(x => x.InstrumentCode).Select(p => p.Key).Distinct().ToList();
            foreach (var code in benchmarkCodesForMonth)
            {
                benchmarkPerformances.Add(GenerateBenchamarkPerformanceForMonthAndCode(priceDifferenceListForMonthAndYear, yearAndMonth.Year, yearAndMonth.Month, code));
            }

            return benchmarkPerformances;
        }

        public static BenchmarkPerformance GenerateBenchamarkPerformanceForMonthAndCode(List<PriceDifferenceDto> priceDifferenceListForMonthAndYear, int year, int month, string? benchmarkCode)
        {
            List<BenchmarkPerformance> benchmarkPerformances = new();
            double average = priceDifferenceListForMonthAndYear.Count > 0 ? 1 : 0;

            foreach (var price in priceDifferenceListForMonthAndYear.Where(x => x.InstrumentCode == benchmarkCode).ToList())
            {
                average = ((double)price.PriceDifferencePercentage / 100 + 1) * average;
            }

            average = (average - 1) * 100;

            BenchmarkPerformance benchmarkPerformance = new BenchmarkPerformance()
            {
                BenchmarkCode = benchmarkCode,
                DateFrom = DateTime.ParseExact($"{year}{month.ToString().PadLeft(2, '0')}01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                DateTo = DateTime.ParseExact($"{year}{month.ToString().PadLeft(2, '0')}{DateTime.DaysInMonth(year, month)}", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                Performance = average,
            };
            return benchmarkPerformance;
        }

        public static List<PriceDifferenceDto> CalculatePriceDifferences(List<Price> priceList)
        {
            priceList = priceList.OrderBy(x => x.PriceDate).ToList();
            List<PriceDifferenceDto> priceDifferenceList = new();

            for (int i = 1; i < priceList.Count; i++)
            {
                var previousDayPrice = priceList[i - 1];
                var previousDayPriceValue = previousDayPrice != null ? previousDayPrice.PriceClose : 0;
                var currentDayPriceValue = priceList[i].PriceClose;
                double priceChangePercentage = 1;

                if (previousDayPriceValue == 0 && currentDayPriceValue == 0)
                {
                    priceChangePercentage = 0;
                }
                else if (previousDayPriceValue != 0)
                {
                    priceChangePercentage = ((double)currentDayPriceValue / previousDayPriceValue - 1) * 100;
                }
                priceDifferenceList.Add(new PriceDifferenceDto()
                {
                    InstrumentCode = priceList[i].InstrumentCode,
                    PriceDate = priceList[i].PriceDate,
                    PriceDifferencePercentage = priceChangePercentage
                });
            }
            return priceDifferenceList.OrderBy(x=>x.PriceDate).ToList();
        }

        //public static List<BenchmarkPerformance> CreateLiveAndHistoryBenchmark(IBenchmarkPerformanceRepository benchmarkPerformanceRepository, string selectedBenchmarkCode, DateTime endDate, IPriceRepository priceRepository, DateTime benchmarkhistorySwitchDate)
        //{
        //    var liveBenchmarks = CreateLiveOrHistoricBenchmarkPerformances(selectedBenchmarkCode, DateTime.MinValue, endDate, benchmarkPerformanceRepository, priceRepository);
        //    var historicBenchmarks = CreateLiveOrHistoricBenchmarkPerformances(selectedBenchmarkCode, benchmarkhistorySwitchDate, endDate, benchmarkPerformanceRepository, priceRepository,true);
        //    var fullBenchmarkPerformances = liveBenchmarks.OrderBy(x => x.DateTo).ToList();
        //    fullBenchmarkPerformances.AddRange(historicBenchmarks);
        //    fullBenchmarkPerformances = fullBenchmarkPerformances.OrderBy(x => x.DateTo).ToList();
        //    return fullBenchmarkPerformances;
        //}

        public static List<BenchmarkPerformance> CreateLiveBenchmarkPerformances(string selectedBenchmarkCode, DateTime endDate, IPriceRepository priceRepository)//, out List<Price> priceList, out List<BenchmarkPerformance> liveBenchmarkPerformances)
        {
            var priceList = priceRepository.GetByInstrumentCodeAndEndDate(selectedBenchmarkCode,endDate);
            var priceListDifferences = BenchmarkPerformanceCalculator.CalculatePriceDifferences(priceList);
            var liveBenchmarkPerformances = BenchmarkPerformanceCalculator.GenerateBenchmarkPerformances(priceListDifferences);
            return liveBenchmarkPerformances;
        }

        public static List<BenchmarkPerformance> CreateLiveOrHistoricBenchmarkPerformances(string selectedBenchmarkCode, DateTime startDate, DateTime endDate,IBenchmarkPerformanceRepository benchmarkPerformanceRepository, IPriceRepository priceRepository, bool useHistoricData = false)
        {
            if (!useHistoricData)
            {
                return CreateLiveBenchmarkPerformances(selectedBenchmarkCode, endDate, priceRepository);
            }
            return benchmarkPerformanceRepository.GetByInstrumentCodeBetwenDates(selectedBenchmarkCode, startDate, endDate);
        }

        public static List<BenchmarkPerformance> CreateBenchmarkForOneOrMoreInstruments(IBenchmarkPerformanceRepository benchmarkPerformanceRepository, string initialBenchmarkCode, DateTime endDate, IPriceRepository priceRepository, SortedList<DateTime,Tuple<string,bool>> selectedBenchmarkCodes=null, bool useHistoricForInitial = false)
        {
            //Generates benchmarks for multiple scenarios
            //
            //SortedList<DateTime,Tuple<string,bool>> selectedBenchmarkCodes : SortedList<switchingStartDate,Tuple<BenchmarkCode,isHistoric>>
            //A list of sorted dates, specifying the start date for the benchmark code and if the benchmark uses historic data or not
            //
            //

            List<BenchmarkPerformance> benchmarkPerformances = new ();
            var currentBenchmarkCode = initialBenchmarkCode;
            List<BenchmarkPerformance> currentBenchmarks = new ();
            bool useHistoricData = useHistoricForInitial;
            DateTime currentStartDate = DateTime.MinValue;

            if (selectedBenchmarkCodes != null)
            {
                for (int i = 0; i < selectedBenchmarkCodes.Count; i++)
                {
                    var nextStartDate = selectedBenchmarkCodes.ElementAt(i).Key;
                    var currentEndDate = nextStartDate.AddDays(-1);
                    currentBenchmarks = CreateLiveOrHistoricBenchmarkPerformances(currentBenchmarkCode, currentStartDate, currentEndDate,benchmarkPerformanceRepository, priceRepository,useHistoricData);
                    benchmarkPerformances.AddRange(currentBenchmarks);
                    currentBenchmarkCode = selectedBenchmarkCodes.ElementAt(i).Value.Item1;
                    currentStartDate = nextStartDate;
                    useHistoricData = selectedBenchmarkCodes.ElementAt(i).Value.Item2;


                }
            }
            currentBenchmarks = CreateLiveOrHistoricBenchmarkPerformances(currentBenchmarkCode, currentStartDate, endDate, benchmarkPerformanceRepository, priceRepository, useHistoricData);
            benchmarkPerformances.AddRange(currentBenchmarks);

            return benchmarkPerformances;

        }
    }
}
