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
        public BO.j72TheGridState LoadTheGridState(string strEntity,int intJ02ID);
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
        public BO.j72TheGridState LoadTheGridState(string strEntity, int intJ02ID)
        {
            return _db.Load<BO.j72TheGridState>(string.Format("SELECT a.*,{0} FROM j72TheGridState a WHERE a.j72Entity=@entity AND a.j02ID=@j02id", _db.GetSQL1_Ocas("j72")), new { entity = strEntity,j02id=intJ02ID });
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
            string s = "";
            
            if (mq.explicit_columns !=null)
            {
                if (bolGetTotalsRow == false)
                {
                    s = string.Join(",", mq.explicit_columns.Select(p => p.FinalSqlSyntax));
                }
                else
                {
                    s = string.Join(",", mq.explicit_columns.Select(p => p.FinalSqlSyntaxSum));
                }
                
                
                //var sels = new List<string>();
                //var sums = new List<string>;
                //foreach(var col in mq.explicit_columns)
                //{

                //    if (col.SqlSyntax == null)
                //    {
                //        sels.Add(col.Field);
                //        if (col.IsShowTotals) sums.Add(col.Field);
                //    }
                //    else
                //    {
                //        sels.Add(col.SqlSyntax+" AS "+col.Field);
                //    }                    
                //}
                //s = String.Join(",", sels);
            }
            switch (mq.Prefix)
            {
                case "j02":                    
                    if (s=="")
                    {
                        s = "a.*,j04.j04Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28,p28.p28Name,a.j02LastName+' '+a.j02FirstName+ISNULL(' '+a.j02TitleBeforeName,' ') as fullname_desc";
                    }                    
                    s = string.Format("SELECT {0},{1} from j02Person a LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", s, GetSQL_SELECT_Ocas("j02", bolGetTotalsRow));
                    break;
                case "j04":                    
                    s = string.Format("SELECT {0},a.* from j04UserRole a", GetSQL_SELECT_Ocas("j04", bolGetTotalsRow));
                    break;
                case "b02":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.b02Entity) as EntityAlias from b02Status a", GetSQL_SELECT_Ocas("b02", bolGetTotalsRow));                   
                    break;
                case "p28":
                    if (s == "")
                    {
                        s = "a.*";
                    }
                    s = string.Format("SELECT {0},{1} from p28Company a", GetSQL_SELECT_Ocas("p28", bolGetTotalsRow),s);
                    break;
                case "p26":
                    if (s == "") { s = "a.*,b02.b02Name,p28.p28Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28"; }
                   
                    s = string.Format("SELECT {0},{1} FROM p26Msz a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", GetSQL_SELECT_Ocas("p28", bolGetTotalsRow), s);
                    break;
                case "p21":
                    if (s == "") { s = "a.*,b02.b02Name,p28.p28Name,'p28/?pid=' + convert(varchar(10), a.p28ID) as p28"; }
                    
                    s = String.Format("SELECT {0},{1} FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID", GetSQL_SELECT_Ocas("p21", bolGetTotalsRow),s);
                    break;
                case "o12":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.o12Entity) as EntityAlias from o12Category a", GetSQL_SELECT_Ocas("o12", bolGetTotalsRow));
                    break;
                case "p10":
                    if (s == "") { s = "a.*,o12.o12Name,b02.b02Name,p13.p13Code,p13.p13Name,'p13/?pid='+convert(varchar(10),a.p13ID) as p13"; }
                 
                    s = string.Format("SELECT {0},{1} FROM p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID", GetSQL_SELECT_Ocas("p10", bolGetTotalsRow), s);
                    break;
                case "p13":
                    s = string.Format("SELECT {0},a.* from p13MasterTpv a", GetSQL_SELECT_Ocas("p13", bolGetTotalsRow));
                    break;
                case "p14":
                    s = string.Format("SELECT {0},a.* from p14MasterOper a", GetSQL_SELECT_Ocas("p14", bolGetTotalsRow));                    
                    break;
                case "o23":
                    if (s == "")
                    {
                        s = "a.*,o12.o12Name,b02.b02Name,dbo.getEntityAlias(a.o23Entity) as EntityAlias,dbo.getRecordAlias(a.o23Entity,a.o23RecordPid) as RecordUrlName,a.o23Entity+'/?pid='+convert(varchar(10),a.o23RecordPid) as RecordUrl";
                    }                    
                    s = string.Format("SELECT {0},{1} FROM o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID", GetSQL_SELECT_Ocas("o23", bolGetTotalsRow), s);
                    break;
                default:
                    break;
            }

          
            //parametrický dotaz s WHERE klauzulí
            
            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(s,mq,true);
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }


        public int SaveTheGridState(BO.j72TheGridState rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.j72ID);
            p.Add("j70ID", BO.BAS.TestIntAsDbKey(rec.j70ID));
            p.Add("j02ID", BO.BAS.TestIntAsDbKey(rec.j02ID));
            
            p.Add("j72Entity", rec.j72Entity);
            p.Add("j72Columns", rec.j72Columns);
            p.Add("j72SplitterFlag", rec.j72SplitterFlag);
            p.Add("j72HeightPanel1", rec.j72HeightPanel1);

            p.Add("j72PageSize", rec.j72PageSize);
            p.Add("j72CurrentPagerIndex", rec.j72CurrentPagerIndex);
            p.Add("j72CurrentRecordPid", rec.j72CurrentRecordPid);

            p.Add("j72SortDataField", rec.j72SortDataField);            
            p.Add("j72SortOrder", rec.j72SortOrder);
            p.Add("j72Filter", rec.j72Filter);

            p.Add("j72IsNoWrap", rec.j72IsNoWrap);
            p.Add("j72SelectableFlag", rec.j72SelectableFlag);

            return _db.SaveRecord("j72TheGridState", p, rec);
        }
    }
}
