using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TagsBatch:BaseViewModel
    {
        public int j72ID { get; set; }
        public string Record_Entity { get; set; }
        public string Record_Pids { get; set; }

        public IEnumerable<int> SelectedO51IDs { get; set; }
        public IEnumerable<BO.o51Tag> ApplicableTags { get; set; }
    }
}
