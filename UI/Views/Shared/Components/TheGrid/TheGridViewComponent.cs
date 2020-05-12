using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent: ViewComponent
    {
        BL.Factory _f;
        public TheGridViewComponent(BL.Factory f)
        {
            _f = f;
        }

        public IViewComponentResult
            Invoke(string entity,int j72id,int go2pid,string master_entity,int master_pid,int contextmenuflag,string ondblclick)
        {
            var ret = new TheGridViewModel();
            ret.Entity = entity;
            var mq = new BO.myQuery(entity);
           

            BO.j72TheGridState cJ72 = null;
            if (j72id == 0)
            {
                cJ72=_f.gridBL.LoadTheGridState(entity, _f.CurrentUser.pid,master_entity);
            }
            else
            {
                cJ72 = _f.gridBL.LoadTheGridState(j72id);
            }
            var colsProvider = new BL.TheColumnsProvider(mq);

            if (cJ72 == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols= colsProvider.getDefaultPallete(1,2);    //výchozí paleta sloupců
                
                cJ72 = new BO.j72TheGridState() { j72Entity = entity, j03ID = _f.CurrentUser.pid,j72Columns=String.Join(",",cols.Select(p=>p.UniqueName)),j72PageSize=100,j72MasterEntity= master_entity };
                var intJ72ID = _f.gridBL.SaveTheGridState(cJ72);
                cJ72= _f.gridBL.LoadTheGridState(intJ72ID);
            }

            cJ72.j72CurrentRecordPid = go2pid;
            cJ72.j72ContextMenuFlag = contextmenuflag;
            ret.ondblclick = ondblclick;
            ret.GridState = cJ72;
            ret.Columns = colsProvider.getSelectedPallete(cJ72.j72Columns);
            ret.AdhocFilter = colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter);
            ret.MasterEntity = master_entity;
            ret.MasterPID = master_pid;


            return View("Default", ret);

            


        }
    }
}
