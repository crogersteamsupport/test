var _pageSize = 10;
var _allAssignedLoaded = false;
var _allCreatedLoaded = false;
var _start = 0;

var _currentTab = 0;

$(document).ready(function () {
    $('#tasks-Refresh').click(function (e) {
        switch (_currentTab) {
            case 0:
                alert('My Tasks Refresh')
                break;
            case 1:
                alert('My Tasks Refresh')
                break;
            case 2:
                alert('Closed Tasks Refresh')
                break;
            default:
                alert('hello');
        }
        window.location = window.location;
    });

    $('.action-new').click(function (e) {
        //e.preventDefault();
        $('#myModal').modal('show');
        parent.Ts.System.logAction('Tasks Page - New Task');
        //parent.Ts.MainPage.newTask();

    });

    LoadUsers();

    $('#DueDate').datetimepicker({});
    $('#ReminderDate').datetimepicker({});

    $('#taskSaveBtn').click(function (e) {
        debugger;
        //e.preventDefault();
        //e.stopPropagation();

        $('#taskSaveBtn').prop("disabled", true);

        parent.Ts.System.logAction('New Task Page - Added New Task');

        var taskInfo = new Object();
        taskInfo.TaskName = $("#inputName").val();
        taskInfo.Description = $("#Description").val();
        taskInfo.UserID = $("#ddlUser").val();
        taskInfo.TaskIsComplete = $("#cbComplete").prop('checked');
        taskInfo.TaskDueDate = $("#DueDate").val();
        taskInfo.IsDismissed = !$("#cbReminder").prop('checked');
        taskInfo.DueDate = $("#ReminderDate").val();

        //taskInfo.Tickets = new Array();
        //$('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
        //    taskInfo.Tickets[taskInfo.Tickets.length] = $(this).data('Ticket');
        //});

        //taskInfo.Groups = new Array();
        //$('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
        //    taskInfo.Groups[taskInfo.Groups.length] = $(this).data('Group');
        //});

        //taskInfo.Products = new Array();
        //$('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
        //    taskInfo.Products[taskInfo.Products.length] = $(this).data('Product');
        //});

        //taskInfo.Company = new Array();
        //$('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
        //    taskInfo.Company[taskInfo.Company.length] = $(this).data('Company');
        //});

        //taskInfo.User = new Array();
        //$('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
        //    taskInfo.User[taskInfo.User.length] = $(this).data('User');
        //});

        //if (taskInfo.Tickets.length > 0) window.parent.parent.Ts.System.logAction('New Task - Ticket Inserted');
        //if (taskInfo.Groups.length > 0) window.parent.parent.Ts.System.logAction('New Task - Group Inserted');
        //if (taskInfo.Products.length > 0) window.parent.parent.Ts.System.logAction('New Task - Product Inserted');
        //if (taskInfo.Company.length > 0) window.parent.parent.Ts.System.logAction('New Task - Company Inserted');
        //if (taskInfo.User.length > 0) window.parent.parent.Ts.System.logAction('New Task - User Inserted');

        //var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div.ticket-removable-item');

        window.parent.parent.Ts.Services.Task.NewTask(parent.JSON.stringify(taskInfo), function (newTask) {
            if (attcontainer.length > 0) {
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/Tasks/' + newTask.ReminderID;
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            //parent.Ts.MainPage.openNewTask(newTask.ReminderID);
            //parent.Ts.MainPage.closenewTaskTab();
        });

        $('#myModal').modal('hide');

        $(this).parent().removeClass("saving");
        $('.frame-content').animate({ scrollTop: 0 }, 600);


    });


    $('#TaskList').on('click', 'a.tasklink', function () {
        //e.preventDefault();
        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);

    });

    function LoadMyTasks(tasks) {
        var container = $('.assignedresults');
        if (tasks.length < _pageSize) {
            _allAssignedLoaded = true;
        }
        if (_start == 0) {
            insertSearchResults(container, tasks);
            if (tasks.length == 0) {
                $('.results-empty').show();
                $('.results').hide();
            }
            //else {
            //    debugger;
            //    $('.assigned-results-empty').hide();
            //    if (tasks[0].TaskIsComplete) {
            //        //$('.assigned-tasks-filter').removeClass('active');
            //        $('.assigned-tasks-filter-completed').addClass('active');
            //    }
            //}
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
        if (_start == 0) {
            insertSearchResults(container, tasks);
            if (tasks.length == 0) {
                $('.created-results-empty').show();
            }
            //else {
            //    $('.created-results-empty').hide();
            //    if (!tasks[0].TaskIsComplete) {
            //        $('.created-tasks-filter li.active').removeClass('active');
            //        $('.created-tasks-filter-pending').parent().addClass('active');
            //    }
            //}
        }
        else {
            appendSearchResults(container, tasks);
        }
    }

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(container, items) {
        container.empty();
        appendSearchResults(container, items);
    }

    function appendSearchResults(container, tasks) {
        $('.results-loading').hide();
        $('.results-empty').hide();

        if (tasks.length < 1) {
            $('.results').hide();
            $('.results-empty').show();
        } else {
            var source;

            switch (_currentTab) {
                case 0:
                    source = $("#mytask-task-template").html();
                    break;
                case 1:
                    source = $("#assigned-task-template").html();
                    break;
                case 2:
                    source = $("#closed-task-template").html();
                    break;
                default:
                    source = $("#task-template").html();
            }

            //var source = $("#task-template").html();
            var template = Handlebars.compile(source);
            data = { taskList: tasks };
            console.log(data);

            $("#handlebarsTaskList").html(template(data));

            $('.results').show();
        }
        _isLoading = false;
    }

    function isNullOrWhiteSpace(str) {
        return str === null || String(str).match(/^ *$/) !== null;
    }

    function fetchTasks() {

        showLoadingIndicator();

        //parent.Ts.Services.Task.GetTasks($('#searchString').val(), start, 20, searchPending, searchComplete, false, function (items) {
        parent.Ts.Services.Task.LoadPage(_start, _pageSize, _currentTab, function (pageData) {
            $('.searchresults').fadeTo(0, 1);

            switch (_currentTab) {
                case 0:
                    LoadMyTasks(pageData.AssignedItems)
                    //if (pageData.AssignedItems.length > 0) {
                    //    LoadMyTasks(pageData.AssignedItems);
                    //    //if (fristLoad.AssignedItems[0].IsDismissed == 1) {
                    //    //    set completed active
                    //    //}
                    //}
                    //else {
                    //    //HideAssigned();
                    //}
                    break;
                case 1:
                    LoadCreated(pageData.CreatedItems);
                    break;
                case 2:
                    LoadMyTasks(pageData.AssignedItems);
                    break;
                default:
                    LoadMyTasks(pageData.AssignedItems);
            }
            //}

            $('.results-loading').hide();
        });
    }

    function LoadUsers() {
        parent.Ts.Services.Customers.LoadUsers(function (users) {
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].FirstName + ' ' + users[i].LastName).data('o', users[i]).appendTo('#ddlUser');
            }
        });
    }

    //$('.action-new').click(function (e) {
    //    e.preventDefault();
    //    parent.Ts.System.logAction('Tasks Page - New Task');
    //    parent.Ts.MainPage.newTask();
    //});

    $('.tab-assigned-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _allAssignedLoaded = false;
        _currentTab = 0;
        _assignedTab = 0;
        _createdTab = 0;
        _start = 0;
        fetchTasks();
    });

    $('.tab-created-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _currentTab = 1;
        _allCreatedLoaded = false;
        _createdTab = 1;
        _assignedTab = 0;
        _start = 0;
        fetchTasks();
    });

    $('.tab-closed-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tabs button').removeClass('active');
        $(this).addClass('active');

        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _assignedTab = 0;
        _currentTab = 2
        _allCreatedLoaded = false;
        _createdTab = 1
        _assignedTab = 0;
        _start = 0;
        fetchTasks();
    });

    $('#TaskList').on('click', '.change-task-status', function (e) {
        var id = $(this).data('reminderid');
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Tasks Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked);

        $(this).parent().parent().fadeOut(600, function () { $(this).remove() });
    });

    fetchTasks();

    //$('.tabs').on('click', 'a', function (e) {
    //    debugger;
    //    e.preventDefault();
    //    $('.tab-created-tasks').removeClass('active');
    //    $(this).parent().addClass('active');
    //    parent.Ts.System.logAction('Tasks Page - Change Filter');

    //    if ($(this).hasClass('tab-assigned-tasks')) {
    //        $('#createdColumn').hide();
    //        $('#assignedColumn').show();
    //        debugger;
    //        _createdTab = 0;
    //        _allAssignedLoaded = false;
    //        _assignedTab = GetAssignedTab();
    //        _start = 0;
    //    }
    //    else {
    //        $('#assignedColumn').hide();
    //        $('#createdColumn').show();
    //        _assignedTab = 0;
    //        _allCreatedLoaded = false;
    //        _createdTab = GetCreatedTab();
    //        _start = 0;
    //    }


    //    fetchTasks();
    //});

    Handlebars.registerHelper("formatDate", function (datetime) {
        if (datetime != null) {
            return parent.Ts.Utils.getMsDate(datetime).localeFormat(parent.Ts.Utils.getDatePattern());
        }
        else return null;
    });

    Handlebars.registerHelper("formatTaskName", function (Task) {
        var name = Task.TaskName;

        if (Task.TaskName == null) {
            debugger;
            if (Task.Description == null || Task.Description == "") {
                name = 'No Title';
            }
            else {
                name = Task.Description;
            }
        }

        return name;
    });

    Handlebars.registerHelper("formatRow", function (task) {
        var cssClasses = '';
        if (task.TaskIsComplete != true && new Date() > new Date(task.TaskDueDate)) {
            cssClasses = 'danger';
        }
        else {
            return null;
        }

        return cssClasses;
    });

    Handlebars.registerHelper("taskComplete", function (taskdate) {
        return taskdate != null ? ' checked="checked"' : '';
    });

    Handlebars.registerHelper("mapAssociation", function (association) {
        var result = '';
        var functionName = '';
        var associationName = '';
        var iconClass = '';

        switch (association.RefType) {
            //case 3: leaving attachments off for now
            //    associationName = association.Attachment;
            //    iconClass = attIcon;
            //    refcode = '<i class="fa fa-paperclip" title="' + association.Attachment + '"></i>'
            //    break;
            case 6:
                associationName = association.Group;
                iconClass = "groupIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openGroup(' + association.RefID + '); return false;';
                break;
            case 9:
                associationName = association.Company;
                iconClass = "companyIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewCustomer(' + association.RefID + '); return false;';
                break;
            case 13:
                associationName = association.Product;
                iconClass = "productIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewProduct(' + association.RefID + '); return false;';
                break;
            case 17:
                associationName = association.TicketName;
                iconClass = "ticketIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openTicketByID(' + association.RefID + '); return false;'
                break;
            case 22:
                associationName = association.User;
                iconClass = "userIcon";
                functionName = 'window.parent.parent.Ts.MainPage.openNewContact(' + association.RefID + '); return false;'
                break;
            default:
                functionName = null;
        }

        if (functionName != null) {
            result = '<span><a target="_blank" class="ui-state-default ts-link ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></a></span>'
        }

        return new Handlebars.SafeString(result);
    });
});


