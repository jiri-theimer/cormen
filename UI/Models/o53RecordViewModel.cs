using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class o53RecordViewModel:BaseViewModel
    {
        public BO.o53TagGroup Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }


        public bool IsAllEntities { get; set; }
        public List<BO.TheEntity> ApplicableEntities { get; set; }
        public List<int> SelectedEntities { get; set; }

    }
}
