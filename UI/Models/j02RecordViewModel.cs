using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class j02RecordViewModel
    {
        public BO.j02Person Rec { get; set; }
        
        public MyToolbarViewModel Toolbar { get; set; }

        public MyComboViewModel ComboJ04ID { get; set; }
        public MyComboViewModel ComboP28ID { get; set; }

        public MyAutoCompleteViewModel TitleBeforeName { get; set; }
        public MyAutoCompleteViewModel TitleAfterName { get; set; }



    }
}
