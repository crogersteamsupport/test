using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
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
        private ReportSenderPublicLog _publicLog;

        public ReportSender()
		{
		  _debugAddresses = new MailAddressCollection();
		}

		public override void Run()
		{
            ScheduledReports.UnlockThread(LoginUser, (int)_threadPosition);

            while (!IsStopped)
			{
                Logs.WriteHeader("Starting Run");

				try
				{
                    int waitBeforeLoggingWithoutScheduledReportsDue = 15;
                    DateTime noScheduledReportsDue = DateTime.UtcNow;

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

                            ScheduledReport scheduledReport = GetNextScheduledReport(LoginUser.ConnectionString, (int)_threadPosition, Logs);

                            if (scheduledReport != null)
                            {
                                string path = AttachmentPath.GetPath(LoginUser, scheduledReport.OrganizationId, AttachmentPath.Folder.ScheduledReportsLogs);
                                _publicLog = new ReportSenderPublicLog(path, scheduledReport.Id);
                                QueueEmail(scheduledReport);
                                noScheduledReportsDue = DateTime.UtcNow;
                            }
                            else
							{
                                if (DateTime.UtcNow.Subtract(noScheduledReportsDue).Minutes >= waitBeforeLoggingWithoutScheduledReportsDue)
                                {
                                    Log(string.Format("No scheduled reports due found in the last {0} minutes", waitBeforeLoggingWithoutScheduledReportsDue));
                                    noScheduledReportsDue = DateTime.UtcNow;
                                }

								Thread.Sleep(10000);
								continue;
							}
						}
						catch (Exception ex)
						{
                            Log("Error sending report email - Ending Thread");
							Logs.WriteException(ex);
							ExceptionLogs.LogException(LoginUser, ex, "ReportSender", "Error sending report email");
						}
                        finally
                        {
                            UpdateHealth();
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

		private static ScheduledReport GetNextScheduledReport(string connectionString, int lockID, Logs logs = null)
		{
			ScheduledReport result;
			LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);

			lock (_staticLock)
            {
                result = ScheduledReports.GetNextWaiting(loginUser, lockID.ToString());
            }

			return result;
		}

		public override void ReleaseAllLocks()
		{
			Emails.UnlockAll(LoginUser);
		}

        public static string[] GetReportColumnNames(LoginUser scheduledReportCreator, int reportID)
        {
            List<string> result = new List<string>();
            ReportColumn[] columns = GetReportColumns(scheduledReportCreator, reportID);
            foreach (ReportColumn column in columns)
            {
                result.Add(column.Name);
            }
            return result.ToArray();
        }

        public static ReportColumn[] GetReportColumns(LoginUser scheduledReportCreator, int reportID)
        {
            Report report = Reports.GetReport(scheduledReportCreator, reportID);
            if (report.ReportDefType == ReportType.Table || report.ReportDefType == ReportType.TicketView) return report.GetTabularColumns();
            return report.GetSqlColumns();
        }

        private void QueueEmail(ScheduledReport scheduledReport)
        {
            Log(string.Format("Scheduled Report Id: {0}, Report Id: {1}, Organization {2}", scheduledReport.Id.ToString(), scheduledReport.ReportId, scheduledReport.OrganizationId));
            Log(string.Format("Set to start on: {0}", scheduledReport.NextRun), LogType.Both);

            try
            {
                LoginUser scheduledReportCreator = new LoginUser(scheduledReport.CreatorId, scheduledReport.OrganizationId);
                Log(string.Format("Creator: {0} ({1})", scheduledReportCreator.GetUserFullName(), scheduledReportCreator.UserID));

                Log("Getting report");
                Report report = Reports.GetReport(scheduledReportCreator, scheduledReport.ReportId, LoginUser.UserID);
                Log(string.Format("Report \"{0}\" settings loaded", report.Name), LogType.Both);

                Log("Generating Report", LogType.Both);
                string reportAttachmentFile = GetReportDataToFile(scheduledReportCreator, report, scheduledReport.Id, "", false, true, Logs);
                Log(string.Format("Report generated and file attachment created: {0}", Path.GetFileName(reportAttachmentFile)), LogType.Public);
                Log(string.Format("Report file to attach: {0}", reportAttachmentFile));

                Organization organization = Organizations.GetOrganization(scheduledReportCreator, scheduledReportCreator.OrganizationID);
                MailMessage message = scheduledReport.GetMailMessage(reportAttachmentFile, organization);
                Log("Email message created", LogType.Both);
                Log(string.Format("Email Recipients: {0}", string.Join(",", message.To.Select(p => p.Address).ToArray())), LogType.Both);

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

                Log("Queueing email(s)", LogType.Both);
                AddMessage(scheduledReport.OrganizationId, string.Format("Scheduled Report Sent [{0}]", scheduledReport.Id), message, null, new string[] { reportAttachmentFile });
                Log("Email was queued to Emails for the emailprocessor");

                scheduledReport.RunCount = scheduledReport.RunCount != null ? (short)(scheduledReport.RunCount + 1) : (short)1;
                scheduledReport.LastRun = DateTime.UtcNow;
                scheduledReport.LockProcessId = null;

                if ((ScheduledReportFrequency)scheduledReport.RecurrencyId == ScheduledReportFrequency.Once)
                {
                    scheduledReport.NextRun = null;
                }
                else
                {
                    scheduledReport.SetNextRun();
                }

                scheduledReport.Collection.Save();
                Log(string.Format("Set next run to: {0}", scheduledReport.NextRun == null ? "Never" : scheduledReport.NextRun.ToString()), LogType.Both);
            }
            catch (Exception ex)
            {
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

        private static string GetReportDataToFile(LoginUser scheduledReportCreator, Report report, int scheduledReportId, string sortField, bool isDesc, bool useUserFilter, Logs logs = null)
        {
            if (logs != null)
            {
                logs.WriteEvent("GetReportTableAll");
            }
            
            DataTable dataTable = GetReportTableAll(scheduledReportCreator, report, sortField, isDesc, useUserFilter, false);

            if (logs != null)
            {
                logs.WriteEventFormat("dataTable created with {0} rows and {1} columns", dataTable.Rows.Count, dataTable.Columns.Count);
            }

            StringBuilder stringBuilder = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            stringBuilder.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                stringBuilder.AppendLine(string.Join(",", fields));
            }

            string reportAttachmentPath = AttachmentPath.GetPath(scheduledReportCreator, scheduledReportCreator.OrganizationID, AttachmentPath.Folder.ScheduledReports);
            string fileName = string.Format("{0}\\{1}_{2}.csv", reportAttachmentPath, scheduledReportId, report.ReportID);

            File.WriteAllText(fileName, stringBuilder.ToString());

            if (logs != null)
            {
                logs.WriteEvent("File.WriteAllText completed");
            }

            return fileName;
        }

        private static DataTable GetReportTableAll(LoginUser scheduledReportCreator, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            report.GetCommand(command, includeHiddenFields, false, useUserFilter);

            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                if (string.IsNullOrWhiteSpace(sortField))
                {
                    sortField = GetReportColumnNames(scheduledReportCreator, report.ReportID)[0];
                    isDesc = false;
                }

                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(scheduledReportCreator.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(scheduledReportCreator, ex, "Report Data");
                        throw;
                    }
                }

                connection.Close();
            }

            return table;
        }

        [Flags]
        private enum LogType
        {
            None = 0,
            Internal = 1,
            Public = 2,
            Both = Internal | Public
        }

        private void Log(string message, LogType logType = LogType.Internal)
        {
            switch (logType)
            {
                case LogType.Internal:
                    Logs.WriteEvent(message);
                    break;
                case LogType.Public:
                    _publicLog.Write(message);
                    break;
                case LogType.Both:
                    Logs.WriteEvent(message);
                    _publicLog.Write(message);
                    break;
                default:
                    Logs.WriteEvent(message);
                    break;
            }
        }

        public class ReportSenderPublicLog
        {
            private string _logPath;
            private string _fileName;

            public ReportSenderPublicLog(string path, int scheduledReportID)
            {
                _logPath = path;
                _fileName = scheduledReportID.ToString() + ".txt";

                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
            }

            public void Write(string text)
            {
                if (!File.Exists(_logPath + @"\" + _fileName))
                {
                    foreach (string oldFileName in Directory.GetFiles(_logPath))
                    {
                        if (File.GetLastWriteTime(oldFileName).AddDays(30) < DateTime.Today)
                        {
                            File.Delete(oldFileName);
                        }
                    }
                }

                int timeOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
                File.AppendAllText(_logPath + @"\" + _fileName, string.Format("{0} {1} ({2}): {3}{4}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), timeOffset, text, Environment.NewLine));
            }
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
