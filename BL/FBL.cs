using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface IFBL
    {
        public IEnumerable<BO.COM.GetString> GetListAutoComplete(int o15flag);
    }
    class FBL:IFBL
    {
        private DL.DbHandler _db;
        public FBL(DL.DbHandler db)
        {
            _db = db;
        }
        
        public IEnumerable<BO.COM.GetString> GetListAutoComplete(int intO15Flag)
        {
            return _db.GetList<BO.COM.GetString>("SELECT o15Value as Value FROM o15AutoComplete WHERE o15Flag=@flag ORDER BY o15Ordinary,o15Value", new { flag = intO15Flag });
        }
    }
}
