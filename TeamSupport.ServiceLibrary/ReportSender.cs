using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using TeamSupport.Data;
using Quiksoft.EasyMail.SMTP;

namespace TeamSupport.ServiceLibrary
{
	[Serializable]
	public class ReportSender : ServiceThreadPoolProcess
	{
		private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };
		private bool _isDebug = false;
		private static object _staticLock = new object();
		private MailAddressCollection _debugAddresses;

		public ReportSender()
		{
		  _debugAddresses = new MailAddressCollection();
		}

		public override void Run()
		{
			while (!IsStopped)
			{
				/*
				 1) check scheduled reports that are due to be created/emailed (NextRun)
				 2) Process one by one. Order By nextRun ascending
				 3) Process report
				 4) Create/save file
				 5) Send email
				 */

				Logs.WriteHeader("Starting Run");

				try
				{
					while (true)
					{
						try
						{
							if (ServiceStopped)
							{
								Logs.WriteHeader("ServiceThread.ServiceStopped");
								break;
							}

							if (IsStopped)
							{
								Logs.WriteHeader("IsStopped");
								break;
							}

							ScheduledReport scheduleReport = GetNextScheduledReport(LoginUser.ConnectionString, (int)_threadPosition);

							if (scheduleReport == null)
							{
								Thread.Sleep(10000);
								continue;
							}

                            QueueEmail(scheduleReport);
						}
						catch (Exception ex)
						{
							Logs.WriteEvent("Error sending report email - Ending Thread");
							Logs.WriteException(ex);
							ExceptionLogs.LogException(LoginUser, ex, "ReportSender", "Error sending report email");
							return;
						}
					}
				}
				catch (Exception ex)
				{
					Logs.WriteException(ex);
				}
				finally
				{
					Logs.WriteHeader("Exiting.");
				}
			}
		}

		private static ScheduledReport GetNextScheduledReport(string connectionString, int lockID)
		{
			ScheduledReport result;
			LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
			lock (_staticLock) { result = ScheduledReports.GetNextWaiting(loginUser, lockID.ToString()); }

			return result;
		}

		public override void ReleaseAllLocks()
		{
			Emails.UnlockAll(LoginUser);
		}

