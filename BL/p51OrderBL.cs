using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip51OrderBL
    {
        public BO.p51Order Load(int pid);
        public IEnumerable<BO.p51Order> GetList(BO.myQuery mq);
        public int Save(BO.p51Order rec, List<BO.p52OrderItem> newitems);

        //public void AdjustItemsCode(BO.p51Order rec);
    }
    class p51OrderBL : BaseBL, Ip51OrderBL
    {
        public p51OrderBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p51") + ",b02.b02Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p28.p28Code FROM p51Order a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
        }
       
        public BO.p51Order Load(int pid)
        {
            BO.p51Order c= _db.Load<BO.p51Order>(string.Format("{0} WHERE a.p51ID=@pid", GetSQL1()), new { pid = pid });
            c.TagHtml = _mother.o51TagBL.GetTagging("p51Order", pid).TagHtml;
            return c;
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

        

        public int Save(BO.p51Order rec,List<BO.p52OrderItem> newitems)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            if (newitems != null)
            {
                if (newitems.Where(p => p.p11ID == 0).Count() > 0)
                {
                    _mother.CurrentUser.AddMessage("V položkách je nevyplněný produkt.");return 0;
                }
                if (newitems.Where(p => p.p52UnitsCount<= 0).Count() > 0)
                {
                    _mother.CurrentUser.AddMessage("V položkách je nekorektně zadané množství produktu."); return 0;
                }
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);            
            p.AddInt("p28ID", rec.p28ID, true);
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("p51", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddBool("p51IsDraft", rec.p51IsDraft);
            p.AddString("p51Name", rec.p51Name);
            p.AddString("p51Code", rec.p51Code);
            p.AddString("p51Memo", rec.p51Memo);
            p.AddString("p51CodeByClient", rec.p51CodeByClient);
           
            p.AddDateTime("p51Date", rec.p51Date);
            p.AddDateTime("p51DateDelivery", rec.p51DateDelivery);
            p.AddDateTime("p51DateDeliveryConfirmed", rec.p51DateDeliveryConfirmed);

            int intP51ID= _db.SaveRecord("p51Order", p.getDynamicDapperPars(), rec);

            if (newitems != null)
            {
                foreach (var c in newitems)
                {
                    c.p51ID = intP51ID;
                    _mother.p52OrderItemBL.Save(c);

                    //_db.RunSql("INSERT INTO p52OrderItem(p51ID,p11ID,p52UnitsCount) VALUES(@p51id,@p11id,@unitscount)", new { p51id = intP51ID, p11id = c.p11ID, unitscount = c.p52UnitsCount });
                }
            }

            

            return intP51ID;
        }

        private bool ValidateBeforeSave(BO.p51Order rec)
        {
            if (LoadByCode(rec.p51Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jinou objednávkou [{0}].", LoadByCode(rec.p51Code, rec.pid).p51Name));
                return false;
            }

            if (rec.p28ID==0 && string.IsNullOrEmpty(rec.p51Name) == true)
            {
                _db.CurrentUser.AddMessage("Musíte vyplnit klienta nebo název objednávky.");
                return false;
            }

            return true;
        }


        

        //public void AdjustItemsCode(BO.p51Order rec)
        //{
        //    _db.RunSql("update a set p52Code=@s+'.'+convert(varchar(10),RowID) from (SELECT ROW_NUMBER() OVER(ORDER BY p52id ASC) AS RowID,* FROM p52OrderItem WHERE p51ID=@p51id) a", new { s = rec.p51Code, p51id = rec.p51ID });

        //}


    }
}

