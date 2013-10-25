/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var reportsPage = null;
$(document).ready(function () {
  reportsPage = new ReportsPage();
  reportsPage.refresh();

  var _hiddenVisible = false;

  $('.reports-refresh').click(
    function (e) {
      e.preventDefault();
      location = location;
    }
  );

  top.Ts.Services.Reports.GetReports(loadReports);

  $('.reports-list').on('click', '.action-view', function (e) {
    e.preventDefault();
    alert('in dev');
  });

  $('.reports-list').on('click', '.action-edit', function (e) {
    e.preventDefault();
    alert('in dev');
  });

  $('.reports-list').on('click', '.action-clone', function (e) {
    e.preventDefault();
    var item = $(this).parents('.report-item');
    var report = item.data('o');
    top.Ts.Services.Reports.CloneReport(report.ReportID, function (report) {
      if (report != null) {
        getNewReportItem(report).hide().insertAfter(item).fadeIn("slow");
      }
    });
  });

  $('.reports-list').on('click', '.action-export', function (e) {
    e.preventDefault();
    alert('export');
  });

  $('.reports-list').on('click', '.action-delete', function (e) {
    e.preventDefault();
    var item = $(this).parents('.report-item');
    var report = item.data('o');

    if (confirm("Are you sure you would like to delete '" + report.Name + "'?")) {
      top.Ts.Services.Reports.DeleteReport(report.ReportID, function () {
        item.fadeOut("slow", function () { $(this).remove(); });
      });
    }

  });

  $('.reports-list').on('click', '.action-hide', function (e) {
    e.preventDefault();
    var item = $(this).parents('.report-item');
    var report = item.data('o');
    var isHidden = item.hasClass('report-hidden');

    top.Ts.Services.Reports.SetHidden(report.ReportID, !isHidden, function () {
      if (isHidden) {
        item.removeClass('report-hidden');
        item.find('.action-hide').html('<i class="icon-fixed-width icon-ban-circle"></i> Hide');
      }
      else {
        item.find('.action-hide').html('<i class="icon-fixed-width icon-ban-circle"></i> Show');
        if (_hiddenVisible == true) {
          item.addClass('report-hidden').show();
        }
        else {
          item.fadeOut("slow", function () { $(this).addClass('report-hidden'); });
        }
      }
    });

  });

  $('.reports-list').on('click', '.action-favorite', function (e) {
    e.preventDefault();
    var item = $(this).parents('.report-item');
    var report = item.data('o');
    var isFavorite = item.find('.report-star i').hasClass('icon-star');
    top.Ts.Services.Reports.SetFavorite(report.ReportID, !isFavorite, function () {
      if (isFavorite) {
        item.find('.report-star i').removeClass('icon-star').addClass('icon-star-empty');
      }
      else {
        item.find('.report-star i').removeClass('icon-star-empty').addClass('icon-star');
      }
    });
  });

  $('.action-new').click(function (e) {
    e.preventDefault();
    window.location = "reports_edit.html";
  });

  $('.action-toggle-hidden').click(function (e) {
    e.preventDefault();
    _hiddenVisible = !_hiddenVisible;
    $('.report-hidden').toggle(_hiddenVisible);
    $(this).text(_hiddenVisible == true ? "Hide hidden reports" : "Show hidden reports");
  });

  function loadReports(reports) {
    $('.reports-list').empty();

    for (var i = 0; i < reports.length; i++) {
      $('.reports-list').append(getNewReportItem(reports[i]));
    }
  }

  function getNewReportItem(report) {
    var item = $('<li>').data('o', report).addClass('report-item reportid-' + report.ReportID).html($('.reports-list-template li').html());
    if (report.IsHidden == true) {
      item.addClass('report-hidden');
      item.find('.action-hide').html('<i class="icon-fixed-width icon-ban-circle"></i> Show');
    }
    item.find('.report-name').text(report.Name);
    item.find('.report-star i').addClass(report.IsFavorite == true ? 'icon-star' : 'icon-star-empty');

    item.find('.report-description').html(report.Description == null ? '&nbsp' : report.Description);

    if (report.Description == null && report.OrganizationID == null) {
      item.find('.report-description').html('This is a stock report.');
    }

    switch (report.ReportType) {
      case 1: item.find('.report-type i').addClass('icon-bar-chart'); break;
      case 2: item.find('.report-type i').addClass('icon-globe'); break;
      case 3: item.find('.report-type i').addClass('icon-wrench'); break;
      default: item.find('.report-type i').addClass('icon-table');
    }

    if ((top.Ts.System.User.IsSystemAdmin == false && report.CreatorID != top.Ts.System.User.UserID)) {
      item.find('.action-edit').parent().addClass('disabled');
      item.find('.action-delete').parent().addClass('disabled');
    }

    if (report.ReportType == 3) {
      item.find('.action-edit').parent().addClass('disabled');
      item.find('.action-clone').parent().addClass('disabled');
    }
    return item;
  }

});



function onShow() {
  reportsPage.refresh();
};


ReportsPage = function () {
  $('.loading-section').hide().next().show();
};


ReportsPage.prototype = {
  constructor: ReportsPage,
  refresh: function () {

  }
};
