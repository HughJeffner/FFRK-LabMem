using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FFRK_Machines.Services.Notifications
{
    public class EmailNotification : Notifications.Notification
    {

        public string SMTPHost { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string To { get; set; }
        public string From { get; set; }

        public override async Task Notify(Notifications.NotificationArgs args)
        {

            ColorConsole.Debug(ColorConsole.DebugCategory.Notifcation, "Sending email: {0}", this);
            using (var smtpClient = new SmtpClient(SMTPHost)
            {
                Port = Port,
                Credentials = new NetworkCredential(UserName, Password),
                EnableSsl = EnableSsl,
            })
            {
                var body = String.Format(@"LabMem encountered event: {0}\n{1}", args.EventType, args.Message);
                await smtpClient.SendMailAsync(From, To, "FFRK-LabMem Notification", body);
            };
            
            
        }

        public override string ToString()
        {
            return String.Format("server: {0}:{1} ssl:{2} credentials:{3}:{4} from:{5} to:{6}", 
                SMTPHost, 
                Port, 
                EnableSsl, 
                UserName, 
                new string('*', Password.Length), 
                From, 
                To
            );
        }

    }
}
