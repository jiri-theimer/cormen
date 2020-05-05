using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class RunningUser
    {
        public int pid { get; set; }
        public int j02ID { get; set; }
        public int p28ID { get; set; }
        public string p28Name { get; set; }
        public bool isclosed { get; set; }
        public string j03Login { get; set; }
        
        public string FullName { get; set; }
        public int j04PermissionValue { get;set;}
        public bool j03IsMustChangePassword { get; set; }
        public int j03FontStyleFlag { get; set; }
        public int j03EnvironmentFlag { get; set; }


        public List<BO.COM.StringPairValue> Messages4Notify { get; set; }

        public void AddMessage(string strMessage,string strTemplate="error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.COM.StringPairValue>(); };
            Messages4Notify.Add(new BO.COM.StringPairValue() { Key = strTemplate, Value = strMessage }); ;
        }

        public string getFontStyle()
        {
            switch (this.j03FontStyleFlag)
            {
                case 1:
                    return "font: 0.6875rem/1.0 var(--font-family-sans-serif)";
                case 2:
                    return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
                case 3:
                    return "font: 0.85rem/1.0 var(--font-family-sans-serif)";
                case 4:
                    return "font: 1rem/1.2 var(--font-family-sans-serif);";
                default:
                    return "font: 0.75rem/1.0 var(--font-family-sans-serif)";
            }
        }

        public bool TestPermission(BO.UserPermFlag oneperm)
        {
            int x = (int)oneperm;
            int y = x & this.j04PermissionValue;

            if (y == x)
            {
                return true;
            }
            else
            {
                return false;
            }
            
           
        }
    }
}
