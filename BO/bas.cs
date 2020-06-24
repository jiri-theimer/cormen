using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO
{
    public static class BAS
    {
        public static string OM2(string s,int maxlen)
        {
            if (s.Length > maxlen)
            {
                return s.Substring(0, maxlen - 1) + "...";
            }
            else
            {
                return s;
            }
        }
        public static int InInt(string s)
        {
            if (int.TryParse(s, out int x))
            {
                return x;
            }
            else
            {
                return 0;
            }
        }
        public static Double InDouble(string s)
        {
            if (double.TryParse(s, out Double x))
            {
                return x;
            }
            else
            {
                return 0;
            }
        }

        public static List<string> ConvertString2List(string s, string strDelimiter = ",")
        {
            var lis = new List<string>();

            if (s == null)
                return lis;
            
            lis.AddRange(s.Split(strDelimiter));

            return lis;
        }
        public static List<int> ConvertString2ListInt(string s, string strDelimiter = ",")
        {
            var lis = new List<int>();

            if (String.IsNullOrEmpty(s) == true)
                return lis;
            foreach(var ss in s.Split(strDelimiter))
            {
                lis.Add(BO.BAS.InInt(ss));
            }
            
            return lis;
        }
        public static string RemoveValueFromDelimitedString(string s,string sremove, string strDelimiter = ",")
        {
            List<string> lis = ConvertString2List(s, strDelimiter);
            if (lis.Contains(sremove))
            {
                lis.Remove(sremove);
            }
            
            return String.Join(strDelimiter, lis);
        }

        public static int? TestIntAsDbKey(int intPID)
        {
            if (intPID == 0)
            {
                return null;
            }
            else
            {
                return intPID;
            }
        }
        public static double? TestDouleAsDbKey(double dbl)
        {
            if (dbl == 0)
            {
                return null;
            }
            else
            {
                return dbl;
            }
        }
        public static decimal? TestDecimalAsDbKey(decimal dcl)
        {
            if (dcl == 0)
            {
                return null;
            }
            else
            {
                return dcl;
            }
        }
        
        public static DateTime String2Date(string d)
        {
            string[] arr = d.Split(".");
            if (arr.Length < 3) return (DateTime.Today);
            return (new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0])));
        }
        public static string ObjectDate2String(object d,string format="dd.MM.yyyy")
        {
            if (d == System.DBNull.Value || d==null) return "";
            return Convert.ToDateTime(d).ToString(format);
        }
        public static string ObjectDateTime2String(object d, string format = "dd.MM.yyyy HH:mm")
        {            
            return ObjectDate2String(d, format);
        }
        public static string Number2String(double n)
        {
            return string.Format("{0:#,0.00}",n);
        }

        

        public static string GetGuid()
        {

            return System.Guid.NewGuid().ToString("N");
        }


        public static string FormatFileSize(int byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824)
                size = String.Format("{0:##.##}", byteCount / 1073741824) + " GB";
            else if (byteCount >= 1048576)
                size = String.Format("{0:##.##}", byteCount / 1048576) + " MB";
            else if (byteCount >= 1024)
                size = String.Format("{0:##.##}", byteCount / 1024) + " KB";
            else if (byteCount > 0 && byteCount < 1024)
                size = byteCount.ToString() + " Bytes";

            return size;
        }


        public static string ParseCellValueFromDb(System.Data.DataRow dbRow, BO.TheGridColumn c)
        {
            if (dbRow[c.UniqueName] == System.DBNull.Value)
            {
                return "";
            }
            switch (c.FieldType)
            {
                case "bool":
                    if (Convert.ToBoolean(dbRow[c.UniqueName]) == true)
                    {
                        return "&#10004;";
                    }
                    else
                    {
                        return "";
                    }
                case "num0":
                    return string.Format("{0:#,0}", dbRow[c.UniqueName]);

                case "num":
                    return string.Format("{0:#,0.00}", dbRow[c.UniqueName]);
                case "num3":
                    return string.Format("{0:#,0.000}", dbRow[c.UniqueName]);
                case "num4":
                    return string.Format("{0:#,0.0000}", dbRow[c.UniqueName]);
                case "num5":
                    return string.Format("{0:#,0.00000}", dbRow[c.UniqueName]);
                case "num6":
                    return string.Format("{0:#,0.000000}", dbRow[c.UniqueName]);
                case "num1":
                    return string.Format("{0:#,0.0}", dbRow[c.UniqueName]);

                case "date":
                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy");


                case "datetime":

                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy HH:mm");
                case "datetimesec":

                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy HH:mm:ss");
                case "time":
                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("HH:mm");
                default:
                    return dbRow[c.UniqueName].ToString();
            }


        }

        public static string RightString(string input, int num)
        {
            if (num > input.Length)
            {
                num = input.Length;
            }
            return input.Substring(input.Length - num);
        }




    }




    
}
