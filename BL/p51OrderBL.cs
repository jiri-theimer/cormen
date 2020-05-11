using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip51OrderBL
    {
        public BO.p51Order Load(int pid);
        public IEnumerable<BO.p51Order> GetList(BO.myQuery mq);
        public int Save(BO.p51Order rec);
        public BO.p52OrderItem LoadOrderItem(int intP52ID);
        public IEnumerable<BO.p52OrderItem> GetList_OrderItems(int intP51ID);
        public int SaveOrderItem(BO.p52OrderItem rec);
    }
    class p51OrderBL : BaseBL, Ip51OrderBL
    {
        public p51OrderBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p51") + ",b02.b02Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p26.p26Name FROM p51Order a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        private string GetSQL2()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p52") + ",dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p11.p11Name,p11.p11Code,p51.p51Code FROM p52OrderItem a INNER JOIN p51Order p51 ON a.p51ID=p51.p51ID INNER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID";
        }
        public BO.p51Order Load(int pid)
        {
            return _db.Load<BO.p51Order>(string.Format("{0} WHERE a.p51ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p51Order LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p51Order>(string.Format("{0} WHERE a.p51Code LIKE @code AND a.p51ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        
        public IEnumerable<BO.p51Order> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p51Order>(fq.FinalSql, fq.Parameters);

        }

        

        public int Save(BO.p51Order rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);            
            p.AddInt("p28ID", rec.p28ID, true);
            p.AddInt("p26ID", rec.p26ID, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddBool("p51IsDraft", rec.p51IsDraft);
            p.AddString("p51Name", rec.p51Name);
            p.AddString("p51Code", rec.p51Code);
            p.AddString("p51Memo", rec.p51Memo);
            p.AddString("p51CodeByClient", rec.p51CodeByClient);
           
            p.AddDateTime("p51Date", rec.p51Date);
            p.AddDateTime("p51DateDelivery", rec.p51DateDelivery);

            return _db.SaveRecord("p51Order", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p51Order rec)
        {
            if (LoadByCode(rec.p51Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jinou objednávkou [{0}].", LoadByCode(rec.p51Code, rec.pid).p51Name));
                return false;
            }

            return true;
        }


        public BO.p52OrderItem LoadOrderItem(int intP52ID)
        {
            return _db.Load<BO.p52OrderItem>(string.Format("{0} WHERE a.p52ID=@pid", GetSQL2()), new { pid = intP52ID });
        }
        public IEnumerable<BO.p52OrderItem> GetList_OrderItems(int intP51ID)
        {

            return _db.GetList<BO.p52OrderItem>(GetSQL2() + " WHERE a.p51ID=@p51id", new { p51id = intP51ID });

        }
        public int SaveOrderItem(BO.p52OrderItem rec)
        {
            if (rec.p51ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí hlavička objednávky.");return 0;                
            }
            if (rec.p11ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit produkt."); return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p52ID);
            p.AddInt("p51ID", rec.p51ID, true);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p11ID", rec.p11ID, true);
            p.AddDouble("p52UnitsCount",rec.p52UnitsCount);
            
            int intP52ID= _db.SaveRecord("p52OrderItem", p.getDynamicDapperPars(), rec);
            BO.p51Order cP51 = Load(rec.p51ID);

            _db.RunSql("update a set p52Code=@s+'.'+convert(varchar(10),RowID) from (SELECT ROW_NUMBER() OVER(ORDER BY p52id ASC) AS RowID,* FROM p52OrderItem WHERE p51ID=@p51id) a", new {s= cP51.p51Code, p51id = rec.p51ID });

            return intP52ID;
        }


    }
}

