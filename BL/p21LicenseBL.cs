using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip21LicenseBL
    {
        public BO.p21License Load(int pid);
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq);
        public int Save(BO.p21License rec, List<int> p10ids = null);
        BO.Result CreateClientProducts(int intP21ID);
    }
    class p21LicenseBL:BaseBL,Ip21LicenseBL
    {
        public p21LicenseBL(BL.Factory mother):base(mother)
        {
           
        }
      
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p21") + ",b02.b02Name,p28.p28Name FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p21License Load(int pid)
        {
            return _db.Load<BO.p21License>(string.Format("{0} WHERE a.p21ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p21License LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p21License>(string.Format("{0} WHERE a.p21Code LIKE @code AND a.p21ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p21License>(fq.FinalSql, fq.Parameters);
            
        }
        

        public int Save(BO.p21License rec,List<int> p10ids)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p21ID);
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("p21", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("p28ID", rec.p28ID, true);

            p.AddEnumInt("p21PermissionFlag", rec.p21PermissionFlag);
            p.AddString("p21Name", rec.p21Name);
            p.AddString("p21Code", rec.p21Code);
            p.AddString("p21Memo", rec.p21Memo);
            p.AddDouble("p21Price", rec.p21Price);
            p.AddDateTime("ValidFrom", rec.ValidFrom);
            p.AddDateTime("ValidUntil", rec.ValidUntil);

            int intPID= _db.SaveRecord("p21License", p.getDynamicDapperPars(), rec);
            if (intPID>0 && p10ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM p22LicenseBinding WHERE p21ID=@pid", new { pid = rec.pid });
                }
                if (p10ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO p22LicenseBinding(p21ID,p10ID) SELECT @pid,p10ID FROM p10MasterProduct WHERE p10ID IN (" + string.Join(",", p10ids) + ")", new { pid = intPID });
                }
                
            }

            return intPID;

            //var strGUID = BO.BAS.GetGuid();
            //BO.p85Tempbox cP85;

            //cP85 = new BO.p85Tempbox() {p85RecordPid=rec.pid, p85GUID = strGUID, p85Prefix = "p21", p85FreeText01 = rec.p21Name, p85FreeText02 = rec.p21Code, p85Message = rec.p21Memo, p85OtherKey1 = rec.p28ID, p85OtherKey2 = rec.b02ID, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil,p85FreeNumber01=rec.p21Price };
            //_mother.p85TempboxBL.Save(cP85);

            //foreach (var p10id in p10ids)
            //{
            //    cP85= new BO.p85Tempbox() { p85GUID = strGUID, p85Prefix = "p22", p85OtherKey1 = p10id };                
            //    _mother.p85TempboxBL.Save(cP85);
            //}

            //var p = new Dapper.DynamicParameters();
            //p.Add("userid", _db.CurrentUser.pid);                 
            //p.Add("guid", strGUID);
            //p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            //p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            //string s1 = _db.RunSp("p21_save",ref p);
            //if (s1 == "1")
            //{
            //    return p.Get<int>("pid_ret");
            //}
            //else
            //{                                
            //    return 0;
            //}           

        }

        public BO.Result CreateClientProducts(int intP21ID)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("userid", _db.CurrentUser.pid);            
            p.Add("p21id", intP21ID, System.Data.DbType.Int32);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = _db.RunSp("p21_create_clientproducts", ref p);
            if (s1 == "1")
            {
                return new BO.Result(false,"Operace proběhla");
            }
            else
            {
                return new BO.Result(true, s1);
               
            }

        }



    }
}

