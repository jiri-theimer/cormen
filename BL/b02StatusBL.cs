using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib02StatusBL
    {
        public BO.b02Status Load(int pid);
        public BO.b02Status LoadByCode(string b02code);
        public int LoadStartStatusPID(string entityprefix,int defaultB02ID);
        public IEnumerable<BO.b02Status> GetList(BO.myQuery mq);
        public int Save(BO.b02Status rec);
    }
    class b02StatusBL : BaseBL,Ib02StatusBL
    {        
        public b02StatusBL(BL.Factory mother) : base(mother)
        {
       
        }
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("b02") + " FROM b02Status a";
        }
        public BO.b02Status Load(int pid)
        {
            return _db.Load<BO.b02Status>(string.Format("{0} WHERE a.b02ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.b02Status LoadByCode(string b02code)
        {
            return _db.Load<BO.b02Status>(string.Format("{0} WHERE a.b02Code LIKE @code", GetSQL1()), new { code = b02code });
        }
        public int LoadStartStatusPID(string entityprefix, int defaultB02ID)
        {
            BO.COM.GetInteger c = _db.Load<BO.COM.GetInteger>("SELECT b02ID as Value FROM b02Status WHERE b02StartFlag=1 AND b02Entity like @prefix", new { prefix = entityprefix });
            if (c == null || c.Value==0)
            {
                return defaultB02ID;
            }
            return c.Value;
        }
        public IEnumerable<BO.b02Status> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b02Status>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.b02Status rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b02ID);         
            p.AddString("b02Name", rec.b02Name);
            p.AddString("b02Code", rec.b02Code);
            p.AddString("b02Entity", rec.b02Entity);
            p.AddInt("b02Ordinary", rec.b02Ordinary);
            p.AddString("b02Memo", rec.b02Memo);
            p.AddEnumInt("b02StartFlag", rec.b02StartFlag);
            p.AddEnumInt("b02MoveFlag", rec.b02MoveFlag);
            p.AddString("b02MoveBySql", rec.b02MoveBySql);

            int intPID= _db.SaveRecord("b02Status", p.getDynamicDapperPars(), rec);
            if (rec.b02StartFlag == BO.b02StartFlagENUM.DefaultStatus)
            {
                _db.RunSql("UPDATE b02Status SET b02StartFlag=0 WHERE b02Entity LIKE @entity AND b02ID<>@pid", new {entity=rec.b02Entity, pid = intPID });
            }
            return intPID;
        }


    }
}
