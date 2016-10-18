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
    LoadAssociations();

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
            if (task.IsDismissed) {
                $('#reminderDateGroup').hide();
            }
            else {
                $('#reminderDateGroup').show();
            }

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

    function resetDisplay() {
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
        $(".newticket-group").val(-1);
        $(".newticket-product").val(-1);
    }

    function LoadAssociations() {
        window.parent.parent.Ts.Services.Task.GetAttachments(_reminderID, function (attachments) {
            if (attachments.length > 0) {
                var attdiv = $('<div>')
                .addClass('attachment-list')
                .appendTo($('#associationsContainer'));
            }
            for (var i = 0; i < attachments.length; i++) {
                var blockDiv = $('<div>').appendTo(attdiv);
                var atticon = $('<span>')
                .addClass('ts-icon ts-icon-attachment')
                .appendTo(blockDiv);

                $('<a>')
                .attr('target', '_blank')
                .attr('filetype', attachments[i].FileType)
                .text(ellipseString(attachments[i].FileName, 20))
                .addClass('attfilename ui-state-default ts-link preview')
                .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                .hover(function (e) {
                    if ($(this).attr('filetype').indexOf('image') >= 0) {
                        $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                        $("#preview")
			        .css("top", (e.pageY - 10) + "px")
			        .css("left", (e.pageX + 30) + "px")
			        .fadeIn("fast");
                    }
                },
	            function () {
	                $("#preview").remove();
	            })
                .appendTo(blockDiv);

            }
        });
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

    $('#fieldComplete').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Task.SetTaskIsCompleted(_reminderID, ($(this).text() !== 'true'), function (result) {
            top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
            $('#fieldComplete').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the task is complete.');
        });
    });

    $('#fieldDueDate').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.System.logAction('Task Detail - Due Date Clicked');
        var parent = $(this).hide();
        var container = $('<div>')
              .insertAfter(parent);

        var container1 = $('<div>')
                .addClass('col-xs-9')
              .appendTo(container);

        var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val($(this).val())
                .datetimepicker({ pickTime: true })
                .appendTo(container1)
                .focus();

        $('<i>')
              .addClass('col-xs-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#taskEdit').removeClass("disabled");
                  top.Ts.System.logAction('Task - Due Date change cancelled');
              })
              .insertAfter(container1);
        $('<i>')
              .addClass('col-xs-1 fa fa-check')
              .click(function (e) {
                  var value = top.Ts.Utils.getMsDate(input.val());
                  container.remove();
                  top.Ts.Services.Task.SetTaskDueDate(_reminderID, value, function (result) {
                      var date = result === null ? null : top.Ts.Utils.getMsDate(result);
                      parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      $('#taskEdit').removeClass("disabled");
                      top.Ts.System.logAction('Task Detail - Due Date Change');
                  },
                  function (error) {
                      parent.show();
                      alert('There was an error saving the Task Due Date.');
                      $('#taskEdit').removeClass("disabled");
                  });
                  $('#taskEdit').removeClass("disabled");
                  $(this).closest('div').remove();
                  parent.show();
              })
              .insertAfter(container1);
        $('#taskEdit').addClass("disabled");
    });

    $('#fieldReminder').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Task.SetIsDismissed(_reminderID, ($(this).text() !== 'false'), function (result) {
            top.Ts.System.logAction('Task Detail - Toggle IsDismissed');
            $('#fieldReminder').text((result === true ? 'false' : 'true'));
            if (result) {
                $('#reminderDateGroup').hide();
            }
            else {
                $('#reminderDateGroup').show();
            }
        },
        function (error) {
            header.show();
            alert('There was an error saving the task is dismissed.');
        });
    });

    $('#fieldReminderDate').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.System.logAction('Task Detail - Reminder Date Clicked');
        var parent = $(this).hide();
        var container = $('<div>')
              .insertAfter(parent);

        var container1 = $('<div>')
                .addClass('col-xs-9')
              .appendTo(container);

        var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val($(this).val())
                .datetimepicker({ pickTime: true })
                .appendTo(container1)
                .focus();

        $('<i>')
              .addClass('col-xs-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#taskEdit').removeClass("disabled");
                  top.Ts.System.logAction('Task - Reminder Date change cancelled');
              })
              .insertAfter(container1);
        $('<i>')
              .addClass('col-xs-1 fa fa-check')
              .click(function (e) {
                  var value = top.Ts.Utils.getMsDate(input.val());
                  container.remove();
                  top.Ts.Services.Task.SetDueDate(_reminderID, value, function (result) {
                      var date = result === null ? null : top.Ts.Utils.getMsDate(result);
                      parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      $('#taskEdit').removeClass("disabled");
                      top.Ts.System.logAction('Task Detail - Reminder Date Change');
                  },
                  function (error) {
                      parent.show();
                      alert('There was an error saving the Task Reminder Date.');
                      $('#taskEdit').removeClass("disabled");
                  });
                  $('#taskEdit').removeClass("disabled");
                  $(this).closest('div').remove();
                  parent.show();
              })
              .insertAfter(container1);
        $('#taskEdit').addClass("disabled");
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
    })
    .tooltip();
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
    })
    .tooltip();
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
    })
    .tooltip();
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
    })
    .tooltip();
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
    })
    .tooltip();
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

    })
    .tooltip();

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

            $('.upload-queue div.ticket-removable-item').each(function (i, o) {
                var data = $(o).data('data');
                data.url = '../../../Upload/Tasks/' + _reminderID;
                data.jqXHR = data.submit();
                $(o).data('data', data);
            });
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
            window.parent.parent.Ts.System.logAction('Task - Attachment Added');
            $('.commentcontainer').hide();
            $('.faketextcontainer').show();
            $('#messagecontents').val('');
            resetDisplay();
        }
    });

    $('.task-tooltip').tooltip({ placement: 'bottom', container: 'body' });
    $('.taskProperties p, #taskName').toggleClass("editable");
});


TaskDetailPage = function () {

};

TaskDetailPage.prototype = {
    constructor: TaskDetailPage,
    refresh: function () {

    }
};
