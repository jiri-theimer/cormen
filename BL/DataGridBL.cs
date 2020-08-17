using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(BO.myQuery mq, bool bolGetTotalsRow = false);
       
    }
    class DataGridBL:BaseBL,IDataGridBL
    {
        
        public DataGridBL(BL.Factory mother):base(mother)
        {
            
        }


        


        private string GetSQL_SELECT_Ocas(string strPrefix, bool bolGetTotalsRow)
        {            
            if (bolGetTotalsRow)
            {
                return string.Format("COUNT(a.{0}ID) as RowsCount", strPrefix);
            }
            else
            {
                return string.Format("a.{0}ID as pid,convert(bit,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end) as isclosed", strPrefix);
            }
            
        }
        public DataTable GetList(BO.myQuery mq,bool bolGetTotalsRow=false)
        {            
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");
            if (mq.TopRecordsOnly > 0)
            {
                sb.Append("TOP "+mq.TopRecordsOnly.ToString()+" ");
            }

            if (mq.explicit_columns == null || mq.explicit_columns.Count()==0)
            {
                
                mq.explicit_columns = new BL.TheColumnsProvider(_mother.App).getDefaultPallete(false,mq);    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
            }
            if (bolGetTotalsRow)
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SUM())));   //součtová řádka gridu
            }
            else
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SELECT())));    //grid sloupce               
            }
            sb.Append("," + GetSQL_SELECT_Ocas(mq.Prefix, bolGetTotalsRow));  //konstantní pole jako pid,isclosed
            if (bolGetTotalsRow==false && (mq.Prefix == "p21" || mq.Prefix == "p41" || mq.Prefix=="o23" || mq.Prefix=="p51" || mq.Prefix=="p10" || mq.Prefix=="p26" || mq.Prefix=="p11"))
            {
                sb.Append(",bc.b02Color as bgcolor");
            }
            else
            {
                sb.Append(",NULL as bgcolor");
            }
            if (mq.explicit_selectsql != null){
                sb.Append("," + mq.explicit_selectsql);
            }

            sb.Append(" FROM ");
            BO.TheEntity ce = BL.TheEntities.ByPrefix(mq.Prefix);
            sb.Append(ce.SqlFromGrid);    //úvodní FROM klauzule s primární "a" tabulkou            
            
            List<string> relSqls = new List<string>();
            foreach (BO.TheGridColumn col in mq.explicit_columns.Where(x => x.RelName != null && x.RelName != "a"))
            {
                if (col.RelSqlDependOn != null && relSqls.Exists(p=>p==col.RelSqlDependOn)==false)
                {                    
                    //if (mq.Prefix=="p41" && (col.RelName == "p11_p20" || col.RelName== "p11_p20pro"))
                    //{   //natvrdo relace, protože bohužel neumíme závilost ob dvě relace:
                    //    if (relSqls.Exists(p => p == "INNER JOIN p52OrderItem p41_p52 ON a.p52ID = p41_p52.p52ID") == false)
                    //    {
                    //        relSqls.Add("INNER JOIN p52OrderItem p41_p52 ON a.p52ID=p41_p52.p52ID");                            
                    //        sb.Append(" INNER JOIN p52OrderItem p41_p52 ON a.p52ID=p41_p52.p52ID");
                    //    }                            
                    //}
                    
                    relSqls.Add(col.RelSqlDependOn);
                    sb.Append(" ");
                    sb.Append(col.RelSqlDependOn);

                }
                if (relSqls.Exists(p => p == col.RelSql) == false)
                {
                    relSqls.Add(col.RelSql);
                    sb.Append(" ");
                    sb.Append(col.RelSql);
                }

            }
            

           
            //vždy musí být nějaké výchozí třídění v ce.SqlOrderBy!!
            if (bolGetTotalsRow == false && String.IsNullOrEmpty(mq.explicit_orderby)) mq.explicit_orderby = ce.SqlOrderBy;



            //parametrický dotaz s WHERE klauzulí

            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(sb.ToString(),mq,_mother.CurrentUser, true);    //závěrečné vygenerování WHERE a ORDERBY klauzule

            if (bolGetTotalsRow == false && mq.OFFSET_PageSize > 0)
            {
                q.FinalSql += " OFFSET @pagesize*@pagenum ROWS FETCH NEXT @pagesize ROWS ONLY";
                if (q.Parameters4DT == null) q.Parameters4DT = new List<DL.Param4DT>();
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagesize", ParValue = mq.OFFSET_PageSize });
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagenum", ParValue = mq.OFFSET_PageNum });

            }
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }


        
        
        
        
    }
}
