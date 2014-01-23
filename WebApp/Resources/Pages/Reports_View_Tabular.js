﻿$(document).ready(function () {
    var _layout = null;
    var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
    var _grid;
    var datamodel = new TeamSupport.DataModels.Reports(_reportID);
    var _report = null;

    top.Ts.Utils.webMethod("ReportService", "GetReport", {
        "reportID": _reportID
    }, function (report) {
        _report = report;
        _report.Def = JSON.parse(report.ReportDef);
        _report.Settings = report.UserSettings == '' ? new Object() : JSON.parse(report.UserSettings);

        if (report.IsFavorite) {
            $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
        }
        $('.report-title').text(report.Name);

        switch (report.ReportType) {
            case 0:
                $('.reports-header i').addClass('fa-table color-red');
                break;
            case 1:
                $('.reports-header i').addClass('fa-bar-chart-o color-green');
                break;
            case 2:
                $('.reports-header i').addClass('fa-globe color-blue');
                break;
            case 3:
                $('.reports-header i').addClass('fa-wrench color-orange');
                break;
            case 4:
                $('.reports-header i').addClass('fa-tasks color-yellow');
                break;
            default:
        }

        if ((top.Ts.System.User.IsSystemAdmin == false && report.CreatorID != top.Ts.System.User.UserID) || report.ReportType == 3) {
            $('.reports-edit').remove();
        }

        top.Ts.Services.Reports.GetFields(_report.Def.Subcategory, function (fields) {
            $('#filter-user').reportFilter({ "fields": fields });
            $('#filter-global').reportFilter({ "fields": fields });
        });

        top.Ts.Utils.webMethod("ReportService", "GetReportColumnNames", {
            "reportID": _reportID
        }, function (repCols) {
            //var idx = new Object();idx.id = "index";idx.name = "index";idx.field = "index";columns.push(idx);

            function findRepCol(id) {
                for (var i = 0; i < repCols.length; i++) {
                    var repCol = repCols[i];
                    if (repCol.Name == id) {
                        repCols.splice(i, 1);
                        return repCol;
                    }
                }
                return null;
            }

            var columns = new Array();

            function addRepCol(repCol) {
                var column = new Object();
                column.id = repCol.Name;
                column.name = repCol.Name;
                column.field = repCol.Name;
                column.width = repCol.Width || 250;
                column.sortable = true;
                column.fieldID = repCol.FieldID;
                column.isCustomField = repCol.IsCustomField;

                if (repCol.DataType == "datetime") {
                    column.formatter = dateFormatter;
                } else if (repCol.DataType == "float") {
                    column.formatter = floatFormatter;
                } else if (repCol.DataType == "bit") {
                    column.formatter = bitFormatter;
                } else if (repCol.IsEmail == true) {
                    column.formatter = emailFormatter;
                } else if (repCol.IsLink == true) {
                    column.formatter = linkFormatter;
                } else if (repCol.IsOpenable == true) {
                    column.formatter = openFormatter;
                    column.openField = repCol.OpenField;
                }
                columns.push(column);
            }

            if (_report.Settings.Columns) {
                for (var i = 0; i < _report.Settings.Columns.length; i++) {
                    var userCol = _report.Settings.Columns[i];
                    var repCol = findRepCol(userCol.id);
                    if (repCol) {
                        repCol.Width = userCol.width;
                        addRepCol(repCol);
                    }
                }
            }

            for (var i = 0; i < repCols.length; i++) {
                addRepCol(repCols[i]);
            }

            initGrid(columns);
        });

    });

    $('.filter-modal').modal({
        "show": false,
        "backdrop": 'static'
    });

    $('.reports-filter').click(function (e) {
        e.preventDefault();
        $('.filter-modal').modal('show');
        $('#filter-global').reportFilter("loadFilters", _report.Def.Filters);
        if (_report.Settings.Filters) $('#filter-user').reportFilter("loadFilters", _report.Settings.Filters);
    });

    $('.filter-save').click(function (e) {
        e.preventDefault();
        _report.Def.Filters = $('#filter-global').reportFilter('getObject');
        _report.Settings.Filters = $('#filter-user').reportFilter('getObject');

        top.Ts.Utils.webMethod("ReportService", "SaveReportDef",
            {
                "reportID": _report.ReportID,
                "data": JSON.stringify(_report.Def)
            },
            function () {
                saveUserSettings(function () {
                    refresh();
                }
            );
            },
            function (error) { alert(error.get_message()); }
        );

        $('.filter-modal').modal('hide');

    });

    function saveUserSettings(callback) {
        top.Ts.Utils.webMethod("ReportService", "SaveUserSettings",
            {
                "reportID": _report.ReportID,
                "data": JSON.stringify(_report.Settings)
            },
            function () { if (callback) callback(); },
            function (error) { alert(error.get_message()); }
        );
    }


    function saveColumns() {
        var columns = _grid.getColumns();
        _report.Settings.Columns = new Array();
        _report.Settings.forceFitColumns = _grid.getOptions().forceFitColumns == true;

        for (var i = 0; i < columns.length; i++) {
            var item = new Object();
            item.id = columns[i].id;
            item.width = columns[i].width;
            _report.Settings.Columns.push(item);
        }

        saveUserSettings();
    }


    $('.reports-refresh').click(function (e) {
        e.preventDefault();
        window.location.assign(window.location.href);
    });

    $('.reports-edit').click(function (e) {
        e.preventDefault();
        window.location.assign("reports_edit.html?ReportID=" + _reportID + '&ReturnURL=' + encodeURIComponent(location.href));
    });

    $('.reports-back').click(function (e) {
        e.preventDefault();
        window.location.assign('reports.html');
    });

    $('.reports-fav').click(function (e) {
        e.preventDefault();
        _report.IsFavorite = !_report.IsFavorite;
        top.Ts.Utils.webMethod("ReportService", "SetFavorite", {
            "reportID": _reportID,
            "value": _report.IsFavorite
        }, function () {
            if (_report.IsFavorite) $('.reports-fav i').removeClass('fa-star-o').addClass('fa-star');
            else $('.reports-fav i').removeClass('fa-star').addClass('fa-star-o');
        });
    });

    $('.reports-export').click(function (e) {
        e.preventDefault();
        //'../dc/1078/reports/95'
        window.open('../../../dc/' + top.Ts.System.Organization.OrganizationID + '/reports/' + _report.ReportID, 'Report Download');
    });

    _layout = $('#reports-tabview-layout').layout({
        //resizeNestedLayout: true,
        //maskIframesOnResize: true,
        defaults: {
            spacing_open: 5,
            closable: false
        },
        center: {

            onresize: resizeGrid,
            triggerEventsOnLoad: false,
            minSize: 500
        },
        north: {
            size: 85,
            spacing_open: 0,
            resizable: false
        }
    });


    function resizeGrid(paneName, paneElement, paneState, paneOptions, layoutName) {
        if (loadingIndicator) {
            loadingIndicator.remove();
            loadingIndicator = null;
        }
        try {
            var vp = _grid.getViewport();
            var t = vp.top;
            datamodel.clear();
            datamodel.ensureData(vp.top, vp.bottom + 50, function () {
                if (t > 10) _grid.scrollRowIntoView(t + 10, false);
                _grid.resizeCanvas();
            });
        } catch (e) {
            alert(e.message);
        }

    }

    var dateFormatter = function (row, cell, value, columnDef, dataContext) {
        var date = dataContext[columnDef.id];
        return date ? top.Ts.Utils.getMsDate(date).localeFormat(top.Ts.Utils.getDateTimePattern()) : '';
    };

    var bitFormatter = function (row, cell, value, columnDef, dataContext) {
        return dataContext[columnDef.id] == true ? "True" : "False";
    };

    var floatFormatter = function (row, cell, value, columnDef, dataContext) {
        return value;
    };

    var emailFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="mailto:' + dataContext[columnDef.id] + '">' + dataContext[columnDef.id] + '</a>';
    };

    var linkFormatter = function (row, cell, value, columnDef, dataContext) {
        return '<a href="' + dataContext[columnDef.id] + '" target="_blank">' + dataContext[columnDef.id] + '</a>';
    };

    var openFormatter = function (row, cell, value, columnDef, dataContext) {
        if (columnDef.openField == "TicketID") {
            return '<a href="#" onclick="top.Ts.MainPage.openTicketByID(' + dataContext["hiddenTicketID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
        } else if (columnDef.openField == "OrganizationID") {
            return '<a href="#" onclick="top.Ts.MainPage.openCustomer(' + dataContext["hiddenOrganizationID"] + '); return false;">' + dataContext[columnDef.id] + '</a>';
        }
        return value;
    };

    var tmrDelayIndicator = null;
    var tmrHideLoading = null;
    var loadingIndicator = null;
    showLoadingIndicator();

    function showLoadingIndicator(delay) {
        if (!delay) {
            if (!loadingIndicator) {
                loadingIndicator = $("<div class='grid-loading'></div>").appendTo(document.body);
                loadingIndicator.position({
                    my: "center center",
                    at: "center center",
                    of: _layout.panes.center,
                    collision: "none"
                });
            }
            loadingIndicator.show();
        } else {
            if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
            tmrDelayIndicator = setTimeout(function () {
                showLoadingIndicator();
            }, delay);
        }
        if (tmrHideLoading) {
            clearTimeout(tmrHideLoading);
        }
        tmrHideLoading = setTimeout(function () {
            hideLoadingIndicator();
        }, 3000);
    }

    function hideLoadingIndicator() {
        tmrHideLoading = null;
        if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
        tmrDelayIndicator = null;
        if (loadingIndicator) loadingIndicator.fadeOut();
    }


    function initGrid(columns) {
        var options = {
            rowHeight: 32,
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            multiSelect: false,
            enableColumnReorder: true,
            forceFitColumns: false
        };

        _grid = new Slick.Grid("#reports-tabview-grid-container", datamodel.data, columns, options);
        _grid.setSortColumn(columns[0].field, true);

        _grid.onViewportChanged.subscribe(function (e, args) {
            var vp = _grid.getViewport();
            datamodel.ensureData(vp.top, vp.bottom);
        });

        _grid.onSort.subscribe(function (e, args) {
            datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
            _report.Settings.SortField = args.sortCol.field;
            _report.Settings.IsSortAsc = args.sortAsc;
            saveUserSettings();
            var vp = _grid.getViewport();
            datamodel.ensureData(vp.top, vp.bottom);
        });

        _grid.onColumnsReordered.subscribe(function (e, args) { saveColumns(); });
        _grid.onColumnsResized.subscribe(function (e, args) { saveColumns(); });


        datamodel.onDataLoading.subscribe(function () {
            showLoadingIndicator(250);
        });

        datamodel.onDataLoaded.subscribe(function (e, args) {
            for (var i = args.from; i <= args.to; i++) {
                _grid.invalidateRow(i);
            }

            _grid.updateRowCount();
            _grid.render();


            hideLoadingIndicator();
        });

        // filter header buttons
        $('.slick-sort-indicator').each(function () {
            var button = $('<div>')
              .addClass('report-header-filter')
              .insertAfter(this)
              .click(function (e) {
                  $(this).addClass('color-orange');
                  var columnDef = $(this).parent().data("column");
                  $('.filter-modal').modal('show');
                  e.preventDefault();
                  e.stopPropagation();
                  $('#filter-user').reportFilter('addCondition', columnDef.fieldID, columnDef.isCustomField);
              });
            $('<i>').addClass('fa fa-filter').appendTo(button);

        });
        if (_report.Settings.SortField) {
          _grid.setSortColumn(_report.Settings.SortField, _report.Settings.IsSortAsc);
      }
      if (!_report.Settings.Columns) _grid.autosizeColumns();
        refresh();
    }

    function refresh() {
        
        _layout.resizeAll();
    }
});