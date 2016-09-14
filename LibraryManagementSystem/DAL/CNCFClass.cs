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
            DateTime endOfDay = new DateTime(y, M, d).AddDays(1).AddMinutes(-1);
            //hoặc  
            //DateTime endOfDay = new DateTime(y, M, d, 23, 59, 0, 0);  
            return endOfDay;
        }
    }
}