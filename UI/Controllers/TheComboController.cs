using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class TheComboController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;

        public TheComboController(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;
        }

        public string GetHtml4TheCombo(string entity, string tableid, string param1, string pids, string filterflag, string searchstring,string masterprefix,int masterpid) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            mq.query_by_entity_prefix = param1;
            mq.IsRecordValid = true;    //v combo nabídce pouze časově platné záznamy
            if (filterflag == "1")
            {
                mq.SearchString = searchstring; //filtrování na straně serveru
                mq.TopRecordsOnly = 50; //maximálně prvních 50 záznamů, které vyhovují podmínce
            }

            var cols = _colsProvider.getDefaultPallete(true,mq);
            mq.explicit_columns = cols;

            mq.explicit_orderby = BL.TheEntities.ByPrefix(mq.Prefix).SqlOrderByCombo;
           
            switch (mq.Prefix) 
            {                
                case "p18":
                    mq.p18flag = BO.BAS.InInt(param1);
                  
                    mq.explicit_orderby = "a.p18Code";                    
                    if (masterprefix == "p25" && masterpid==0)  //v recepturách je kódy operací povinné zobrazovat v kontextu k typu zařízení
                    {                      
                        return "<p>Na vstupu chybí vybrat typ zařízení.</p>";
                    }
                    
                    break;
                
            }
            mq.InhaleMasterEntityQuery(masterprefix, masterpid);
          

            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();

            if (mq.TopRecordsOnly>0)
            {
                if (intRows >= mq.TopRecordsOnly)
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Zobrazeno prvních {0} záznamů. Zpřesněte filtrovací podmínku.</small>", intRows));
                }
                else
                {
                    s.AppendLine(string.Format("<small style='margin-left:10px;'>Počet záznamů: {0}.</small>", intRows));
                }
                
            }

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
                if (mq.Prefix=="p18" || mq.Prefix == "p19" || mq.Prefix == "p21" || mq.Prefix == "p26" || mq.Prefix == "p10" || mq.Prefix == "p13" || mq.Prefix == "p12" || mq.Prefix == "p11")
                {
                    //po výběru hodnoty z comba bude SelectedText kód + název a nikoliv hodnota prvního sloupce
                    s.Append(string.Format(" data-t='{0} - {1}'", dt.Rows[i]["a__"+mq.Entity+"__"+mq.Prefix + "Code"], dt.Rows[i]["a__" + mq.Entity + "__" + mq.Prefix + "Name"]));
                }
                //if (mq.Prefix == "p51")
                //{
                //    s.Append(string.Format(" data-t='{0} - {1} ## {2}'", dt.Rows[i]["a__p51Order__p51Code"], dt.Rows[i]["a__p51Order__p51Name"], dt.Rows[i]["p51_p28_p28Company_p28Name"]));
                //}

                s.Append(">");
                foreach (var col in cols)
                {
                    if (col.NormalizedTypeName == "num")
                    {
                        s.Append(string.Format("<td style='text-align:right;'>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }
                    else
                    {
                        s.Append(string.Format("<td>{0}</td>", BO.BAS.ParseCellValueFromDb(dt.Rows[i], col)));
                    }
                    
                    
                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
        }

    }
}