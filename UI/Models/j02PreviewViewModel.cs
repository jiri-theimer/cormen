using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class j02PreviewViewModel:BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public BO.j03User UserProfile { get; set; }
    }
}
