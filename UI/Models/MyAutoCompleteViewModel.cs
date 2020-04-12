using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyAutoCompleteViewModel
    {
        public int o15Flag { get; set; }
        public string SelectedText { get; set; }
        public string ControlID { get; set; }
        public string ControlID_Table { get; set; }

        public string PlaceHolder { get; set; }
        public string DropDownWidth { get; set; } = "200px";

        public string ControlID_Dropdown { get; set; }

        public MyAutoCompleteViewModel(int intO15Flag, string strSelectedText, string strPlaceHolder = "", string strControlID="pop1")
        {
            this.o15Flag = intO15Flag;
            this.PlaceHolder = strPlaceHolder;
            this.SelectedText = strSelectedText;
            this.ControlID = strControlID;

            ControlID_Table = ControlID + "_table";
            ControlID_Dropdown = ControlID + "_dropdown";


        }
        public MyAutoCompleteViewModel()
        {

        }
    }
}
