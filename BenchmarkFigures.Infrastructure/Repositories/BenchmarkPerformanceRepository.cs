using BenchmarkFigures.DomainServices;
using Domain.Models;
using Infrastructure.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BenchmarkPerformanceRepository : CsvHelper<BenchmarkPerformance>, IBenchmarkPerformanceRepository
    {
        public BenchmarkPerformanceRepository(string fullFilePathName, string[] delimeter) : base(fullFilePathName, delimeter)
        {
        }

        public List<BenchmarkPerformance> GetAll()
        {
            return ReadFromCsvFileToList();
        }

        public List<BenchmarkPerformance> GetByInstrumentCode(string benchmarkCode)
        {
            return GetAll().Where(BenchmarkPerformance => BenchmarkPerformance.BenchmarkCode.ToUpper() == benchmarkCode.ToUpper()).ToList();
        }

        public List<BenchmarkPerformance> GetBetweenDates(DateTime priceDateFrom, DateTime priceDateTo)
        {
            return GetAll()
                .Where(BenchmarkPerformance => DateTime.Compare(BenchmarkPerformance.DateFrom, priceDateFrom) >= 0
                    && DateTime.Compare(BenchmarkPerformance.DateTo, priceDateTo) <= 0)
                .ToList();
        }

        public List<BenchmarkPerformance> GetByInstrumentCodeBetwenDates(string benchmarkCode, DateTime priceDateFrom, DateTime priceDateTo)
        {
            return GetByInstrumentCode(benchmarkCode)
                .Where(BenchmarkPerformance => DateTime.Compare(BenchmarkPerformance.DateFrom, priceDateFrom) >= 0
                    && DateTime.Compare(BenchmarkPerformance.DateTo, priceDateTo) <= 0)
                .ToList();
        }

        public List<BenchmarkPerformance> GetByInstrumentCodeAndEndDate(string benchmarkCode, DateTime endDate)
        {
            return GetByInstrumentCode(benchmarkCode)
                .Where(BenchmarkPerformance => DateTime.Compare(BenchmarkPerformance.DateTo, endDate) <= 0)
                .ToList();
        }
    }
}
