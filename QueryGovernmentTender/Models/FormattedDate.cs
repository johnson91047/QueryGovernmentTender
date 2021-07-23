using System;

namespace QueryGovernmentTender.Models
{
    public static class FormattedDate
    {
        public static readonly string TaiwanYear = (DateTime.Now.Year - 1911).ToString();
        public static readonly string NowString = DateTime.Now.ToString($"{TaiwanYear}/MM/dd");
        public static readonly string WeekAgoString = DateTime.Now.AddDays(-6).ToString($"{TaiwanYear}/MM/dd");
    }
}
