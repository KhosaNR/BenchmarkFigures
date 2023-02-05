using Domain.Models;
using Domain.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkFigures.DomainServices.Interfaces
{
    public interface IBenchmarkPerformanceCalcultor
    {
        //Take prices and generate benchmark performance
        List<PriceDifferenceDto> CalculatePriceDifferences(List<Price> priceList);
        List<BenchmarkPerformance> GenerateBenchmarkPerformances(List<PriceDifferenceDto> priceListDifferences);

    }
}
