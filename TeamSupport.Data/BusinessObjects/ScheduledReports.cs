using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
	public partial class ScheduledReport
	{
		public string Creator
		{
			get
			{
				if (Row.Table.Columns.Contains("Creator") && Row["Creator"] != DBNull.Value)
				{
					return (string)Row["Creator"];
				}
				else return "";
			}
		}

		public string Modifier
		{
			get
			{
				if (Row.Table.Columns.Contains("Modifier") && Row["Modifier"] != DBNull.Value)
				{
					return (string)Row["Modifier"];
				}
				else return "";
			}
		}

		public string ReportName
		{
			get
			{
				if (Row.Table.Columns.Contains("ReportName") && Row["ReportName"] != DBNull.Value)
				{
					return (string)Row["ReportName"];
				}
				else return "";
			}
		}

		public void SetNextRun()
		{
			DateTime dateOnly = StartDateUtc.Date;
			DateTime timeOnly = default(DateTime).Add(StartDateUtc.TimeOfDay);
			DayOfWeek dayOfWeek = DayOfWeek.Sunday;

			if (LastRunUtc != null)
			{
				dateOnly = StartDateUtc > LastRunUtc ? StartDateUtc : (DateTime)LastRunUtc;
			}

			switch ((ScheduledReportFrequency)RecurrencyId)
			{
				case ScheduledReportFrequency.Once:
					NextRun = StartDateUtc;
					break;
				case ScheduledReportFrequency.Weekly:
					//vv we need: startdate, every, weekday (1:Sun, ..., 7:Sat)
					while (dateOnly < DateTime.UtcNow)
					{
						int totalDaysInAWeek = 7;
						int totalDays = (byte)Every * totalDaysInAWeek;
						dateOnly = dateOnly.AddDays(totalDays);

						//The list in the UI is: 1: Sunday, ..., 7: Saturday. So we need to substract 1 to convert it to DayOfWeek
						dayOfWeek = (DayOfWeek)(byte)Weekday - 1;

						if (dateOnly.DayOfWeek != dayOfWeek)
						{
							dateOnly = dateOnly.AddDays(-totalDaysInAWeek);
							dateOnly = dateOnly.AddDays(dayOfWeek - dateOnly.DayOfWeek);
						}
					}

					NextRun = dateOnly.Add(timeOnly.TimeOfDay);

					break;
				case ScheduledReportFrequency.Monthly:
					//vv we need: startdate, every, weekday (1:Sun, ..., 7:Sat), 
					//				monthday (if < 5 then weekday can have a value: the 1st monday.. the 3rd wednesday, etc;
					//						else weekday has to be null: the 5th of the month, the 20th of the month, etc)

					DateTime startOfTheMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

					while (dateOnly < DateTime.UtcNow)
					{
						if (Monthday < 5)
						{
							int totalDaysInAWeek = 7;
							dayOfWeek = (DayOfWeek)(byte)Weekday - 1;
							startOfTheMonth = new DateTime(dateOnly.Year, dateOnly.Month, 1);
							startOfTheMonth = startOfTheMonth.AddMonths((byte)Every);

							// set the first ocurrence of the monthday
							int diff = dayOfWeek - startOfTheMonth.DayOfWeek;

							if (diff < 0)
							{
								startOfTheMonth = startOfTheMonth.AddDays(totalDaysInAWeek + diff);
								diff = 0;
							}

							dateOnly = startOfTheMonth.AddDays(diff + (((byte)Monthday - 1) * totalDaysInAWeek));
						}
						else
						{
							Weekday = null;

							if (DateTime.UtcNow > StartDateUtc)
							{
								if (DateTime.UtcNow.DayOfYear < startOfTheMonth.AddDays((byte)Monthday).DayOfYear)
								{
									dateOnly = startOfTheMonth.AddDays((byte)Monthday);
								}
								else
								{
									startOfTheMonth = new DateTime(StartDateUtc.Year, StartDateUtc.Month, 1);
									dateOnly = startOfTheMonth.AddDays((byte)Monthday);
								}
							}

							//Add the rest of the Every "N" months, the first one was calculated above.
							dateOnly.AddMonths((byte)Every - 1);
						}
					}

					NextRun = dateOnly.Add(timeOnly.TimeOfDay);

					break;
				default:
					break;
			}
		}

        public MailMessage GetMailMessage(string attachment)
        {
            MailMessage message = new MailMessage();
            message.From = GetEmailAddressFromString(""); //vv FROM?
            AddEmailAddressesFromString(message.To, EmailRecipients);
            message.Subject = EmailSubject;
            message.Body = EmailBody;

            //vv ? message.IsBodyHtml = IsHtml;
            bool isHtml = false; //vv ??
            if (isHtml)
            {
                //string img = "<div><img src=\"http://integrate.teamsupport.com/MailRead.aspx?OrganizationID={0}&EmailID={1}\" alt=\"\" width=\"0\" height=\"0\" style=\"width: 0px; height: 0px, border: 0px;\"/></div>";
                //message.Body = Body + string.Format(img, OrganizationID, EmailID);
            }

            if (!string.IsNullOrEmpty(attachment))
            {
                foreach (string fileName in attachment.Split(';'))
                {
                    try
                    {
                        message.Attachments.Add(new System.Net.Mail.Attachment(fileName));
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return message;
        }

        private MailAddress GetEmailAddressFromString(string text)
        {
            string name = "";
            int start = text.IndexOf('"');
            int end = -1;
            if (start > -1)
            {
                start++;
                end = text.IndexOf('"', start + 1);
                if (end > -1)
                {
                    name = text.Substring(start, end - start).Trim();
                }
            }

            if (name == "") return new MailAddress(text);
            string address = text;

            start = text.IndexOf('<');
            end = -1;
            if (start > -1)
            {
                start++;
                end = text.IndexOf('>', start + 1);
                if (end > -1)
                {
                    address = text.Substring(start, end - start).Trim();
                }
            }

            return new MailAddress(address, name);
        }

        private void AddEmailAddressesFromString(MailAddressCollection collection, string text)
        {
            if (string.IsNullOrEmpty(text.Trim())) return;
            string[] list = text.Split('|');
            foreach (string s in list)
            {
                MailAddress address = GetEmailAddressFromString(s);
                if (address != null) collection.Add(address);
            }

        }
    }

	public partial class ScheduledReports
	{
		public void LoadAll(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT ScheduledReports.*, Creator.FirstName + ' ' + Creator.LastName AS Creator, Modifier.FirstName + ' ' + Modifier.LastName AS Modifier, Reports.Name AS ReportName
										FROM ScheduledReports
											JOIN Reports ON ScheduledReports.ReportId = Reports.ReportID
											LEFT JOIN [Users] AS Creator ON ScheduledReports.creatorId = Creator.UserID
											LEFT JOIN [Users] AS Modifier ON ScheduledReports.modifierId = Modifier.UserID
										WHERE (ScheduledReports.organizationId = @OrganizationID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public static ScheduledReport GetNextWaiting(LoginUser loginUser, string processID)
		{
			ScheduledReports scheduledReports = new ScheduledReports(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"UPDATE ScheduledReports
SET LockProcessID = @ProcessID 
OUTPUT Inserted.*
WHERE Id IN (
  SELECT TOP 1 Id 
FROM ScheduledReports 
WHERE 
	LockProcessID IS NULL 
	AND NextRun IS NOT NULL 
	AND ISNULL(NextRun,0) > ISNULL(LastRun,0)
	AND NextRun < GETUTCDATE()
    ORDER BY NextRun
    )
";

				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ProcessID", processID);
                scheduledReports.Fill(command);
			}

			if (scheduledReports.IsEmpty)
				return null;
			else
				return scheduledReports[0];
		}
	}

	[DataContract]
	public class ScheduledReportItem
	{

		public ScheduledReportItem(ScheduledReport scheduledReport)
		{
			this.Id = scheduledReport.Id;
			this.OrganizationId = scheduledReport.OrganizationId;
			this.EmailSubject = scheduledReport.EmailSubject;
			this.EmailBody = scheduledReport.EmailBody;
			this.EmailRecipients = scheduledReport.EmailRecipients;
			this.ReportId = scheduledReport.ReportId;
			this.ReportName = scheduledReport.ReportName;
			this.RunCount = scheduledReport.RunCount ?? 0;
			this.IsActive = scheduledReport.IsActive;
			this.LastRun = scheduledReport.LastRun;
			this.NextRun = scheduledReport.NextRun;
			this.CreatorId = scheduledReport.CreatorId;
			this.Creator = scheduledReport.Creator ?? "Unknown";
			this.ModifierId = scheduledReport.ModifierId;
			this.Modifier = scheduledReport.Modifier ?? "Unknown";
			this.DateCreated = scheduledReport.DateCreated;
			this.DateModified = scheduledReport.DateModified;
		}

		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int? OrganizationId { get; set; }
		[DataMember]
		public string EmailSubject { get; set; }
		[DataMember]
		public string EmailBody { get; set; }
		[DataMember]
		public string EmailRecipients { get; set; }
		[DataMember]
		public int ReportId { get; set; }
		[DataMember]
		public string ReportName { get; set; }
		[DataMember]
		public short RunCount { get; set; }
		[DataMember]
		public bool IsActive { get; set; }
		[DataMember]
		public DateTime? LastRun { get; set; }
		[DataMember]
		public DateTime? NextRun { get; set; }
		[DataMember]
		public int CreatorId { get; set; }
		[DataMember]
		public int? ModifierId { get; set; }
		[DataMember]
		public string Creator { get; set; }
		[DataMember]
		public string Modifier { get; set; }
		[DataMember]
		public DateTime DateCreated { get; set; }
		[DataMember]
		public DateTime? DateModified { get; set; }
	}
}
