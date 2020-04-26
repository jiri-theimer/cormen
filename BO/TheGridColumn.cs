using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheGridColumn
    {
        public string Entity { get; set; }
        public string Field { get; set; }
        public string Header { get; set; }
        public string SqlSyntax { get; set; }
        public bool IsDefault {get;set;}

    }
}
