using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{

    public enum x40StateFlag
    {
        NotSpecified = 0,
        InQueque = 1,
        Error = 2,
        Proceeded = 3,
        Stopped = 4,
        WaitOnConfirm = 5
    }
    public class x40MailQueue : BaseBO
    {
        [Key]
        public int x40ID { get; set; }
        public int j03ID { get; set; }
        public int j40ID { get; set; }
        public string x40MessageGuid { get; set; }
        public string x40EmlFolder { get; set; }
        public int x40EmlFileSize { get; set; }
        public x40StateFlag x40State { get; set; }
        public int x40RecordPid { get; set; }
        public string x40Entity { get; set; }
        public string x40SenderAddress { get; set; }
        public string x40SenderName { get; set; }
        public string x40To { get; set; }
        public string x40Bcc { get; set; }
        public string x40Cc { get; set; }
        public string x40Attachments { get; set; }
        public string x40Subject { get; set; }
        public string x40Body { get; set; }
        public bool x40IsHtmlBody { get; set; }
        public DateTime? x40WhenProceeded { get; set; }
        public string x40ErrorMessage { get; set; }

        public string j40Name { get; set; }

        public string StateAlias
        {
            get
            {

                if (this.x40State == x40StateFlag.Error) return "Chyba";
                if (this.x40State == x40StateFlag.Proceeded) return "Odesláno";

                return this.x40State.ToString();
            }
        }
    }
}
