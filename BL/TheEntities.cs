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
        public static BO.TheEntity ByIntPrefix(int intPrefix)
        {
            if (intPrefix==0) return null;
            TestPallete();
            string s = intPrefix.ToString().Substring(0, 1);
            string t = intPrefix.ToString().Substring(1, 2);
            switch (s)
            {
                case "1":
                    s = "j";
                    break;
                case "2":
                    s = "o";
                    break;
                case "3":
                    s = "p";
                    break;
            }
            return ByPrefix(s + t);

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
            
            AE("p26Msz", "Skupiny zařízení", "Skupina zařízení", "p26Msz a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p26Name");
            AE("p21License", "Licence", "Licence", "p21License a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p21Name");
            AE("p31CapacityFond", "Časové fondy", "Časový fond", "p31CapacityFond a", "a.p31Name");

            AE("p18OperCode", "Kódy operací", "Kód operace", "p18OperCode a", "a.p18Code", "a.p18Code");
            AE("p19Material", "Suroviny", "Surovina", "p19Material a", "a.p19Name");
            AE("p20Unit", "Měrné jednotky", "Měrná jednotka", "p20Unit a","a.p20Name");
            AE("p10MasterProduct", "Master produkty", "Master produkt", "p10MasterProduct a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p10Name");
            AE("p13MasterTpv", "Master receptury", "Master receptura", "p13MasterTpv a", "a.p13Name");
            AE("p14MasterOper", "Technologický rozpis operací", "Technologická operace", "p14MasterOper a", "a.p14RowNum", "a.p14RowNum");

            AE("p11ClientProduct", "Klientské produkty", "Klientský produkt", "p11ClientProduct a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p11Name",null,true);
            AE("p12ClientTpv", "Klientské receptury", "Klientská receptura", "p12ClientTpv a", "a.p12Name");
            AE("p15ClientOper", "Technologický rozpis operací", "Technologická operace", "p15ClientOper a", "a.p15RowNum", "a.p15RowNum");

            AE("p41Task", "Zakázky", "Zakázka", "p41Task a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p41ID DESC",null,true);
            
            AE("p44TaskOperPlan", "Plán výrobních operací", "Plán výrobních operací", "p44TaskOperPlan a", "a.p44RowNum", "a.p44RowNum",true);
            
            AE("p45TaskOperReal", "Skutečná výroba", "Skutečná výroba", "p45TaskOperReal a", "a.p45RowNum", "a.p45RowNum");


            AE("p51Order", "Objednávky", "Objednávka", "p51Order a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", "a.p51Name,a.p51Code");
            AE("p52OrderItem", "Položky objednávky", "Položka objednávky", "p52OrderItem a","a.p52Code");

            AE("o23Doc", "Dokumenty", "Dokument", "o23Doc a LEFT OUTER JOIN b02Status bc ON a.b02ID=bc.b02ID", null);

            AE("x40MailQueue", "Outbox", "Poštovní zpráva", "x40MailQueue a", null,"a.x40ID DESC");
            AE("b02Status", "Workflow stavy", "Workflow stav", "b02Status a", "a.b02Ordinary,a.b02Name", "a.b02Ordinary,a.b02Name");
            AE("b03StatusGroup", "Skupiny stavů", "Skupina stavu", "b03StatusGroup a", "a.b03Name", "a.b03Name");
            


            AE_TINY("j04UserRole", "Aplikační role", "Aplikační role");            
            AE_TINY("p25MszType", "Typy zařízení", "Typ zařízení");
            AE_TINY("p27MszUnit", "Zařízení", "Zařízení");
            AE_TINY("p28Company", "Subjekty | Klienti", "Klient");
            
            AE_TINY("o51Tag", "Položky kategorií", "Položka kategorie");            
            AE_TINY("o53TagGroup", "Kategorie", "Kategorie");
            AE_TINY("j40MailAccount", "Poštovní účty", "Poštovní účet");


            AE_TINY("j90LoginAccessLog", "LOGIN Log", "LOGIN Log");
            AE_TINY("j92PingLog", "PING Log", "PING Log");

            AE_TINY("o54TagBindingInline", "Kategorizace", "Kategorizace");

            //VIEW:
            AE("z01_produkty_plan_vyroby", "Plánování výroby produktů", "Plánování výroby produktů", "dbo.z01_produkty_plan_vyroby(@gd1,@gd2) a", "a.p10ID", "a.p10ID",true);
            AE("z02_suroviny_plan_vyroby", "Plánování výroby surovin", "Plánování výroby surovin", "dbo.z02_suroviny_plan_vyroby(@gd1,@gd2) a", "a.p19ID", "a.p19ID", true);
            AE("sum_p11_p41", "Plán výroby produktů", "Plán výroby produktů", "sum_p11_p41 a", "a.p11ID", "a.p11ID", true);
            AE("sum_p11_p51", "Objednávky produktů", "Objednávky produktů", "sum_p11_p51 a", "a.p11ID", "a.p11ID", true);
        }

        private static void AE (string strTabName, string strPlural, string strSingular, string strSqlFromGrid,string strSqlOrderByCombo, string strSqlOrderBy=null,bool bolGlobalPeriodQuery=false)
        {
            if (strSqlOrderBy == null) strSqlOrderBy = "a." + strTabName.Substring(0, 3) + "ID DESC";
            _lis.Add(new BO.TheEntity() {TableName = strTabName, AliasPlural = strPlural, AliasSingular = strSingular,SqlFromGrid=strSqlFromGrid,SqlOrderByCombo= strSqlOrderByCombo, SqlOrderBy = strSqlOrderBy,IsGlobalPeriodQuery= bolGlobalPeriodQuery });
            
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
                    lis.Add(getREL("o54TagBindingInline", "j02_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='j02') j02_o54 ON a.j02ID=j02_o54.o54RecordPid"));
                    break;
                case "j90":
                    lis.Add(getREL("j03User", "j90_j03", "Uživatelský účet", "INNER JOIN j03User j90_j03 ON a.j03ID=j90_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j90_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "j92":
                    lis.Add(getREL("j03User", "j92_j03", "Uživatelský účet", "INNER JOIN j03User j92_j03 ON a.j03ID=j92_j03.j03ID INNER JOIN j04UserRole j03_j04 ON j92_j03.j04ID=j03_j04.j04ID"));
                    break;
                case "p10":
                    lis.Add(getREL("p20Unit", "p10_p20", "MJ", "INNER JOIN p20Unit p10_p20 ON a.p20ID=p10_p20.p20ID"));
                    lis.Add(getREL("p20Unit", "p10_p20pro", "VJ", "LEFT OUTER JOIN p20Unit p10_p20pro ON a.p20ID_Pro=p10_p20pro.p20ID"));
                    lis.Add(getREL("p13MasterTpv", "p10_p13", "Master receptura", "LEFT OUTER JOIN p13MasterTpv p10_p13 ON a.p13ID=p10_p13.p13ID"));
                    lis.Add(getREL("b02Status", "p10_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p10_b02 ON a.b02ID = p10_b02.b02ID"));
                    
                    lis.Add(getREL("o54TagBindingInline", "p10_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p10') p10_o54 ON a.p10ID=p10_o54.o54RecordPid"));
                    break;
                case "p11":
                    lis.Add(getREL("p20Unit", "p11_p20", "Měrná jednotka", "INNER JOIN p20Unit p11_p20 ON a.p20ID=p11_p20.p20ID"));
                    lis.Add(getREL("p20Unit", "p11_p20pro", "Výrobní jednotka", "LEFT OUTER JOIN p20Unit p11_p20pro ON a.p20ID_Pro=p11_p20pro.p20ID"));
                    lis.Add(getREL("p21License", "p11_p21", "Licence", "INNER JOIN p21License p11_p21 ON a.p21ID=p11_p21.p21ID"));
                    lis.Add(getREL("p28Company", "p21_p28", "Klient licence", "LEFT OUTER JOIN p28Company p21_p28 ON p11_p21.p28ID=p21_p28.p28ID", "p11_p21"));
                    lis.Add(getREL("p12ClientTpv", "p11_p12", "Receptura", "INNER JOIN p12ClientTpv p11_p12 ON a.p12ID=p11_p12.p12ID"));
                    lis.Add(getREL("p25MszType", "p12_p25", "Typ zařízení", "LEFT OUTER JOIN (select xa.p13ID,xb.* from p13MasterTpv xa INNER JOIN p25MszType xb ON xa.p25ID=xb.p25ID) p12_p25 ON p11_p12.p13ID_Master=p12_p25.p13ID", "p11_p12"));
                    lis.Add(getREL("p10MasterProduct", "p11_p10", "Master produkt", "LEFT OUTER JOIN p10MasterProduct p11_p10 ON a.p10ID_Master = p11_p10.p10ID"));
                    lis.Add(getREL("p19Material", "p11_p19", "Surovina", "LEFT OUTER JOIN p19Material p11_p19 ON a.p11Code=p11_p19.p19Code"));

                    lis.Add(getREL("b02Status", "p11_b02", "Workflow stav produktu", "LEFT OUTER JOIN b02Status p11_b02 ON a.b02ID = p11_b02.b02ID"));
                    lis.Add(getREL("o54TagBindingInline", "p11_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p11') p11_o54 ON a.p11ID=p11_o54.o54RecordPid"));
                    
                    string ss = "LEFT OUTER JOIN (";
                    ss += "select xb.p11ID,count(xa.p41ID) as PocetVZ,sum(xa.p41PlanUnitsCount) as PlanMnozstvi,min(xa.p41PlanStart) as MinStart,max(xa.p41PlanStart) as MaxStart,min(xa.p41Code) as p41CodeFirst";
                    ss += ",sum(case when xa.b02ID IN (23,37,24,25,27) then xa.p41PlanUnitsCount end) as PlanMnozstvi1";
                    ss += " FROM p41Task xa INNER JOIN p52OrderItem xb ON xa.p52ID=xb.p52ID";
                    ss += " WHERE 1=1 AND xa.p41PlanEnd BETWEEN @gd1 AND @gd2 GROUP BY xb.p11ID";
                    ss += ") sum_p11_p41 ON a.p11ID=sum_p11_p41.p11ID";

                    lis.Add(getREL("sum_p11_p41", "sum_p11_p41", "Σ plán výroby", ss));
                    
                    ss = "LEFT OUTER JOIN (";
                    ss += " SELECT xa.p11ID,count(DISTINCT xb.p51ID) as PocetP51,count(xa.p52ID) as PocetP52,sum(xa.p52UnitsCount) as ObjednaneMnozstvi";
                    ss += ",sum(case when xb.b02ID NOT IN (14,20) then xa.p52UnitsCount end) as ObjednaneMnozstvi1";
                    ss += " FROM p52OrderItem xa INNER JOIN p51Order xb ON xa.p51ID=xb.p51ID";
                    ss += " WHERE 1=1 AND isnull(xb.p51DateDeliveryConfirmed,xb.p51DateDelivery) BETWEEN @gd1 AND @gd2 GROUP BY xa.p11ID";
                    ss += ") sum_p11_p51 ON a.p11ID=sum_p11_p51.p11ID";

                    lis.Add(getREL("sum_p11_p51", "sum_p11_p51", "Σ objednáno", ss));
                    break;
                    
                case "p12":
                    lis.Add(getREL("p25MszType", "p12_p25", "Typ zařízení", "INNER JOIN p25MszType p12_p25 ON a.p25ID=p12_p25.p25ID"));
                    lis.Add(getREL("p21License", "p12_p21", "Licence", "INNER JOIN p21License p12_p21 ON a.p21ID=p12_p21.p21ID"));
                    lis.Add(getREL("p28Company", "p21_p28", "Klient", "LEFT OUTER JOIN p28Company p21_p28 ON p12_p21.p28ID=p21_p28.p28ID", "p12_p21"));
                    lis.Add(getREL("p13MasterTpv", "p12_p13", "Master receptura", "LEFT OUTER JOIN p13MasterTpv p12_p13 ON a.p13ID_Master=p12_p13.p13ID"));
                    lis.Add(getREL("o54TagBindingInline", "p12_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p12') p12_o54 ON a.p12ID=p12_o54.o54RecordPid"));
                    break;
                case "p13":
                    lis.Add(getREL("p25MszType", "p13_p25", "Typ zařízení", "INNER JOIN p25MszType p13_p25 ON a.p25ID=p13_p25.p25ID"));
                    lis.Add(getREL("o54TagBindingInline", "p13_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p13') p13_o54 ON a.p13ID=p13_o54.o54RecordPid"));
                    break;
                case "p14":
                    lis.Add(getREL("p19Material", "p14_p19", "Suroviny", "LEFT OUTER JOIN p19Material p14_p19 ON a.p19ID=p14_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p14_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p14_p18 ON a.p18ID=p14_p18.p18ID"));
                    break;
                case "p15":
                    lis.Add(getREL("p19Material", "p15_p19", "Suroviny", "LEFT OUTER JOIN p19Material p15_p19 ON a.p19ID=p15_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p15_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p15_p18 ON a.p18ID=p15_p18.p18ID"));
                    break;
                case "p18":
                    lis.Add(getREL("p25MszType", "p18_p25", "Typ zařízení", "INNER JOIN p25MszType p18_p25 ON a.p25ID=p18_p25.p25ID"));                    
                    lis.Add(getREL("p19Material", "p18_p19", "Suroviny", "LEFT OUTER JOIN p19Material p18_p19 ON a.p19ID=p18_p19.p19ID"));
                    lis.Add(getREL("o54TagBindingInline", "p18_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p18') p18_o54 ON a.p18ID=p18_o54.o54RecordPid"));
                    break;
                case "p19":
                    lis.Add(getREL("p20Unit", "p19_p20", "Měrná jednotka", "INNER JOIN p20Unit p19_p20 ON a.p20ID=p19_p20.p20ID"));
                    lis.Add(getREL("p28Company", "p19_p28", "Klient", "LEFT OUTER JOIN p28Company p19_p28 ON a.p28ID=p19_p28.p28ID"));
                    lis.Add(getREL("p10MasterProduct", "p19_p10", "Master produkt (Polotovar)", "LEFT OUTER JOIN p10MasterProduct p19_p10 ON a.p10ID_Master = p19_p10.p10ID"));
                    lis.Add(getREL("j02Person", "p19_owner", "Vlastník záznamu", getOwnerSql("p19")));
                    lis.Add(getREL("o54TagBindingInline", "p19_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p19') p19_o54 ON a.p19ID=p19_o54.o54RecordPid"));
                    break;
                case "p20":                    
                    lis.Add(getREL("p28Company", "p20_p28", "Klient", "LEFT OUTER JOIN p28Company p20_p28 ON a.p28ID=p20_p28.p28ID"));                    
                    lis.Add(getREL("j02Person", "p20_owner", "Vlastník záznamu", getOwnerSql("p20")));
                    
                    break;
                case "p21":
                    lis.Add(getREL("p28Company", "p21_p28", "Klient licence", "LEFT OUTER JOIN p28Company p21_p28 ON a.p28ID=p21_p28.p28ID"));
                    lis.Add(getREL("b02Status", "p21_b02", "Workflow stav licence", "LEFT OUTER JOIN b02Status p21_b02 ON a.b02ID = p21_b02.b02ID"));
                    
                    lis.Add(getREL("o54TagBindingInline", "p21_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p21') p21_o54 ON a.p21ID=p21_o54.o54RecordPid"));
                    break;
                case "p26":
                    lis.Add(getREL("p25MszType", "p26_p25", "Typ zařízení", "INNER JOIN p25MszType p26_p25 ON a.p25ID=p26_p25.p25ID"));
                    lis.Add(getREL("b02Status", "p26_b02", "Workflow stav skupiny", "LEFT OUTER JOIN b02Status p26_b02 ON a.b02ID = p26_b02.b02ID"));
                    lis.Add(getREL("p28Company", "p26_p28", "Klient stroje", "LEFT OUTER JOIN p28Company p26_p28 ON a.p28ID=p26_p28.p28ID"));

                    lis.Add(getREL("o54TagBindingInline", "p26_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p26') p26_o54 ON a.p26ID=p26_o54.o54RecordPid"));
                    break;
                case "p27":
                    lis.Add(getREL("p25MszType", "p27_p25", "Kmenový typ zařízení", "INNER JOIN p25MszType p27_p25 ON a.p25ID_HW=p27_p25.p25ID"));
                    lis.Add(getREL("p31CapacityFond", "p27_p31", "Časový fond", "LEFT OUTER JOIN p31CapacityFond p27_p31 ON a.p31ID=p27_p31.p31ID"));

                    lis.Add(getREL("o54TagBindingInline", "p27_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p27') p27_o54 ON a.p27ID=p27_o54.o54RecordPid"));
                    break;
                case "p28":
                    lis.Add(getREL("j02Person", "p28_owner", "Vlastník záznamu", getOwnerSql("p28")));
                    lis.Add(getREL("o54TagBindingInline", "p28_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p28') p28_o54 ON a.p28ID=p28_o54.o54RecordPid"));
                    break;
                case "p31":
                    lis.Add(getREL("p28Company", "p31_p28", "Klient", "LEFT OUTER JOIN p28Company p31_p28 ON p31_owner.p28ID=p31_p28.p28ID", "p31_owner"));                    
                    lis.Add(getREL("j02Person", "p31_owner", "Vlastník záznamu", getOwnerSql("p31")));
                    break;
                case "p41":
                    lis.Add(getREL("p27MszUnit", "p41_p27", "Zařízení", "LEFT OUTER JOIN p27MszUnit p41_p27 ON a.p27ID=p41_p27.p27ID"));
                    lis.Add(getREL("b02Status", "p41_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p41_b02 ON a.b02ID = p41_b02.b02ID"));

                    lis.Add(getREL("p52OrderItem", "p41_p52", "Položka objednávky", "INNER JOIN p52OrderItem p41_p52 ON a.p52ID=p41_p52.p52ID"));
                    lis.Add(getREL("p11ClientProduct", "p52_p11", "Produkt", "INNER JOIN p11ClientProduct p52_p11 ON p41_p52.p11ID = p52_p11.p11ID","p41_p52"));
                    lis.Add(getREL("p20Unit", "p11_p20", "Měrná jednotka", "LEFT OUTER JOIN p20Unit p11_p20 ON p52_p11.p20ID=p11_p20.p20ID", "p52_p11"));
                    lis.Add(getREL("p20Unit", "p11_p20pro", "Výrobní jednotka", "LEFT OUTER JOIN p20Unit p11_p20pro ON p52_p11.p20ID_Pro=p11_p20pro.p20ID", "p52_p11"));
                    

                    lis.Add(getREL("p51Order", "p52_p51", "Objednávka", "INNER JOIN p51Order p52_p51 ON p41_p52.p51ID=p52_p51.p51ID","p41_p52"));
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON p52_p51.p28ID=p51_p28.p28ID", "p52_p51"));
                    lis.Add(getREL("o54TagBindingInline", "p41_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p41') p41_o54 ON a.p41ID=p41_o54.o54RecordPid"));

                    lis.Add(getREL("p41Task", "p41_successor", "Následovník", "LEFT OUTER JOIN p41Task p41_successor ON a.p41SuccessorID = p41_successor.p41ID"));
                    lis.Add(getREL("p41Task", "p41_predecessor", "Předchůdce", "LEFT OUTER JOIN (select * from p41Task where p41SuccessorID IS NOT NULL) p41_predecessor ON a.p41ID = p41_predecessor.p41SuccessorID"));

                    break;
                case "p44":
                    lis.Add(getREL("p19Material", "p44_p19", "Suroviny", "LEFT OUTER JOIN p19Material p44_p19 ON a.p19ID=p44_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p44_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p44_p18 ON a.p18ID=p44_p18.p18ID"));
                    lis.Add(getREL("p41Task", "p44_p41", "Zakázka", "LEFT OUTER JOIN p41Task p44_p41 ON a.p41ID=p44_p41.p41ID"));
                    lis.Add(getREL("p27MszUnit", "p41_p27", "Zařízení", "LEFT OUTER JOIN p27MszUnit p41_p27 ON p44_p41.p27ID=p41_p27.p27ID", "p44_p41"));
                    break;
                case "p45":
                    lis.Add(getREL("p19Material", "p45_p19", "Suroviny", "LEFT OUTER JOIN p19Material p45_p19 ON a.p19ID=p45_p19.p19ID"));
                    lis.Add(getREL("p18OperCode", "p45_p18", "Kód operace", "LEFT OUTER JOIN p18OperCode p45_p18 ON a.p18ID=p45_p18.p18ID"));
                    break;
                case "p51":
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON a.p28ID=p51_p28.p28ID"));
                    lis.Add(getREL("b02Status", "p51_b02", "Workflow stav", "LEFT OUTER JOIN b02Status p51_b02 ON a.b02ID = p51_b02.b02ID"));
                    lis.Add(getREL("j02Person", "p51_owner", "Vlastník záznamu", getOwnerSql("p51")));
                    lis.Add(getREL("o54TagBindingInline", "p51_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p51') p51_o54 ON a.p51ID=p51_o54.o54RecordPid"));
                    break;
                case "p52":
                    lis.Add(getREL("p51Order", "p52_p51", "Objednávka", "INNER JOIN p51Order p52_p51 ON a.p51ID = p52_p51.p51ID"));
                    lis.Add(getREL("p11ClientProduct", "p52_p11", "Produkt", "INNER JOIN p11ClientProduct p52_p11 ON a.p11ID = p52_p11.p11ID"));
                    lis.Add(getREL("p28Company", "p51_p28", "Klient", "LEFT OUTER JOIN p28Company p51_p28 ON p52_p51.p28ID=p51_p28.p28ID","p52_p51"));
                    lis.Add(getREL("p20Unit", "p11_p20", "Měrná jednotka", "INNER JOIN p20Unit p11_p20 ON p52_p11.p20ID=p11_p20.p20ID", "p52_p11"));
                    lis.Add(getREL("p20Unit", "p11_p20pro", "Výrobní jednotka", "LEFT OUTER JOIN p20Unit p11_p20pro ON p52_p11.p20ID_Pro=p11_p20pro.p20ID", "p52_p11"));
                    lis.Add(getREL("p13MasterTpv", "p10_p13", "Master receptura", "LEFT OUTER JOIN p10MasterProduct p11_p10 ON p52_p11.p10ID_Master=p11_p10.p10ID LEFT OUTER JOIN p13MasterTpv p10_p13 ON p11_p10.p13ID=p10_p13.p13ID", "p52_p11"));
                    lis.Add(getREL("p25MszType", "p13_p25", "Typ zařízení", "LEFT OUTER JOIN p25MszType p13_p25 ON p10_p13.p25ID=p13_p25.p25ID"));

                    break;
                case "o23":
                    lis.Add(getREL("b02Status", "o23_b02", "Workflow stav", "LEFT OUTER JOIN b02Status o23_b02 ON a.b02ID = o23_b02.b02ID"));
                    
                    lis.Add(getREL("j02Person", "o23_owner", "Vlastník záznamu", getOwnerSql("o23")));
                    lis.Add(getREL("o54TagBindingInline", "o23_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='o23') o23_o54 ON a.o23ID=o23_o54.o54RecordPid"));
                    break;
                case "j40":
                    lis.Add(getREL("j02Person", "j40_owner", "Vlastník záznamu", getOwnerSql("j40")));
                    break;
                case "o51":                    
                    lis.Add(getREL("o53TagGroup", "o51_o53", "Kategorie", "LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID"));
                    //lis.Add(getREL("j02Person", "o51_owner", "Vlastník záznamu", getOwnerSql("o51")));
                    break;
                case "z01":
                    lis.Add(getREL("p10MasterProduct", "z01_p10", "Master produkt", "INNER JOIN p10MasterProduct z01_p10 ON a.p10ID = z01_p10.p10ID"));
                    lis.Add(getREL("p13MasterTpv", "z01_p13", "Master receptura", "LEFT OUTER JOIN p13MasterTpv z01_p13 ON a.p13ID=z01_p13.p13ID"));
                    lis.Add(getREL("p19Material", "z01_p19", "Surovina", "LEFT OUTER JOIN p19Material z01_p19 ON a.p10Code=z01_p19.p19Code"));
                    lis.Add(getREL("p11ClientProduct", "z01_p11", "Klientský produkt", "INNER JOIN p11ClientProduct z01_p11 ON a.p11ID = z01_p11.p11ID"));
                    break;
                case "z02":
                    lis.Add(getREL("p19Material", "z02_p19", "Surovina", "INNER JOIN p19Material z02_p19 ON a.p19ID=z02_p19.p19ID"));
                    lis.Add(getREL("p10MasterProduct", "z02_p10", "Master produkt", "LEFT OUTER JOIN p10MasterProduct z02_p10 ON a.p19Code = z02_p10.p10Code"));

                    lis.Add(getREL("p20Unit", "z02_p20", "Měrná jednotka", "INNER JOIN p20Unit z02_p20 ON a.p20ID=z02_p20.p20ID"));
                    
                    
                    lis.Add(getREL("o54TagBindingInline", "p19_o54", "Kategorie", "LEFT OUTER JOIN (SELECT * FROM o54TagBindingInline WHERE o54RecordEntity='p19') p19_o54 ON a.p19ID=p19_o54.o54RecordPid"));


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
