using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL.DL
{
   public class basQuery
    {
        
        public static DL.FinalSqlCommand ParseFinalSql(string strPrimarySql,BO.myQuery mq,string strOrderBySql=null, bool bolPrepareParam4DT = false)
        {
            var lis = new List<DL.QueryRow>();

            if (mq.pids !=null && mq.pids.Any())
            {
                AQ(ref lis, mq.Entity + "ID IN (" + String.Join(",", mq.pids) + ")", "", null);
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
                if (mq.Entity == "p10") AQ(ref lis, "a.p10ID IN (select p10ID FROM p22LicenseBinding WHERE p21ID=@p21id)", "p21id", mq.p21id);
            }
            if (mq.p10id > 0)
            {
                if (mq.Entity == "p21") AQ(ref lis, "a.p21ID IN (select p21ID FROM p22LicenseBinding WHERE p10ID=@p10id)", "p10id", mq.p10id);
            }

            if (mq.p28id > 0)
            {
                if (mq.Entity == "j02" || mq.Entity == "p26") AQ(ref lis, "a.p28ID=@p28id", "p28id", mq.p28id);
            }
            if (mq.Entity == "b02" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.b02Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu stavů podle druhu entity
            }
            if (mq.Entity == "o12" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.o12Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu kategorií podle druhu entity
            }
            if (mq.SearchString !=null && mq.SearchString.Length>1)
            {
                if (mq.Entity == "p28")
                {
                    AQ(ref lis, "(a.p28Name LIKE '%'+@expr+'%' OR a.p28RegID LIKE '%'+@expr+'%' OR a.p28VatID LIKE '%'+@expr+'%' OR a.p28Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Entity == "p26")
                {
                    AQ(ref lis, "(a.p26Name LIKE '%'+@expr+'%' OR a.p26Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Entity == "p10")
                {
                    AQ(ref lis, "(a.p10Name LIKE '%'+@expr+'%' OR a.p10Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Entity == "o23")
                {
                    AQ(ref lis, "(a.o23Name LIKE '%'+@expr+'%' OR a.o23Code LIKE '%'+@expr+'%' OR a.o23Memo LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
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


                ret.SqlWhere = String.Join(" AND ", lis.Select(p => p.StringWhere)).Trim();
            }

            if (!string.IsNullOrEmpty(ret.SqlWhere))
            {
                strPrimarySql += " WHERE " + ret.SqlWhere;
            }
            if (!string.IsNullOrEmpty(strOrderBySql))
            {
                strPrimarySql += " ORDER BY " + strOrderBySql;
            }

            ret.FinalSql = strPrimarySql;
            return ret;

        }

        private static void AQ(ref List<DL.QueryRow> lis, string strWhere, string strParName, object ParValue)
        {
            lis.Add(new DL.QueryRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue });
        }

    }
}
