// See https://aka.ms/new-console-template for more information
using BenchmarkFigures.DomainServices;
using Infrastructure.Files;
using Infrastructure.Repositories;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace BenchmarkFigures.ConsoleApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static IBenchmarkPerformanceRepository BenchmarkPerformanceRepository;
        static IPriceRepository PriceRepository;
        static string priceListFilePath, benchmarkHistoryFilePath, csvSavePath;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("HI, WELCOME TO THE PORTFOLIO REPORT GENERATOR.\n\n");
                Console.WriteLine("Please press Ctrl + C at any time to close.\n");
                Console.WriteLine("Please follow the instructions below:\n");

                if(string.IsNullOrEmpty(priceListFilePath))
                    priceListFilePath = PromptForValidPath("price list");
                if (string.IsNullOrEmpty(benchmarkHistoryFilePath))
                    benchmarkHistoryFilePath = PromptForValidPath("benchmark history list");

                BenchmarkPerformanceRepository = new BenchmarkPerformanceRepository(benchmarkHistoryFilePath, new string[] { "," });
                PriceRepository = new PriceRepository(priceListFilePath, new string[] { "," });

                int reportingOption = SelectPortfolioReportingType();
                var portfolioReport = CreateReport(reportingOption);

                SaveReport(portfolioReport);
                Console.WriteLine("\n\nYOUR REPORT IS READY...\n\nPlease press Ctrl + C to close \n\nPress ENTER to generate another report.\n");
                Console.ReadLine();
            }
        }
        static SortedList<DateTime, Tuple<string, bool>> GetSwitchingData(bool switchMultipleInstrumentCode = false, string initialBenchmarkCode = "")
        {
            SortedList<DateTime, Tuple<string, bool>> switchingCodes = new();
            if (!switchMultipleInstrumentCode)
            {
                var benchmarkSwitchDate = GetDateFromInput("switching date");
                Tuple<string, bool> CodeAndUseHistoric = Tuple.Create(initialBenchmarkCode, true);
                switchingCodes.Add(benchmarkSwitchDate, CodeAndUseHistoric);
                return switchingCodes;
            }


            bool continueCapturing = true;
            while (continueCapturing)
            {
                DateTime startDate;
                string switchingCode;
                bool dataIsLive;

                GetSwitchingDatesData(out startDate, out switchingCode, out dataIsLive);
                var benchmarkSwitchDate = startDate;
                Tuple<string, bool> CodeAndUseHistoric = Tuple.Create(switchingCode, !dataIsLive);
                switchingCodes.Add(benchmarkSwitchDate, CodeAndUseHistoric);

                Console.WriteLine("Do you want to capture switching data again? True= Yes, False = No");
                var again = Console.ReadLine();
                while (!bool.TryParse(again, out continueCapturing))
                {
                    Console.WriteLine("Invalid input. Please try again!!\n");
                }
            }

            return switchingCodes.Count>0? switchingCodes: null;
        }


        static PortfolioReport CreateReport(int reportingOption)
        {

            var endDate = GetDateFromInput("end date");
            string initialBenchmarkCode = GetStringFromInput("instrument code");
            SortedList<DateTime, Tuple<string, bool>> switchingCodes = null;
            var useHistoricDate = false;

            switch (reportingOption)
            {
                case 1:
                    useHistoricDate = true;
                    break;
                case 2:
                    useHistoricDate = false;
                    break;
                case 3:
                    //Enter switching date
                    switchingCodes = GetSwitchingData(false, initialBenchmarkCode);
                    useHistoricDate = false;
                    break;
                case 4:
                    //Enter switching date
                    switchingCodes = GetSwitchingData(true, initialBenchmarkCode);
                    useHistoricDate = false;
                    break;
            }

            var combinedMixedBenchmark = BenchmarkPerformanceCalculator
            .CreateBenchmarkForOneOrMoreInstruments(
            BenchmarkPerformanceRepository
            , initialBenchmarkCode
            , endDate
            , PriceRepository
            , switchingCodes
            , useHistoricDate
            );

            return PortfolioPerformanceReportCalculator.GeneratePortfolioReport(endDate, combinedMixedBenchmark);
        }

        static string GetStringFromInput(string nameOfInput)
        {
            string inputString = string.Empty;
            while (string.IsNullOrEmpty(inputString))
            {
                Console.WriteLine($"Please enter {nameOfInput}:\n");
                inputString = Console.ReadLine();
                if (string.IsNullOrEmpty(inputString))
                {
                    Console.WriteLine("Invalid input. Please try again!!\n");
                }
            }

            return inputString;
        }

        static DateTime GetDateFromInput(string nameOfDate)
        {
            DateTime chosenDate = DateTime.MinValue;
            while (DateTime.Compare(chosenDate, DateTime.MinValue) == 0)
            {
                Console.WriteLine($"Please enter {nameOfDate}: in formart yyyy/MM/dd Example 2018/12/31\n");
                string dateString = Console.ReadLine();
                if (!DateTime.TryParse(dateString, out chosenDate))
                {
                    Console.WriteLine("Invalid input. Please try again!!\n");
                }
            }

            return chosenDate;
        }


        static string PromptForValidPath(string fileName)
        {
            string priceListFullPath;
            string promptMessage = $"Please enter the full path of the {fileName} csv file and press Enter:\n";
            Console.WriteLine(promptMessage);
            priceListFullPath = Console.ReadLine();
            while (!File.Exists(@$"{priceListFullPath}"))
            {
                Console.WriteLine("File not found! ");
                Console.WriteLine(promptMessage);
                priceListFullPath = Console.ReadLine();
            }

            return priceListFullPath;
        }

        static int SelectPortfolioReportingType()
        {
            int selectedOption = 0;
            while (selectedOption == 0)
            {
                Console.WriteLine("Please choose from one of the options below:\n\n".ToUpper());
                Console.WriteLine("Press 1 for Portfolio report based on benchmark history for one instrument code.\n");
                Console.WriteLine("Press 2 for Portfolio report based on live prices for one instrument code.\n");
                Console.WriteLine("Press 3 for Portfolio report based on a combination of live prices and benchmark history for one instrument code.\n");
                Console.WriteLine("Press 4 for Portfolio report based on live prices and benchmark history with an option to switch benchmark codes.\n");
                var processingOption = Console.ReadLine();
                if (!int.TryParse(processingOption, out selectedOption))
                {
                    Console.WriteLine("Invalid input!\n\n");
                }
            }
            return selectedOption;
        }

        static bool GetSwitchingDatesData(out DateTime startDate, out string switchingCode, out bool dataIsLive)
        {
            bool inputIsValid = false;
            startDate = DateTime.MinValue;
            switchingCode = "";
            dataIsLive = true;
            while (!inputIsValid)
            {
                Console.WriteLine("Please enter start date, benchmark code and specify if this is live data. Example: 2018/12/31,JB01,true\n");
                var switchingData = Console.ReadLine();
                var inputArray = switchingData.Split(',');
                if (inputArray.Length != 3)
                {
                    Console.WriteLine("Invalid input. Please try again!!\n");
                    continue;
                }
                //DateTime startDate;
                if (!DateTime.TryParse(inputArray[0], out startDate))
                {
                    Console.WriteLine("Invalid date input. Please try again!!\n");
                    continue;
                }
                if (string.IsNullOrEmpty(inputArray[1]))
                {
                    Console.WriteLine("Invalid code input. Please try again!!\n");
                    continue;
                }
                bool isLive;
                if (!bool.TryParse(inputArray[2], out isLive))
                {
                    Console.WriteLine("Invalid true/false input. Please try again!!\n");
                    continue;
                }
                inputIsValid = true;

                switchingCode = inputArray[1];
                dataIsLive = isLive;
            }

            return inputIsValid;
        }

        static void SaveReport(PortfolioReport portfolioReport)
        {
            var portfolioCsvString = portfolioReport.GenerateCsvString();

            if (string.IsNullOrEmpty(csvSavePath))
            {
                Console.WriteLine("Please specify path to save report to and press Enter:\n");
                csvSavePath = Console.ReadLine();
            }
            var CsvWriter = new CsvHelper<Object>($@"{csvSavePath}\PortfolioReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv", new string[] { "," });
            var writingToCsvOutput = CsvWriter.WriteToCsv(portfolioCsvString);
            if (!string.IsNullOrEmpty(writingToCsvOutput))
            {
                Console.WriteLine(writingToCsvOutput);
            }
        }

    }
}



