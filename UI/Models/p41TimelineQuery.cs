using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41TimelineQuery
    {
        public bool IsPoPre { get; set; } = true;
        public bool IsPoPost { get; set; } = true;
        public bool IsTo { get; set; } = true;

        public string SelectedP27IDs { get; set; }
        public string SelectedP27Names { get; set; }
        public string SelectedP26IDs { get; set; }
        public string SelectedP26Names { get; set; }
        public string SelectedB02IDs { get; set; }
        public string SelectedB02Names { get; set; }
        public int ColStart { get; set; }
        public int ColSpan { get; set; }
    }
}
