using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ix31ReportBL
    {
        public BO.x31Report Load(int pid);
        public BO.x31Report LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.x31Report> GetList(BO.myQuery mq);
        public int Save(BO.x31Report rec);


    }
    class x31ReportBL : BaseBL, Ix31ReportBL
    {
        public x31ReportBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,");
            sb(_db.GetSQL1_Ocas("x31"));
            sb(" FROM x31Report a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID");
            sb(strAppend);
            return sbret();
        }
        public BO.x31Report Load(int pid)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31ID=@pid"), new { pid = pid });
        }
        public BO.x31Report LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31PID LIKE @code AND a.x31ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }


        public IEnumerable<BO.x31Report> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.x31Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x31Report>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x31Report rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x31ID);
            p.AddString("x31Entity", rec.x31Entity);
            p.AddString("x31FileName", rec.x31FileName);
            p.AddString("x31Name", rec.x31Name);
            p.AddString("x31Code", rec.x31Code);
            
            p.AddEnumInt("x31ReportFormat", rec.x31ReportFormat);
            p.AddString("x31Description", rec.x31Description);
            p.AddBool("x31Is4SingleRecord", rec.x31Is4SingleRecord);

            int intPID = _db.SaveRecord("x31Report", p.getDynamicDapperPars(), rec);
           
           

            return intPID;
        }

        public bool ValidateBeforeSave(BO.x31Report rec)
        {
            if (string.IsNullOrEmpty(rec.x31Name))
            {
                this.AddMessage("Chybí vyplnit [Název sestavy]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x31Code))
            {
                this.AddMessage("Chybí vyplnit [Kód sestavy]."); return false;
            }
            
            if (rec.x31ReportFormat==BO.x31ReportFormatEnum.DOC && rec.x31Is4SingleRecord == false)
            {
                this.AddMessage("Sestavy formátu DOCX mohou být pouze kontextové k vybranému záznamu");return false;
            }
          
            

            return true;
        }

       

    }
}
