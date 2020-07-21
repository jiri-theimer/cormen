using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheQueryFieldProvider
    {
        //private readonly BL.TheEntitiesProvider _ep;
        private string _Prefix;
        private List<BO.TheQueryField> _lis;
        private string _lastEntity;

        public TheQueryFieldProvider(string strPrefix)
        {
            _Prefix = strPrefix;
            //_ep = ep;
            _lis = new List<BO.TheQueryField>();
            SetupPallete();


        }
        public List<BO.TheQueryField> getPallete()
        {
            return _lis;
        }
        private void SetupPallete()
        {
            BO.TheQueryField of;
            switch (_Prefix)
            {
                case "p44":
                    
                    of=AF("p44TaskOperPlan", "p41-b02ID", "b02ID", "Stav zakázky", "b02Status", null, "multi");
                    of.SqlWrapper = "a.p41ID IN (select p41ID FROM p41Task WHERE #filter#)";

                    of = AF("p44TaskOperPlan", "p41-p27ID", "p27ID", "Zařízení", "p27MszUnit", null, "multi");
                    of.SqlWrapper = "a.p41ID IN (select p41ID FROM p41Task WHERE #filter#)";

                    of = AF("p44TaskOperPlan", "p41-p26ID", "xb.p26ID", "Skupina zařízení", "p26Msz", null, "multi");
                    of.SqlWrapper = "a.p41ID IN (select xa.p41ID FROM p41Task xa INNER JOIN p29MszUnitBinding xb ON xa.p27ID=xb.p27ID WHERE #filter#)";

                    of=AF("p44TaskOperPlan", "p41-p41PlanStart", "p41PlanStart", "Plán zahájení", null, null, "date");
                    of.SqlWrapper = "a.p41ID IN (select p41ID FROM p41Task WHERE #filter#)";

                    of=AF("p44TaskOperPlan", "p41-p41PlanEnd", "p41PlanEnd", "Plán dokončení", null, null, "date");
                    of.SqlWrapper = "a.p41ID IN (select p41ID FROM p41Task WHERE #filter#)";

                    of = AF("p44TaskOperPlan", "p41-p10TypeFlag", "p10.p10TypeFlag", "Typ produktu (1-6)", "p10MasterProduct", null, "number");
                    of.SqlWrapper = "a.p41ID IN (select xa.p41ID FROM p41Task xa INNER JOIN p52OrderItem p52 ON xa.p52ID=p52.p52ID INNER JOIN p11ClientProduct p11 ON p52.p11ID=p11.p11ID INNER JOIN p10MasterProduct p10 ON p11.p10ID_Master=p10.p10ID WHERE #filter#)";

                    AF("p44TaskOperPlan", "p44-p18ID", "a.p18ID", "Šablona kódu operace", "p18OperCode", null, "multi");
                    of = AF("p44TaskOperPlan", "p44-p19Code", "p18Code", "Kód operace", null, null, "string");
                    of.SqlWrapper = "a.p18ID IN (select p18ID FROM p18OperCode WHERE #filter#)";
                    of = AF("p44TaskOperPlan", "p44-p25ID", "p25ID", "Typ zařízení operace", "p25MszType", null, "multi");
                    of.SqlWrapper = "a.p18ID IN (select p18ID FROM p18OperCode WHERE #filter#)";

                    AF("p44TaskOperPlan", "p44-p19ID", "a.p19ID", "Surovina", "p19Material", null, "combo");                    
                    of = AF("p44TaskOperPlan", "p44-p19TypeFlag", "p19TypeFlag", "Typ suroviny (1-6)", null, null, "number");
                    of.SqlWrapper = "a.p19ID IN (select p19ID FROM p19Material WHERE #filter#)";
                    of = AF("p44TaskOperPlan", "p44-p19Code", "p19Code", "Kód suroviny", null, null, "string");
                    of.SqlWrapper = "a.p19ID IN (select p19ID FROM p19Material WHERE #filter#)";

                    

                    break;
                case "p41":
                    
                    AF("p41Task", "b02ID", "a.b02ID", "Workflow stav", "b02Status", null, "multi");
                    AF("p41Task", "p27ID", "a.p27ID", "Zařízení", "p27MszUnit", null, "multi");

                    of = AF("p41Task", "p41-p26ID", "p26ID", "Skupina zařízení", "p26Msz", null, "multi");
                    of.SqlWrapper = "a.p27ID IN (select p27ID FROM p29MszUnitBinding WHERE #filter#)";

                    of =AF("p41Task", "p41-p10ID", "p11.p10ID_Master", "Master produkt", "p10MasterProduct", null, "combo");
                    of.SqlWrapper = "a.p52ID IN (select p52.p52ID FROM p52OrderItem p52 INNER JOIN p11ClientProduct p11 ON p52.p11ID=p11.p11ID WHERE #filter#)";
                    
                    AF("p41Task", "j02ID_Owner", "a.j02ID_Owner", "Vlastník záznamu", "j02Person", null, "combo");
                    
                    AF("p41Task", "p41PlanStart", "a.p41PlanStart", "Plán zahájení", null, null, "date");
                    AF("p41Task", "p41PlanEnd", "a.a01DateUntil", "Plán dokončení", null, null, "date");
                    
                    AF("p41Task", "p41PlanUnitsCount", "a.p41PlanUnitsCount", "Plánované množství VJ", null, null, "number");

                    AF("p41Task", "p41RealStart", "a.p41RealStart", "Skutečnost zahájení", null, null, "date");
                    AF("p41Task", "p41RealEnd", "a.p41RealEnd", "Skutečnost dokončení", null, null, "date");

                   

                    break;

                case "p10":
                    AF("p10MasterProduct", "p10TypeFlag", "a.p10TypeFlag", "Typ produktu (1-6)",null, null, "number");
                    AF("p10MasterProduct", "b02ID", "a.b02ID", "Workflow stav", "b02Status", null, "multi");
                    of = AF("p10MasterProduct", "p10-p21ID", "p21ID", "Licence", "p21License", null, "multi");
                    of.SqlWrapper = "a.p10ID IN (select p10ID FROM p22LicenseBinding WHERE #filter#)";

                    AF("p10MasterProduct", "p20ID", "a.p20ID", "Měrná jednotka", "p20Unit", null, "multi");
                    AF("p10MasterProduct", "p20ID_Pro", "a.p20ID_Pro", "Výrobní měrná jednotka", "p20Unit", null, "multi");
                    break;

                default:
                    break;
            }




        }


        private BO.TheQueryField AF(string strEntity, string strField,string strSqlSyntax, string strHeader, string strSourceEntity = null, string strSourceSql = null, string strFieldType = "string")
        {
            if (strEntity != _lastEntity)
            {
                //zatím nic
            }

            _lis.Add(new BO.TheQueryField() { Field = strField,FieldSqlSyntax=strSqlSyntax, Entity = strEntity, Header = strHeader, FieldType = strFieldType, SourceEntity = strSourceEntity, SourceSql = strSourceSql, TranslateLang1 = strHeader, TranslateLang2 = strHeader, TranslateLang3 = strHeader });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }
    }
}
