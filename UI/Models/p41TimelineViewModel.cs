using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41TimelineViewModel:BaseViewModel
    {
        public DateTime CurrentDate { get; set; }

        public int p26ID { get; set; }
        public string p26Name { get; set; }

        public List<BO.p27MszUnit> lisP27 { get; set; }

        public IEnumerable<BO.p41Task> Tasks { get; set; }

        public IEnumerable<BO.p33CapacityTimeline> lisFond { get; set; }

        public UI.Models.p41TimelineQuery localQuery { get; set; }

        public string SelectedP27IDs { get; set; }
        public string SelectedP27Names { get; set; }
    }
}
