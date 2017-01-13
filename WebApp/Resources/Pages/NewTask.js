/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var _taskParentID;
var _parentTaskName;

$(document).ready(function () {
    $('body').layout({
        defaults: {
            spacing_open: 0,
            closable: false
        },
        north: {
            spacing_open: 1,
            size: 100
        },
        center: {
            maskContents: true
        }
    });

    initScheduledReportEditor($('#Description'), function (ed) {
    });

    _taskParentID = top.Ts.Utils.getQueryValue("taskparentid", window);
    _parentTaskName = top.Ts.Utils.getQueryValue("parenttaskname", window);

    if (_taskParentID) {
        var parentName = $('<h6>')
            .addClass('parentName');

        $('<a>')
          .attr('href', '#')
          .addClass('parentLink')
          .data('reminderid', _taskParentID)
          .text(_parentTaskName + ' >')
          .appendTo(parentName)

        $('.parentLinkContainer').prepend(parentName);
    }

    $('.parentLinkContainer').on('click', '.parentLink', function (e) {
        e.preventDefault();

        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('New Task - View Parent Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    LoadUsers();
    initAssociationControls();
    resetDisplay();

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    function LoadUsers() {
        parent.Ts.Services.Customers.LoadUsers(function (users) {
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].FirstName + ' ' + users[i].LastName).data('o', users[i]).appendTo('#ddlUser');
            }
        });
    }

    function initAssociationControls() {
        $("#attachmentinput").show();
        $(".arrow-up").css('left', '7px');

        $("#ticketinput").hide();
        $("#userinput").hide();
        $("#customerinput").hide();
        $("#groupinput").hide();
        $("#productinput").hide();
    }

    function resetDisplay() {
        $("#inputName").empty();
        $("#Description").empty();
        $("#cbComplete").attr('checked', false);
        $("#DueDate").empty();
        $("#cbReminder").attr('checked', true);
        $("#ReminderDate").empty();
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
    }

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    window.parent.parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        //$('.datepicker').attr("data-format", dateformat);
        //$('.datepicker').datetimepicker({ pickTime: false });

        //$('#DueDate').attr("data-format", dateformat);
        //$('#ReminderDate').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({});
    });

    $('#cbReminder').on('click', function () {
        if ($(this).prop("checked") == true) {
            $('#reminderDateForm').show();
        }
        else {
            $('#reminderDateForm').hide();
        }
    });

    $('.addatt').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").show();
        $(this).parent().parent().find("#ticketinsert").show();
        $(this).parent().find(".arrow-up").css('left', '7px');
        $('#associationsBreak').addClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addticket').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").show();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '30px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.adduser').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").show();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '53px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcustomer').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").show();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '78px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addgroup').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").show();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '104px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addproduct').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").show();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '125px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();

    $('#associationsContainer').on('click', '.associationDelete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this task association?')) {
            window.parent.parent.Ts.System.logAction('New Task - Delete Association');
            var blockDiv = $(this).parent();
            if (blockDiv.data('attachmentID')) {
                parent.privateServices.DeleteAttachment(blockDiv.data('attachmentID'), function (e) {
                    blockDiv.hide();
                });
            }
            else {
                window.parent.parent.Ts.Services.Task.DeleteAssociation(_reminderID, blockDiv.data('refID'), blockDiv.data('refType'), function (result) {
                    blockDiv.hide();
                });
            }
        }
    });

    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = window.parent.parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetUsers = null;
    function getUsers(request, response) {
        if (execGetUsers) { execGetUsers._executor.abort(); }
        execGetUsers = window.parent.parent.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
    }

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
        if (execGetTicket) { execGetTicket._executor.abort(); }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = window.parent.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });

    }

    var execGetGroups = null;
    function getGroupsByTerm(request, response) {
        if (execGetGroups) { execGetGroups._executor.abort(); }
        execGetTicket = window.parent.parent.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
    }

    var execGetProducts = null;
    function getProductByTerm(request, response) {
        if (execGetProducts) { execGetProducts._executor.abort(); }
        execGetProducts = window.parent.parent.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
    }

    $('.user-search')
    .focusin(function () { $(this).val('').removeClass('user-search-blur'); })
    .focusout(function () { $(this).val('Search for a user...').addClass('user-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('user-search-blur'); })
    .val('Search for a user...')
    .autocomplete({
        minLength: 3,
        source: getUsers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('User')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.user-queue')).data('User', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.company-search')
    .focusin(function () { $(this).val('').removeClass('company-search-blur'); })
    .focusout(function () { $(this).val('Search for a company...').addClass('company-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('company-search-blur'); })
    .val('Search for a company...')
    .autocomplete({
        minLength: 3,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Company')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.customer-queue')).data('Company', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.main-quick-ticket')
    .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .val('Search for a ticket...')
    .autocomplete({
        minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.ticket-queue')).data('Ticket', ui.item.data);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

    $('.group-search')
    .focusin(function () { $(this).val('').removeClass('group-search-blur'); })
    .focusout(function () { $(this).val('Search for a group...').addClass('group-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('group-search-blur'); })
    .val('Search for a group...')
    .autocomplete({
        minLength: 2,
        source: getGroupsByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Group')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.group-queue')).data('Group', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this).val("");
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.product-search')
    .focusin(function () { $(this).val('').removeClass('product-search-blur'); })
    .focusout(function () { $(this).val('Search for a product...').addClass('product-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('product-search-blur'); })
    .val('Search for a product...')
    .autocomplete({
        minLength: 3,
        source: getProductByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Product')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.product-queue')).data('Product', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.file-upload').fileupload({
        namespace: 'task_attachment',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var bg = $('<div>')
                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                .appendTo($(this).parent().parent().find('.upload-queue'));

                data.context = bg;
                bg.data('data', data);

                $('<span>')
                .text(ellipseString(data.files[i].name, 20) + '  (' + window.parent.parent.Ts.Utils.getSizeString(data.files[i].size) + ')')
                .addClass('filename')
                .appendTo(bg);

                $('<div>')
                .addClass('progress')
                .hide()
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-close')
                .click(function (e) {
                    e.preventDefault();
                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                })
                .appendTo(bg);

                $('<span>')
                .addClass('ui-icon ui-icon-cancel')
                .hide()
                .click(function (e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                })
                .appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress').progressbar('value', 50);
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
        },
        start: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar().show();
            $(this).parent().parent().find('.upload-queue .ui-icon-close').hide();
            $(this).parent().parent().find('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar('value', 100);
            window.parent.parent.Ts.System.logAction('New Task - Attachment Added');
            $('.commentcontainer').hide();
            $('.faketextcontainer').show();
            $('#messagecontents').val('');
            resetDisplay();
        }
    });

    $('#taskSaveBtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        $('#taskSaveBtn').prop("disabled", true);

        parent.Ts.System.logAction('New Task - Save New Task');

        var taskInfo = new Object();
        taskInfo.TaskParentID = _taskParentID;
        taskInfo.TaskName = $("#inputName").val();
        taskInfo.Description = $("#Description").val();
        taskInfo.UserID = $("#ddlUser").val();
        taskInfo.TaskIsComplete = $("#cbComplete").prop('checked');
        taskInfo.TaskDueDate = $("#DueDate").val();
        taskInfo.IsDismissed = !$("#cbReminder").prop('checked');
        taskInfo.DueDate = $("#ReminderDate").val();

        taskInfo.Tickets = new Array();
        $('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Tickets[taskInfo.Tickets.length] = $(this).data('Ticket');
        });

        taskInfo.Groups = new Array();
        $('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Groups[taskInfo.Groups.length] = $(this).data('Group');
        });

        taskInfo.Products = new Array();
        $('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Products[taskInfo.Products.length] = $(this).data('Product');
        });

        taskInfo.Company = new Array();
        $('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
            taskInfo.Company[taskInfo.Company.length] = $(this).data('Company');
        });

        taskInfo.User = new Array();
        $('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
            taskInfo.User[taskInfo.User.length] = $(this).data('User');
        });

        if (taskInfo.Tickets.length > 0) window.parent.parent.Ts.System.logAction('New Task - Ticket Inserted');
        if (taskInfo.Groups.length > 0) window.parent.parent.Ts.System.logAction('New Task - Group Inserted');
        if (taskInfo.Products.length > 0) window.parent.parent.Ts.System.logAction('New Task - Product Inserted');
        if (taskInfo.Company.length > 0) window.parent.parent.Ts.System.logAction('New Task - Company Inserted');
        if (taskInfo.User.length > 0) window.parent.parent.Ts.System.logAction('New Task - User Inserted');

        var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div.ticket-removable-item');

        window.parent.parent.Ts.Services.Task.NewTask(parent.JSON.stringify(taskInfo), function (newTask) {
            if (attcontainer.length > 0) {
                attcontainer.each(function (i, o) {
                    var data = $(o).data('data');
                    data.url = '../../../Upload/Tasks/' + newTask.ReminderID;
                    data.jqXHR = data.submit();
                    $(o).data('data', data);
                });
            }
            parent.Ts.MainPage.openNewTask(newTask.ReminderID);
            parent.Ts.MainPage.closenewTaskTab();
        });
        $(this).parent().removeClass("saving");
        $('.frame-content').animate({ scrollTop: 0 }, 600);
    });

    $('#taskCancelBtn').click(function (e) {
        parent.Ts.MainPage.closenewTaskTab();
    });
});