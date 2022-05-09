using System;

namespace DataClassGenerator.Engine
{
    public class ConvertUtility
    {
        private static string[] arrStrings = new string[]
        {
            "ntext",
            "nvarchar",
            "varchar"
        };

        private static string[] arrInts = new string[]
        {
            "int",
            "bigint",
            "byte",
            "bit"
        };

        private static string[] arrChars = new string[]
        {
            "char",
            "nchar"
        };

        private static string[] arrDateTimes = new string[]
        {
            "datetime"
        };

        public static string ToString(object obj)
        {
            string result;
            try
            {
                result = Convert.ToString(obj);
            }
            catch
            {
                result = "";
            }
            return result;
        }

        public static int ToInt(object obj)
        {
            int result;
            try
            {
                result = Convert.ToInt32(obj);
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        public static string ToVarType(string varType)
        {
            string[] array = ConvertUtility.arrStrings;
            string result;
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                if (item == varType.ToLower())
                {
                    result = "string";
                    return result;
                }
            }
            string[] array2 = ConvertUtility.arrInts;
            for (int j = 0; j < array2.Length; j++)
            {
                string item2 = array2[j];
                if (item2 == varType.ToLower())
                {
                    result = "int";
                    return result;
                }
            }
            string[] array3 = ConvertUtility.arrChars;
            for (int k = 0; k < array3.Length; k++)
            {
                string item3 = array3[k];
                if (item3 == varType.ToLower())
                {
                    result = "char";
                    return result;
                }
            }
            string[] array4 = ConvertUtility.arrDateTimes;
            for (int l = 0; l < array4.Length; l++)
            {
                string item4 = array4[l];
                if (item4 == varType.ToLower())
                {
                    result = "DateTime";
                    return result;
                }
            }
            result = "string";
            return result;
        }

        public static string ToVarPrivate(string varName)
        {
            return "_" + varName.Substring(0,1).ToLower() + varName.Substring(1);
        }

        public static string ToVarConvert(string varType)
        {
            string[] array = ConvertUtility.arrStrings;
            string result;
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                if (item == varType.ToLower())
                {
                    result = "AppUtil.ToString";
                    return result;
                }
            }
            string[] array2 = ConvertUtility.arrInts;
            for (int j = 0; j < array2.Length; j++)
            {
                string item2 = array2[j];
                if (item2 == varType.ToLower())
                {
                    result = "AppUtil.ToInt32";
                    return result;
                }
            }
            string[] array3 = ConvertUtility.arrChars;
            for (int k = 0; k < array3.Length; k++)
            {
                string item3 = array3[k];
                if (item3 == varType.ToLower())
                {
                    result = "AppUtil.ToChar";
                    return result;
                }
            }
            string[] array4 = ConvertUtility.arrDateTimes;
            for (int l = 0; l < array4.Length; l++)
            {
                string item4 = array4[l];
                if (item4 == varType.ToLower())
                {
                    result = "AppUtil.ToDateTime";
                    return result;
                }
            }
            result = "AppUtil.ToString";
            return result;
        }

        public static string ToSQLParameters(string varType, string varDataLength)
        {
            string result;
            if (varType.ToLower() == "ntext")
            {
                result = varType;
            }
            else
            {
                string[] array = ConvertUtility.arrStrings;
                for (int i = 0; i < array.Length; i++)
                {
                    string item = array[i];
                    if (item.ToLower() == varType.ToLower())
                    {
                        result = varType + " (" + varDataLength + ")";
                        return result;
                    }
                }
                result = varType;
            }
            return result;
        }
    }
}
