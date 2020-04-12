using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public abstract class BaseBO
    {
        public int pid { get; set; }
        public bool isclosed;
        public string entity;
        public string UserInsert { get; set; }
        public DateTime DateInsert { get; set; }

        public string UserUpdate { get; set; }
        public DateTime DateUpdate { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }



    }
}
