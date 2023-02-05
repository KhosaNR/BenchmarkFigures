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
    public class PriceRepository : CsvHelper<Price>, IPriceRepository
    {

        public PriceRepository(string csvFilePath, string[] delimeter) : base(csvFilePath, delimeter) { }
        public List<Price> GetAll()
        {
            return ReadFromCsvFileToList();
        }

        public List<Price> GetByInstrumentCode(string instrumentCode)
        {
            return GetAll().Where(price => price.InstrumentCode.ToUpper() == instrumentCode.ToUpper()).ToList();
        }

        public List<Price> GetBetweenDates(DateTime priceDateFrom, DateTime priceDateTo)
        {
            return GetAll()
                .Where(price => DateTime.Compare(price.PriceDate, priceDateFrom) >= 0
                    && DateTime.Compare(price.PriceDate, priceDateTo) <= 0)
                .ToList();
        }

        public List<Price> GetByEndDate(DateTime priceDateFrom, DateTime priceDateTo)
        {
            return GetAll()
                .Where(price => DateTime.Compare(price.PriceDate, priceDateTo) <= 0)
                .ToList();
        }

        public List<Price> GetByInstrumentCodeBetwenDates(string instrumentCode, DateTime priceDateFrom, DateTime priceDateTo)
        {
            return GetByInstrumentCode(instrumentCode)
                .Where(price => DateTime.Compare(price.PriceDate, priceDateFrom) >= 0
                    && DateTime.Compare(price.PriceDate, priceDateTo) <= 0)
                .ToList();
        }

        public List<Price> GetByInstrumentCodeAndEndDate(string instrumentCode, DateTime endDate)
        {
            return GetByInstrumentCode(instrumentCode)
                .Where(price => DateTime.Compare(price.PriceDate, endDate) <= 0)
                .ToList();
        }
    }
}
