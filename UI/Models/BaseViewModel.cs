using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class BaseViewModel
    {
        public int Jednicka;
        //public List<BO.COM.StringPairValue> NotifyMessages { get; set; }
        //public void Notify(string strContent,string strTemplate="error")
        //{
        //    if (NotifyMessages == null) NotifyMessages = new List<BO.COM.StringPairValue>();
        //    NotifyMessages.Add(new BO.COM.StringPairValue() { Key = strTemplate, Value = strContent });

        //}
        //public void Notify(List<BO.COM.StringPairValue> lis)
        //{
        //    if (lis == null) return;
        //    foreach(var c in lis)
        //    {
        //        Notify(c.Value, c.Key);
        //    }           

        //}
    }
}
