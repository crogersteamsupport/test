
$(document).ready(function () {
  var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
  var _report = null;

  top.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
    _report = report;

    if (report.IsFavorite) {
      $('.reports-extview-fav i').removeClass('fa-star-o').addClass('fa-star');
    }

    $('#mainFrame').attr('src', report.ReportDef);
  });

  $('.reports-extview-refresh').click(function (e) {
    e.preventDefault();
    window.location.assign(window.location.href);
  });

  $('.reports-extview-edit').click(function (e) {
    e.preventDefault();
    window.location.assign("reports_edit.html?ReportID=" + _reportID);
  });

  $('.reports-extview-back').click(function (e) {
    e.preventDefault();
    window.location.assign('reports.html');
  });

  $('.reports-extview-fav').click(function (e) {
    e.preventDefault();
    _report.IsFavorite = !_report.IsFavorite;
    top.Ts.Utils.webMethod("ReportService", "SetFavorite", { "reportID": _reportID, "value": _report.IsFavorite }, function () {
      if (_report.IsFavorite) $('.reports-extview-fav i').removeClass('fa-star-o').addClass('fa-star'); else $('.reports-extview-fav i').removeClass('fa-star').addClass('fa-star-o');
    });
  });



  $('body').layout({
    north: {
      size: 60
    },
    center: {
      maskContents: true
    }
  });

});


