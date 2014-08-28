﻿/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
var userID = null;
var ratingFilter = '';
var _execGetAsset = null;
$(document).ready(function () {
    userID = top.Ts.Utils.getQueryValue("user", window);
    noteID = top.Ts.Utils.getQueryValue("noteid", window);
    $('.customer-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    var _isAdmin = top.Ts.System.User.IsSystemAdmin || top.Ts.System.User.IsAdminOnlyCustomers;
    var historyLoaded = 0;
    $('input, textarea').placeholder();

    initEditor($('#fieldNoteDesc'), function (ed) {
        $('#fieldNoteDesc').tinymce().focus();
    });

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

    //if (top.Ts.System.User.OrganizationID != 1078 && top.Ts.System.User.OrganizationID != 13679 && top.Ts.System.User.OrganizationID != 1088) {
        //$('#ratingsTab').hide();
    //}

    $(".maincontainer").on("keypress", "input", (function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    $('#contactEdit').click(function (e) {
        top.Ts.System.logAction('Contact Detail - Edit Contact');
        $('.userProperties p').toggleClass("editable");
        $('.customProperties p').toggleClass("editable");
        $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        $("#addressPanel #editmenu").toggleClass("hiddenmenu");

        if ($(".userProperties #fieldName").hasClass('editable'))
            $(".userProperties #fieldEmail").removeClass("link");
        else if ($(".userProperties #fieldEmail").text() != "Empty")
            $(".userProperties #fieldEmail").addClass("link");

        $(".userProperties #fieldCompany").toggleClass("link");
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

    if (!_isAdmin && !top.Ts.System.User.CanEditContact)
    {
        $('#contactEdit').hide();
        $('#contactPhoneButton').hide();
        $('#contactAddressButton').hide();
    }

    if (!_isAdmin)
        $('#contactDelete').hide();

    $('#contactRefresh').click(function (e) {
        top.Ts.System.logAction('Contact Detail - Refresh Window');
        window.location = window.location;
    });

    $('#historyToggle').on('click', function () {
        top.Ts.System.logAction('Contact Detail - Toggle History View');
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
        top.Ts.System.logAction('Contact Detail - Refresh History');
            LoadHistory(1);
    });

    $("#btnSaveReminder").click(function (e) {
        top.Ts.System.logAction('Contact Detail - Save Reminder');
        if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
            top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Users, userID, $('#reminderDesc').val(), top.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val(), function () { });
            $('#modalReminder').modal('hide');
        }
        else
            alert("Please fill in all the fields");
    });

    $('#contactDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this contact?')) {
            top.privateServices.DeleteUser(userID, function (e) {
                top.Ts.System.logAction('Contact Detail - Delete  Contact');
                if (window.parent.document.getElementById('iframe-mniCustomers'))
                    window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
                top.Ts.MainPage.closeNewContactTab(userID);
                //top.Ts.MainPage.closeNewContact(userID);
            });


        }
    });

    function GetUser(){
        top.Ts.Services.Customers.GetUser(userID, function (user) {
            var firstLast = user.FirstName + " " + user.LastName;
            $('#contactName').text(user.FirstName + " " + user.LastName);
            $('.userProperties #fieldName').text(firstLast.length > 1 ? user.FirstName + " " + user.LastName : "Unassigned");
            $('.userProperties #fieldName').attr("first", user.FirstName);
            $('.userProperties #fieldName').attr("middle", user.MiddleName);
            $('.userProperties #fieldName').attr("last", user.LastName);
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
    $('.userProperties').on('click', '#fieldName', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        else{
            e.preventDefault();
            var fname, mname, lastname;
            var header = $(this).hide();
            top.Ts.Services.Customers.GetUser(userID, function (user) {

                top.Ts.System.logAction('Contact Detail - Edit Contact Name');
                var container = $('<form>')
                  .addClass('form-inline')
                  .insertAfter(header);

                var container1 = $('<div>')
                    .addClass('form-group')
                  .appendTo(container);

                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.FirstName)
                  .attr('placeholder','First Name')
                  .appendTo(container1)
                  .focus();
                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.MiddleName)
                  .attr('placeholder', 'Middle Name')
                  .appendTo(container1)
                  .focus();
                $('<input type="text">')
                  .addClass('form-control')
                  .val(user.LastName)
                  .attr('placeholder', 'Last Name')
                  .appendTo(container1)
                  .focus();
                $('<i>')
                  .addClass('fa fa-check')
                  .click(function (e) {
                      if ($(this).prev().prev().prev().val() == "") {
                          alert("The first name can not be blank");
                          return;
                      }

                      top.Ts.Services.Customers.SetContactName(userID, $(this).prev().prev().prev().val(), $(this).prev().prev().val(), $(this).prev().val(), function (result) {
                          GetUser();
                          $('#contactEdit').removeClass("disabled");
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
                  .addClass('fa fa-times')
                  .click(function (e) {
                      $(this).closest('div').remove();
                      header.show();
                      $('#contactEdit').removeClass("disabled");
                  })
                  .appendTo(container1);
                $('input, textarea').placeholder();
                $('#contactEdit').addClass("disabled");
            });


        }

    });
    $('.userProperties').on('click', '#fieldEmail', function (e) {
        if ($(this).hasClass('link')) {
            window.location.href = "mailto:" + $('#fieldEmail').text();
            return;
        }
        else {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;

            top.Ts.System.logAction('Contact Detail - Edit Contact Email');
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
              .addClass('col-md-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  header.show();
                  $('#contactEdit').removeClass("disabled");
              })
              .insertAfter(container1);
            $('<i>')
              .addClass('col-md-1 fa fa-check')
              .click(function (e) {
                  top.Ts.Services.Customers.SetContactEmail(userID, $(this).prev().find('input').val(), function (result) {
                      header.text(result);
                      $('#contactEdit').removeClass("disabled");
                  },
                  function (error) {
                      header.show();
                      alert('There was an error saving the customer email.');
                      $('#contactEdit').removeClass("disabled");
                  });
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
            $('#contactEdit').addClass("disabled");
        }
    });
    $('.userProperties').on('click', '#fieldTitle', function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;

        top.Ts.System.logAction('Contact Detail - Edit Contact Title');
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
          .addClass('col-md-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-md-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetContactTitle(userID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#contactEdit').removeClass("disabled");
              },
              function (error) {
                  header.show();
                  alert('There was an error saving the customer title.');
                  $('#contactEdit').removeClass("disabled");
              });
              $(this).closest('div').remove();
              header.show();
              $('#contactEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#contactEdit').addClass("disabled");


    });
    $('.userProperties').on('click', '#fieldActive', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactActive(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldActive').text((result === true ? 'Yes' : 'No'));
            top.Ts.System.logAction('Contact Detail - Edit Contact Active State');
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
    });
    $('.userProperties').on('click', '#fieldPortalUser', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactPortalUser(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPortalUser').text((result == 0 ? 'No' : 'Yes'));
            top.Ts.System.logAction('Contact Detail - Edit Contact Portal User');
            if (result == 1 || _isAdmin || top.Ts.System.User.CanEditContact)
                $('#btnSendNewPW').show();
            else
                $('#btnSendNewPW').hide();

            if (result == 2)
                if (confirm("This users company does not have portal access enabled. Would you like to enable it now?"))
                    top.Ts.Services.Customers.SetCompanyPortalAccessUser(userID);
                
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer portal user status.');
        });
    });
    $('.userProperties').on('click', '#fieldDisableOrganizationTicketsViewonPortal', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactPortalLimitOrgTickets(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldDisableOrganizationTicketsViewonPortal').text((result === true ? 'Yes' : 'No'));
            top.Ts.System.logAction('Contact Detail - Edit Contact Portal Limit Org Tickets');
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer Portal Limit Org Tickets status.');
        });
    });
    $('.userProperties').on('click', '#fieldCompany', function (e) {
        if ($(this).hasClass('link')) {
            top.Ts.System.logAction('Contact Detail - Open Contacts Company');
            top.Ts.MainPage.openNewCustomer($(this).attr('orgID'));
            return;
        }
        else {

            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;

            top.Ts.System.logAction('Contact Detail - Edit Contact Company');
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
              .addClass('col-md-1 fa fa-times')
              .click(function (e) {
                  $(this).closest('div').remove();
                  header.show();
                  $('#contactEdit').removeClass("disabled");
              })
              .insertAfter(container1);
            $('<i>')
              .addClass('col-md-1 fa fa-check')
              .click(function (e) {
                  var neworgID = $(this).prev().find('input').data('item');
                  if(neworgID != undefined){
                  top.Ts.Services.Customers.SetContactCompany(userID, neworgID, function (result) {
                      header.text(result);
                      header.attr('orgid', neworgID);
                      $('#contactEdit').removeClass("disabled");
                  },
                  function (error) {
                      header.show();
                      alert('There was an error saving the customer company.');
                      $('#contactEdit').removeClass("disabled");
                  });
                  }
                  $(this).closest('div').remove();
                  header.show();
              })
              .insertAfter(container1);
            $('#contactEdit').addClass("disabled");
        }
    });
    $('.userProperties').on('click', '#fieldPreventemailfromcreatingtickets', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactPreventEmail(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPreventemailfromcreatingtickets').text((result === true ? 'Yes' : 'No'));
            top.Ts.System.logAction('Contact Detail - Edit Prevent Email From Creating Tickets');
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer block email status.');
        });
    });
    $('.userProperties').on('click', '#fieldSystemAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactSystemAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldSystemAdministrator').text((result === true ? 'Yes' : 'No'));
            top.Ts.System.logAction('Contact Detail - Edit Is Contact System Administrator');
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer system admin status.');
        });
    });
    $('.userProperties').on('click', '#fieldFinancialAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetContactFinancialAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldFinancialAdministrator').text((result === true ? 'Yes' : 'No'));
            top.Ts.System.logAction('Contact Detail - Edit Contact Financial Administrator');
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer financial admin status.');
        });
    });



    $('#noteToggle').click(function (e) {
        $('#noteForm').toggle();
        $('#fieldNoteTitle').focus();
        top.Ts.System.logAction('Contact Detail - Toggle Note Form');
    });

    $('#fileToggle').click(function (e) {
        top.Ts.System.logAction('Contact Detail - Toggle File Form');
        $('#fileForm').toggle();
    });

    $("input[type=text], textarea").autoGrow();

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Tickets")
            $('#ticketIframe').attr("src", "tickettabs.html?ContactID=" + userID);
        else if (e.target.innerHTML == "Notes")
            LoadNotes();
        else if (e.target.innerHTML == "Files")
            LoadFiles();
        else if (e.target.innerHTML == "Inventory")
            LoadInventory();
        else if (e.target.innerHTML == "Ratings")
            LoadRatings('', 1);
    })

    $('#phonePanel').on('click', '.delphone', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this phone number?')) {
            top.Ts.System.logAction('Contact Detail - Delete Phone Number');
            top.privateServices.DeletePhone($(this).attr('id'), function (e) {
                LoadPhoneNumbers(1);
            });
        }
    });

    $("#phonePanel").on("click", '.editphone', function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Contact Detail - Edit Phone Number');
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
                top.Ts.System.logAction('Contact Detail - Delete Address');
                top.privateServices.DeleteAddress($(this).attr('id'), function (e) {
                    LoadAddresses(1);
                });
                
            }
        });

    $("#addressPanel").on("click", '.editaddress', function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Contact Detail - Edit Address');
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
        top.Ts.System.logAction('Contact Detail - Save Phone Number');
        phoneInfo.PhoneTypeID = $('#phoneType').val();
        phoneInfo.Number = $('#phoneNumber').val();
        phoneInfo.Extension = $('#phoneExt').val();
        phoneInfo.PhoneID = $('#phoneID').val() != "" ? $('#phoneID').val() : "-1";
        var inEditmode = $('#contactEdit').hasClass("btn-success")
        top.Ts.Services.Customers.SavePhoneNumber(top.JSON.stringify(phoneInfo), userID, top.Ts.ReferenceTypes.Users, function (f) {
            $("#phoneType")[0].selectedIndex = 0;
            $('#phoneNumber').val('');
            $('#phoneExt').val('')
            $('#phoneID').val('-1');
            $('#modalPhone').modal('hide');
            if (inEditmode)
                LoadPhoneNumbers(1);
            else
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
        top.Ts.System.logAction('Contact Detail - Save Address');
        addressInfo.Description = $('#addressDesc').val();
        addressInfo.Addr1 = $('#address1').val();
        addressInfo.Addr2 = $('#address2').val();
        addressInfo.Addr3 = $('#address3').val();
        addressInfo.City = $('#addressCity').val();
        addressInfo.State = $('#addressState').val();
        addressInfo.Zip = $('#addressZip').val();
        addressInfo.Country = $('#addressCountry').val();
        addressInfo.AddressID = $('#addressID').val();
        var inEditmode = $('#customerEdit').hasClass("btn-success")

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
            if (inEditmode)
                LoadAddresses(1);
            else
                LoadAddresses();
        }, function () {
            alert('There was an error saving this address.  Please try again.');
        });

    });

    $('#tblFiles').on('click', '.viewFile', function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Contact Detail - View File Attachment');
        top.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
    });

    $('#tblFiles').on('click', '.delFile', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this attachment?')) {
            top.Ts.System.logAction('Contact Detail - Delete File Attachment');
            top.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
                LoadFiles();
            });
            
        }
    });

    $('#tblNotes').on('click', '.editNote', function (e) {
        e.stopPropagation();
        $(this).prop('disabled', true);
        top.Ts.System.logAction('Contact Detail - Edit Note');
        top.Ts.Services.Customers.LoadNote($(this).parent().parent().attr('id'), function (note) {
            $('#fieldNoteTitle').val(note.Title);
            var desc = note.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //$('#fieldNoteDesc').val(desc);
            $('#fieldNoteID').val(note.NoteID);
            $('#noteContactAlert').prop('checked', note.IsAlert);
            $('#btnNotesSave').text("Edit Note");
            $('#btnNotesCancel').show();
            $('#noteForm').show();
            $('#fieldNoteDesc').tinymce().setContent(desc);
            $('#fieldNoteDesc').tinymce().focus();
        });
    });

    $('#tblNotes').on('click', '.deleteNote', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this note?')) {
            top.Ts.System.logAction('Contact Detail - Delete Note');
            top.privateServices.DeleteNote($(this).parent().parent().attr('id'), function () {;
                LoadNotes();
                $('.noteDesc').toggle(false);
            });
        }
    });

    $('#tblNotes').on('click', '.viewNote', function (e) {
        e.preventDefault();
        var desc = $(this).data('description');
        $('#tblNotes tbody tr').removeClass("active");

        $(this).addClass("active");
        $('.noteDesc').toggle();
        $('.noteDesc').html("<strong>Description</strong> <p>" + desc + "</p>");
        top.Ts.System.logAction('Contact Detail - View Note');
    });

    $("#btnNotesCancel").click(function (e) {
        e.preventDefault();
        $('#fieldNoteTitle').val('');
        $('#fieldNoteDesc').val('');
        $('#fieldNoteID').val('-1');
        $('#noteContactAlert').prop('checked', false);
        $('#btnNotesSave').text("Save Note");
        $('#noteForm').toggle();
        top.Ts.System.logAction('Contact Detail - Cancel Note Edit / Add');
    });

    $("#btnNotesSave").click(function (e) {
        e.preventDefault();

        var title = $('#fieldNoteTitle').val();
        var description = $('#fieldNoteDesc').val();
        var noteID = $('#fieldNoteID').val();
        var isAlert = $('#noteContactAlert').prop('checked');
        if ((title.length || description.length) < 1) {
            alert("Please fill in all the required information");
            return;
        }
        $(this).prop('disabled', true);
        top.Ts.System.logAction('Contact Detail - Save Note');
        top.Ts.Services.Customers.SaveNote(title, description, noteID, userID, top.Ts.ReferenceTypes.Users, isAlert, function (note) {
            $('#fieldNoteTitle').val('');
            $('#fieldNoteDesc').val('');
            $('#fieldNoteID').val('-1');
            $('#noteContactAlert').prop('checked', false);
            $('#btnNotesSave').text("Save Note");
            LoadNotes();
            $('#noteForm').toggle();
            $(this).prop('disabled', false);
        });
    });

    $("#btnFilesCancel").click(function (e) {
        $('.upload-queue').empty();
        $('#attachmentDescription').val('');
        $('#fileForm').toggle();
        top.Ts.System.logAction('Contact Detail - Cancel File Upload');
    });

    $('#btnFilesSave').click(function (e) {
        $(this).prop('disabled', true);
        top.Ts.System.logAction('Contact Detail - Save Files');
        if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
                var data = $(o).data('data');
                data.formData = { description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n") };
                data.url = '../../../Upload/UserAttachments/' + userID;
                data.jqXHR = data.submit();
                $(o).data('data', data);
            });
        }
        //$('#fileForm').toggle();
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
                  .addClass('fa fa-times')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

                $('<span>')
                  .addClass('fa fa-times')
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
            //$('.progress-bar').css('width', '100%');
            LoadFiles();
            $('.upload-queue').empty();
            $('#attachmentDescription').val('');
            $('#fileForm').toggle();
            $('#btnFilesSave').prop('disabled', true);
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
        top.Ts.System.logAction('Contact Detail - Send Welcome Message');
        top.Ts.Services.Customers.SendWelcome(userID, function (msg) {
            alert(msg);
        });
    });

    $('#btnSendNewPW').click(function (e) {
        top.Ts.System.logAction('Contact Detail - Send New Password');
        top.Ts.Services.Customers.PasswordReset(userID, function (msg) {
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

                if (_isAdmin || note[i].CreatorID == top.Ts.System.User.UserID || top.Ts.System.User.CanEditContact)
                    html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                else
                    html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';


                $('<tr>').addClass("viewNote")
                .attr("id", note[i].NoteID)
                .html(html)
                .data("description", note[i].Description)
                .appendTo('#tblNotes > tbody:last');
                //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                if (noteID != null && noteID == note[i].NoteID) {
                  $('.noteDesc').html("<strong>Description</strong> <p>" + note[i].Description + "</p>");
                  $('.noteDesc').show();
                }
            }
        });
    }

    function LoadFiles() {
        $('#tblFiles tbody').empty();
        top.Ts.Services.Customers.LoadFiles(userID, top.Ts.ReferenceTypes.Users, function (files) {
            var html;
            for (var i = 0; i < files.length; i++) {

                if (_isAdmin || files[i].CreatorID == top.Ts.System.User.UserID || top.Ts.System.User.CanEditContact)
                    html = '<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
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

    function LoadPhoneNumbers(reload) {
        $('#phonePanel').empty();
        top.Ts.Services.Customers.LoadPhoneNumbers(userID,top.Ts.ReferenceTypes.Users, function (phone) {
            for (var i = 0; i < phone.length; i++) {
                $('#phonePanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + phone[i].PhoneTypeName + "</label> \
                                        <div class='col-md-5 '> \
                                            <p class='form-control-static '>" + phone[i].Number + ((phone[i].Extension != null && phone[i].Extension != '') ? ' Ext:' + phone[i].Extension : '') + "</p> \
                                        </div> \
                                        <div id='editmenu' class='col-md-2 hiddenmenu'> \
                                            <p class='form-control-static'> \
                                            <a href='' id='" + phone[i].PhoneID + "' class='editphone'><span class='fa fa-pencil'></span></a>\
                                            <a href='' id='" + phone[i].PhoneID + "' class='delphone'><span class='fa fa-trash-o'></span></a/>\
                                            </p> \
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadAddresses(reload) {
        $('#addressPanel').empty();
        top.Ts.Services.Customers.LoadAddresses(userID, top.Ts.ReferenceTypes.Users, function (address) {
            for (var i = 0; i < address.length; i++) {
                $('#addressPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + address[i].Description + "</label> \
                                        <div class='col-md-5'> \
                                            " + ((address[i].Addr1 != null ) ? "<p class='form-control-static'><a href='" + address[i].MapLink + "' target='_blank' id='" + address[i].AddressID + "' class='mapphone'><span class='fa fa-map-marker'></span></a> " + address[i].Addr1 + "</p>" : "") + " \
                                            " + ((address[i].Addr2 != null ) ? "<p class='form-control-static pt0'>" + address[i].Addr2 + "</p>" : "") + " \
                                            " + ((address[i].Addr3 != null ) ? "<p class='form-control-static pt0'>" + address[i].Addr3 + "</p>" : "") + " \
                                            " + ((address[i].City != null) ? "<p class='form-control-static pt0'>" + address[i].City + ((address[i].State != null) ? ", " + address[i].State : "") + ((address[i].Zip != null) ? " " + address[i].Zip : "") + "</p>" : "") + " \
                                            " + ((address[i].Country.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Country + "</p>" : "") + " \
                                        </div> \
                                        <div id='editmenu' class='col-md-2 hiddenmenu'> \
                                            <a href='#' id='" + address[i].AddressID + "' class='editaddress'><span class='fa fa-pencil'></span></a>\
                                            <a href='#' id='" + address[i].AddressID + "' class='deladdress'><span class='fa fa-trash-o'></span></a/>\
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
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

            $('#userInfo').html(user[0]);
            $('#userProp').html(user[1]);

            $('.userProperties p').toggleClass("editable");

            if ($('#fieldPortalUser').text() == "No")
                $('#btnSendNewPW').hide();
            else if ($('#fieldPortalUser').text() == "Yes" || _isAdmin || top.Ts.System.User.CanEditContact)
                $('#btnSendNewPW').show();
            else
                $('#btnSendNewPW').hide();

            if($('#fieldEmail').text() != "Empty")
            {
                $('.userProperties #fieldEmail').attr('mailto', $('#fieldEmail').text());
                $('.userProperties #fieldEmail').addClass("link");
            }

            top.Ts.Services.Customers.GetUser(userID, function (user1) {
                $('.userProperties #fieldCompany').attr('orgID', user1.OrganizationID);
                $('.userProperties #fieldCompany').addClass("link");
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
                var option =  $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('#reminderUsers');
                if (top.Ts.System.User.UserID === users[i].UserID) { option.attr('selected', 'selected'); }
            }
        }
    }

    $('.customProperties').on('keydown', '.number', function (event) {
        // Allow only backspace and delete
        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
            // let it happen, don't do anything
        }
        else {
            // Ensure that it is a number and stop the keypress
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
    });

    function LoadRatings(ratingOption, start) {

        if (start == 1)
            $('#tblRatings tbody').empty();
        top.Ts.Services.Customers.LoadAgentRatings(userID, ratingOption, $('#tblRatings tbody > tr').length + 1, top.Ts.ReferenceTypes.Users, function (ratings) {
            var agents = "";
            for (var i = 0; i < ratings.length; i++) {
                for (var j = 0; j < ratings[i].users.length; j++) {
                    if (j != 0)
                        agents = agents + ", ";

                    agents = agents + '<a href="#" target="_blank" onclick="top.Ts.MainPage.openUser(' + ratings[i].users[j].UserID + '); return false;">' + ratings[i].users[j].FirstName + ' ' + ratings[i].users[j].LastName + '</a>';
                }

                var tr = $('<tr>')
                //.html('<td><a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">Ticket ' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td>' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .html('<td><a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td><a href="#" onclick="top.Ts.MainPage.openNewContact(' + ratings[i].reporter.UserID + '); return false;">' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</a></td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .appendTo('#tblRatings > tbody:last');

                agents = "";
            }

        });

        top.Ts.Services.Organizations.GetAgentRatingOptions(top.Ts.System.Organization.OrganizationID, function (o) {
            if (o != null) {
                if (o.PositiveImage)
                    $('#positiveImage').attr('src', o.PositiveImage);
                if (o.NeutralImage)
                    $('#neutralImage').attr('src', o.NeutralImage);
                if (o.NegativeImage)
                    $('#negativeImage').attr('src', o.NegativeImage);
            }
        });

        top.Ts.Services.Customers.LoadRatingPercents(userID, top.Ts.ReferenceTypes.Users, function (results) {
            $('#negativePercent').text(results[0] + "%");
            $('#neutralPercent').text(results[1] + "%");
            $('#positivePercent').text(results[2] + "%");
        });
    }

    $('#positiveImage').click(function () {
        LoadRatings(1, 1);
        ratingFilter = 1;
    });
    $('#neutralImage').click(function () {
        LoadRatings(0, 1);
        ratingFilter = 0;
    });
    $('#negativeImage').click(function () {
        LoadRatings(-1, 1);
        ratingFilter = -1;
    });
    $('#viewAll').click(function () {
        LoadRatings('', 1);
        ratingFilter = '';
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
          .text(tickets[i].TicketNumber + ': ' + ellipseString(tickets[i].Name, 50))
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

    function LoadHistory(start) {

        if (start == 1)
        $('#tblHistory tbody').empty();
        top.Ts.Services.Customers.LoadContactHistory(userID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
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
                $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center chart-header").attr("title","No Open Tickets");
            }
            else{
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
            }
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
                $('#closedChart').html("No Closed Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Closed Tickets");
            }
            else{
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
            }
        });
    }
    $('.userProperties p').toggleClass("editable");
    
    top.Ts.Services.Customers.LoadAlert(userID, top.Ts.ReferenceTypes.Users, function (note) {
        if (note != null) {
            $('#modalAlertMessage').html(note.Description);
            var buttons = {
                "Close": function () {
                    $(this).dialog("close");
                },
                "Snooze": function () {
                    top.Ts.Services.Customers.SnoozeAlert(userID, top.Ts.ReferenceTypes.Users);
                    $(this).dialog("close");
                }
            }

            if (!top.Ts.System.Organization.HideDismissNonAdmins || top.Ts.System.User.IsSystemAdmin) {
                buttons["Dismiss"] = function () {
                    top.Ts.Services.Customers.DismissAlert(userID, top.Ts.ReferenceTypes.Users);
                    $(this).dialog("close");
                }
            }

            $("#dialog").dialog({
                resizable: false,
                width: 'auto',
                height: 'auto',
                create: function () {
                    $(this).css('maxWidth', '800px');
                },
                modal: true,
                buttons: buttons
            });

        }
    });

    $('#alertSnooze').click(function (e) {
        top.Ts.Services.Customers.SnoozeAlert(userID, top.Ts.ReferenceTypes.Users);
        $('#modalAlert').modal('hide');
        top.Ts.System.logAction('Contact Detail - Snooze Alert');
    });

    $('#alertDismiss').click(function (e) {
        top.Ts.Services.Customers.DismissAlert(userID, top.Ts.ReferenceTypes.Users);
        top.Ts.System.logAction('Contact Detail - Dismiss Alert');
        $('#modalAlert').modal('hide');
    });

    $('.tab-content').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            LoadRatings(ratingFilter, $('#tblRatings tbody > tr').length + 1);
        }
    });

    $('.asset-action-assign').click(function (e) {
        e.preventDefault();
    });

    $("#dateShipped").datetimepicker();

    $('.assetList').on('click', '.assetLink', function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Contact Detail - Open Asset From List');
        top.Ts.MainPage.openNewAsset(this.id);
    });

    var getAssets = function (request, response) {
      if (_execGetAsset) { _execGetAsset._executor.abort(); }
      _execGetAsset = top.Ts.Services.Organizations.GetWarehouseAssets(request.term, function (result) { response(result); });
    }

    $('#inputAsset').autocomplete({
      open: function () {
        $('.ui-menu').width($('#inputAsset').width());
      },
      minLength: 2,
      source: getAssets,
      select: function (event, ui) {
        $(this).data('item', ui.item);
      }
    });

    $('#btnSaveAssign').click(function (e) {
      if ($('#inputAsset').data('item') && $('#dateShipped').val()) {
        var assetAssignmentInfo = new Object();

        assetAssignmentInfo.RefID = userID;
        assetAssignmentInfo.RefType = 32;
        assetAssignmentInfo.DateShipped = $('#dateShipped').val();
        assetAssignmentInfo.TrackingNumber = $('#trackingNumber').val();
        assetAssignmentInfo.ShippingMethod = $('#shippingMethod').val();
        assetAssignmentInfo.ReferenceNumber = $('#referenceNumber').val();
        assetAssignmentInfo.Comments = $('#comments').val();
        assetAssignmentInfo.AssigneeName = $('#contactName').text() + ' [' + $('#fieldCompany').text() + ']';

        top.Ts.Services.Assets.AssignAsset($('#inputAsset').data('item').id, top.JSON.stringify(assetAssignmentInfo), function (assetHtml) {
          top.Ts.System.logAction('Contact Detail - Asset Assigned');
          $('#modalAssign').modal('hide');
          $('.assetList').prepend(assetHtml);
        }, function () {
          alert('There was an error assigning this asset.  Please try again.');
        });
      }
      else {
        if (!$('#inputAsset').data('item')) {
          alert("Please select a valid asset to assign to this customer.");
        }
        else {
          alert("Please enter a valid date shipped.");
        }
      }
      //    if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
      //      top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), top.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
      //      $('#modalReminder').modal('hide');
      //    }
      //    else
      //      alert("Please fill in all the fields");
    });

    function LoadInventory() {
        $('.assetList').empty();
        top.Ts.Services.Customers.LoadAssets(userID, top.Ts.ReferenceTypes.Contacts, function (assets) {
            $('.assetList').append(assets)
            //for (var i = 0; i < users.length; i++) {
            //    $('<a>').attr('class', 'list-group-item').text(users[i].FirstName + ' ' + users[i].LastName).appendTo('.userList');
            //}
        });
    }

});

var initEditor = function (element, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor",
            toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
            statusbar: false,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            body_class: "ui-widget ui-widget-content",

            convert_urls: true,
            remove_script_host: false,
            relative_urls: false,
            template_external_list_url: "tinymce/jscripts/template_list.js",
            external_link_list_url: "tinymce/jscripts/link_list.js",
            external_image_list_url: "tinymce/jscripts/image_list.js",
            media_external_list_url: "tinymce/jscripts/media_list.js",
            menubar: false,

            setup: function (ed) {
                ed.on('init', function (e) {
                    top.Ts.System.refreshUser(function () {
                        if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
                        }
                        else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
                        }

                        if (top.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
                        }
                        else if (top.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSizeDescription);
                        }
                    });
                });

                ed.on('paste', function (ed, e) {
                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                });
            }
            , oninit: init
        };
        $(element).tinymce(editorOptions);
    });
}

