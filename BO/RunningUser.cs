using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class RunningUser
    {
        public int pid { get; set; }
        public string j02Login { get; set; }
        public string FullName { get; set; }
        public int j04PermissionValue { get;set;}
        public bool j02IsMustChangePassword { get; set; }

        public string ErrorMessage { get; set; }
    }
}