        private void QueueEmail(ScheduledReport scheduledReport)
        {
            Logs.WriteLine();
            Logs.WriteHeader("Processing Scheduled Report");
            Logs.WriteEventFormat("Scheduled Report Id: {0}, Report Id: {1}", scheduledReport.Id.ToString(), scheduledReport.ReportId.ToString());

            try
            {
                // email.Attempts = email.Attempts + 1;
                // email.Collection.Save();
                // Logs.WriteEvent("Attempt: " + email.Attempts.ToString());
                string report = ""; //vv this should be the generated report, saved as file and attached to the email
                MailMessage message = scheduledReport.GetMailMessage(report);
                Logs.WriteEventFormat("Report file to attach: {0}", report);

                if (_isDebug == true)
                {
                    string debugWhiteList = Settings.ReadString("Debug Email White List", "");
                    string debugDomains = Settings.ReadString("Debug Email Domains", "");
                    string debugAddresses = Settings.ReadString("Debug Email Address", "");

                    if (!string.IsNullOrWhiteSpace(debugWhiteList))
                    {
                        Logs.WriteEvent("DEBUG Whitelist: " + debugWhiteList);
                        string[] addresses = debugWhiteList.Replace(';', ',').Split(',');
                        List<MailAddress> mailAddresses = new List<MailAddress>();

                        foreach (MailAddress mailAddress in message.To)
                        {
                            foreach (string address in addresses)
                            {
                                if (mailAddress.Address.ToLower().IndexOf(address.ToLower()) > -1)
                                {
                                    mailAddresses.Add(mailAddress);
                                }
                            }
                        }

                        message.To.Clear();

                        foreach (MailAddress mailAddress in mailAddresses)
                        {
                            message.To.Add(mailAddress);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(debugDomains))
                    {
                        Logs.WriteEvent("DEBUG Domains: " + debugDomains);
                        string[] domains = debugDomains.Replace(';', ',').Split(',');
                        List<MailAddress> mailAddresses = new List<MailAddress>();

                        foreach (MailAddress mailAddress in message.To)
                        {
                            foreach (string domain in domains)
                            {
                                if (mailAddress.Address.ToLower().IndexOf(domain.ToLower()) > -1)
                                {
                                    mailAddresses.Add(mailAddress);
                                }
                            }
                        }

                        message.To.Clear();

                        foreach (MailAddress mailAddress in mailAddresses)
                        {
                            message.To.Add(mailAddress);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(debugAddresses))
                    {
                        Logs.WriteEvent("DEBUG Addresses: " + debugAddresses);
                        message.To.Clear();
                        message.To.Add(debugAddresses.Replace(';', ','));
                    }
                    else
                    {
                        Logs.WriteEvent("NO DEBUG FILTERS SET");
                        return;
                    }

                    if (message.To.Count < 1)
                    {
                        Logs.WriteEvent("No Debug Address specified.");
                        return;
                    }

                    message.Subject = string.Format("[{0}] {1}", Settings.ReadString("Debug Email Subject", "TEST MODE"), message.Subject);
                }

                Logs.WriteEvent("Sending email");

                AddMessage(scheduledReport.OrganizationId, string.Format("Scheduled Report Sent [{0}]", scheduledReport.Id), message, null, new string[] { report });
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error sending email");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, _threadPosition.ToString() + " - Report Sender", scheduledReport.Row);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Log File: " + _threadPosition.ToString());
                builder.AppendLine(ex.Message);
                builder.AppendLine(ex.StackTrace);
            }
        }

        private void AddMessage(int organizationID, string description, MailMessage message, string replyToAddress = null, string[] attachments = null, DateTime? timeToSend = null, Ticket ticket = null)
        {
            Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
            string replyAddress = organization.GetReplyToAddress(replyToAddress).Trim();

            int i = 0;
            while (i < message.To.Count)
            {
                MailAddress address = message.To[i];
                if (address.Address.ToLower().Trim() == message.From.Address.ToLower().Trim() || address.Address.ToLower().Trim() == replyAddress || address.Address.ToLower().Trim() == organization.SystemEmailID.ToString().Trim().ToLower() + "@teamsupport.com")
                {
                    message.To.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (message.To.Count < 1) return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message.Body);
            message.HeadersEncoding = Encoding.UTF8;
            message.Body = builder.ToString();
            List<MailAddress> addresses = message.To.ToList();
            string body = message.Body;
            string subject = message.Subject;

            foreach (MailAddress address in addresses)
            {
                message.To.Clear();
                Logs.WriteEvent(string.Format("Adding email address [{0}]", address.ToString()));
                message.To.Add(GetMailAddress(address.Address, address.DisplayName));
                Logs.WriteEvent(string.Format("Successfuly added email address [{0}]", address.ToString()));
                message.HeadersEncoding = Encoding.UTF8;
                message.Body = body;
                message.Subject = subject;

                Logs.WriteEvent(string.Format("Adding ReplyTo Address[{0}]", replyAddress.Replace("<", "").Replace(">", "")));
                MailAddress replyMailAddress = null;
                try
                {
                    replyMailAddress = GetMailAddress(replyAddress);
                }
                catch (Exception)
                {
                    replyMailAddress = GetMailAddress(organization.GetReplyToAddress());
                }

                message.From = replyMailAddress;
                EmailTemplates.ReplaceMailAddressParameters(message);
                Emails.AddEmail(LoginUser, organizationID, null, description, message, attachments, timeToSend);

                if (message.Subject == null) message.Subject = "";
                Logs.WriteEvent(string.Format("Queueing email [{0}] - {1}  Subject: {2}", description, address.ToString(), message.Subject));
            }
        }
        private MailAddress GetMailAddress(string address, string displayName)
        {
            return new MailAddress(FixMailAddress(address), FixMailName(displayName));
        }

        private string FixMailAddress(string address)
        {
            return address.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        private string FixMailName(string displayName)
        {
            return displayName.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        private MailAddress GetMailAddress(string address)
        {
            return new MailAddress(FixMailAddress(address));
        }

        private MailAddress GetMailAddress(string address, string displayName, Encoding displayNameEncoding)
        {
            return new MailAddress(FixMailAddress(address), FixMailName(displayName), displayNameEncoding);
        }


        /*
		  _isDebug = Settings.ReadBool("Debug", false);
		  _debugAddresses.Clear();

		  try
		  {
			string[] addresses = Settings.ReadString("Debug Email Address").Split(';');
			foreach (string item in addresses) { _debugAddresses.Add(new MailAddress(item.Trim())); }
		  }
		  catch (Exception)
		  {
		  }


			EmailPosts emailPosts = new EmailPosts(LoginUser);
			emailPosts.LoadReportEmails();

			foreach (EmailPost emailPost in emailPosts)
			{
			  if (emailPost.EmailPostType != EmailPostType.SendReport) continue;

			  if (DateTime.UtcNow > ((DateTime)emailPost.Row["DateCreated"]).AddSeconds(emailPost.HoldTime) || _isDebug)
			  {
				try
				{
				  SetTimeZone(emailPost);
				  ProcessEmail(emailPost);
				}
				catch (Exception ex)
				{
				  ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
				}
				emailPost.Collection.DeleteFromDB(emailPost.EmailPostID);
			  }
			  System.Threading.Thread.Sleep(0);
			  if (IsStopped) break;
			}
		  }
		  catch (Exception ex)
		  {
			ExceptionLogs.LogException(LoginUser, ex, "Email", "Error processing emails");
		  }

		  try
		  {
			SendEmails();
		  }
		  catch (Exception ex)
		  {
			ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending emails");
		  }*/
    }

    /*
    public void ProceesReportEmail(int reportID, int userID)
    {
      User user = Users.GetUser(LoginUser, userID);
      Report report = Reports.GetReport(LoginUser, reportID);

      if (report == null || (report.OrganizationID != LoginUser.OrganizationID && report.OrganizationID != null)) { return; }

      string sql = report.GetSql(false);
      SqlCommand command = new SqlCommand(sql);
      Report.CreateParameters(LoginUser, command, userID);
      string text = DataUtils.CommandToCsv(LoginUser, command, false);
      /*
      MemoryStream stream = new MemoryStream();
      ZipOutputStream zip = new ZipOutputStream(stream);
      zip.SetLevel(9);
      zip.PutNextEntry(new ZipEntry(report.Name + ".csv"));
      Byte[] bytes = Encoding.UTF8.GetBytes(text);
      zip.Write(bytes, 0, bytes.Length);
      zip.CloseEntry();
      zip.Finish();
      stream.WriteTo(context.Response.OutputStream);
      //context.Response.ContentType = "application/x-zip-compressed";
      context.Response.ContentType = "application/octet-stream";
      context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + report.Name + ".zip\"");
      context.Response.AddHeader("Content-Length", stream.Length.ToString());
      zip.Close();* /

      context.Response.Write(text);
      context.Response.ContentType = "application/octet-stream";
      context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + report.Name + ".csv\"");
      //context.Response.AddHeader("Content-Length", text.Length.ToString());
      
      
      MailMessage message = new MailMessage();
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      message.From = new MailAddress("support@teamsupport.com");
      message.IsBodyHtml = true;
      message.Body = "Test";
      message.Subject = "Test";


      MemoryStream stream = new MemoryStream();
      ZipOutputStream zip = new ZipOutputStream(stream);
      zip.SetLevel(9);
      zip.PutNextEntry(new ZipEntry(report.Name + ".csv"));
      Byte[] bytes = Encoding.UTF8.GetBytes(text);
      zip.Write(bytes, 0, bytes.Length);
      zip.CloseEntry();
      zip.Finish();
      message.Attachments.Add(new System.Net.Mail.Attachment(stream, report.Name));

      
      zip.Close();
      

      
    }

    private void SendEmails()
    {
      Emails emails = new Emails(LoginUser);
      emails.LoadTop100Waiting();
      if (emails.IsEmpty) return;

      SmtpClient client = new SmtpClient();
      client = new SmtpClient(Settings.ReadString("SMTP Host"), Settings.ReadInt("SMTP Port"));
      string username = Settings.ReadString("SMTP UserName", "");
      if (username.Trim() != "") client.Credentials = new System.Net.NetworkCredential(Settings.ReadString("SMTP UserName"), Settings.ReadString("SMTP Password"));

      foreach (Email email in emails)
      {
        if (IsStopped) break;
        try
        {
          if (email.NextAttempt < DateTime.UtcNow)
          {

            email.Attempts = email.Attempts + 1;
            email.Collection.Save();
            MailMessage message = email.GetMailMessage();
            message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
            message.Headers.Add("X-xsMailingId", email.EmailID.ToString());
            client.Send(message);
            email.IsSuccess = true;
            email.IsWaiting = false;
            email.Body = "";
            email.DateSent = DateTime.UtcNow;
            email.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          StringBuilder builder = new StringBuilder();
          builder.AppendLine(ex.Message);
          builder.AppendLine("<br />");
          builder.AppendLine("<br />");
          builder.AppendLine("<br />");
          builder.AppendLine(ex.StackTrace);
          email.NextAttempt = DateTime.UtcNow.AddMinutes(_nextAttempts[email.Attempts - 1] * email.Attempts);
          email.LastFailedReason = builder.ToString();
          email.IsSuccess = false;
          email.IsWaiting = (email.Attempts < _nextAttempts.Length);
          email.Collection.Save();
        }
      }
    }
*/    
}
