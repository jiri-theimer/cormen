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
                return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed", strPrefix);
            }
            
        }
        public DataTable GetList(BO.myQuery mq,bool bolGetTotalsRow=false)
        {            
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");

            if (mq.explicit_columns == null)
            {
                mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete();    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
            }
            if (bolGetTotalsRow)
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SUM(mq.Prefix))));   //součtová řádka gridu
            }
            else
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SELECT(mq.Prefix))));    //grid sloupce               
            }
            sb.Append(","+GetSQL_SELECT_Ocas(mq.Prefix, bolGetTotalsRow));  //konstantní pole jako pid,isclosed

            switch (mq.Prefix)
            {
                case "j02":                                        
                    sb.Append(" FROM j02Person a LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID");
                    break;
                case "j03":
                    sb.Append(" FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON j02.p28ID=p28.p28ID");
                    break;
                case "j04":
                    sb.Append(" FROM j04UserRole a");
                    break;
                case "b02":
                    sb.Append(" FROM b02Status a");               
                    break;
                case "p28":
                    sb.Append(" FROM p28Company a");
                    break;
                case "p26":
                    sb.Append(" FROM p26Msz a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID");
                    break;
                case "p21":
                    sb.Append(" FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID");
                    break;
                case "o12":
                    sb.Append(" FROM o12Category a");
                    break;
                case "p10":
                    sb.Append(" FROM p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
                    break;
                case "p13":
                    sb.Append(" FROM p13MasterTpv a");
                    break;
                case "p14":
                    sb.Append(" FROM p14MasterOper a");
                    if (String.IsNullOrEmpty(mq.explicit_orderby) && bolGetTotalsRow==false) mq.explicit_orderby = "a.p14RowNum";
                    break;
                case "o23":
                    sb.Append(" FROM o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
                    break;
                case "p11":
                    sb.Append(" FROM p11ClientProduct a LEFT OUTER JOIN p12ClientTpv p12 ON a.p12ID=p12.p12ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID");
                    sb.Append(" LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p20Unit p20 ON a.p20ID=p20.p20ID");
                    break;
                case "p12":
                    sb.Append(" FROM p12ClientTpv a LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID");
                    break;                
                case "p15":
                    sb.Append(" FROM p15ClientOper a");
                    if (String.IsNullOrEmpty(mq.explicit_orderby) && bolGetTotalsRow == false) mq.explicit_orderby = "a.p15RowNum";
                    break;
                case "p41":
                    sb.Append(" FROM p41Task a LEFT OUTER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID");
                    sb.Append(" LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID");
                    break;
                default:
                    break;
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
