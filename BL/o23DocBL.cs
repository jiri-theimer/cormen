﻿using System;
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
            return "SELECT a.*," + _db.GetSQL1_Ocas("o23") + ",b02.b02Name,dbo.getRecordAlias(a.o23Entity,a.o23RecordPid) as RecordPidAlias,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner FROM o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.o23Doc Load(int pid)
        {
            return _db.Load<BO.o23Doc>(string.Format("{0} WHERE a.o23ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o23Doc> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
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
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.o23ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner,true);
            p.AddInt("o23RecordPid", rec.o23RecordPid,true);
            p.AddString("o23Entity", rec.o23Entity);
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("o23", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("b02ID", rec.b02ID,true);
            p.AddDateTime("o23Date", rec.o23Date);
            p.AddString("o23Name", rec.o23Name);
            p.AddString("o23Code", rec.o23Code);
            p.AddString("o23Entity", rec.o23Entity);
            p.AddString("o23Memo", rec.o23Memo);


            int intO23ID= _db.SaveRecord( "o23Doc", p.getDynamicDapperPars(), rec);
            if (intO23ID > 0 && lisO27_Append != null && lisO27_Append.Count>0)
            {
                foreach(var c in lisO27_Append)
                {
                    p = new DL.Params4Dapper();
                    p.AddInt("pid", 0);
                    p.AddInt("o23ID", intO23ID,true);
                    p.AddString("o27Name", c.o27Name);
                    p.AddString("o27ArchiveFileName", c.o27ArchiveFileName);
                    p.AddString("o27ArchiveFolder", c.o27ArchiveFolder);
                    p.AddInt("o27FileSize", c.o27FileSize);
                    p.AddString("o27ContentType", c.o27ContentType);
                    p.AddString("o27GUID", c.o27GUID);
                    _db.SaveRecord( "o27Attachment", p.getDynamicDapperPars(), c);
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
