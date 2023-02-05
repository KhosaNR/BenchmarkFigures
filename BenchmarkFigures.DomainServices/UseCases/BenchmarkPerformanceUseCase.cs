//using BenchmarkFigures.DomainServices;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BenchmarkFigures.DomainServices.UseCases
//{
//    public class BenchmarkPerformanceUseCase
//    {
//        public PortfolioReport GeneratePortfolioReportUsingLiveData(IPriceRepository priceRepository,DateTime endDate, string instrumentCode)
//        {
//            var combinedMixedBenchmark = BenchmarkPerformanceCalculator
//                    .CreateBenchmarkWithMixedCodes(
//                    null
//                    , instrumentCode
//                    , endDate
//                    , priceRepository
//                    );

//            return PortfolioPerformanceReportCalculator.GeneratePortfolioReport(endDate, combinedMixedBenchmark);
//        }

//        public PortfolioReport GeneratePortfolioUsingHistoryData(IBenchmarkPerformanceRepository benchmarkPerformanceRepository, IPriceRepository priceRepository, DateTime endDate, string instrumentCode, DateTime switchingDate)
//        {
//            var combinedMixedBenchmark = BenchmarkPerformanceCalculator
//                        .CreateBenchmarkWithMixedCodes(
//                        benchmarkPerformanceRepository
//                        , instrumentCode
//                        , endDate
//                        , null
//                        , null
//                        , true
//                        );

//            return PortfolioPerformanceReportCalculator.GeneratePortfolioReport(endDate, combinedMixedBenchmark);
//        }

//        public PortfolioReport GeneratePortfolioReportCombingLiveAndHistoryData(IBenchmarkPerformanceRepository benchmarkPerformanceRepository, DateTime endDate, string instrumentCode)
//        {

//        }
//    }
//}
