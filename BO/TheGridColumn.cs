using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheGridColumn
    {
       

        public string Entity { get; set; }
        public string Field { get; set; }
        public string FieldType { get; set; }   //string, bool, int, num, date, datetime
        public string Header { get; set; }
        public string SqlSyntax { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSortable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public bool IsShowTotals { get; set; }
        public int FixedWidth { get; set; }
        
        public string CssClass
        {
            get
            {
                if (FieldType == "num0" || FieldType == "num") return "tdn";
                if (FieldType == "bool") return "tdb";
                return "";
            }
        }
        public string UniqueName
        {
            get
            {
                return Entity + "|" + Field;
            }
        }

    }
}
