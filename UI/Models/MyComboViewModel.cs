using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyComboViewModel
    {
        public string SelectedValue { get; set; }
        public string SelectedText { get; set; }
        public string Entity { get; set; }
        public string ControlID { get; set; } = "cbx1";
        public string TableWidth { get; set; } = "100%;";
        public string Param1 { get; set; }
        public string OnChange_Event { get; set; }

        public string ControlID_Table { get; set; }
        
        public string ControlID_Dropdown { get; set; }
        
        
        
        public MyComboViewModel(string strEntity,string strSelectedValue,string strSelectedText,string strControlID)
        {
            this.Entity = strEntity;
            this.SelectedValue = strSelectedValue;            
            this.SelectedText = strSelectedText;
            this.ControlID = strControlID;

            ControlID_Table = ControlID + "_table";            
            ControlID_Dropdown = ControlID + "_dropdown";
            
            
        }
        public MyComboViewModel()
        {
           
        }

    }
}
