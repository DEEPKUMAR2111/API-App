﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Utility
{
    public class EmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
          /*  var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("hello@DeepPatel.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //send Email

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("UserId", "Password");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }*/

            return Task.CompletedTask;
        }
    }
}
