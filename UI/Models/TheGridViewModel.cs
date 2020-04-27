using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridViewModel
    {
        public string Entity { get; set; }
        public BO.j72TheGridState GridState { get; set; }
        public IEnumerable<BO.TheGridColumn> Columns { get; set; }

        public List<TheGridColumnFilter> AdhocFilter { get; set; }
        public List<TheGridColumnSorter> AdhocSorter { get; set; }

        
    }

    public class TheGridColumnFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class TheGridColumnSorter
    {
        public string Field { get; set; }
        public string Direction { get; set; }
    }
}
