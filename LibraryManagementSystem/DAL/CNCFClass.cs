using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.DAL
{
    public class CNCFClass
    {
        public static DateTime GoToEndOfDay(DateTime date)
        {
            int y = date.Year;
            int M = date.Month;
            int d = date.Day;
            DateTime endOfDay = new DateTime(y, M, d, 23, 59, 59, DateTimeKind.Unspecified);
            //hoặc  
            //DateTime endOfDay = new DateTime(y, M, d).AddDays(1).AddMinutes(-1);
            return endOfDay;
        }
        public static DateTime GoToBeginOfDay(DateTime date)
        {
            int y = date.Year;
            int M = date.Month;
            int d = date.Day;
            DateTime beginOfDay = new DateTime(y, M, d, 0, 0, 1, DateTimeKind.Unspecified);
            return beginOfDay;
        }
    }
}