using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery
    {
        private string _prefix;
        private string _pkfield;
        public myQuery(string strEntity)
        {
            this.Entity = strEntity;
        }

        public string Entity {
            get
            {
                return Entity;
            }
            set
            {
                Entity = value;
                _prefix= value.Substring(0, 3);
                _pkfield="a." + value.Substring(0, 3) + "ID";
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

        public List<int> pids;
        public string explicit_selectfields { get; set; }
        public string explicit_orderby { get; set; }

        public bool? IsRecordValid;

        public int j04id { get; set; }
        public int b02id { get; set; }
        public int p28id { get; set; }
        public int p21id { get; set; }
        public int p10id { get; set; }
        public int p13id { get; set; }
        public string query_by_entity_prefix;

        public string SearchString;

        
       
        
        public void SetPids(string strPids)
        {
            this.pids = BO.BAS.ConvertString2ListInt(strPids);

        }
        

      
    }
}
