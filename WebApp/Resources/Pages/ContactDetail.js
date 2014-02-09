/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
var userID = null;

$(document).ready(function () {
    userID = top.Ts.Utils.getQueryValue("user", window);
    noteID = top.Ts.Utils.getQueryValue("noteid", window);
    var _isAdmin = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.IsAdminOnlyCustomers;
    var historyLoaded = 0;

    LoadNotes();
    LoadFiles();
    LoadPhoneTypes();
    LoadPhoneNumbers();
    LoadAddresses();
    LoadProperties();
    LoadCustomProperties();
    LoadReminderUsers();
    UpdateRecentView();
    GetUser()
    var isTSUser;

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

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    $('#contactEdit').click(function (e) {
        $('#userProperties p').toggleClass("editable");
        $('#customProperties p').toggleClass("editable");
        $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        $("#addressPanel #editmenu").toggleClass("hiddenmenu");
        $("#userProperties #fieldEmail").toggleClass("link");
        $("#userProperties #fieldCompany").toggleClass("link");
        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
        //$('#contactName').toggleClass("editable");
        $('#contactTabs a:first').tab('show');
    });

    if (top.Ts.System.Organization.ParentID != null) {
        $('#btnSendWelcome').hide();
    }

    if (noteID != null) {
        $('#contactTabs a:first').tab('show');
        $('#contactTabs a[href="#contact-notes"]').tab('show');
    }
    else {
        $('#contactTabs a:first').tab('show');
    }

    if (!_isAdmin)
    {
        $('#contactEdit').hide();
        $('#contactPhoneButton').hide();
        $('#contactAddressButton').hide();
        $('#contactDelete').hide();
    }

    $('#historyToggle').on('click', function () {
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory();
        }
    });

    $("#btnSaveReminder").click(function (e) {
        if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
            top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Contacts, userID, $('#reminderDesc').val(), $('#reminderDate').val(), $('#reminderUsers').val(), function () { });
            $('#modalReminder').modal('hide');
        }
        else
            alert("Please fill in all the fields");
    });

    $('#contactDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this contact?')) {
            top.privateServices.DeleteUser(userID);
            top.Ts.MainPage.closeNewContactTab(userID);
            top.Ts.MainPage.closeNewContact(userID);

        }
    });

    function GetUser(){
        top.Ts.Services.Customers.GetUser(userID, function (user) {
            $('#contactName').text(user.FirstName + " " + user.LastName);
            $('#userProperties #fieldName').text(user.FirstName + " " + user.LastName);
            $('#userProperties #fieldName').attr("first", user.FirstName);
            $('#userProperties #fieldName').attr("middle", user.MiddleName);
            $('#userProperties #fieldName').attr("last", user.LastName);
            top.privateServices.SetUserSetting('SelectedOrganizationID', user.OrganizationID);
            top.privateServices.SetUserSetting('SelectedContactID', user.UserID);
        });
        }

    
    $('#reminderDate').datetimepicker({});

    var execGetCompany = null;
    function getCompany(request, response) {
        if (execGetCompany) { execGetCompany._executor.abort(); }
        execGetCompany = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
    }

    //$('#contactName').click(function (e) {
        $('#userProperties').on('click', '#fieldName', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        else{
            e.preventDefault();
            var fname, mname, lastname;
            var header = $(this).hide();
            top.Ts.Services.Customers.GetUser(userID, function (user) {


                var container = $('<form>')
                  .addClass('form-inline')
                  .insertAfter(header);

                var container1 = $('<div>')
                    .addClass('form-group')
                  .appendTo(container);

                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.FirstName)
                  .appendTo(container1)
                  .focus();
                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.MiddleName)
                  .appendTo(container1)
                  .focus();
                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.LastName)
                  .appendTo(container1)
                  .focus();
                $('<i>')
                  .addClass('glyphicon glyphicon-ok')
                  .click(function (e) {
                      if ($(this).prev().prev().prev().val() == "") {
                          alert("The first name can not be blank");
                          return;
                      }

                      top.Ts.Services.Customers.SetContactName(userID, $(this).prev().prev().prev().val(), $(this).prev().prev().val(), $(this).prev().val(), function (result) {
                          GetUser();
                      },
                      function (error) {
                          header.show();
                          alert('There was an error saving the customer name.');
                      });
                      $(this).closest('div').remove();
                      header.show();
                  })
                  .appendTo(container1);
                $('<i>')
                  .addClass('glyphicon glyphicon-remove')
                  .click(function (e) {
                      $(this).closest('div').remove();
                      header.show();
                  })
                  .appendTo(container1);
            });


        }

    });

    $('#userProperties').on('click', '#fieldEmail', function (e) {
        if ($(this).hasClass('link')) {
            window.location.href = "mailto:" + $('#fieldEmail').text();
            return;
        }
        else {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var header = $(this).hide();
            var container = $('<div>')
              .insertAfter(header);

            var container1 = $('<div>')
                .addClass('col-md-9')
              .appendTo(container);

            var test = $('<input type="text">')
              .addClass('col-md-10 form-control')
              .val($(this).text() == "Empty" ? "" : $(this).text())
              .appendTo(container1)
              .focus();

            $('<i>')
              .addClass('col-md-1 glyphicon glyphicon-remove')
              .click(function (e) {
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
            $('<i>')
              .addClass('col-md-1 glyphicon glyphicon-ok')
              .click(function (e) {
                  top.Ts.Services.Customers.SetContactEmail(userID, $(this).prev().find('input').val(), function (result) {
                      header.text(result);
                  },
                  function (error) {
                      header.show();
                      alert('There was an error saving the customer email.');
                  });
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
        }
    });
    $('#userProperties').on('click', '#fieldTitle', function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-md-9')
          .appendTo(container);

        var text = $('<input type="text">')
          .addClass('col-md-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-md-1 glyphicon glyphicon-remove')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-md-1 glyphicon glyphicon-ok')
          .click(function (e) {
              top.Ts.Services.Customers.SetContactTitle(userID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
              },
              function (error) {
                  header.show();
                  alert('There was an error saving the customer title.');
              });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);


    });
    $('#userProperties').on('click', '#fieldActive', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactActive(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldActive').text((result === true ? 'Yes' : 'No'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
    });
    $('#userProperties').on('click', '#fieldPortalUser', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactPortalUser(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPortalUser').text((result === true ? 'Yes' : 'No'));
            if(result != true)
                $('#btnSendNewPW').hide();
            else
                $('#btnSendNewPW').show();
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer portal user status.');
        });
    });
    $('#userProperties').on('click', '#fieldCompany', function (e) {
        if ($(this).hasClass('link')) {
            top.Ts.MainPage.openNewCustomer($(this).attr('orgID'));
            return;
        }
        else {

            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var header = $(this).hide();
            var container = $('<div>')
              .insertAfter(header);

            var container1 = $('<div>')
                .addClass('col-md-9')
              .appendTo(container);

            var text = $('<input type="text">')
              .addClass('col-md-10 form-control')
              .val($(this).text())
              .appendTo(container1)
              .autocomplete({
                  minLength: 2,
                  source: getCompany,
                  select: function (event, ui) {
                      $(this)
                      .data('item', ui.item.id)
                      .removeClass('ui-autocomplete-loading')
                  }
              })
              .focus();

            $('<i>')
              .addClass('col-md-1 glyphicon glyphicon-remove')
              .click(function (e) {
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
            $('<i>')
              .addClass('col-md-1 glyphicon glyphicon-ok')
              .click(function (e) {
                  var neworgID = $(this).prev().find('input').data('item');
                  if(neworgID != undefined){
                  top.Ts.Services.Customers.SetContactCompany(userID, neworgID, function (result) {
                      header.text(result);
                      header.attr('orgid', neworgID);
                  },
                  function (error) {
                      header.show();
                      alert('There was an error saving the customer company.');
                  });
                  }
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
        }
    });
    $('#userProperties').on('click', '#fieldPreventemailfromcreatingtickets', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactPreventEmail(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPreventemailfromcreatingtickets').text((result === true ? 'Yes' : 'No'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer block email status.');
        });
    });
    $('#userProperties').on('click', '#fieldSystemAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactSystemAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldSystemAdministrator').text((result === true ? 'Yes' : 'No'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer system admin status.');
        });
    });
    $('#userProperties').on('click', '#fieldFinancialAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactFinancialAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldFinancialAdministrator').text((result === true ? 'Yes' : 'No'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer financial admin status.');
        });
    });



    $('#noteToggle').click(function (e) {
        $('#noteForm').toggle();
        $('#fieldNoteTitle').focus();
    });

    $('#fileToggle').click(function (e) {
        $('#fileForm').toggle();
    });

    $("input[type=text], textarea").autoGrow();

    $('#contactTabs a:first').tab('show');

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Tickets")
            $('#ticketIframe').attr("src", "tickettabs.html?ContactID=" + userID);
    })

    $('#phonePanel').on('click', '.delphone', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this phone number?')) {
            top.privateServices.DeletePhone($(this).attr('id'));
            LoadPhoneNumbers();
        }
    });

    $("#phonePanel").on("click", '.editphone', function (e) {
        e.preventDefault();
        top.Ts.Services.Customers.LoadPhoneNumber($(this).attr('id'), function (phone) {
            $('#phoneType').val(phone[0].PhoneTypeID);
            $('#phoneNumber').val(phone[0].Number);
            $('#phoneExt').val(phone[0].Extension);
            $('#phoneID').val(phone[0].PhoneID);
            $('#modalPhone').modal('show');
        });
    });

        $('#addressPanel').on('click', '.deladdress', function (e) {
            e.preventDefault();
            if (confirm('Are you sure you would like to remove this address?')) {
                top.privateServices.DeleteAddress($(this).attr('id'));
                LoadAddresses();
            }
        });

    $("#addressPanel").on("click", '.editaddress', function (e) {
        e.preventDefault();
        top.Ts.Services.Customers.LoadAddress($(this).attr('id'), function (phone) {
            $('#addressDesc').val(phone[0].Description);
            $('#address1').val(phone[0].Addr1);
            $('#address2').val(phone[0].Addr2);
            $('#address3').val(phone[0].Addr3);
            $('#addressCity').val(phone[0].City);
            $('#addressState').val(phone[0].State);
            $('#addressZip').val(phone[0].Zip);
            $('#addressCountry').val(phone[0].Country);
            $('#addressID').val(phone[0].AddressID);
            $('#modalAddress').modal('show');
        });
    });

    $("#btnPhoneSave").click(function (e) {
        var phoneInfo = new Object();

        phoneInfo.PhoneTypeID = $('#phoneType').val();
        phoneInfo.Number = $('#phoneNumber').val();
        phoneInfo.Extension = $('#phoneExt').val();
        phoneInfo.PhoneID = $('#phoneID').val();

        top.Ts.Services.Customers.SavePhoneNumber(top.JSON.stringify(phoneInfo), userID, top.Ts.ReferenceTypes.Users, function (f) {
            $("#phoneType")[0].selectedIndex = 0;
            $('#phoneNumber').val('');
            $('#phoneExt').val('')
            $('#phoneID').val('-1');
            $('#modalPhone').modal('hide');
            LoadPhoneNumbers();
        }, function () {
            alert('There was an error saving this phone number.  Please try again.');
        });

    });

    //$("#phonePanel, #addressPanel").on('mouseenter', '.content', function () {
    //    $(this).find(".hiddenmenu").toggle();
    //});
    //$("#phonePanel, #addressPanel").on('mouseleave', '.content', function () {
    //    $(this).find(".hiddenmenu").toggle();
    //});

    $("#btnAddressSave").click(function (e) {
        var addressInfo = new Object();
        addressInfo.Description = $('#addressDesc').val();
        addressInfo.Addr1 = $('#address1').val();
        addressInfo.Addr2 = $('#address2').val();
        addressInfo.Addr3 = $('#address3').val();
        addressInfo.City = $('#addressCity').val();
        addressInfo.State = $('#addressState').val();
        addressInfo.Zip = $('#addressZip').val();
        addressInfo.Country = $('#addressCountry').val();
        addressInfo.AddressID = $('#addressID').val();

        top.Ts.Services.Customers.SaveAddress(top.JSON.stringify(addressInfo), userID, top.Ts.ReferenceTypes.Users, function (f) {

            addressInfo.Description = $('#addressDesc').val('');
            addressInfo.Addr1 = $('#address1').val('');
            addressInfo.Addr2 = $('#address2').val('');
            addressInfo.Addr3 = $('#address3').val('');
            addressInfo.City = $('#addressCity').val('');
            addressInfo.State = $('#addressState').val('');
            addressInfo.Zip = $('#addressZip').val('');
            addressInfo.Country = $('#addressCountry').val('');
            addressInfo.AddressID = $('#addressID').val('-1');
            $('#modalAddress').modal('hide');
            LoadAddresses();
        }, function () {
            alert('There was an error saving this address.  Please try again.');
        });

    });

    $('#tblFiles').on('click', '.viewFile', function (e) {
        e.preventDefault();
        top.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
    });

    $('#tblFiles').on('click', '.delFile', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this attachment?')) {
            top.privateServices.DeleteAttachment($(this).parent().parent().attr('id'));
            LoadFiles();
        }
    });

    $('#tblNotes').on('click', '.editNote', function (e) {
        e.stopPropagation();
        top.Ts.Services.Customers.LoadNote($(this).parent().parent().attr('id'), function (note) {
            $('#fieldNoteTitle').val(note.Title);
            var desc = note.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            $('#fieldNoteDesc').val(desc);
            $('#fieldNoteID').val(note.NoteID);
            $('#btnNotesSave').text("Edit Note");
            $('#btnNotesCancel').show();
            $('#noteForm').show();
        });
    });

    $('#tblNotes').on('click', '.deleteNote', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this note?')) {
            top.privateServices.DeleteNote($(this).parent().parent().attr('id'), function () {;
                LoadNotes();
            });
        }
    });

    $('#tblNotes').on('click', '.viewNote', function (e) {
        e.preventDefault();
        var desc = $(this).data('description');
        $('.noteDesc').toggle();
        $('.noteDesc').html("<strong>Description</strong> <p>" + desc + "</p>");
    });

    $("#btnNotesCancel").click(function (e) {
        e.preventDefault();
        $('#fieldNoteTitle').val('');
        $('#fieldNoteDesc').val('');
        $('#fieldNoteID').val('-1');
        $('#btnNotesSave').text("Save Note");
        $('#noteForm').toggle();
    });

    $("#btnNotesSave").click(function (e) {
        e.preventDefault();
        var title = $('#fieldNoteTitle').val();
        var description = $('#fieldNoteDesc').val();
        var noteID = $('#fieldNoteID').val();
        if ((title.length || description.length) < 1) {
            alert("Please fill in all the required information");
            return;
        }

        top.Ts.Services.Customers.SaveNote(title, description, noteID, userID, top.Ts.ReferenceTypes.Users, function (note) {
            $('#fieldNoteTitle').val('');
            $('#fieldNoteDesc').val('');
            $('#fieldNoteID').val('-1');
            $('#btnNotesSave').text("Save Note");
            LoadNotes();
            $('#noteForm').toggle();
        });
    });

    $("#btnFilesCancel").click(function (e) {
        $('.upload-queue').empty();
        $('#attachmentDescription').val('');
        $('#fileForm').toggle();
    });

    $('#btnFilesSave').click(function (e) {

        if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
                var data = $(o).data('data');
                data.formData = { description: $('#attachmentDescription').val() };
                data.url = '../../../Upload/UserAttachments/' + userID;
                data.jqXHR = data.submit();
                $(o).data('data', data);
            });
        }
        $('#fileForm').toggle();
    });

    $('.file-upload').fileupload({
        namespace: 'custom_attachment',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {
                var item = $('<li>')
                  .appendTo($('.upload-queue'));

                data.context = item;
                item.data('data', data);

                var bg = $('<div>')
                  .addClass('ts-color-bg-accent')
                  .appendTo(item);

                $('<div>')
                  .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);

                $('<span>')
                  .addClass('glyphicon glyphicon-remove')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

                $('<span>')
                  .addClass('glyphicon glyphicon-remove')
                  .hide()
                  .click(function (e) {
                      e.preventDefault();
                      var data = $(this).closest('li').data('data');
                      data.jqXHR.abort();
                  })
                  .appendTo(bg);

                var progress = $('<div>')
                  .addClass('progress progress-striped active')
                  .hide();

                $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

                progress.appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                data.context.find('.progress-bar').css('width', '50%');
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.upload-queue .ui-icon-close').hide();
            $('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $('.progress-bar').css('width', '100%');
            LoadFiles();
            $('.upload-queue').empty();
            $('#attachmentDescription').val('');

            //if (_doClose != true) top.Ts.MainPage.openTicketByID(_ticketID);
            //top.Ts.MainPage.closeNewTicketTab();
        }
    });

    $("#modalPhone").on('hidden.bs.modal', function () {
        $('#modalPhone input').val('');
    });
    $("#modalAddress").on('hidden.bs.modal', function () {
        $('#modalAddress input').val('');
    });
    $("#modalReminder").on('hidden.bs.modal', function () {
        $('#modalReminder input').val('');
    });

    $('#btnSendWelcome').click(function (e) {
        top.Ts.Services.Customers.SendWelcome(userID, function (msg) {
            alert(msg);
        });
    });

    top.Ts.Services.Customers.GetContactTickets(userID, 0, function (e) {
        $('#openTicketCount').text("Open Tickets: " + e);
    });

    function LoadNotes() {
        top.Ts.Services.Customers.LoadNotes(userID, top.Ts.ReferenceTypes.Users, function (note) {
            $('#tblNotes tbody').empty();
            var html;
            for (var i = 0; i < note.length; i++) {

                if (_isAdmin || note[i].CreatorID == top.Ts.System.User.UserID)
                    html = '<td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                else
                    html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';


                $('<tr>').addClass("viewNote")
                .attr("id", note[i].NoteID)
                .html(html)
                .data("description", note[i].Description)
                .appendTo('#tblNotes > tbody:last');
                //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
            }
        });
    }

    function LoadFiles() {
        $('#tblFiles tbody').empty();
        top.Ts.Services.Customers.LoadFiles(userID, top.Ts.ReferenceTypes.Users, function (files) {
            var html;
            for (var i = 0; i < files.length; i++) {

                if (_isAdmin || files[i].CreatorID == top.Ts.System.User.UserID)
                    html = '<td><i class="glyphicon glyphicon-trash delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                else
                    html = '<td></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';

                var tr = $('<tr>')
                .attr('id', files[i].AttachmentID)
                .html(html)
                .appendTo('#tblFiles > tbody:last');


                //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
            }
        });
    }

    function LoadPhoneNumbers() {
        $('#phonePanel').empty();
        top.Ts.Services.Customers.LoadPhoneNumbers(userID,top.Ts.ReferenceTypes.Users, function (phone) {
            for (var i = 0; i < phone.length; i++) {
                $('#phonePanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-md-4 control-label'>" + phone[i].PhoneTypeName + "</label> \
                                        <div class='col-md-4 '> \
                                            <p class='form-control-static '>" + phone[i].Number + ((phone[i].Extension.length > 0) ? ' Ext:' + phone[i].Extension : '') + "</p> \
                                        </div> \
                                        <div id='editmenu' class='col-md-2 hiddenmenu'> \
                                            <p class='form-control-static'> \
                                            <a href='' id='" + phone[i].PhoneID + "' class='editphone'><span class='glyphicon glyphicon-pencil'></span></a>\
                                            <a href='' id='" + phone[i].PhoneID + "' class='delphone'><span class='glyphicon glyphicon-trash'></span></a/>\
                                            </p> \
                                        </div> \
                                    </div>");
            }
        });
    }

    function LoadAddresses() {
        $('#addressPanel').empty();
        top.Ts.Services.Customers.LoadAddresses(userID, top.Ts.ReferenceTypes.Users, function (address) {
            for (var i = 0; i < address.length; i++) {
                $('#addressPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-md-4 control-label'>" + address[i].Description + "</label> \
                                        <div class='col-md-5'> \
                                            " + ((address[i].Addr1.length > 0) ? "<p class='form-control-static'>" + address[i].Addr1 + "</p>" : "") + " \
                                            " + ((address[i].Addr2.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Addr2 + "</p>" : "") + " \
                                            " + ((address[i].Addr3.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Addr3 + "</p>" : "") + " \
                                            " + ((address[i].City.length > 0) ? "<p class='form-control-static pt0'>" + address[i].City + "</p>" : "") + " \
                                            " + ((address[i].State.length > 0) ? "<p class='form-control-static pt0'>" + address[i].State + "</p>" : "") + " \
                                            " + ((address[i].Zip.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Zip + "</p>" : "") + " \
                                            " + ((address[i].Country.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Country + "</p>" : "") + " \
                                            <p class='form-control-static'><a href='" + address[i].MapLink + "' target='_blank' id='" + address[i].AddressID + "' class='mapphone'><span class='glyphicon glyphicon-map-marker'></span></a>\
                                        </div> \
                                        <div id='editmenu' class='col-md-2 hiddenmenu'> \
                                            <a href='#' id='" + address[i].AddressID + "' class='editaddress'><span class='glyphicon glyphicon-pencil'></span></a>\
                                            <a href='#' id='" + address[i].AddressID + "' class='deladdress'><span class='glyphicon glyphicon-trash'></span></a/></p>\
                                        </div> \
                                    </div>");
            }
        });
    }

    function LoadPhoneTypes() {
        top.Ts.Services.Customers.LoadPhoneTypes(top.Ts.System.User.OrganizationID, function (pt) {
            for (var i = 0; i < pt.length; i++) {
                $('<option>').attr('value', pt[i].PhoneTypeID).text(pt[i].Name).data('o', pt[i]).appendTo('#phoneType');
            }
        });
    }

    function LoadProperties()
    {
        top.Ts.Services.Customers.LoadContactProperties(userID, function (user) {
            $('#userProperties').html(user);
            $('#userProperties p').toggleClass("editable");
            if ($('#fieldPortalUser').text() == "No")
                $('#btnSendNewPW').hide();

            $('#userProperties #fieldEmail').attr('mailto', $('#fieldEmail').text());
            $('#userProperties #fieldEmail').addClass("link");

            top.Ts.Services.Customers.GetUser(userID, function (user1) {
                $('#userProperties #fieldCompany').attr('orgID', user1.OrganizationID);
                $('#userProperties #fieldCompany').addClass("link");
            });
        });

    }

    function LoadCustomProperties() {
        top.Ts.Services.Customers.GetCustomValues(userID, top.Ts.ReferenceTypes.Contacts, function (html) {
            //$('#customProperties').append(html);
            appendCustomValues(html);

        });
    }

    function UpdateRecentView() {
        top.Ts.Services.Customers.UpdateRecentlyViewed("u"+userID, function (resultHtml) {
            if (window.parent.document.getElementById('iframe-mniCustomers'))
                window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
        });

    }

    function LoadReminderUsers() {
        var users = top.Ts.Cache.getUsers();
        if (users != null) {
            for (var i = 0; i < users.length; i++) {
                $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('#reminderUsers');
            }
        }
    }

    $('.number').on('keydown', function (event) {
        // Allow only backspace and delete
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
    });

    top.Ts.Services.Tickets.Load5MostRecentByContactID(userID, function (tickets) {
        var max = 5;
        if (tickets.length < 5)
            max = tickets.length;
        

        for (var i = 0; i < max; i++) {
            var div = $('<div>')
          .data('o', tickets[i])
          .addClass('ticket');

            $('<span>')
          .addClass('ts-icon ts-icon-info')
          .attr('rel', '../Tips/Ticket.aspx?TicketID=' + tickets[i].TicketID)
          .appendTo(div);

            var caption = $('<span>')
          .addClass('ticket-name')
          .appendTo(div);

            $('<a>')
          .addClass('ts-link ui-state-defaultx')
          .attr('href', '#')
          .text(tickets[i].TicketNumber + ': ' + tickets[i].Name)
          .appendTo(caption)
          .click(function (e) {

              top.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
          });


            div.appendTo(tickets[i].IsClosed == false ? '#openTickets' : '#closedTickets');
        }

        if ($('#openTickets .ticket').length < 1) {
            $('<div>')
            .addClass('no-tickets')
            .text('There are no recent tickets to display')
            .appendTo('#openTickets');
        }
    });

    $('#historyToggle').click(function (e) {
        LoadHistory();
    });

    function LoadHistory() {

        $('#tblHistory tbody').empty();
        top.Ts.Services.Customers.LoadContactHistory(userID, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                .appendTo('#tblHistory > tbody:last');
                //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
            }
        });
    }

    createTestChart();
    function createTestChart() {

        top.Ts.Services.Customers.LoadContactChartData(userID, true, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Open Tickets", 1]);
                //$('#openChart').text("No Open Tickes").addClass("text-center");
                $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center chart-header");
            }
            else
            $('#openChart').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    height: 250,
                },
                credits: {
                    enabled: false
                },
                title: {
                    text: 'Open Tickets'
                },
                tooltip: {
                    //formatter: function() {
                    //    var tooltip;
                    //    if (this.y == 0) {
                    //        tooltip = this.series.name + ": 0 - <b>100%</b>";
                    //    }
                    //    else {
                    //        tooltip = this.series.name + ": " + this.y + " - <b>"+ this.percentage +"</b>";
                    //        //tooltip =  '<span style="color:' + this.series.color + '">' + this.series.name + '</span>: <b>' + this.y + '</b><br/>';
                    //    }
                    //    return tooltip;
                    //}

                    pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        }
                    }
                },
                series: [{
                    type: 'pie',
                    name: 'Open Tickets',
                    data: []
                }]
            });

            var chart = $('#openChart').highcharts();
            chart.series[0].setData(chartData);
        });

        top.Ts.Services.Customers.LoadContactChartData(userID, false, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Closed Tickets", 1]);
                //$('#closedChart').text("No Closed Tickets").addClass("text-center");
                $('#closedChart').html("No Closed Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header");
            }
            else
            $('#closedChart').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    height: 250,
                },
                credits: {
                    enabled: false
                },
                title: {
                    text: 'Closed Tickets'
                },
                tooltip: {
                    pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: false
                        }
                    }
                },
                series: [{
                    type: 'pie',
                    name: 'Closed Tickets',
                    data: []
                }]
            });

            var chart = $('#closedChart').highcharts();
            chart.series[0].setData(chartData);
        });
    }
    $('#userProperties p').toggleClass("editable");
    

});

