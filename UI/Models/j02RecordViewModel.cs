using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class j02RecordViewModel : BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public bool IsUserProfile { get; set; }
        public BO.j03User UserProfile {get;set;}
        
        public MyToolbarViewModel Toolbar { get; set; }


        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        public string ResetPassword { get; set; }



    }
}
