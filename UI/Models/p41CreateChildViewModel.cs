using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41CreateChildViewModel:BaseViewModel
    {
        public IEnumerable<BO.p18OperCode> lisP18 { get; set; }
        public int p25ID { get; set; }  //typ zařízení pro combo středisek
        public int MasterID { get; set; }
        public List<int> SelectedP18IDs { get; set; }

        public BO.p41Task RecMasterP41 { get; set; }
        public int p18flag { get; set; }

       

        public int SelectedP27ID { get; set; }
        public string SelectedP27Name { get; set; }

        public string p41Name { get; set; }
       
    }
}
