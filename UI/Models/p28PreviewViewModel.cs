using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class p28PreviewViewModel : BaseViewModel
    {
        public BO.p28Company Rec { get; set; }

        public bool IsPossible2SetupCloudID = false;
    }
}
