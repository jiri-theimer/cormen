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

            if (mq.b02id > 0)
            {
                AQ(ref lis, "a.b02ID=@b02id", "b02id", mq.b02id);
            }
            if (mq.j04id > 0)
            {
                AQ(ref lis, "a.j04ID=@j04id", "j04id", mq.j04id);
            }
            if (mq.p28id > 0)
            {
                if (mq.Entity == "j02" || mq.Entity == "p26") AQ(ref lis, "a.p28ID=@p28id", "p28id", mq.p28id);
            }
            if (mq.Entity == "b02" && mq.query_by_entity_prefix == "p26")
            {
                AQ(ref lis, "a.b02Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu stavů podle druhu entity
            }

            var ret = new DL.FinalSqlCommand();
            
            if (lis.Count > 0)
            {
                ret.Parameters = new Dapper.DynamicParameters();
                if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                foreach (var c in lis)
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
