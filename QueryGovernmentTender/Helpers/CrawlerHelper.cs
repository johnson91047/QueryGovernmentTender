using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using QueryGovernmentTender.Models;

namespace QueryGovernmentTender.Helpers
{
    public class CrawlerHelper
    {
        private const string Home =
           "https://web.pcc.gov.tw/tps/main/pss/pblm/tender/basic/search/mainListCommon.jsp?searchType=basic";
        private const string PostPath = "https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=basic";
        private const string BidPathBase = "https://web.pcc.gov.tw/tps/pss";
        private static readonly HttpClient Client = new HttpClient();

        public async Task<List<BidInfo>> GetAllBidInfo()
        {
            List<BidInfo> infos = new List<BidInfo>();
            List<string> allOrgId = GetAllOrgId();
            ConsoleHelper.Print($"共 {allOrgId.Count} 筆機關代碼");
            foreach (string orgId in allOrgId)
            {
                ConsoleHelper.PrintDivider();
                ConsoleHelper.Print($"尋找招標公告, 機關代碼 : {orgId}");
                
                string htmlContent = await SubmitSearchForm(orgId);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                infos.AddRange(GetBidInformation(doc));
                ConsoleHelper.PrintDivider();
                ConsoleHelper.Print("等待1秒...");
                await Task.Delay(1000);
            }

            return infos;
        }

        private List<BidInfo> GetBidInformation(HtmlDocument doc)
        {
            List<BidInfo> result = new List<BidInfo>();
            HtmlNodeCollection rowCollection = doc.DocumentNode.SelectNodes("//tr[@onmouseover]");

            if (rowCollection == null)
            {
                return new List<BidInfo>();
            }

            foreach (HtmlNode row in rowCollection)
            {
                if (row.SelectSingleNode("./td[1]").InnerText == "無符合條件資料")
                {
                    ConsoleHelper.Print("無公告");
                    continue;
                }
                
                result.Add(new BidInfo
                {
                    OrgName = row.SelectSingleNode("./td[2]").InnerText.Replace("&nbsp;", string.Empty),
                    BidId = GetBidId(row.SelectSingleNode("./td[3]").InnerText),
                    BidName = row.SelectSingleNode("./td[3]/a").Attributes["title"].Value,
                    Times = Convert.ToInt32(row.SelectSingleNode("./td[4]/a").Attributes["title"].Value),
                    BidType = row.SelectSingleNode("./td[5]").InnerText,
                    BuyType = row.SelectSingleNode("./td[6]").InnerText,
                    AnnounceDate = row.SelectSingleNode("./td[7]").InnerText,
                    EndDate = row.SelectSingleNode("./td[8]").InnerText,
                    Budget = GetBudget(row.SelectSingleNode("./td[9]").InnerText),
                    Link = $"{BidPathBase}/{row.SelectSingleNode("./td[3]/a").Attributes["href"].Value}",
                });
            }
            ConsoleHelper.Print($"找到招標公告, 共 {result.Count} 筆");
            return result;
        }

        private static string GetBidId(string innerText)
        {
            innerText = innerText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)[0];
            innerText = innerText.Trim('\t');
            return string.IsNullOrEmpty(innerText) ? string.Empty : innerText;
        }

        private static ulong GetBudget(string innerText)
        {
            innerText = innerText.Replace("\t", string.Empty).Replace("\r\n", string.Empty);
            if (string.IsNullOrEmpty(innerText)) return 0;
            return Convert.ToUInt64(innerText.Replace(",", string.Empty));
        }

        private List<string> GetAllOrgId()
        {
            List<string> result = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument homeDoc = web.Load(Home);
            HtmlNodeCollection departmentsCollection = homeDoc.DocumentNode.SelectNodes(@"//p/a[@style='color: blue;']");
            foreach (HtmlNode departmentUrlNode in departmentsCollection)
            {
                HtmlDocument inner = web.Load("https://web.pcc.gov.tw/tps/main/pss/pblm/tender/basic/search/" + departmentUrlNode.Attributes["href"].Value);
                string title = departmentUrlNode.Attributes["title"].Value;
                if(title == "各級學校") break;
                ConsoleHelper.PrintDivider();
                ConsoleHelper.Print($"尋找{title}..." );
                ConsoleHelper.PrintDivider();
                HtmlNodeCollection announceCollection = inner.DocumentNode.SelectNodes(@"//tr[@align='center']/td[@align='left'][1]");
                foreach (HtmlNode announceUrlNode in announceCollection)
                {
                    string text = announceUrlNode.InnerText;
                    if (string.IsNullOrWhiteSpace(text) || text == "機關代碼") continue;
                    ConsoleHelper.Print($"機關代碼 : {text}");
                    result.Add(text);
                }
            }
            return result;
        }

        private static async Task<string> SubmitSearchForm(string orgId)
        {
            HttpRequestMessage data = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(PostPath),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString() , "application/x-www-form-urlencoded" },
                    {HttpRequestHeader.Accept.ToString(), "*/*" },
                },
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("method", "search"),
                    new KeyValuePair<string, string>("searchMethod", "true"),
                    new KeyValuePair<string, string>("orgId", orgId),
                    new KeyValuePair<string, string>("hid_1", "1"),
                    new KeyValuePair<string, string>("tenderType", "tenderDeclaration"),
                    new KeyValuePair<string, string>("tenderWay", "1,2,3,4,5,6,7,10,12"),
                    new KeyValuePair<string, string>("tenderDateRadio", "on"),
                    new KeyValuePair<string, string>("tenderStartDateStr", FormattedDate.WeekAgoString),
                    new KeyValuePair<string, string>("tenderEndDateStr", FormattedDate.NowString),
                    new KeyValuePair<string, string>("tenderStartDate", FormattedDate.WeekAgoString),
                    new KeyValuePair<string, string>("tenderEndDate", FormattedDate.NowString),
                    new KeyValuePair<string, string>("isSpdt", "N"),
                    new KeyValuePair<string, string>("btnQuery", "查詢"),
                })

            };

            var response = await Client.SendAsync(data);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
