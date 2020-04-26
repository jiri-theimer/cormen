using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyComboViewModel
    {
        public string Entity { get; set; }
        public string ControlID { get; set; }
        
        public int SelectedValue { get; set; }
        public string SelectedText { get; set; }
        
        public string PlaceHolder { get; set; }
        public string Param1 { get; set; }

        public int ViewFlag { get; set; } = 0;

        public string Event_After_ChangeValue { get; set; }

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
