using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridViewModel
    {
        public string Entity { get; set; }
        public int MasterPID { get; set; }
        public string MasterEntity { get; set; }
        public BO.j72TheGridState GridState { get; set; }
        
        public IEnumerable<BO.TheGridColumn> Columns { get; set; }

        public List<BO.TheGridColumnFilter> AdhocFilter { get; set; }
        
        
    }

   
}
