

var ticketGrid = null;
$(document).ready(function () {
    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: '.grid-ticket-toolbar', animation: false });

    top.Ts.Services.Settings.ReadUserSetting('TicketGrid-Settings', '', function (result) {
        var options = result == '' ? new Object() : JSON.parse(result);
        ticketGrid = new TicketGrid(options);

        top.Ts.Services.Settings.ReadUserSetting('TicketGrid-sort-' + window.location.search, 'DateModified|false', function (result) {
            var values = result.split('|');
            ticketGrid._loader.setSort(values[0], values[1] === "true");
            ticketGrid._grid.setSortColumn(values[0], values[1] === "true");
            ticketGrid.refresh();
        });
    });

    

});

function onShow() {
    ticketGrid.refresh();
}




TicketGrid = function (options) {
    var ticketLoadFilter = top.Ts.Utils.queryToTicketFilter(window);
    var self = this;
    var grid = null;
    var preview = $('iframe');
    var i = 0;
    var j = 0;
    this._currentTicket = null;
    var loadingIndicator = null;
    var tmrDelayIndicator = null;
    var tmrHideLoading = null;
    this.defaults = { "isCompact": false };
    this.options = $.extend({}, this.defaults, options);

    function saveOptions() {
        top.Ts.Services.Settings.WriteUserSetting('TicketGrid-Settings', JSON.stringify(self.options));
    }

    this.showLoadingIndicator = function (delay) {
        if (!delay) {
            if (!loadingIndicator) {
                loadingIndicator = $("<div class='grid-loading'><label></div>").appendTo(document.body);
                loadingIndicator.position({ my: "center center", at: "center center", of: layout.panes.center, collision: "none" });
            }
            if (loadingIndicator.is(':visible') != true) { loadingIndicator.show(); }
        }
        else {
            if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
            tmrDelayIndicator = setTimeout("ticketGrid.showLoadingIndicator()", delay);
        }
        if (tmrHideLoading) {
            clearTimeout(tmrHideLoading);
            tmrHideLoading = setTimeout(function () { self.hideLoadingIndicator(); }, 3000);
        }
    }

    this.hideLoadingIndicator = function () {
        tmrHideLoading = null;
        if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
        tmrDelayIndicator = null;
        if (loadingIndicator) loadingIndicator.fadeOut();
    }

    $('head').append(top.Ts.MainPage.getCalcStyle());

    this._layout = $('.grid-ticket-layout').layout({
        resizeNestedLayout: true,
        maskIframesOnResize: true,
        defaults: {
            spacing_open: 5,
            closable: false
        },
        center: { paneSelector: ".grid-ticket-container",
            onresize: resizeGrid,
            triggerEventsOnLoad: false,
            minSize: 500
        },
        north: {
            paneSelector: ".grid-ticket-toolbar",
            size: 65,
            spacing_open: 0,
            resizable: false
        },
        south: {
            spacing_open: 5,
            paneSelector: ".grid-ticket-preview",
            size: 225,
            closable: false
        }
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
            loader.ensureData(vp.top, vp.bottom + 50, function () {
                if (t > 10) grid.scrollRowIntoView(t + 10, false);
                grid.resizeCanvas();
                self.hideLoadingIndicator();
            });
        } catch (e) {
            alert(e.message);
        }
    }

    function refreshGrid() {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }

        var vp = grid.getViewport();
        loader.clear();
        loader.ensureData(vp.top, vp.bottom, function () {
            self.hideLoadingIndicator();
        });
    }


    $('.tickets-new').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.newTicket();
        top.Ts.System.logAction('Ticket Grid - New Ticket');
    });


    $('.tickets-own').click(function (e) {
        e.preventDefault();

        var ids = getSelectedIDs();
        if (ids.length > 1) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.TakeOwnerships(JSON.stringify(ids), function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Take Ownership');
        }
        else {
            var ticket = getActiveTicket();
            top.Ts.Services.Tickets.TakeOwnership(ticket.TicketID, function () { refreshGrid(); });
            top.Ts.System.logAction('Ticket Grid - Take Ownership');
        }
    });
    $('.tickets-request').click(function (e) {
        e.preventDefault();

        var ids = getSelectedIDs();
        if (ids.length > 1) {
            top.Ts.Services.Tickets.RequestUpdate(JSON.stringify(ids), function () { alert('You have requested an update for ' + ids.length + ' selected tickets.'); });
            top.Ts.System.logAction('Ticket Grid - Request Update');
        }
        else {
            var ticket = getActiveTicket();
            top.Ts.Services.Tickets.RequestUpdate(ticket.TicketID, function () { alert('You have requested an update for Ticket ' + ticket.TicketNumber + '.'); });
            top.Ts.System.logAction('Ticket Grid - Request Update');
        }
    });


    $('.ticket-menu-actions li > a').click(function (e) {
        e.preventDefault();
        var el = $(this);
        var ids = getSelectedIDs();
        if (ids.length < 1) return;
        var data = JSON.stringify(ids);

        if (el.hasClass('ticket-action-read')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketReads(data, true, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Mark Read');
        }
        else if (el.hasClass('ticket-action-unread')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketReads(data, false, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Mark Unread');
        }
        else if (el.hasClass('ticket-action-user')) {
            var select = $('#assignUser');
            if (select.find('option').length < 1) {

                $('<option>')
                      .text('Unassigned')
                      .attr('value', -1)
                      .appendTo(select);

                var users = top.Ts.Cache.getUsers();

                for (var i = 0; i < users.length; i++) {
                    $('<option>')
                      .text(users[i].Name)
                      .attr('value', users[i].UserID)
                      .appendTo(select)
                }
            }


            $('#dialog-user').modal('show');
        }
        else if (el.hasClass('ticket-action-group')) {
            var select = $('#assignGroup');
            if (select.find('option').length < 1) {

                $('<option>')
                      .text('Unassigned')
                      .attr('value', -1)
                      .appendTo(select);
                var groups = top.Ts.Cache.getGroups();
                for (var i = 0; i < groups.length; i++) {
                    $('<option>')
                      .text(groups[i].Name)
                      .attr('value', groups[i].GroupID)
                      .appendTo(select)
                }
            }
            $('#dialog-group').modal('show');
        }
        else if (el.hasClass('ticket-action-severity')) {
            var select = $('#assignSeverity');
            if (select.find('option').length < 1) {
                var severities = top.Ts.Cache.getTicketSeverities();

                for (var i = 0; i < severities.length; i++) {
                    $('<option>')
                      .text(severities[i].Name)
                      .attr('value', severities[i].TicketSeverityID)
                      .appendTo(select)
                }
            }
            $('#dialog-severity').modal('show');
        }
        else if (el.hasClass('ticket-action-status')) {
            var form = $('#dialog-status .modal-body form');
            if (form.find('select').length < 1) {
                var ticketTypes = top.Ts.Cache.getTicketTypes();
                var statuses = top.Ts.Cache.getTicketStatuses();

                for (var i = 0; i < ticketTypes.length; i++) {
                    var ticketTypeID = ticketTypes[i].TicketTypeID;
                    var div = $('<div>')
                      .attr('data-tickettypeid', ticketTypeID)
                      .addClass('form-group');

                    $('<label>')
                      .text('Ticket status for ' + ticketTypes[i].Name)
                      .attr('for', 'ticketTypeStatus-' + ticketTypeID)
                      .appendTo(div);

                    var select = $('<select>')
                      .addClass('form-control')
                      .attr('id', 'ticketTypeStatus-' + ticketTypeID)
                      .data('tickettypeid', ticketTypeID)
                      .appendTo(div);

                    for (var j = 0; j < statuses.length; j++) {
                        if (statuses[j].TicketTypeID == ticketTypeID) {
                            $('<option>')
                              .text(statuses[j].Name)
                              .attr('value', statuses[j].TicketStatusID)
                              .appendTo(select);
                        }
                    }

                    form.append(div);
                }
            }

            form.find('.form-group').hide();
            ticketTypes = [];
            var rows = grid.getSelectedRows();
            for (var i = 0, l = rows.length; i < l; i++) {
                var ticket = loader.data[rows[i]];
                if (ticket) {
                    if (ticketTypes.indexOf(ticket.TicketTypeID) < 0) {
                        ticketTypes.push(ticket.TicketTypeID);
                    }
                }
            }

            for (var i = 0; i < ticketTypes.length; i++) {
                form.find('.form-group[data-tickettypeid="' + ticketTypes[i] + '"]').show();
            }


            $('#dialog-status').modal('show');
        }
        else if (el.hasClass('ticket-action-flag')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketFlags(data, true, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Mark Flagged');
        }
        else if (el.hasClass('ticket-action-unflag')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketFlags(data, false, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Mark Unflagged');
        }
        else if (el.hasClass('ticket-action-subscribe')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketSubcribes(data, true, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Subscribed');
        }
        else if (el.hasClass('ticket-action-unsubscribe')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetTicketSubcribes(data, false, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Unsubscribed');
        }
        else if (el.hasClass('ticket-action-enqueue')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetUserQueues(data, true, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Enqueued');
        }
        else if (el.hasClass('ticket-action-dequeue')) {
            self.showLoadingIndicator();
            top.Ts.Services.Tickets.SetUserQueues(data, false, function () { refreshGrid(); deselectRows(); });
            top.Ts.System.logAction('Ticket Grid - Dequeued');
        }
    });

    $('.tickets-save-user').click(function (e) {
        e.preventDefault();
        $('#dialog-user').modal('hide');
        self.showLoadingIndicator();
        var ids = getSelectedIDs();
        var val = $('#assignUser').val() == -1 ? null : $('#assignUser').val();
        top.Ts.Services.Tickets.SetTicketsUser(JSON.stringify(ids), val, function () {
            top.Ts.System.logAction('Ticket Grid - Updated user');
            refreshGrid();
        });
        deselectRows();
    });

    $('.tickets-save-group').click(function (e) {
        e.preventDefault();
        $('#dialog-group').modal('hide');
        self.showLoadingIndicator();
        var ids = getSelectedIDs();
        var val = $('#assignGroup').val() == -1 ? null : $('#assignGroup').val();
        top.Ts.Services.Tickets.SetTicketsGroup(JSON.stringify(ids), val, function () {
            top.Ts.System.logAction('Ticket Grid - Updated group');
            refreshGrid();
        });
        deselectRows();
    });

    $('.tickets-save-severity').click(function (e) {
        e.preventDefault();
        $('#dialog-severity').modal('hide');
        self.showLoadingIndicator();
        var ids = getSelectedIDs();
        top.Ts.Services.Tickets.SetTicketsSeverity(JSON.stringify(ids), $('#assignSeverity').val(), function () {
            top.Ts.System.logAction('Ticket Grid - Updated severity');
            refreshGrid();
        });
        deselectRows();
    });

    $('.tickets-save-status').click(function (e) {
        e.preventDefault();
        $('#dialog-status').modal('hide');
        self.showLoadingIndicator();
        var ids = getSelectedIDs();
        var statuses = [];

        $('#dialog-status .modal-body form select').each(function () {
            statuses.push($(this).val());
        });
        top.Ts.Services.Tickets.SetTicketsStatus(JSON.stringify(ids), JSON.stringify(statuses), function () {
            top.Ts.System.logAction('Ticket Grid - Updated statuses');
            refreshGrid();
        });
        deselectRows();
    });

    function deselectRows() {
        var cell = grid.getActiveCell();
        if (cell) {
            grid.setSelectedRows([cell.row]);
        }
        else {
            grid.setSelectedRows([]);
        }

        previewActiveTicket();
    }


    $('.tickets-export').click(function (e) {
        e.preventDefault();
        var s = JSON.stringify(ticketLoadFilter);
        window.open('../../../dc/1078/ticketexport?filter=' + encodeURIComponent(s));
        top.Ts.System.logAction('Ticket Grid - Export');
    });

    $('.tickets-delete').click(function (e) {
        e.preventDefault();
        var ids = getSelectedIDs();
        if (ids.length > 1) {
            if (confirm('Are you sure you would like to delete ' + ids.length + ' selected tickets?')) {
                top.Ts.System.logAction('Ticket Grid - Delete Tickets');
                self.showLoadingIndicator();
                top.top.Ts.Services.Tickets.DeleteTickets(JSON.stringify(ids), function () {
                    refreshGrid();
                    deselectRows();
                });
            }
        }
        else {
            var ticket = getActiveTicket();
            if (confirm('Are you sure you would like to delete Ticket ' + ticket.TicketNumber + '?')) {
                top.Ts.System.logAction('Ticket Grid - Delete Ticket');
                top.top.Ts.Services.Tickets.DeleteTicket(ticket.TicketID, function () { refreshGrid(); });
            }
        }
    });

    function getSelectedIDs() {
        selectedRowIds = [];
        var rows = grid.getSelectedRows();
        for (var i = 0, l = rows.length; i < l; i++) {
            var ticket = loader.data[rows[i]];
            if (ticket) selectedRowIds.push(ticket.TicketID);
        }
        return selectedRowIds;

    }

    $('.tickets-refresh').click(function (e) {
        e.preventDefault();
        refreshGrid();
        top.Ts.System.logAction('Ticket Grid - Refreshed');
    });


    $('#dialog-columns').modal({ show: false });
    $('#dialog-user').modal({ show: false });

    var _lastDialogColumnNo = 0;

    $('.tickets-columns').click(function (e) {
        e.preventDefault();
        _lastDialogColumnNo = 0;
        $('#dialog-columns').modal('show');
        $('.dialog-columns-forcefit input').prop('checked', grid.getOptions().forceFitColumns);
        $('.dialog-columns-compact input').prop('checked', self.options.isCompact && self.options.isCompact == true);
        $('.dialog-columns-list div.checkbox').remove();

        var gridColumns = grid.getColumns();
        for (var i = 0; i < gridColumns.length; i++) {
            addDialogColumn(gridColumns[i], true);
        }

        var availColumns = getAllColumns();
        for (var i = 0; i < availColumns.length; i++) {
            if (grid.getColumnIndex(availColumns[i].id) == null) {
                addDialogColumn(availColumns[i], false);
            }
        }
        $('.dialog-columns').dialog('open');
        top.Ts.System.logAction('Ticket Grid - Columns Adjusted');

    });

    function addDialogColumn(column, isChecked) {
        if (column.name == 'checked') return;
        if (column.name == 'move') return;
        var label = $('<label>').html('&nbsp;' + column.name);
        $('<input>').attr('type', 'checkbox').prop('checked', isChecked).data('o', column).data('col-no', _lastDialogColumnNo).prependTo(label);
        var div = $('<div>').addClass('checkbox').append(label)
        $('.dialog-columns-list .dialog-column-' + _lastDialogColumnNo % 3).append(div);
        _lastDialogColumnNo++;
    }

    $('.tickets-save-columns').click(function (e) {
        e.preventDefault();

        var columns = [];
        var list = $('.dialog-columns-list input:checked');
        list.sort(function (a, b) {
            return $(a).data('col-no') - $(b).data('col-no');
        });
        var i = 0;
        list.each(function () {
            columns.push($(this).data('o'));
            i++;

        });

        self.setTicketColumns(columns);

        if ($('.dialog-columns-forcefit input').prop('checked') == true) {
            grid.setOptions({ forceFitColumns: true });
            grid.autosizeColumns();
        } else {
            grid.setOptions({ forceFitColumns: false });
        }

        self.options.isCompact = $('.dialog-columns-compact input').prop('checked') == true;
        saveOptions();
        saveColumns();
        $('#dialog-columns').modal('hide');
        refreshGrid();
    });

    $('.tickets-default-columns').click(function (e) {
        e.preventDefault();
        _lastDialogColumnNo = 0;

        $('.dialog-columns-forcefit input').prop('checked', false);
        $('.dialog-columns-list div.checkbox').remove();

        var allColumns = getAllColumns();
        var defColumns = getDefaultColumns();

        for (var i = 0; i < defColumns.length; i++) {
            addDialogColumn(defColumns[i], true);
        }

        for (var i = 0; i < allColumns.length; i++) {
            var flag = false;
            for (var j = 0; j < defColumns.length; j++) {
                if (defColumns[j].id == allColumns[i].id) {
                    flag = true;
                    break;
                }
            }

            if (flag == false) { addDialogColumn(allColumns[i], false); }
        }
    });


    var data = [];
    this._loader = new TeamSupport.DataModel(getData, getItemMetadata);
    var loader = this._loader;

    function getData(from, to, sortcol, isdesc, callback) {
        ticketLoadFilter.SortColumn = sortcol;
        ticketLoadFilter.SortAsc = isdesc === false;

        req = top.Ts.Services.Tickets.GetTicketRange(from, to, ticketLoadFilter, callback);

        return req;
    }

    function getItemMetadata(index, data) {
        if (data[index] == null) return;
        var ticket = data[index];

        var result = 'ticket-grid-row';
        if (ticket) {
            if (ticket['SlaWarningTime'] && ticket['SlaWarningTime'] < 0) {
                result = result + ' ticket-grid-row-violated';
            }
            else if (ticket['SlaViolationTime'] && ticket['SlaViolationTime'] < 0) {
                result = result + ' ticket-grid-row-warning';
            }
            if (ticket['IsRead'] && ticket['IsRead'] === true) { result = result + ' ticket-grid-row-read'; } else { result = result + ' ticket-grid-row-unread'; }
            if (ticket['UserID'] == top.Ts.System.User.UserID) { result = result + ' ticket-grid-row-mine'; } else { result = result + ' ticket-grid-row-notmine'; }
            if (ticket['IsClosed'] == true) { result = result + ' ticket-grid-row-closed'; } else { result = result + ' ticket-grid-row-open'; }

        }
        return { cssClasses: result };
    }


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
        return '<i class="fa fa-external-link-square color-green" title="Click to open this ticket"></i>';
    };

    var isReadColumnFormatter = function (row, cell, value, columnDef, ticket) {
        return value == false ? '<i class="fa fa-circle color-blue" title="Click to mark this ticket as read"></i>' : '<i class="fa fa-circle color-lightgray" title="Click to mark this ticket as unread"></i>'
    };

    var isFlaggedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="fa fa-flag color-lightgray" title="Click to flag this ticket for follow up"></i>' : '<i class="fa fa-flag color-red" title="Click to unflag this ticket"></i>'
    };

    var isEnqueuedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="ts-text-icon bgcolor-lightgray color-white" title="Click to add this ticket to your queue">Q</i>' : '<i class="ts-text-icon bgcolor-green color-white" title="Click to remove this ticket from your queue">Q</i>';
    };

    var isSubscribedColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return value == false ? '<i class="fa fa-rss color-lightgray" title="Click to subscribe to this ticket"></i>' : '<i class="fa fa-rss color-yellow" title="Click to unsubscribe to this ticket"></i>'
    };

    var dateTicketColumnFormatter = function (row, cell, value, columnDef, dataContext) {
        return dataContext[columnDef.id].localeFormat(top.Ts.Utils.getDateTimePattern());
    };

    var ticketSourceColumnFormatter = function (row, cell, value, columnDef, ticket) {
        var style = "background: transparent url('../" + top.Ts.Utils.getTicketSourceIcon(value) + "');"
        return '<span class="ts-icon" style="' + style + '" title="Ticket Source: ' + value + '"></span>'
    };

    var checkedFormatter = function (row, cell, value, columnDef, ticket) {
        return '<i class="fa fa-square-o"></i>'
    };

    var moveFormatter = function (row, cell, value, columnDef, ticket) {
        return '<i class="fa fa-bars"></i>'
    };

    var linkFormatter = function (row, cell, value, columnDef, ticket) {
        return '<a href="#" class="cell-link" data-id="' + columnDef.id + '" data-row="' + row + '">' + ticket[columnDef.id] + '</a>'
    };

    function getAllColumns() {
        return [

  	{ id: "openButton", name: "Open Ticket", maxWidth: 24, formatter: openTicketColumnFormatter, unselectable: true, resizable: false, sortable: false, cssClass: 'ticket-grid-cell-sla', headerCssClass: 'no-header-name' },
    { id: "IsRead", name: "Read", field: "IsRead", maxWidth: 24, sortable: true, formatter: isReadColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsFlagged", name: "Flagged", field: "IsFlagged", maxWidth: 24, sortable: true, formatter: isFlaggedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsSubscribed", name: "Subscribed", field: "IsSubscribed", maxWidth: 24, sortable: true, formatter: isSubscribedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsEnqueued", name: "Enqueued", field: "IsEnqueued", maxWidth: 24, sortable: true, formatter: isEnqueuedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "TicketNumber", name: "Number", field: "TicketNumber", width: 75, sortable: true, cssClass: 'ticket-grid-cell-ticketnumber', formatter: linkFormatter },
    { id: "TicketTypeName", name: "Type", field: "TicketTypeName", width: 125, sortable: true },
    { id: "Name", name: "Name", field: "Name", width: 200, sortable: true },
    { id: "UserName", name: "Assigned To", field: "UserName", width: 125, sortable: true },
    { id: "Status", name: "Status", field: "Status", width: 125, sortable: true },
    { id: "Severity", name: "Severity", field: "Severity", width: 125, sortable: true },
    { id: "Customers", name: "Customers", field: "Customers", width: 125, sortable: true },
    { id: "Contacts", name: "Contacts", field: "Contacts", width: 125, sortable: true },
    { id: "GroupName", name: "Group", field: "GroupName", width: 125, sortable: true },
    { id: "DateModified", name: "Last Modified", field: "DateModified", width: 150, sortable: true, formatter: dateTicketColumnFormatter },
    { id: "DaysOpened", name: "Days Opened", field: "DaysOpened", width: 100, sortable: true },
    { id: "ProductName", name: "Product", field: "ProductName", width: 150, sortable: true },
    { id: "CategoryName", name: "Forum Category", field: "CategoryName", width: 150, sortable: true },
    { id: "ReportedVersion", name: "Reported", field: "ReportedVersion", width: 100, sortable: true },
    { id: "SolvedVersion", name: "Resolved", field: "SolvedVersion", width: 100, sortable: true },
    { id: "DateCreated", name: "Date Opened", field: "DateCreated", width: 150, sortable: true, formatter: dateTicketColumnFormatter },
    { id: "IsClosed", name: "Closed", field: "IsClosed", width: 75, sortable: true },
    { id: "CloserName", name: "Closed By", field: "CloserName", width: 125, sortable: true },
    { id: "SlaViolationTime", name: "SLA Violation Time", field: "SlaViolationTime", width: 125, sortable: true, formatter: slaTicketColumnFormatter },
    { id: "TicketSource", name: "Ticket Source", field: "TicketSource", maxWidth: 24, sortable: true, formatter: ticketSourceColumnFormatter, headerCssClass: 'no-header-name' }
	];
    }
    this.getAllColumns = getAllColumns;

    function getDefaultColumns() {
        var cols = getAllColumns();
        var result = [];
        var defaults = ["openButton", "IsRead", "IsFlagged", "IsSubscribed", "IsEnqueued", "TicketNumber", "TicketTypeName", "Name", "UserName", "Status",
        "Severity", "Customers", "Contacts", "GroupName", "DateModified", "DaysOpened"];
        for (var i = 0; i < cols.length; i++) {
            if (defaults.indexOf(cols[i].id) > -1) { result.push(cols[i]); }
        }
        return result;
    }

    function addManColumns(columns) {
        columns.unshift({ id: "checked", name: "checked", maxWidth: 24, formatter: checkedFormatter, unselectable: true, resizable: false, sortable: false, headerCssClass: 'no-header-name' });
        if (ticketLoadFilter.IsEnqueued) {
            columns.unshift({ id: "move", name: "move", behavior: "selectAndMove", maxWidth: 24, formatter: moveFormatter, unselectable: true, resizable: false, sortable: false, headerCssClass: 'no-header-name' });
            for (var i = 0; i < columns.length; i++) {
                columns[i].sortable = false;
            }
        }
        return columns;
    }

    function removeViewColumns(columns) {
        if (ticketLoadFilter.ViewerID && ticketLoadFilter.ViewerID != top.Ts.System.User.UserID) {
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].id == 'IsRead' || columns[i].id == 'IsFlagged' || columns[i].id == 'IsSubscribed' || columns[i].id == 'IsEnqueued') {
                    columns.splice(i, 1);
                    i--;
                }
            }
        }
        return columns;
    }

    this.setTicketColumns = function (columns) {
        grid.setColumns(removeViewColumns(addManColumns(columns)));
    }


    var gridOptions = {
        rowHeight: (self.options.isCompact == true ? 24 : 32),
        editable: false,
        enableAddRow: false,
        enableCellNavigation: true,
        multiSelect: true,
        enableColumnReorder: true
    };

    $('.grid-ticket').toggleClass('grid-compact', self.options.isCompact == true);

    $(layout.panes.center).disableSelection();
    this._grid = new Slick.Grid(layout.panes.center, loader.data, removeViewColumns(addManColumns(getDefaultColumns())), gridOptions);
    grid = this._grid;
    grid.setSelectionModel(new Slick.RowSelectionModel());

    if (ticketLoadFilter.IsClosed && ticketLoadFilter.IsClosed == 'true') { $('.grid-ticket').addClass('grid-closed'); } else { $('.grid-ticket').addClass('grid-notclosed'); }

    if (ticketLoadFilter.IsEnqueued) {
        $('.grid-ticket').addClass('grid-queue');

        var moveRowsPlugin = new Slick.RowMoveManager({
            cancelEditOnDrag: true
        });

        moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
            for (var i = 0; i < data.rows.length; i++) {
                if (data.rows[i] == data.insertBefore) {
                    e.stopPropagation();
                    return false;
                }
            }
            return true;
        });

        moveRowsPlugin.onMoveRows.subscribe(function (e, args) {
            var ids = [];
            var rows = args.rows;
            for (var i = 0; i < rows.length; i++) {
                ids.push(loader.data[rows[i]].TicketID);
            }

            top.Ts.Services.Tickets.MoveUserQueueTickets(JSON.stringify(ids), loader.data.length == args.insertBefore ? -1 : loader.data[args.insertBefore].TicketID, ticketLoadFilter.ViewerID, function () {
                refreshGrid();
                deselectRows();

            });
        });


        grid.registerPlugin(moveRowsPlugin);
        grid.onDragInit.subscribe(function (e, dd) {
            // prevent the grid from cancelling drag'n'drop by default
            e.stopImmediatePropagation();
        });

    }


    grid.onViewportChanged.subscribe(function (e, args) {
        var vp = grid.getViewport();
        loader.ensureData(vp.top, vp.bottom, self.hideLoadingIndicator);
    });

    grid.onColumnsReordered.subscribe(function (e, args) { saveColumns(); });
    $('.slick-columnpicker').on('mouseleave', function (e) { setTimeout(saveColumns, 1000); });

    grid.onColumnsResized.subscribe(function (e, args) { saveColumns(); });

    function saveColumns() {
        var columns = grid.getColumns();
        var info = new Object();
        info.columns = [];
        info.forceFitColumns = grid.getOptions().forceFitColumns == true;

        for (var i = 0; i < columns.length; i++) {
            var item = new Object();
            item.id = columns[i].id;
            item.width = columns[i].width;
            info.columns.push(item);
        }
        info.version = 1;
        top.Ts.Services.Settings.WriteUserSetting('TicketGrid-Columns', JSON.stringify(info));
    }

    $('.grid-ticket').on('click', 'a.cell-link', function (e) {
        var column = $(this).data('id');
        var row = $(this).data('row');
        var ticket = loader.data[row];

        switch (column) {
            case "TicketNumber":
            case "Name":
                e.preventDefault();
                e.stopPropagation();
                top.Ts.MainPage.openTicket(ticket.TicketNumber)
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                break;
            default:
        }
    });

    grid.onClick.subscribe(function (e, args) {
        var cell = args.cell;
        var row = args.row;
        var ticket = loader.data[row];
        var ids = getSelectedIDs();
        var data = JSON.stringify(ids);

        switch (grid.getColumns()[cell].id) {
            case "IsRead":
                var setRead = !ticket.IsRead;
                if (ids.length > 1) {
                    self.showLoadingIndicator();
                    top.Ts.Services.Tickets.SetTicketReads(data, setRead, function () { refreshGrid(); deselectRows(); });
                }
                else {
                    ticket.IsRead = setRead;
                    top.Ts.Services.Tickets.SetTicketRead(ticket.TicketID, ticket.IsRead, function () {
                        top.Ts.MainPage.updateMyOpenTicketReadCount();
                    });
                    if (ticket.IsRead) {
                        $('.slick-row[row="' + row + '"]').addClass('ticket-grid-row-read');
                    }
                    else {
                        $('.slick-row[row="' + row + '"]').removeClass('ticket-grid-row-read');
                    }
                    grid.invalidateRow(row);
                    grid.updateRow(row);
                    grid.render();

                }

                top.Ts.System.logAction('Ticket Grid - Changed Read Status');
                e.stopPropagation();
                e.stopImmediatePropagation();

                return true;
            case "IsFlagged":
                var setIsFlagged = !ticket.IsFlagged;
                if (ids.length > 1) {
                    self.showLoadingIndicator();
                    top.Ts.Services.Tickets.SetTicketFlags(data, setIsFlagged, function () { refreshGrid(); deselectRows(); });
                }
                else {
                    ticket.IsFlagged = setIsFlagged;
                    top.Ts.Services.Tickets.SetTicketFlag(ticket.TicketID, ticket.IsFlagged, function () {
                        grid.invalidateRow(row);
                        grid.updateRow(row);
                        grid.render();
                    });
                }

                top.Ts.System.logAction('Ticket Grid - Changed Flagged Status');
                e.stopPropagation();
                e.stopImmediatePropagation();
                return true;
            case "IsEnqueued":
                var setIsEnqueued = !ticket.IsEnqueued;
                if (ids.length > 1) {
                    self.showLoadingIndicator();
                    top.Ts.Services.Tickets.SetUserQueues(data, setIsEnqueued, function () { refreshGrid(); deselectRows(); });
                }
                else {
                    ticket.IsEnqueued = setIsEnqueued;
                    top.Ts.Services.Tickets.SetUserQueue(ticket.TicketID, setIsEnqueued, function () {
                        grid.invalidateRow(row);
                        grid.updateRow(row);
                        grid.render();
                    });
                }
                top.Ts.System.logAction('Ticket Grid - Changed Queue Status');
                e.stopPropagation();
                e.stopImmediatePropagation();
                return true;
            case "IsSubscribed":
                var setIsSubscribed = !ticket.IsSubscribed;
                if (ids.length > 1) {
                    self.showLoadingIndicator();
                    top.Ts.Services.Tickets.SetTicketSubcribes(data, setIsSubscribed, function () { refreshGrid(); deselectRows(); });
                }
                else {
                    ticket.IsSubscribed = setIsSubscribed;
                    top.Ts.Services.Tickets.SetSubscribed(ticket.TicketID, ticket.IsSubscribed, null, function () {
                        grid.invalidateRow(row);
                        grid.updateRow(row);
                        grid.render();
                    });
                }
                top.Ts.System.logAction('Ticket Grid - Changed Subscribed Status');
                e.stopPropagation();
                e.stopImmediatePropagation();
                return true;
            case "openButton":
                top.Ts.MainPage.openTicket(loader.data[row].TicketNumber);
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                e.stopPropagation();
                e.stopImmediatePropagation();
                return true;
            case "checked":
                var rows = grid.getSelectedRows();

                for (var i = 0; i < rows.length; i++) {
                    if (rows[i] == row) {
                        if (rows.length == 1) return;
                        rows.splice(i, 1);
                        grid.setSelectedRows(rows);

                        var activeCell = grid.getActiveCell();
                        if (activeCell && activeCell.row == row) {
                            grid.setActiveCell(rows[0], 0);
                            grid.setSelectedRows(rows);
                        }
                        grid.invalidateRow(row);
                        grid.updateRow(row);
                        grid.render();

                        e.stopPropagation();
                        e.stopImmediatePropagation();

                        return true;
                    }
                }

                rows.push(row);
                grid.setSelectedRows(rows);
                e.stopPropagation();
                e.stopImmediatePropagation();
                return true;
            default:

        }
        return false;
    });

    grid.onSort.subscribe(function (e, args) {
        var sortCol = args.sortCol;
        var sortAsc = args.sortAsc;
        top.Ts.Services.Settings.WriteUserSetting('TicketGrid-sort-' + window.location.search, sortCol.field + '|' + sortAsc);
        loader.setSort(sortCol.field, sortAsc);
        var vp = grid.getViewport();
        loader.ensureData(vp.top, vp.bottom, self.hideLoadingIndicator);
    });

    grid.onDblClick.subscribe(function (e, args) {
        top.Ts.MainPage.openTicket(loader.data[args.row].TicketNumber, true);
    });

    grid.onActiveCellChanged.subscribe(function (e, o) {
        var ticket = loader.data[o.row];
        if (!ticket) {
            var vp = grid.getViewport();
            loader.ensureData(vp.top, vp.bottom, self.hideLoadingIndicator);
            clearPreview();
        }
        else {
            previewTicket(ticket);
        }
    });

    grid.onSelectedRowsChanged.subscribe(function (e, args) {
        previewActiveTicket();
    });

    function previewActiveTicket() {
        var ticket = getActiveTicket();
        if (!ticket) {
            var vp = grid.getViewport();
            loader.ensureData(vp.top, vp.bottom, self.hideLoadingIndicator);
            clearPreview();
        }
        else {
            previewTicket(ticket);
        }
    }

    function getActiveTicket() {
        var cell = grid.getActiveCell();
        if (cell) {
            return loader.data[cell.row];
        }
        return null;
    }

    function clearPreview() {
        if (preview[0].contentWindow.clearHtml) preview[0].contentWindow.clearHtml();
    }

    function previewTicket(ticket) {
        if (ticket == null) {
            clearPreview();
            $('.ticket-action').prop('disabled', true);
            return;
        }
        $('.ticket-action').prop('disabled', false);
        $('.tickets-delete').prop('disabled', ticket.CreatorID != top.Ts.System.User.UserID && !top.Ts.System.User.IsSystemAdmin);

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
                if (preview[0].contentWindow.writeHtml) preview[0].contentWindow.writeHtml(html);

            });

        });
    }

    loader.onDataLoading.subscribe(function () {
        self.showLoadingIndicator(250);
    });

    loader.onDataLoaded.subscribe(function (e, args) {
        for (var i = args.from; i <= args.to; i++) {
            grid.invalidateRow(i);
        }
        grid.updateRowCount();
        grid.render();

        $('.grid-count').text(loader.data.length + ' Tickets');
        self.hideLoadingIndicator();
        if (self._currentTicket == null) {
            self._currentTicket = getActiveTicket();
            previewTicket(self._currentTicket);
            if (self._currentTicket == null) top.Ts.MainPage.addDebugStatus('null'); else top.Ts.MainPage.addDebugStatus(self._currentTicket.TicketNumber);
        }
    });

};


