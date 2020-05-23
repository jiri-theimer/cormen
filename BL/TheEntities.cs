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
            AE("j02Person", "Lidé", "Jméno","j02Person a","a.j02LastName,a.j02FirstName");
            

            AE("j03User", "Uživatelé", "Uživatel", "j03User a INNER JOIN j04UserRole j03_j04 ON a.j04ID=j03_j04.j04ID", "a.j03Login");
            
            AE("p26Msz", "Stroje | MSZ", "Stroj | MSZ", "p26Msz a", "a.p26Name");
            AE("p21License", "Licence", "Licence", "p21License a", "p28.p28Name,a.p21Name");
            AE("p31CapacityFond", "Kapacitní fondy", "Kapacitní fond", "p31CapacityFond a", "a.p31Name");

            AE("p18OperCode", "Kódy operací", "Kód operace", "p18OperCode a", "a.p18Code");
            AE("p19Material", "Materiály", "Materiál", "p19Material a", "a.p19Name");
            AE("p20Unit", "Měrné jednotky", "Měrná jednotka", "p20Unit a","a.p20Name");
            AE("p10MasterProduct", "Master produkty", "Master produkt", "p10MasterProduct a", "a.p10Name");
            AE("p13MasterTpv", "Master receptury", "Master receptura", "p13MasterTpv a", "a.p13Name");
            AE("p14MasterOper", "Technologický rozpis operací", "Technologická operace", "p14MasterOper a", "a.p14RowNum", "a.p14RowNum");

            AE("p11ClientProduct", "Klientské produkty", "Klientský produkt", "p11ClientProduct a", "a.p11Name");
            AE("p12ClientTpv", "Klientské receptury", "Klientská receptura", "p12ClientTpv a", "a.p12Name");
            AE("p15ClientOper", "Technologický rozpis operací", "Technologická operace", "p15ClientOper a", "a.p15RowNum", "a.p15RowNum");

            AE("p41Task", "Zakázky", "Zakázka", "p41Task a", "a.p14Code DESC");
            
            AE("p51Order", "Objednávky", "Objednávka", "p51Order a", "a.p51Name,a.p51Code");
            AE("p52OrderItem", "Položky objednávky", "Položka objednávky", "p52OrderItem a","a.p52Code");

            AE("o23Doc", "Dokumenty", "Dokument", "o23Doc a", null);

            AE("x40MailQueue", "Outbox", "Poštovní zpráva", "x40MailQueue a", null,"a.x40ID DESC");


            AE_TINY("j04UserRole", "Aplikační role", "Aplikační role");
            AE_TINY("b02Status", "Workflow stavy", "Workflow stav");
            AE_TINY("p25MszType", "Typy zařízení", "Typ zařízení");
            AE_TINY("p27MszUnit", "Střediska", "Středisko");
            AE_TINY("p28Company", "Subjekty | Klienti", "Klient");
            AE_TINY("o12Category", "Kategorie", "Kategorie");
            AE_TINY("j40MailAccount", "Poštovní účty", "Poštovní účet");


            AE_TINY("j90LoginAccessLog", "LOGIN Log", "LOGIN Log");
            AE_TINY("j92PingLog", "PING Log", "PING Log");


        }

        private static void AE (string strTabName, string strPlural, string strSingular, string strSqlFromGrid,string strSqlOrderByCombo, string strSqlOrderBy=null)
        {
            if (strSqlOrderBy == null) strSqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC";
            _lis.Add(new BO.TheEntity() {TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular,SqlFromGrid=strSqlFromGrid,SqlOrderByCombo= strSqlOrderByCombo, SqlOrderBy = strSqlOrderBy });
            
        }
        private static void AE_TINY(string strTabName, string strPlural, string strSingular)
        {
            
            _lis.Add(new BO.TheEntity() {TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid=strTabName+" a", SqlOrderByCombo = "a." + strTabName.Substring(0, 3) + "Name", SqlOrderBy= "a." + strTabName.Substring(0, 3) + "ID DESC" });
        }
        private static BO.EntityRelation getREL(string strTabName,string strRelName, string strSingular, string strSqlFrom,string strDependOnRel=null)
        {
            return new BO.EntityRelation() { TableName = strTabName, RelName = strRelName, AliasSingular = strSingular, SqlFrom = strSqlFrom, RelNameDependOn= strDependOnRel };
            
            

        }

        public static List<BO.EntityRelation>getApplicableRelations(string strPrimaryPrefix)
        {
            TestPallete();
            var lis = new List<BO.EntityRelation>();
            BO.TheEntity ce = ByPrefix(strPrimaryPrefix);

            //lis.Add(new BO.EntityRelation() { IsPrimaryTable = true, TableName = ce.TableName, AliasSingular = ce.AliasSingular, RelName = "a", SqlFrom = ce.SqlFromGrid });


            switch (strPrimaryPrefix)
            {
                case "j02":                                    
                    lis.Add(getREL("j03User", "j02_j03", "Uživatelský účet", "LEFT OUTER JOIN j03User j02_j03 ON a.j02ID=j02_j03.j02ID LEFT OUTER JOIN j04UserRole j03_j04 ON j02_j03.j04ID=j03_j04.j04ID"));
                    lis.Add(getREL("p28Company", "j02_p28", "Klient", "LEFT OUTER JOIN p28Company j02_p28 ON a.p28ID=j02_p28.p28ID"));
                    lis.Add(getREL("j02Person", "j02_owner", "Vlastník záznamu", getOwnerSql("j02")));
                    break;
                case "j90":
                    lis.Add(getREL("j03User", "j90_j03", "Uživatelský účet", "INNER JOIN j03User j90_j03 ON a.j03ID=j90_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j90_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "j92":
                    lis.Add(getREL("j03User", "j92_j03", "Uživatelský účet", "INNER JOIN j03User j92_j03 ON a.j03ID=j92_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j92_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "p10":
                    lis.Add(getREL("p20Unit", "p10_p20", "Měrná jednotka", "INNER JOIN p20Unit p10_p20 ON a.p20ID=p10_p20.p20ID"));
                    lis.Add(getREL("p13MasterTpv", "p10_p13", "Master receptura", "LEFT OUTER JOIN p13MasterTpv p10_p13 ON a.p13ID=p10_p13.p13ID"));
                    lis.Add(getREL("b02Status", "p10_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p10_b02 ON a.b02ID = p10_b02.b02ID"));
                    lis.Add(getREL("o12Category", "p10_o12", "Kategorie", "LEFT OUTER JOIN o12Category p10_o12 ON a.o12ID=p10_o12.o12ID"));
                    break;
                case "p11":
                    lis.Add(getREL("p20Unit", "p11_p20", "Měrná jednotka", "INNER JOIN p20Unit p11_p20 ON a.p20ID=p11_p20.p20ID"));
                    lis.Add(getREL("p21License", "p11_p21", "Licence", "INNER JOIN p21License p11_p21 ON a.p21ID=p11_p21.p21ID"));
                    lis.Add(getREL("p28Company", "p21_p28", "Klient", "LEFT OUTER JOIN p28Company p21_p28 ON p11_p21.p28ID=p21_p28.p28ID", "p11_p21"));
                    lis.Add(getREL("p12ClientTpv", "p11_p12", "Receptura", "INNER JOIN p12ClientTpv p11_p12 ON a.p12ID=p11_p12.p12ID"));
                    lis.Add(getREL("p10MasterProduct", "p11_p10", "Vzorový Master produkt", "LEFT OUTER JOIN p10MasterProduct p11_p10 ON a.p10ID_Master = p11_p10.p10ID"));                                        
                    lis.Add(getREL("b02Status", "p11_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p11_b02 ON a.b02ID = p11_b02.b02ID"));
                    break;
                case "p12":
                    lis.Add(getREL("p25MszType", "p12_p25", "Typ zařízení", "INNER JOIN p25MszType p12_p25 ON a.p25ID=p12_p25.p25ID"));
                    lis.Add(getREL("p21License", "p12_p21", "Licence", "INNER JOIN p21License p12_p21 ON a.p21ID=p12_p21.p21ID"));
                    lis.Add(getREL("p28Company", "p21_p28", "Klient", "LEFT OUTER JOIN p28Company p21_p28 ON p12_p21.p28ID=p21_p28.p28ID", "p12_p21"));
                    lis.Add(getREL("p13MasterTpv", "p12_p13", "Master receptura", "LEFT OUTER JOIN p13MasterTpv p12_p13 ON a.p13ID_Master=p12_p13.p13ID"));
                    break;
                case "p13":
                    lis.Add(getREL("p25MszType", "p13_p25", "Typ zařízení", "INNER JOIN p25MszType p13_p25 ON a.p25ID=p13_p25.p25ID"));                    
                    break;
                case "p14":
                    lis.Add(getREL("p19Material", "p14_p19", "Materiál", "LEFT OUTER JOIN p19Material p14_p19 ON a.p19ID=p14_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p14_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p14_p18 ON a.p18ID=p14_p18.p18ID"));
                    break;
                case "p15":
                    lis.Add(getREL("p19Material", "p15_p19", "Materiál", "LEFT OUTER JOIN p19Material p15_p19 ON a.p19ID=p15_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p15_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p15_p18 ON a.p18ID=p15_p18.p18ID"));
                    break;
                case "p18":
                    lis.Add(getREL("p25MszType", "p18_p25", "Typ zařízení", "INNER JOIN p25MszType p18_p25 ON a.p25ID=p18_p25.p25ID"));                    
                    lis.Add(getREL("p19Material", "p18_p19", "Materiál", "LEFT OUTER JOIN p19Material p18_p19 ON a.p19ID=p18_p19.p19ID"));                    
                    break;
                case "p19":
                    lis.Add(getREL("p20Unit", "p19_p20", "Měrná jednotka", "INNER JOIN p20Unit p19_p20 ON a.p20ID=p19_p20.p20ID"));
                    lis.Add(getREL("p28Company", "p19_p28", "Klient", "LEFT OUTER JOIN p28Company p19_p28 ON a.p28ID=p19_p28.p28ID"));
                    lis.Add(getREL("o12Category", "p19_o12", "Kategorie materiálu", "LEFT OUTER JOIN o12Category p19_o12 ON a.o12ID=p19_o12.o12ID"));
                    lis.Add(getREL("j02Person", "p19_owner", "Vlastník záznamu", getOwnerSql("p19")));
                    break;
                case "p20":                    
                    lis.Add(getREL("p28Company", "p20_p28", "Klient", "LEFT OUTER JOIN p28Company p20_p28 ON a.p28ID=p20_p28.p28ID"));                    
                    lis.Add(getREL("j02Person", "p20_owner", "Vlastník záznamu", getOwnerSql("p20")));
                    break;
                case "p21":
                    lis.Add(getREL("p28Company", "p21_p28", "Klient licence", "LEFT OUTER JOIN p28Company p21_p28 ON a.p28ID=p21_p28.p28ID"));
                    lis.Add(getREL("b02Status", "p21_b02", "Workflow stav licence", "LEFT OUTER JOIN b02Status p21_b02 ON a.b02ID = p21_b02.b02ID"));
                    lis.Add(getREL("o12Category", "p21_o12", "Kategorie licence", "LEFT OUTER JOIN o12Category p21_o12 ON a.o12ID=p21_o12.o12ID"));
                    break;
                case "p26":
                    lis.Add(getREL("p28Company", "p26_p28", "Klient stroje", "LEFT OUTER JOIN p28Company p26_p28 ON a.p28ID=p26_p28.p28ID"));
                    lis.Add(getREL("p25MszType", "p26_p25", "Typ zařízení", "INNER JOIN p25MszType p26_p25 ON a.p25ID=p26_p25.p25ID"));                    
                    lis.Add(getREL("p31CapacityFond", "p26_p31", "Kapacitní fond", "LEFT OUTER JOIN p31CapacityFond p26_p31 ON a.p31ID=p26_p31.p31ID"));
                    lis.Add(getREL("b02Status", "p26_b02", "Workflow stav stroje", "LEFT OUTER JOIN b02Status p26_b02 ON a.b02ID = p26_b02.b02ID"));
                    lis.Add(getREL("o12Category", "p26_o12", "Kategorie stroje", "LEFT OUTER JOIN o12Category p26_o12 ON a.o12ID=p26_o12.o12ID"));
                    break;
                case "p27":
                    lis.Add(getREL("p26Msz", "p27_p26", "Stroj", "INNER JOIN p26Msz p27_p26 ON a.p26ID=p27_p26.p26ID"));
                    break;
                case "p28":
                    lis.Add(getREL("j02Person", "p28_owner", "Vlastník záznamu", getOwnerSql("p28")));                
                    break;
                case "p31":
                    lis.Add(getREL("p28Company", "p31_p28", "Klient", "LEFT OUTER JOIN p28Company p31_p28 ON p31_owner.p28ID=p31_p28.p28ID", "p31_owner"));                    
                    lis.Add(getREL("j02Person", "p31_owner", "Vlastník záznamu", getOwnerSql("p31")));
                    break;
                case "p41":
                    lis.Add(getREL("p27MszUnit", "p41_p27", "Středisko", "LEFT OUTER JOIN p27MszUnit p41_p27 ON a.p27ID=p41_p27.p27ID"));
                    lis.Add(getREL("b02Status", "p41_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p41_b02 ON a.b02ID = p41_b02.b02ID"));

                    lis.Add(getREL("p52OrderItem", "p41_p52", "Položka objednávky", "INNER JOIN p52OrderItem p41_p52 ON a.p52ID=p41_p52.p52ID"));
                    lis.Add(getREL("p11ClientProduct", "p52_p11", "Produkt", "INNER JOIN p11ClientProduct p52_p11 ON p41_p52.p11ID = p52_p11.p11ID","p41_p52"));
                    lis.Add(getREL("p51Order", "p52_p51", "Objednávka", "INNER JOIN p51Order p52_p51 ON p41_p52.p51ID=p52_p51.p51ID","p41_p52"));
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON p52_p51.p28ID=p51_p28.p28ID", "p52_p51"));
                    break;
                case "p51":
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON a.p28ID=p51_p28.p28ID"));
                    lis.Add(getREL("b02Status", "p51_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p51_b02 ON a.b02ID = p51_b02.b02ID"));
                    lis.Add(getREL("j02Person", "p51_owner", "Vlastník záznamu", getOwnerSql("p51")));
                    break;
                case "p52":
                    lis.Add(getREL("p51Order", "p52_p51", "Objednávka", "INNER JOIN p51Order p52_p51 ON a.p51ID = p52_p51.p51ID"));
                    lis.Add(getREL("p11ClientProduct", "p52_p11", "Produkt", "INNER JOIN p11ClientProduct p52_p11 ON a.p11ID = p52_p11.p11ID"));
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON p52_p51.p28ID=p51_p28.p28ID","p52_p51"));
                    lis.Add(getREL("p20Unit", "p11_p20", "Měrná jednotka", "INNER JOIN p20Unit p11_p20 ON p52_p11.p20ID=p11_p20.p20ID", "p52_p11"));
                    break;
                case "o23":
                    lis.Add(getREL("b02Status", "o23_b02", "Workflow stav", "LEFT OUTER JOIN b02Status o23_b02 ON a.b02ID = o23_b02.b02ID"));
                    lis.Add(getREL("o12Category", "o23_o12", "Kategorie dokumentu", "LEFT OUTER JOIN o12Category o23_o12 ON a.o12ID=o23_o12.o12ID"));
                    lis.Add(getREL("j02Person", "o23_owner", "Vlastník záznamu", getOwnerSql("o23")));
                    break;
                case "j40":
                    lis.Add(getREL("j02Person", "j40_owner", "Vlastník záznamu", getOwnerSql("j40")));
                    break;
                default:
                    break;
            }

            return lis;
        }

        private static string getOwnerSql(string prefix)
        {
            return string.Format("LEFT OUTER JOIN j02Person {0}_owner ON a.j02ID_Owner = {0}_owner.j02ID LEFT OUTER JOIN p28Company {0}_owner_company ON {0}_owner.p28ID = {0}_owner_company.p28ID",prefix);
        }

    }
}
