using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheGridColumns
    {
        private List<BO.TheGridColumn> _lis;
        private BO.myQuery _mq;

        public TheGridColumns(BO.myQuery mq)
        {
            _lis = new List<BO.TheGridColumn>();
            _mq = mq;

        }

        private void AF(string strEntity, string strField, string strHeader,bool bolIsDefault=false, string strSqlSyntax = null,string strFieldType="string",bool bolIsShowTotals=false)
        {
            _lis.Add(new BO.TheGridColumn() { Field = strField,Entity=strEntity, Header = strHeader,IsDefault= bolIsDefault, SqlSyntax = strSqlSyntax,FieldType= strFieldType,IsShowTotals=bolIsShowTotals });
            }

        private void SetupPallete(bool bolIncludeOutsideEntity)
        {
            if (bolIncludeOutsideEntity || _mq.Prefix == "p28")
            {                
                AF("p28Contact", "p28Name", "Název", true);
                AF("p28Contact","p28Street1", "Ulice",true);
                AF("p28Contact","p28City1", "Město",true);
                AF("p28Contact","p28RegID", "IČ",true);
                
                AF("p28Contact", "Pokus1", "BOOL 1", true, "convert(bit,1)", "bool");
                AF("p28Contact", "Pokus2", "NUM 1", true, "convert(float,a.p28ID*99.1234)", "num",true);
                
                AF("p28Contact", "Pokus4", "NUM 2", true, "convert(int,a.p28ID*100)", "num0",true);
                AF("p28Contact", "Pokus3", "Dat 2", true, "a.DateInsert", "datetime");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p10")
            {
                AF("p10MasterProduct","p10Name", "Název",true);
                AF("p10MasterProduct","p10Code", "Kód produktu",true);
                AF("p10MasterProduct","p13Code", "TPV",true,"p13.p13Code");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p21")
            {
                AF("p21License","p21Name", "Název",true);
                AF("p21License", "p21Code", "Kód", true);
                AF("p21License","p28Name", "Klient",true,"p28.p28Name");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p26")
            {
                AF("p26Msz","p26Name", "Název",true);
                AF("p26Msz","p26Code", "Kód",true);
                AF("p26Msz","p28Name", "Klient",true,"p28.p28Name");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j02")
            {
                AF("j02Person", "fullname_desc", "Jméno",true);
                AF("j02Person","j02Email", "E-mail",true);
                AF("j02Person","p28Name", "Klient",true,"p28.p28Name");
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p13")
            {
                AF("p13MasterTpv", "p13Name", "Název", true);
                AF("p13MasterTpv", "p13Code", "Číslo postupu", true);
                
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "o23")
            {
                AF("o23Doc","o23Name", "Název",true);
                AF("o23Doc","RecordUrlName", "Vazba",true);
                AF("o23Doc","EntityAlias", "",true);
                AF("o23Doc","o12Name", "Kategorie",true,"o12.o12Name");
                AF("o23Doc","b02Name", "Stav",true,"b02.b02Name");

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
    }
}