TicketGrid.prototype = {
    constructor: TicketGrid,
    refresh: function () {
        var self = this;
        top.Ts.Services.Settings.ReadUserSetting('TicketGrid-Columns', null, function (info) {
            var columnInfo = JSON.parse(info);

            if (columnInfo != null) {
                if (columnInfo.forceFitColumns == true) {
                    self._grid.setOptions({ forceFitColumns: true });
                    self._grid.autosizeColumns();
                } else {
                    self._grid.setOptions({ forceFitColumns: false });
                }


                $('.grid-ticket').toggleClass('grid-compact', self.options.isCompact == true);
                self._grid.setOptions({ rowHeight: (self.options.isCompact == true ? 24 : 32) });
                

                var newColumns = [];
                var allColumns = self.getAllColumns();
                var qCol = null;
                var hasQ = false;
                for (var i = 0; i < columnInfo.columns.length; i++) {
                    if (columnInfo.columns[i].id == 'IsEnqueued') { hasQ = true; }
                    for (var j = 0; j < allColumns.length; j++) {
                        if (columnInfo.columns[i].id == allColumns[j].id) {
                            if (allColumns[j].width && allColumns[j].width != null) { allColumns[j].width = columnInfo.columns[i].width; }
                            newColumns.push(allColumns[j]);
                        }

                        if (allColumns[j].id == 'IsEnqueued' && qCol == null) { qCol = allColumns[j]; }
                    }
                }

                if (!columnInfo.version && !hasQ) {
                    newColumns.unshift(qCol);
                }

                if (newColumns.length > 0) self.setTicketColumns(newColumns);
            }
            else {
                self._grid.setOptions({ forceFitColumns: true });
                self._grid.autosizeColumns();
            }

            self._currentTicket = null;
            self._layout.resizeAll();
        });
    }

};




// fix for missing indexOf in IE8
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;
        var from = Number(arguments[1]) || 0;
        from = (from < 0) ? Math.ceil(from) : Math.floor(from);
        if (from < 0) from += len;

        for (; from < len; from++) {
            if (from in this && this[from] === elt) return from;
        }
        return -1;
    };
}