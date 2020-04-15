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

        public static string ObjectDate2String(object d,string format="dd.MM.yyyy")
        {
            if (d == System.DBNull.Value) return "";
            return Convert.ToDateTime(d).ToString(format);
        }

        public static string getEntityAlias(string strEntity)
        {
            switch (strEntity)
            {
                case "p28":return "Klient";
                case "p26":return "Stroj";
                case "j02":return "Osoba/Uživatel";
                case "p21": return "Licence";
                case "p10": return "Master produkt";
                case "p13": return "Master TPV";
                case "o23": return "Dokument";
                case "p41": return "Výrobní zakázka";
                default:
                    return "";
            }
        }

    }




    
}
