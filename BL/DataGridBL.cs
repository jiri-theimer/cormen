using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(BO.myQuery mq);

        
    }
    class DataGridBL:BaseBL,IDataGridBL
    {
      
        public DataGridBL(BL.Factory mother):base(mother)
        {            
           
        }
        
        private string GetSQL_SELECT_Ocas(string strPrefix)
        {
            //return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity,'{0}/?pid='+convert(varchar(10),a.{0}ID) as {0}", strPrefix);
            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed", strPrefix);
        }
        public DataTable GetList(BO.myQuery mq)
        {
            string s = "";
            if (mq.explicit_columns !=null)
            {
                var sels = new List<string>();
                foreach(var col in mq.explicit_columns)
                {
                    if (col.SqlSyntax == null)
                    {
                        sels.Add(col.Field);
                    }
                    else
                    {
                        sels.Add(col.SqlSyntax+" AS "+col.Field);
                    }                    
                }
                s = String.Join(",", sels);
            }
            switch (mq.Prefix)
            {
                case "j02":                    
                    if (s=="")
                    {
                        s = "a.*,j04.j04Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28,p28.p28Name,a.j02LastName+' '+a.j02FirstName+ISNULL(' '+a.j02TitleBeforeName,' ') as fullname_desc";
                    }                    
                    s = string.Format("SELECT {0},{1} from j02Person a LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", s, GetSQL_SELECT_Ocas("j02"));
                    break;
                case "j04":                    
                    s = string.Format("SELECT {0},a.* from j04UserRole a", GetSQL_SELECT_Ocas("j04"));
                    break;
                case "b02":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.b02Entity) as EntityAlias from b02Status a", GetSQL_SELECT_Ocas("b02"));                   
                    break;
                case "p28":
                    if (s == "")
                    {
                        s = "a.*";
                    }
                    s = string.Format("SELECT {0},{1} from p28Company a", GetSQL_SELECT_Ocas("p28"),s);
                    break;
                case "p26":
                    if (s == "") { s = "a.*,b02.b02Name,p28.p28Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28"; }
                   
                    s = string.Format("SELECT {0},{1} FROM p26Msz a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", GetSQL_SELECT_Ocas("p28"), s);
                    break;
                case "p21":
                    if (s == "") { s = "a.*,b02.b02Name,p28.p28Name,'p28/?pid=' + convert(varchar(10), a.p28ID) as p28"; }
                    
                    s = String.Format("SELECT {0},{1} FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID", GetSQL_SELECT_Ocas("p21"),s);
                    break;
                case "o12":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.o12Entity) as EntityAlias from o12Category a", GetSQL_SELECT_Ocas("o12"));
                    break;
                case "p10":
                    if (s == "") { s = "a.*,o12.o12Name,b02.b02Name,p13.p13Code,p13.p13Name,'p13/?pid='+convert(varchar(10),a.p13ID) as p13"; }
                 
                    s = string.Format("SELECT {0},{1} FROM p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID", GetSQL_SELECT_Ocas("p10"), s);
                    break;
                case "p13":
                    s = string.Format("SELECT {0},a.* from p13MasterTpv a", GetSQL_SELECT_Ocas("p13"));
                    break;
                case "p14":
                    s = string.Format("SELECT {0},a.* from p14MasterOper a", GetSQL_SELECT_Ocas("p14"));                    
                    break;
                case "o23":
                    if (s == "")
                    {
                        s = "a.*,o12.o12Name,b02.b02Name,dbo.getEntityAlias(a.o23Entity) as EntityAlias,dbo.getRecordAlias(a.o23Entity,a.o23RecordPid) as RecordUrlName,a.o23Entity+'/?pid='+convert(varchar(10),a.o23RecordPid) as RecordUrl";
                    }                    
                    s = string.Format("SELECT {0},{1} FROM o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID", GetSQL_SELECT_Ocas("o23"), s);
                    break;
                default:
                    break;
            }

          
            //parametrický dotaz s WHERE klauzulí
            
            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(s,mq,true);
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }

        
    }
}
