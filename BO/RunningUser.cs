using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class RunningUser
    {
        public int pid { get; set; }
        public bool isclosed { get; set; }
        public string j02Login { get; set; }
        public string FullName { get; set; }
        public int j04PermissionValue { get;set;}
        public bool j02IsMustChangePassword { get; set; }

      
        public List<BO.COM.StringPairValue> Messages4Notify { get; set; }

        public void AddMessage(string strMessage,string strTemplate="error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.COM.StringPairValue>(); };
            Messages4Notify.Add(new BO.COM.StringPairValue() { Key = strTemplate, Value = strMessage }); ;
        }
    }
}
