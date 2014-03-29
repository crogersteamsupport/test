
$(document).ready(function () {
    var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
    var _report = null;
    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: 'body' });

    top.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
        _report = report;
        if ((top.Ts.System.User.IsSystemAdmin == false && report.CreatorID != top.Ts.System.User.UserID) || report.OrganizationID == null) { $('.reports-edit').remove(); }

        if (report.IsFavorite) {
            $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
        }

        $('#mainFrame').attr('src', report.ReportDef);
        $('.report-title').text(report.Name);
    });

    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        window.location.assign(window.location.href);
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



    $('body').layout({
        resizeNestedLayout: true,
        maskIframesOnResize: true,
        defaults: {
            spacing_open: 0,
            closable: false
        },
        north: {
            spacing_open: 1,
            size: 100
        },
        center: {
            maskContents: true
        }
    });

});


