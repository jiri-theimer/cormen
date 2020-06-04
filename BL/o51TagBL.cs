using BO;
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
        public BO.TaggingHelper GetTagging(List<int> o51ids);
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
            return "SELECT a.*,o51_o53.o53Name,o51_o53.o53Entities,o51_o53.o53IsMultiSelect,o51_o53.o53Ordinary," + _db.GetSQL1_Ocas("o51") + " FROM o51Tag a LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID";
        }
        public BO.o51Tag Load(int pid)
        {
            return _db.Load<BO.o51Tag>(string.Format("{0} WHERE a.o51ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "o51_o53.o53Ordinary,o51_o53.o53Name,a.o51Ordinary,a.o51Name";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o51Tag>(fq.FinalSql, fq.Parameters);

        }
        
        public IEnumerable<BO.o51Tag> GetList(string record_entity,int record_pid)
        {

            return _db.GetList<BO.o51Tag>(GetSQL1()+ " INNER JOIN o52TagBinding o52 ON a.o51ID=o52.o51ID WHERE o52.o52RecordPid=@pid AND o52.o52RecordEntity=@entity ORDER BY o51_o53.o53Ordinary,o51_o53.o53Name,a.o51Ordinary,a.o51Name", new { pid = record_pid, entity = record_entity });

        }


        public BO.TaggingHelper GetTagging(string record_entity, int record_pid)
        {
            record_entity = record_entity.Substring(0, 3);
            return handle_tagging(GetList(record_entity, record_pid));
        }
        public BO.TaggingHelper GetTagging(List<int> o51ids)
        {
            var mq = new BO.myQuery("o51Tag");
            mq.pids = o51ids;
            return handle_tagging(GetList(mq));
        }
        private BO.TaggingHelper handle_tagging(IEnumerable<BO.o51Tag> lis)
        {
            string s = String.Join(",", lis.Select(p => p.pid));

            var ret = new BO.TaggingHelper() { Tags = lis };
            if (lis.Count() > 0)
            {
                ret.TagPids = String.Join(",", lis.Select(p => p.pid));
                int intLastGroup = 0;
                ret.TagHtml = "";
                ret.TagNames = "";
                var sb = new System.Text.StringBuilder();
                int x = 0;
                foreach (BO.o51Tag c in lis)
                {
                    if (intLastGroup != c.o53ID)
                    {
                        if (x > 0)
                        {
                            sb.Append("</div>");
                            sb.Append("</div>");
                        }
                        sb.Append("<div class='form-row'>");
                        sb.Append(string.Format("<label class='col-sm-1 col-md-2 col-form-label'>{0}★:</label>", c.o53Name));
                        sb.Append("<div class='col-sm-11 col-md-10'>");
                        if (ret.TagNames == "")
                        {
                            
                            ret.TagNames = c.o53Name + ": ";
                        }
                        else
                        {
                            
                            ret.TagNames += " ★" + c.o53Name + ": ";
                        }
                        ret.TagNames += c.o51Name;
                    }
                    else
                    {
                        ret.TagNames += ", " + c.o51Name;
                    }
                    
                    sb.Append(c.HtmlText);

                    x += 1;
                    intLastGroup = c.o53ID;
                }
                if (x > 0)
                {
                    sb.Append("</div>");
                    sb.Append("</div>");
                }
                ret.TagHtml = sb.ToString();
            }
            

            return ret;
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
                _mother.CurrentUser.AddMessage("Název položky kategorie nesmí obsahovat čárku.");
                return 0;
            }
            if (rec.o51Name.Length>30)
            {
                _mother.CurrentUser.AddMessage("V názvu položky kategorie může být maximálně 30 znaků.");
                return 0;
            }

            if (GetList(new BO.myQuery("o51Tag")).Where(p=>p.pid !=rec.pid && p.o51Name.ToLower() == rec.o51Name.Trim().ToLower()).Count()>0)
            {
                _mother.CurrentUser.AddMessage("Položka kategorie s tímto názvem již existuje.");
                return 0;
            }

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.o51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("o53ID", rec.o53ID, true);
            p.AddString("o51Name", rec.o51Name);
            p.AddString("o51Code", rec.o51Code);
            p.AddInt("o51Ordinary", rec.o51Ordinary);
            
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
