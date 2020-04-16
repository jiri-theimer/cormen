using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class o23PreviewViewModel:BaseViewModel
    {
        public BO.o23Doc Rec { get; set; }

        public IEnumerable<BO.o27Attachment> lisO27;
    }
}
