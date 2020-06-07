﻿using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip41TaskBL
    {
        public BO.p41Task Load(int pid);
        public BO.p41Task LoadByCode(string strCode, int intExcludePID);
        public IEnumerable<BO.p41Task> GetList(BO.myQuery mq);
        public int Save(BO.p41Task rec);
        public bool ValidateBeforeSave(BO.p41Task rec, string premessage = "");
        public int SaveBatch(List<BO.p41Task> lisP41);
        public string EstimateTaskCode(string strP52Code, int x);
        public int AppendPos(BO.p41Task recP41, List<BO.p18OperCode> lisP18, int p18flag);
    }
    class p41TaskBL : BaseBL, Ip41TaskBL
    {
        public p41TaskBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p41") + ",b02.b02Name,p11.p11Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p26.p26Name,p52.p52Code,p27.p27Name,p51.p51Code,p52.p11ID FROM p41Task a INNER JOIN p52OrderItem p52 ON a.p52ID=p52.p52ID INNER JOIN p27MszUnit p27 ON a.p27ID=p27.p27ID INNER JOIN p11ClientProduct p11 ON p52.p11ID=p11.p11ID INNER JOIN p51Order p51 ON p52.p51ID=p51.p51ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON p51.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON p27.p26ID=p26.p26ID";
        }

        public string EstimateTaskCode(string strP52Code, int x)
        {
            if (x == 0) x = 1;
            string strCode = strP52Code.Replace("R", "T") + "." + BO.BAS.RightString("000" + x.ToString(), 3);
            while (LoadByCode(strCode, 0) != null)
            {
                x += 1;
                strCode = strP52Code.Replace("R", "T") + "." + BO.BAS.RightString("000" + x.ToString(), 3);
            }

            return strCode;
        }
        public BO.p41Task Load(int pid)
        {
            return _db.Load<BO.p41Task>(string.Format("{0} WHERE a.p41ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p41Task LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p41Task>(string.Format("{0} WHERE a.p41Code LIKE @code AND a.p41ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p41Task> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p41Task>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p41Task rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p41ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p27ID", rec.p27ID, true);
            p.AddInt("p52ID", rec.p52ID, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddBool("p41IsDraft", rec.p41IsDraft);
            p.AddString("p41Name", rec.p41Name);
            p.AddString("p41Code", rec.p41Code);
            p.AddString("p41Memo", rec.p41Memo);
            p.AddString("p41StockCode", rec.p41StockCode);

            p.AddDateTime("p41PlanStart", rec.p41PlanStart);
            //p.AddDouble("p41Duration", rec.p41Duration);
            //p.AddDateTime("p41PlanEnd", rec.p41PlanStart.AddMinutes(rec.p41Duration));
            //p.AddDouble("p41TotalDuration", rec.p41TotalDuration);
            //p.AddDateTime("p41RealStart", rec.p41RealStart);
            //p.AddDateTime("p41RealEnd", rec.p41RealEnd);

            p.AddDouble("p41PlanUnitsCount", rec.p41PlanUnitsCount);
            //p.AddDouble("p41RealUnitsCount", rec.p41RealUnitsCount);            

            int intPID = _db.SaveRecord("p41Task", p.getDynamicDapperPars(), rec);

            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("pid", intPID, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("p41_after_save", ref pars);

            return intPID;
        }

        public int SaveBatch(List<BO.p41Task> lisP41)
        {
            int x = 1;
            int intErrs = 0;
            int intLastP52ID = 0;

            foreach (var rec in lisP41.OrderBy(p => p.p52ID).ThenBy(p => p.p41PlanEnd))
            {
                var cP52 = _mother.p52OrderItemBL.Load(rec.p52ID);
                if (cP52 != null)
                {

                    if (string.IsNullOrEmpty(rec.p41Code) == true)
                    {
                        rec.p41Code = EstimateTaskCode(cP52.p52Code, x);
                    }
                    if (String.IsNullOrEmpty(rec.p41Name) == true && cP52 != null)
                    {
                        rec.p41Name = cP52.p11Name + " [" + cP52.p11Code + "]";

                    }
                }

                if (ValidateBeforeSave(rec, string.Format("Zakázka #{0}: ", x)) == false)
                {
                    intErrs += 1;
                }
                x += 1;
                intLastP52ID = rec.p52ID;
            }

            if (intErrs > 0)
            {
                return 0;
            }

            x = 0;
            foreach (var rec in lisP41)
            {
                if (Save(rec) > 0)
                {
                    x += 1;
                }

            }
            return x;
        }

        public bool ValidateBeforeSave(BO.p41Task rec, string premessage = "")
        {
            if (String.IsNullOrEmpty(rec.p41Name) || string.IsNullOrEmpty(rec.p41Code))
            {
                _db.CurrentUser.AddMessage(premessage + "Chybí vyplnit název nebo kód zakázky.");
                return false;
            }
            if (rec.p27ID == 0 || rec.p52ID == 0)
            {
                _db.CurrentUser.AddMessage(premessage + "Na vstupu chybí středisko nebo objednávka.");
                return false;
            }
            if (rec.p41PlanUnitsCount <= 0)
            {
                _db.CurrentUser.AddMessage(premessage + "Plánované množství musí být větší než NULA.");
                return false;
            }
            if (rec.p41PlanUnitsCount > 0)
            {
                var c = _mother.p27MszUnitBL.Load(rec.p27ID);
                if (c.p27Capacity < rec.p41PlanUnitsCount)
                {
                    _db.CurrentUser.AddMessage(premessage + "Plánované množství nesmí být větší než kapacita střediska.");
                    return false;
                }
                double n = _db.Load<BO.COM.GetDouble>("select sum(p41PlanUnitsCount) as Value FROM p41Task WHERE p52ID=@p52id AND p41ID<>@pid", new { p52id = rec.p52ID, pid = rec.pid }).Value;
                double limit = _mother.p52OrderItemBL.Load(rec.p52ID).Recalc2Kg;
                if (limit < n + rec.p41PlanUnitsCount)
                {
                    _db.CurrentUser.AddMessage(premessage + "Plánované množství zakázky nesmí překročit objednané množství.");
                    return false;
                }
            }
            if (rec.p41PlanStart == null)
            {
                _db.CurrentUser.AddMessage(premessage + "Chybí vyplnit čas plánovaného zahájení.");
                return false;
            }
            
            if (String.IsNullOrEmpty(rec.p41Code) == false)
            {
                if (LoadByCode(rec.p41Code, rec.pid) != null)
                {
                    _db.CurrentUser.AddMessage(string.Format(premessage + "Zadaný kód {0} nemůže být duplicitní s jinou zákazkou [{1}].", rec.p41Code, LoadByCode(rec.p41Code, rec.pid).p41Name));
                    return false;
                }
            }


            return true;
        }


        public int AppendPos(BO.p41Task recP41, List<BO.p18OperCode> lisP18, int p18flag)   //uložit do plánu PO operace p18flag>1
        {
            if (recP41 == null)
            {
                _mother.CurrentUser.AddMessage("Chybí vazba na zakázku.");
                return 0;
            }
            if (lisP18.Where(p => p.p18Flag <= 1).Count() > 0)
            {
                _mother.CurrentUser.AddMessage("Do plánu lze vkládat pouze [PO] operace.");
                return 0;
            }

            _db.RunSql("DELETE FROM p44TaskOperPlan WHERE p41ID=@p41id AND p18ID IN (select p18ID FROM p18OperCode WHERE p18flag=@p18flag)", new { p41id = recP41.pid, p18flag = p18flag });
            
            int x = 0;
            if (p18flag == 3)
            {   //u POST operace zjistit poslední použité OperNum z TO operací a poté ho navyšovat o 10
                x = _db.Load<BO.COM.GetInteger>("SELECT MAX(p44OperNum) as Value FROM p44TaskOperPlan WHERE p41ID=@p41id AND p18ID IN (select p18ID FROM p18OperCode WHERE p18flag=1)", new { p41id = recP41.pid }).Value;
            }
            int rets = 0;
            foreach (var c in lisP18.Where(p => p.p18Flag == p18flag).OrderBy(p => p.p18Code))
            {
                var rec = new BO.p44TaskOperPlan() { p18ID = c.pid, p19ID = c.p19ID, p41ID = recP41.pid };

                var p = new DL.Params4Dapper();
                p.AddInt("p41ID", recP41.pid, true);
                p.AddInt("p19ID", rec.p19ID, true);
                p.AddInt("p18ID", rec.p18ID, true);
                p.AddDouble("p44DurationPreOper", c.p18DurationPreOper);
                p.AddDouble("p44DurationOper", c.p18DurationOper);
                p.AddDouble("p44DurationPostOper", c.p18DurationPostOper);
                if (p18flag == 2)
                {   //u PRE operace je 1-9 použitelných pozic
                    rec.p44RowNum = -1000 + x;
                    rec.p44OperNum = x + 1;
                    x += 1;
                }
                if (p18flag == 3)
                {   //u POST operace navyšovat stále o 10
                    rec.p44RowNum = x + 900;
                    rec.p44OperNum = x + 10;
                    x += 10;
                }
                p.AddInt("p44RowNum", rec.p44RowNum);
                p.AddInt("p44OperNum", rec.p44OperNum);

                if (_db.SaveRecord("p44TaskOperPlan", p.getDynamicDapperPars(), rec) > 0)
                {
                    rets += 1;
                }

            }
            if (lisP18.Count() == 0)
            {
                rets= 1;   //pouze vyčištění
            }
            _db.RunSql("update a set p44RowNum=RowID from (SELECT ROW_NUMBER() OVER(ORDER BY p44RowNum ASC) AS RowID,* FROM p44TaskOperPlan WHERE p41ID=@p41id) a", new { p41id = recP41.pid });


            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("pid", recP41.pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("p41_after_save", ref pars);

            return rets;

        }
    }
}
