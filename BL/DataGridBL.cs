﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(BO.myQuery mq, bool bolGetTotalsRow = false);
        public BO.j72TheGridState LoadTheGridState(int intJ72ID);
        public BO.j72TheGridState LoadTheGridState(string strEntity,int intJ02ID,string strMasterEntity);
        public int SaveTheGridState(BO.j72TheGridState rec);

    }
    class DataGridBL:BaseBL,IDataGridBL
    {
      
        public DataGridBL(BL.Factory mother):base(mother)
        {            
           
        }

        public BO.j72TheGridState LoadTheGridState(int intJ72ID)
        {
            return _db.Load<BO.j72TheGridState>(string.Format("SELECT a.*,{0} FROM j72TheGridState a WHERE a.j72ID=@j72id", _db.GetSQL1_Ocas("j72")), new { j72id = intJ72ID });
        }
        public BO.j72TheGridState LoadTheGridState(string strEntity, int intJ03ID, string strMasterEntity)
        {
            if (String.IsNullOrEmpty(strMasterEntity))
            {
                return _db.Load<BO.j72TheGridState>(string.Format("SELECT a.*,{0} FROM j72TheGridState a WHERE a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity IS NULL", _db.GetSQL1_Ocas("j72")), new { entity = strEntity, j03id = intJ03ID });
            }
            else
            {
                return _db.Load<BO.j72TheGridState>(string.Format("SELECT a.*,{0} FROM j72TheGridState a WHERE a.j72Entity=@entity AND a.j03ID=@j03id AND a.j72MasterEntity=@masterentity", _db.GetSQL1_Ocas("j72")), new { entity = strEntity, j03id = intJ03ID,masterentity=strMasterEntity });
            }
            
        }


        private string GetSQL_SELECT_Ocas(string strPrefix, bool bolGetTotalsRow)
        {
            //return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity,'{0}/?pid='+convert(varchar(10),a.{0}ID) as {0}", strPrefix);
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
                mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete(1,1);    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
            }
            if (bolGetTotalsRow)
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SUM(mq.Prefix))));   //součtová řádka gridu
            }
            else
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SELECT(mq.Prefix))));    //grid sloupce               
            }
            sb.Append("," + GetSQL_SELECT_Ocas(mq.Prefix, bolGetTotalsRow));  //konstantní pole jako pid,isclosed

            sb.Append(" FROM ");
            BO.TheEntity ce = BL.TheEntities.ByPrefix(mq.Prefix);
            sb.Append(ce.SqlFrom);
            if (ce.SqlOrderBy != null)  //výchozí třídění záznamů entity
            {
                if (String.IsNullOrEmpty(mq.explicit_orderby) && bolGetTotalsRow == false) mq.explicit_orderby = ce.SqlOrderBy;
            }
            

           

            //parametrický dotaz s WHERE klauzulí

            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(sb.ToString(),mq,_mother.CurrentUser, true);    //závěrečné vygenerování WHERE a ORDERBY klauzule
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }


        public int SaveTheGridState(BO.j72TheGridState rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j72ID);
            p.AddInt("j70ID", rec.j70ID,true);
            p.AddInt("j03ID", rec.j03ID,true);
            
            p.AddString("j72Entity", rec.j72Entity);
            p.AddString("j72MasterEntity", rec.j72MasterEntity);
            p.AddString("j72Columns", rec.j72Columns);
            p.AddInt("j72SplitterFlag", rec.j72SplitterFlag);
            p.AddInt("j72HeightPanel1", rec.j72HeightPanel1);

            p.AddInt("j72PageSize", rec.j72PageSize);
            p.AddInt("j72CurrentPagerIndex", rec.j72CurrentPagerIndex);
            p.AddInt("j72CurrentRecordPid", rec.j72CurrentRecordPid);

            p.AddString("j72SortDataField", rec.j72SortDataField);            
            p.AddString("j72SortOrder", rec.j72SortOrder);
            p.AddString("j72Filter", rec.j72Filter);

            p.AddBool("j72IsNoWrap", rec.j72IsNoWrap);
            p.AddInt("j72SelectableFlag", rec.j72SelectableFlag);

            return _db.SaveRecord("j72TheGridState", p.getDynamicDapperPars(), rec);
        }
    }
}
