using Domain.Models;

namespace BenchmarkFigures.DomainServices.Interfaces
{
    public interface IPortfolioPerformanceReportCalculator
    {
        double GetPerformanceForMonths(DateTime endDate, List<BenchmarkPerformance> benchmarkPerformances, int numberOfMonths);
        Dictionary<string, double> GetPortfolioReport(DateTime endDate, List<BenchmarkPerformance> benchmarkPerformances);
    }
}