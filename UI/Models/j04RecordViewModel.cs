using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class j04RecordViewModel:BaseViewModel
    {
        public BO.j04UserRole Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public List<BO.UserPermission> PermCatalogue { get; set; }

        public List<int> SelectedPermissions { get; set; }

        public j04RecordViewModel()
        {
            PermCatalogue = new List<BO.UserPermission>();
            PermCatalogue.Add(new BO.UserPermission() { Name = "MASTER Admin",Value=BO.UserPermFlag.MasterAdmin });
            PermCatalogue.Add(new BO.UserPermission() { Name = "MASTER Reader",Value=BO.UserPermFlag.MasterReader });
            PermCatalogue.Add(new BO.UserPermission() { Name = "CLIENT Admin", Value = BO.UserPermFlag.ClientAdmin });
            PermCatalogue.Add(new BO.UserPermission() { Name = "CLIENT Reader", Value = BO.UserPermFlag.ClientReader });
        }
    }
}
