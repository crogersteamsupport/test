using System;
using System.Web.Services;
using TeamSupport.Data;
using System.Web.UI;

public partial class ScheduledReportChartGenerate : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        test.Text = string.Format("parameters received: ReportID {0}, ScheduledReportID {1}", Request["ReportID"], Request["ScheduledReportID"]);
    }

    [WebMethod]
    public static ReportItem GetReport(int scheduledReportId)
    {
        ScheduledReport scheduledReport = ScheduledReports.GetScheduledReport(LoginUser.Anonymous, scheduledReportId);
        LoginUser loginUser = new LoginUser(scheduledReport.CreatorId, scheduledReport.OrganizationId, null);
        Report report = Reports.GetReport(loginUser, scheduledReport.ReportId, loginUser.UserID);
        report.MigrateToNewReport();

        return new ReportItem(report, true);
    }
}