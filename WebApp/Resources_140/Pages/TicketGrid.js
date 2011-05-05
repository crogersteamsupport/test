/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var ticketGrid = null;
$(document).ready(function () {
  ticketGrid = new TicketGrid();

  top.Ts.Services.Settings.ReadUserSetting('TicketGrid-sort-' + window.location.search, 'TicketNumber|false', function (result) {
    var values = result.split('|');
    ticketGrid._loader.setSort(values[0], values[1] === "true");
    ticketGrid.refresh();
  });

});

function onShow() {
  ticketGrid.refresh();
}


TicketGrid = function () {
  var ticketLoadFilter = top.Ts.Utils.queryToTicketFilter(window);
  var self = this;
  var grid = null;
  var preview = $('iframe');
  var i = 0;
  var j = 0;
  this._currentTicket = null;
  var loadingIndicator = null;
  $('head').append(top.Ts.MainPage.getCalcStyle());

  this._layout = $('.grid-ticket-layout').layout({
    resizeNestedLayout: true,
    defaults: {
      spacing_open: 5,
      closable: false
    },
    center: { paneSelector: ".grid-ticket-container",
      onresize: resizeGrid,
      triggerEventsOnLoad: false
    },
    north: { paneSelector: ".grid-ticket-toolbar", size: 31, spacing_open: 0, resizable: false },
    south: { spacing_open: 5, paneSelector: ".grid-ticket-preview", size: 225, closable: false }
  });

  var layout = this._layout;

  function resizeGrid(paneName, paneElement, paneState, paneOptions, layoutName) {
    if (loadingIndicator) {
      loadingIndicator.remove();
      loadingIndicator = null;
    }
    try {
      var vp = grid.getViewport();
      var t = vp.top;
      loader.clear();
      loader.ensureData(vp.top, vp.bottom, function () {
        if (t > 10) grid.scrollRowIntoView(t+10, false);
        grid.resizeCanvas();
      });
    } catch (e) {
      alert(e.message);
    }

  }

  function addToolbarButton(id, icon, caption, callback) {
    var html = '<a href="#" id="' + id + '" class="ts-toolbar-button ui-corner-all"><span class="ts-toolbar-icon ts-icon ' + icon + '"></span><span class="ts-toolbar-caption">' + caption + '</span></a>';
    $('.grid-ticket-toolbar').append(html).find('#' + id).click(callback).hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });
  }

  addToolbarButton('btnNew', 'ts-icon-new', 'New', function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.MainPage.newTicket();
  });

  addToolbarButton('btnDelete', 'ts-icon-delete', 'Delete', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var ticket = getSelectedTicket();
    if (confirm('Are you sure you would like to delete Ticket ' + ticket.TicketNumber + '?')) {
      top.top.Ts.Services.Tickets.DeleteTicket(ticket.TicketID, function () { self.refresh(); });
    }
  });

  /*
  addToolbarButton('btnSubscribe', 'ts-icon-subscribed', 'Subscribe', function (e) {
  e.preventDefault();
  e.stopPropagation();
  var ticket = getSelectedTicket();
  top.top.Ts.Services.Tickets.Subscribe(ticket.TicketID, function (result) {
  if (result == false) {
  alert('You are now unsubscribed to Ticket ' + ticket.TicketNumber + '.');
  }
  else {
  alert('You are now subscribed to Ticket ' + ticket.TicketNumber + '.');
  }

  });
  });*/

  addToolbarButton('btnEnqueue', 'ts-icon-enqueue', 'Enqueue', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var ticket = getSelectedTicket();
    top.Ts.Services.Tickets.Enqueue(ticket.TicketID, function () { });
  });

  addToolbarButton('btnTakeOwnership', 'ts-icon-takeownership', 'Take Ownership', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var ticket = getSelectedTicket();
    top.Ts.Services.Tickets.TakeOwnership(ticket.TicketID, function () { self.refresh(); });
  });

  addToolbarButton('btnRequestUpdate', 'ts-icon-request', 'Request Update', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var ticket = getSelectedTicket();
    top.Ts.Services.Tickets.RequestUpdate(ticket.TicketID, function () { alert('You have requested an update for Ticket ' + ticket.TicketNumber + '.'); });

  });

  addToolbarButton('btnExport', 'ts-icon-export', 'Export', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var s = JSON.stringify(ticketLoadFilter);
    window.open('dc/1078/ticketexport?filter=' + encodeURIComponent(s));
  });

  addToolbarButton('btnHistory', 'ts-icon-history', 'History', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var wnd = top.GetHistoryDialog(17, getSelectedTicket().TicketID);
    wnd.show();
  });

  addToolbarButton('btnRefresh', 'ts-icon-refresh', 'Refresh', function (e) {
    e.preventDefault();
    e.stopPropagation();
    self.refresh();
  });


  var data = [];
  this._loader = new TicketGridModel(ticketLoadFilter);
  var loader = this._loader;

  var slaTicketColumnFormatter = function (row, cell, value, columnDef, dataContext) {
    var min = dataContext["SlaViolationTime"];
    if (min) {
      if (min < 0)
        return '<span class="ticket-grid-cell-sla-text">' + Math.round(min / 60) + ' hours</span>';
      else
        return Math.round(min / 60) + ' hours';
    }
    return "";

  };

  var openTicketColumnFormatter = function (row, cell, value, columnDef, dataContext) {
    return '<span class="ts-icon ts-icon-open" title="Click to open this ticket"></span>';
  };

  var isReadColumnFormatter = function (row, cell, value, columnDef, ticket) {
    return value == false ? '<span class="ts-icon ts-icon-read" title="Click to mark this ticket as read"></span>' : '<span class="ts-icon ts-icon-unread" title="Click to mark this ticket as unread"></span>'
  };

  var isFlaggedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
    return value == false ? '<span class="ts-icon ts-icon-unflagged" title="Click to flag this ticket for follow up"></span>' : '<span class="ts-icon ts-icon-flagged" title="Click to unflag this ticket"></span>'
  };

  var isSubscribedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
    return value == false ? '<span class="ts-icon ts-icon-unsubscribed" title="Click to subscribe to this ticket"></span>' : '<span class="ts-icon ts-icon-subscribed" title="Click to unsubscribe to this ticket"></span>'
  };

  var dateTicketColumnFormatter = function (row, cell, value, columnDef, dataContext) {
    //return dataContext[columnDef.id].localeFormat(top.Ts.System.Culture.DateTimePattern.ShortDateShortTime);
    //alert(top.Ts.Utils.getDateTimePattern());
    return dataContext[columnDef.id].localeFormat(top.Ts.Utils.getDateTimePattern());
  };

  var columns = [
  	{ id: "openButton", name: "", width: 24, formatter: openTicketColumnFormatter, unselectable: true, resizable: false, sortable: false, cssClass: 'ticket-grid-cell-sla' },
    { id: "IsRead", name: "", field: "IsRead", width: 24, sortable: true, formatter: isReadColumnFormatter, unselectable: true, resizeable: false },
    { id: "IsFlagged", name: "", field: "IsFlagged", width: 24, sortable: true, formatter: isFlaggedColumnFormatter, unselectable: true, resizeable: false },
    { id: "IsSubscribed", name: "", field: "IsSubscribed", width: 24, sortable: true, formatter: isSubscribedColumnFormatter, unselectable: true, resizeable: false },
    { id: "TicketNumber", name: "Number", field: "TicketNumber", width: 75, sortable: true, cssClass: 'ticket-grid-cell-ticketnumber' },
    { id: "Name", name: "Name", field: "Name", width: 200, sortable: true },
    { id: "TicketTypeName", name: "Type", field: "TicketTypeName", width: 125, sortable: true },
    { id: "Status", name: "Status", field: "Status", width: 125, sortable: true },
    { id: "Severity", name: "Severity", field: "Severity", width: 125, sortable: true },
    { id: "UserName", name: "Assigned To", field: "UserName", width: 125, sortable: true },
    { id: "Customers", name: "Customers", field: "Customers", width: 125, sortable: true },
    { id: "Contacts", name: "Contacts", field: "Contacts", width: 125, sortable: true },
    { id: "ProductName", name: "Product", field: "ProductName", width: 150, sortable: true },
    { id: "ReportedVersion", name: "Reported", field: "ReportedVersion", width: 100, sortable: true },
    { id: "SolvedVersion", name: "Resolved", field: "SolvedVersion", width: 100, sortable: true },
    { id: "GroupName", name: "Group", field: "GroupName", width: 125, sortable: true },
    { id: "DateModified", name: "Last Modified", field: "DateModified", width: 150, sortable: true, formatter: dateTicketColumnFormatter },
    { id: "DateCreated", name: "Date Opened", field: "DateCreated", width: 150, sortable: true, formatter: dateTicketColumnFormatter },
    { id: "DaysOpened", name: "Days Opened", field: "DaysOpened", width: 100, sortable: true },
    { id: "IsClosed", name: "Closed", field: "IsClosed", width: 75, sortable: true },
    { id: "CloserName", name: "Closed By", field: "CloserName", width: 125, sortable: true },
    { id: "SlaViolationTime", name: "SLA Violation Time", field: "SlaViolationTime", width: 125, sortable: true, formatter: slaTicketColumnFormatter }
	];

  var options = {
    rowHeight: 22,
    editable: false,
    enableAddRow: false,
    enableCellNavigation: true,
    multiSelect: false,
    enableColumnReorder: false,
    rowCssClasses: function (row) {
      var result = 'ticket-grid-row';
      if (row) {
        if (row['SlaWarningTime'] && row['SlaWarningTime'] < 0) {
          result = result + ' ticket-grid-row-violated';
        }
        else if (row['SlaViolationTime'] && row['SlaViolationTime'] < 0) {
          result = result + ' ticket-grid-row-warning';
        }
        if (row['IsRead'] && row['IsRead'] === true) result = result + ' ticket-grid-row-read';
      }
      return result;
    }
  };

  var loadingTimer = null;

  $(layout.panes.center).disableSelection();
  this._grid = new Slick.Grid(layout.panes.center, loader.data, columns, options);
  grid = this._grid;
  $('.ticket-grid-header-flagged .slick-column-name').addClass('ts-icon ts-icon-flagged');
  $('.ticket-grid-header-read .slick-column-name').addClass('ts-icon ts-icon-read');

  grid.onViewportChanged = function () {
    var vp = grid.getViewport();
    loader.ensureData(vp.top, vp.bottom);

  };

  grid.onClick = function (e, row, cell) {
    switch (columns[cell].id) {
      case "IsRead":
        var ticket = loader.data[row];
        ticket.IsRead = !ticket.IsRead;
        top.Ts.Services.Tickets.SetTicketRead(ticket.TicketID, ticket.IsRead);
        grid.updateRow(row);
        if (ticket.IsRead) {
          $('.slick-row[row="' + row + '"]').addClass('ticket-grid-row-read');
        }
        else {
          $('.slick-row[row="' + row + '"]').removeClass('ticket-grid-row-read');
        }
        return true;
      case "IsFlagged":
        var ticket = loader.data[row];
        ticket.IsFlagged = !ticket.IsFlagged;
        top.Ts.Services.Tickets.SetTicketFlag(ticket.TicketID, ticket.IsFlagged);
        grid.updateRow(row);
        return true;
      case "IsSubscribed":
        var ticket = loader.data[row];
        ticket.IsSubscribed = !ticket.IsSubscribed;
        top.Ts.Services.Tickets.SetSubscribed(ticket.TicketID, ticket.IsSubscribed);
        grid.updateRow(row);
        return true;
      case "openButton":
        top.Ts.MainPage.openTicket(loader.data[row].TicketNumber);
        grid.updateRow(row);

        return true;

      default:

    }

    return false;
  }

  grid.onSort = function (sortCol, sortAsc) {
    top.Ts.Services.Settings.WriteUserSetting('TicketGrid-sort-' + window.location.search, sortCol.field + '|' + sortAsc);
    loader.setSort(sortCol.field, sortAsc);
    var vp = grid.getViewport();
    loader.ensureData(vp.top, vp.bottom);
  };

  grid.onDblClick = function (e, row, cell) {
    top.Ts.MainPage.openTicket(loader.data[row].TicketNumber, true);
  };

  grid.onSelectedRowsChanged = function () {
    //    selectedRowIds = [];
    //    var rows = grid.getSelectedRows();
    //    for (var i = 0, l = rows.length; i < l; i++) {
    //    var ticket = loader.data[rows[i]];
    //    alert(item);
    //    if (item) selectedRowIds.push(item.id);
    //    }

    var rows = grid.getSelectedRows();
    if (rows.length > 0) {
      var ticket = loader.data[rows[0]];
      if (!ticket) {
        var vp = grid.getViewport();
        loader.ensureData(vp.top, vp.bottom);
        clearPreview();
      }
      else {
        previewTicket(ticket);
      }
    }
    else {
      clearPreview();
    }
  };

  function getSelectedTicket() {
    var rows = grid.getSelectedRows();
    if (rows.length > 0) {
      return loader.data[rows[0]];
    }
    else {
      return null;
    }

  }

  function clearPreview() {
    if (preview[0].contentWindow.clearHtml) preview[0].contentWindow.clearHtml();
  }

  function previewTicket(ticket) {
    if (ticket == null) {
      clearPreview();
      return;
    }

    function writeProp(name, val, colSpan) {
      if (val == null || val == '') val = '[Unassigned]';
      if (colSpan == null) colSpan = 0;
      return '<td' + (colSpan > 0 ? ' colspan="' + colSpan + '">' : '>') + '<span class="ticket-preview-property-name">' + name + ':</span><span class="ticket-preview-property-value">' + val + '</span></td>'
    }
    var html = '<div class="ticket-preview-wrapper ui-widget"><div class="ticket-preview-main"><div class="ticket-preview-header ui-widget-content"><h1>' + ticket.TicketNumber + ': ' + ticket.Name + '</h1>';
    html = html + '<table class="ticket-preview-properties ui-widget-content"><tr>';
    html = html + writeProp('Ticket Type', ticket.TicketTypeName);
    html = html + writeProp('Status', ticket.Status);
    html = html + writeProp('Severity', ticket.Severity);
    html = html + writeProp('Assigned To', ticket.UserName);
    html = html + '</tr><tr>';
    html = html + writeProp('Group', ticket.GroupName);
    html = html + writeProp('Product', ticket.ProductName);
    html = html + writeProp('Version Reported', ticket.ReportedVersion);
    html = html + writeProp('Version Resolved', ticket.SolvedVersion);
    html = html + '</tr>';

    var timLoading = setTimeout(function () { preview[0].contentWindow.writeHtml('<div class="ticket-preview-loading ts-loading"></div>'); timLoading = null; }, 500);
    top.Ts.Services.Tickets.GetContactsAndCustomers(ticket.TicketID, function (customers) {
      if (customers.length > 0) {
        html = html + '<tr>'
        var s = '';
        for (var i = 0; i < customers.length; i++) {
          if (i !== 0) s = s + ', ';
          s = s + customers[i];
        }
        html = html + writeProp('Customers', s, 4);
        html = html + '</tr>'
      }
      html = html + '</table></div>';

      top.Ts.Services.Tickets.GetActions(ticket.TicketID, function (actions) {
        if (timLoading) clearTimeout(timLoading);
        html = html + '<div class="ticket-preview-actions ui-widget-content">';
        for (var i = 0; i < actions.length; i++) {
          html = html + '<div class="ticket-preview-action">';
          html = html + '<h1 class="ui-widget-header ui-corner-all">' + actions[i].ActionType + '</h1>';
          html = html + '<p>' + actions[i].Description + '</p>';
          html = html + '<div><strong>Knowledge Base: </strong>' + actions[i].IsKnowledgeBase + ' &nbsp&nbsp&nbsp&nbsp <strong>Visible on Portal: </strong>' + actions[i].IsVisibleOnPortal + '</div>';
          html = html + '<div>';
          if (actions[i].CreatorName) html = html + '<span class="ticket-preview-action-author">' + actions[i].CreatorName + '</span> - ';
          html = html + '<span class="ticket-preview-action-date">' + actions[i].DateCreated.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.FullDateTimePattern) + '</span></div>';




          html = html + '</div>';
        }
        html = html + '</div></div></div>';
        preview[0].contentWindow.writeHtml(html);

      });

    });
  }


  loader.onDataLoading.subscribe(function () {
    if (!loadingIndicator) {
      loadingIndicator = $("<div class='grid-loading'><label></div>").appendTo(document.body);
      loadingIndicator.position({ my: "center center", at: "center center", of: layout.panes.center, collision: "none" });
    }

    loadingIndicator.show();

    //if (loadingTimer !== null) clearTimeout(loadingTimer);    loadingTimer = setTimeout(loadingIndicator.fadeOut(), 10000);
  });

  loader.onDataLoaded.subscribe(function (args) {
    for (var i = args.from; i < args.to; i++) { grid.removeRow(i); }
    grid.updateRowCount();
    grid.render();

    parent.setGridCount(loader.data.length);
    grid.setSortColumn(ticketLoadFilter.SortColumn, ticketLoadFilter.SortAsc);
    //$(element).parents('.tickets-layout').find('.tickets-panel-footer .ticket-grid-count').text();
    //if (loadingTimer !== null) { clearTimeout(loadingTimer); loadingTimer = null; }
    loadingIndicator.fadeOut();
    if (self._currentTicket == null) {
      self._currentTicket = getSelectedTicket();
      previewTicket(self._currentTicket);
      if (self._currentTicket == null) top.Ts.MainPage.addDebugStatus('null'); else top.Ts.MainPage.addDebugStatus(self._currentTicket.TicketNumber);
    }
  });


};


