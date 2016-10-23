using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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
            DateTime beginOfDay = new DateTime(y, M, d, 0, 0, 0, DateTimeKind.Unspecified);
            return beginOfDay;
        }
    }
    public enum TrangThai
    {
        [Display(Name = "Có Sẵn", Description = "Có Sẵn")]
        CoSan,

        [Display(Name = "Đang Được Mượn", Description = "Đang Được Mượn")]
        DangMuon,

        [Display(Name = "Mất", Description = "Mất")]
        Mat,
        [Display(Name = "Đã Thanh Lý", Description = "Đã Thanh Lý")]
        ThanhLy
    }

    public enum Lop
    {
        [Display(Name = "Lớp 1", Description = "Lớp 1")]
        LopMot,
        [Display(Name = "Lớp 2", Description = "Lớp 2")]
        LopHai,
        [Display(Name = "Lớp 3", Description = "Lớp 3")]
        LopBa,
        [Display(Name = "Lớp 4", Description = "Lớp 4")]
        LopBon,
        [Display(Name = "Lớp 5", Description = "Lớp 5")]
        LopNam,
    }

    public enum LOAIBAOCAO
    {
        [Display(Name = "Tháng", Description = "Tháng")]
        Thang,
        [Display(Name = "Năm", Description = "Năm")]
        Nam,
        [Display(Name = "Khoảng thời gian", Description = "Lớp 5")]
        KhoanThoiGian
    }

    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        public static T GetValueFromName(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentOutOfRangeException("name");
        }
    }
}