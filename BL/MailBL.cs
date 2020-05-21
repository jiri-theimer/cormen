using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;
using BO;

namespace BL
{
    public interface IMailBL
    {
        public BO.Result SendMessage(int j40id, MailMessage message); //v Result.pid vrací x40id
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml); //v Result.pid vrací x40id
        public void AddAttachment(string fullpath);
        public void AddAttachment(Attachment att);
        public BO.j40MailAccount LoadJ40(int pid);
        public BO.j40MailAccount LoadDefaultJ40();
        public IEnumerable<BO.j40MailAccount> GetListJ40();
        public int SaveJ40(BO.j40MailAccount rec);


    }
    class MailBL : BaseBL, IMailBL
    {
        private BO.j40MailAccount _account;
        public MailBL(BL.Factory mother) : base(mother)
        {

        }
        private List<Attachment> _attachments;

        private string GetSQL1()
        {
            return "SELECT a.*,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner," + _db.GetSQL1_Ocas("j40") + " FROM " + BL.TheEntities.ByPrefix("j40").SqlFrom;
        }

        public BO.j40MailAccount LoadJ40(int pid)
        {
            return _db.Load<BO.j40MailAccount>(string.Format("{0} WHERE a.j40ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.j40MailAccount LoadDefaultJ40()
        {
            return _db.Load<BO.j40MailAccount>(string.Format("{0} WHERE (a.j02ID_Owner=@j02id AND a.j40UsageFlag=1) OR a.j40UsageFlag=2 ORDER BY a.j40UsageFlag", GetSQL1()), new { j02id = _mother.CurrentUser.j02ID });
        }
        public IEnumerable<BO.j40MailAccount> GetListJ40()
        {

            return _db.GetList<BO.j40MailAccount>(GetSQL1());
        }
        public int SaveJ40(BO.j40MailAccount rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j40ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("j40UsageFlag", (int)rec.j40UsageFlag);
            p.AddString("j40SmtpHost", rec.j40SmtpHost);
            p.AddInt("j40SmtpPort", rec.j40SmtpPort);
            p.AddString("j40SmtpName", rec.j40SmtpName);
            p.AddString("j40SmtpEmail", rec.j40SmtpEmail);
            p.AddString("j40SmtpLogin", rec.j40SmtpLogin);
            p.AddString("j40SmtpPassword", rec.j40SmtpPassword);
            p.AddBool("j40SmtpUseDefaultCredentials", rec.j40SmtpUseDefaultCredentials);
            p.AddBool("j40SmtpEnableSsl", rec.j40SmtpEnableSsl);
            p.AddBool("j40SmtpUsePersonalReply", rec.j40SmtpUsePersonalReply);
            p.AddString("j40ImapHost", rec.j40ImapHost);
            p.AddString("j40ImapLogin", rec.j40ImapLogin);
            p.AddString("j40ImapPassword", rec.j40ImapPassword);
            p.AddInt("j40ImapPort", rec.j40ImapPort);

            return _db.SaveRecord("j40MailAccount", p.getDynamicDapperPars(), rec);
        }

        public void AddAttachment(string fullpath)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            _attachments.Add(new Attachment(fullpath));
        }
        public void AddAttachment(Attachment att)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            _attachments.Add(att);
        }
        private BO.x40MailQueue InhaleMessageRecord(int j40id,int x40id)
        {
            if (j40id > 0)
            {
                _account = LoadJ40(j40id);
            }
            else
            {
                _account = LoadDefaultJ40();                
            }
            if (_account == null)
            {
                return new BO.x40MailQueue() { j40ID = 0 };
            }
            
            BO.x40MailQueue rec = new BO.x40MailQueue() {j40ID=_account.pid, x40SenderAddress = _account.j40SmtpEmail, x40SenderName = _account.j40SmtpName };

            if (_account.j40SmtpUsePersonalReply)
            {
                rec.x40SenderAddress = _mother.CurrentUser.j02Email;
                rec.x40SenderName = _mother.CurrentUser.FullName;
            }
            
            return rec;
        }
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml)  //v BO.Result.pid vrací x40id
        {            
            MailMessage m = new MailMessage() { Body = body, Subject = subject, IsBodyHtml = ishtml };
            BO.x40MailQueue rec = InhaleMessageRecord(j40id,0);

            m.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);

            if (string.IsNullOrEmpty(toName) == false || string.IsNullOrEmpty(toEmail) == false)
            {
                if (string.IsNullOrEmpty(toName) == false)
                {
                    m.To.Add(new MailAddress(toEmail, toName));
                }
                else
                {
                    m.To.Add(new MailAddress(toEmail));
                }
            }
            

            return handle_smtp_finish( m,rec);
        }
        public BO.Result SendMessage(int j40id, MailMessage message)
        {
            BO.x40MailQueue rec = InhaleMessageRecord(j40id, 0);
            if (message.From == null)
            {
                message.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);
            }
            return handle_smtp_finish(message,rec);
        }


        private BO.Result handle_smtp_finish(MailMessage m,BO.x40MailQueue rec)
        {
            if (_account == null)
            {
                return handle_result_error("Chybí poštovní účet odesílatele");
            }
            if (m.From == null)
            {
                return handle_result_error( "Chybí odesílatel zprávy");
            }
            if (m.To.Count == 0)
            {
                return handle_result_error("Chybí příjemce zprávy");
            }
            if (string.IsNullOrEmpty(m.Body) == true)
            {
                return handle_result_error("Chybí text zprávy.");
            }
            if (string.IsNullOrEmpty(m.Subject) == true)
            {
                return handle_result_error("Chybí předmět zprávy.");
            }

            if (_account.j40SmtpUsePersonalReply)
            {
                m.ReplyToList.Add(new MailAddress(_mother.CurrentUser.j02Email, _mother.CurrentUser.FullName));
            }

            BO.Result ret = new BO.Result(false);

            using (SmtpClient client = new SmtpClient(_account.j40SmtpHost, _account.j40SmtpPort))
            {
                client.UseDefaultCredentials = _account.j40SmtpUseDefaultCredentials;
                if (client.UseDefaultCredentials == false)
                {                    
                    client.Credentials = new System.Net.NetworkCredential(_account.j40SmtpLogin, _account.j40SmtpPassword);
                }


                m.BodyEncoding = Encoding.UTF8;
                m.SubjectEncoding = Encoding.UTF8;


                if (_attachments != null)
                {
                    foreach (var att in _attachments)
                    {
                        m.Attachments.Add(att);
                    }
                }

                
                

                try
                {
                    client.Send(m);
                    rec.x40ErrorMessage = "";
                    rec.x40State = BO.x40StateFlag.Proceeded;
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Success;

                }
                catch (Exception ex)
                {

                    _db.CurrentUser.AddMessage(ex.Message);
                    rec.x40ErrorMessage = ex.Message;
                    rec.x40State = BO.x40StateFlag.Error;
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Failed;
                    ret.Message = rec.x40ErrorMessage;
                }

                
            }

            return ret;


        }

        private int SaveX40(MailMessage m,BO.x40MailQueue rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x40ID);
            p.AddInt("j40ID", rec.j40ID, true);
            if (rec.j03ID == 0) rec.j03ID = _mother.CurrentUser.pid;
            p.AddInt("j03ID", rec.j03ID, true);
            p.AddInt("x40RecordPid", rec.x40RecordPid, true);
            p.AddString("x40Entity", rec.x40Entity);
          
            p.AddString("x40SenderAddress", m.From.Address);
            p.AddString("x40SenderName", m.From.DisplayName);
            p.AddString("x40To", String.Join(",", m.To.Select(p => p.Address)));
            p.AddString("x40Bcc", String.Join(",", m.Bcc.Select(p => p.Address)));
            p.AddString("x40Cc", String.Join(",", m.CC.Select(p => p.Address)));
            p.AddString("x40Subject", m.Subject);
            p.AddString("x40Body", m.Body);
            p.AddBool("x40IsHtmlBody", m.IsBodyHtml);
            p.AddDateTime("x40WhenProceeded", DateTime.Now);
            p.AddString("x40ErrorMessage", rec.x40ErrorMessage);
            p.AddInt("x40State", (int)rec.x40State);
            p.AddString("x40Attachments", String.Join(",", m.Attachments.Select(p => p.Name)));

            return _db.SaveRecord("x40MailQueue", p.getDynamicDapperPars(), rec);
        }

        private BO.Result handle_result_error(string strError)
        {
            _mother.CurrentUser.AddMessage(strError);
            return new BO.Result(true, strError);
        }

    }
}
