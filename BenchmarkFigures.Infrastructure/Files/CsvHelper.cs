using Domain.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Files
{
    public  class CsvHelper<T> where T : class
    {
        string CsvFilePath { get; set; }
        string[] Delimeter { get; set; }
        public CsvHelper(string fullFilePathName, string[] delimeter)
        {
            CsvFilePath = fullFilePathName;
            Delimeter = delimeter;
        }

        public string WriteToCsv(string stringToWrite)
        {
            string errorMessage = string.Empty;
            try
            {
                File.WriteAllText(CsvFilePath, stringToWrite);
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
            return errorMessage;
        }

        public  List<T> ReadFromCsvFileToList()
        {

            var path = $@"{CsvFilePath}"; 
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(Delimeter);
                csvParser.HasFieldsEnclosedInQuotes = true;

                string[] headerFields = new string[0];

                // read header
                if (!csvParser.EndOfData) 
                    headerFields = csvParser.ReadFields();
                List<T> list = new List<T>();
                while (!csvParser.EndOfData)
                {
                    //Price price = new() { };
                    var price = Activator.CreateInstance(typeof(T), new object[] { });
                    string[] row = csvParser.ReadFields();
                    for (int i=0; i< headerFields.Length; i++)
                    {
                        var propertyInfo = price.GetType().GetProperty(headerFields[i]);
                        var value = Convert.ChangeType(row[i], propertyInfo.PropertyType);
                        propertyInfo.SetValue(price, value, null);

                    }
                    list.Add((T)price);
                }

                return list;
            }
        }
    }
}
