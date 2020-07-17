using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.FileIO;

namespace BL
{
    public class TheColumnsProvider
    {
        private readonly BL.RunningApp _app;
        //private readonly BL.Factory _f;
        private List<BO.TheGridColumn> _lis;
        //private BO.myQuery _mq;
        private string _lastEntity;
        private string _curEntityAlias;
        
        public TheColumnsProvider(BL.RunningApp runningapp)
        {
            _app = runningapp;
            //_f = f;
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();

        }

        public void Refresh()
        {
            _lis = new List<BO.TheGridColumn>();
            SetupPallete();
        }

        private int SetDefaultColWidth(string strFieldType)
        {
            switch (strFieldType)
            {
                case "date":
                    return 90;                    
                case "datetime":
                    return 120;                   
                case "num":
                case "num4":
                case "num5":                
                case "num3":
                    return 100;
                case "num6":
                case "num7":
                    return 115;
                case "num0":
                    return 75;
                case "bool":
                    return 75;
                default:
                    return 0;
            }
            
        }

        private BO.TheGridColumn AF(string strEntity, string strField, string strHeader, int intDefaultFlag = 0, string strSqlSyntax = null, string strFieldType = "string", bool bolIsShowTotals = false,bool bolNotShowRelInHeader=false)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = BL.TheEntities.ByTable(strEntity).AliasSingular;
            }
           
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, DefaultColumnFlag = intDefaultFlag, SqlSyntax = strSqlSyntax, FieldType = strFieldType, IsShowTotals = bolIsShowTotals,NotShowRelInHeader= bolNotShowRelInHeader,FixedWidth= SetDefaultColWidth(strFieldType) });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }

        


        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax, string strFieldType)
        {
            if (strEntity != _lastEntity)
            {
                _curEntityAlias = BL.TheEntities.ByTable(strEntity).AliasSingular;
            }
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, EntityAlias = _curEntityAlias, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType, FixedWidth = SetDefaultColWidth(strFieldType) });
            _lastEntity = strEntity;
        }

        private void AppendTimestamp(string strEntity)
        {
            AF_TIMESTAMP(strEntity, "DateInsert_" + strEntity, "Založeno", "a.DateInsert", "datetime");
            AF_TIMESTAMP(strEntity, "UserInsert_" + strEntity, "Založil", "a.UserInsert", "string");
            AF_TIMESTAMP(strEntity, "DateUpdate_" + strEntity, "Aktualizace", "a.DateUpdate", "datetime");
            AF_TIMESTAMP(strEntity, "UserUpdate_" + strEntity, "Aktualizoval", "a.UserUpdate", "string");
            AF_TIMESTAMP(strEntity, "ValidUntil_" + strEntity, "Platnost záznamu", "a.ValidUntil", "datetime");
            AF_TIMESTAMP(strEntity, "IsValid_" + strEntity, "Časově platné", "convert(bit,case when GETDATE() between a.ValidFrom AND a.ValidUntil then 1 else 0 end)", "bool");
        }
        private void SetupPallete()
        {
            BO.TheGridColumn onecol;

            //p28Company = klienti
            AF("p28Company", "p28Name", "Klient", 1,null,"string",false,true);
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
            AF("p10MasterProduct", "p10Name", "Master produkt", 1,null,"string",false,true);
            AF("p10MasterProduct", "p10Code", "Kód produktu", 1);
            AF("p10MasterProduct", "p10TypeFlag", "Typ produktu", 1, "case a.p10TypeFlag when 1 then 'Zboží' when 2 then 'Polotovar' when 3 then 'Výrobek' when 4 then 'Surovina' when 5 then 'Obal' when 6 then 'Etiketa' end");
            AF("p10MasterProduct", "p10Memo", "Podrobný popis");
            AF("p10MasterProduct", "p10SwLicenseFlag", "SW licence", 0, "case when a.p10SwLicenseFlag>0 then 'SW licence '+convert(varchar(10),a.p10SwLicenseFlag) else null end");
            AF("p10MasterProduct", "p10RecalcUnit2Kg", "Přepočet MJ na VJ", 0, null, "num3");
            AF("p10MasterProduct", "p10PackagingCode", "Kód obalu");
            AppendTimestamp("p10MasterProduct");

            //p21 = licence
            AF("p21License", "p21Name", "Licence", 1,null,"string",false,true);
            AF("p21License", "p21Code", "Kód", 1);
            AF("p21License", "p21PermissionFlag", "Typ licence", 1, "case a.p21PermissionFlag when 1 then 'Standard' when 2 then 'Extend' when 3 then 'Full' else '???' end","string",false,true);
            AF("p21License", "p21Price", "Cena", 0, null, "num", true);
            AppendTimestamp("p21License");

            //p26 = skupina zařízení
            AF("p26Msz", "p26Name", "Skupina zařízení", 1,null,"string",false,true);
            AF("p26Msz", "p26Code", "Kód", 1);
            AF("p26Msz", "p26Memo", "Podrobný popis");
            AppendTimestamp("p26Msz");

            //p27 = zařízení
            AF("p27MszUnit", "p27Name", "Zařízení", 1,null,"string",false,true);
            AF("p27MszUnit", "p27Code", "Kód zařízení", 2);
            AF("p27MszUnit", "p27Capacity", "Kapacita", 1, null, "num0", true);
            AppendTimestamp("p27MszUnit");

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
            AF("j03User", "j03Login", "Login", 1,null,"string",false,true);
            AF("j03User", "j04Name", "Aplikační role", 1, "j03_j04.j04Name","string",false,true);
            AF("j03User", "j03PingTimestamp", "Last ping", 0, "a.j03PingTimestamp", "datetime");

            //p13 = master receptury
            AF("p13MasterTpv", "p13Name", "Název receptury", 1,null,"string",false,true);
            AF("p13MasterTpv", "p13Code", "Číslo receptury", 1,null,"string",false,true);
            AF("p13MasterTpv", "p13Memo", "Podrobný popis");
            AppendTimestamp("p13MasterTpv");

            //o23 = dokumenty
            AF("o23Doc", "o23Name", "Název", 1);
            AF("o23Doc", "RecordPidAlias", "Vazba", 1, "dbo.getRecordAlias(a.o23Entity,a.o23RecordPid)");
            AF("o23Doc", "EntityAlias", "Druh vazby", 1, "dbo.getEntityAlias(a.o23Entity)");
            
            AF("o23Doc", "o23Memo", "Podrobný popis");
            AF("o23Doc", "o23Date", "Datum dokumentu", 0, null, "date");
            AF("o23Doc", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("o23Doc");

            //b02 = workflow stavy            
            AF("b02Status", "b02Name", "Stav", 1,null,"string",false,true);
            AF("b02Status", "b02Color", "🚩", 1, "case when a.b02Color IS NOT NULL then '<div style='+char(34)+'background-color:'+a.b02Color+';'+char(34)+'>&nbsp;</div>' end");
            AF("b02Status", "b02Code", "Kód",1);
            AF("b02Status", "EntityAlias", "Vazba", 1, "dbo.getEntityAlias(a.b02Entity)");            
            AF("b02Status", "b02Ordinary", "Pořadí", 0, null, "num0");
            AF("b02Status", "b02StartFlag", "Start", 0, "case when a.b02StartFlag=1 then 'ANO' end");
            AF("b02Status", "b02MoveFlag", "Pohyb", 0, "case a.b02MoveFlag when 1 then 'Uživatel' when 2 then 'Systém' end");
            AF("b02Status", "b02Memo", "Poznámka");

            //o51 = položka kategorie
            AF("o51Tag", "o51Name", "Položka kategorie", 1, null, "string", false, true);            
            AF("o51Tag", "o51IsColor", "Má barvu", 2, null, "bool");
            AF("o51Tag", "o51ForeColor", "Barva písma",2, "'<div style=\"background-color:'+a.o51ForeColor+';\">písmo</div>'");
            AF("o51Tag", "o51BackColor", "Barva pozadí", 2, "'<div style=\"background-color:'+a.o51BackColor+';\">pozadí</div>'");
            AF("o51Tag", "o51Ordinary", "Pořadí", 0, null, "num0");


            //o53 = kategorie (skupiny položek kategorie)
            AF("o53TagGroup", "o53Name", "Název kategorie", 1, null, "string", false, true);            
            AF("o53TagGroup", "o53IsMultiSelect", "Multi-Select",0,null,"bool");
            AF("o53TagGroup", "o53Entities", "Vazby", 1, "dbo.getEntityAlias_Multi(a.o53Entities)");
            AF("o53TagGroup", "o53Ordinary", "Pořadí", 0, null, "num0");

            AF("o54TagBindingInline", "o54InlineHtml", "Kategorie", 1, null, "string", false, true);
            AF("o54TagBindingInline", "o54InlineText", "Kategorie (pouze text)",1, null, "string", false, true);

            //zatím provizorně v rámci SINGLETON režimu této třídy:
            DL.DbHandler db = new DL.DbHandler(_app.ConnectString, new BO.RunningUser(), _app.LogFolder);
            var dt = db.GetDataTable("select * from o53TagGroup WHERE o53Field IS NOT NULL AND o53Entities IS NOT NULL ORDER BY o53Ordinary");
            foreach(System.Data.DataRow dbrow in dt.Rows)
            {
                
               onecol= AF("o54TagBindingInline", dbrow["o53Field"].ToString(), dbrow["o53Name"].ToString(), 0, null, "string", false, true);
               onecol.VisibleWithinEntityOnly = dbrow["o53Entities"].ToString();
            }
            


            //p19=materiál
            AF("p19Material", "p19Code", "Kód suroviny", 1,null,"string",false,true);
            
            AF("p19Material", "p19Name", "Název suroviny", 1,null,"string",false,true);
            AF("p19Material", "p19TypeFlag", "Typ suroviny", 1, "case a.p19TypeFlag when 1 then 'Zboží' when 2 then 'Polotovar' when 3 then 'Výrobek' when 4 then 'Surovina' when 5 then 'Obal' when 6 then 'Etiketa' end","string",false,true);
            AF("p19Material", "p19Supplier", "Dodavatel", 2);
            AF("p19Material", "p19Intrastat", "Intrastat", 2);
            AF("p19Material", "p19NameAlias", "NAME-ALIAS");
            AF("p19Material", "p19ITSINC", "ITSINC");
            AF("p19Material", "p19ITSCAS", "ITSCAS");
            AF("p19Material", "p19ITSEINECS", "ITSEINECS");

            AF("p19Material", "p19StockActual", "Stav na skladě", 0, null, "num");
            AF("p19Material", "p19StockReserve", "Rezervováno na skladě", 0, null, "num");
            AF("p19Material", "p19StockDate", "Sklad k", 0, null, "datetime");

            AF("p19Material", "p19Lang1", "Jazyk1");
            AF("p19Material", "p19Lang2", "Jazyk2");
            AF("p19Material", "p19Lang3", "Jazyk3");
            AF("p19Material", "p19Lang4", "Jazyk4");

            //j04=aplikační role
            AF("j04UserRole", "j04Name", "Název role", 1);
            AF("j04UserRole", "j04IsClientRole", "Klientská role", 2, null, "bool");

            //p25 = typy zařízení
            AF("p25MszType", "p25Name", "Typ zařízení", 1,null,"string",false,true);
            AF("p25MszType", "p25Code", "Kód", 2);

            //p31 = kapacitní fondy
            AF("p31CapacityFond", "p31Name", "Časový fond", 1,null,"string",false,true);
            AF("p31CapacityFond", "p31DayHour1", "První hodina", 0, null, "num0");
            AF("p31CapacityFond", "p31DayHour2", "Poslední hodina", 0, null, "num0");
            AF("p31CapacityFond", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");
            AppendTimestamp("p31CapacityFond");

            //p14 = technologické operace master receptury
            AF("p14MasterOper", "p14RowNum", "Číslo řádku", 1, null, "num0");
            onecol=AF("p14MasterOper", "p14OperNum", "Číslo Oper", 2, "RIGHT('000'+convert(varchar(10),a.p14OperNum),3)");
            onecol.FixedWidth = 70;

            
            AF("p14MasterOper", "p14OperParam", "Parameter", 2, null, "num1");

            AF("p14MasterOper", "p14UnitsCount", "Množství na 1VJ", 2, null, "num7", true);
            AF("p14MasterOper", "p14DurationPreOper", "Před Oper. Čas", 2, null, "num0", true);
            AF("p14MasterOper", "p14DurationOper", "Oper. Čas", 2, null, "num4", true);
            AF("p14MasterOper", "p14DurOperMinutes", "Počet minut OČ", 0, null, "num", false, true);
            AF("p14MasterOper", "p14DurOperUnits", "Počet jednotek OČ", 0, null, "num", false, true);
            AF("p14MasterOper", "p14DurationPostOper", "Po Oper. Čas", 2, null, "num0", true);
            AF("p14MasterOper", "TotalDuration", "Celk.Čas", 2, "isnull(a.p14DurationPreOper,0)+isnull(a.p14DurationOper,0)+isnull(a.p14DurationPostOper,0)", "num", true);

            AppendTimestamp("p14MasterOper");

            //p11 = klientský produkt
            AF("p11ClientProduct", "p11Code", "Kód produktu", 1,null,"string",false,true);
            AF("p11ClientProduct", "p11Name", "Produkt", 1,null,"string",false,true);

            AF("p11ClientProduct", "p11Memo", "Podrobný popis");
            AF("p11ClientProduct", "p11UnitPrice", "Jedn.cena", 0, null, "num");            
            AF("p11ClientProduct", "p11TypeFlag", "Typ produktu", 1, "case a.p11TypeFlag when 1 then 'Zboží' when 2 then 'Polotovar' when 3 then 'Výrobek' when 4 then 'Surovina' when 5 then 'Obal' when 6 then 'Etiketa' end");

            AF("p11ClientProduct", "p11RecalcUnit2Kg", "Přepočet MJ na VJ", 0, null, "num3");
            AF("p11ClientProduct", "p11PackagingCode", "Kód obalu");

            AppendTimestamp("p11ClientProduct");

            //p12 = klientské receptury
            AF("p12ClientTpv", "p12Name", "Název receptury", 1,null,"string",false,true);
            AF("p12ClientTpv", "p12Code", "Číslo receptury", 1,null,"string",false,true);
            AF("p12ClientTpv", "p12Memo", "Podrobný popis");
            AppendTimestamp("p12ClientTpv");

            //p20 = měrné jednotky
            AF("p20Unit", "p20Code", "MJ", 1,null,"string",false,false);
            AF("p20Unit", "p20Name", "Měrná jednotka", 1,null,"string",false,true);

            AppendTimestamp("p20Unit");

            //p15 = technologické operace klientské receptury
            AF("p15ClientOper", "p15RowNum", "Číslo řádku", 1, null, "num0");            
            onecol=AF("p15ClientOper", "p15OperNum", "Číslo Oper", 2, "RIGHT('000'+convert(varchar(10),a.p15OperNum),3)");
            onecol.FixedWidth = 70;

            
            AF("p15ClientOper", "p15OperParam", "Parametr", 2, null, "num1");

            AF("p15ClientOper", "p15UnitsCount", "Množství na 1VJ", 2, null, "num7", true);
            AF("p15ClientOper", "p15DurationPreOper", "Před Oper. Čas", 2, null, "num0", true);
            AF("p15ClientOper", "p15DurationOper", "Oper. Čas", 2, null, "num4", true);
            AF("p15ClientOper", "p15DurOperMinutes", "Počet minut OČ", 0, null, "num", false, true);
            AF("p15ClientOper", "p15DurOperUnits", "Počet jednotek OČ", 0, null, "num", false, true);
            AF("p15ClientOper", "p15DurationPostOper", "Po Oper. Čas", 2, null, "num0", true);
            AF("p15ClientOper", "TotalDuration", "Celk.Čas", 2, "isnull(a.p15DurationPreOper,0)+isnull(a.p15DurationOper,0)+isnull(a.p15DurationPostOper,0)", "num", true);

            AppendTimestamp("p15ClientOper");

            //p41 = zakázky
            AF("p41Task", "p41Code", "Kód", 1);
            AF("p41Task", "p41Name", "Název", 1);

            AF("p41Task", "p41PlanStart", "Plán zahájení", 2, null, "datetime");
            AF("p41Task", "p41PlanEnd", "Plán dokončení", 2, null, "datetime");
            AF("p41Task", "p41PlanUnitsCount", "Plán kg", 2, null, "num", true);
            AF("p41Task", "p41Duration", "Plán trvá (min)", 2, null, "num", true);
            AF("p41Task", "DurationHours", "Plán trvá (hod)", 0, "a.p41Duration/60", "num", true);
            AF("p41Task", "p41DurationPoPre", "Plán PO-PRE trvá (min)", 2, null, "num", true);
            AF("p41Task", "p41DurationPoPost", "Plán PO-POST trvá (min)", 2, null, "num", true);


            AF("p41Task", "p41StockCode", "Kód skladu", 0);
            AF("p41Task", "p41ActualRowNum", "Aktuální RowNum", 0, null, "num0");

            AF("p41Task", "p41RealStart", "Reálné zahájení", 0, null, "datetime");
            AF("p41Task", "p41RealEnd", "Reálné dokončení", 0, null, "datetime");
            AF("p41Task", "p41RealUnitsCount", "Skutečné množství", 0, null, "num");

            AF("p41Task", "p41Memo", "Podrobný popis");
            AF("p41Task", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p41Task");

            //p44 = plán výrobních operací
            AF("p44TaskOperPlan", "p44RowNum", "Číslo řádku", 1, null, "num0");
            onecol=AF("p44TaskOperPlan", "p44OperNum", "Číslo Oper", 2, "RIGHT('000'+convert(varchar(10),a.p44OperNum),3)");
            onecol.FixedWidth = 70;

            AF("p44TaskOperPlan", "p44StartRounded", "Start", 1, "DATEADD(minute, DATEDIFF(minute, 0,DATEADD(second, 30 - DATEPART(second, a.p44Start + '00:00:30.000'),a.p44Start)), 0)", "time");
            AF("p44TaskOperPlan", "p44EndRounded", "Stop", 1, "DATEADD(minute, DATEDIFF(minute, 0,DATEADD(second, 30 - DATEPART(second, a.p44End + '00:00:30.000'),a.p44End)), 0)", "time");
            AF("p44TaskOperPlan", "p44StartTime", "Start HH:mm", 0, "a.p44Start", "time");
            AF("p44TaskOperPlan", "p44EndTime", "Stop HH:mm", 0, "a.p44End", "time");
            AF("p44TaskOperPlan", "p44Start", "Start (D+HM)", 0, null, "datetime");
            AF("p44TaskOperPlan", "p44End", "Stop (D+HM)", 0, null, "datetime");
            AF("p44TaskOperPlan", "p44Start", "Start (D+HMS)", 0, null, "datetimesec");
            AF("p44TaskOperPlan", "p44End", "Stop (D+HMS)", 0, null, "datetimesec");
            AF("p44TaskOperPlan", "p44TotalDurationOperMin", "Celk. Čas", 2, null, "num", true);

            
            AF("p44TaskOperPlan", "p44OperParam", "Parametr", 2, null, "num1");

            AF("p44TaskOperPlan", "p44MaterialUnitsCount", "Množství ∑", 2, null, "num7", true);
            AF("p44TaskOperPlan", "p44DurationPreOper", "Před Oper. Čas", 2, null, "num0", true);
            AF("p44TaskOperPlan", "p44DurationOper", "Oper. Čas", 2, null, "num4", true);
            AF("p44TaskOperPlan", "p44DurationPostOper", "Po Oper. Čas", 2, null, "num0", true);


            //p45 = skutečná výroba
            AF("p45TaskOperReal", "p45RowNum", "Číslo řádku", 1, null, "num0");
            onecol = AF("p45TaskOperReal", "p45OperNum", "Číslo Oper", 2, "RIGHT('000'+convert(varchar(10),a.p45OperNum),3)");
            onecol.FixedWidth = 70;

            AF("p45TaskOperReal", "p45StartRounded", "Start", 1, "DATEADD(minute, DATEDIFF(minute, 0,DATEADD(second, 30 - DATEPART(second, a.p45Start + '00:00:30.000'),a.p45Start)), 0)", "time");
            AF("p45TaskOperReal", "p45EndRounded", "Stop", 1, "DATEADD(minute, DATEDIFF(minute, 0,DATEADD(second, 30 - DATEPART(second, a.p45End + '00:00:30.000'),a.p45End)), 0)", "time");
            AF("p45TaskOperReal", "p45StartTime", "Start HH:mm", 0, "a.p45Start", "time");
            AF("p45TaskOperReal", "p45EndTime", "Stop HH:mm", 0, "a.p45End", "time");
            AF("p45TaskOperReal", "p45Start", "Start (D+HM)", 0, null, "datetime");
            AF("p45TaskOperReal", "p45End", "Stop (D+HM)", 0, null, "datetime");
            AF("p45TaskOperReal", "p45Start", "Start (D+HMS)", 0, null, "datetimesec");
            AF("p45TaskOperReal", "p45End", "Stop (D+HMS)", 0, null, "datetimesec");
            AF("p45TaskOperReal", "p45TotalDurationOperMin", "Celk. Čas", 2, null, "num", true);

            AF("p45TaskOperReal", "p45OperParam", "Parametr", 2, null, "num1");
            AF("p45TaskOperReal", "p45MaterialBatch", "Šarže suroviny");

            AF("p45TaskOperReal", "p45MaterialUnitsCount", "Množství ∑", 2, null, "num7", true);
            

            AF("p45TaskOperReal", "p45Operator", "Operátor", 2);


            //p51 = objednávky
            AF("p51Order", "p51Code", "Kód objednávky", 1,null,"string",false,true);
            AF("p51Order", "p51Name", "Název", 1);

            AF("p51Order", "p51Date", "Datum přijetí", 2, null, "datetime",false,true);
            AF("p51Order", "p51DateDelivery", "Termín dodání", 2, null, "datetime",false,true);
            AF("p51Order", "p51DateDeliveryConfirmed", "Potvrzený termín dodání", 2, null, "datetime",false,true);

            //AF("p51Order", "p51Order_Kg", "Objednáno kg", 1, null, "num", true);
            //AF("p51Order", "p51Task_Kg", "Již naplánováno kg", 0, null, "num", true);            
            //AF("p51Order", "ZbyvaNaplanovat", "Zbývá naplánovat kg  ", 1, "a.p51Order_Kg - isnull(a.p51Task_Kg,0)", "num", true);


            AF("p51Order", "p51CodeByClient", "Kód podle klienta", 0);
            AF("p51Order", "p51IsDraft", "Draft", 0, null, "bool");

            
            AF("p51Order", "p51Memo", "Podrobný popis");
            AF("p51Order", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p51Order");

            //p52 = položky objednávky
            AF("p52OrderItem", "p52Code", "Kód", 1);
            AF("p52OrderItem", "p52UnitsCount", "Množství", 1, null, "num");
            onecol=AF("p52OrderItem", "Recalc2Kg", "Přepočet na VJ", 0, "a.p52UnitsCount*p11RecalcUnit2Kg", "num", true);
            onecol.RelName = "p52_p11";

            AF("p52OrderItem", "p52Task_UnitsCount", "Již naplánováno", 0, null, "num");
            AF("p52OrderItem", "ZbyvaNaplanovatUnits", "Zbývá naplánovat", 1, "a.p52UnitsCount-isnull(a.p52Task_UnitsCount,0)", "num");
            AF("p52OrderItem", "p52Task_Kg", "Již naplánováno VJ", 0, null, "num",true);
            AF("p52OrderItem", "ZbyvaNaplanovatKg", "Zbývá naplánovat VJ", 0, "a.p52UnitsCount*p11RecalcUnit2Kg-isnull(a.p52Task_Kg,0)", "num",true);

            AF("p52OrderItem", "RecordOwner", "Vlastník záznamu", 0, "dbo.j02_show_as_owner(a.j02ID_Owner)");

            AppendTimestamp("p52OrderItem");

            //p18 = Kódy technologických operací
            //AF("p18OperCode", "p18Code", "Kód operace", 1);
           
            onecol=AF("p18OperCode", "p18Code", "Kód operace", 1, null, "string", false, true);
            onecol.FixedWidth = 70;
            
            AF("p18OperCode", "p18Name", "Název operace",1, null, "string", false, true);
            AF("p18OperCode", "p18Flag", "Atribut pro plánování", 2, "case a.p18Flag when 1 then 'TO' when 2 then 'PO-PRE' when 3 then 'PO-POST' when 4 then 'PO-COOP' end", "string", false, true);
            

            AF("p18OperCode", "p18Memo", "Poznámka");
            //AF("p18OperCode", "p25Name", "Typ zařízení", 2, "p25.p25Name");
            //AF("p18OperCode", "p19Name", "Materiál", 2, "p19.p19Name");
            AF("p18OperCode", "p18UnitsCount", "Množství", 2, null, "num7");
            AF("p18OperCode", "p18DurationPreOper", "Před Oper. Čas", 2, null, "num0");
            AF("p18OperCode", "p18DurationOper", "Oper. Čas", 2, null, "num4");
            AF("p18OperCode", "p18DurOperMinutes", "Počet minut OČ", 0, null, "num",false,true);
            AF("p18OperCode", "p18DurOperUnits", "Počet jednotek OČ", 0, null, "num",false,true);

            AF("p18OperCode", "p18DurationPostOper", "Po Oper. Čas", 2, null, "num0");
            AF("p18OperCode", "p18OperParam", "Parametr", 2, null, "num1");
            AF("p18OperCode", "p18IsRepeatable", "Opakovatelná operace", 0, null, "bool", false, true);
            AF("p18OperCode", "p18IsManualAmount", "Ruční zadání množství", 0, null, "bool", false, true);

            AF("p18OperCode", "p18DurOperUnits", "Počet jednotek pro Oper-čas", 0, null, "num");
            AF("p18OperCode", "p18DurOperMinutes", "Počet minut pro Oper-čas", 0, null, "num");

            AF("p18OperCode", "p18Lang1", "Jazyk1");
            AF("p18OperCode", "p18Lang2", "Jazyk2");
            AF("p18OperCode", "p18Lang3", "Jazyk3");
            AF("p18OperCode", "p18Lang4", "Jazyk4");
            AppendTimestamp("p18OperCode");

            //j90 = access log uživatelů
            AF("j90LoginAccessLog", "j90Date", "Čas", 1, null, "datetime");
            AF("j90LoginAccessLog", "j90BrowserFamily", "Prohlížeč", 1);
            AF("j90LoginAccessLog", "j90BrowserOS", "OS", 1);
            AF("j90LoginAccessLog", "j90BrowserDeviceType", "Device");
            AF("j90LoginAccessLog", "j90BrowserAvailWidth", "Šířka (px)", 1);
            AF("j90LoginAccessLog", "j90BrowserAvailHeight", "Výška (px)", 1);
            AF("j90LoginAccessLog", "j90LocationHost", "Host", 1);
            AF("j90LoginAccessLog", "j90LoginMessage", "Chyba", 1);
            AF("j90LoginAccessLog", "j90CookieExpiresInHours", "Expirace přihlášení", 1, null, "num0");
            AF("j90LoginAccessLog", "j90LoginName", "Login", 1);

            //j92 = ping log uživatelů
            AF("j92PingLog", "j92Date", "Čas", 1, null, "datetime");
            AF("j92PingLog", "j92BrowserFamily", "Prohlížeč", 1);
            AF("j92PingLog", "j92BrowserOS", "OS", 1);
            AF("j92PingLog", "j92BrowserDeviceType", "Device", 1);
            AF("j92PingLog", "j92BrowserAvailWidth", "Šířka (px)", 1);
            AF("j92PingLog", "j92BrowserAvailHeight", "Výška (px)", 1);
            AF("j92PingLog", "j92RequestURL", "Url", 1);


            //j40 = poštovní účty
            AF("j40MailAccount", "j40SmtpHost", "Smtp server", 1);
            AF("j40MailAccount", "j40SmtpName", "Název odesílatele", 2);
            AF("j40MailAccount", "j40SmtpEmail", "Adresa odesílatele", 1);
            AF("j40MailAccount", "j40SmtpPort", "Smtp Port", 2, null, "num0");
            AF("j40MailAccount", "j40UsageFlag", "Typ účtu", 1, "case a.j40UsageFlag when 1 then 'Privátní Smtp účet' when 2 then 'Globální Smtp účet' when 3 then 'Osobní Imap účet' when 4 then 'Globální Imap účet' else null end");


            //x40 = OUTBOX            
            AF("x40MailQueue", "x40WhenProceeded", "Čas", 1, null, "datetime");
            AF("x40MailQueue", "x40SenderName", "Odesílatel", 1);
            AF("x40MailQueue", "x40SenderAddress", "Odesílatel (adresa)");
            AF("x40MailQueue", "x40To", "Komu", 1);
            AF("x40MailQueue", "x40Cc", "Cc");
            AF("x40MailQueue", "x40Bcc", "Bcc");
            AF("x40MailQueue", "x40Subject", "Předmět zprávy", 1);
            AF("x40MailQueue", "x40Body", "Text zprávy", 1, "convert(varchar(150),a.x40Body)+'...'");
            AF("x40MailQueue", "x40Attachments", "Přílohy");
            AF("x40MailQueue", "x40EmlFileSize_KB", "Velikost (kB)", 0, "a.x40EmlFileSize/1024", "num0", true);
            AF("x40MailQueue", "x40EmlFileSize_MB", "Velikost (MB)", 0, "convert(float,a.x40EmlFileSize)/1048576", "num", true);
            AF("x40MailQueue", "x40ErrorMessage", "Chyba", 1);

        }

        
        public List<BO.TheGridColumn> getDefaultPallete(bool bolComboColumns, BO.myQuery mq)
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
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j03Login", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_j03", "j03User", "j04Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("j02_p28", "p28Company", "p28Name", rels, bolComboColumns));

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
                    ret.Add(InhaleColumn4Relation("p14_p18", "p18OperCode", "p18Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p14_p19", "p19Material", "p19Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p14_p19", "p19Material", "p19Name", rels, bolComboColumns));
                    break;
                case "p15":
                    ret.Add(InhaleColumn4Relation("p15_p18", "p18OperCode", "p18Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p15_p18", "p18OperCode", "p18Name", rels, bolComboColumns));
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
                    ret.Add(InhaleColumn4Relation("p26_p25", "p25MszType", "p25Name", rels, bolComboColumns));
                    break;
                case "p27":
                    
                    ret.Add(InhaleColumn4Relation("p27_p31", "p31CapacityFond", "p31Name", rels, bolComboColumns));
                    
                    break;
                case "p41":

                    ret.Add(InhaleColumn4Relation("p41_p27", "p27MszUnit", "p27Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p41_b02", "b02Status", "b02Name", rels, bolComboColumns));
                    break;
                case "p44":
                    ret.Add(InhaleColumn4Relation("p44_p18", "p18OperCode", "p18Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p44_p18", "p18OperCode", "p18Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p44_p19", "p19Material", "p19Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p44_p19", "p19Material", "p19Name", rels, bolComboColumns));
                    break;
                case "p51":
                    ret.Add(InhaleColumn4Relation("p51_p28", "p28Company", "p28Name", rels, bolComboColumns));

                    ret.Add(InhaleColumn4Relation("p51_b02", "b02Status", "b02Name", rels, bolComboColumns));
                    break;
                case "p52":
                    ret.Add(InhaleColumn4Relation("p52_p11", "p11ClientProduct", "p11Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p51_p28", "p28Company", "p28Name", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p11_p20", "p20Unit", "p20Code", rels, bolComboColumns));
                    ret.Add(InhaleColumn4Relation("p52_p51", "p51Order", "p51DateDelivery", rels, bolComboColumns));
                    break;
                case "o51":
                    ret.Add(InhaleColumn4Relation("o51_o53", "o53TagGroup", "o53Name", rels, bolComboColumns));
                    break;
            }

            return ret;


        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {

            return _lis;


        }
        private BO.TheGridColumn InhaleColumn4Relation(string strRelName, string strFieldEntity, string strFieldName, List<BO.EntityRelation> applicable_rels, bool bolComboColumns)
        {
            BO.TheGridColumn c0 = ByUniqueName("a__" + strFieldEntity + "__" + strFieldName);
            BO.TheGridColumn c = Clone2NewInstance(c0);


            BO.EntityRelation rel = applicable_rels.Where(p => p.RelName == strRelName).First();
            c.RelName = strRelName;
            c.RelSql = rel.SqlFrom;
            if (rel.RelNameDependOn != null)
            {
                c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
            }

            if (c.NotShowRelInHeader == true)
            {
                return c;   //nezobrazovat u sloupce název relace
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
            if (_lis.Where(p => p.UniqueName == strUniqueName).Count() > 0)
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
            return new BO.TheGridColumn() { Entity = c.Entity, EntityAlias = c.EntityAlias, Field = c.Field, FieldType = c.FieldType, FixedWidth = c.FixedWidth, Header = c.Header, SqlSyntax = c.SqlSyntax, IsFilterable = c.IsFilterable, IsShowTotals = c.IsShowTotals, IsTimestamp = c.IsTimestamp, RelName = c.RelName, RelSql = c.RelSql, RelSqlDependOn = c.RelSqlDependOn,NotShowRelInHeader=c.NotShowRelInHeader };

        }



        public List<BO.TheGridColumn> ParseTheGridColumns(string strPrimaryPrefix, string strJ72Columns)
        {
            //v strJ72Columns je čárkou oddělený seznam sloupců z pole j72Columns: název relace+__+entita+__+field


            List<BO.EntityRelation> applicable_rels = BL.TheEntities.getApplicableRelations(strPrimaryPrefix);
            List<string> sels = BO.BAS.ConvertString2List(strJ72Columns, ",");
            List<BO.TheGridColumn> ret = new List<BO.TheGridColumn>();

            string[] arr;
            BO.EntityRelation rel;

            for (var i = 0; i < sels.Count; i++)
            {
                arr = sels[i].Split("__");

                if (_lis.Exists(p => p.Entity == arr[1] && p.Field == arr[2]))
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
                        if (c.NotShowRelInHeader == false)
                        {
                            c.Header = c.Header + " [" + rel.AliasSingular + "]";   //zobrazovat název entity v záhlaví sloupce
                        }
                        

                        if (rel.RelNameDependOn != null)
                        {
                            c.RelSqlDependOn = applicable_rels.Where(p => p.RelName == rel.RelNameDependOn).First().SqlFrom;    //relace závisí na jiné relaci
                        }
                    }



                    if ((i == sels.Count - 1) && (c.FieldType == "num" || c.FieldType == "num0" || c.FieldType == "num3"))
                    {
                        c.CssClass = "tdn_lastcol";
                    }
                    ret.Add(c);
                }


            }

            return ret;


        }

        public List<BO.TheGridColumnFilter> ParseAdhocFilterFromString(string strJ72Filter, IEnumerable<BO.TheGridColumn> explicit_cols)
        {
            var ret = new List<BO.TheGridColumnFilter>();
            if (String.IsNullOrEmpty(strJ72Filter) == true) return ret;


            List<string> lis = BO.BAS.ConvertString2List(strJ72Filter, "$$$");
            foreach (var s in lis)
            {
                List<string> arr = BO.BAS.ConvertString2List(s, "###");
                if (explicit_cols.Where(p => p.UniqueName == arr[0]).Count() > 0)
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
