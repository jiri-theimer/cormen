using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface ICBL
    {
        public string DeleteRecord(string prefix, int pid);
        public string LoadUserParam(string strKey);
        public bool SetUserParam(string strKey, string strValue);
    }
    class CBL : ICBL
    {
        private BO.RunningUser _cUser;
        public CBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private IEnumerable<BO.COM.StringPairValue> _userparams = null;
        public string DeleteRecord(string entity,int pid)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _cUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            switch (entity)
            {
                case "":
                    
                    break;
                
                default:
                    return DL.DbHandler.RunSp(entity + "_delete", pars);                    
            }

            return "";
        }

        public string LoadUserParam(string strKey)
        {     
            if (_userparams == null)
            {
                _userparams= DL.DbHandler.GetList<BO.COM.StringPairValue>("SELECT x36Key as [Key],x36Value as [Value] FROM x36UserParam WHERE j02ID=@j02id", new { j02id = _cUser.pid });
            }

            if (_userparams.Where(p => p.Key == strKey).Count() > 0)
            {
                return _userparams.Where(p => p.Key == strKey).First().Value;
            }
            else
            {
                return "";
            }            
        }
        public bool SetUserParam(string strKey,string strValue)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _cUser.pid, System.Data.DbType.Int32);
            pars.Add("x36key", strKey, System.Data.DbType.String);
            pars.Add("x36value", strValue, System.Data.DbType.String);

            if (DL.DbHandler.RunSp("x36userparam_save", pars) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
       
        //public IEnumerable<BO.COM.StringPairValue> GetUserParams()
        //{           
        //    return DL.DbHandler.GetList<BO.COM.StringPairValue>("SELECT x36Key as [Key],x36Value as [Value] FROM x36UserParam WHERE j02ID=@j02id", new { j02id = AppRunning.Get().UserID });
        //}
    }
}
