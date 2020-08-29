using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FileUploadSingleViewModel:BaseViewModel
    {
        public string Guid { get; set; }

        public string OrigFileName { get; set; }
        public string TempFileName { get; set; }
    }
}