$.fn.autoGrow = function () {
    return this.each(function () {
        // Variables
        var colsDefault = 130; //this.cols;
        var rowsDefault = this.rows;

        //Functions
        var grow = function () {
            growByRef(this);
        }

        var growByRef = function (obj) {
            var linesCount = 0;
            var lines = obj.value.split('\n');

            for (var i = lines.length - 1; i >= 0; --i) {
                linesCount += Math.floor((lines[i].length / colsDefault) + 1);
            }

            if (linesCount > rowsDefault)
                obj.rows = linesCount + 1;
            else
                obj.rows = rowsDefault;
        }

        var characterWidth = function (obj) {
            var characterWidth = 0;
            var temp1 = 0;
            var temp2 = 0;
            var tempCols = obj.cols;

            obj.cols = 1;
            temp1 = obj.offsetWidth;
            obj.cols = 2;
            temp2 = obj.offsetWidth;
            characterWidth = temp2 - temp1;
            obj.cols = tempCols;

            return characterWidth;
        }

        // Manipulations
        //this.style.width = "auto";
        this.style.height = "auto";
        this.style.overflow = "hidden";
        //this.style.width = ((characterWidth(this) * this.cols) + 6) + "px";
        this.onkeyup = grow;
        this.onfocus = grow;
        this.onblur = grow;
        growByRef(this);
    });
};

