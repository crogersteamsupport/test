var _pageSize = 10;
var _allAssignedLoaded = false;
var _allCreatedLoaded = false;
var _assignedTab = -1;
var _createdTab = 0;
var _start = 0;

$(document).ready(function () {
    $('.action-new').click(function (e) {
        e.preventDefault();
        parent.Ts.System.logAction('Tasks Page - New Task');
        parent.Ts.MainPage.newTask();

    });

    $('#pendingTaskList').on('click', 'a.tasklink', function () {
        //e.preventDefault();
        debugger;
        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);

    });

    $('#pendingTaskList').on('click', 'a.tasklink', function () {
        //e.preventDefault();
        debugger;
        alert('do scrub');
        //var id = $(this).data('reminderid');
        //parent.Ts.System.logAction('Tasks Page - View Task');
        //parent.Ts.MainPage.openNewTask(id);

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
            //$('.results-done').show();
        } else {
            var source = $("#task-template").html();
            var template = Handlebars.compile(source);
            data = { taskList: tasks };
            console.log(data);

            debugger;
            $("#handlebarsTaskList").html(template(data));
        }
        _isLoading = false;
    }


    function isNullOrWhiteSpace(str) {
        return str === null || String(str).match(/^ *$/) !== null;
    }


    function fetchTasks() {

        showLoadingIndicator();

        //parent.Ts.Services.Task.GetTasks($('#searchString').val(), start, 20, searchPending, searchComplete, false, function (items) {
        parent.Ts.Services.Task.LoadPage(_start, _pageSize, _assignedTab, _createdTab, function (pageData) {
            debugger;
            $('.searchresults').fadeTo(0, 1);

            if (_assignedTab == -1 && _createdTab == -1 && pageData.AssignedCount == 0 && pageData.CreatedCount == 0) {
                ShowNoTasks();
            }
            else {
                switch (_assignedTab) {
                    case -1:
                        if (pageData.AssignedCount > 0) {
                            LoadMyTasks(pageData.AssignedItems);
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
                        LoadMyTasks(pageData.AssignedItems);
                }

                switch (_createdTab) {
                    case -1:
                        if (pageData.CreatedCount > 0) {
                            LoadCreated(pageData.CreatedItems);
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
                        LoadCreated(pageData.CreatedItems);
                }

                //if (pageData.AssignedItems.length < _pageSize && pageData.CreatedItems < _pageSize) {
                //    $('.tasks-more').hide();
                //    //$('.results-done').show();
                //}
            }
        });
    }

    $('.action-new').click(function (e) {
        e.preventDefault();
        parent.Ts.System.logAction('Tasks Page - New Task');
        parent.Ts.MainPage.newTask();
    });

    $('.tab-assigned-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tab-created-tasks').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _allAssignedLoaded = false;
        _assignedTab = 1;
        _createdTab = 0;
        _start = 0;
        fetchTasks();
    });

    $('.tab-created-tasks').on('click', function (e) {
        e.preventDefault();
        $('.tab-assigned-tasks').removeClass('active');
        $(this).addClass('active');
        parent.Ts.System.logAction('Tasks Page - Change Filter');
        _assignedTab = 0;
        _allCreatedLoaded = false;
        _createdTab = 1
        _assignedTab = 0;
        _start = 0;
        fetchTasks();
    });

    $('#pendingTaskList').on('click', '.change-task-status', function (e) {
        var id = $(this).data('reminderid');
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Tasks Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked);

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

    Handlebars.registerHelper("taskComplete", function (taskdate) {
        return taskdate != null ? ' checked="checked"' : '';
    });

    Handlebars.registerHelper("mapAssociation", function (association) {
        var str = '';
        
        var refcode = '';

        switch(association.RefType) {
            case 3:
                refcode = '<i class="fa fa-paperclip" title="' + association.Attachment + '"></i>'
                break;
            case 6:
                refcode = '<i class="fa fa-users" title="' + association.Group + '"></i>'
                break;
            case 9:
                refcode = '<i class="fa fa-building" title="' + association.Company + '"></i>'
                break;
            case 13:
                refcode = '<i class="fa fa-archive" title="'+ association.Product +'"></i>'
                break;
            case 17:
                refcode = '<i class="fa fa-ticket" title="' + association.TicketName + '"></i>'
                break;
            case 22:
                refcode = '<i class="fa fa-user" title="' + association.User + '"></i>'
                break;
            default:
                refcode = null;
        }

        if (refcode != null) {
            str = '<span><a href="#" class="association">' + refcode + '</a></span>'; ;
        }

        return new Handlebars.SafeString(str);
    });
});


