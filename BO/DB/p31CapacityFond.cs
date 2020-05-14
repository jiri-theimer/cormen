using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p31CapacityFond:BaseBO
    {
        [Key]
        public int p31ID { get; set; }
        public int j02ID_Owner { get; set; }
        public string p31Name { get; set; }

    }
}
