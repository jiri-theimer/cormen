using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class TheComboController : BaseController
    {

        public string GetHtml4TheCombo(string entity, string tableid, string param1, string pids, string filterflag, string searchstring) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            mq.query_by_entity_prefix = param1;
            mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            if (filterflag == "1")
            {
                mq.SearchString = searchstring; //filtrování na straně serveru
            }

            var cols = new BL.TheColumnsProvider(mq).getDefaultPallete(1, 3);
            mq.explicit_columns = cols;

            if (mq.Prefix == "p18")
            {
                mq.p25id = BO.BAS.InInt(param1);    //kódy operací je povinné zobrazovat v kontextu k typu zařízení
            }
            if (mq.Prefix == "p18" && mq.p25id == 0)
            {
                return "<p>Na vstupu chybí vybrat typ zařízení.</p>";
            }


            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();

            s.Append(string.Format("<table id='{0}' class='table table-hover'>", tableid));

            s.Append("<thead><tr>");
            foreach (var col in cols)
            {
                s.Append(string.Format("<th>{0}</th>", col.Header));
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", tableid));
            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr class='txz' data-v='{0}'", dt.Rows[i]["pid"]));
                if (mq.Prefix == "p19" || mq.Prefix == "p21" || mq.Prefix == "p26" || mq.Prefix == "p10" || mq.Prefix == "p13" || mq.Prefix == "p12" || mq.Prefix == "p11")
                {
                    s.Append(string.Format(" data-t='{1} ## {0}'", dt.Rows[i][mq.Prefix + "Code"], dt.Rows[i][mq.Prefix + "Name"]));
                }
                if (mq.Prefix == "p51")
                {
                    s.Append(string.Format(" data-t='{0} - {1} ## {2}'", dt.Rows[i]["p51Code"], dt.Rows[i]["p51Name"], dt.Rows[i]["p28Name"]));
                }

                s.Append(">");
                foreach (var col in cols)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][col.Field]));
                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
        }

    }
}