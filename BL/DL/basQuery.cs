﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL.DL
{
   public class basQuery
    {
        
        public static DL.FinalSqlCommand ParseFinalSql(string strPrimarySql,BO.myQuery mq, bool bolPrepareParam4DT = false)
        {
            var lis = new List<DL.QueryRow>();

            if (mq.pids !=null && mq.pids.Any())
            {
                AQ(ref lis, mq.PkField + " IN (" + String.Join(",", mq.pids) + ")", "", null);
            }
            if (mq.b02id > 0)
            {
                AQ(ref lis, "a.b02ID=@b02id", "b02id", mq.b02id);
            }
            if (mq.j04id > 0)
            {
                AQ(ref lis, "a.j04ID=@j04id", "j04id", mq.j04id);
            }
            if (mq.p21id >0)
            {
                if (mq.Prefix == "p10") AQ(ref lis, "a.p10ID IN (select p10ID FROM p22LicenseBinding WHERE p21ID=@p21id)", "p21id", mq.p21id);
            }
            if (mq.p10id > 0)
            {
                if (mq.Prefix == "p21") AQ(ref lis, "a.p21ID IN (select p21ID FROM p22LicenseBinding WHERE p10ID=@p10id)", "p10id", mq.p10id);
            }
            if (mq.p13id > 0)
            {
                if (mq.Prefix == "p14") AQ(ref lis, "a.p13ID=@p13id", "p13id", mq.p13id);
            }

            if (mq.p28id > 0)
            {
                if (mq.Prefix == "j02" || mq.Prefix == "p26") AQ(ref lis, "a.p28ID=@p28id", "p28id", mq.p28id);
            }
            if (mq.Prefix == "b02" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.b02Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu stavů podle druhu entity
            }
            if (mq.Prefix == "o12" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.o12Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu kategorií podle druhu entity
            }
            if (String.IsNullOrEmpty(mq.SearchString)==false && mq.SearchString.Length>1)
            {
                if (mq.Prefix == "p28")
                {
                    AQ(ref lis, "(a.p28Name LIKE '%'+@expr+'%' OR a.p28RegID LIKE '%'+@expr+'%' OR a.p28VatID LIKE '%'+@expr+'%' OR a.p28Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p26")
                {
                    AQ(ref lis, "(a.p26Name LIKE '%'+@expr+'%' OR a.p26Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p10")
                {
                    AQ(ref lis, "(a.p10Name LIKE '%'+@expr+'%' OR a.p10Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p13")
                {
                    AQ(ref lis, "(a.p13Name LIKE '%'+@expr+'%' OR a.p13Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "o23")
                {
                    AQ(ref lis, "(a.o23Name LIKE '%'+@expr+'%' OR a.o23Code LIKE '%'+@expr+'%' OR a.o23Memo LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
            }
            if (String.IsNullOrEmpty(mq.j72Filter)==false)
            {
                ParseSqlFromTheGridFilter(mq,ref lis);  //složit filtrovací podmínku ze sloupcového filtru gridu
            }

            var ret = new DL.FinalSqlCommand();
            
            if (lis.Count > 0)
            {
                ret.Parameters = new Dapper.DynamicParameters();
                if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                foreach (var c in lis.Where(p=>String.IsNullOrEmpty(p.ParName)==false))
                {
                    ret.Parameters.Add(c.ParName, c.ParValue);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.ParName, ParValue = c.ParValue });
                }


                ret.SqlWhere = String.Join(" ", lis.Select(p =>p.AndOrZleva+" "+ p.StringWhere)).Trim();    //složit závěrčnou podmínku
                
            }

            if (!string.IsNullOrEmpty(ret.SqlWhere))
            {
                strPrimarySql += " WHERE " + ret.SqlWhere;
            }
            if (!string.IsNullOrEmpty(mq.explicit_orderby))
            {
                strPrimarySql += " ORDER BY " + mq.explicit_orderby;
            }

            ret.FinalSql = strPrimarySql;
            return ret;

        }

        private static void AQ(ref List<DL.QueryRow> lis, string strWhere, string strParName, object ParValue,string strAndOrZleva="AND")
        {
            if (lis.Count == 0)
            {
                strAndOrZleva = ""; //první podmínka zleva
            }
            lis.Add(new DL.QueryRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue,AndOrZleva= strAndOrZleva });
        }
        private static object get_param_value(string colType,string colValue)
        {
            if (String.IsNullOrEmpty(colValue)==true){
                return null;
            }
            if (colType == "num")
            {
                return  BO.BAS.InDouble(colValue);
            }
            if (colType == "date")
            {
                return Convert.ToDateTime(colValue);
            }
            if (colType == "bool")
            {
                if (colValue == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return colValue;
        }
        private static void ParseSqlFromTheGridFilter(BO.myQuery mq,ref List<DL.QueryRow> lisAQ)
        {
            var colsProvider = new BL.TheColumnsProvider(mq);
            var rows = colsProvider.ParseAdhocFilterFromString(mq.j72Filter);
            int x = 0;
            foreach (var filterrow in rows)
            {
                var col = filterrow.BoundColumn;
                var strF = "a."+col.Field;
                if (col.SqlSyntax != null) strF = col.SqlSyntax;
                x += 1;
                string parName = "par" + x.ToString();
               
                int endIndex = 0;
                string[] arr = new string[] { filterrow.value };
                if (filterrow.value.IndexOf(";") > -1)  //v podmnínce sloupcového filtru může být středníkem odděleno více hodnot!
                {
                    arr = filterrow.value.Split(";");
                    endIndex = arr.Count()-1;
                }
                
                switch (filterrow.oper)
                {
                    case "1":   //IS NULL
                        AQ(ref lisAQ,strF+ " IS NULL", "", null);
                        break;
                    case "2":   //IS NOT NULL
                        AQ(ref lisAQ, strF + " IS NOT NULL", "", null);
                        break;
                    case "10":   //větší než nula
                        AQ(ref lisAQ, strF + " > 0", "", null);
                        break;
                    case "11":   //je nula nebo prázdné
                        AQ(ref lisAQ, "ISNULL("+strF+ ",0)=0", "", null);
                        break;
                    case "8":   //ANO
                        AQ(ref lisAQ, strF+" = 1", "", null);
                        break;
                    case "9":   //NE
                        AQ(ref lisAQ, strF + " = 0", "", null);
                        break;
                    case "3":   //obsahuje                
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE '%'+@{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR"); ;
                            }
                            
                        }
                        
                        break;
                    case "5":   //začíná na 
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE @{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR");
                            }
                                
                        }
                            
                        break;
                    case "6":   //je rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " = @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }
                                
                        }
                        
                        break;
                    case "4":   //interval
                        AQ(ref lisAQ, string.Format(strF + " >= @{0}", parName+"c1"), parName+"c1", get_param_value(col.NormalizedTypeName, filterrow.c1value));
                        AQ(ref lisAQ, string.Format(strF + " <= @{0}", parName+"c2"), parName+"c2", get_param_value(col.NormalizedTypeName, filterrow.c2value));
                        break;
                    case "7":   //není rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " <> @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }
                        }
                        
                        break;
                }
              
            }

           
            string leva_zavorka(int i,int intEndIndex)
            {
                if (intEndIndex > 0 && i == 0)
                {
                    return "(";
                }
                else
                {
                    return "";
                }
            }
            string prava_zavorka(int i, int intEndIndex)
            {
                if (intEndIndex > 0 && i == intEndIndex)
                {
                    return ")";
                }
                else
                {
                    return "";
                }
            }

        }

    }
}
