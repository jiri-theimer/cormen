using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class b02BatchUpdateViewModel: BaseViewModel
    {
        public string pids { get; set; }
        public string prefix { get; set; }
        public string Entity { get; set; }
        public int b02ID { get; set; }
        public string b02Name { get; set; }
    }
}
