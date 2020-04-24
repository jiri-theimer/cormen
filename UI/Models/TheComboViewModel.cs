using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheComboViewModel
    {
        public string Entity { get; set; }
        public string ControlID { get; set; }
        public string SelectedValue { get; set; }
        public string SelectedText { get; set; }
        public string CallerIDValue { get; set; }
        public string CallerIDText { get; set; }
        public string PlaceHolder { get; set; }
        public string Param1 { get; set; }

        public string getPrefix()
        {
            return this.Entity.Substring(0, 3);
        }
        public string getNameField()
        {
            return this.Entity.Substring(0, 3) + "Name";
        }
    }
}
