using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class MenuController : BaseController
    {
        

        private List<BO.MenuItem> _lis;

        public MenuController()
        {
            _lis = new List<BO.MenuItem>();
        }


        public IActionResult Index()
        {
            return View();
        }

        public string CurrentUserMenu()
        {

            AMI("Můj profil", "/Home/MyProfile");
            AMI("Odeslat zprávu", "javascript:_sendmail()");
            AMI("Změnit přístupové heslo", "/Home/ChangePassword");
            AMI("O aplikaci", "/Home/About");
            DIV();
            AMI("Odhlásit se", "/Home/logout");

           
            return FlushResult_NAVLINKs();
        }
        public string CurrentUserFontMenu()
        {
            
            for (int i = 1; i <= 4; i++)
            {
                string s = "Malé písmo";
                if (i == 2) s = "Výchozí velikost písma";
                if (i == 3) s = "Větší";
                if (i == 4) s = "Velké";
                if (Factory.CurrentUser.j03FontStyleFlag == i) s += "&#10004;";
                AMI(s, string.Format("javascript: save_fontstyle_menu({0})",i));
                
            }
          
            return FlushResult_NAVLINKs();
        }
        public string GlobalCreateMenu()
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 2)
            {
                if (!this.TestIfUserEditor(false, true))
                {
                    return "<p>Přístup pouze pro čtení</p>";
                }
                AMI("Objednávka", "javascript:_window_open('/p51/record')");
                AMI("Zakázka", "javascript:_window_open('/p41/record')");
                DIV();
                AMI("Dokument", "javascript:_window_open('/o23/record')");
                AMI("Klient", "javascript:_window_open('/p28/record'");
                AMI("Osoba (kontakt nebo uživatel)", "javascript:_window_open('/j02/record')");
                DIV();
                AMI("Produkt [Client]", "javascript:_window_open('/p11/record')");
                AMI("Receptura [Client]", "javascript:_window_open('/p12/record')");
                DIV();
                AMI("Materiál", "javascript:_window_open('/p19/record')");
                AMI("Měrná jednotka", "javascript:_window_open('/p20/record')");
                AMI("Kapacitní fond", "javascript:_window_open('/p31/record')");
                AMI("Středisko", "javascript:_window_open('/p27/record')");
            }


            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                if (!this.TestIfUserEditor(true, false))
                {
                    return "<p>Přístup pouze pro čtení</p>";
                }
                AMI("Dokument", "javascript:_window_open('/o23/record')");
                AMI("Klient", "javascript:_window_open('/p28/record')");
                AMI("Osoba (kontakt nebo uživatel)", "javascript:_window_open('/j02/record')");
                DIV();
                AMI("Produkt [Master]", "javascript:_window_open('/p10/record')");
                AMI("Receptura [Master]", "javascript:_window_open('/p13/record')");
                AMI("Licence", "javascript:_window_open('/p21/record')");
                DIV();                
                AMI("Stroj", "javascript:_window_open('/p26/record')");
                AMI("Středisko", "javascript:_window_open('/p27/record')");
                DIV();
                AMI("Workflow stav", "javascript:_window_open('/b02/record')");
                AMI("Kategorie", "javascript:_window_open('/o12/record')");
                AMI("Aplikační role", "javascript:_window_open('/j04/record')");
                DIV();
                AMI("Kód operace", "javascript:_window_open('/p18/record')");
                AMI("Materiál", "javascript:_window_open('/p19/record')");
                AMI("Měrná jednotka", "javascript:_window_open('/p20/record')");
                AMI("Typ zařízení", "javascript:_window_open('/p25/record')");
                AMI("Poštovní účet", "javascript:_window_open('/Mail/record_j40')");
                AMI("Kapacitní fond", "javascript:_window_open('/p31/record')");

            }

            return FlushResult_NAVLINKs();
        }

        public string ContextMenu(string entity,int pid)
        {            
            string prefix = entity.Substring(0, 3);

            switch (prefix)
            {
                case "j40":
                    AMI("Upravit (<small>vč. Archivovat a Odstranit</small>)", string.Format("javascript:_edit_full('Mail','record_j40',{0})", pid));
                    AMI("Kopírovat", string.Format("javascript:_clone_full('Mail','record_j40',{0})", pid));
                    break;
                case "x40":
                    AMI("Detail odeslané zprávy", string.Format("javascript:_edit_full('Mail','Record_x40',{0})",pid));
                    DIV();
                    AMI("Zkopírovat do nové zprávy", string.Format(string.Format("javascript: _window_open('/Mail/SendMail?x40id={0}')",pid)));
                    break;
                case "j90":
                case "p14":
                case "p15":
                    AMI("Záznam bez nabídky kontextového menu", "");
                    break;

                default:
                    if (this.TestIfUserEditor(true, true))
                    {
                        AMI("Upravit (<small>vč. Archivovat a Odstranit</small>)", string.Format("javascript:_edit('{0}',{1})", prefix, pid));
                        AMI("Kopírovat", string.Format("javascript:_clone('{0}',{1})", prefix, pid));
                        
                    }
                                        

                    break;
            }

            if (this.TestIfUserEditor(true, true))
            {
                if (prefix == "p28" || prefix == "j02" || prefix == "p10" || prefix == "p13" || prefix == "p26" || prefix == "p21" || prefix == "p51" || prefix == "p41")
                {
                    DIV();
                    AMI("Připojit dokument", string.Format("javascript:_append_doc('{0}',{1})", prefix, pid));
                }
            }
                

            return FlushResult_UL();
        }


        private void AMI(string strName,string strUrl)
        {
            _lis.Add(new BO.MenuItem() { Name = strName, Url = strUrl });
        }
        private void DIV()
        {
            _lis.Add(new BO.MenuItem());
        }

        private string FlushResult_UL()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<ul style='border:0px;'>");
            foreach(var c in _lis)
            {                
                if (c.Name==null)
                {
                    sb.Append("<li><hr></li>");  //divider
                }
                else
                {
                    if (c.Url == null)
                    {
                        sb.Append(string.Format("<li>{0}</li>", c.Name));
                    }
                    else
                    {
                        sb.Append(string.Format("<li><a href=\"{0}\">{1}</a></li>", c.Url, c.Name));
                    }
                }                                

            }

            sb.AppendLine("</ul>");

            return sb.ToString();
        }
        private string FlushResult_NAVLINKs()
        {
            var sb = new System.Text.StringBuilder();

            foreach (var c in _lis)
            {
                if (c.Name == null)
                {
                    sb.Append("<hr>");  //divider
                }
                else
                {
                    if (c.Url == null)
                    {
                        sb.Append(string.Format("<span>{0}</span>", c.Name));
                    }
                    else
                    {
                        sb.Append(string.Format("<a class='nav-link' href=\"{0}\">{1}</a>", c.Url, c.Name));
                    }
                }

            }


            return sb.ToString();
        }
    }
}