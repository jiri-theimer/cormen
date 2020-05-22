using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery
    {
        private string _prefix;
        private string _pkfield;
        private string _Entity;
        public myQuery(string strEntity)
        {
            if (String.IsNullOrEmpty(strEntity)) { strEntity = "??????"; };
            _Entity = strEntity;
            this.Refresh();
        }

        public int OFFSET_PageSize { get; set; }
        public int OFFSET_PageNum { get; set; }
        public string Entity {
            get
            {
                return _Entity;
            }
            set
            {
                _Entity = value;
                this.Refresh();
            }
        }
        public string Prefix
        {
            get
            {
                return _prefix;
            }
        }
        public string PkField
        {
            get
            {
                return _pkfield;
            }
        }
        public List<BO.TheGridColumnFilter> TheGridFilter { get; set; }

        public List<int> pids;
        public IEnumerable<BO.TheGridColumn> explicit_columns { get; set; }
        public string explicit_orderby { get; set; }

        public bool? IsRecordValid;

        public int j04id { get; set; }
        public int b02id { get; set; }
        public int p28id { get; set; }
        public int p21id { get; set; }
        public int p10id { get; set; }
        public int p26id { get; set; }
        public int j02id {get;set;}
        public int p11id { get; set; }
        public int p12id { get; set; }
        public int p41id { get; set; }
        public int p13id { get; set; }
        public int p51id { get; set; }
        public int p25id { get; set; }
        public string query_by_entity_prefix;

        public string SearchString;
        public int TopRecordsOnly;





        public void SetPids(string strPids)
        {
            this.pids = BO.BAS.ConvertString2ListInt(strPids);

        }
        
        private void Refresh()
        {
            _prefix = _Entity.Substring(0, 3);
            _pkfield = "a." + _Entity.Substring(0, 3) + "ID";
        }

      
    }
}
