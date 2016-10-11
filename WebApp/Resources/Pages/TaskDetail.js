var _taskDetailPage = null;
var _reminderID = null;
var _taskName = null;

$(document).ready(function () {
    _taskDetailPage = new TaskDetailPage();
    _taskDetailPage.refresh();
    $('.task-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    $('body').layout({
        defaults: {
            spacing_open: 0,
            resizable: false,
            closable: false
        },
        north: {
            size: 100,
            spacing_open: 1
        },
        center: {
            maskContents: true,
            size: 'auto'
        }
    });

    _reminderID = window.parent.parent.Ts.Utils.getQueryValue("reminderid", window);
    parent.privateServices.SetUserSetting('SelectedReminderID', _reminderID);

    LoadProperties();
    initAssociationControls();

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    function LoadProperties() {
        window.parent.parent.Ts.Services.Task.GetTask(_reminderID, function (task) {
            if (task.TaskName) {
                $('#taskName').text(ellipseString(task.TaskName, 73));
            }
            else if (task.Description) {
                $('#taskName').text(ellipseString(task.Description, 73));
            }
            else {
                $('#taskName').text(task.ReminderID);
            }
            _taskName = $('#taskName').text();

            $('#fieldUser').text(task.UserName == "" ? "Unassigned" : task.UserName);
            $('#fieldUser').data('field', task.UserID);
            $('#fieldComplete').text(task.TaskIsComplete);
            $('#fieldDueDate').text(window.parent.parent.Ts.Utils.getMsDate(task.TaskDueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
            $('#fieldReminder').text(!task.IsDismissed);
            $('#fieldReminderDate').text(window.parent.parent.Ts.Utils.getMsDate(task.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));

            $('#fieldCreator').text(task.Creator);
            $('#fieldDateCreated').text(window.parent.parent.Ts.Utils.getMsDate(task.DateCreated).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));
            //$('#fieldModifier').text(task.ModifierName);
            //$('#fieldDateModified').text(window.parent.parent.Ts.Utils.getMsDate(task.DateModified).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()));

            $('#fieldDescription').html(task.Description != null && task.Description != "" ? task.Description : "Empty");
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

    $('#taskEdit').click(function (e) {
        $('.taskProperties p, #taskName').toggleClass("editable");
        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
    });

    $('#taskName').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;

        top.Ts.System.logAction('Task Detail - Edit Name');
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val(_taskName)
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#taskName').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.System.logAction('Task Detail - Save Name');
              top.Ts.Services.Task.SetName(_reminderID, $(this).prev().find('input').val(), function (result) {
                  _taskName = result;
                  header.text(result);
                  $('#taskName').text(result);
                  $('#taskEdit').removeClass("disabled");
              },
              function (error) {
                  header.show();
                  alert('There was an error saving the task name.');
                  $('#taskEdit').removeClass("disabled");
              });
              $('#taskEdit').removeClass("disabled");
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldDescription').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        top.Ts.System.logAction('Task Detail - Edit Description');
        top.Ts.Services.Task.GetTask(_reminderID, function (task) {
            var desc = task.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //        $('#fieldDesc').tinymce().setContent(desc);
            //        $('#fieldDesc').tinymce().focus();
            $('#fieldDesc').html(desc);
            $('#descriptionContent').hide();
            $('#descriptionForm').show();
        });

        $('#btnDescriptionCancel').click(function (e) {
            e.preventDefault();
            $('#descriptionForm').hide();
            $('#descriptionContent').show();
            header.show();
            $('#taskEdit').removeClass("disabled");
        });

        $('#btnDescriptionSave').click(function (e) {
            e.preventDefault();
            top.Ts.System.logAction('Task Detail - Save Description Edit');
            top.Ts.Services.Task.SetDescription(_reminderID, $(this).prev().find('textarea').val(), function (result) {
                header.html(result);
                $('#taskEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the task description.');
                $('#taskEdit').removeClass("disabled");
            });

            $('#descriptionForm').hide();
            $('#descriptionContent').show();
            header.show();
        })
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldUser').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        top.Ts.System.logAction('Task Detail - Edit User');
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryContact').appendTo(container1);
        var organizationID = top.Ts.System.Organization.OrganizationID;
        window.parent.parent
        top.Ts.Services.Customers.LoadOrgUsers(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].UserID).text(contacts[i].FirstName + " " + contacts[i].LastName).data('o', contacts[i]);
                if (header.data('field') == contacts[i].UserID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#taskEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryContact').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();
            top.Ts.System.logAction('Task Detail - Save User');
            top.Ts.Services.Task.SetUser(_reminderID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#taskEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your task user.");
                $('#taskEdit').removeClass("disabled");
            });
        });
        $('#taskEdit').addClass("disabled");
    });

    $('.taskProperties p, #taskName').toggleClass("editable");
});


TaskDetailPage = function () {

};

TaskDetailPage.prototype = {
    constructor: TaskDetailPage,
    refresh: function () {

    }
};
