var ticketGrid = null;
$(document).ready(function () {
    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: '.grid-ticket-toolbar', animation: false });

    ticketGrid = new TicketGrid();

    top.Ts.Services.Settings.ReadUserSetting('TicketGrid-sort-' + window.location.search, 'DateModified|false', function (result) {
        var values = result.split('|');
        ticketGrid._loader.setSort(values[0], values[1] === "true");
        ticketGrid._grid.setSortColumn(values[0], values[1] === "true");
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
    var tmrDelayIndicator = null;
    var tmrHideLoading = null;

    this.setTicketColumns = function (columns) {
        grid.setColumns(addManColumns(columns));
    }

    this.showLoadingIndicator = function (delay) {
        if (!delay) {
            if (!loadingIndicator) {
                loadingIndicator = $("<div class='grid-loading'><label></div>").appendTo(document.body);
                loadingIndicator.position({ my: "center center", at: "center center", of: layout.panes.center, collision: "none" });
            }
            loadingIndicator.show();
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


    $('.tickets-new').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.newTicket();
        top.Ts.System.logAction('Ticket Grid - New Ticket');
    });

    $('.tickets-queue').click(function (e) {
        e.preventDefault();
        var ticket = getActiveTicket();
        top.Ts.Services.Tickets.Enqueue(ticket.TicketID, function () { });
        top.Ts.System.logAction('Ticket Grid - Queued');
        top.Ts.System.logAction('Queued');
    });

    $('.tickets-own').click(function (e) {
        e.preventDefault();

        var ids = getSelectedIDs();
        if (ids.length > 1) {
            top.Ts.Services.Tickets.TakeOwnerships(JSON.stringify(ids), function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Take Ownership');
        }
        else {
            var ticket = getActiveTicket();
            top.Ts.Services.Tickets.TakeOwnership(ticket.TicketID, function () { self.refresh(); });
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
            top.Ts.Services.Tickets.SetTicketReads(data, true, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Mark Read');
        }
        else if (el.hasClass('ticket-action-unread')) {
            top.Ts.Services.Tickets.SetTicketReads(data, false, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Mark Unread');
        }
        else if (el.hasClass('ticket-action-reassign')) {

        }
        else if (el.hasClass('ticket-action-status')) {

        }
        else if (el.hasClass('ticket-action-flag')) {
            top.Ts.Services.Tickets.SetTicketFlags(data, true, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Mark Flagged');
        }
        else if (el.hasClass('ticket-action-unflag')) {
            top.Ts.Services.Tickets.SetTicketFlags(data, false, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Mark Unflagged');
        }
        else if (el.hasClass('ticket-action-subscribe')) {
            top.Ts.Services.Tickets.SetTicketSubcribes(data, true, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Subscribed');
        }
        else if (el.hasClass('ticket-action-unsubscribe')) {
            top.Ts.Services.Tickets.SetTicketSubcribes(data, false, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Unsubscribed');
        }
        else if (el.hasClass('ticket-action-enqueue')) {
            top.Ts.Services.Tickets.SetUserQueues(data, true, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Enqueued');
        }
        else if (el.hasClass('ticket-action-dequeue')) {
            top.Ts.Services.Tickets.SetUserQueues(data, false, function () { self.refresh(); grid.setSelectedRows([]); });
            top.Ts.System.logAction('Ticket Grid - Dequeued');
        }
    });





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
                top.top.Ts.Services.Tickets.DeleteTickets(JSON.stringify(ids), function () {
                    self.refresh();
                    grid.setSelectedRows([]);
                });
            }
        }
        else {
            var ticket = getActiveTicket();
            if (confirm('Are you sure you would like to delete Ticket ' + ticket.TicketNumber + '?')) {
                top.Ts.System.logAction('Ticket Grid - Delete Ticket');
                top.top.Ts.Services.Tickets.DeleteTicket(ticket.TicketID, function () { self.refresh(); });
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
        console.log(selectedRowIds);
        return selectedRowIds;

    }

    $('.tickets-refresh').click(function (e) {
        e.preventDefault();
        self.refresh();
        top.Ts.System.logAction('Ticket Grid - Refreshed');
    });


    $('#dialog-columns').modal({ show: false });

    var _lastDialogColumnNo = 0;

    $('.tickets-columns').click(function (e) {
        e.preventDefault();
        _lastDialogColumnNo = 0;
        $('#dialog-columns').modal('show');
        if (grid.getOptions().forceFitColumns) {
            $('.dialog-columns-forcefit input').prop('checked', true);
        }
        else {
            $('.dialog-columns-forcefit input').prop('checked', false);
        }

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
            return $(a).data('col-no') > $(b).data('col-no');
        });
        list.each(function () {
            columns.push($(this).data('o'));
        });

        grid.setColumns(addManColumns(columns));

        if ($('.dialog-columns-forcefit input').prop('checked') == true) {
            grid.setOptions({ forceFitColumns: true });
            grid.autosizeColumns();
        } else {
            grid.setOptions({ forceFitColumns: false });
        }

        saveColumns();
        $('#dialog-columns').modal('hide');

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
        var row = data[index];

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
        return { cssClasses: result };
    }




    function getAllColumns() {
        return [

  	{ id: "openButton", name: "Open Ticket", maxWidth: 24, formatter: openTicketColumnFormatter, unselectable: true, resizable: false, sortable: false, cssClass: 'ticket-grid-cell-sla', headerCssClass: 'no-header-name' },
    { id: "IsRead", name: "Read", field: "IsRead", maxWidth: 24, sortable: true, formatter: isReadColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsFlagged", name: "Flagged", field: "IsFlagged", maxWidth: 24, sortable: true, formatter: isFlaggedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsSubscribed", name: "Subscribed", field: "IsSubscribed", maxWidth: 24, sortable: true, formatter: isSubscribedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsEnqueued", name: "IsEnqueued", field: "IsEnqueued", maxWidth: 24, sortable: true, formatter: isEnqueuedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "TicketNumber", name: "Number", field: "TicketNumber", width: 75, sortable: true, cssClass: 'ticket-grid-cell-ticketnumber' },
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
        return [
  	{ id: "openButton", name: "Open Ticket", maxWidth: 24, formatter: openTicketColumnFormatter, unselectable: true, resizable: false, sortable: false, cssClass: 'ticket-grid-cell-sla', headerCssClass: 'no-header-name' },
    { id: "IsRead", name: "Read", field: "IsRead", maxWidth: 24, sortable: true, formatter: isReadColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsFlagged", name: "Flagged", field: "IsFlagged", maxWidth: 24, sortable: true, formatter: isFlaggedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsSubscribed", name: "Subscribed", field: "IsSubscribed", maxWidth: 24, sortable: true, formatter: isSubscribedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "IsEnqueued", name: "IsEnqueued", field: "IsEnqueued", maxWidth: 24, sortable: true, formatter: isEnqueuedColumnFormatter, unselectable: true, resizeable: false, headerCssClass: 'no-header-name' },
    { id: "TicketNumber", name: "Number", field: "TicketNumber", width: 75, sortable: true, cssClass: 'ticket-grid-cell-ticketnumber' },
    { id: "TicketTypeName", name: "Type", field: "TicketTypeName", width: 125, sortable: true },
    { id: "Name", name: "Name", field: "Name", width: 200, sortable: true },
    { id: "UserName", name: "Assigned To", field: "UserName", width: 125, sortable: true },
    { id: "Status", name: "Status", field: "Status", width: 125, sortable: true },
    { id: "Severity", name: "Severity", field: "Severity", width: 125, sortable: true },
    { id: "DueDate", name: "DueDate", field: "Due Date", width: 125, sortable: true },
    { id: "Customers", name: "Customers", field: "Customers", width: 125, sortable: true },
    { id: "Contacts", name: "Contacts", field: "Contacts", width: 125, sortable: true },
    { id: "GroupName", name: "Group", field: "GroupName", width: 125, sortable: true },
    { id: "DateModified", name: "Last Modified", field: "DateModified", width: 150, sortable: true, formatter: dateTicketColumnFormatter },
    { id: "DaysOpened", name: "Days Opened", field: "DaysOpened", width: 100, sortable: true }
	];
    }

    function addManColumns(columns) {
        columns.unshift({ id: "checked", name: "checked", maxWidth: 24, formatter: checkedFormatter, unselectable: true, resizable: false, sortable: false, headerCssClass: 'no-header-name' });
        if (ticketLoadFilter.IsEnqueued) {
            columns.unshift({ id: "move", name: "move", behavior: "selectAndMove", maxWidth: 24, formatter: moveFormatter, unselectable: true, resizable: false, sortable: false, headerCssClass: 'no-header-name' });

        }
        return columns;
    }

    var options = {
        rowHeight: 32,
        editable: false,
        enableAddRow: false,
        enableCellNavigation: true,
        multiSelect: true,
        enableColumnReorder: true
    };

    $(layout.panes.center).disableSelection();
    this._grid = new Slick.Grid(layout.panes.center, loader.data, addManColumns(getDefaultColumns()), options);
    grid = this._grid;
    grid.setSelectionModel(new Slick.RowSelectionModel());

    if (ticketLoadFilter.IsEnqueued) {
        var moveRowsPlugin = new Slick.RowMoveManager({
            cancelEditOnDrag: true
        });

        moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
            for (var i = 0; i < data.rows.length; i++) {
                // no point in moving before or after itself
                if (data.rows[i] == data.insertBefore || data.rows[i] == data.insertBefore - 1) {
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

            top.Ts.Services.Tickets.MoveUserQueueTickets(JSON.stringify(ids), loader.data[args.insertBefore].TicketID, ticketLoadFilter.UserID, function () {
                self.refresh();
                grid.setSelectedRows([]);

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

        top.Ts.Services.Settings.WriteUserSetting('TicketGrid-Columns', JSON.stringify(info));
    }

    grid.onClick.subscribe(function (e, args) {
        var cell = args.cell;
        var row = args.row;
        switch (grid.getColumns()[cell].id) {
            case "IsRead":
                var ticket = loader.data[row];
                ticket.IsRead = !ticket.IsRead;
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
                top.Ts.System.logAction('Ticket Grid - Changed Read Status');

                return true;
            case "IsFlagged":
                var ticket = loader.data[row];
                ticket.IsFlagged = !ticket.IsFlagged;
                top.Ts.Services.Tickets.SetTicketFlag(ticket.TicketID, ticket.IsFlagged);
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                top.Ts.System.logAction('Ticket Grid - Changed Flagged Status');
                return true;
            case "IsEnqueued":
                var ticket = loader.data[row];
                ticket.IsEnqueued = !ticket.IsEnqueued;
                top.Ts.Services.Tickets.SetUserQueue(ticket.TicketID, ticket.IsEnqueued, function () { });
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                top.Ts.System.logAction('Ticket Grid - Changed Queue Status');
                top.Ts.System.logAction('Ticket Grid - Queued');
                top.Ts.System.logAction('Queued');
                return true;
            case "IsSubscribed":
                var ticket = loader.data[row];
                ticket.IsSubscribed = !ticket.IsSubscribed;
                top.Ts.Services.Tickets.SetSubscribed(ticket.TicketID, ticket.IsSubscribed, null);
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                top.Ts.System.logAction('Ticket Grid - Changed Subscribed Status');
                return true;
            case "openButton":
                top.Ts.MainPage.openTicket(loader.data[row].TicketNumber);
                grid.invalidateRow(row);
                grid.updateRow(row);
                grid.render();
                return true;
            case "checked":
                var rows = grid.getSelectedRows();

                for (var i = 0; i < rows.length; i++) {
                    if (rows[i] == row) {
                        rows.splice(i, 1);
                        grid.setSelectedRows(rows);
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
        //    selectedRowIds = [];
        //    var rows = grid.getSelectedRows();
        //    for (var i = 0, l = rows.length; i < l; i++) {
        //    var ticket = loader.data[rows[i]];
        //    alert(item);
        //    if (item) selectedRowIds.push(item.id);
        //    }
        /*
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
        */
    });

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
        return value == false ? '<i class="fa fa-list-ol color-lightgray" title="Click to add this ticket to your queue"></i>' : '<i class="fa fa-list-ol color-darkgray" title="Click to remove this ticket from your queue"></i>'
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

                var newColumns = [];
                var allColumns = self.getAllColumns();
                for (var i = 0; i < columnInfo.columns.length; i++) {
                    for (var j = 0; j < allColumns.length; j++) {
                        if (columnInfo.columns[i].id == allColumns[j].id) {
                            if (allColumns[j].width && allColumns[j].width != null) { allColumns[j].width = columnInfo.columns[i].width; }
                            newColumns.push(allColumns[j]);
                        }
                    }
                }

                if (newColumns.length > 0) self.setTicketColumns(newColumns);
            }
            else {
                self._grid.setOptions({ forceFitColumns: true });
                self._grid.autosizeColumns();
            }

            self._currentTicket = null;
            //this._loader.clear();
            self._layout.resizeAll();
            //this._grid.resizeCanvas();
            //this._grid.onViewportChanged();
        });
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

    function getItemMetadata(index) {
        if (data[index] == null) return;
        var row = data[index];

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
        return { cssClasses: result };
    }

    data.getItemMetadata = getItemMetadata;

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
        data.getItemMetadata = getItemMetadata;
    }

    function reloadData(from, to, callback) {
        for (var i = from; i <= to; i++)
            delete data[i];
        ensureData(from, to, callback);
    }

    function ensureData(from, to, loadedCallback) {
        if (req) {
            req.get_executor().abort();
            for (var i = req.from; i <= req.to; i++)
                data[i] = undefined;
        }

        to = to + 10;
        from = from - 10;
        if (from < 0) { from = 0; }

        while (data[from] !== undefined && from < to) { from++; }
        while (data[to] !== undefined && from < to) { to--; }
        if (from >= to) {
            if (loadedCallback) loadedCallback();
            return;
        }


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

        }, 100);
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



