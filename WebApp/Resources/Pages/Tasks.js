$(document).ready(function () {

    function LoadAssigned(tasks) {
        var container = $('.assignedresults');
        insertSearchResults(container, tasks);
    }

    function LoadCreated(tasks) {
        var container = $('.createdresults');
        insertSearchResults(container, tasks);
    }

    function HideAssigned() {
        $('#assignedColumn').hide();
        $('#createdColumn').removeClass('col-xs-6').addClass('col-xs-12');
        $('#createdColumn').find('.tasks-header').hide();
    }

    function HideCreated() {
        $('#createdColumn').hide();
        $('#assignedColumn').removeClass('col-xs-6').addClass('col-xs-12');
        $('#assignedColumn').find('.tasks-header').hide();
    }

    function ShowNoTasks() {
        HideCreated();
        $('#assignedColumn').find('.results-empty').show();
    }

    function fetchItems(start) {
        start = start || 0;
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        var term = $('#searchString').val();

        var searchPending = false;
        var searchComplete = false;

        if ($('.tasks-filter-all').parent().hasClass('active')) {
            searchPending = true;
            searchComplete = true;
        } else if ($('.tasks-filter-pending').parent().hasClass('active')) {
            searchPending = true;
            searchComplete = false;
        } else if ($('.tasks-filter-completed').parent().hasClass('active')) {
            searchPending = false;
            searchComplete = true;
        }

        //parent.Ts.Services.Task.GetTasks($('#searchString').val(), start, 20, searchPending, searchComplete, false, function (items) {
        parent.Ts.Services.Task.GetFirstLoad(function (firstLoad) {
            $('.searchresults').fadeTo(0, 1);

            if (firstLoad.AssignedCount > 0) {
                LoadAssigned(firstLoad.AssignedItems);
                if (firstLoad.CreatedCount > 0) {
                    LoadCreated(firstLoad.CreatedItems);
                }
                else {
                    HideCreated();
                }
            }
            else if (firstLoad.CreatedCount > 0) {
                LoadCreated(firstLoad.CreatedItems);
                HideAssigned();
            }
            else {
                ShowNoTasks();
            }
        });
    }

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(container, items) {
        container.empty();
        appendSearchResults(container, items);
        _isLoading = false;
    }

    function appendSearchResults(container, items) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();

        if (items.length < 1) {
            $('.results-done').show();
        } else {
            for (var i = 0; i < items.length; i++) {
                appendItem(container, items[i]);
            }
        }
        _isLoading = false;
    }

    function appendItem(container, item) {
        var el = $('<tr>');
        var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
        var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

        if (parent.Ts.System.User.UserID != item.UserID) {
            var avaspn = $('<span>')
                .appendTo(el);

            var avaimg = $('<img>')
                .addClass('topicavatarsm')
                    .attr("src", "/dc/" + parent.Ts.System.User.OrganizationID + "/UserAvatar/" + item.UserID + "/40/" + new Date().getTime())
                .appendTo(avaspn);

            $('<td>').addClass('widthfix').append(
              $('<span>').append(avaspn)
            ).appendTo(el);
        }

        var div = $('<div>')
              .addClass('taskinfo');

        $('<td>').append(div).appendTo(el);

        circle.addClass('color-pink');
        icon.addClass('fa-check');

        appendAsset(div, item);

        el.appendTo(container);
    }

    function appendAsset(el, item) {
        var displayName = item.Description;
        var displayNameIsSerialNumber = false;

        if (parent.Ts.System.User.UserID == item.UserID) {
            var checkbox = $('<input />', { type: 'checkbox', id: 'cb' + item.ReminderID, value: '' });
            $('<div>').addClass('checkbox').append(
                $('<a>')
                  .attr('href', '#')
                  .addClass('assetlink')
                  .data('reminderid', item.ReminderID)
                  .text(displayName)
                  .append(checkbox)
            ).appendTo($('<h4>').appendTo(el));
        }
        else {
            $('<a>')
              .attr('href', '#')
              .addClass('assetlink')
              .data('reminderid', item.ReminderID)
              .text(displayName)
              .appendTo($('<h4>').appendTo(el));
        }

        var list = $('<ul>').appendTo(el);

        //var firstRow = $('<li>').appendTo(list);

        //$('<a>')
        //      .attr('target', '_blank')
        //      .text(item.Description)
        //      .appendTo(firstRow);

        //if (!isNullOrWhiteSpace(item.productVersionNumber)) {
        //    firstRow.append(' - ');
        //    $('<a>')
        //          .attr('target', '_blank')
        //          .text(item.productVersionNumber)
        //          .appendTo(firstRow);
        //}

        var secondRow = $('<li>').appendTo(list);
        //if (!displayNameIsSerialNumber) {
        //    secondRow.append('SN: ');
        //    if (isNullOrWhiteSpace(item.serialNumber)) {
        //        secondRow.append('Empty');
        //    }
        //    else {
        //        secondRow.append(item.serialNumber);
        //    }
        //    secondRow.append(' - ');
        //}

        secondRow.append('Due Date: ');
        if (isNullOrWhiteSpace(item.DueDate)) {
            secondRow.append('Unassigned');
        }
        else {
            secondRow.append(parent.Ts.Utils.getMsDate(item.DueDate).localeFormat(parent.Ts.Utils.getDatePattern()));
        }

    }

    function isNullOrWhiteSpace(str) {
        return str === null || String(str).match(/^ *$/) !== null;
    }

    fetchItems();
});