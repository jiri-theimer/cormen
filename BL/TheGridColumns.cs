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

        private void AF(string strEntity, string strField, string strHeader,bool bolIsDefault=false,string strSqlSyntax=null)
        {
            _lis.Add(new BO.TheGridColumn() { Field = strField,Entity=strEntity, Header = strHeader,IsDefault= bolIsDefault, SqlSyntax = strSqlSyntax });
        }

        private void SetupPallete(bool bolIncludeOutsideEntity)
        {
            if (bolIncludeOutsideEntity || _mq.Prefix == "p28")
            {                
                AF("p28Contact", "p28Name", "Název", true);
                AF("p28Contact","p28Street1", "Ulice",true);
                AF("p28Contact","p28City1", "Město",true);
                AF("p28Contact","p28RegID", "IČ",true);
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p10")
            {
                AF("p10MasterProduct","p10Name", "Název",true);
                AF("p10MasterProduct","p10Code", "Kód produktu",true);
                AF("p10MasterProduct","p13Code", "TPV",true);
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p21")
            {
                AF("p21License","p21Name", "Název",true);
                AF("p21License", "p21Code", "Kód", true);
                AF("p21License","p28Name", "Klient",true);
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "p26")
            {
                AF("p26Msz","p26Name", "Název",true);
                AF("p26Msz","p26Code", "Kód",true);
                AF("p26Msz","p28Name", "Klient",true);
            }
            if (bolIncludeOutsideEntity || _mq.Prefix == "j02")
            {
                AF("j02Person","FullName", "Jméno",true);
                AF("j02Person","j02Email", "E-mail",true);
                AF("j02Person","p28Name", "Klient",true);
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
                AF("o23Doc","o12Name", "Kategorie",true);
                AF("o23Doc","b02Name", "Stav",true);

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
    }
}
