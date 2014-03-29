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
            if ((top.Ts.System.User.IsSystemAdmin == false && report.CreatorID != top.Ts.System.User.UserID) || report.OrganizationID == null) { $('.reports-edit').remove(); }


            top.Ts.Utils.webMethod(null, "reportdata/chart",
              { "reportID": _reportID },
              function (data) {
                  
                  createChart('.chart-container', _report.Def.Chart, data);
              },
              function (error) {
                  showChartError('.chart-container', error.statusText);
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

    $('.reports-export-png').click(function (e) {
        e.preventDefault();
        exportChart('image/png');
    });

    $('.reports-export-jpg').click(function (e) {
        e.preventDefault();
        exportChart('image/jpeg');
    });

    $('.reports-export-svg').click(function (e) {
        e.preventDefault();
        exportChart('image/svg+xml');
    });

    function exportChart(exportType) {
        var options = { type: exportType, url: '../../../chartexport', filename: '"' + _report.Name + '"' , width: 1900 };
        $('.chart-container').highcharts().exportChart(options);
    }

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        top.Ts.Utils.webMethod("ReportService", "SetFavorite", { "reportID": _reportID, "value": _report.IsFavorite }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star'); else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });

    loadChart();

});
