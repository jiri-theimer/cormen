using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyGridViewModel
    {
        public string Entity { get; set; }
        public List<MyGridColumn> Columns { get; set; }
        public string ColFieldPid { get; set; } = "pid";
        public string ScrollingHeight { get; set; }        
        public string TableElementID { get; set; }
        public System.Data.DataTable DT;
        public int RowsCount
        {
            get
            {
                if (this.DT.Rows == null) return 0;
                return this.DT.Rows.Count;
            }
        }
        public MyGridViewModel(string strEntity,string strColFieldPid="pid", string strTableElementID="mg1")
        {
            this.Entity = strEntity;
            this.ScrollingHeight = "500px";
            this.ColFieldPid = strColFieldPid;
            this.TableElementID = strTableElementID;
            Columns = new List<MyGridColumn>();
        }

        public void AddStringCol(string strColHeader, string strField)
        {
            var c = new MyGridColumn() { ColType = "string", ColField = strField, ColHeader = strColHeader };
            Columns.Add(c);
        }
        public void AddNumericCol(string strColHeader, string strField)
        {
            var c = new MyGridColumn() { ColType = "number", ColField = strField, ColHeader = strColHeader };
            Columns.Add(c);
        }
        public void AddDateCol(string strColHeader, string strField)
        {
            var c = new MyGridColumn() { ColType = "date", ColField = strField, ColHeader = strColHeader };
            Columns.Add(c);
        }
        public void AddDateTimeCol(string strColHeader, string strField)
        {
            var c = new MyGridColumn() { ColType = "datetime", ColField = strField, ColHeader = strColHeader };
            Columns.Add(c);
        }
        public void AddLinkCol(string strColHeader, string strField)    //navíc musí existovat sloupec _LinkAlias
        {
            var c = new MyGridColumn() { ColType = "link", ColField = strField, ColHeader = strColHeader };
            Columns.Add(c);
        }
    }

    public class MyGridColumn
    {
        public string ColField { get; set; }
        public string ColType { get; set; }
        public string ColHeader { get; set; }
    }

}
