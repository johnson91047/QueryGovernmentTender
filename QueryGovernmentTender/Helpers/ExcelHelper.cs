using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using QueryGovernmentTender.Models;

namespace QueryGovernmentTender.Helpers
{
    public class ExcelHelper
    {
        private const int StartColumn = 1;
        private const int StartRow = 1;
        private readonly string _savePath;
        public ExcelHelper(string path)
        {
            _savePath = path;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void ExportAllBidInformation(List<BidInfo> bidInformation)
        {
            using ExcelPackage targetExcel = new ExcelPackage(new FileInfo(_savePath));
            string worksheetName = FormattedDate.NowString.Replace("/", "-");
            ExcelWorksheet worksheet = targetExcel.Workbook.Worksheets[worksheetName] ?? targetExcel.Workbook.Worksheets.Add(worksheetName);

            PrepareHeader(worksheet);

            int rowOffset = 1;

            ConsoleHelper.PrintDivider();
            ConsoleHelper.Print($"準備寫入, 共 {bidInformation.Count} 筆");
            ConsoleHelper.PrintDivider();
            for(int i = 0 ; i < bidInformation.Count ; i++)
            {
                var valueList = bidInformation[i].ToOrderedValueList();
                for (int j = 0; j < valueList.Count; j++)
                {
                    worksheet.SetValue(StartRow + rowOffset, StartColumn + j, valueList[j]);
                }

                rowOffset++;
                ConsoleHelper.Print($"寫入{i+1}筆");
            }


            ConsoleHelper.PrintDivider();
            ConsoleHelper.Print("儲存Excel");
            ConsoleHelper.PrintDivider();
            targetExcel.Save();
        }

        private static void PrepareHeader(ExcelWorksheet worksheet)
        {
            int columnOffset = 0;
            foreach (string columnName in BidInfo.ColumnNameList)
            {
                worksheet.SetValue(StartRow, StartColumn + columnOffset, columnName);
                columnOffset++;
            }
        }
    }
}