var appendCustomValues = function (fields) {
    if (fields === null || fields.length < 1) {
        $('#customProperties').empty();
        return;
    }
    var container = $('#customProperties').empty().removeClass('ts-loading');
    for (var i = 0; i < fields.length; i++) {
        var item = null;

        var field = fields[i];

        var div = $('<div>').addClass('form-group').data('field', field);
        $('<label>')
          .addClass('col-md-4 control-label')
          .text(field.Name)
          .appendTo(div);

        switch (field.FieldType) {
            case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
            case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
            case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
            case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
            case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
            case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
            case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
            default:
        }

        container.append(div);
    }
    $('#customProperties p').toggleClass("editable");
    //$('#contactName').toggleClass("editable");
}

var appendCustomEditCombo = function (field, element) {
    var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var select = $('<select>').addClass('form-control').attr('id', field.Name.replace(/\s/g, '')).appendTo(container1);

          var items = field.ListValues.split('|');
          for (var i = 0; i < items.length; i++) {
              var option = $('<option>').text(items[i]).appendTo(select);
              if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
          }

          $('#' + field.Name.replace(/\s/g, '')).on('change', function () {
              var value = $(this).val();
              container.remove();

              if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
                  result.parent().addClass('has-error');
              }
              else {
                  result.parent().removeClass('has-error');
              }
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  parent.show();
              }, function () {
                  alert("There was a problem saving your contact property.");
              });
          });
      });
    var items = field.ListValues.split('|');
    if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditNumber = function (field, element) {
    var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-md-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .numeric()
            .focus();

          $('<i>')
            .addClass('col-md-1 glyphicon glyphicon-remove')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-md-1 glyphicon glyphicon-ok')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                }, function () {
                    alert("There was a problem saving your contact property.");
                });
                parent.show();
            })
            .insertAfter(container1);
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditBool = function (field, element) {

    var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          //$('.form-group').prev().show().next().remove();
          var parent = $(this);
          var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
          top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
          }, function () {
              alert("There was a problem saving your contact property.");
          });
      });
}

