﻿var reportPage = null;
$(document).ready(function () {
    reportPage = new ReportPage();
    
});

function onShow() {
    reportPage.refresh();
};

ReportPage = function () {
    var _rowClone = {};
    var _settings = {};
    $('.btn-group [data-toggle="tooltip"]').tooltip({ placement: 'bottom', container: 'body' });

    this.refresh = function () {
        getReports();
    }


    $('.report-refresh').click(
    function (e) {
        e.preventDefault();
        getReports();
    });

    $('body').click(function (e) {
        $('.open').removeClass('open');
    });

    function writeSettings(settings) {
        top.Ts.Services.Settings.WriteUserSetting('Reports-Settings', JSON.stringify(settings));
    }

    function readSettings(callback) {
        top.Ts.Services.Settings.ReadUserSetting('Reports-Settings', null, function (settings) {
            if (callback) { callback(settings == null ? {} : JSON.parse(settings)); }
        });
    }

    readSettings(function (settings) {
        _settings = settings;
        if (_settings.sortField) {
            var item = $('.report-list-header[data-sortfield="' + _settings.sortField + '"]');
            if (item) {
                $('.report-list-header i').removeClass('fa fa-angle-down fa-angle-up');

                item.find('i').addClass(_settings.isDesc ? 'fa fa-angle-down' : 'fa fa-angle-up');
            }
        }

        getReports();
    });



    top.Ts.Services.Reports.GetFolders(function (folders) {
        top.Ts.Settings.User.read('reports-folder', '[]', function (userFolders) {
            var savedIDs = JSON.parse(userFolders);

            function findFolder(id) {
                for (var i = 0; i < folders.length; i++) {
                    var folder = folders[i];
                    if (folder.FolderID == id) {
                        folders.splice(i, 1);
                        return folder;
                    }
                }
            }

            $('<option>').text('No Folder').attr('value', -1).appendTo('.select-folders');


            for (var i = 0; i < savedIDs.length; i++) {
                var folder = findFolder(savedIDs[i]);
                if (folder) addFolder(folder);
            }

            for (var i = 0; i < folders.length; i++) {
                addFolder(folders[i]);
            }

            $(".report-menu ul").sortable({
                delay: 150,
                forceHelperSize: true,
                items: 'li.report-folder',
                update: function (event, ui) {
                    var ids = new Array();
                    $('.report-menu li.report-folder').each(function () {
                        ids.push($(this).data('o').FolderID);
                    });
                    top.Ts.Settings.User.write('reports-folder', JSON.stringify(ids));

                }
            });
        });
    });

    function addFolder(folder) {
        var item = $('<li>')
                    .data('o', folder)
                    .addClass('report-menu-item report-folder report-folder-' + folder.FolderID)
                    .html($('.report-folder-template li').html())
                    .appendTo('.report-menu > ul');
        item.find('.report-folder-name').text(folder.Name);
        item.find('.fa-caret-square-o-down').click(function (e) {
            e.preventDefault();
            $(this).parent().next().dropdown('toggle');
            e.stopPropagation();
        });
        item.find('.report-folder-rename').click(function (e) {
            e.preventDefault();
            $('.open').removeClass('open');
            $('#folderName').val(item.data('o').Name);
            $('.modal-folder-name').modal('toggle').data('folderid', item.data('o').FolderID);
            e.stopPropagation();
        });
        item.find('.report-folder-delete').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('.open').removeClass('open');
            var item = $(this).closest('.report-folder');
            var data = item.data('o');
            if (confirm('Are you sure you would like to delete this folder?  Your reports will not be deleted.')) {
                top.Ts.Services.Reports.DeleteFolder(data.FolderID, function () {
                    item.remove();
                });
            }
        });

        item.droppable({
            activeClass: "drop-active",
            hoverClass: "drop-hover",
            drop: function (event, ui) {
                moveSelectedReports($(this).data('o').FolderID);
                //$(_rowClone.tr).remove();
                $(_rowClone.helper).remove();
            }
        });

        $('<option>')
            .text(folder.Name)
            .attr('value', folder.FolderID)
            .appendTo('.select-folders');
    }

    $('.report-menu').on('click', '.report-menu-item', function (e) {
        e.preventDefault();
        $('.active.report-menu-item').removeClass('active');
        $(this).addClass('active');
        getReports();
    });



    $('.modal-folder-name').modal({ show: false, "backdrop": 'static' });
    $('.modal-folder-move').modal({ show: false, "backdrop": 'static' });

    $('.report-folder-new').click(function (e) {
        e.preventDefault();
        $('.open').removeClass('open');
        $('#folderName').val('');
        $('.modal-folder-name').modal('show').data('folderid', null);
    });

    $('.report-save-folder').click(function (e) {
        e.preventDefault();
        $('.modal-folder-name').modal('hide');
        var id = $('.modal-folder-name').data('folderid');
        top.Ts.Services.Reports.SaveFolder(id, $('#folderName').val(), function (folder) {
            var item = $('.report-folder-' + folder.FolderID);
            if (item.length > 0) {
                item.data('o', folder).find('.report-folder-name').text(folder.Name);
            }
            else {
                addFolder(folder);
            }
        });

    });


    $('.report-list th.report-list-selection').click(function (e) {
        e.preventDefault();
        var check = $(this).find('i');
        if (check.hasClass('fa-square-o')) {
            check.removeClass('fa-square-o').addClass('fa-check-square-o');
            $('.report-item:visible').addClass('report-selected').find('.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
        }
        else {
            check.removeClass('fa-check-square-o').addClass('fa-square-o');
            $('.report-selected').removeClass('report-selected').find('.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');

        }
        updateToolbar();
        e.stopPropagation();
    });

    $('.report-list').on('click', 'td.report-list-selection', function (e) {
        e.preventDefault();
        var row = $(this).closest('.report-item');
        if (row.hasClass('report-selected')) {
            row.removeClass('report-selected').find('.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
        }
        else {
            row.addClass('report-selected').find('.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
        }

        if ($('.report-item:visible').length == $('.report-selected.report-item:visible').length) {
            $('.report-list th.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
        }
        else {
            $('.report-list th.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
        }

        $('.report-active').removeClass('report-active');
        row.addClass('report-active');
        updateToolbar();
        e.stopPropagation();
    });

    $('.report-list').on('click', 'tr.report-item', function (e) {
        e.preventDefault();
        var row = $(this);

        if (e.ctrlKey) {
            if (row.hasClass('report-selected')) {
                row.removeClass('report-selected').find('.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
            }
            else {
                row.addClass('report-selected').find('.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
            }

            if ($('.report-item:visible').length == $('.report-selected.report-item:visible').length) {
                $('.report-list th.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
            }
            else {
                $('.report-list th.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
            }

            if (document.selection)
                document.selection.empty();
            else if (window.getSelection)
                window.getSelection().removeAllRanges();
        }
        else {
            $('.report-selected').removeClass('report-selected').find('.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
            $('.report-list th.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
            row.addClass('report-selected').find('.report-list-selection i').removeClass('fa-square-o').addClass('fa-check-square-o');
        }

        $('.report-active').removeClass('report-active');
        row.addClass('report-active');
        updateToolbar();
        e.stopPropagation();
    });


    $('.report-list').on('click', '.report-list-title a', function (e) {
        e.preventDefault();
        var report = $(this).parents('.report-item').data('o');
        if (report.ReportType == 5) {
            top.Ts.MainPage.openTicketView(report.ReportID, report.IsPrivate);
        }
        else {
            top.Ts.MainPage.openReport($(this).parents('.report-item').data('o'));
        }
    });

    function updateToolbar() {
        if ($('.report-active:visible').length > 0) {
            $('.report-clone').removeClass('disabled');
        } else {
            $('.report-clone').addClass('disabled');
        }
        if ($('.report-selected:visible').length > 0) {
            $('.report-delete').removeClass('disabled');
            $('.report-move').removeClass('disabled');
        } else {
            $('.report-delete').addClass('disabled');
            $('.report-move').addClass('disabled');
        }
    }

    $('.report-move').click(function (e) {
        e.preventDefault();
        $('.modal-folder-move').modal('show');
    });

    $('.report-move-folder').click(function (e) {
        e.preventDefault();
        var button = $(this);
        if (button.hasClass('disabled')) return;
        moveSelectedReports($('.select-folders').val());
        $('.modal-folder-move').modal('hide');
    });

    function moveSelectedReports(folderID) {
        var ids = new Array();
        if (folderID < 0) folderID = null;

        $('.report-list .report-item.report-selected:visible').not('.ui-draggable-dragging').each(function () {
            var report = $(this).data('o');
            report.FolderID = folderID;
            $(this).data('o', report).attr('data-folderid', folderID).attr('data-reporttype', report.ReportType);
            ids.push(report.ReportID);
        });

        top.Ts.Services.Reports.MoveReports(JSON.stringify(ids), folderID, function () { getReports(); });
    }

    $('.report-clone').click(function (e) {
        e.preventDefault();
        var button = $(this);
        if (button.hasClass('disabled')) return;

        var item = $('.report-active');
        if (item.length < 1) return;
        var report = item.data('o');
        top.Ts.Services.Reports.CloneReport(report.ReportID, function (clone) {
            if (report != null) {
                getNewReportItem(clone).hide().insertAfter(item).fadeIn("slow");
                if (clone.ReportType == 5) {
                    top.Ts.MainPage.addNewTicketView(clone, clone.IsPrivate, false);
                }
            }
        });
    });

    $('.report-delete').click(function (e) {
        e.preventDefault();
        var button = $(this);
        if (button.hasClass('disabled')) return;
        var ids = new Array();

        $('.report-item.report-selected:visible').each(function () {
            ids.push($(this).data('o').ReportID);
        });


        if (confirm("Are you sure you would like to delete selected reports?")) {
            top.Ts.Services.Reports.DeleteReports(JSON.stringify(ids), function (results) {
                for (var i = 0; i < results.length; i++) {
                    var item = $('.reportid-' + results[i]);
                    item.fadeOut("slow", function (results) { item.remove(); });
                    top.Ts.MainPage.closeReportTab(results[i]);
                }
            });
        }

    });

    $('.report-list').on('click', '.report-list-star', function (e) {
        e.preventDefault();
        var item = $(this).parents('.report-item');
        var report = item.data('o');
        var isFavorite = item.find('.report-list-star i').hasClass('fa-star');
        top.Ts.Services.Reports.SetFavorite(report.ReportID, !isFavorite, function () {
            if (isFavorite) {
                item.find('.report-list-star i').removeClass('fa-star color-amber').addClass('fa-star-o');
            }
            else {
                item.find('.report-list-star i').removeClass('fa-star-o').addClass('fa-star color-amber');
            }
        });
    });

    function filterReport() {
        $('.report-list .no-reports:visible').hide();
        $('.report-list table:hidden').show();
        if (_tmrSearch) { clearTimeout(_tmrSearch); }
        _tmrSearch = null;
        $('.report-list th.report-list-selection i').removeClass('fa-check-square-o').addClass('fa-square-o');
        $('.report-list .report-item:hidden').show();

        sortReports();
        applySearch();
    }

    function applySearch() {
        var term = $('.report-search').val().toLowerCase();
        if (term.length > 0) {
            $('.report-clear-search i.fa-search').removeClass('fa-search').addClass('fa-times');
        }
        else {
            $('.report-clear-search i.fa-times').removeClass('fa-times').addClass('fa-search');
        }

        $('.report-list .report-item:visible .report-list-title a').filter(function () {
            return $(this).text().toLowerCase().indexOf(term) < 0;
        }).closest('.report-item').hide();


        if ($('.report-list .report-item:visible').length < 1) {
            $('.report-list table:visible').hide();
            $('.report-list .no-reports span').text(term.length > 0 ? ('There are no reports to display with "' + term + '" in the name.') : 'There are no reports to display.')
            $('.report-list .no-reports:hidden').show();
        }
    }

    var _tmrSearch = null;

    $('.report-search').keyup(function (e) {
        if (_tmrSearch) { clearTimeout(_tmrSearch); }
        _tmrSearch = setTimeout(function () { filterReport(); }, 250);
    });

    $('.report-clear-search').click(function (e) {
        e.preventDefault();
        $('.report-search').val('');
        filterReport();
    });


    $('.report-list-header').click(function (e) {
        e.preventDefault();
        var item = $(this);
        var i = item.find('i');
        var isAsc = !i.hasClass('fa-angle-up');
        $('.report-list-header i').removeClass('fa fa-angle-down fa-angle-up');
        i.addClass(isAsc ? 'fa fa-angle-up' : 'fa fa-angle-down');
        _settings.sortField = item.data('sortfield');
        _settings.isDesc = !isAsc;
        writeSettings(_settings);
        sortReports();
    });


    function sortReports() {
        var item = $('.report-list-header .fa');
        var fieldname = item.closest('th').data('sortfield')
        var asc = !item.hasClass('fa-angle-down');

        if (asc == true) {
            $('.report-list table .report-item').sortElements(function (a, b) {
                var val1 = $(a).data('o')[fieldname];
                var val2 = $(b).data('o')[fieldname];
                if (val1 && val1.toLowerCase) { val1 = val1.toLowerCase(); }
                if (val2 && val2.toLowerCase) { val2 = val2.toLowerCase(); }
                if (val1 == null) return -1;
                if (val2 == null) return 1

                return val1 > val2 ? 1 : -1;
            });
        }
        else {
            $('.report-list table .report-item').sortElements(function (a, b) {
                var val1 = $(a).data('o')[fieldname];
                var val2 = $(b).data('o')[fieldname];
                if (val1 && val1.toLowerCase) { val1 = val1.toLowerCase(); }
                if (val2 && val2.toLowerCase) { val2 = val2.toLowerCase(); }
                if (val1 == null) return 1;
                if (val2 == null) return -1
                return val1 < val2 ? 1 : -1;
            });
        }
    }


    function getReports() {
        var item = $('.report-menu-item.active');
        if (item.hasClass('menu-all')) { top.Ts.Services.Reports.GetAllReports(loadReports); }
        else if (item.hasClass('menu-starred')) { top.Ts.Services.Reports.GetStarredReports(loadReports); }
        else if (item.hasClass('menu-tablular')) { top.Ts.Services.Reports.GetReportsByReportType(0, loadReports); }
        else if (item.hasClass('menu-summary')) { top.Ts.Services.Reports.GetReportsByReportType(4, loadReports); }
        else if (item.hasClass('menu-charts')) { top.Ts.Services.Reports.GetReportsByReportType(1, loadReports); }
        else if (item.hasClass('menu-external')) { top.Ts.Services.Reports.GetReportsByReportType(2, loadReports); }
        else if (item.hasClass('menu-custom')) { top.Ts.Services.Reports.GetReportsByReportType(3, loadReports); }
        else if (item.hasClass('menu-stock')) { top.Ts.Services.Reports.GetStockReports(loadReports); }
        else if (item.hasClass('menu-tickets')) { top.Ts.Services.Reports.GetTicketViews(loadReports); }
        else if (item.hasClass('report-folder')) {
            var folder = item.data('o');
            top.Ts.Services.Reports.GetReportsByFolder(folder.FolderID, loadReports);
        }

    }

    function loadReports(data) {
        var reports = JSON.parse(data);
        $('.report-list .report-item').remove();
        for (var i = 0; i < reports.length; i++) {
            $('.report-list table').append(getNewReportItem(reports[i]));
        }

        $(".report-list .report-item").draggable({
            revert: "invalid",
            delay: 250,
            helper: "clone",
            start: function (event, ui) {
                _rowClone.tr = this;
                _rowClone.helper = ui.helper;
            }
        });
        filterReport();
    }



    function getNewReportItem(report) {
        var item = $('<tr>').addClass('report-item reportid-' + report.ReportID).html($('.report-row-template tr').html());
        setReportItem(report, item);
        return item;
    }

    function setReportItem(report, item) {
        var isTsReport = report.OrganizationID == null || report.ReportType == 3;
        report.Creator = isTsReport ? 'TeamSupport' : report.Creator;
        item.attr('data-folderid', report.FolderID);
        item.attr('data-isstock', report.OrganizationID ? 0 : 1);
        item.attr('data-reporttype', report.ReportType);
        item.find('.report-list-title a').text(report.Name);
        item.find('.report-list-star i').addClass(report.IsFavorite == true ? 'fa-star color-amber' : 'fa-star-o');
        item.find('.report-list-owner').text(report.Creator);
        var name = isTsReport ? "" : (report.EditorID == top.Ts.System.User.UserID ? "me" : report.Editor);
        item.find('.report-list-modified').html('<span>' + top.Ts.Utils.getDateString(report.DateEdited, true, false, true) + '</span> <span class="text-muted">' + name + '</span>');
        item.find('.report-list-lastviewed').text(report.LastViewed ? top.Ts.Utils.getDateString(report.LastViewed, true, true, true) : "Never");
        switch (report.ReportType) {
            case 1: item.find('.report-list-title i').addClass('fa-bar-chart-o color-green'); break;
            case 2: item.find('.report-list-title i').addClass('fa-globe color-blue'); break;
            case 3: item.find('.report-list-title i').addClass('fa-wrench color-darkorange'); break;
            case 4: item.find('.report-list-title i').addClass('fa-tasks color-amber'); break;
            case 5: item.find('.report-list-title i').addClass('fa-list'); break;
            default: item.find('.report-list-title i').addClass('fa-table color-red');
        }

        item.data('o', report);

    }
}



if (!Date.prototype.toISOString) {
    (function () {

        function pad(number) {
            if (number < 10) {
                return '0' + number;
            }
            return number;
        }

        Date.prototype.toISOString = function () {
            return this.getUTCFullYear() +
        '-' + pad(this.getUTCMonth() + 1) +
        '-' + pad(this.getUTCDate()) +
        'T' + pad(this.getUTCHours()) +
        ':' + pad(this.getUTCMinutes()) +
        ':' + pad(this.getUTCSeconds()) +
        '.' + (this.getUTCMilliseconds() / 1000).toFixed(3).slice(2, 5) +
        'Z';
        };

    } ());
}