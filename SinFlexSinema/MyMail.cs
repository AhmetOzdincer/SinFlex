﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SinFlexSinema
{
    public class MyMail
    {
        #region - Şifre -

        private const string password = "ahmtozdncr14760045"; 
        #endregion
        public string ToMail { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public MyMail(string _tomail,string _subject, string _body)
        {
            this.ToMail = _tomail;
            this.Subject = _subject;
            this.Body = _body;
        }
        public void SendMail()
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress("ahmtozdncr@gmail.com", "Ahmet Özdinçer")
            };
            mail.To.Add(this.ToMail);
            mail.Subject = this.Subject;
            mail.Body = this.Body;

            SmtpClient client = new SmtpClient()
            {
                Port=587,
                Host="smtp.gmail.com",
                EnableSsl = true

            };
            client.Credentials = new System.Net.NetworkCredential("ahmtozdncr@gmail.com", password);
            client.Send(mail);
        }
    }
}