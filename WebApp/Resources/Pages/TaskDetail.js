var _taskDetailPage = null;
var _reminderID = null;
var _taskName = null;
var _historyLoaded = 0;
var _subtasksLoaded = 0;

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
    LoadSubtasks();

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
            $('#fieldComplete').text(task.TaskIsComplete ? "yes" : "no");
            $('#taskComplete').text(task.TaskIsComplete ? "Incomplete" : "Complete");
            $('#fieldDueDate').html(task.TaskDueDate == null ? "[None]" : window.parent.parent.Ts.Utils.getMsDate(task.TaskDueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearDueDate" class="col-xs-1 fa fa-times clearDate"></i>');
            $('#fieldReminder').text(task.IsDismissed ? "no" : "yes");
            $('#fieldReminderDate').html(task.DueDate == null ? "[None]" : window.parent.parent.Ts.Utils.getMsDate(task.DueDate).localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '<i id="clearReminderDate" class="col-xs-1 fa fa-times clearDate"></i>');
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

            if (task.TaskParentID)
            {
                $('#subtasksDiv').hide();
                var parentName = $('<h6>')
                    .addClass('parentName');

                $('<a>')
                  .attr('href', '#')
                  .addClass('parentLink')
                  .data('reminderid', task.TaskParentID)
                  .text(task.TaskParentName + ' >')
                  .appendTo(parentName)
                    
                $('.header-nav').prepend(parentName);

                $('.btn-toolbar').addClass('subtaskButtonsAdjustement');
            }
        });
    }

    function initAssociationControls() {
        $("#attachmentinput").show();
        $(".arrow-up").css('left', '7px');

        $("#ticketinput").hide();
        $("#userinput").hide();
        $("#customerinput").hide();
        $("#contactinput").hide();
        $("#groupinput").hide();
        $("#productinput").hide();
    }

    function resetDisplay() {
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.contact-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
        $(".newticket-group").val(-1);
        $(".newticket-product").val(-1);
    }

    function LoadAssociations() {
        window.parent.parent.Ts.Services.Task.GetAttachments(_reminderID, function (attachments) {
            var attdiv = $('#associationsContainer');
            attdiv.empty();
            //if (attachments.length > 0) {
            //    var attdiv = $('<div>')
            //    .addClass('attachment-list')
            //    .appendTo($('#associationsContainer'));
            //}
            for (var i = 0; i < attachments.length; i++) {
                var blockDiv = $('<div>')
                .data('attachmentID', attachments[i].AttachmentID)
                .hover(function (e) {
                    $(this).find('.associationDelete').show();
                    //if ($(this).attr('filetype').indexOf('image') >= 0) {
                    //    $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                    //    $("#preview")
			        //.css("top", (e.pageY - 10) + "px")
			        //.css("left", (e.pageX + 30) + "px")
			        //.fadeIn("fast");
                    //}
                },
	            function () {
	                $(this).find('.associationDelete').hide();
	                $("#preview").remove();
	            })
                .appendTo(attdiv);

                var atticon = $('<span>')
                .addClass('ts-icon ts-icon-attachment')
                .appendTo(blockDiv);

                $('<a>')
                .attr('target', '_blank')
                .attr('filetype', attachments[i].FileType)
                .text(ellipseString(attachments[i].FileName, 100))
                .addClass('attfilename ui-state-default ts-link preview')
                .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                .appendTo(blockDiv);

                $('<i>')
                .addClass('fa fa-trash-o associationDelete')
                .hide()
                .appendTo(blockDiv);
            }

            window.parent.parent.Ts.Services.Task.LoadAssociations(_reminderID, function (associations) {
                for (var i = 0; i < associations.length; i++) {
                    var blockDiv = $('<div>')
                    .data('refID', associations[i].RefID)
                    .data('refType', associations[i].RefType)
                    .hover(function (e) {
                        $(this).find('.associationDelete').show();
                        //if ($(this).attr('filetype').indexOf('image') >= 0) {
                        //    $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                        //    $("#preview")
                        //.css("top", (e.pageY - 10) + "px")
                        //.css("left", (e.pageX + 30) + "px")
                        //.fadeIn("fast");
                        //}
                    },
                    function () {
                        $(this).find('.associationDelete').hide();
                        //$("#preview").remove();
                    })
                    .appendTo(attdiv);

                    var atticon = $('<span>')
                    //.addClass('ts-icon ts-icon-attachment')
                    .appendTo(blockDiv);

                    var link = $('<a>')
                    .attr('target', '_blank')
                    //.attr('filetype', associations[i].RefType)
                    //.text(ellipseString(attachments[i].FileName, 20))
                    .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                    //.attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                    .appendTo(blockDiv);

                    switch (associations[i].RefType) {
                        case window.parent.parent.Ts.ReferenceTypes.Tickets:
                            atticon.addClass('ticketIcon');
                            link.text(ellipseString(associations[i].TicketNumber + ': ' + associations[i].TicketName, 100));
                            link.attr('href', window.parent.parent.Ts.System.AppDomain + '?TicketID=' + associations[i].RefID);
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openTicketByID(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Users:
                            atticon.addClass('userIcon');
                            link.text(ellipseString(associations[i].User, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Organizations:
                            atticon.addClass('companyIcon');
                            link.text(ellipseString(associations[i].Company, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewCustomer(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Contacts:
                            atticon.addClass('contactIcon');
                            link.text(ellipseString(associations[i].Contact, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Groups:
                            atticon.addClass('groupIcon');
                            link.text(ellipseString(associations[i].Group, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openGroup(' + associations[i].RefID + '); return false;');
                            break;
                        case window.parent.parent.Ts.ReferenceTypes.Products:
                            atticon.addClass('productIcon');
                            link.text(ellipseString(associations[i].Product, 100));
                            link.attr('href', '#');
                            link.attr('target', '_blank');
                            link.attr('onclick', 'window.parent.parent.Ts.MainPage.openNewProduct(' + associations[i].RefID + '); return false;');
                            break;

                    }

                    $('<i>')
                    .addClass('fa fa-trash-o associationDelete')
                    .hide()
                    .appendTo(blockDiv);
                }
            })
        });
    }

    function LoadSubtasks() {
        $('#tblSubtasks tbody').empty();

        window.parent.parent.Ts.Services.Task.LoadSubtasks(_reminderID, function (subtasks) {
            for (var i = 0; i < subtasks.length; i++) {
                var displayName;
                if (subtasks[i].TaskName) {
                    displayName = ellipseString(subtasks[i].TaskName, 40);
                }
                else if (subtasks[i].Description) {
                    displayName = ellipseString(subtasks[i].Description, 40);
                }
                else {
                    displayName = subtasks[i].ReminderID;
                }
                
                var row = $('<tr>').appendTo('#tblSubtasks > tbody:last');
                var nameCel = $('<td>').appendTo(row);
                $('<a>')
                  .attr('href', '#')
                  .addClass('tasklink')
                  .data('reminderid', subtasks[i].ReminderID)
                  .text(displayName)
                  .appendTo(nameCel)

                var userCel = $('<td>').append(subtasks[i].UserName).appendTo(row);

                var dueDateCel = $('<td>').append(subtasks[i].TaskDueDate.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern())).appendTo(row);

                //$('<tr>').html('<td>' + subtasks[i].TaskName + '</td><td>' + subtasks[i].UserID + '</td><td>' + subtasks[i].TaskDueDate.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td>')
                
                //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
            }
            //if (history.length == 50)
            //    $('<button>').text("Load More").addClass('btn-link')
            //    .click(function (e) {
            //        LoadHistory($('#tblSubtasks tbody > tr').length + 1);
            //        $(this).remove();
            //    })
            //   .appendTo('#tblSubtasks > tbody:last');
        });
    }

    function LoadHistory(start) {
        if (start == 1)
            $('#tblHistory tbody').empty();

        window.parent.parent.Ts.Services.Task.LoadHistory(_reminderID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(window.parent.parent.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                .appendTo('#tblHistory > tbody:last');
                //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
            }
            if (history.length == 50)
                $('<button>').text("Load More").addClass('btn-link')
                .click(function (e) {
                    LoadHistory($('#tblHistory tbody > tr').length + 1);
                    $(this).remove();
                })
               .appendTo('#tblHistory > tbody:last');
        });
    }

    $('#taskRefresh').click(function (e) {
        window.location = window.location;
    });

    $('#taskComplete').click(function (e) {
        if ($(this).text() !== 'Incomplete')
        {
            window.parent.parent.Ts.Services.Task.GetIncompleteSubtasks(_reminderID, function (result) {
                if (result)
                {
                    alert('Please complete all the subtasks before completing this task.')
                }
                else
                {
                    window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_reminderID, ($(this).text() !== 'yes'), function (result) {
                        top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                        $('#fieldComplete').text((result === true ? 'yes' : 'no'));
                        $('#taskComplete').text((result === true ? 'Incomplete' : 'Complete'));
                    },
                    function (error) {
                        header.show();
                        alert('There was an error saving the task is complete.');
                    });
                }
            },
            function (error) {
                header.show();
                alert('There was an error getting the subtasks.');
            });
        }
        else
        {
            window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_reminderID, ($(this).text() !== 'Incomplete'), function (result) {
                top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                $('#fieldComplete').text((result === true ? 'yes' : 'no'));
                $('#taskComplete').text((result === true ? 'Incomplete' : 'Complete'));
            },
            function (error) {
                header.show();
                alert('There was an error saving the task is complete.');
            });
        }
    });

    //$('#taskEdit').click(function (e) {
    //    $('p, #taskName').toggleClass("editable");
    //    $(this).toggleClass("btn-primary");
    //    $(this).toggleClass("btn-success");
    //    if ($(this).hasClass("btn-primary"))
    //        $(this).html('<i class="fa fa-pencil"></i> Edit');
    //    else
    //        $(this).html('<i class="fa fa-pencil"></i> Save');
    //});

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
              window.parent.parent.Ts.System.logAction('Task Detail - Save Name');
              window.parent.parent.Ts.Services.Task.SetName(_reminderID, $(this).prev().find('input').val(), function (result) {
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
        window.parent.parent.Ts.System.logAction('Task Detail - Edit Description');
        window.parent.parent.Ts.Services.Task.GetTask(_reminderID, function (task) {
            var desc = task.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //        $('#fieldDesc').tinymce().setContent(desc);
            //        $('#fieldDesc').tinymce().focus();
            //$('#fieldDesc').html(desc);

            initScheduledReportEditor($('#fieldDesc'), function (ed) {
                $('#fieldDesc').tinymce().setContent(desc);
                $('#fieldDesc').tinymce().focus();
            });

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
            window.parent.parent.Ts.System.logAction('Task Detail - Save Description Edit');
            window.parent.parent.Ts.Services.Task.SetDescription(_reminderID, $(this).prev().find('textarea').val(), function (result) {
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
        window.parent.parent.Ts.Services.Customers.LoadOrgUsers(organizationID, function (contacts) {
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
            window.parent.parent.Ts.System.logAction('Task Detail - Save User');
            window.parent.parent.Ts.Services.Task.SetUser(_reminderID, value, function (result) {
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
        if ($(this).text() !== 'yes')
        {
            window.parent.parent.Ts.Services.Task.GetIncompleteSubtasks(_reminderID, function (result) {
                if (result)
                {
                    alert('Please complete all the subtasks before completing this task.')
                }
                else
                {
                    window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_reminderID, ($(this).text() !== 'yes'), function (result) {
                        top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                        $('#fieldComplete').text((result === true ? 'yes' : 'no'));
                    },
                    function (error) {
                        header.show();
                        alert('There was an error saving the task is complete.');
                    });
                }
            },
            function (error) {
                header.show();
                alert('There was an error getting the subtasks.');
            });
        }
        else
        {
            window.parent.parent.Ts.Services.Task.SetTaskIsCompleted(_reminderID, ($(this).text() !== 'yes'), function (result) {
                top.Ts.System.logAction('Task Detail - Toggle TaskIsCompleted');
                $('#fieldComplete').text((result === true ? 'yes' : 'no'));
            },
            function (error) {
                header.show();
                alert('There was an error saving the task is complete.');
            });
        }
    });

    $('#fieldDueDate').on('click', '#clearDueDate', function (e) {
        window.parent.parent.Ts.Services.Task.ClearDueDate(_reminderID, function () {
            top.Ts.System.logAction('Task Detail - Clear Due Date');
            $('#fieldDueDate').text("[None]");
        },
        function (error) {
            header.show();
            alert('There was an error clearing the due date.');
        });
    });

    $('#fieldDueDate').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        window.parent.parent.Ts.System.logAction('Task Detail - Due Date Clicked');
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
                  window.parent.parent.Ts.Services.Task.SetTaskDueDate(_reminderID, value, function (result) {
                      var date = result === null ? null : top.Ts.Utils.getMsDate(result);
                      parent.html((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern()) + '<i id="clearDueDate" class="col-xs-1 fa fa-times clearDate"></i>'))
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
        window.parent.parent.Ts.Services.Task.SetIsDismissed(_reminderID, ($(this).text() !== 'no'), function (result) {
            top.Ts.System.logAction('Task Detail - Toggle IsDismissed');
            $('#fieldReminder').text((result === true ? 'no' : 'yes'));
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

    $('#fieldReminderDate').on('click', '#clearReminderDate', function (e) {
        window.parent.parent.Ts.Services.Task.ClearReminderDate(_reminderID, function () {
            top.Ts.System.logAction('Task Detail - Clear Reminder Date');
            $('#fieldReminderDate').text("[None]");
        },
        function (error) {
            header.show();
            alert('There was an error clearing the reminder date.');
        });
    });

    $('#fieldReminderDate').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        window.parent.parent.Ts.System.logAction('Task Detail - Reminder Date Clicked');
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
                  window.parent.parent.Ts.Services.Task.SetDueDate(_reminderID, value, function (result) {
                      var date = result === null ? null : top.Ts.Utils.getMsDate(result);
                      parent.html((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern()) + '<i id="clearReminderDate" class="col-xs-1 fa fa-times clearDate"></i>'))
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
        $(this).parent().parent().find("#contactinput").hide();
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
        $(this).parent().parent().find("#contactinput").hide();
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
        $(this).parent().parent().find("#contactinput").hide();
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
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '78px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addcontact').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").show();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '102px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addgroup').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").show();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '128px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();
    $('.addproduct').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#contactinput").hide();
        $(this).parent().parent().find("#productinput").show();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '150px');
        $('#associationsBreak').removeClass('associationsBreakAdjustement');
    }).tooltip();

    $('#associationsContainer').on('click', '.associationDelete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this task association?')) {
            window.parent.parent.Ts.System.logAction('Task Detail - Delete Association');
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

    $('#subtasksAdd').click(function (e) {
        //e.preventDefault();
        parent.Ts.System.logAction('Tasks Detail Page - New Task');
        parent.Ts.MainPage.newTask(_reminderID, _taskName);

    });

    $('#tblSubtasks').on('click', '.tasklink', function (e) {
        e.preventDefault();

        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('Task Detail Page - View Subtask');
        parent.Ts.MainPage.openNewTask(id);

        //parent.Ts.Services.Assets.UpdateRecentlyViewed('o' + id, function (resultHtml) {
        //    $('.recent-container').empty();
        //    $('.recent-container').html(resultHtml);
        //});

    });

    $('.header-nav').on('click', '.parentLink', function (e) {
        e.preventDefault();

        var id = $(this).data('reminderid');
        parent.Ts.System.logAction('Task Detail Page - View Parent Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = window.parent.parent.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetContact = null;
    function getContacts(request, response) {
        if (execGetContact) { execGetContact._executor.abort(); }
        execGetContact = parent.parent.Ts.Services.Organizations.GetContacts(request.term, function (result) { response(result); });
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
                window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Users, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Users)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('userIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
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
                window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Organizations, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Organizations)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('companyIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewCustomer(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.contact-search')
    .focusin(function () { $(this).val('').removeClass('contact-search-blur'); })
    .focusout(function () { $(this).val('Search for a contact...').addClass('contact-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('contact-search-blur'); })
    .val('Search for a contact...')
    .autocomplete({
        minLength: 3,
        source: getContacts,
        select: function (event, ui) {
            if (ui.item) {
                window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Contacts, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Contacts)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
	                    function () {
	                        $(this).find('.associationDelete').hide();
	                        $("#preview").remove();
	                    })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('contactIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewContact(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
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
            window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.data, window.parent.parent.Ts.ReferenceTypes.Tickets, function (success) {
                if (success) {
                    var attdiv = $('#associationsContainer');
                    //if (attachments.length > 0) {
                        //var attdiv = $('<div>')
                        //.addClass('attachment-list')
                        //.appendTo($('#associationsContainer'));
                    //}
                    //for (var i = 0; i < attachments.length; i++) {
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.data)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Tickets)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('ticketIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        //.attr('filetype', attachments[i].FileType)
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', window.parent.parent.Ts.System.AppDomain + '?TicketNumber=' + ui.item.id)
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openTicket(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    //}
                }
            });
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
                window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Groups, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Groups)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('groupIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openGroup(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
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
                window.parent.parent.Ts.Services.Task.AddAssociation(_reminderID, ui.item.id, window.parent.parent.Ts.ReferenceTypes.Products, function (success) {
                    if (success) {
                        var attdiv = $('#associationsContainer');
                        var blockDiv = $('<div>')
                        .data('refID', ui.item.id)
                        .data('refType', window.parent.parent.Ts.ReferenceTypes.Products)
                        .hover(function (e) {
                            $(this).find('.associationDelete').show();
                        },
                        function () {
                            $(this).find('.associationDelete').hide();
                            $("#preview").remove();
                        })
                        .appendTo(attdiv);
                        var atticon = $('<span>')
                        .addClass('productIcon')
                        .appendTo(blockDiv);

                        $('<a>')
                        .attr('target', '_blank')
                        .text(ellipseString(ui.item.value, 100))
                        .addClass('attfilename ui-state-default ts-link preview attfilenamefix')
                        .attr('href', '#')
                        .attr('target', '_blank')
                        .attr('onclick', 'window.parent.parent.Ts.MainPage.openNewProduct(' + ui.item.id + '); return false;')
                        .appendTo(blockDiv);

                        $('<i>')
                        .addClass('fa fa-trash-o associationDelete')
                        .hide()
                        .appendTo(blockDiv);
                    }
                });
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

    $('#associationsRefresh').on('click', function () {
        LoadAssociations();
    });

    $('#subtasksAdd').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - Subtasks Add');
        //Pending implementation
    });

    $('#subtasksRefresh').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - Subtasks Refresh');
        LoadSubtasks(1);
    });

    $('#historyToggle').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - History Toggle');
        if (_historyLoaded == 0) {
            _historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
        window.parent.parent.Ts.System.logAction('Task - History Refresh');
        LoadHistory(1);
    });

    //$('.taskProperties p, #taskName').toggleClass("editable");
});


TaskDetailPage = function () {

};

TaskDetailPage.prototype = {
    constructor: TaskDetailPage,
    refresh: function () {

    }
};
