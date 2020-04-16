using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(string strEntity, BO.myQuery mq=null);

        
    }
    class DataGridBL:IDataGridBL
    {
        private BO.RunningUser _cUser;
        public DataGridBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL_SELECT_Ocas(string strPrefix)
        {
            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity,'{0}/?pid='+convert(varchar(10),a.{0}ID) as {0}", strPrefix);
        }
        public DataTable GetList(string strEntity,BO.myQuery mq=null)
        {
            string s = "";            
            switch (strEntity)
            {
                case "j02":
                    s = string.Format("SELECT {0},a.*,j04.j04Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28,p28.p28Name,a.j02LastName+' '+a.j02FirstName+ISNULL(' '+a.j02TitleBeforeName,' ') as fullname_desc from j02Person a LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", GetSQL_SELECT_Ocas("j02"));
                    break;
                case "j04":
                    s = string.Format("SELECT {0},a.* from j04UserRole a", GetSQL_SELECT_Ocas("j04"));
                    break;
                case "b02":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.b02Entity) as EntityAlias from b02Status a", GetSQL_SELECT_Ocas("b02"));                   
                    break;
                case "p28":
                    s = string.Format("SELECT {0},a.* from p28Company a", GetSQL_SELECT_Ocas("p28"));
                    break;
                case "p26":
                    s = string.Format("SELECT {0},a.*,b02.b02Name,p28.p28Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28 FROM p26Msz a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", GetSQL_SELECT_Ocas("p26"));
                    break;
                case "p21":
                    s = string.Format("SELECT {0},a.*,b02.b02Name,p28.p28Name,'p28/?pid='+convert(varchar(10),a.p28ID) as p28 FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID", GetSQL_SELECT_Ocas("p21"));
                    break;
                case "o12":
                    s = string.Format("SELECT {0},a.*,dbo.getEntityAlias(a.o12Entity) as EntityAlias from o12Category a", GetSQL_SELECT_Ocas("o12"));
                    break;
                case "p10":
                    s = string.Format("SELECT {0},a.*,o12.o12Name,b02.b02Name,p13.p13Name,'p13/?pid='+convert(varchar(10),a.p13ID) as p13 FROM p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID", GetSQL_SELECT_Ocas("p10"));
                    break;
                case "p13":
                    s = string.Format("SELECT {0},a.* from p13MasterTpv a", GetSQL_SELECT_Ocas("p13"));
                    break;
                case "o23":
                    s = string.Format("SELECT {0},a.*,o12.o12Name,b02.b02Name,dbo.getEntityAlias(a.o23Entity) as EntityAlias,dbo.getRecordAlias(a.o23Entity,a.o23RecordPid) as RecordUrlName,a.o23Entity+'/?pid='+convert(varchar(10),a.o23RecordPid) as RecordUrl", GetSQL_SELECT_Ocas("o23"));
                    s+= " FROM o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID";
                    break;
                default:
                    break;
            }

            if (mq == null)
            {
                return DL.DbHandler.GetDataTable(s);
            }
            //parametrický dotaz s WHERE klauzulí
            if (mq.Entity == "") mq.Entity = strEntity;
            DL.FinalSqlCommand q = DL.basQuery.ParseFinalSql(s,mq,null,true);
            
            return DL.DbHandler.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }

        
    }
}
