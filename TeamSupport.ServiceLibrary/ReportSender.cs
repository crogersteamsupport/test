using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
	[Serializable]
	public class ReportSender : ServiceThreadPoolProcess
	{
		private bool _isDebug = false;
		private static object _staticLock = new object();
		private MailAddressCollection _debugAddresses;
        private ReportSenderPublicLog _publicLog;
        public const string PHANTOMJSCOMMAND = @"phantomjs highcharts-convert.js -infile {0} -outfile {1}";

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
                                string publicLogPath = AttachmentPath.GetPath(LoginUser, scheduledReport.OrganizationId, AttachmentPath.Folder.ScheduledReportsLogs);
                                _publicLog = new ReportSenderPublicLog(publicLogPath, scheduledReport.Id);
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

        private string GetReportChartFile(LoginUser scheduledReportCreator, ScheduledReport scheduledReport, Report report)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string chartOptionsFilesDirectory = "ChartOptionsFiles";
            string chartOptionsFilesPath = Path.Combine(path, chartOptionsFilesDirectory);

            if (!Directory.Exists(chartOptionsFilesPath))
            {
                Directory.CreateDirectory(chartOptionsFilesPath);
            }

            //ToDo //vv generate the JS file with the chart options and data!!
            DataTable dataTable = GetReportTableAll(scheduledReportCreator, report, "", false, true, false);
            ReportItem reportItem = new ReportItem(report, true);

            SummaryReport summaryReport = Newtonsoft.Json.JsonConvert.DeserializeObject<SummaryReport>(report.ReportDef);
            DataTable table = Reports.GetSummaryData(scheduledReportCreator, summaryReport, true, report);
            string reportData = Reports.BuildChartData(scheduledReportCreator, table, summaryReport);
            string optionsFileData = GetChartOptionsAndData(report.ReportDef, reportData);

            string optionsFile = string.Format("{0}_{1}.js", scheduledReport.Id, scheduledReport.ReportId);
            optionsFile = Path.Combine(chartOptionsFilesPath, optionsFile);
            File.WriteAllText(optionsFile, optionsFileData);

            string outputImagePath = AttachmentPath.GetPath(_loginUser, scheduledReport.OrganizationId, AttachmentPath.Folder.ScheduledReports);
            string outputImage = string.Format("{0}\\{1}_{2}.png", outputImagePath, scheduledReport.Id, scheduledReport.ReportId);

            //Create Batch File
            string batchFile = string.Format("thread_{0}.bat", _threadPosition);
            string batchFileCommand = string.Format(PHANTOMJSCOMMAND, optionsFile, outputImage);
            string batchFileFullPath = Path.Combine(path, batchFile);
            File.WriteAllText(batchFileFullPath, batchFileCommand);

            bool isImageCreated = ExecuteCommand(batchFileFullPath);

            if (!isImageCreated)
            {
                outputImage = string.Empty;
            }

            return outputImage;
        }

        /// <summary>
        /// Unfortunately we need to recreate the UI process that generates the reports including the JS function that append the json needed.
        /// This needs to match (in its C# version) to the addChartData function in ..\WebApp\Resources\Pages\ReportCharts.js !!!!!!!!!!!!! 
        /// </summary>
        /// <param name="reportDef"></param>
        /// <param name="recordsData"></param>
        /// <returns></returns>
        private static string GetChartOptionsAndData(string reportDef, string recordsData)
        {
            dynamic reportDefObject = new System.Dynamic.ExpandoObject();
            reportDefObject.Def = JsonConvert.DeserializeObject(reportDef);

            dynamic options = new System.Dynamic.ExpandoObject();
            options = JsonConvert.DeserializeObject(reportDefObject.Def.Chart.ToString());
            dynamic records = new System.Dynamic.ExpandoObject();
            records = JsonConvert.DeserializeObject(recordsData);

            string[] old = { "#3276B1", "#193b58", "#78A300", "#e72b19", "#008080", "#E57B3A", "#bd4cff", "#FFC312", "#BA55D3" };
            string[] berry = { "#8A2BE2", "#BA55D3", "#4169E1", "#C71585", "#0000FF", "#8019E0", "#DA70D6", "#7B68EE", "#C000C0", "#0000CD", "#800080" };
            string[] bright = { "#008000", "#0000FF", "#800080", "#800080", "#FF00FF", "#008080", "#FFFF00", "#808080", "#00FFFF", "#000080", "#800000", "#FF3939", "#7F7F00", "#C0C0C0", "#FF6347", "#FFE4B5" };
            string[] brightPastel = { "#418CF0", "#FCB441", "#DF3A02", "#056492", "#BFBFBF", "#1A3B69", "#FFE382", "#129CDD", "#CA6B4B", "#005CDB", "#F3D288", "#506381", "#F1B9A8", "#E0830A", "#7893BE" };
            string[] chocolate = { "#A0522D", "#D2691E", "#8B0000", "#CD853F", "#A52A2A", "#F4A460", "#8B4513", "#C04000", "#B22222", "#B65C3A" };
            string[] earthTones = { "#33023", "#B8860B", "#C04000", "#6B8E23", "#CD853F", "#C0C000", "#228B22", "#D2691E", "#808000", "#20B2AA", "#F4A460", "#00C000", "#8FBC8B", "#B22222", "#843A05", "#C00000"};
            string[] excel = { "#9999FF", "#993366", "#FFFFCC", "#CCFFFF", "#660066", "#FF8080", "#0063CB", "#CCCCFF", "#000080", "#FF00FF", "#FFFF00", "#00FFFF", "#800080", "#800000", "#007F7F", "#0000FF" };
            string[] fire = { "#FFD700", "#FF0000", "#FF1493", "#DC143C", "#FF8C00", "#FF00FF", "#FFFF00", "#FF4500", "#C71585", "#DDE221" };
            string[] grayScale = { "#C8C8C8", "#BDBDBD", "#B2B2B2", "#A7A7A7", "#9C9C9C", "#919191", "#868686", "#7A7A7A", "#707070", "#656565", "#565656", "#4F4F4F", "#424242", "#393939", "#2E2E2E", "#232323" };
            string[] light = { "#E6E6FA", "#FFF0F5", "#FFDAB9", "#", "#FFFACD", "#", "#FFE4E1", "#F0FFF0", "#F0F8FF", "#F5F5F5", "#FAEBD7", "#E0FFFF" };
            string[] pastel = { "#87CEEB", "#32CD32", "#BA55D3", "#F08080", "#4682B4", "#9ACD32", "#40E0D0", "#FF69B4", "#F0E68C", "#D2B48C", "#8FBC8B", "#6495ED", "#DDA0DD", "#5F9EA0", "#FFDAB9", "#FFA07A" };
            string[] seaGreen = { "#2E8B57", "#66CDAA", "#4682B4", "#008B8B", "#5F9EA0", "#38B16E", "#48D1CC", "#B0C4DE", "#8FBC8B", "#87CEEB" };
            string[] semiTransparent = { "#FF6969", "#69FF69", "#6969FF", "#FFFF5D", "#69FFFF", "#FF69FF", "#CDB075", "#FFAFAF", "#AFFFAF", "#AFAFFF", "#FFFFAF", "#AFFFFF", "#FFAFFF", "#E4D5B5", "#A4B086", "#819EC1" };

            options.colors = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(brightPastel));
            string series = string.Empty;
            string xAxis = string.Empty;

            if (options.ts.chartType == "pie")
            {
                if (records.Count > 2)
                {
                    return "Please do not select a series field to plot a pie chart.";
                }

                int total = 0;

                for (int i = 0; i < records[1].data.Count; i++)
                {
                    total += (int)records[1].data[i];
                }

                string seriesFormat = "[{{ type: 'pie', name: '" + options.ts.seriesTitle.ToString() + "', data: [{0}]}}]";
                List<string> seriesData = new List<string>();

                for (var i = 0; i < records[1].data.Count; i++)
                {
                    float val = (float)(records[1].data[i]) / total * 100;
                    seriesData.Add("['" + fixBlankSeriesName(records[0].data[i].ToString()) + "', " + val.ToString("#.00") + "]");
                }

                if (seriesData.Any() && seriesData.Count > 0)
                {
                    series = string.Format(seriesFormat, string.Join(",", seriesData.ToArray()));
                }
                else
                {
                    series = string.Format(seriesFormat, "");
                }
            }
            else if ((records[0].fieldType == "datetime" && records[0].format == "date") || (records[1].fieldType == "datetime" && records[1].format == "date"))
            {
                options.series = new string[1];//vv
                options.xAxis = "{ type: 'datetime' }";

                if (records.Count == 3)
                {

                    for (var i = 0; i<records[0].data.Count; i++)
                    {
                        var val = records[0].data[i];
                        series = "";//vv findSeries(options, val);

                        if (string.IsNullOrEmpty(series))
                        {
                            series = "{ name: " + fixBlankSeriesName(fixRecordName(records[0], i)) + ", value: val, data: []"; //vv
                        };

                            options.series.push(series);
                    }

                    List<string> item = new List<string>();
                    //vv item.push(records[2].data[i]);

                    if (item[0] != null)
                    {
                        if (item[1] == null) item[1] = "0";
                    }
                }
                else if (records.Count = 2)
                {
                    options.series.push("{ name: records[1].name, data: [] }");

                    for (var i = 0; i<records[0].data.length; i++)
                    {
                        List<string> item = new List<string>();
                        //vv item.push(Date.parse(records[0].data[i]));
                        //vv item.push(records[1].data[i]);

                        if (item[0] != null)
                        {
                            if (item[1] == null) item[1] = "0";
                            options.series[0].data.push(item);
                        }
                    }
                }
            }
            else
            {
                //options.series = new List<string>(); //vv [];

                if (records.Count == 3)
                {
                    options.xAxis = "{ categories: [] }";

                    // build categories
                    for (var i = 0; i<records[1].data.length; i++)
                    {
                        var name = fixRecordName(records[1], i);

                        if (indexOfCategory(options, name) < 0)
                        {
                            options.xAxis.categories.push(name);
                        }
                    }

                    for (var i = 0; i<records[0].data.length; i++)
                    {
                        var val = records[0].data[i];
                        series = findSeries(options, val);
                
                        if (!string.IsNullOrEmpty(series))
                        {
                            series = "{ name: " + fixBlankSeriesName(fixRecordName(records[0], i)) + ", value: val, data: createDataArray() }";
                            options.series.push(series);
                        }

                        var catIndex = indexOfCategory(options, fixRecordName(records[1], i));
                        //vv if (records[2].data[i]) series.data[catIndex] = records[2].data[i];
                    }

                }
                else if (records.Count == 2)
                {
                    List<string> list = new List<string>();

                    for (int i = 0; i < records[0].data.Count; i++)
                    {
                        list.Add(records[0].data[i].ToString());
                    }

                    xAxis = "{ categories: [" + (list.Any() ? string.Join(",", list.Select(x => string.Format("'{0}'", x)).ToArray()) : "") + "]}";

                    string seriesFormat = "[{{ name: '" + records[1].name.ToString() + "', data: [{0}]}}]";
                    list = new List<string>();

                    for (int i = 0; i < records[1].data.Count; i++)
                    {
                        list.Add(records[1].data[i].ToString());
                    }

                    series = string.Format(seriesFormat, (list.Any() ? string.Join(",", list.ToArray()) : ""));
                }
            }

            if (!string.IsNullOrEmpty(series) && records[1].data.Count > 1000)
            {
                return "Your series has " + records[1].data.Count + " items. This might cause issues rendering the chart, please try filtering your data to less than 1000.";
            }

            string jsonOptionsAndData = JsonConvert.SerializeObject(options);

            if (options.ts.chartType == "stackedcolumn")
            {
                jsonOptionsAndData = jsonOptionsAndData.Substring(0, jsonOptionsAndData.Length - 1) + ", xAxis: " + xAxis + "}";
            }

            jsonOptionsAndData = jsonOptionsAndData.Substring(0, jsonOptionsAndData.Length - 1) + ", series: " + series + "}";

            return jsonOptionsAndData;
        }

        private static string fixRecordName(dynamic record, int index)
        {
            if (record.fieldType == "bool")
            {
                return record.name + (record.data[index] == true ? " = True" : " = False");
            }

            return record.data[index];
        }

        private static string fixBlankSeriesName(string val)
        {
            return string.IsNullOrEmpty(val) ? "Unknown" : val + "";
        }

        private static string createDataArray(dynamic options)
        {
            List<int> result = new List<int>();

            for (var i = 0; i < options.xAxis.categories.length; i++)
            {
                result.Add(0);
            }

            return result.ToString();
        }

        private static string findSeries(dynamic options, string value)
        {
            for (var i = 0; i < options.series.length; i++)
            {
                if (options.series[i].value == value) return options.series[i];
            }

            return null;
        }

        private static int indexOfCategory(dynamic options, string name)
        {
            for (var i = 0; i < options.xAxis.categories.length; i++)
            {
                if (options.xAxis.categories[i] == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool ExecuteCommand(string command)
        {
            bool isSuccessful = false;
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            isSuccessful = exitCode == 0 && string.IsNullOrEmpty(error);

            return isSuccessful;
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
                Log(string.Format("Generating {0} Report", report.ReportDefType.ToString()), LogType.Both);

                string reportAttachmentFile = "";

                if (report.ReportDefType == ReportType.Chart)
                {
                    reportAttachmentFile = GetReportChartFile(scheduledReportCreator, scheduledReport, report);
                }
                else
                {
                    reportAttachmentFile = GetReportDataToFile(scheduledReportCreator, report, scheduledReport.Id, "", false, true, Logs);
                }

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
