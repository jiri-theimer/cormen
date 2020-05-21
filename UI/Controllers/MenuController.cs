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


        public IActionResult Index()
        {
            return View();
        }

        public string ContextMenu(string entity,int pid)
        {
            _lis = new List<BO.MenuItem>();
            string prefix = entity.Substring(0, 3);

            switch (prefix)
            {
                case "j40":
                    AMI("Upravit (<small>vč. Archivovat a Odstranit</small>)", string.Format("javascript:_edit_full('Mail','record_j40',{0})", pid));
                    AMI("Kopírovat", string.Format("javascript:_clone_full('Mail','record_j40',{0})", pid));
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
                

            return FlushMenu();
        }


        private void AMI(string strName,string strUrl)
        {
            _lis.Add(new BO.MenuItem() { Name = strName, Url = strUrl });
        }
        private void DIV()
        {
            _lis.Add(new BO.MenuItem());
        }

        private string FlushMenu()
        {
            var sb = new System.Text.StringBuilder();


            sb.AppendLine("<ul style='border:0px;'>");
            foreach(var c in _lis)
            {
                sb.AppendLine(c.Html);
            }

            sb.AppendLine("</ul>");


            return sb.ToString();
        }
    }
}