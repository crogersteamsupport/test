<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ScheduledReportChartGenerate.aspx.cs" Inherits="ScheduledReportChartGenerate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <script src="vcr/1_9_0/Js/jquery-latest.min.js" type="text/javascript"></script>
    <script src="vcr/1_9_0/Js/Ts/ts.utils.js" type="text/javascript"></script>
    <script src="vcr/1_9_0/Js/highcharts.js" type="text/javascript"></script>
    <script src="vcr/1_9_0/Js/HighChartModules/exporting.js" type="text/javascript"></script>
    <script src="vcr/1_9_0/Pages/ScheduledReports_Chart.js" type="text/javascript"></script>
    <script src="vcr/1_9_0/Pages/ReportCharts.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">    
        $(document).ready(function () {
            var _reportID = parent.Ts.Utils.getQueryValue('ReportID', window);
            var _scheduledReportID = parent.Ts.Utils.getQueryValue('ScheduledReportID', window);
            var _report = null;

            PageMethods.GetReport(_scheduledReportID, function (report) {
                _report = report;
                _report.Def = JSON.parse(report.ReportDef);
                var params = {
                    "reportID": _reportID,
                    "scheduledreportid": _scheduledReportID
                };
                $.ajax({
                    type: "POST",
                    url: "/reportdata/chart",
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (reportData) {
                        createChart('.chart-container', _report.Def.Chart, reportData);
                    },
                    error: function (xhr, status, error) { }
                });
            },
            function (error) {
                alert(error);
            });

            var username = '13679';
            var password = 'e4fd0a31-8c8b-494a-aea6-7d24ca3d3528';
            usename = '1007010';
            password = 'beb57627-1be2-4ed8-8ade-3478d04ee942';

            $.ajax({
                url: "https://app.na2.teamsupport.com/api/json/Tickets/993",
                type: "GET",
                headers: {
                    "Authorization": "Basic " + btoa(username + ":" + password)
                },
                success: function (data) {
                    alert(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(thrownError);
                }
            });
        });        
    </script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManagerMain"
            runat="server"
            EnablePageMethods="true" 
            ScriptMode="Release" 
            LoadScriptsBeforeUI="true">
        </asp:ScriptManager>
        <asp:Literal ID="test" runat="server"></asp:Literal>
    </form>
    <div class="container frame-container">
        <div class="chart-container">
        </div>
    </div>
</body>
</html>
