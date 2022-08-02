using MimeKit;
using MailKit.Net.Smtp;
using System;

namespace Visionmore.Email {
    public class EmailHelper {
        public bool SendEmail(string userEmail, string confirmationLink) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vision & more", "ahmadharissa1997@gmail.com"));
            message.To.Add(new MailboxAddress("me", userEmail));
            message.Subject = "Confirm Email";
            message.Body = new TextPart("plain") {
                Text = confirmationLink
            };
            using (var client = new SmtpClient()) {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ahmadharissa1997@gmail.com", "115500aKH269");
                
                try {
                    client.Send(message);
                    return true;
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }

                client.Disconnect(true);
                return false;
            }
        }
    }
}
