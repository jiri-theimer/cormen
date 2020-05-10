using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p51Order:BaseBO
    {
        [Key]
        public int p51ID { get; set; }
        public int p28ID { get; set; }
        public int p26ID { get; set; }
        public int b02ID { get; set; }
        public int j02ID_Owner { get; set; }
        public string p51Name { get; set; }
        public bool p51IsDraft { get; set; }
        public string p51Code { get; set; }
        public string p51CodeByClient { get; set; }
        public DateTime p51Date { get; set; }
        public DateTime p51DateDelivery { get; set; }
        public string p51Memo { get; set; }

        public string RecordOwner;
        public string p28Name { get; set; } //get+set: kvůli mycombo
        public string p26Name { get; set; } //get+set: kvůli mycombo
        public string b02Name { get; set; } //get+set: kvůli mycombo
    }
}
