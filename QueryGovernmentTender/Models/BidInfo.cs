using System.Collections.Generic;

namespace QueryGovernmentTender.Models
{
    public class BidInfo
    {
        public const string OrgNameColumnName = "機關名稱";
        public const string BidIdColumnName = "標案案號";
        public const string BidNameColumnName = "標案名稱";
        public const string TimesColumnName = "傳輸次數";
        public const string BidTypeColumnName = "招標方式";
        public const string BuyTypeColumnName = "採購性質";
        public const string AnnounceDateColumnName = "公告日期";
        public const string EndDateColumnName = "截止投標";
        public const string BudgetColumnName = "預算金額";
        public const string LinkColumnName = "公告連結";
        public static readonly List<string> ColumnNameList = new List<string>
        {
            OrgNameColumnName,
            BidIdColumnName,
            BidNameColumnName,
            TimesColumnName,
            BidTypeColumnName,
            BuyTypeColumnName,
            AnnounceDateColumnName,
            EndDateColumnName,
            BudgetColumnName,
            LinkColumnName
        };

        public string OrgName { get; set; }
        public string BidId { get; set; }
        public string BidName { get; set; }
        public int Times { get; set; }
        public string BidType { get; set; } 
        public string BuyType { get; set; }
        public string AnnounceDate { get; set; }
        public string EndDate { get; set; }
        public ulong Budget { get; set; }
        public string Link { get; set; }

        public List<string> ToOrderedValueList()
        {
            return new List<string>
            {
                OrgName,
                BidId,
                BidName,
                Times.ToString(),
                BidType,
                BuyType,
                AnnounceDate,
                EndDate,
                Budget.ToString(),
                Link
            };
        }
    }
}
