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

        public static string getEntityFromPrefix(string strEntity)
        {
            switch (strEntity)
            {
                case "p28": return "p28Company";
                case "p26": return "p26Msz";
                case "j02": return "j02Person";
                case "j03": return "j03User";
                case "p21": return "p21License";
                case "p10": return "p10MasterProduct";
                case "p13": return "p13MasterTpv";
                case "p14": return "p14MasterOper";
                case "o23": return "o23Doc";
                case "p41": return "p41Task";
                case "p51":return "p51Order";
                case "p52":return "p52OrderItem";
                case "b02":return "b02Status";
                case "o12":return "o12Category";
                case "j04":return "j04UserRole";
                case "p11":return "p11ClientProduct";
                case "p12":return "p12ClientTpv";
                case "p15":return "p15ClientOper";
                case "p19":return "p19Material";
                case "p18":return "p18OperCode";
                default:
                    return "";
            }
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
                    case "p10": return "Produkty [Master]";
                    case "p13": return "TPV [Master]";
                    case "p14": return "Technologický rozpis operací [Master]";
                    case "o23": return "Dokumenty";
                    case "p41": return "Výrobní zakázky";
                    case "p51":return "Objednávky";
                    case "p52":return "Položky objednávek";
                    case "b02":return "Workflow stavy";
                    case "o12":return "Kategorie";
                    case "j04":return "Aplikační role";
                    case "p11":return "Produkty [Klient]";
                    case "p12":return "TPV [Klient]";
                    case "p15": return "Technologický rozpis operací [Klient]";
                    case "p19":return "Materiály";                    
                    default:
                        return strEntity;
                }
            }
            switch (strEntity.Substring(0,3))
            {
                case "p28":return "Klient";
                case "p26":return "Stroj";
                case "j02":return "Jméno";
                case "j03":return "Uživatel";
                case "p21": return "Licence";
                case "p10": return "Produkt [Master]";
                case "p13": return "TPV [Master]";
                case "p14": return "Technologický rozpis operací [Master]";
                case "o23": return "Dokument";
                case "p41": return "Výrobní zakázka";
                case "b02": return "Workflow stav";
                case "o12": return "Kategorie";
                case "j04": return "Aplikační role";
                case "p11": return "Produkt [Klient]";
                case "p12": return "TPV [Klient]";
                case "p19":return "Material";
                default:
                    return strEntity;
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
