using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j03User:BaseBO
    {
        [Key]
        public int j03ID { get; set; }        
        public int j02ID { get; set; }
        
        public string j03Login { get; set; }


        public bool j03IsMustChangePassword { get; set; }
        public int j04ID { get; set; }
        public string j03PasswordHash { get; set; }
        public int j03AccessFailedCount { get; set; }
        public int j03ModalDialogFlag { get; set; }
        public int j03FontStyleFlag { get; set; }
        public int j03SideBarFlag { get; set; }


        //readonly:
        public string fullname_desc;
        public string j02Email;
        public string p28Name;
        public string j04Name;
        
    }
}
