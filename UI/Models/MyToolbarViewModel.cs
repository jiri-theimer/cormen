using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyToolbarViewModel
    {        
        public BO.BaseBO Record { get; set; }
        public string ControllorName { get; set; } = "Record";
        
        public bool IsCurrentClone { get; set; }
        public bool IsSave { get; set; }
        public bool IsRefresh { get; set; }
        public bool IsClose { get; set; } = true;
        public bool IsDelete { get; set; }
        public bool IsApply { get; set; } = false;
        public bool IsNew { get; set; }
        public bool IsClone { get; set; }
        public bool IsToArchive { get; set; }
        public bool IsFromArchive { get; set; }
        public bool AllowArchive { get; set; } = true;
        public string Message { get; set; }
        public string ArchiveFlag { get; set; }
        public string BG;

        public MyToolbarViewModel(BO.BaseBO rec)
        {                        
            Record = rec;
            RefreshState();

        }
        public MyToolbarViewModel()
        {

        }

        public void MakeClone()
        {
            Message = "Kopírovaný záznam";
            if (Record != null) Record.pid = 0;
            RefreshState();
        }

        private void RefreshState()
        {            
            if (Record == null) return;

            IsSave = true;
            BG = "bg-light";
            if (Record.pid == 0)
            {
                IsDelete = false;
                IsNew = false;
                IsClone = false;
                IsRefresh = false;
                IsToArchive = false;
                IsFromArchive = false;
            }
            else
            {
                IsDelete = true;
                IsNew = true;
                IsClone = true;
                IsRefresh = true;
                if (Record.isclosed)
                {
                    IsFromArchive = this.AllowArchive;
                    BG = "bg-dark";
                }
                else
                {
                    IsToArchive = this.AllowArchive;
                }
                
            }
        }

        public DateTime? GetValidUntil(BO.BaseBO rec)
        {
            switch (this.ArchiveFlag)
            {
                case "1":
                    return DateTime.Now;                    
                case "2":
                    return new DateTime(3000, 1, 1);                    
                default:
                    if (rec.pid == 0)
                    {
                        return new DateTime(3000, 1, 1);
                    }
                    else
                    {
                        return rec.ValidUntil;
                    }
                                        
            }            
        }
        public DateTime? GetValidFrom(BO.BaseBO rec)
        {
            if (rec.pid == 0)
            {
                return DateTime.Now.AddSeconds(-10);
            }
            else
            {
                return rec.ValidFrom;
            }
        }
    }
}
