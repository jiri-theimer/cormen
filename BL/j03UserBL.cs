using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij03UserBL
    {
        public BO.j03User Load(int pid);
        public BO.j03User LoadByLogin(string strLogin);
        
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq);
        public int Save(BO.j03User rec);
        public void UpdateCurrentUserPing(BO.j92PingLog c);

    }
    class j03UserBL : BaseBL, Ij03UserBL
    {
        public j03UserBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("j03") + ",j04.j04Name,p28.p28Name,j02.j02LastName+' '+j02.j02FirstName+ISNULL(' '+j02.j02TitleBeforeName,' ') as fullname_desc,j02.j02Email FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON j02.p28ID=p28.p28ID";
        }
        public BO.j03User Load(int intPID)
        {
            return _db.Load<BO.j03User>(GetSQL1() + " WHERE a.j03ID=@pid", new { pid = intPID });
        }
       
        public BO.j03User LoadByLogin(string strLogin)
        {
            return _db.Load<BO.j03User>(GetSQL1() + " WHERE a.j03Login LIKE @login", new { login = strLogin });
        }
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j03User>(fq.FinalSql, fq.Parameters);
        }
        

        public int Save(BO.j03User rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j03ID);
            p.AddInt("j02ID", rec.j02ID,true);
            p.AddInt("j04ID", rec.j04ID,true);
            p.AddBool("j03IsMustChangePassword", rec.j03IsMustChangePassword);            
            p.AddString("j03Login", rec.j03Login);         
            p.AddInt("j03AccessFailedCount", rec.j03AccessFailedCount);
            p.AddInt("j03ModalDialogFlag", rec.j03ModalDialogFlag);
            p.AddInt("j03FontStyleFlag", rec.j03FontStyleFlag);
            p.AddInt("j03SideBarFlag", rec.j03SideBarFlag);
            p.AddInt("j03EnvironmentFlag", rec.j03EnvironmentFlag);
            p.AddInt("j03GridSelectionModeFlag", rec.j03GridSelectionModeFlag);
            p.AddDateTime("j03LiveChatTimestamp", rec.j03LiveChatTimestamp);
            if (!String.IsNullOrEmpty(rec.j03PasswordHash))
            {
                p.Add("j03PasswordHash", rec.j03PasswordHash);
            }


            return _db.SaveRecord("j03User", p.getDynamicDapperPars(), rec);
        }


        public void UpdateCurrentUserPing(BO.j92PingLog c) //zápis pravidelně po 2 minutách do PING logu
        {
            _db.RunSql("UPDATE j03User set j03PingTimestamp=GETDATE() WHERE j03ID=@pid", new { pid = _mother.CurrentUser.pid });    //ping aktualizace

            string s = "INSERT INTO j92PingLog(j03ID,j92Date,j92BrowserUserAgent,j92BrowserFamily,j92BrowserOS,j92BrowserDeviceType,j92BrowserDeviceFamily,j92BrowserAvailWidth,j92BrowserAvailHeight,j92BrowserInnerWidth,j92BrowserInnerHeight,j92RequestUrl)";
            s += " VALUES(@j03id,GETDATE(),@useragent,@browser,@os,@devicetype,@devicefamily,@aw,@ah,@iw,@ih,@requesturl)";
            _db.RunSql(s, new { j03id = _mother.CurrentUser.pid, useragent = c.j92BrowserUserAgent, browser = c.j92BrowserFamily, os = c.j92BrowserOS, devicetype = c.j92BrowserDeviceType, devicefamily = c.j92BrowserDeviceFamily, aw = c.j92BrowserAvailWidth, ah = c.j92BrowserAvailHeight, iw = c.j92BrowserInnerWidth, ih = c.j92BrowserInnerHeight, requesturl=c.j92RequestURL });


            if (_mother.CurrentUser.j03LiveChatTimestamp != null)   //hlídat, aby se automaticky vypnul live-chat box po 20ti minutách
            {
                if (_mother.CurrentUser.j03LiveChatTimestamp.Value.AddMinutes(20) < DateTime.Now)
                {                    
                    var rec = Load(_mother.CurrentUser.pid);
                    rec.j03LiveChatTimestamp = null;   //vypnout smartsupp
                    Save(rec);
                }
            }

        }
    }
}
