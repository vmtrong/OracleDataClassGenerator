using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OracleDataClassGenerator.Engine
{
    public class AppUtil
    {

        public static string TableName { get; set; }
        public static string NumberField { get; set; }
        public static string ConnectionString { get; set; }

        public const string CS_DISPLAY_DATE_FORMAT = "dd/MM/yyyy"; //"dd-MMM-yyyy";
        public const string CS_DISPLAY_TIME_FORMAT = "HH:mm";
        public const string CS_DISPLAY_DATETIME_FORMAT = "dd-MMM-yyyy HH:mm";
        public const string CS_DISPLAY_TIMEDATE_FORMAT = "HH:mm dd-MMM-yyyy";
        public const string CS_DISPLAY_NUMBER_FORMAT = "#,##0";
        public const string CS_EDIT_DATE_FORMAT = "dd/MM/yyyy";
        public const string CS_EDIT_DATETIME_FORMAT = "dd/MM/yyyy HH:mm";
        public const string CS_EDIT_TIME_FORMAT = "HH:mm";
        public const string CS_DECIMAL_SYMBOL = ",";
        public const string CS_DIGIT_GROUP_SYMBOL = ".";
        public const byte CS_DECIMAL_SCALE = 2;
        public const string CS_DATETIME_SYMBOL = "/";

        public static void Application_SetCultureInfo(System.Threading.Thread mThread)
        {
            mThread.CurrentCulture = new CultureInfo("en-US");
            // format Date
            mThread.CurrentCulture.DateTimeFormat = GetDateFormat();

            // format Number
            mThread.CurrentCulture.NumberFormat = GetNumberFormat();
            //
        }
        public static DateTimeFormatInfo GetDateFormat()
        {
            var dateformat = new DateTimeFormatInfo();
            dateformat.ShortDatePattern = CS_DISPLAY_DATE_FORMAT;
            dateformat.DateSeparator = CS_DATETIME_SYMBOL;
            dateformat.FullDateTimePattern = CS_DISPLAY_DATETIME_FORMAT;
            //Set upper case for abb month name
            string[] arrMonthName = dateformat.AbbreviatedMonthNames;
            arrMonthName.Clone();
            for (int i = 0; i <= arrMonthName.Length - 1; i++)
            {
                arrMonthName[i] = arrMonthName[i].ToUpper();
            }
            dateformat.AbbreviatedMonthNames = arrMonthName;
            return dateformat;
        }
        public static NumberFormatInfo GetNumberFormat()
        {
            var numberformat = new NumberFormatInfo();
            numberformat.NumberDecimalSeparator = CS_DECIMAL_SYMBOL;
            numberformat.NumberGroupSeparator = CS_DIGIT_GROUP_SYMBOL;
            numberformat.CurrencyDecimalSeparator = CS_DECIMAL_SYMBOL;
            numberformat.CurrencyGroupSeparator = CS_DIGIT_GROUP_SYMBOL;
            return numberformat;
        }
        public static DateTime ToDateTime(object obj, string format = AppUtil.CS_DISPLAY_DATE_FORMAT)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();

            var ci = new CultureInfo("en-US");
            ci.DateTimeFormat = GetDateFormat();
            ci.DateTimeFormat.ShortDatePattern = format;
            if (format.Contains("/"))
                ci.DateTimeFormat.DateSeparator = "/";
            else if (format.Contains("-"))
                ci.DateTimeFormat.DateSeparator = "-";
            else
                ci.DateTimeFormat.DateSeparator = " ";

            try
            {
                DateTime date = DateTime.Parse(sValue, ci);
                return date;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
        public static Decimal ToDecimal(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();


            //var ci = new CultureInfo(StrCultureInfo);
            //ci.NumberFormat = GetNumberFormat();

            try
            {
                var culture = new CultureInfo("vi-VN");
                culture.NumberFormat.NumberGroupSeparator = ".";
                culture.NumberFormat.NumberDecimalSeparator = ",";
                return Decimal.Parse(sValue, culture);
            }
            catch (Exception)
            {
            }


            try
            {
                var culture = new CultureInfo("vi-VN");
                culture.NumberFormat.NumberGroupSeparator = ",";
                culture.NumberFormat.NumberDecimalSeparator = ".";
                return Decimal.Parse(sValue, culture);

            }
            catch (Exception)
            {
            }

            return 0;
        }
        public static Double ToDouble(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();

            //var ci = new CultureInfo(StrCultureInfo);
            //ci.NumberFormat = GetNumberFormat();

            var culture = new CultureInfo("vi-VN");
            try
            {
                culture.NumberFormat.NumberGroupSeparator = ".";
                culture.NumberFormat.NumberDecimalSeparator = ",";
                return Double.Parse(sValue, culture);
            }
            catch (Exception)
            {
            }

            try
            {
                culture.NumberFormat.NumberGroupSeparator = ",";
                culture.NumberFormat.NumberDecimalSeparator = ".";
                return Double.Parse(sValue, culture);

            }
            catch (Exception)
            {
            }

            return 0;
        }
        public static Int32 ToInt32(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();

            //var ci = new CultureInfo(StrCultureInfo);
            //ci.NumberFormat = GetNumberFormat();

            var culture = new CultureInfo("vi-VN");
            try
            {

                culture.NumberFormat.NumberGroupSeparator = ".";
                culture.NumberFormat.NumberDecimalSeparator = ",";
                return Int32.Parse(sValue, culture);
            }
            catch (Exception)
            {
            }


            try
            {
                culture.NumberFormat.NumberGroupSeparator = ",";
                culture.NumberFormat.NumberDecimalSeparator = ".";
                return Int32.Parse(sValue, culture);

            }
            catch (Exception)
            {
            }

            return 0;
        }
        public static Int64 ToInt64(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = "";
            else
                sValue = obj.ToString();

            //var ci = new CultureInfo(StrCultureInfo);
            //ci.NumberFormat = GetNumberFormat();
            var culture = new CultureInfo("vi-VN");
            try
            {
                culture.NumberFormat.NumberGroupSeparator = ".";
                culture.NumberFormat.NumberDecimalSeparator = ",";
                return Int64.Parse(sValue, culture);
            }
            catch (Exception)
            {
            }


            try
            {
                culture.NumberFormat.NumberGroupSeparator = ",";
                culture.NumberFormat.NumberDecimalSeparator = ".";
                return Int64.Parse(sValue, culture);

            }
            catch (Exception)
            {
            }
            return 0;
        }
        public static Boolean ToBoolean(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = string.Empty;
            else
                sValue = obj.ToString();

            Boolean retVal;

            if (sValue == "1" || sValue.ToLower() == "true" || sValue.ToLower() == "y")
                retVal = true;
            else if (!Boolean.TryParse(sValue, out retVal))
                retVal = false;


            return retVal;
        }
        public static String ToString(object obj)
        {
            string sValue;
            if (obj == null || obj == DBNull.Value)
                sValue = string.Empty;
            else
                sValue = obj.ToString();

            return sValue.Trim().Length == 0 ? string.Empty : sValue;
        }
        public static String DateTimeToString(DateTime date, string format = AppUtil.CS_DISPLAY_DATE_FORMAT)
        {
            if (date != null && date != DateTime.MinValue)
                return date.ToString(format);
            else
                return string.Empty;
        }
        public static object DateTimeToDBObject(DateTime date)
        {
            if (date == null || date == DateTime.MinValue)
                return DBNull.Value;
            else
                return date;
        }
    }
}
