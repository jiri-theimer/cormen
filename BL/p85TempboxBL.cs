using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip85TempboxBL
    {
        public BO.p85Tempbox Load(int pid);
        public BO.p85Tempbox LoadByGuid(string strGUID);
        public IEnumerable<BO.p85Tempbox> GetList(string strGUID, bool bolIncludeDeleted = false);
        public int Save(BO.p85Tempbox rec);
        public BO.p85Tempbox VirtualDelete(int intPID);
    }
    class p85TempboxBL : Ip85TempboxBL
    {
        private BO.RunningUser _cUser;
        public p85TempboxBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p85") + " FROM p85Tempbox a";
        }
        public BO.p85Tempbox Load(int pid)
        {
            return DL.DbHandler.Load<BO.p85Tempbox>(string.Format("{0} WHERE a.p85ID={1}", GetSQL1(), pid));
        }
        public BO.p85Tempbox LoadByGuid(string strGUID)
        {
            return DL.DbHandler.Load<BO.p85Tempbox>(string.Format("{0} WHERE a.p85GUID = @guid", GetSQL1()), new { guid = strGUID });
        }
        public IEnumerable<BO.p85Tempbox> GetList(string strGUID,bool bolIncludeDeleted=false)
        {
            var s = string.Format("{0} WHERE a.p85GUID=@guid", GetSQL1());
            if (bolIncludeDeleted==false) { s += " AND a.p85IsDeleted=0"; };
            return DL.DbHandler.GetList<BO.p85Tempbox>(s, new { guid = strGUID });


        }
        public BO.p85Tempbox VirtualDelete(int intPID)
        {
            return DL.DbHandler.Load<BO.p85Tempbox>("UPDATE p85Tempbox set p85Isdeleted=1 WHERE p85ID=@pid; "+string.Format("{0} WHERE a.p85ID=@pid", GetSQL1()),new { pid = intPID });
        }

        public int Save(BO.p85Tempbox rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p85ID);
            p.Add("p85GUID", rec.p85GUID);
            p.Add("p85Prefix", rec.p85Prefix);
            p.Add("p85RecordPid", BO.BAS.TestIntAsDbKey(rec.p85RecordPid));
            p.Add("p85ClonePid", BO.BAS.TestIntAsDbKey(rec.p85ClonePid));
            p.Add("p85IsDeleted", rec.p85IsDeleted);

            p.Add("p85OtherKey1", BO.BAS.TestIntAsDbKey(rec.p85OtherKey1));
            p.Add("p85OtherKey2", BO.BAS.TestIntAsDbKey(rec.p85OtherKey2));
            p.Add("p85OtherKey3", BO.BAS.TestIntAsDbKey(rec.p85OtherKey3));
            p.Add("p85OtherKey4", BO.BAS.TestIntAsDbKey(rec.p85OtherKey4));
            p.Add("p85OtherKey5", BO.BAS.TestIntAsDbKey(rec.p85OtherKey5));

            p.Add("p85FreeText01", rec.p85FreeText01);
            p.Add("p85FreeText02", rec.p85FreeText02);
            p.Add("p85FreeText03", rec.p85FreeText03);
            p.Add("p85FreeText04", rec.p85FreeText04);
            p.Add("p85FreeText05", rec.p85FreeText05);
            p.Add("p85FreeText06", rec.p85FreeText06);

            p.Add("p85FreeBoolean01", rec.p85FreeBoolean01);
            p.Add("p85FreeBoolean02", rec.p85FreeBoolean02);
            p.Add("p85FreeBoolean03", rec.p85FreeBoolean03);
            p.Add("p85FreeBoolean04", rec.p85FreeBoolean04);

            p.Add("p85FreeNumber01", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber01), System.Data.DbType.Double);
            p.Add("p85FreeNumber02", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber02), System.Data.DbType.Double);
            p.Add("p85FreeNumber03", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber03), System.Data.DbType.Double);
            p.Add("p85FreeNumber04", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber04), System.Data.DbType.Double);
            p.Add("p85FreeNumber05", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber05), System.Data.DbType.Double);
            p.Add("p85FreeNumber06", BO.BAS.TestDouleAsDbKey(rec.p85FreeNumber06), System.Data.DbType.Double);

            p.Add("p85FreeDate01", rec.p85FreeDate01, System.Data.DbType.DateTime);
            p.Add("p85FreeDate02", rec.p85FreeDate02, System.Data.DbType.DateTime);
            p.Add("p85FreeDate03", rec.p85FreeDate03, System.Data.DbType.DateTime);
            p.Add("p85FreeDate04", rec.p85FreeDate04, System.Data.DbType.DateTime);

            return DL.DbHandler.SaveRecord(_cUser, "p85Tempbox", p, rec);
        }
    }
}
