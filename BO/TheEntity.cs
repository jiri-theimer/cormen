using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheEntity
    {
        private string _Prefix;
        private string _TableName;        
        public string AliasSingular { get; set; }
        public string AliasPlural { get; set; }        
        public string SqlFrom { get; set; }
        public string SqlOrderBy { get; set; }

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                _TableName = value;

                _Prefix = _TableName.Substring(0, 3);
            }
        }
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
        }

      
    }
}