var appendCustomEdit = function (field, element) {
    
    var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

    var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-md-10 form-control')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-md-1 glyphicon glyphicon-remove')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-md-1 glyphicon glyphicon-ok')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                }, function () {
                    alert("There was a problem saving your contact property.");
                });
                parent.show();
            })
            .insertAfter(container1);
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }
}


var appendCustomEditDate = function (field, element) {
    var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getDatePattern()))
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-remove')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-ok')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate(input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                }, function () {
                    alert("There was a problem saving your contact property.");
                });
                parent.show();
            })
            .insertAfter(container1);
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

var appendCustomEditDateTime = function (field, element) {
    var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getDateTimePattern()))
            .datetimepicker({
            })

            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-remove')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-ok')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate(input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                }, function () {
                    alert("There was a problem saving your contact property.");
                });
                parent.show();
            })
            .insertAfter(container1);
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

var appendCustomEditTime = function (field, element) {
    var date = field.Value == null ? null : field.Value;

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
              return false;
          var parent = $(this).hide();

          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getTimePattern()))
            .datetimepicker({ pickDate: false })

            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-remove')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 glyphicon glyphicon-ok')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                }, function () {
                    alert("There was a problem saving your contact property.");
                });
                parent.show();
            })
            .insertAfter(container1);
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}