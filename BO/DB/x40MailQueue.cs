using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x40MailQueue : BaseBO
    {
        [Key]
        public int x40ID { get; set; }
        public int j03ID { get; set; }
        public int j40ID { get; set; }
        public int x40State { get; set; }
        public int x40RecordPid { get; set; }
        public string x40SenderAddress { get; set; }
        public string x40SenderName { get; set; }
        public string x40To { get; set; }
        public string x40Bcc { get; set; }
        public string x40Cc { get; set; }
        public string x40Attachments { get; set; }
        public string x40Subject { get; set; }
        public string x40Body {get;set;}
        public bool x40IsHtmlBody { get; set; }
        public DateTime? x40WhenProceeded { get; set; }
        public string x40ErrorMessage { get; set; }

        public string j40Name { get; set; }
    }
}
