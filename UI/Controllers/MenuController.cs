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
                AMI("Zakázka", "javascript:_window_open('/p41/Create',2,'Vytvořit zakázky')");
                DIV();
                AMI("Dokument", "javascript:_window_open('/o23/record')");
                AMI("Klient", "javascript:_window_open('/p28/record')");
                AMI("Osoba (kontakt nebo uživatel)", "javascript:_window_open('/j02/record')");
                DIV();
                AMI("Produkt [Client]", "javascript:_window_open('/p11/record')");
                AMI("Receptura [Client]", "javascript:_window_open('/p12/record')");
                DIV();
                AMI("Surovina", "javascript:_window_open('/p19/record')");
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
                AMI("Skupina zařízení", "javascript:_window_open('/p26/record')");
                AMI("Středisko", "javascript:_window_open('/p27/record')");
                DIV();
                AMI("Šablona tiskové sestavy", "javascript:_window_open('/x31/record')");
                AMI("Workflow stav", "javascript:_window_open('/b02/record')");
                AMI("Skupina stavů", "javascript:_window_open('/b03/record')");
                AMI("Položka kategorie", "javascript:_window_open('/o51/record')");
                AMI("Kategorie (skupina)", "javascript:_window_open('/o53/record')");
                AMI("Aplikační role", "javascript:_window_open('/j04/record')");
                DIV();
                AMI("Kód operace", "javascript:_window_open('/p18/record')");
                AMI("Surovina", "javascript:_window_open('/p19/record')");
                AMI("Měrná jednotka", "javascript:_window_open('/p20/record')");
                AMI("Typ zařízení", "javascript:_window_open('/p25/record')");
                AMI("Poštovní účet", "javascript:_window_open('/j40/record')");
                AMI("Kapacitní fond", "javascript:_window_open('/p31/record')");

            }

            return FlushResult_NAVLINKs();
        }

        public string ContextMenu(string entity,int pid)
        {            
            string prefix = entity.Substring(0, 3);

            switch (prefix)
            {
               
                case "x40":
                    AMI("Detail odeslané zprávy", string.Format("javascript:_edit_full('Mail','Record',{0})",pid));
                    DIV();
                    AMI("Zkopírovat do nové zprávy", string.Format(string.Format("javascript: _window_open('/Mail/SendMail?x40id={0}')",pid)));
                    break;
                
                case "j90":
                case "j92":
                case "p44":
                case "p45":               
                case "y02":
                    AMI("Bez nabídky kontextového menu", "");
                    break;
                case "y01":
                    AMI("Náhled sestavy", string.Format("javascript:report_nocontext({0})", pid));
                    DIV();
                    AMI("Administrace sestavy", string.Format("javascript:_edit('x31',{0})", pid));
                    break;
                case "z01":
                    AMI("Karta produktu", string.Format("javascript:_edit('p10',{0})", pid));
                    break;
                case "z02":
                    AMI("Karta suroviny", string.Format("javascript:_edit('p19',{0})", pid));
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
                if (prefix == "p51")
                {
                   
                    DIV();
                    AMI("Přidat položku do objednávky", string.Format("javascript:_window_open('/p52/Record?p51id={0}')", pid));
                    AMI("Naimportovat položky objednávky přes MS-EXCEL", string.Format("javascript:_window_open('/p51/Import')", pid));
                }
                if (prefix == "p52")
                {
                    DIV();
                    AMI("Z položky objednávky naplánovat výrobní zakázky", string.Format("javascript:_window_open('/p41/Create?p52id={0}')", pid));
                }
                if (prefix == "p13")
                {                    
                    DIV();
                    AMI("Přidat do receptury technologickou operaci", string.Format("javascript:_window_open('/p14/Record?p13id={0}')", pid));
                }
                if (prefix == "p41")
                {
                    var rec = Factory.p41TaskBL.Load(pid);
                    if (rec.p41MasterID == 0 || rec.p41SuccessorID>0)
                    {
                        DIV();
                        AMI("Přidej/Aktualizuj PRE operace", string.Format("javascript:_window_open('/p41/p41AppendPo?p18flag=2&p41id={0}')", pid));                        
                    }
                    if (rec.p41MasterID==0 || rec.p41SuccessorID == 0)
                    {
                        AMI("Přidej/Aktualizuj POST operace", string.Format("javascript:_window_open('/p41/p41AppendPo?p18flag=3&p41id={0}')", pid));
                        DIV();
                    }
                    if (rec.p41MasterID == 0)
                    {
                        AMI("Přidej PRE zakázku", string.Format("javascript:_window_open('/p41/p41CreateChild?p18flag=2&p41id={0}')", pid));
                        AMI("Přidej POST zakázku", string.Format("javascript:_window_open('/p41/p41CreateChild?p18flag=3&p41id={0}')", pid));
                        
                        
                        
                    }
                    var recP11 = Factory.p11ClientProductBL.Load(rec.p11ID);
                    if (recP11 != null)
                    {
                        AMI(string.Format("Obnovit plán výroby podle receptury: [{0} - {1}]",recP11.p12Code,recP11.p12Name), string.Format("javascript:_p41_record_recovery({0})", pid));
                    }



                }
                if (prefix == "p14")
                {
                    DIV();
                    //AMI("Od následujího záznamu nahodit OperNum postupku", string.Format("javascript:p14_precisluj_opernum({0})", pid));
                    AMI("Přečíslovat všechny následující operace", string.Format("javascript:p14_precisluj_opernum({0})", pid));
                }
                if (prefix == "p15")
                {
                    DIV();
                    //AMI("Od následujího záznamu nahodit OperNum postupku", string.Format("javascript:p14_precisluj_opernum({0})", pid));
                    AMI("Přečíslovat všechny následující operace", string.Format("javascript:p15_precisluj_opernum({0})", pid));
                }
                if (prefix == "p28" || prefix == "j02" || prefix == "p10" || prefix == "p13" || prefix == "p26" || prefix == "p21" || prefix == "p51" || prefix == "p41")
                {
                    DIV();
                    AMI("Připojit dokument", string.Format("javascript:_append_doc('{0}',{1})", prefix, pid));
                }
                if (prefix == "p21" && this.TestIfUserEditor(true, false)==true)
                {                    
                    DIV();
                    AMI("Aktualizovat klientovi jeho produkty a receptury podle licence", string.Format("javascript:p21_handle_create_client_products({0})", pid));
                    
                }
                
            }

            if (prefix == "p28" || prefix == "j02" || prefix == "p10" || prefix == "p13" || prefix == "p26" || prefix=="p27" || prefix == "p21" || prefix == "p51" || prefix == "p41" || prefix=="p11" || prefix=="p12" || prefix=="o23")
            {
                DIV();
                AMI("Info", string.Format("javascript:_window_open('/{0}/Index?pid={1}')", prefix, pid));
            }
            if (prefix == "p41")
            {               
                AMI("Plán výrobních operací", string.Format("javascript:_window_open('/p41/p44List?pid={1}',2,'Plán výrobních operací zakázky')", prefix, pid));
                var mq = new BO.myQuery("p45TaskOperReal");
                mq.p41id = pid;
                if (Factory.p45TaskOperRealBL.GetList(mq).Count() > 0)
                {
                    AMI("Skutečná výroba", string.Format("javascript:_window_open('/p41/p45List?pid={1}',2,'Skutečná výroba zakázky')", prefix, pid));
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