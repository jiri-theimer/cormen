﻿using System;
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
        public string EstimateRecordCode(string entity);
    }
    class CBL :BaseBL, ICBL
    {

        public CBL(BL.Factory mother):base(mother)
        {
           
        }
      
        private IEnumerable<BO.COM.StringPairValue> _userparams = null;
        public string DeleteRecord(string entity,int pid)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            switch (entity)
            {
                case "":
                    
                    break;
                
                default:
                    return _db.RunSp(entity + "_delete", ref pars);                    
            }

            return "";
        }
        public string EstimateRecordCode(string entity)
        {
            BO.COM.GetString c = _db.Load<BO.COM.GetString>("select dbo.getRecordCode(@ent,@j03id) as Value",new { ent = entity,j03id=_mother.CurrentUser.pid });
            return c.Value;
        }

        public string LoadUserParam(string strKey)
        {     
            if (_userparams == null)
            {
                _userparams= _db.GetList<BO.COM.StringPairValue>("SELECT x36Key as [Key],x36Value as [Value] FROM x36UserParam WHERE j03ID=@j03id", new { j03id = _db.CurrentUser.pid });
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
            pars.Add("userid", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("x36key", strKey, System.Data.DbType.String);
            pars.Add("x36value", strValue, System.Data.DbType.String);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if (_db.RunSp("x36userparam_save", ref pars) == "1")
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
