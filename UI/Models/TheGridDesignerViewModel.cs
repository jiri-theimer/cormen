using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridDesignerViewModel:BaseViewModel
    {
        public BO.j72TheGridState Rec { get; set; }
        public IEnumerable<BO.TheGridColumn> AllColumns;
        public List<BO.TheGridColumn> SelectedColumns;


    }
}