var getUrls = function (input) {
    var source = (input || '').toString();
    var url;
    var matchArray;
    var result = '';

    // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
    var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;

    // Iterate through any URLs in the text.
    while ((matchArray = regexToken.exec(source)) !== null) {
        url = matchArray[0];
        if (url.length > 2 && url.substring(0, 3) == 'www') {
            url = 'http://' + url;
        }
        result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '">' + matchArray[0] + '</a>'
    }

    return result == '' ? input : result;
}

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
        $('.customProperties').empty();
        return;
    }
    var containerL = $('#customPropertiesL').empty();
    var containerR = $('#customPropertiesR').empty();


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

        if (i % 2)
            containerR.append(div);
        else
            containerL.append(div);
        
    }
    $('.customProperties p').toggleClass("editable");
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
          top.Ts.System.logAction('Contact Detail - Edit Custom Combobox');
          var container = $('<div>')
            .insertAfter(parent);

          var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var select = $('<select>').addClass('form-control').attr('id', field.Name.replace(/\W/g, '')).appendTo(container1);

          var items = field.ListValues.split('|');
          for (var i = 0; i < items.length; i++) {
              var option = $('<option>').text(items[i]).appendTo(select);
              if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
          }

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);

          $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
              var value = $(this).val();
              container.remove();

              if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
                  result.parent().addClass('has-error');
              }
              else {
                  result.parent().removeClass('has-error');
              }
              top.Ts.System.logAction('Contact Detail - Save Custom Edit Change');
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  parent.show();
                  $('#contactEdit').removeClass("disabled");
              }, function () {
                  alert("There was a problem saving your contact property.");
                  $('#contactEdit').removeClass("disabled");
              });
          });

          $('#contactEdit').addClass("disabled");
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
          top.Ts.System.logAction('Contact Detail - Edit Custom Number');
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
            .focus();

          $('<i>')
            .addClass('col-md-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-md-1 fa fa-check')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.System.logAction('Contact Detail - Save Custom Number Edit');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                    $('#contactEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#contactEdit').removeClass("disabled");
                });
                parent.show();
                $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('#contactEdit').addClass("disabled");
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
          top.Ts.System.logAction('Contact Detail - Edit Custom Boolean Value');
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
      .html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
          if ($(this).has('a') && !$(this).hasClass('editable')) {
              return;
          }
          else {
              e.preventDefault();
              if (!$(this).hasClass('editable'))
                  return false;
              var parent = $(this).hide();
              top.Ts.System.logAction('Contact Detail - Edit Custom Textbox');
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

              if (field.Mask) {
                input.mask(field.Mask);
                input.attr("placeholder", field.Mask);
              }

              $('<i>')
                .addClass('col-md-1 fa fa-times')
                .click(function (e) {
                    $(this).closest('div').remove();
                    parent.show();
                    $('#contactEdit').removeClass("disabled");
                })
                .insertAfter(container1);
              $('<i>')
                .addClass('col-md-1 fa fa-check')
                .click(function (e) {
                    var value = input.val();
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        result.parent().addClass('has-error');
                    }
                    else {
                        result.parent().removeClass('has-error');
                    }
                    top.Ts.System.logAction('Contact Detail - Save Custom Textbox Edit');
                    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                        $('#contactEdit').removeClass("disabled");
                    }, function () {
                        alert("There was a problem saving your contact property.");
                        $('#contactEdit').removeClass("disabled");
                    });
                    parent.show();
                })
                .insertAfter(container1);
              $('#contactEdit').addClass("disabled");
          }
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
          top.Ts.System.logAction('Contact Detail - Edit Custom Date');
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
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate(input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.System.logAction('Contact Detail - Save Custom Date Change');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                    $('#contactEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#contactEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#contactEdit').addClass("disabled");
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
          top.Ts.System.logAction('Contact Detail - Edit Custom DateTime');
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
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate(input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.System.logAction('Contact Detail - Save Custom DateTime');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                    $('#contactEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#contactEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#contactEdit').addClass("disabled");
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
          top.Ts.System.logAction('Contact Detail - Edit Custom Time');
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
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.System.logAction('Contact Detail - Save Custom Time');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                    $('#contactEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#contactEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#contactEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

function openNote(noteID) {
    top.Ts.Services.Customers.LoadNote(noteID, function (note) {
        var desc = note.Description;
        desc = desc.replace(/<br\s?\/?>/g, "\n");
        $('.noteDesc').show();
        $('.noteDesc').html("<strong>Description</strong> <p>" + desc + "</p>");
        $('#contactTabs a[href="#contact-notes"]').tab('show');
    });
}

function GetTinyMCEFontName(fontFamily) {
  var result = '';
  switch (fontFamily) {
    case 1:
      result = "'andale mono', times";
      break;
    case 2:
      result = "arial, helvetica, sans-serif";
      break;
    case 3:
      result = "'arial black', 'avant garde'";
      break;
    case 4:
      result = "'book antiqua', palatino";
      break;
    case 5:
      result = "'comic sans ms', sans-serif";
      break;
    case 6:
      result = "'courier new', courier";
      break;
    case 7:
      result = "georgia, palatino";
      break;
    case 8:
      result = "helvetica";
      break;
    case 9:
      result = "impact, chicago";
      break;
    case 10:
      result = "symbol";
      break;
    case 11:
      result = "tahoma, arial, helvetica, sans-serif";
      break;
    case 12:
      result = "terminal, monaco";
      break;
    case 13:
      result = "'times new roman', times";
      break;
    case 14:
      result = "'trebuchet ms', geneva";
      break;
    case 15:
      result = "verdana, geneva";
      break;
    case 16:
      result = "webdings";
      break;
    case 17:
      result = "wingdings, 'zapf dingbats'";
      break;
  }
  return result;
}