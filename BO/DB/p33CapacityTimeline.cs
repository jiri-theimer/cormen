using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class p33CapacityTimeline: BaseBO
    {
        [Key]
        public int p33ID { get; set; }
        public int p31ID { get; set; }
        public int p33Day { get; set; }
        public int p33Hour { get; set; }
        public int p33Minute { get; set; }
       
        public DateTime p33Date { get; set; }
        public DateTime p33DateTime { get; set; }
        public DateTime p33DateTimeUntil {
            get {
                return this.p33DateTime.AddMinutes(30).AddSeconds(-1);
            } 
        }
        public int p33MinuteUntil
        {
            get
            {
                return this.p33Minute + 30;
            }
        }
    }
}
