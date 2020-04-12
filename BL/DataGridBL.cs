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
        private string GetSQL_SELECT_Ocas(string strPrefix)
        {
            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity", strPrefix);
        }
        public DataTable GetList(string strEntity,BO.myQuery mq=null)
        {
            string s = "";            
            switch (strEntity)
            {
                case "j02":
                    s = string.Format("SELECT {0},a.*,j04.j04Name,p28.p28Name,a.j02LastName+' '+a.j02FirstName+ISNULL(' '+a.j02TitleBeforeName,' ') as fullname_desc from j02Person a LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID", GetSQL_SELECT_Ocas("j02"));
                    break;
                case "j04":
                    s = string.Format("SELECT {0},a.* from j04UserRole a", GetSQL_SELECT_Ocas("j04"));
                    break;
                case "b02":
                    s = string.Format("SELECT {0},a.* from b02Status a", GetSQL_SELECT_Ocas("b02"));                   
                    break;
                case "p28":
                    s = string.Format("SELECT {0},a.* from p28Company a", GetSQL_SELECT_Ocas("p28"));
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
