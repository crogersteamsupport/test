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
                  updateSeriesOptions(_report.Def.Chart, JSON.parse(data));
                  $('.chart-container').highcharts(_report.Def.Chart);
              },
              function (error) {
                  alert(error.get_message());
              });

        });
    }

    function updateSeriesOptions(options, data) {
        if (options.ts.chartType == 'pie') {
            var total = 0;
            for (var i = 0; i < data.Series[0].data.length; i++) {
                total += data.Series[0].data[i];
            }

            options.series = [{ type: 'pie', name: options.ts.seriesTitle, data: []}];

            for (var i = 0; i < data.Categories.length; i++) {
                var val = data.Series[0].data[i] / total * 100;
                options.series[0].data.push([data.Categories[i], parseFloat(val.toFixed(2))]);
            }
        }
        else {
            options.series = data.Series;
            options.xAxis = { categories: data.Categories };
        }
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
