using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41CreateChildViewModel:BaseViewModel
    {
        public List<BO.AppendPostPreP44Oper> lisDestOper { get; set; }
        public int MasterID { get; set; }
        

        public BO.p41Task RecMasterP41 { get; set; }
        public int p18flag { get; set; }

       

        public int SelectedP27ID { get; set; }
        public string SelectedP27Name { get; set; }

        public string p41Name { get; set; }
       
    }
}
