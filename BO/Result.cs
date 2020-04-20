using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum ResultEnum
    {
        Failed,
        Success,
        InfoOnly
    }
    public class Result
    {
        private string _message;

        public ResultEnum Flag { get; set; }
        public string Message { get {
                if (this.PreMessage == null)
                {
                    return _message;
                }
                else
                {
                    return this.PreMessage + ": " + _message;
                }
                
            }
            set { Message = _message; }
        }
        public string PreMessage { get; set; }
        public Result(bool bolError, string strMessage= null)
        {
            if (bolError)
            {
                this.Flag = ResultEnum.Failed;
            }
            else
            {
                this.Flag = ResultEnum.Success;
            }
            _message = strMessage;
        }
        
    }
}
