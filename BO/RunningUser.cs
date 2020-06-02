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
        public string j02Email { get; set; }
        
        public string FullName { get; set; }
        public int j04PermissionValue { get;set;}
        public bool j03IsMustChangePassword { get; set; }
        public int j03FontStyleFlag { get; set; }
        public int j03EnvironmentFlag { get; set; }
        public int j03GridSelectionModeFlag { get; set; }
        public DateTime? j03LiveChatTimestamp { get; set; }
        public DateTime? j03PingTimestamp { get; set; }


        public List<BO.StringPair> Messages4Notify { get; set; }

        public void AddMessage(string strMessage,string strTemplate="error")
        {
            if (Messages4Notify == null) { Messages4Notify = new List<BO.StringPair>(); };
            Messages4Notify.Add(new BO.StringPair() { Key = strTemplate, Value = strMessage }); ;
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
        public bool TestIfClientRole()  //vrací true, pokud se jedná o uživatele s klientským oprávněním
        {
            if (TestPermission(UserPermFlag.MasterAdmin) || TestPermission(UserPermFlag.MasterReader))
            {
                return false;
            }
            return true;
        }
    }
}
