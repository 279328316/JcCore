using System;  
using System.Collections.Generic;  
using System.ComponentModel;  
using System.Data;  
using System.Text;  
using System.Net;  
using System.Net.Mail;  
using System.Net.Mime;  
using System.IO;  

namespace Jc.Core.Helper
{
    public class MailHelper
    {
        public static void SendMial(string serverAddress, string msgBody, string msgSubject, string msgFromAddress, string msgFromName, List<string> msgToAddress)
        {
            try
            {
                SmtpClient client = new SmtpClient(serverAddress);
                MailAddress from = new MailAddress(msgFromAddress, msgFromName, System.Text.Encoding.UTF8);
                // Set destinations for the e-mail message.
                MailMessage message = new MailMessage();
                message.From = from;
                for (int i = 0; i < msgToAddress.Count; i++)
                {
                    message.To.Add(new MailAddress(msgToAddress[i]));
                }
                message.Body = msgBody;
                message.Body += Environment.NewLine;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = msgSubject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                string userState = "t";
                client.SendAsync(message, userState);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("邮件发送失败:" + ex.Message);
            }
        }
    }    
}
