﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class ThePeriodProvider
    {
        private List<BO.ThePeriod> _lis;

        public ThePeriodProvider()
        {

            _lis = new List<BO.ThePeriod>();
            SetupPallete();
        }

        private void SetupPallete()
        {
            _lis.Add(new BO.ThePeriod() { pid = 0, PeriodName = "--Pojmenované období--", PeriodInterval = "" });
            AF(10);
            AF(11);
            
            AF(20);
            AF(21);
            
            AF(30);
            AF(31);
            AF(33); AF(34); AF(35); AF(36);
            AF(40); AF(41);
            AF(50); AF(51);

            //budoucnost:
            AF(12);
            AF(22);
            AF(32);
            AF(42);
            AF(52);
        }

        public List<BO.ThePeriod> getPallete()
        {
            if (_lis[1].d1 != DateTime.Today)
            {
                _lis.Clear();
                SetupPallete();
            }
            return _lis;
        }
         
        
        private void AF(int pid)
        {
            DateTime d1 = DateTime.Now; DateTime d2 = DateTime.Now; string strName = ""; string strInterval = null;

            switch (pid)
            {
                case 10:
                    strName = "Dnes"; d1 = DateTime.Today; d2 = DateTime.Today;
                    break;
                case 11:
                    strName = "Včera"; d1 = DateTime.Today.AddDays(-1); d2 = DateTime.Today.AddDays(-1);
                    break;
                case 12:
                    strName = "Zítra"; d1 = DateTime.Today.AddDays(1); d2 = DateTime.Today.AddDays(1);
                    break;
                case 20:
                    strName = "Tento týden";
                    d1 = FirstDateInWeek(DateTime.Today);
                    d2 = d1.AddDays(6);
                    break;
                case 21:
                    strName = "Minulý týden";
                    d1 = FirstDateInWeek(DateTime.Today.AddDays(-7));
                    d2 = d1.AddDays(6);
                    break;
                case 22:
                    strName = "Příští týden";
                    d1 = FirstDateInWeek(DateTime.Today.AddDays(7));
                    d2 = d1.AddDays(6);
                    break;
                case 30:
                    strName = "Tento měsíc";
                    d1 = new DateTime(d1.Year, d1.Month, 1);
                    d2 = d1.AddMonths(1).AddDays(-1);
                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 31:
                    strName = "Minulý měsíc";
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(-1);
                    d2 = d1.AddMonths(1).AddDays(-1);
                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 32:
                    strName = "Příští měsíc";
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(1);
                    d2 = d1.AddMonths(1).AddDays(-1);

                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 33:
                    strName = "Měsíc -2";
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(-2);
                    d2 = d1.AddMonths(1).AddDays(-1);

                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 34:
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(-3);
                    d2 = d1.AddMonths(1).AddDays(-1);
                    strName = "Měsíc -3";
                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 35:
                    strName = "Měsíc -4";
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(-4);
                    d2 = d1.AddMonths(1).AddDays(-1);

                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 36:
                    strName = "Měsíc -5";
                    d1 = new DateTime(d1.Year, d1.Month, 1).AddMonths(-5);
                    d2 = d1.AddMonths(1).AddDays(-1);

                    strInterval = d1.Month.ToString() + "/" + d1.Year.ToString();
                    break;
                case 40:
                    strName = "Tento kvartál";
                    int x = (int)System.Math.Ceiling((decimal)d1.Month / 3);
                    d1 = new DateTime(d1.Year, (3 * x) - 2, 1);
                    d2 = d1.AddMonths(3).AddDays(-1);
                    break;
                case 41:
                    strName = "Minulý kvartál";
                    int xx = (int)System.Math.Ceiling((decimal)d1.Month / 3);
                    d1 = new DateTime(d1.Year, (3 * xx) - 2, 1);
                    d1 = d1.AddMonths(-3);
                    d2 = d1.AddMonths(3).AddDays(-1);
                    break;
                case 42:
                    strName = "Příští kvartál";
                    int xxx = (int)System.Math.Ceiling((decimal)d1.Month / 3);
                    d1 = new DateTime(d1.Year, (3 * xxx) - 2, 1);
                    d1 = d1.AddMonths(3);
                    d2 = d1.AddMonths(3).AddDays(-1);
                    break;
                case 50:
                    strName = "Tento rok";
                    d1 = new DateTime(d1.Year, 1, 1);
                    d2 = new DateTime(d1.Year, 12, 31);
                    strInterval = d1.Year.ToString();
                    break;
                case 51:
                    strName = "Minulý rok";
                    d1 = new DateTime(d1.Year - 1, 1, 1);
                    d2 = new DateTime(d1.Year - 1, 12, 31);
                    strInterval = d1.Year.ToString();
                    break;
                case 52:
                    strName = "Příští rok";
                    d1 = new DateTime(d1.Year + 1, 1, 1);
                    d2 = new DateTime(d1.Year + 1, 12, 31);
                    strInterval = d1.Year.ToString();
                    break;
            }

            if (strInterval == null)
            {
                if (d1 == d2)
                {
                    strInterval = BO.BAS.ObjectDate2String(d1);
                }
                else
                {
                    if (d1.Month==d2.Month && d1.Year == d2.Year)
                    {
                        strInterval = BO.BAS.ObjectDate2String(d1, "dd.") + " - "+BO.BAS.ObjectDate2String(d2);
                    }
                    else
                    {
                        if (d1.Year == d2.Year)
                        {
                            strInterval = BO.BAS.ObjectDate2String(d1, "dd.MM.") + " - " + BO.BAS.ObjectDate2String(d2);
                        }
                        else
                        {
                            strInterval = BO.BAS.ObjectDate2String(d1) + " - " + BO.BAS.ObjectDate2String(d2);
                        }
                    }
                    
                }
            }


            _lis.Add(new BO.ThePeriod() {pid=pid, PeriodName = strName, PeriodInterval = strInterval, d1 = d1, d2 = d2 });


        }


        private DateTime FirstDateInWeek(DateTime dt)
        {
            while (dt.DayOfWeek != System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                dt = dt.AddDays(-1);
            return dt;
        }
    }
}
