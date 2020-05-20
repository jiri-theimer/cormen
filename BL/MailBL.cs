using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;

namespace BL
{
    public interface IMailBL
    {
        public BO.Result SendMessage(int j40id, MailMessage message);
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml);
        public void AddAttachment(string fullpath);
        public void AddAttachment(Attachment att);
        public BO.j40MailAccount LoadJ40(int pid);
        public IEnumerable<BO.j40MailAccount> GetListJ40();
        public int SaveJ40(BO.j40MailAccount rec);


    }
    class MailBL : BaseBL, IMailBL
    {
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
        private BO.j40MailAccount InhaleMessageSender(int j40id)
        {
            if (j40id == 0)
            {
                j40id = _mother.MailBL.GetListJ40().Where(p => p.j40UsageFlag == BO.MailUsageFlag.SmtpGlobal).First().pid;
            }
            var c = LoadJ40(j40id);
            if (c.j40SmtpUsePersonalReply == true)
            {
                c.j40SmtpEmail = _mother.CurrentUser.j02Email;
                c.j40SmtpName = _mother.CurrentUser.FullName;
            }
            return c;
        }
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml)
        {
            
            MailMessage m = new MailMessage() { Body = body, Subject = subject, IsBodyHtml = ishtml };
            var c = InhaleMessageSender(j40id);

            m.From = new MailAddress(c.j40SmtpEmail, c.j40SmtpName);

            if (string.IsNullOrEmpty(toName) == false)
            {
                m.To.Add(new MailAddress(toEmail, toName));
            }
            else
            {
                m.To.Add(new MailAddress(toEmail));
            }

            return handle_smtp_finish(LoadJ40(j40id), m);
        }
        public BO.Result SendMessage(int j40id, MailMessage message)
        {
            var c = InhaleMessageSender(j40id);
            if (message.From == null)
            {
                message.From = new MailAddress(c.j40SmtpEmail, c.j40SmtpName);
            }
            return handle_smtp_finish(c, message);
        }


        private BO.Result handle_smtp_finish(BO.j40MailAccount c, MailMessage m)
        {
            if (m.From == null)
            {
                return new BO.Result(true, "Chybí odesílatel zprávy");
            }
            if (m.To.Count == 0)
            {
                return new BO.Result(true, "Chybí příjemce zprávy");
            }
            if (string.IsNullOrEmpty(m.Body) == true)
            {
                return new BO.Result(true, "Chybí text zprávy.");
            }
            if (string.IsNullOrEmpty(m.Subject) == true)
            {
                return new BO.Result(true, "Chybí předmět zprávy.");
            }

            if (c.j40SmtpUsePersonalReply == true)
            {
                m.ReplyToList.Add(new MailAddress(_mother.CurrentUser.j02Email, _mother.CurrentUser.FullName));
            }


            using (SmtpClient client = new SmtpClient(c.j40SmtpHost, c.j40SmtpPort))
            {
                client.UseDefaultCredentials = c.j40SmtpUseDefaultCredentials;
                if (c.j40SmtpUseDefaultCredentials == false)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(c.j40SmtpLogin, c.j40SmtpPassword);
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

                client.Send(m);

                _attachments = null;


                return new BO.Result(false);
            }


        }

    }
}
