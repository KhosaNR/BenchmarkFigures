using Domain.Models;

namespace BenchmarkFigures.DomainServices
{
    public interface IBenchmarkPerformanceRepository
    {
        List<BenchmarkPerformance> GetAll();
        List<BenchmarkPerformance> GetBetweenDates(DateTime priceDateFrom, DateTime priceDateTo);
        List<BenchmarkPerformance> GetByInstrumentCode(string benchmarkCode);
        List<BenchmarkPerformance> GetByInstrumentCodeAndEndDate(string benchmarkCode, DateTime endDate);
        List<BenchmarkPerformance> GetByInstrumentCodeBetwenDates(string benchmarkCode, DateTime priceDateFrom, DateTime priceDateTo);
    }
}