TicketGrid.prototype = {
  constructor: TicketGrid,
  refresh: function () {
    this._currentTicket = null;
    //this._loader.clear();
    this._layout.resizeAll();
    //this._grid.resizeCanvas();
    //this._grid.onViewportChanged();
  }

};

TicketGridModel = function (ticketLoadFilter) {
  // private
  var data = { length: 0 };
  var total = -1;
  var h_request = null;
  var req = null; // ajax request
  var req_page;
  // events
  var onDataLoading = new top.Ts.Utils.EventHandler();
  var onDataLoaded = new top.Ts.Utils.EventHandler();


  function init() {
  }


  function isDataLoaded(from, to) {
    for (var i = from; i <= to; i++) {
      if (data[i] == undefined || data[i] == null)
        return false;
    }

    return true;
  }


  function clear() {
    for (var key in data) {
      delete data[key];
    }
    data.length = 0;
  }

  function reloadData(from, to) {
    for (var i = from; i <= to; i++)
      delete data[i];

    ensureData(from, to);
  }

  function ensureData(from, to, loadedCallback) {
    to = to - 1;
    if (req) {
      req.get_executor().abort();
      for (var i = req.from; i <= req.to; i++)
        data[i] = undefined;
    }

    if (from < 0) { from = 0; }
    while (data[from] !== undefined && from < to) { from++; }
    while (data[to] !== undefined && from < to) { to--; }
    if (from >= to) { return; }


    if (h_request != null)
      clearTimeout(h_request);

    h_request = setTimeout(function () {
      for (var i = from; i < to - 1; i++) data[i] = null; // null indicates a 'requested but not available yet'

      onDataLoading.notify({ from: from, to: to });
      req = top.Ts.Services.Tickets.GetTicketRange(from, to, ticketLoadFilter, function (ticketRange) {
        onSuccess(ticketRange);
        if (loadedCallback) loadedCallback();
      },
      function (error) {
        onError(error);
        if (loadedCallback) loadedCallback();
      });
      req.from = from;
      req.to = to;

    }, 50);
  }


  function onError(error) {
    //alert(error.get_message());
  }

  function onSuccess(ticketRange) {
    if (total > -1 && total != ticketRange.Total) {
      clear();
    } else {
      total = ticketRange.Total;
    }
    data.length = parseInt(ticketRange.Total);
    for (var i = 0; i < ticketRange.Tickets.length; i++) {
      data[ticketRange.From + i] = ticketRange.Tickets[i];
      data[ticketRange.From + i].index = ticketRange.From + i;
    }

    req = null;

    onDataLoaded.notify({ from: ticketRange.From, to: (ticketRange.From + ticketRange.Tickets.length) });
  }




  function setSort(column, asc) {
    ticketLoadFilter.SortColumn = column;
    ticketLoadFilter.SortAsc = asc;
    clear();
  }

  function setSearch(str) {
    searchstr = str;
    clear();
  }


  init();

  return {
    // properties
    "data": data,

    // methods
    "clear": clear,
    "isDataLoaded": isDataLoaded,
    "ensureData": ensureData,
    "reloadData": reloadData,
    "setSort": setSort,
    "setSearch": setSearch,

    // events
    "onDataLoading": onDataLoading,
    "onDataLoaded": onDataLoaded
  };
}


