using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BL
{
    public class TheColumnsProvider
    {
        private List<BO.TheGridColumn> _lis;
        //private BO.myQuery _mq;
        private string _lastEntity;
        private string _curEntityAlias;

        public TheColumnsProvider()
        {
            _lis = new List<BO.TheGridColumn>();         
            SetupPallete();

        }

        private void AF(string strEntity, string strField, string strHeader,int intDefaultFlag=0, string strSqlSyntax = null,string strFieldType="string",bool bolIsShowTotals=false,string strRelName=null)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = BL.TheEntities.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, EntityAlias= _curEntityAlias,RelName=strRelName, Header = strHeader,DefaultColumnFlag= intDefaultFlag, SqlSyntax = strSqlSyntax,FieldType= strFieldType,IsShowTotals=bolIsShowTotals });
            _lastEntity = strEntity;
        }
      

        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax,string strFieldType)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = BL.TheEntities.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType });
            _lastEntity = strEntity;
        }

        private void AppendTimestamp(string strEntity)
        {
            AF_TIMESTAMP(strEntity, "DateInsert_"+ strEntity, "Založeno", "a.DateInsert", "datetime");
            AF_TIMESTAMP(strEntity, "UserInsert_"+ strEntity, "Založil", "a.UserInsert", "string");
            AF_TIMESTAMP(strEntity, "DateUpdate_"+ strEntity, "Aktualizace",  "a.DateUpdate", "datetime");
            AF_TIMESTAMP(strEntity, "UserUpdate_"+ strEntity, "Aktualizoval",  "a.UserUpdate", "string");
            AF_TIMESTAMP(strEntity, "ValidUntil_"+ strEntity, "Platnost záznamu",  "a.ValidUntil", "datetime");
            AF_TIMESTAMP(strEntity, "IsValid_" + strEntity, "Časově platné", "convert(bit,case when GETDATE() between a.ValidFrom AND a.ValidUntil then 1 else 0 end)", "bool");
        }
        private void SetupPallete()
        {
            //p28Company = klienti
            AF("p28Company", "p28Name", "Název", 1);
            AF("p28Company", "p28Code", "Kód");
            AF("p28Company", "p28ShortName", "Zkrácený název");

            AF("p28Company", "p28Street1", "Ulice", 1);
            AF("p28Company", "p28City1", "Město", 1);
            AF("p28Company", "p28PostCode1", "PSČ");
            AF("p28Company", "p28Country1", "Stát");

            AF("p28Company", "p28Street2", "Ulice 2");
            AF("p28Company", "p28City2", "Město 2");
            AF("p28Company", "p28PostCode2", "PSČ 2");
            AF("p28Company", "p28Country2", "Stát 2");

            AF("p28Company", "p28RegID", "IČ", 1);
            AF("p28Company", "p28VatID", "DIČ");
            AF("p28Company", "p28CloudID", "CLOUD ID");
            AF("p28Company", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");
            AppendTimestamp("p28Company");

            //p10 = master produkty
            AF("p10MasterProduct", "p10Name", "Název", 1);
            AF("p10MasterProduct", "p10Code", "Kód produktu", 1);            
            AF("p10MasterProduct", "p10Memo", "Podrobný popis");
            AF("p10MasterProduct", "p10SwLicenseFlag", "SW licence", 0, "case when a.p10SwLicenseFlag>0 then 'SW licence '+convert(varchar(10),a.p10SwLicenseFlag) else null end");
            AF("p10MasterProduct", "p10RecalcUnit2Kg", "Přepočet MJ na KG", 0, null, "num3");
            AppendTimestamp("p10MasterProduct");

            //p21 = licence
            AF("p21License", "p21Name", "Název", 1);
            AF("p21License", "p21Code", "Kód", 1);
            AF("p21License", "p21PermissionFlag", "Typ licence", 1, "case a.p21PermissionFlag when 1 then 'Standard' when 2 then 'Cyber' else '???' end");            
            AF("p21License", "p21Price", "Cena", 0, null, "num", true);
            AppendTimestamp("p21License");

            //p26 = stroje
            AF("p26Msz", "p26Name", "Název", 1);
            AF("p26Msz", "p26Code", "Kód", 1);
            AF("p26Msz", "p26Memo", "Podrobný popis");
            AppendTimestamp("p26Msz");

            //j02 = osoby
            AF("j02Person", "fullname_desc", "Příjmení+Jméno", 1, "a.j02LastName+' '+a.j02FirstName");
            AF("j02Person", "fullname_asc", "Jméno+Příjmení", 0, "a.j02FirstName+' '+a.j02LastName");
            AF("j02Person", "j02Email", "E-mail", 1);
            AF("j02Person", "j02FirstName", "Jméno");
            AF("j02Person", "j02LastName", "Příjmení");
            AF("j02Person", "j02TitleBeforeName", "Titul před");
            AF("j02Person", "j02TitleAfterName", "Titul za");
            AF("j02Person", "j02Tel1", "TEL1");
            AF("j02Person", "j02Tel2", "TEL2");
            AF("j02Person", "fullname_plus_client", "Jméno+Firma", 0, "a.j02FirstName+' '+a.j02LastName+isnull(' ['+relname._company.p28Name+']','')");

            AppendTimestamp("j02Person");

            //j03 = uživatelé
            AF("j03User", "j03Login", "Login", 1);
            AF("j03User", "j04Name", "Aplikační role", 1, "j03_j04.j04Name");
            AF("j03User", "j03PingTimestamp", "Last ping", 0, "a.j03PingTimestamp", "datetime");

            //p13 = master receptury
            AF("p13MasterTpv", "p13Name", "Název", 1);
            AF("p13MasterTpv", "p13Code", "Číslo receptury", 1);            
            AF("p13MasterTpv", "p13Memo", "Podrobný popis");
            AppendTimestamp("p13MasterTpv");

            //o23 = dokumenty
            AF("o23Doc", "o23Name", "Název", 1);
            AF("o23Doc", "RecordPidAlias", "Vazba", 1, "dbo.getRecordAlias(a.o23Entity,a.o23RecordPid)");
            AF("o23Doc", "EntityAlias", "Druh vazby", 1, "dbo.getEntityAlias(a.o23Entity)");
            AF("o23Doc", "o12Name", "Kategorie", 0, "o12.o12Name");
            AF("o23Doc", "b02Name", "Stav", 0, "b02.b02Name");
            AF("o23Doc", "o12Name", "Kategorie", 0, "o12.o12Name");
            AF("o23Doc", "o23Memo", "Podrobný popis");
            AF("o23Doc", "o23Date", "Datum dokumentu", 0, null, "date");
            AF("o23Doc", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("o23Doc");

            //b02 = workflow stavy
            AF("b02Status", "b02Name", "Název", 1);
            AF("b02Status", "EntityAlias", "Vazba", 1, "dbo.getEntityAlias(a.b02Entity)");

            //o12 = kategorie
            AF("o12Category", "o12Name", "Název", 1);
            AF("o12Category", "EntityAlias", "Vazba", 1, "dbo.getEntityAlias(a.o12Entity)");

            //p19=materiál
            AF("p19Material", "p19Code", "Kód", 1);
            AF("p19Material", "p19Name", "Název", 1);

            AF("p19Material", "p19Lang1", "Jazyk1");
            AF("p19Material", "p19Lang2", "Jazyk2");
            AF("p19Material", "p19Lang3", "Jazyk3");
            AF("p19Material", "p19Lang4", "Jazyk4");

            //j04=aplikační role
            AF("j04UserRole", "j04Name", "Název role", 1);
            AF("j04UserRole", "j04IsClientRole", "Klientská role", 2, null, "bool");

            //p25 = typy zařízení
            AF("p25MszType", "p25Name", "Název", 1);

            //p31 = kapacitní fondy
            AF("p31CapacityFond", "p31Name", "Název", 1);
            AF("p31CapacityFond", "p31DayHour1", "První hodina",0,null,"num0");
            AF("p31CapacityFond", "p31DayHour2", "Poslední hodina",0,null,"num0");
            AF("p31CapacityFond", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");            
            AppendTimestamp("p31CapacityFond");

            //p14 = technologické operace master receptury
            AF("p14MasterOper", "p14RowNum", "RowNum", 1, null, "num0");
            AF("p14MasterOper", "p14OperNum", "OperNum", 2);

            AF("p14MasterOper", "p14Name", "Name", 1);
            AF("p14MasterOper", "p14OperParam", "OperPar", 2, null, "num0");

            AF("p14MasterOper", "p14UnitsCount", "UnitsCount", 2, null, "num");
            AF("p14MasterOper", "p14DurationPreOper", "DurationPreOper", 2, null, "num0");
            AF("p14MasterOper", "p14DurationOper", "DurationOper", 2, null, "num3");
            AF("p14MasterOper", "p14DurationPostOper", "DurationPostOper", 2, null, "num0");

            AppendTimestamp("p14MasterOper");

            //p11 = klientský produkt
            AF("p11ClientProduct", "p11Code", "Kód produktu", 1);
            AF("p11ClientProduct", "p11Name", "Název", 1);

            AF("p11ClientProduct", "p11Memo", "Podrobný popis");
            AF("p11ClientProduct", "p11UnitPrice", "Jedn.cena", 0, null, "num");
            
            AF("p11ClientProduct", "p11RecalcUnit2Kg", "Přepočet MJ na KG", 0, null, "num3");

            AppendTimestamp("p11ClientProduct");

            //p12 = klientské receptury
            AF("p12ClientTpv", "p12Name", "Název", 1);
            AF("p12ClientTpv", "p12Code", "Číslo receptury", 1);
            AF("p12ClientTpv", "p12Memo", "Podrobný popis");           
            AppendTimestamp("p12ClientTpv");

            //p20 = měrné jednotky
            AF("p20Unit", "p20Code", "Kód", 1);
            AF("p20Unit", "p20Name", "Název", 1);

            AppendTimestamp("p20Unit");

            //p15 = technologické operace klientské receptury
            AF("p15ClientOper", "p15RowNum", "RowNum", 1, null, "num0");
            AF("p15ClientOper", "p15OperNum", "OperNum", 2);
            //AF("p15ClientOper", "p18Code", "OperCode", 1,"p18.p18Code");
            //AF("p15ClientOper", "p18Name", "OperCodeName", 2, "p18.p18Name");

            AF("p15ClientOper", "p15Name", "Name", 1);
            AF("p15ClientOper", "p15OperParam", "OperPar", 2, null, "num0");

            //AF("p15ClientOper", "p19Code", "MaterialCode", 2,"p19.p19Code");
            //AF("p15ClientOper", "p19Name", "MaterialName", 2,"p19.p19Name");

            AF("p15ClientOper", "p15UnitsCount", "UnitsCount", 2, null, "num");
            AF("p15ClientOper", "p15DurationPreOper", "DurationPreOper", 2, null, "num0");
            AF("p15ClientOper", "p15DurationOper", "DurationOper", 2, null, "num3");
            AF("p15ClientOper", "p15DurationPostOper", "DurationPostOper", 2, null, "num0");

            AppendTimestamp("p15ClientOper");

            //p41 = zakázky
            AF("p41Task", "p41Code", "Kód", 1);
            AF("p41Task", "p41Name", "Název", 1);

            AF("p41Task", "p41PlanStart", "Plán zahájení", 2, null, "datetime");
            AF("p41Task", "p41PlanEnd", "Plán dokončení", 2, null, "datetime");
            AF("p41Task", "p41RealStart", "Reálné zahájení", 0, null, "datetime");
            AF("p41Task", "p41RealEnd", "Reálné dokončení", 0, null, "datetime");

            AF("p41Task", "p41StockCode", "Kód skladu", 0);
            AF("p41Task", "p41ActualRowNum", "Aktuální RowNum", 0, null, "num0");
            AF("p41Task", "p41PlanUnitsCount", "Plán množství", 0, null, "num");
            AF("p41Task", "p41RealUnitsCount", "Skutečné množství", 0, null, "num");

            AF("p41Task", "p41Memo", "Podrobný popis");
            AF("p41Task", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p41Task");

            //p51 = objednávky
            AF("p51Order", "p51Code", "Kód", 1);
            AF("p51Order", "p51Name", "Název", 1);
            
            AF("p51Order", "p51Date", "Datum", 2, null, "datetime");
            AF("p51Order", "p51DateDelivery", "Termín dodání", 2, null, "datetime");

            AF("p51Order", "p51CodeByClient", "Kód podle klienta", 0);
            AF("p51Order", "p51IsDraft", "Draft", 0, null, "bool");

            AF("p51Order", "p51Memo", "Podrobný popis");
            AF("p51Order", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p51Order");

            //p52 = položky objednávky
            AF("p52OrderItem", "p52Code", "Kód", 1);            
            AF("p52OrderItem", "p52UnitsCount", "Množství", 1, null, "num");            
            AF("p52OrderItem", "Recalc2Kg", "Přepočteno na KG", 1, "a.p52UnitsCount * p52_p11.p11RecalcUnit2Kg", "num", true, "p52_p11");

            AF("p52OrderItem", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p52OrderItem");

            //p18 = Kódy technologických operací
            AF("p18OperCode", "p18Code", "Kód", 1);
            AF("p18OperCode", "p18Name", "Název", 1);
            //AF("p18OperCode", "p25Name", "Typ zařízení", 2, "p25.p25Name");
            //AF("p18OperCode", "p19Name", "Materiál", 2, "p19.p19Name");
            AF("p18OperCode", "p18UnitsCount", "UnitsCount", 2, null, "num");
            AF("p18OperCode", "p18DurationPreOper", "DurationPreOper", 2, null, "num3");
            AF("p18OperCode", "p18DurationOper", "DurationPreOper", 2, null, "num3");
            AF("p18OperCode", "p18DurationPostOper", "DurationPostOper", 2, null, "num3");

            AF("p18OperCode", "p18Lang1", "Jazyk1");
            AF("p18OperCode", "p18Lang2", "Jazyk2");
            AF("p18OperCode", "p18Lang3", "Jazyk3");
            AF("p18OperCode", "p18Lang4", "Jazyk4");
            AppendTimestamp("p18OperCode");

            //j90 = access log uživatelů
            AF("j90LoginAccessLog", "j90Date", "Čas", 1, null, "datetime");
            AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", 1);
            AF("j90LoginAccessLog", "j90BrowserOS", "OS", 1);
            AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device", 1);
            AF("j90LoginAccessLog", "j90BrowserAvailWidth", "Šířka (px)", 1);
            AF("j90LoginAccessLog", "j90BrowserAvailHeight", "Výška (px)", 1);
            AF("j90LoginAccessLog", "j90LocationHost", "Host", 1);
            AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", 1);
            AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", 1, null, "num0");
            AF("j90LoginAccessLog", "j90LoginName", "Login", 1);


            //j40 = poštovní účty
            AF("j40MailAccount", "j40SmtpHost", "Smtp server", 1);
            AF("j40MailAccount", "j40SmtpName", "Název odesílatele", 2);
            AF("j40MailAccount", "j40SmtpEmail", "Adresa odesílatele", 1);
            AF("j40MailAccount", "j40SmtpPort", "Smtp Port", 2,null,"num0");
            AF("j40MailAccount", "j40UsageFlag", "Typ účtu", 1, "case a.j40UsageFlag when 1 then 'Privátní Smtp účet' when 2 then 'Globální Smtp účet' when 3 then 'Osobní Imap účet' when 4 then 'Globální Imap účet' else null end");


            //x40 = OUTBOX            
            AF("x40MailQueue", "x40WhenProceeded", "Čas", 1,null,"datetime");            
            AF("x40MailQueue", "x40SenderName", "Odesílatel", 1);
            AF("x40MailQueue", "x40SenderAddress", "Odesílatel (adresa)");
            AF("x40MailQueue", "x40To", "Komu", 1);
            AF("x40MailQueue", "x40Cc", "Cc");
            AF("x40MailQueue", "x40Bcc", "Bcc");            
            AF("x40MailQueue", "x40Subject", "Předmět zprávy", 1);
            AF("x40MailQueue", "x40Body", "Text zprávy", 1, "convert(varchar(150),a.x40Body)+'...'");
            AF("x40MailQueue", "x40Attachments", "Přílohy");
            AF("x40MailQueue", "x40EmlFileSize_KB", "Velikost (kB)", 0, "a.x40EmlFileSize/1024", "num0",true);
            AF("x40MailQueue", "x40EmlFileSize_MB", "Velikost (MB)", 0, "convert(float,a.x40EmlFileSize)/1048576", "num",true);
            AF("x40MailQueue", "x40ErrorMessage", "Chyba", 1);
            
        }

        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns,BO.myQuery mq)
        {
            int intDefaultFlag1 = 1; int intDefaultFlag2 = 2;
            if (bolComboColumns == true)
            {
                intDefaultFlag2 = 3;
            }

            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            foreach (BO.TheGridColumn c in _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)))
            {
                ret.Add(Clone2NewInstance(c));
            }

            //List<BO.TheGridColumn> ret = _lis.Where(p => p.Prefix == mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)).ToList();

            List<BO.EntityRelation> rels = BL.TheEntities.getApplicableRelations(mq.Prefix);
            
            switch (mq.Prefix)
            {
                case "j02":
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User","j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User","j04Name", rels, bolComboColumns));                    
                    ret.Add(InhaleColumn4Relation("j02_p28", "p28Company","p28Name", rels, bolComboColumns));
                    
                    break;
                case "p10":
                    ret.Add(InhaleColumn4Relation("p10_p20", "p20Unit", "p20Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p10_p13", "p13MasterTpv", "p13Code", rels, bolComboColumns));
                    break;
                case "p11":
                    ret.Add(InhaleColumn4Relation("p11_p20", "p20Unit", "p20Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p11_p21", "p21License", "p21Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p11_p21", "p21License", "p21PermissionFlag", rels, bolComboColumns));
                    break;
                case "p12":
                    ret.Add(InhaleColumn4Relation("p12_p25", "p25MszType", "p25Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p12_p21", "p21License", "p21Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p12_p21", "p21License", "p21PermissionFlag", rels, bolComboColumns));
                    
                    break;
                case "p13":
                    ret.Add(InhaleColumn4Relation("p13_p25", "p25MszType", "p25Name", rels, bolComboColumns));
                    break;
                case "p14":
                    ret.Add(InhaleColumn4Relation("p14_p18", "p18OperCode", "p18Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p14_p19", "p19Material", "p19Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p14_p19", "p19Material", "p19Name", rels, bolComboColumns));
                    break;
                case "p15":
                    ret.Add(InhaleColumn4Relation("p15_p18", "p18OperCode", "p18Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p15_p19", "p19Material", "p19Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p15_p19", "p19Material", "p19Name", rels, bolComboColumns));
                    break;
                case "p21":
                    ret.Add(InhaleColumn4Relation("p21_p28", "p28Company", "p28Name", rels, bolComboColumns));
                    break;
                case "p19":
                    ret.Add(InhaleColumn4Relation("p19_p20", "p20Unit", "p20Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p19_p28", "p28Company", "p28Name", rels, bolComboColumns));
                    break;
                case "p26":
                    ret.Add(InhaleColumn4Relation("p26_p31", "p31CapacityFond", "p31Name", rels, bolComboColumns));
                    break;
                case "p41":
                    ret.Add(InhaleColumn4Relation("p41_p28", "p28Company", "p28Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p41_p26", "p26Msz", "p26Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p41_b02", "b02Status", "b02Name", rels, bolComboColumns));
                    break;
                case "p51":
                    ret.Add(InhaleColumn4Relation("p51_p28", "p28Company", "p28Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p51_p26", "p26Msz", "p26Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p51_b02", "b02Status", "b02Name", rels, bolComboColumns));
                    break;
                case "p52":                    
                    ret.Add(InhaleColumn4Relation("p52_p11", "p11ClientProduct", "p11Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p11_p20", "p20Unit", "p20Code", rels, bolComboColumns));
                    break;                
            }
            
            return ret;


        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {
            
            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity,string strFieldName, List<BO.EntityRelation> applicable_rels,bool bolComboColumns)
        {
            BO.TheGridColumn c0 = ByUniqueName("a__"+strFieldEntity + "__"+strFieldName);            
            BO.TheGridColumn c = Clone2NewInstance(c0);
           

            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
            if (rel.RelNameDependOn != null)
            {
                c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
            }
            
            if (bolComboColumns == true)
            {
                c.Header = rel.AliasSingular;
            }
            else
            {
                c.Header = c.Header + " [" + rel.AliasSingular + "]";
            }
           
            
            return c;
        }
        public BO.TheGridColumn ByUniqueName(string strUniqueName)
        {
            if (_lis.Where(p=>p.UniqueName == strUniqueName).Count() > 0)
            {
               return _lis.Where(p => p.UniqueName == strUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        private BO.TheGridColumn Clone2NewInstance(BO.TheGridColumn c)
        {
            return new BO.TheGridColumn() { Entity = c.Entity, EntityAlias = c.EntityAlias, Field = c.Field, FieldType = c.FieldType, FixedWidth = c.FixedWidth, Header = c.Header, SqlSyntax = c.SqlSyntax, IsFilterable = c.IsFilterable, IsShowTotals = c.IsShowTotals, IsTimestamp = c.IsTimestamp,RelName=c.RelName,RelSql=c.RelSql,RelSqlDependOn=c.RelSqlDependOn };

        }



        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix,string strJ72Columns)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field
            

            List<BO.EntityRelation> applicable_rels = BL.TheEntities.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            
            string[] arr;            
            BO.EntityRelation rel;

            for (var i=0;i<sels.Count; i++)
            {                
                arr = sels[i].Split("__");                
               
                if (_lis.Exists(p => p.Entity == arr[1] && p.Field==arr[2]))
                {
                    //var c0 = _lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First();
                    BO.TheGridColumn c = Clone2NewInstance(_lis.Where(p => p.Entity == arr[1] && p.Field == arr[2]).First());

                    if (arr[0] == "a")
                    {
                        c.RelName = null;
                    }
                    else
                    {
                        c.RelName = arr[0]; //název relace v sql dotazu
                        rel = applicable_rels.Where(p => p.RelName == c.RelName).First();
                        c.RelSql = rel.SqlFrom;    //sql klauzule relace                        
                        c.Header = c.Header + " [" + rel.AliasSingular + "]";
                        
                        if (rel.RelNameDependOn != null)
                        {
                            c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
                        }
                    }
                   
                                        

                    if ((i == sels.Count - 1) && (c.FieldType=="num" || c.FieldType=="num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);
                }

                
            }
         
            return ret;


        }

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter,IEnumerable<BO.TheGridColumn> explicit_cols)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;
            
            
            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr= BO.BAS.ConvertString2List(s, "###");
                if (explicit_cols.Where(p => p.UniqueName == arr[0]).Count()>0)
                {
                    var c = new BO.TheGridColumnFilter() { field = arr[0], oper = arr[1], value = arr[2] };
                    c.BoundColumn = explicit_cols.Where(p => p.UniqueName == arr[0]).First();
                    ParseFilterValue(ref c);
                    ret.Add(c);
                }
                
                
            }
            return ret;
        }

        private void ParseFilterValue(ref BO.TheGridColumnFilter col)
        {

            {          
                if (col.value.Contains("|"))
                {
                    var a = col.value.Split("|");
                    col.c1value = a[0];
                    col.c2value = a[1];
                }
                else
                {
                    col.c1value = col.value;
                    col.c2value = "";
                }
                switch (col.oper)
                {
                    case "1":
                        {
                            col.value_alias = "Je prázdné";
                            break;
                        }

                    case "2":
                        {
                            col.value_alias = "Není prázdné";
                            break;
                        }

                    case "3":  // obsahuje
                        {
                            col.value_alias = col.c1value;
                            break;
                        }

                    case "5":  // začíná na
                        {
                            
                            col.value_alias = "[*=] " + col.c1value;
                            break;
                        }

                    case "6":  // je rovno
                        {                            
                            col.value_alias = "[=] " + col.c1value;
                            break;
                        }

                    case "7":  // není rovno
                        {                            
                            col.value_alias = "[<>] " + col.c1value;
                            break;
                        }

                    case "8":
                        {
                            col.value_alias = "ANO";
                            break;
                        }

                    case "9":
                        {
                            col.value_alias = "NE";
                            break;
                        }

                    case "10": // je větší než nula
                        {
                            col.value_alias = "větší než 0";
                            break;
                        }

                    case "11":
                        {
                            col.value_alias = "0 nebo prázdné";
                            break;
                        }

                    case "4":  // interval
                        {
                            
                            if (col.BoundColumn.FieldType == "date" | col.BoundColumn.FieldType == "datetime")
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;   // datum
                            }
                            else
                            {
                                col.value_alias = col.c1value + " - " + col.c2value;
                            }    // číslo

                            break;
                        }
                }



               
               
            }


        }


    }
}
