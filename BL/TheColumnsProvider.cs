using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheColumnsProvider
    {
        private List<BO.TheGridColumn> _lis;
        private BO.myQuery _mq;

        public TheColumnsProvider(BO.myQuery mq)
        {
            _lis = new List<BO.TheGridColumn>();
            _mq = mq;

        }

        private void AF(string strEntity, string strField, string strHeader,bool bolIsDefault=false, string strSqlSyntax = null,string strFieldType="string",bool bolIsShowTotals=false)
        {
            _lis.Add(new BO.TheGridColumn() { Field = strField, Entity = strEntity, Header = strHeader,IsDefault= bolIsDefault, SqlSyntax = strSqlSyntax,FieldType= strFieldType,IsShowTotals=bolIsShowTotals });
            }
        private void AF_TIMESTAMP(string strEntity, string strField, string strHeader, string strSqlSyntax,string strFieldType)
        {
            _lis.Add(new BO.TheGridColumn() { IsTimestamp = true, Field = strField, Entity = strEntity, Header = strHeader, SqlSyntax = strSqlSyntax, FieldType = strFieldType });
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
                AF("p28Company", "p28Name", "Název", true);
                AF("p28Company", "p28Code", "Kód");
                AF("p28Company", "p28ShortName", "Zkrácený název");
               
                AF("p28Company", "p28Street1", "Ulice",true);
                AF("p28Company", "p28City1", "Město",true);
                AF("p28Company", "p28PostCode1", "PSČ");
                AF("p28Company", "p28Country1", "Stát");

                AF("p28Company", "p28Street2", "Ulice 2");
                AF("p28Company", "p28City2", "Město 2");
                AF("p28Company", "p28PostCode2", "PSČ 2");
                AF("p28Company", "p28Country2", "Stát 2");

                AF("p28Company", "p28RegID", "IČ",true);
                AF("p28Company", "p28VatID", "DIČ");
                AF("p28Company", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                AppendTimestamp("p28Company");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p10")
            {
                AF("p10MasterProduct","p10Name", "Název",true);
                AF("p10MasterProduct","p10Code", "Kód produktu",true);
                AF("p10MasterProduct", "p13Code", "Číslo postupu", true,"p13.p13Code");
                AF("p10MasterProduct","b02Name", "Stav",false,"b02.b02Name");
                AF("p10MasterProduct", "o12Name", "Kategorie", false, "o12.o12Name");
                AF("p10MasterProduct", "p10Memo", "Podrobný popis");
                AF("p10MasterProduct", "p10SwLicenseFlag", "SW licence", false, "case when a.p10SwLicenseFlag>0 then 'SW licence '+convert(varchar(10),a.p10SwLicenseFlag) else null end");

                AppendTimestamp("p10MasterProduct");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p21")
            {
                AF("p21License","p21Name", "Název",true);
                AF("p21License", "p21Code", "Kód", true);
                AF("p21License","p21Price", "Cena",false,null,"num",true);
                AppendTimestamp("p21License");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p26")
            {
                AF("p26Msz","p26Name", "Název",true);
                AF("p26Msz","p26Code", "Kód",true);
                AF("p26Msz","p28Name", "Klient",true,"p28.p28Name");
                AF("p26Msz", "b02Name", "Stav", false, "b02.b02Name");
                AF("p26Msz", "p26Memo", "Podrobný popis");
                AppendTimestamp("p26Msz");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j02")
            {
                AF("j02Person", "fullname_desc", "Příjmení+Jméno",true,"a.j02LastName+' '+a.j02FirstName");
                AF("j02Person", "fullname_asc", "Jméno+Příjmení", false, "a.j02FirstName+' '+a.j02LastName");
                AF("j02Person", "j02Email", "E-mail", true);
                AF("j02Person", "j02FirstName", "Jméno");
                AF("j02Person", "j02LastName", "Příjmení");
                AF("j02Person", "j02TitleBeforeName", "Titul před");
                AF("j02Person", "j02TitleAfterName", "Titul za");
                AF("j02Person", "p28Name", "Klient", true, "p28.p28Name");
                AF("j02Person", "j02Tel1", "TEL1");
                AF("j02Person", "j02Tel2", "TEL2");
                AF("j02Person", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("j02Person");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j03")
            {
                AF("j03User", "j03Login", "Login", true);
                AF("j03User", "j04Name", "Aplikační role", true,"j04.j04Name");
                AF("j03User", "j03PingTimestamp", "Last ping", false, "j03.j03PingTimestamp", "datetime");
                
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p13")
            {
                AF("p13MasterTpv", "p13Name", "Název", true);
                AF("p13MasterTpv", "p13Code", "Číslo postupu", true);
                AF("p13MasterTpv", "p13Memo", "Podrobný popis");
                AppendTimestamp("p13MasterTpv");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "o23")
            {
                AF("o23Doc","o23Name", "Název",true);
                AF("o23Doc", "RecordPidAlias", "Vazba",true, "dbo.getRecordAlias(a.o23Entity,a.o23RecordPid)");
                AF("o23Doc","EntityAlias", "Druh vazby",true, "dbo.getEntityAlias(a.o23Entity)");
                AF("o23Doc","o12Name", "Kategorie",false,"o12.o12Name");
                AF("o23Doc","b02Name", "Stav",false,"b02.b02Name");
                AF("o23Doc", "o12Name", "Kategorie", false, "o12.o12Name");
                AF("o23Doc", "o23Memo", "Podrobný popis");
                AF("o23Doc", "o23Date", "Datum dokumentu", false,null,"date");
                AF("o23Doc", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                AF("o23Doc", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");
                AppendTimestamp("o23Doc");

            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "b02")
            {
                AF("b02Status", "b02Name", "Název", true);
                AF("b02Status", "EntityAlias", "Vazba", true, "dbo.getEntityAlias(a.b02Entity)");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "o12")
            {
                AF("o12Category", "o12Name", "Název", true);
                AF("o12Category", "EntityAlias", "Vazba", true, "dbo.getEntityAlias(a.o12Entity)");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p19")
            {
                AF("p19Material", "p19Code", "Kód", true);
                AF("p19Material", "p19Name", "Název", true);
                AF("p19Material", "p28Name", "Klient", true,"p28.p28Name");

                AF("p19Material", "o12Name", "Kategorie", true,"o12.o12Name");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j04")
            {
                AF("j04UserRole", "j04Name", "Název role", true);
                AF("j04UserRole", "j04IsClientRole", "Klientská role", true,null,"bool");


            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p14")
            {
                AF("p14MasterOper", "p14RowNum", "RowNum", true,null,"num0");
                AF("p14MasterOper", "p14OperNum", "OperNum", true);
                AF("p14MasterOper", "p18Code", "OperCode", true,"p18.p18Code");
                AF("p14MasterOper", "p18Name", "OperCodeName", false, "p18.p18Name");

                AF("p14MasterOper", "p14Name", "Name", true);
                AF("p14MasterOper", "p14OperParam", "OperPar", true,null,"num0");

                AF("p14MasterOper", "p19Code", "MaterialCode", true,"p19.p19Code");
                AF("p14MasterOper", "p19Name", "MaterialName", true,"p19.p19Name");
               
                AF("p14MasterOper", "p14UnitsCount", "UnitsCount", true,null,"num");
                AF("p14MasterOper", "p14DurationPreOper", "DurationPreOper", true,null,"num");
                AF("p14MasterOper", "p14DurationOper", "DurationOper", true,null,"num");                
                AF("p14MasterOper", "p14DurationPostOper", "DurationPostOper", true,null,"num");

                AppendTimestamp("p14MasterOper");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p11")
            {
                AF("p11ClientProduct", "p11Name", "Název", true);
                AF("p11ClientProduct", "p11Code", "Kód produktu", true);
                AF("p11ClientProduct", "b02Name", "Stav", false, "b02.b02Name");
                AF("p11ClientProduct", "o12Name", "Kategorie", false, "o12.o12Name");
                AF("p11ClientProduct", "p11Memo", "Podrobný popis");
                AF("p11ClientProduct", "p11UnitPrice", "Jedn.cena",false,null,"num");
                AF("p11ClientProduct", "p20Code", "Jednotka",false,"p20.p20Code");

                AppendTimestamp("p11ClientProduct");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p12")
            {
                AF("p12ClientTpv", "p12Name", "Název", true);
                AF("p12ClientTpv", "p12Code", "Číslo postupu", true);
                AF("p12ClientTpv", "p12Memo", "Podrobný popis");
                AppendTimestamp("p13MasterTpv");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p15")
            {
                AF("p15ClientOper", "p15RowNum", "RowNum", true, null, "num0");
                AF("p15ClientOper", "p15OperNum", "OperNum", true);
                AF("p15ClientOper", "p18Code", "OperCode", true,"p18.p18Code");
                AF("p15ClientOper", "p18Name", "OperCodeName", false, "p18.p18Name");

                AF("p15ClientOper", "p15Name", "Name", true);
                AF("p15ClientOper", "p15OperParam", "OperPar", true, null, "num0");

                AF("p15ClientOper", "p19Code", "MaterialCode", true,"p19.p19Code");
                AF("p15ClientOper", "p19Name", "MaterialName", true,"p19.p19Name");

                AF("p15ClientOper", "p15UnitsCount", "UnitsCount", true, null, "num");
                AF("p15ClientOper", "p15DurationPreOper", "DurationPreOper", true, null, "num");
                AF("p15ClientOper", "p15DurationOper", "DurationOper", true, null, "num");
                AF("p15ClientOper", "p15DurationPostOper", "DurationPostOper", true, null, "num");

                AppendTimestamp("p15ClientOper");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p41")
            {
                AF("p41Task", "p41Name", "Název", true);
                AF("p41Task", "p41Code", "Kód", true);
                AF("p41Task", "p28Name", "Klient", true, "p28.p28Name");
                AF("p41Task", "b02Name", "Stav", true, "b02.b02Name");
                AF("p41Task", "p26Name", "Stroj", true, "p26.p26Name");

                AF("p41Task", "p41PlanStart", "Plán zahájení", true,null, "datetime");
                AF("p41Task", "p41PlanEnd", "Plán dokončení", true, null, "datetime");
                AF("p41Task", "p41RealStart", "Reálné zahájení", false, null, "datetime");
                AF("p41Task", "p41RealEnd", "Reálné dokončení", false, null, "datetime");

                AF("p41Task", "p41StockCode", "Kód skladu", false);
                AF("p41Task", "p41ActualRowNum", "Aktuální RowNum", false,null,"num0");
                AF("p41Task", "p41PlanUnitsCount", "Plán množství", false,null, "num");
                AF("p41Task", "p41RealUnitsCount", "Skutečné množství", false, null, "num");

                AF("p41Task", "p41Memo", "Podrobný popis");
                AF("p41Task", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("p41Task");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p51")
            {
                AF("p51Order", "p51Name", "Název", true);
                AF("p51Order", "p51Code", "Kód", true);
                AF("p51Order", "p28Name", "Klient", true, "p28.p28Name");
                AF("p51Order", "b02Name", "Stav", true, "b02.b02Name");
                AF("p51Order", "p26Name", "Stroj", true, "p26.p26Name");

                AF("p51Order", "p51Date", "Datum", true, null, "date");
                AF("p51Order", "p51DateDelivery", "Termín dodání", true, null, "datetime");
               
                AF("p51Order", "p51CodeByClient", "Kód podle klienta", false);
                AF("p51Order", "p51IsDraft", "Draft",false,null,"bool");
               
                AF("p51Order", "p51Memo", "Podrobný popis");
                AF("p51Order", "RecordOwner", "Vlastník záznamu", false, "dbo.j02_show_as_owner(a.j02ID_Owner)");

                AppendTimestamp("p51Order");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p18")
            {
                AF("p18OperCode", "p18Code", "Kód", true);
                AF("p18OperCode", "p18Name", "Název", true);

            }
            if (_lis.Count == 0)
            {
                AF(_mq.Entity,_mq.Prefix + "Name", "Název",true);
            }

            
        }

        public IEnumerable<BO.TheGridColumn> getDefaultPallete()
        {
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(false);

            return _lis.Where(p => p.IsDefault == true);

            
        }
        public IEnumerable<BO.TheGridColumn> AllColumns()
        {
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(false);

            return _lis;


        }
        public BO.TheGridColumn FindOneColumn(string strUniqueName)
        {
            if (_lis.Where(p=>p.UniqueName== strUniqueName).Count() > 0)
            {
               return _lis.Where(p => p.UniqueName == strUniqueName).First();
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<BO.TheGridColumn> ApplicableColumns()    //Sloupce, které se nabízejí v návrháři jako možné k práci pro záznamy dané entity
        {
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(true);
          
            switch (_mq.Prefix)
            {
                case "p28":
                    return _lis.Where(p=>p.Entity==_mq.Entity);
               
                case "p10":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix=="p13" || p.Prefix== "b02" || p.Entity == "o12");
                case "p26":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p28" || p.Prefix == "b02");
                case "p19":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p28" || p.Prefix == "o12");
                case "p21":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p28" || p.Prefix == "b02");
                case "j02":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "j03" || p.Prefix == "p28");
                case "o23":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "b02" || p.Prefix == "o12");
                case "p11":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p12" || p.Prefix=="p21" || p.Prefix == "b02" || p.Prefix=="p28" || p.Prefix=="p10");
                case "p12":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p21" || p.Prefix=="p28");
                case "p41":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix=="p11" || p.Prefix == "p28" || p.Prefix == "p26" || p.Prefix == "b02");
                case "p51":
                    return _lis.Where(p => p.Entity == _mq.Entity || p.Prefix == "p28" || p.Prefix == "p26" || p.Prefix == "b02");
                default:
                    return _lis.Where(p => p.Entity == _mq.Entity);
            }
            

        }

        public List<BO.TheGridColumn> getSelectedPallete(string strUniqueNames)
        {
            if (_lis.Count > 0) { _lis.Clear(); };
            SetupPallete(true);

            var sels = BO.BAS.ConvertString2List(strUniqueNames, ",");
            var ret = new List<BO.TheGridColumn>();
            

            for(var i=0;i<sels.Count; i++)
            {
                if (_lis.Where(p => p.UniqueName == sels[i]).Count() > 0)
                {
                    var c = _lis.Where(p => p.UniqueName == sels[i]).FirstOrDefault();
                    if ((i == sels.Count - 1) && (c.FieldType=="num" || c.FieldType=="num0"))
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
