$(document).ready(function () {
    var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
    var _report = null;

    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: 'body' });

    function loadChart() {
        top.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
            _report = report;
            _report.Def = JSON.parse(report.ReportDef);

            if (report.IsFavorite) {
                $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
            }

            $('#mainFrame').attr('src', report.ReportDef);
            $('.report-title').text(report.Name);


            top.Ts.Utils.webMethod("ReportService", "GetChartReportData",
              { "reportID": _reportID },
              function (data) {
                  data = JSON.parse(data);
                  _report.Def.Chart.series = data.Series;
                  _report.Def.Chart.xAxis = { categories: data.Categories };
                  $('.chart-container').highcharts(_report.Def.Chart);
              },
              function (error) {
                  alert(error.get_message());
              });

        });
    }

    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        loadChart();
    });

    $('.reports-edit').click(function (e) {
        e.preventDefault();
        window.location.assign("reports_edit.html?ReportID=" + _reportID);
    });

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        top.Ts.Utils.webMethod("ReportService", "SetFavorite", { "reportID": _reportID, "value": _report.IsFavorite }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star'); else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });

    loadChart();

});
