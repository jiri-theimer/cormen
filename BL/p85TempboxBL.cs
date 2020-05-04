using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip85TempboxBL
    {
        public BO.p85Tempbox Load(int pid);
        public BO.p85Tempbox LoadByGuid(string strGUID);
        public IEnumerable<BO.p85Tempbox> GetList(string strGUID, bool bolIncludeDeleted = false, string strPrefix = null);
        public int Save(BO.p85Tempbox rec);
        public BO.p85Tempbox VirtualDelete(int intPID);
    }
    class p85TempboxBL : BaseBL,Ip85TempboxBL
    {      
        public p85TempboxBL(BL.Factory mother):base(mother)
        {
         
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p85") + " FROM p85Tempbox a";
        }
        public BO.p85Tempbox Load(int pid)
        {
            return _db.Load<BO.p85Tempbox>(string.Format("{0} WHERE a.p85ID={1}", GetSQL1(), pid));
        }
        public BO.p85Tempbox LoadByGuid(string strGUID)
        {
            return _db.Load<BO.p85Tempbox>(string.Format("{0} WHERE a.p85GUID = @guid", GetSQL1()), new { guid = strGUID });
        }
        public IEnumerable<BO.p85Tempbox> GetList(string strGUID,bool bolIncludeDeleted=false,string strPrefix=null)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("guid", strGUID);
            var s = string.Format("{0} WHERE a.p85GUID=@guid", GetSQL1());
            if (bolIncludeDeleted==false) { s += " AND a.p85IsDeleted=0"; };
            if (strPrefix != null)
            {
                s += " AND a.p85Prefix=@prefix";
                p.Add("prefix", strPrefix);
            }
            return _db.GetList<BO.p85Tempbox>(s,p);
        }
        public BO.p85Tempbox VirtualDelete(int intPID)
        {
            return _db.Load<BO.p85Tempbox>("UPDATE p85Tempbox set p85Isdeleted=1 WHERE p85ID=@pid; "+string.Format("{0} WHERE a.p85ID=@pid", GetSQL1()),new { pid = intPID });
        }

        public int Save(BO.p85Tempbox rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p85ID);
            p.AddString("p85GUID", rec.p85GUID);
            p.AddString("p85Prefix", rec.p85Prefix);
            p.AddInt("p85RecordPid", rec.p85RecordPid,true);
            p.AddInt("p85ClonePid", rec.p85ClonePid);
            p.AddBool("p85IsDeleted", rec.p85IsDeleted);

            p.AddInt("p85OtherKey1",rec.p85OtherKey1,true);
            p.AddInt("p85OtherKey2",rec.p85OtherKey2,true);
            p.AddInt("p85OtherKey3",rec.p85OtherKey3,true);
            p.AddInt("p85OtherKey4", rec.p85OtherKey4,true);
            p.AddInt("p85OtherKey5", rec.p85OtherKey5,true);

            p.AddString("p85FreeText01", rec.p85FreeText01);
            p.AddString("p85FreeText02", rec.p85FreeText02);
            p.AddString("p85FreeText03", rec.p85FreeText03);
            p.AddString("p85FreeText04", rec.p85FreeText04);
            p.AddString("p85FreeText05", rec.p85FreeText05);
            p.AddString("p85FreeText06", rec.p85FreeText06);

            p.AddBool("p85FreeBoolean01", rec.p85FreeBoolean01);
            p.AddBool("p85FreeBoolean02", rec.p85FreeBoolean02);
            p.AddBool("p85FreeBoolean03", rec.p85FreeBoolean03);
            p.AddBool("p85FreeBoolean04", rec.p85FreeBoolean04);

            p.AddDouble("p85FreeNumber01", rec.p85FreeNumber01);
            p.AddDouble("p85FreeNumber02",rec.p85FreeNumber02 );
            p.AddDouble("p85FreeNumber03", rec.p85FreeNumber03 );
            p.AddDouble("p85FreeNumber04", rec.p85FreeNumber04 );
            p.AddDouble("p85FreeNumber05", rec.p85FreeNumber05 );
            p.AddDouble("p85FreeNumber06", rec.p85FreeNumber06 );

            p.AddDateTime("p85FreeDate01", rec.p85FreeDate01);
            p.AddDateTime("p85FreeDate02", rec.p85FreeDate02);
            p.AddDateTime("p85FreeDate03", rec.p85FreeDate03);
            p.AddDateTime("p85FreeDate04", rec.p85FreeDate04);

            return _db.SaveRecord( "p85Tempbox", p.getDynamicDapperPars(), rec);
        }
    }
}
