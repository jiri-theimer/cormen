using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class MenuItem
    {
        public string Name { get; set; }        
        private string _Url;        
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;

                
            }
        }

        

    }
}
