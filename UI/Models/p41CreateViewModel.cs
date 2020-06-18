using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41CreateViewModel:BaseViewModel
    {
        public int p51ID { get; set; }
        public string p51Code { get; set; }
        public int p52ID { get; set; }
        public string p52Code { get; set; }

        public int p27ID { get; set; }
        public string p27Name { get; set; }
        //public BO.p26Msz RecP26 { get; set; }

        public DateTime? Date0 { get; set; }

        public List<BO.p27MszUnit> lisP27 { get; set; }


        public BO.p52OrderItem RecP52 { get; set; }
        public BO.p51Order RecP51 { get; set; }

        public List<BO.p41Task> Tasks { get; set; }

        public int DeleteIndex { get; set; } = -1;

        public int p25ID { get; set; }  //typ zařízení pro combo středisek
    }
}
