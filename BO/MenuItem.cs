using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class MenuItem
    {
        public string Name { get; set; }
        private string _Url;
        private string _Html;
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

        public string Html
        {
            get
            {
                if (String.IsNullOrEmpty(this.Name) == true)
                {
                    return "<li><hr></li>";  //divider
                }
                if (String.IsNullOrEmpty(_Url) == true)
                {
                    return string.Format("<li>{0}</li>", this.Name);
                }
                return string.Format("<li><a href=\"{0}\">{1}</a></li>",_Url, this.Name);
            }
        }

    }
}
