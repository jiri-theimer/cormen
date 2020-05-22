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
            AE("j02Person", "Lidé", "Jméno","j02Person a", "j02Person a LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID");
            

            AE("j03User", "Uživatelé", "Uživatel", "j03User a INNER JOIN j04UserRole j03_j04 ON a.j04ID=j03_j04.j04ID", "j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON j02.p28ID=p28.p28ID");
            
            AE("p26Msz", "Stroje | MSZ", "Stroj | MSZ", "p26Msz a", "p26Msz a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN p31CapacityFond p31 ON a.p31ID=p31.p31ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p21License", "Licence", "Licence", "p21License a", "p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p31CapacityFond", "Kapacitní fondy", "Kapacitní fond", "p31CapacityFond a", "p31CapacityFond a INNER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID LEFT OUTER JOIN p28Company p28 ON j02owner.p28ID=p28.p28ID");

            AE("p18OperCode", "Kódy operací", "Kód operace", "p18OperCode a", "p18OperCode a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID");
            AE("p19Material", "Materiály", "Materiál", "p19Material a", "p19Material a INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p20Unit", "Měrné jednotky", "Měrná jednotka", "p20Unit a","p20Unit a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID");
            AE("p10MasterProduct", "Master produkty", "Master produkt", "p10MasterProduct a", "p10MasterProduct a INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");
            AE("p13MasterTpv", "Master receptury", "Master receptura", "p13MasterTpv a", "p13MasterTpv a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID");
            AE("p14MasterOper", "Technologický rozpis operací", "Technologická operace", "p14MasterOper a", "p14MasterOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID", "a.p14RowNum");

            AE("p11ClientProduct", "Klientské produkty", "Klientský produkt", "p11ClientProduct a", "p11ClientProduct a INNER JOIN p12ClientTpv p12 ON a.p12ID=p12.p12ID INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p10MasterProduct p10 ON a.p10ID_Master=p10.p10ID");
            AE("p12ClientTpv", "Klientské receptury", "Klientská receptura", "p12ClientTpv a", "p12ClientTpv a INNER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID_Master=p13.p13ID LEFT OUTER JOIN p25MszType p25 ON a.p25ID=p25.p25ID");
            AE("p15ClientOper", "Technologický rozpis operací", "Technologická operace", "p15ClientOper a", "p15ClientOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID", "a.p15RowNum");

            AE("p41Task", "Výrobní zakázky", "Výrobní zakázka", "p41Task a", "p41Task a LEFT OUTER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID");
            
            AE("p51Order", "Objednávky", "Objednávka", "p51Order a", "p51Order a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID","a.p51ID DESC");
            AE("p52OrderItem", "Položky objednávky", "Položka objednávky", "p52OrderItem a", "p52OrderItem a INNER JOIN p51Order p51 ON a.p51ID=p51.p51ID INNER JOIN p11ClientProduct p11 ON a.p11ID=p11.p11ID LEFT OUTER JOIN p20Unit p20 ON p11.p20ID=p20.p20ID");

            AE("o23Doc", "Dokumenty", "Dokument", "o23Doc a", "o23Doc a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID");

            AE("x40MailQueue", "Outbox", "Poštovní zpráva", "x40MailQueue a", "x40MailQueue a INNER JOIN j03User j03 ON a.j03ID=j03.j03ID","a.x40ID DESC");


            AE_TINY("j04UserRole", "Aplikační role", "Aplikační role");
            AE_TINY("b02Status", "Workflow stavy", "Workflow stav");
            AE_TINY("p25MszType", "Typy zařízení", "Typ zařízení");
            AE_TINY("p28Company", "Subjekty | Klienti", "Klient");
            AE_TINY("o12Category", "Kategorie", "Kategorie");
            AE_TINY("j40MailAccount", "Poštovní účty", "Poštovní účet");


            AE_TINY("j90LoginAccessLog", "Historie přihlašování", "Historie přihlašování");


        }

        private static void AE (string strTabName, string strPlural, string strSingular, string strSqlFromGrid,string strSqlFrom, string strSqlOrderBy=null)
        {
            if (strSqlOrderBy == null) strSqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC";
            _lis.Add(new BO.TheEntity() {TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular,SqlFromGrid=strSqlFromGrid, SqlFrom = strSqlFrom, SqlOrderBy = strSqlOrderBy });
            
        }
        private static void AE_TINY(string strTabName, string strPlural, string strSingular)
        {
            
            _lis.Add(new BO.TheEntity() {TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular, SqlFromGrid=strTabName+" a", SqlFrom = strTabName + " a",SqlOrderBy= "a." + strTabName.Substring(0, 3) + "ID DESC" });
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
                case "p28":
                    lis.Add(getREL("j02Person", "p28_owner", "Vlastník záznamu", getOwnerSql("p28")));                
                    break;
                case "p31":
                    lis.Add(getREL("p28Company", "p31_p28", "Klient", "LEFT OUTER JOIN p28Company p31_p28 ON p31_owner.p28ID=p31_p28.p28ID", "p31_owner"));                    
                    lis.Add(getREL("j02Person", "p31_owner", "Vlastník záznamu", getOwnerSql("p31")));
                    break;
                case "p41":
                    lis.Add(getREL("p28Company", "p41_p28", "Klient", "LEFT OUTER JOIN p28Company p41_p28 ON a.p28ID=p41_p28.p28ID"));
                    lis.Add(getREL("p26Msz", "p41_p26", "Stroj", "LEFT OUTER JOIN p26Msz p41_p26 ON a.p26ID=p41_p26.p26ID"));
                    lis.Add(getREL("b02Status", "p41_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p41_b02 ON a.b02ID = p41_b02.b02ID"));
                    break;
                case "p51":
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON a.p28ID=p51_p28.p28ID"));
                    lis.Add(getREL("p26Msz", "p51_p26", "Stroj", "LEFT OUTER JOIN p26Msz p51_p26 ON a.p26ID=p51_p26.p26ID"));
                    lis.Add(getREL("b02Status", "p51_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p51_b02 ON a.b02ID = p51_b02.b02ID"));
                    lis.Add(getREL("j02Person", "p51_owner", "Vlastník záznamu", getOwnerSql("p51")));
                    break;
                case "p52":
                    lis.Add(getREL("p51Order", "p52_p51", "Objednávka", "INNER JOIN p51Order p52_p51 ON a.p51ID = p52_p51.p51ID"));
                    lis.Add(getREL("p11ClientProduct", "p52_p11", "Produkt", "INNER JOIN p11ClientProduct p52_p11 ON a.p11ID = p52_p11.p11ID"));                                        
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
