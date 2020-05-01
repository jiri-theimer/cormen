using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io23DocBL
    {
        public BO.o23Doc Load(int pid);
        public IEnumerable<BO.o23Doc> GetList(BO.myQuery mq);
        public int Save(BO.o23Doc rec, List<BO.o27Attachment> lisO27_Append, List<int> o27IDs_Remove = null);
        public IEnumerable<BO.o27Attachment> GetListO27(int intO23ID);
        public BO.o27Attachment LoadO27ByGuid(string strGUID);
    }
    class o23DocBL:BaseBL,Io23DocBL
    {
        public o23DocBL(BL.Factory mother):base(mother)
        {
            
        }
       
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("o23") + ",o12.o12Name,b02.b02Name,dbo.getRecordAlias(a.o23Entity,a.o23RecordPid) as RecordPidAlias FROM o23Doc a LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.o23Doc Load(int pid)
        {
            return _db.Load<BO.o23Doc>(string.Format("{0} WHERE a.o23ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.o23Doc> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.o23Doc>(fq.FinalSql, fq.Parameters);

        }
        public BO.o27Attachment LoadO27ByGuid(string strGUID)
        {
            return _db.Load<BO.o27Attachment>("SELECT a.*," + _db.GetSQL1_Ocas("o27") + " FROM o27Attachment a WHERE a.o27GUID=@guid", new { guid = strGUID });
        }
        public IEnumerable<BO.o27Attachment> GetListO27(int intO23ID)
        {
            return _db.GetList<BO.o27Attachment>("SELECT a.*,"+_db.GetSQL1_Ocas("o27")+" FROM o27Attachment a WHERE a.o23ID=@pid", new { pid = intO23ID });
        }

        public int Save(BO.o23Doc rec,List<BO.o27Attachment> lisO27_Append, List<int> o27IDs_Remove = null)
        {
            if (rec.o23RecordPid == 0)
            {
                _db.CurrentUser.AddMessage( "Chybí vyplnit svázaný záznam k dokumentu!");
                return 0;
            }
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.o23ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;
            p.Add("j02ID_Owner", BO.BAS.TestIntAsDbKey(rec.j02ID_Owner));
            p.Add("o23RecordPid", BO.BAS.TestIntAsDbKey(rec.o23RecordPid));
            p.Add("o23Entity", rec.o23Entity);
            p.Add("o12ID", BO.BAS.TestIntAsDbKey(rec.o12ID));
            p.Add("b02ID", BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("o23Date", rec.o23Date);
            p.Add("o23Name", rec.o23Name);
            p.Add("o23Code", rec.o23Code);
            p.Add("o23Entity", rec.o23Entity);
            p.Add("o23Memo", rec.o23Memo);


            int intO23ID= _db.SaveRecord( "o23Doc", p, rec);
            if (intO23ID > 0 && lisO27_Append != null && lisO27_Append.Count>0)
            {
                foreach(var c in lisO27_Append)
                {
                    p= new Dapper.DynamicParameters();
                    p.Add("pid", 0);
                    p.Add("o23ID", intO23ID);
                    p.Add("o27Name", c.o27Name);
                    p.Add("o27ArchiveFileName", c.o27ArchiveFileName);
                    p.Add("o27ArchiveFolder", c.o27ArchiveFolder);
                    p.Add("o27FileSize", c.o27FileSize);
                    p.Add("o27ContentType", c.o27ContentType);
                    p.Add("o27GUID", c.o27GUID);
                    _db.SaveRecord( "o27Attachment", p, c);
                }
            }
            if (intO23ID>0 && o27IDs_Remove !=null && o27IDs_Remove.Count > 0)
            {
                foreach(var intO27ID in o27IDs_Remove)
                {
                    _db.RunSql("DELETE FROM o27Attachment WHERE o27ID=@o27id", new { o27id = intO27ID });
                }
            }

            return intO23ID;
        }
    }
}
