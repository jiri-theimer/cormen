using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace BL
{
    public class TheEntities
    {
        private static List<BO.TheEntity> _lis;

        public  static BO.TheEntity ByPrefix(string strPrefix)
        {
            TestPallete();

            return _lis.Where(p => p.Prefix == strPrefix).First();

        }
        public static BO.TheEntity ByTable(string strTableName)
        {
            TestPallete();

            return _lis.Where(p => p.TableName == strTableName).First();

        }

        private static void TestPallete()
        {
            if (_lis==null || _lis.Count == 0)
            {
                SetupPallete();
            }
        }

        private static void SetupPallete()
        {           
            _lis = new List<BO.TheEntity>();
            AE("j02Person", "Lidé", "Jméno", "j02Person a LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID");
            AE("j03User", "Uživatelé", "Uživatel", "j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON j02.p28ID=p28.p28ID");
            
            AE("p26Msz", "Stroje | MSZ", "Stroj | MSZ", "p26Msz a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN p31CapTemplate p31 ON a.p31ID=p31.p31ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p21License", "Licence", "Licence", "p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");

            AE("p18OperCode", "Kódy operací", "Kód operace", "p18OperCode a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID");
            AE("p19Material", "Materiály", "Materiál", "p19Material a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p10MasterProduct", "Master produkty", "Master produkt", "p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p13MasterTpv", "Master receptury", "Master receptura", "p13MasterTpv a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID");
            AE("p14MasterOper", "Technologický rozpis operací", "Technologická operace", "p14MasterOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID", "a.p14RowNum");

            AE("p11ClientProduct", "Klientské produkty", "Klientský produkt", "p11ClientProduct a INNER JOIN p12ClientTpv p12 ON a.p12ID=p12.p12ID INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p10MasterProduct p10 ON a.p10ID_Master=p10.p10ID");
            AE("p12ClientTpv", "Klientské receptury", "Klientská receptura", "p12ClientTpv a LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID_Master=p13.p13ID LEFT OUTER JOIN p25MszType p25 ON p13.p25ID=p25.p25ID");
            AE("p15ClientOper", "Technologický rozpis operací", "Technologická operace", "p15ClientOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID", "a.p15RowNum");

            AE("p41Task", "Výrobní zakázky", "Výrobní zakázka", "p41Task a LEFT OUTER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID");
            
            AE("p51Order", "Objednávky", "Objednávka", "p51Order a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID","a.p51ID DESC");
            AE("p52OrderItem", "Položky objednávky", "Položka objednávky", "p52OrderItem a INNER JOIN p51Order p51 ON a.p51ID=p51.p51ID INNER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN p20Unit p20 ON p11.p20ID=p20.p20ID");

            AE("o23Doc", "Dokumenty", "Dokument", "o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");

            
            AE_TINY("j04UserRole", "Aplikační role", "Aplikační role");
            AE_TINY("b02Status", "Workflow stavy", "Workflow stav");
            AE_TINY("p25MszType", "Typy zařízení", "Typ zařízení");
            AE_TINY("p28Company", "Subjekty | Klienti", "Subjekt | Klient");
            AE_TINY("o12Category", "Kategorie", "Kategorie");
            AE_TINY("p31CapTemplate", "Kapacitní fondy", "Kapacitní fond");
            AE_TINY("j90LoginAccessLog", "Historie přihlašování", "Historie přihlašování");


        }

        private static void AE (string strTabName, string strPlural, string strSingular, string strSqlFrom, string strSqlOrderBy=null)
        {            
            _lis.Add(new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFrom = strSqlFrom, SqlOrderBy = strSqlOrderBy });
            //BO.TheEntity c = new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFrom= strSqlFrom, SqlOrderBy= strSqlOrderBy };
        }
        private static void AE_TINY(string strTabName, string strPlural, string strSingular)
        {            
            _lis.Add(new BO.TheEntity() { TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFrom = strTabName + " a" });
        }


    }
}
