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
        public IEnumerable<BO.j73TheGridQuery> lisJ73 { get; set; }
        public List<BO.ThePeriod> lisPeriods { get; set; }

        public List<int> pids;
        public IEnumerable<BO.TheGridColumn> explicit_columns { get; set; }
        public string explicit_orderby { get; set; }
        public string explicit_selectsql { get; set; }

        public bool? IsRecordValid;

        public int j04id { get; set; }
        public int j72id { get; set; }
        public int b02id { get; set; }
        public List<int> b02ids { get; set; }
        public List<int> p27ids { get; set; }
        public int p27id { get; set; }
        public List<int> p26ids { get; set; }
        public int p28id { get; set; }
        public int p21id { get; set; }
        public int p21id_missing { get; set; }
        public int p10id { get; set; }
        public int p19id { get; set; }
        public int p26id { get; set; }
        public int j02id {get;set;}
        public int p11id { get; set; }
        public int p12id { get; set; }
        public int p41id { get; set; }
        public int p13id { get; set; }
        public int p51id { get; set; }
        public int p52id { get; set; }
        public int p25id { get; set; }
        public List<int> p25ids { get; set; }
        public int p18flag { get; set; }
        public List<int> p18flags { get; set; }
        public int o53id { get; set; }
        public string query_by_entity_prefix;

        public string SearchString;
        public int TopRecordsOnly;


        public DateTime? DateBetween { get; set; }
        public int DateBetweenDays { get; set; }


        public void SetPids(string strPids)
        {
            this.pids = BO.BAS.ConvertString2ListInt(strPids);

        }
        
        private void Refresh()
        {
            _prefix = _Entity.Substring(0, 3);
            _pkfield = "a." + _Entity.Substring(0, 3) + "ID";
        }

        public void InhaleMasterEntityQuery(string master_entity, int master_pid)
        {
            if (master_pid == 0 || master_entity==null)
            {
                return;
            }
            switch (master_entity.Substring(0, 3))
            {
                case "p28":
                    this.p28id = master_pid;
                    break;
                case "p10":
                    this.p10id = master_pid;
                    break;
                case "p13":
                    this.p13id = master_pid;
                    break;
                case "p19":
                    this.p19id = master_pid;
                    break;
                case "p21":
                    this.p21id = master_pid;
                    break;
                case "21p":
                    this.p21id_missing = master_pid;    //pro zobrazení produktů, které ještě nejsou v licenci
                    break;
                case "p26":
                    this.p26id = master_pid;
                    break;
                case "j02":
                    this.j02id = master_pid;
                    break;
                case "p11":
                    this.p11id = master_pid;
                    break;
                case "p12":
                    this.p12id = master_pid;
                    break;
                case "p25":
                    this.p25id = master_pid;
                    break;
                case "p41":
                    this.p41id = master_pid;
                    break;               
                case "p51":
                    this.p51id = master_pid;
                    break;                
                case "o53":
                    this.o53id = master_pid;
                    break;               
                default:
                    break;
            }

        }
    }
}
