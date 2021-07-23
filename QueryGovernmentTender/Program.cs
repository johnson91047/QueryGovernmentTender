using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using QueryGovernmentTender.Helpers;
using QueryGovernmentTender.Models;

namespace QueryGovernmentTender
{
    class Program
    {
        private const string DefaultSavePath = ".";
        private const string DefaultFileName = "GovernmentBid.xlsx";
        static async Task Main(string[] args)
        {
            string saveFolder = PromptSavePath();

            string fileName = PromptFileName();

            string savePath = Path.Combine(saveFolder, fileName);
            ConsoleHelper.Print($"輸出到 {Path.GetFullPath(savePath)}");

            ConsoleHelper.Print("擷取機關代號...");
            List<BidInfo> info = await new CrawlerHelper().GetAllBidInfo();

            ConsoleHelper.Print("輸出Excel檔案...");
            new ExcelHelper(savePath).ExportAllBidInformation(info);

            ConsoleHelper.Print("按任一按鍵結束...");
            Console.ReadKey();
        }


        private static string PromptFileName()
        {
            while (true)
            {
                Console.Write(@"輸入儲存Excel檔案名稱 (Enter使用預設""GovernmentBid"") :");
                string fileNameInput = Console.ReadLine();
                string fileName = DefaultFileName;
                if (string.IsNullOrEmpty(fileNameInput) || string.IsNullOrWhiteSpace(fileNameInput))
                {
                    ConsoleHelper.Print("使用預設檔案名稱...");
                }
                else if (fileNameInput.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    ConsoleHelper.Print("不合法的檔案名稱...");
                    continue;
                }
                else
                {
                    ConsoleHelper.Print($"使用{fileNameInput}當作檔案名稱...");
                    fileName = fileNameInput;
                }

                fileName = Path.GetFileNameWithoutExtension(fileName);
                fileName += ".xlsx";
                return fileName;
            }
        }

        private static string PromptSavePath()
        {
            string savePath;
            while (true)
            {
                Console.Write("輸入儲存Excel檔案位置 (Enter使用預設儲存在當前資料夾) :");
                string pathInput = Console.ReadLine();
                savePath = DefaultSavePath;
                if (string.IsNullOrEmpty(pathInput) || string.IsNullOrWhiteSpace(pathInput))
                {
                    ConsoleHelper.Print("使用預設位置...");
                }
                else
                {
                    try
                    {
                        savePath = Path.GetFullPath(pathInput);
                        savePath = Path.GetDirectoryName(savePath);
                    }
                    catch (Exception)
                    {
                        ConsoleHelper.Print("輸入位置錯誤");
                        continue;
                    }
                }
                break;
            }

            return savePath;
        }
    }
}
