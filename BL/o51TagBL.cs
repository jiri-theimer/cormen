using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Io51TagBL
    {
        public BO.o51Tag Load(int pid);
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq);
        public IEnumerable<BO.o51Tag> GetList(string record_entity, int record_pid);
        public BO.TaggingHelper GetTagging(string record_entity, int record_pid);
        public int Save(BO.o51Tag rec);
        public int SaveTagging(string record_entity, int record_pid, string o51ids);
    }
    class o51TagBL : BaseBL, Io51TagBL
    {
        public o51TagBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,o51_o53.o53Name," + _db.GetSQL1_Ocas("o51") + " FROM o51Tag a LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID";
        }
        public BO.o51Tag Load(int pid)
        {
            return _db.Load<BO.o51Tag>(string.Format("{0} WHERE a.o51ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o51Tag>(fq.FinalSql, fq.Parameters);

        }
        public BO.TaggingHelper GetTagging(string record_entity, int record_pid)
        {
            record_entity = record_entity.Substring(0, 3);
            IEnumerable<BO.o51Tag> lis = GetList(record_entity, record_pid);
            string s = String.Join(",", lis.Select(p => p.pid));

            var ret = new BO.TaggingHelper() { Tags = lis };
            if (lis.Count() > 0)
            {                
                ret.TagPids = String.Join(",", lis.Select(p => p.pid));
                ret.TagNames = String.Join(",", lis.Select(p => p.o51Name));
                ret.TagHtml = String.Join("", lis.Select(p => p.HtmlText));
            }
            
            return ret;
        }
        public IEnumerable<BO.o51Tag> GetList(string record_entity,int record_pid)
        {

            return _db.GetList<BO.o51Tag>(GetSQL1()+ " INNER JOIN o52TagBinding o52 ON a.o51ID=o52.o51ID WHERE o52.o52RecordPid=@pid AND o52.o52RecordEntity=@entity", new { pid = record_pid, entity = record_entity });

        }

        public int SaveTagging(string record_entity, int record_pid, string o51ids)
        {
            record_entity = record_entity.Substring(0, 3);
            _db.RunSql("DELETE FROM o52TagBinding WHERE o52RecordPid=@pid AND o52RecordEntity=@entity", new { pid = record_pid, entity = record_entity });
            if (String.IsNullOrEmpty(o51ids) == false)
            {
                _db.RunSql("INSERT INTO o52TagBinding(o52RecordEntity,o52RecordPid,o51ID) SELECT @entity,@pid,o51ID FROM o51Tag WHERE o51ID IN (" + o51ids + ")", new { pid = record_pid, entity = record_entity });
            }
            
            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("record_entity", record_entity, System.Data.DbType.String);
            pars.Add("record_pid", record_pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("o51_tagging_after_save", ref pars);


            return 1;
        }

        public int Save(BO.o51Tag rec)
        {
            if (rec.o51Name.Contains(","))
            {
                _mother.CurrentUser.AddMessage("Název štítku nesmí obsahovat čárku.");
                return 0;
            }

            if (GetList(new BO.myQuery("o51Tag")).Where(p=>p.pid !=rec.pid && p.o51Name.ToLower() == rec.o51Name.Trim().ToLower()).Count()>0)
            {
                _mother.CurrentUser.AddMessage("Štítek s tímto názvem již existuje.");
                return 0;
            }

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.o51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("o53ID", rec.o53ID, true);
            p.AddString("o51Name", rec.o51Name);
            p.AddString("o51Code", rec.o51Code);
            p.AddString("o51Entities", rec.o51Entities);
            p.AddBool("o51IsColor", rec.o51IsColor);
            if (rec.o51IsColor == false)
            {
                rec.o51ForeColor = "";
                rec.o51BackColor = "";
            }            
            p.AddString("o51ForeColor", rec.o51ForeColor);
            p.AddString("o51BackColor", rec.o51BackColor);

            int intPID= _db.SaveRecord("o51Tag", p.getDynamicDapperPars(), rec);

            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("pid", intPID, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("o51_after_save", ref pars);

            return intPID;
        }
    }
}
