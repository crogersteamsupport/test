var _pageSize = 10;
var _allAssignedLoaded = false;
var _allCreatedLoaded = false;
var _assignedTab = -1;
var _createdTab = -1;
var _start = 0;

$(document).ready(function () {

    function LoadAssigned(tasks) {
        var container = $('.assignedresults');
        if (tasks.length < _pageSize) {
            _allAssignedLoaded = true;
        }
        if (_assignedTab == -1) {
            insertSearchResults(container, tasks);
        }
        else {
            appendSearchResults(container, tasks);
        }
    }

    function LoadCreated(tasks) {
        var container = $('.createdresults');
        if (tasks.length < _pageSize) {
            _allCreatedLoaded = true;
        }
        if (_createdTab == -1) {
            insertSearchResults(container, tasks);
        }
        else {
            appendSearchResults(container, tasks);
        }
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

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(container, items) {
        container.empty();
        appendSearchResults(container, items);
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

            if (items.length == _pageSize) {
                $('.tasks-more').show();
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

    function GetAssignedTab() {
        var result = -3;
        if ($('#assignedColumn').is(':hidden')) {
            result = -1;
        }
        else if (_allAssignedLoaded) {
            result = 0;
        }
        else if ($('.assigned-tasks-filter-pending').parent().hasClass('active')) {
            result = 1;
        }
        else if ($('.assigned-tasks-filter-completed').parent().hasClass('active')) {
            result = 2;
        }
        return result;
    }

    function GetGreatedTab() {
        var result = -3;
        if ($('#createdColumn').is(':hidden')) {
            result = -1;
        }
        else if (_allCreatedLoaded) {
            result = 0;
        }
        else if ($('.created-tasks-filter-pending').parent().hasClass('active')) {
            result = 1;
        }
        else if ($('.created-tasks-filter-completed').parent().hasClass('active')) {
            result = 2;
        }
        return result;
    }

    function GetStart() {
        var result = 1;
        if ($('.assignedresults > tbody > tr').length > 0) {
            result = $('.assignedresults > tbody > tr').length + 1;
        }
        if ($('.createdresults > tbody > tr').length >= result) {
            result = $('.createdresults > tbody > tr').length + 1;
        }
        return result;
    }


    function fetchItems() {
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        var term = $('#searchString').val();

        //parent.Ts.Services.Task.GetTasks($('#searchString').val(), start, 20, searchPending, searchComplete, false, function (items) {
        parent.Ts.Services.Task.LoadPage(_start, _pageSize, _assignedTab, _createdTab, function (firstLoad) {
            $('.searchresults').fadeTo(0, 1);


            if (_assignedTab == -1 && _createdTab == -1 && firstLoad.AssignedCount == 0 && firstLoad.CreatedCount == 0) {
                ShowNoTasks();
            }
            else {
                switch (_assignedTab) {
                    case -1:
                        if (firstLoad.AssignedCount > 0) {
                            LoadAssigned(firstLoad.AssignedItems);
                            //if (fristLoad.AssignedItems[0].IsDismissed == 1) {
                            //    set completed active
                            //}
                        }
                        else {
                            HideAssigned();
                        }
                        break;
                    case 0:
                        break;
                    default:
                        LoadAssigned(firstLoad.AssignedItems);
                }

                switch (_createdTab) {
                    case -1:
                        if (firstLoad.CreatedCount > 0) {
                            LoadCreated(firstLoad.CreatedItems);
                            //if (fristLoad.AssignedItems[0].IsDismissed == 0) {
                            //    set pending active
                            //}
                        }
                        else {
                            HideCreated();
                        }
                        break;
                    case 0:
                        break;
                    default:
                        LoadCreated(firstLoad.CreatedItems);
                }
            }
        });
    }

    $('#moreTasks').click(function (e) {
        _assignedTab = GetAssignedTab();
        _createdTab = GetGreatedTab();
        _start = GetStart();
        fetchItems();
    });

    fetchItems();
});