using Domain.Models;

namespace BenchmarkFigures.DomainServices
{
    public interface IPriceRepository
    {
        List<Price> GetAll();
        List<Price> GetBetweenDates(DateTime priceDateFrom, DateTime priceDateTo);
        List<Price> GetByEndDate(DateTime priceDateFrom, DateTime priceDateTo);
        List<Price> GetByInstrumentCode(string instrumentCode);
        List<Price> GetByInstrumentCodeAndEndDate(string instrumentCode, DateTime endDate);
        List<Price> GetByInstrumentCodeBetwenDates(string instrumentCode, DateTime priceDateFrom, DateTime priceDateTo);
    }
}