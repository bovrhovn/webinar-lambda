using System;

namespace Lambada.Base
{
    public static class DateExtensions
    {
        public static string GetShortMonthAcronym(this DateTime dateTime)
        {
            return dateTime.Month == 1 ? "Jan" :
                dateTime.Month == 2 ? "Feb" :
                dateTime.Month == 3 ? "Mar" :
                dateTime.Month == 4 ? "Apr" :
                dateTime.Month == 5 ? "Maj" :
                dateTime.Month == 6 ? "Jun" :
                dateTime.Month == 7 ? "Jul" :
                dateTime.Month == 8 ? "Avg" :
                dateTime.Month == 9 ? "Sept" :
                dateTime.Month == 10 ? "Okt" :
                dateTime.Month == 11 ? "Nov" : "Dec";
        }
    }
}