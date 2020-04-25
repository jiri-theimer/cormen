using System;
using System.Collections.Generic;
using System.Text;

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

        private void AF(string strField, string strHeader,string strSqlSyntax=null)
        {
            _lis.Add(new BO.TheGridColumn() { Field = strField, Header = strHeader,SqlSyntax= strSqlSyntax });
        }

        public List<BO.TheGridColumn> getDefaultPallete()
        {
            switch (_mq.Prefix)
            {
                case "p28":
                    AF("p28Name", "Název");
                    AF("p28Street1", "Ulice");
                    AF("p28City1", "Město");
                    AF("p28RegID", "IČ");
                    AF("pokus", "Čas", "GETDATE()");
                    break;
                case "p10":
                    AF("p10Name", "Název");
                    AF("p10Code", "Kód produktu");
                    AF("p13Code", "TPV");                    
                    break;
                case "p21":
                    AF("p21Name", "Název");
                    AF("p28Name", "Klient");                   
                    break;
                case "p26":
                    AF("p26Name", "Název");
                    AF("p26Code", "Kód");
                    AF("p28Name", "Klient");                   
                    break;
                case "j02":
                    AF("FullName", "Jméno");
                    AF("j02Email", "E-mail");
                    AF("p28Name", "Klient");                    
                    break;
                case "p13":
                    AF("p13Name", "Název");
                    AF("p13Code", "Číslo postupu");                   
                    break;
                case "o23":
                    AF("o23Name", "Název");
                    AF("RecordUrlName", "Vazba");
                    AF("EntityAlias", "");
                    AF("o12Name", "Kategorie");
                    AF("b02Name", "Stav");
                    
                    break;
                default:
                    AF(_mq.Prefix+"Name", "Název");
                    
                    break;
            }

            return _lis;
        }
    }
}
