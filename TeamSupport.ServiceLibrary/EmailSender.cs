using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using Quiksoft.EasyMail.SMTP;



namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class EmailSender : ServiceThreadPoolProcess
    {
        private static int[] _nextAttempts = new int[] { 60, 36, 720, 1440 };
        private bool _isDebug = false;
        private static object _staticLock = new object();

        private static Email GetNextEmail(string connectionString, int lockID)
        {
            Email result;
            LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
            lock (_staticLock) { result = Emails.GetNextWaiting(loginUser, lockID.ToString()); }

            return result;
        }

        public override void ReleaseAllLocks()
        {
            Emails.UnlockAll(LoginUser);
        }

        public override void Run()
        {
            Logs.WriteHeader("Starting Run");
            Emails.UnlockThread(LoginUser, (int)_threadPosition);
            _isDebug = Settings.ReadBool("Debug", false);

            Logs.WriteHeader("Debug: " + _isDebug.ToString());

            Quiksoft.EasyMail.SMTP.License.Key = "Muroc Systems, Inc. (Single Developer)/9983782F406978487783FBAA248A#E86A";
            Quiksoft.EasyMail.SSL.License.Key = "Muroc Systems, Inc. (Single Developer)/9984652F406991896501FC33B3#02AE4B";

            Quiksoft.EasyMail.SSL.SSL ssl = new Quiksoft.EasyMail.SSL.SSL();
            ssl.SecureProtocol = Quiksoft.EasyMail.SSL.SecureProtocol.SSL3;
            SMTP smtp = new Quiksoft.EasyMail.SMTP.SMTP();

            //Set the SMTP server and secure port.
            int port = Settings.ReadInt("SMTP Port");
            string account = Settings.ReadString("SMTP UserName");
            var smtpServer = new SMTPServer
            {
                Name = Settings.ReadString("SMTP Host"),
                Port = port, //465, //Secure port
                Account = account,
                Password = Settings.ReadString("SMTP Password"),
                AuthMode = string.IsNullOrWhiteSpace(account) ? SMTPAuthMode.None : SMTPAuthMode.AuthLogin
            };


            smtp.SMTPServers.Add(smtpServer);
            if (port == 465 || port == 587) smtp.Connect(ssl.GetInterface()); else smtp.Connect();
            int count = 0;
            try
            {
                while (true)
                {
                    try
                    {
                        if (ServiceThread.ServiceStopped)
                        {
                            Logs.WriteHeader("ServiceThread.ServiceStopped");
                            break;
                        }

                        if (IsStopped)
                        {
                            Logs.WriteHeader("IsStopped");
                            break;
                        }


                        Email email = GetNextEmail(LoginUser.ConnectionString, (int)_threadPosition);
                        if (email == null) {
                            Thread.Sleep(10000);
                            continue;
                        }
                        SendEmail(email, smtp);
                        count++;
                        Logs.WriteEvent("Processing: #" + count.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logs.WriteEvent("Error sending email - Ending Thread");
                        Logs.WriteException(ex);
                        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending email");
                        return;
                    }
                }


            }
            finally
            {
                smtp.Disconnect();
                Logs.WriteHeader("Disconnecting");
            }

        }

        private void SendEmail(Email email, SMTP smtp)
        {
            Logs.WriteLine();
            Logs.WriteHeader("Processing Email");
            Logs.WriteEventFormat("EmailID: {0}, EmailPostID: {1}", email.EmailID.ToString(), email.EmailPostID.ToString());

            try
            {
                email.Attempts = email.Attempts + 1;
                email.Collection.Save();
                Logs.WriteEvent("Attempt: " + email.Attempts.ToString());
                Logs.WriteEventFormat("Size: {0}, Attachments: {1}", email.Size.ToString(), email.Attachments);
                MailMessage message = email.GetMailMessage();
                if (_isDebug == true)
                {
                    string debugAddresses = Settings.ReadString("Debug Email Address").Replace(';', ',');
                    Logs.WriteEvent("DEBUG Addresses: " + debugAddresses);
                    StringBuilder builder = new StringBuilder("<div><strong>Orginal To List:</strong></div>");
                    foreach (MailAddress address in message.To)
                    {
                        builder.AppendLine(string.Format("<div>{0}</div>", WebUtility.HtmlEncode(address.ToString())));
                    }
                    builder.AppendLine("<br /><br /><br />");

                    message.To.Clear();
                    message.To.Add(debugAddresses);
                    message.Subject = "[DEBUG] " + message.Subject;
                }
                Logs.WriteEvent("Sending email");

                EmailMessage msg = new EmailMessage();
                msg.CharsetEncoding = System.Text.Encoding.UTF8;
                foreach (var recipient in message.To)
                {
                    msg.Recipients.Add(recipient.Address, recipient.DisplayName);
                }
                msg.From.Name = message.From.DisplayName;
                msg.From.Email = message.From.Address;
                msg.ReplyTo = message.From.Address;
                msg.Subject = message.Subject;
                msg.CustomHeaders.Add("X-xsMessageId", email.OrganizationID.ToString());
                msg.CustomHeaders.Add("X-xsMailingId", email.EmailID.ToString());

                msg.BodyParts.Add(new Quiksoft.EasyMail.SMTP.BodyPart(message.Body, message.IsBodyHtml ? BodyPartFormat.HTML : BodyPartFormat.Plain));
                if (email.Size < 25000)
                {
                    string[] attachments = email.GetAttachments();
                    foreach (var attachment in attachments)
                    {
                        if (File.Exists(attachment))
                        {
                            msg.Attachments.Add(attachment);
                        }
                        else
                        {
                            Logs.WriteEvent("File unavailable :" + attachment);
                        }

                    }
                }

                
                try
                {
                    smtp.Send(msg);
                }
                catch (Quiksoft.EasyMail.SMTP.SMTPProtocolException smtpEx)
                {
                    Logs.WriteEvent(smtpEx.ToString());
                    if (smtpEx.Message.IndexOf("5.3.4") > -1)
                    {
                        msg.Attachments.Clear();
                        smtp.Send(msg);
                    }
                }
                Logs.WriteEvent("Email sent");
                email.IsSuccess = true;
                email.IsWaiting = false;
                email.Body = "";
                email.DateSent = DateTime.UtcNow;
                email.Collection.Save();
                UpdateHealth();
            }
            catch (Exception ex)
            {
                if (ex is Quiksoft.EasyMail.SSL.SSLConnectionException)
                {
                    throw ex;
                }
                else
                {
                    Logs.WriteEvent("Error sending email");
                    Logs.WriteException(ex);
                    ExceptionLogs.LogException(LoginUser, ex, _threadPosition.ToString() + " - Email Sender", email.Row);
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Log File: " + _threadPosition.ToString());
                    builder.AppendLine(ex.Message);
                    builder.AppendLine(ex.StackTrace);
                    email.NextAttempt = DateTime.UtcNow.AddMinutes(_nextAttempts[email.Attempts - 1] * email.Attempts);
                    email.LastFailedReason = builder.ToString();
                    email.IsSuccess = false;
                    email.IsWaiting = (email.Attempts < _nextAttempts.Length);
                    email.LockProcessID = null;
                    email.Collection.Save();
                }
            }
        }

    }
}
