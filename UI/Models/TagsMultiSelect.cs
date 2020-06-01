using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TagsMultiSelect
    {
        public string Entity { get; set; }
        public IEnumerable<BO.o51Tag> Tags { get; set; }
    }
}
