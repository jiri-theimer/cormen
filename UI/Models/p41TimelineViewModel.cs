using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41TimelineViewModel:BaseViewModel
    {
        public DateTime CurrentDate { get; set; }
        
        public List<BO.p27MszUnit> lisP27 { get; set; }
        public List<BO.p26Msz> lisP26 { get; set; }

        public IEnumerable<BO.p41Task> Tasks { get; set; }
        public List<Slot> Slots { get; set; }

        public IEnumerable<BO.p33CapacityTimeline> lisFond { get; set; }

        public UI.Models.p41TimelineQuery localQuery { get; set; }

        public int HourFrom { get; set; }
        public int HourUntil { get; set; }


        public DateTime CurrentDT1
        {
            get
            {
                return this.CurrentDate.AddHours(this.HourFrom);
            }
        }
        public DateTime CurrentDT2
        {
            get
            {
                return this.CurrentDate.AddHours(this.HourUntil).AddSeconds(-1);
            }
        }



    }


    public class Slot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string p41Code { get; set; }
        public string b02Color { get; set; }
        public string b02Name { get; set; }
        public string Title { get; set; }
        public string TitlePre { get; set; }
        public string TitlePost { get; set; }
        public string TitleClear { get; set; }
        public int p41ID { get; set; }
        public int p27ID { get; set; }
        public string CssName { get; set; }
        public int ColSpanKorekce { get; set; } = 0;
        
        public int ColStartPost { get; set; }
        public int ColEndPre { get; set; }

        public int ColStart { get; set; }

        
        
        public int ColSpan
        {
            get
            {
                int intColSpan= (this.End.Hour * 60 + this.End.Minute - this.Start.Hour * 60 - this.Start.Minute);
                if (intColSpan % 10 > 0)
                {
                    intColSpan = intColSpan / 10;
                    intColSpan = intColSpan + 1;
                }
                else
                {
                    intColSpan = intColSpan / 10;
                }
                //intColSpan = intColSpan + 1;
                //if (this.CssName == "popre")
                //{
                //    intColSpan = intColSpan - 1;
                //}

                return intColSpan+ ColSpanKorekce;
            }
        }
    }
}
