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
        private BO.myQuery _mq;
        private string _lastEntity;
        private string _curEntityAlias;

        public TheColumnsProvider(BO.myQuery mq)
        {
            _lis = new List<BO.TheGridColumn>();
            _mq = mq;

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
        private void SetupPallete(bool bolIncludeOutsideEntity)
        {            
            if (bolIncludeOutsideEntity || _mq.Prefix == "p28")
            {                   
                AF("p28Company", "p28Name", "Název", 1);
                AF("p28Company", "p28Code", "Kód");
                AF("p28Company", "p28ShortName", "Zkrácený název");
               
                AF("p28Company", "p28Street1", "Ulice",1);
                AF("p28Company", "p28City1", "Město",1);
                AF("p28Company", "p28PostCode1", "PSČ");
                AF("p28Company", "p28Country1", "Stát");

                AF("p28Company", "p28Street2", "Ulice 2");
                AF("p28Company", "p28City2", "Město 2");
                AF("p28Company", "p28PostCode2", "PSČ 2");
                AF("p28Company", "p28Country2", "Stát 2");

                AF("p28Company", "p28RegID", "IČ",1);
                AF("p28Company", "p28VatID", "DIČ");
                AF("p28Company", "p28CloudID", "CLOUD ID");
                AF("p28Company", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                AppendTimestamp("p28Company");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p10")
            {
                AF("p10MasterProduct","p10Name", "Název",1);
                AF("p10MasterProduct","p10Code", "Kód produktu",1);
                //AF("p10MasterProduct", "p20Code", "MJ", 1, "p20.p20Code");
                //AF("p10MasterProduct", "p13Code", "Číslo receptury", 1,"p13.p13Code");
                //AF("p10MasterProduct","b02Name", "Stav",0,"b02.b02Name");
                //AF("p10MasterProduct", "o12Name", "Kategorie", 0, "o12.o12Name");
                AF("p10MasterProduct", "p10Memo", "Podrobný popis");
                AF("p10MasterProduct", "p10SwLicenseFlag", "SW licence", 0, "case when a.p10SwLicenseFlag>0 then 'SW licence '+convert(varchar(10),a.p10SwLicenseFlag) else null end");
                AF("p10MasterProduct", "p10RecalcUnit2Kg", "Přepočet MJ na KG", 0, null, "num3");
                AppendTimestamp("p10MasterProduct");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p21")
            {
                AF("p21License","p21Name", "Název",1);
                AF("p21License", "p21Code", "Kód", 1);
                AF("p21License", "p21PermissionFlag", "Typ licence", 1, "case a.p21PermissionFlag when 1 then 'Standard' when 2 then 'Cyber' else '???' end");
                //AF("p21License", "p28Name", "Klient", 1,"p28.p28Name");
                AF("p21License","p21Price", "Cena",0,null,"num",true);
                AppendTimestamp("p21License");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p26")
            {
                AF("p26Msz","p26Name", "Název",1);
                AF("p26Msz","p26Code", "Kód",1);
                //AF("p26Msz", "p25Name", "Typ zařízení", 2, "p25.p25Name");
                //AF("p26Msz", "p31Name", "Kapacitní fond", 2, "p31.p31Name");
                //AF("p26Msz","p28Name", "Klient",1,"p28.p28Name");
                //AF("p26Msz", "b02Name", "Stav", 0, "b02.b02Name");
                AF("p26Msz", "p26Memo", "Podrobný popis");
                AppendTimestamp("p26Msz");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j02")
            {
                AF("j02Person", "fullname_desc", "Příjmení+Jméno",1,"a.j02LastName+' '+a.j02FirstName");
                AF("j02Person", "fullname_asc", "Jméno+Příjmení", 0, "a.j02FirstName+' '+a.j02LastName");
                AF("j02Person", "j02Email", "E-mail", 1);
                AF("j02Person", "j02FirstName", "Jméno");
                AF("j02Person", "j02LastName", "Příjmení");
                AF("j02Person", "j02TitleBeforeName", "Titul před");
                AF("j02Person", "j02TitleAfterName", "Titul za");                
                AF("j02Person", "j02Tel1", "TEL1");
                AF("j02Person", "j02Tel2", "TEL2");
                AF("j02Person", "fullname_plus_client", "Jméno+Firma", 0, "a.j02FirstName+' '+a.j02LastName+isnull(' ['+j02_p28.p28Name+']','')","string",false,"j02_p28");

                AppendTimestamp("j02Person");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j03")
            {
                AF("j03User", "j03Login", "Login", 1);
                AF("j03User", "j04Name", "Aplikační role", 1, "j03_j04.j04Name");
                AF("j03User", "j03PingTimestamp", "Last ping", 0, "a.j03PingTimestamp", "datetime");
                
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p13")
            {
                AF("p13MasterTpv", "p13Name", "Název", 1);
                AF("p13MasterTpv", "p13Code", "Číslo receptury", 1);
                //AF("p13MasterTpv", "p25Name", "Typ zařízení", 2, "p25.p25Name");
                AF("p13MasterTpv", "p13Memo", "Podrobný popis");
                AppendTimestamp("p13MasterTpv");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "o23")
            {
                AF("o23Doc","o23Name", "Název",1);
                AF("o23Doc", "RecordPidAlias", "Vazba",1, "dbo.getRecordAlias(a.o23Entity,a.o23RecordPid)");
                AF("o23Doc","EntityAlias", "Druh vazby",1, "dbo.getEntityAlias(a.o23Entity)");
                AF("o23Doc","o12Name", "Kategorie",0,"o12.o12Name");
                AF("o23Doc","b02Name", "Stav",0,"b02.b02Name");
                AF("o23Doc", "o12Name", "Kategorie", 0, "o12.o12Name");
                AF("o23Doc", "o23Memo", "Podrobný popis");
                AF("o23Doc", "o23Date", "Datum dokumentu", 0,null,"date");
                AF("o23Doc", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                
                AppendTimestamp("o23Doc");

            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "b02")
            {
                AF("b02Status", "b02Name", "Název", 1);
                AF("b02Status", "EntityAlias", "Vazba", 1, "dbo.getEntityAlias(a.b02Entity)");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "o12")
            {
                AF("o12Category", "o12Name", "Název", 1);
                AF("o12Category", "EntityAlias", "Vazba", 1, "dbo.getEntityAlias(a.o12Entity)");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p19")
            {
                AF("p19Material", "p19Code", "Kód", 1);
                AF("p19Material", "p19Name", "Název", 1);
                //AF("p19Material", "p28Name", "Klient", 1,"p28.p28Name");

                //AF("p19Material", "o12Name", "Kategorie", 2,"o12.o12Name");
                //AF("p19Material", "p20Code", "MJ", 1, "p20.p20Code");

                AF("p19Material", "p19Lang1", "Jazyk1");
                AF("p19Material", "p19Lang2", "Jazyk2");
                AF("p19Material", "p19Lang3", "Jazyk3");
                AF("p19Material", "p19Lang4", "Jazyk4");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j04")
            {
                AF("j04UserRole", "j04Name", "Název role", 1);
                AF("j04UserRole", "j04IsClientRole", "Klientská role", 2,null,"bool");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p25")
            {
                AF("p25MszType", "p25Name", "Název", 1);
                
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p31")
            {
                AF("p31CapacityFond", "p31Name", "Název", 1);
                AF("p31CapacityFond", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                //AF("p31CapacityFond", "Link1", "link", 1,"'<a class=\"link-in-grid\" href=/p31/p31Timeline?p31id='+convert(varchar(10),a.p31ID)+'>Timeline</a>'");
                AppendTimestamp("p31CapacityFond");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p14")
            {
                AF("p14MasterOper", "p14RowNum", "RowNum", 1,null,"num0");
                AF("p14MasterOper", "p14OperNum", "OperNum", 2);
                AF("p14MasterOper", "p18Code", "OperCode", 1,"p18.p18Code");
                AF("p14MasterOper", "p18Name", "OperCodeName", 0, "p18.p18Name");

                AF("p14MasterOper", "p14Name", "Name", 1);
                AF("p14MasterOper", "p14OperParam", "OperPar", 2,null,"num0");

                AF("p14MasterOper", "p19Code", "MaterialCode", 2,"p19.p19Code");
                AF("p14MasterOper", "p19Name", "MaterialName", 2,"p19.p19Name");
               
                AF("p14MasterOper", "p14UnitsCount", "UnitsCount", 2,null,"num");
                AF("p14MasterOper", "p14DurationPreOper", "DurationPreOper", 2,null,"num0");
                AF("p14MasterOper", "p14DurationOper", "DurationOper", 2,null,"num3");                
                AF("p14MasterOper", "p14DurationPostOper", "DurationPostOper", 2,null,"num0");

                AppendTimestamp("p14MasterOper");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p11")
            {
                AF("p11ClientProduct", "p11Code", "Kód produktu", 1);
                AF("p11ClientProduct", "p11Name", "Název", 1);
                
                //AF("p11ClientProduct", "b02Name", "Stav", 0, "b02.b02Name");
                
                AF("p11ClientProduct", "p11Memo", "Podrobný popis");
                AF("p11ClientProduct", "p11UnitPrice", "Jedn.cena",0,null,"num");
                //AF("p11ClientProduct", "p20Code", "MJ",1,"p20.p20Code");

                AF("p11ClientProduct", "p11RecalcUnit2Kg", "Přepočet MJ na KG", 0, null, "num3");

                AppendTimestamp("p11ClientProduct");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p12")
            {
                AF("p12ClientTpv", "p12Name", "Název", 1);
                AF("p12ClientTpv", "p12Code", "Číslo receptury", 1);
                AF("p12ClientTpv", "p12Memo", "Podrobný popis");
                //AF("p12ClientTpv", "p25Name", "Typ zařízení", 2, "p25.p25Name");
                //AF("p12ClientTpv", "p21Name", "Licence", 3, "p21.p21Name");
                AF("p21License", "p21PermissionFlag", "Typ licence", 3, "case p21.p21PermissionFlag when 1 then 'Standard' when 2 then 'Cyber' else '???' end");
                AppendTimestamp("p12ClientTpv");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p20")
            {
                AF("p20Unit", "p20Code", "Kód", 1);
                AF("p20Unit", "p20Name", "Název", 1);
                
                AppendTimestamp("p20Unit");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p15")
            {
                AF("p15ClientOper", "p15RowNum", "RowNum", 1, null, "num0");
                AF("p15ClientOper", "p15OperNum", "OperNum", 2);
                AF("p15ClientOper", "p18Code", "OperCode", 1,"p18.p18Code");
                AF("p15ClientOper", "p18Name", "OperCodeName", 2, "p18.p18Name");

                AF("p15ClientOper", "p15Name", "Name", 1);
                AF("p15ClientOper", "p15OperParam", "OperPar", 2, null, "num0");

                AF("p15ClientOper", "p19Code", "MaterialCode", 2,"p19.p19Code");
                AF("p15ClientOper", "p19Name", "MaterialName", 2,"p19.p19Name");

                AF("p15ClientOper", "p15UnitsCount", "UnitsCount", 2, null, "num");
                AF("p15ClientOper", "p15DurationPreOper", "DurationPreOper", 2, null, "num0");
                AF("p15ClientOper", "p15DurationOper", "DurationOper", 2, null, "num3");
                AF("p15ClientOper", "p15DurationPostOper", "DurationPostOper", 2, null, "num0");

                AppendTimestamp("p15ClientOper");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p41")
            {
                AF("p41Task", "p41Code", "Kód", 1);
                AF("p41Task", "p41Name", "Název", 1);
                
                AF("p41Task", "p28Name", "Klient", 1, "p28.p28Name");
                AF("p41Task", "b02Name", "Stav", 2, "b02.b02Name");
                AF("p41Task", "p26Name", "Stroj", 2, "p26.p26Name");

                AF("p41Task", "p41PlanStart", "Plán zahájení", 2,null, "datetime");
                AF("p41Task", "p41PlanEnd", "Plán dokončení", 2, null, "datetime");
                AF("p41Task", "p41RealStart", "Reálné zahájení", 0, null, "datetime");
                AF("p41Task", "p41RealEnd", "Reálné dokončení", 0, null, "datetime");

                AF("p41Task", "p41StockCode", "Kód skladu", 0);
                AF("p41Task", "p41ActualRowNum", "Aktuální RowNum", 0,null,"num0");
                AF("p41Task", "p41PlanUnitsCount", "Plán množství", 0,null, "num");
                AF("p41Task", "p41RealUnitsCount", "Skutečné množství", 0, null, "num");

                AF("p41Task", "p41Memo", "Podrobný popis");
                AF("p41Task", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("p41Task");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p51")
            {
                AF("p51Order", "p51Code", "Kód", 1);
                AF("p51Order", "p51Name", "Název", 1);                
                AF("p51Order", "p28Name", "Klient", 1, "p28.p28Name");
                AF("p51Order", "b02Name", "Stav", 2, "b02.b02Name");
                AF("p51Order", "p26Name", "Stroj", 2, "p26.p26Name");

                AF("p51Order", "p51Date", "Datum", 2, null, "datetime");
                AF("p51Order", "p51DateDelivery", "Termín dodání", 2, null, "datetime");
               
                AF("p51Order", "p51CodeByClient", "Kód podle klienta", 0);
                AF("p51Order", "p51IsDraft", "Draft",0,null,"bool");
               
                AF("p51Order", "p51Memo", "Podrobný popis");
                AF("p51Order", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("p51Order");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p52")
            {
                AF("p52OrderItem", "p52Code", "Kód", 1);                
                AF("p52OrderItem", "p11Name", "Produkt", 1,"p11.p11Name");
                AF("p52OrderItem", "p52UnitsCount", "Množství", 1, null, "num");
                AF("p52OrderItem", "p20Code", "MJ", 1, "p20.p20Code");
                AF("p52OrderItem", "Recalc2Kg", "Přepočteno na KG", 1, "p52UnitsCount * p11.p11RecalcUnit2Kg", "num", true);

                AF("p52OrderItem", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("p52OrderItem");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p18")
            {
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
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j90")
            {
                AF("j90LoginAccessLog", "j90Date", "Čas", 1,null, "datetime");
                AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", 1);
                AF("j90LoginAccessLog", "j90BrowserOS", "OS", 1);
                AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device", 1);
                AF("j90LoginAccessLog", "j90BrowserAvailWidth", "Šířka (px)", 1);
                AF("j90LoginAccessLog", "j90BrowserAvailHeight", "Výška (px)", 1);
                AF("j90LoginAccessLog", "j90LocationHost", "Host", 1);
                AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", 1);
                AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", 1,null,"num0");
                AF("j90LoginAccessLog", "j90LoginName", "Login", 1);

            }
            if (_lis.Count == 0)
            {
                AF(_mq.Entity,_mq.Prefix + "Name", "Název",1);
                AppendTimestamp(_mq.Entity);
            }

            
        }

        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns)
        {
            int intDefaultFlag1 = 1; int intDefaultFlag2 = 2;
            if (bolComboColumns == true)
            {
                intDefaultFlag2 = 3;
            }

            if (_lis.Count > 0) { _lis.Clear(); };  //intDefaultFlag1: 1=Grid+Combo, 2=Pouze Grid, 3=Pouze Combo
            SetupPallete(true);

            List<BO.TheGridColumn> ret = _lis.Where(p => p.Prefix == _mq.Prefix && (p.DefaultColumnFlag == intDefaultFlag1 || p.DefaultColumnFlag == intDefaultFlag2)).ToList();
            List<BO.EntityRelation> rels = BL.TheEntities.getApplicableRelations(_mq.Prefix);
            
            switch (_mq.Prefix)
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
                    break;
                case "p12":
                    ret.Add(InhaleColumn4Relation("p12_p25", "p25MszType", "p25Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p12_p21", "p21License", "p21Name", rels, bolComboColumns));
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
                    break;                
            }
            
            return ret;


        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(false);

            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity,string strFieldName, List<BO.EntityRelation> applicable_rels,bool bolComboColumns)
        {
            BO.TheGridColumn c = ByUniqueName(strFieldEntity+"__"+strFieldName);

            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
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
        public BO.TheGridColumn ByRelUniqueName(string strRelUniqueName)
        {
            if (_lis.Where(p =>p.RelUniqueName == strRelUniqueName).Count() > 0)
            {
                return _lis.Where(p => p.RelUniqueName == strRelUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        

        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix,string strJ72Columns)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(true);

            List<BO.EntityRelation> applicable_rels = BL.TheEntities.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();
            
            string[] arr;
            string strUniqueName = "";
            BO.EntityRelation rel;

            for (var i=0;i<sels.Count; i++)
            {                
                arr = sels[i].Split("__");
                strUniqueName = arr[1] + "__"+arr[2];
                if (_lis.Where(p => p.UniqueName == strUniqueName).Count() > 0)
                {
                    var c = _lis.Where(p => p.UniqueName == strUniqueName).FirstOrDefault();
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

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;
            SetupPallete(true);
            
            
            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr= BO.BAS.ConvertString2List(s, "###");
                if (_lis.Where(p => p.UniqueName == arr[0]).Count() > 0)
                {
                    var c = new BO.TheGridColumnFilter() { field = arr[0], oper = arr[1], value = arr[2] };
                    c.BoundColumn = _lis.Where(p => p.UniqueName == arr[0]).First();
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
