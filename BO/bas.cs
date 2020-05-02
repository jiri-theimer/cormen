using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public static class BAS
    {
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

            if (s == null)
                return lis;
            foreach(var ss in s.Split(strDelimiter))
            {
                lis.Add(BO.BAS.InInt(ss));
            }
            
            return lis;
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

        public static string ObjectDate2String(object d,string format="dd.MM.yyyy")
        {
            if (d == System.DBNull.Value) return "";
            return Convert.ToDateTime(d).ToString(format);
        }

        public static string getEntityAlias(string strEntity,bool bolMnozne=false)
        {
            if (bolMnozne)
            {
                switch (strEntity.Substring(0, 3))
                {
                    case "p28": return "Klienti";
                    case "p26": return "Stroje";
                    case "j02": return "Lidé";
                    case "j03":return "Uživatelské účty";
                    case "p21": return "Licence";
                    case "p10": return "Master produkty";
                    case "p13": return "Master TPV";
                    case "o23": return "Dokumenty";
                    case "p41": return "Výrobní zakázky";
                    default:
                        return "";
                }
            }
            switch (strEntity.Substring(0,3))
            {
                case "p28":return "Klient";
                case "p26":return "Stroj";
                case "j02":return "Jméno";
                case "j03":return "Uživatel";
                case "p21": return "Licence";
                case "p10": return "Master produkt";
                case "p13": return "Master TPV";
                case "o23": return "Dokument";
                case "p41": return "Výrobní zakázka";
                default:
                    return "";
            }
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





    }




    
}
