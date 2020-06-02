using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TagsMultiSelect:BaseViewModel
    {
        public string Entity { get; set; }
        //public IEnumerable<BO.o51Tag> SelectedTags { get; set; }

        public IEnumerable<int> SelectedO51IDs { get; set; }
        public IEnumerable<BO.o51Tag> ApplicableTags { get; set; }

        
    }
}
