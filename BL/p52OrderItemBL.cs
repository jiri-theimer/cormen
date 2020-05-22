using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip52OrderItemBL
    {
       
        public BO.p52OrderItem Load(int pid);
        public IEnumerable<BO.p52OrderItem> GetList(int intP51ID);
        public int Save(BO.p52OrderItem rec);
    }
    class p52OrderItemBL : BaseBL, Ip52OrderItemBL
    {
        public p52OrderItemBL(BL.Factory mother) : base(mother)
        {

        }

     
        private string GetSQL()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p52") + ",dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p11.p11Name,p11.p11Code,p51.p51Code,p20.p20Code,p11.p11RecalcUnit2Kg FROM p52OrderItem a INNER JOIN p51Order p51 ON a.p51ID=p51.p51ID INNER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN p20Unit p20 ON p11.p20ID=p20.p20ID";
        }
       
        public BO.p52OrderItem Load(int pid)
        {
            return _db.Load<BO.p52OrderItem>(string.Format("{0} WHERE a.p52ID=@pid", GetSQL()), new { pid = pid });
        }
        public IEnumerable<BO.p52OrderItem> GetList(int intP51ID)
        {

            return _db.GetList<BO.p52OrderItem>(GetSQL() + " WHERE a.p51ID=@p51id", new { p51id = intP51ID });

        }
        public int Save(BO.p52OrderItem rec)
        {
            if (rec.p51ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí hlavička objednávky."); return 0;
            }
            if (rec.p11ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit produkt."); return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p52ID);
            p.AddInt("p51ID", rec.p51ID, true);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p11ID", rec.p11ID, true);
            p.AddDouble("p52UnitsCount", rec.p52UnitsCount);

            int intP52ID = _db.SaveRecord("p52OrderItem", p.getDynamicDapperPars(), rec);
            _mother.p51OrderBL.AdjustItemsCode(_mother.p51OrderBL.Load(rec.p51ID));


            return intP52ID;
        }

        


    }
}
