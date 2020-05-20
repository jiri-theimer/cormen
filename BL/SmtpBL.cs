using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;


namespace BL
{
    public interface ISmtpBL
    {
        public BO.Result SendMessage(MailMessage message);
        public BO.Result SendMessage(string toEmail,string toName, string subject, string body,bool ishtml);
        public void AddAttachment(string fullpath);
        public void AddAttachment(Attachment att);

    }
    class SmtpBL : BaseBL, ISmtpBL
    {
        public SmtpBL(BL.Factory mother) : base(mother)
        {
            
        }
        private List<Attachment> _attachments;

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
        public BO.Result SendMessage(string toEmail, string toName, string subject, string body, bool ishtml)
        {
            MailMessage m = new MailMessage() { Body = body, IsBodyHtml = ishtml };
            m.From = new MailAddress(_mother.CurrentUser.j02Email, _mother.CurrentUser.FullName);

            if (string.IsNullOrEmpty(toName) == false)
            {
                m.To.Add(new MailAddress(toEmail, toName));                
            }
            else
            {
                m.To.Add(new MailAddress(toEmail));
            }
            

            return handle_finish(m);
        }
        public BO.Result SendMessage(MailMessage message)
        {
            if (message.From == null)
            {
                message.From = new MailAddress(_mother.CurrentUser.j02Email, _mother.CurrentUser.FullName);
            }
            return handle_finish(message);
        }


        private BO.Result handle_finish(MailMessage m)
        {
            if (m.From == null)
            {
                return new BO.Result(true, "Chybí odesílatel zprávy");
            }
            if (m.To.Count == 0)
            {
                return new BO.Result(true,"Chybí příjemce zprávy");
            }


            using (SmtpClient client = new SmtpClient(_mother.App.SmtpHost,_mother.App.SmtpPort))
            {
                if (String.IsNullOrEmpty(_mother.App.SmtpLogin) == false)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_mother.App.SmtpLogin, _mother.App.SmtpPassword);
                }
                else
                {
                    client.UseDefaultCredentials = true;
                }
             
               
                m.BodyEncoding = Encoding.UTF8;

                if (_attachments != null)
                {
                    foreach(var att in _attachments)
                    {
                        m.Attachments.Add(att);
                    }
                }
               
                client.Send(m);

                return new BO.Result(false);
            }

            
        }

    }
}
