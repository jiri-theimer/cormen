using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface IFBL
    {
        public IEnumerable<BO.COM.GetString> GetListAutoComplete(int o15flag);
    }
    class FBL:BaseBL,IFBL
    {       
        public FBL(BL.Factory mother):base(mother)
        {               
            
        }

        public IEnumerable<BO.COM.GetString> GetListAutoComplete(int intO15Flag)
        {
            return _db.GetList<BO.COM.GetString>("SELECT o15Value as Value FROM o15AutoComplete WHERE o15Flag=@flag ORDER BY o15Ordinary,o15Value", new { flag = intO15Flag });
        }
    }
}
