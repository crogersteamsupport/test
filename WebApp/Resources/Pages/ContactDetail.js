﻿/// <reference path="ts/ts.js" />
/// <reference path="ts/_mainFrame.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
var userID = null;
var ratingFilter = '';
var _execGetContact = null;
var _execGetAsset = null;
var _productsSortColumn = 'Date Created';
var _productsSortDirection = 'DESC';
var _productHeadersAdded = false;
var _completeCommentTaskID = 0;
var _userOrgID = null;

function getMainFrame() {
    var result = window.parent;
    var cnt = 0;
    while (!(result.Ts && result.Ts.Services)) {
        result = result.parent;
        cnt++;
        if (cnt > 5) return null;
    }
    return result;
}

var _mainFrame = getMainFrame();

$(document).ready(function () {
    userID = _mainFrame.Ts.Utils.getQueryValue("user", window);
    noteID = _mainFrame.Ts.Utils.getQueryValue("noteid", window);
    $('.customer-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    var _isAdmin = _mainFrame.Ts.System.User.IsSystemAdmin || _mainFrame.Ts.System.User.IsAdminOnlyCustomers;
    var historyLoaded = 0;
    $('input, textarea').placeholder();

    initEditor($('#fieldNoteDesc'), function (ed) {
        $('#fieldNoteDesc').tinymce().focus();
    });

    if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
        LoadProductFamilies();
        $('.productFamilyRow, .productFamilyColumn, .productLineRow').show();
    }

    LoadNotes();
    _mainFrame.Ts.Services.Customers.GetUser(userID, function (user) {
        _userOrgID = user.OrganizationID;        LoadNotesAdditional();
    });
    LoadFiles();
    LoadProductTypes();
    LoadCustomControls(_mainFrame.Ts.ReferenceTypes.UserProducts);
    LoadPhoneTypes();
    LoadPhoneNumbers();
    LoadAddresses();
    LoadProperties();
    LoadCustomProperties();
    LoadReminderUsers();
    UpdateRecentView();
    GetUser();
    LoadHubs();

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

    $('.ui-layout-north').css('zIndex', 3000);

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
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact');
        $('.userProperties p').toggleClass("editable");
        $('.customProperties p').toggleClass("editable");
        $("#emailPanel #editmenu").toggleClass("hiddenmenu");
        $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        $("#addressPanel #editmenu").toggleClass("hiddenmenu");

        if ($(".userProperties #fieldName").hasClass('editable'))
            $(".userProperties #fieldEmail").removeClass("link");
        else if ($(".userProperties #fieldEmail").text() != "Empty")
            $(".userProperties #fieldEmail").addClass("link");

        if ($(".userProperties #fieldLinkedIn").hasClass('editable'))
            $(".userProperties #fieldLinkedIn").removeClass("link");
        else if ($(".userProperties #fieldLinkedIn").text() != "Empty")
            $(".userProperties #fieldLinkedIn").addClass("link");

        $(".userProperties #fieldCompany").toggleClass("link");
        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
        if ($(this).hasClass("btn-primary"))
            $(this).html('<i class="fa fa-pencil"></i> Edit');
        else
            $(this).html('<i class="fa fa-pencil"></i> Save');
        //$('#contactName').toggleClass("editable");
        $('#contactTabs a:first').tab('show');
    });

    if (_mainFrame.Ts.System.Organization.ProductType == _mainFrame.Ts.ProductType.Enterprise) {
        $('#contactReminder').hide();
        $('#taskTab').show();
    }

    if (_mainFrame.Ts.System.Organization.ParentID != null) {
        $('#btnSendWelcome').hide();
    }

    if (noteID != null) {
        $('#contactTabs a:first').tab('show');
        $('#contactTabs a[href="#contact-notes"]').tab('show');
    }
    else {
        $('#contactTabs a:first').tab('show');
    }

    if (!_isAdmin && !_mainFrame.Ts.System.User.CanEditContact) {
        $('#contactEdit').hide();
        $('#contactPhoneButton').hide();
        $('#contactAddressButton').hide();
    }

    if (!_isAdmin) {
        $('#contactDelete').hide();
        $('#Contact-Merge').hide();
    }

    if (!_mainFrame.Ts.System.Organization.IsInventoryEnabled) {
        $('#contactTabs a[href="#contact-products"]').hide();
        $('#contactTabs a[href="#contact-inventory"]').hide();
    }

    if (!_mainFrame.Ts.System.User.CanEditContact && !_isAdmin) {
        $('#productToggle').hide();
    }

    $('#contactRefresh').click(function (e) {
        _mainFrame.Ts.System.logAction('Contact Detail - Refresh Window');
        window.location = window.location;
    });

    function getContacts(request, response) {
        if (_execGetContact) { _execGetContact._executor.abort(); }
        _execGetContact = _mainFrame.Ts.Services.Organizations.GetContactsExceptGiven(request.term, userID, function (result) { response(result); });
        isModified(true);
    }

    $("#Contact-Merge-search").autocomplete({
        minLength: 2,
        source: getContacts,
        select: function (event, ui) {
            if (ui.item.id == userID) {
                alert("Sorry, but you can not merge this contact into itself.");
                return;
            }

            $(this).data('userid', ui.item.id).removeClass('ui-autocomplete-loading');

            try {
                _mainFrame.Ts.Services.Organizations.GetUser(ui.item.id, function (info) {
                    var contactPreviewName = "<div><strong>Contact Name:</strong> " + info.FirstName + " " + info.MiddleName + " " + info.LastName + "</div>";
                    var contactPreviewEmail = "<div><strong>Contact Email:</strong> " + info.Email + "</div>";

                    $('#contactmerge-preview-details').after(contactPreviewName + contactPreviewEmail);
                    $('#dialog-contactmerge-preview').show();
                    $('#dialog-contactmerge-warning').show();
                    $(".dialog-contactmerge").dialog("widget").find(".ui-dialog-buttonpane").find(":button:contains('OK')").prop("disabled", false).removeClass("ui-state-disabled");
                })
            }
            catch (e) {
                alert("Sorry, there was a problem loading the information for that contact.");
            }
        },
        position: { my: "right top", at: "right bottom", collision: "fit flip" }
    });

    $('#contact-merge-complete').click(function (e) {
        e.preventDefault();
        $('#contact-merge-complete').attr('disabled', 'disabled');
        if ($('#Contact-Merge-search').val() == "") {
            alert("Please select a valid contact to merge");
            $('#contact-merge-complete').removeAttr('disabled');
            return;
        }

        if ($('#dialog-contactmerge-confirm').prop("checked")) {
            var winningID = $('#Contact-Merge-search').data('userid');
            //var winningContactName = $('#Contact-Merge-search').data('username');
            var JSTop = top;
            //var window = window;
            $('.merge-processing').show();
            _mainFrame.Ts.Services.Customers.MergeContacts(winningID, userID, function (result) {
                $('.merge-processing').hide();
                $('#contact-merge-complete').removeAttr('disabled');
                if (result != "")
                    alert(result);
                else {
                    $('#MergeModal').modal('hide');
                    JS_mainFrame.Ts.MainPage.closeNewContactTab(userID);
                    JS_mainFrame.Ts.MainPage.openNewContact(winningID);
                    //window.location = window.location;
                    //window.parent.ticketSocket.server.ticketUpdate(userID + "," + winningID, "merge", userFullName);
                }
            });
            //_mainFrame.Ts.Services.Tickets.MergeTickets(winningID, _ticketID, MergeSuccessEvent(_ticketNumber, winningTicketNumber),
            //  function () {
            //  $('#merge-error').show();
            //});
        }
        else {
            alert("You did not agree to the conditions of the merge. Please go back and check the box if you would like to merge.")
            $('#contact-merge-complete').removeAttr('disabled');
        }
    });

    $('#historyToggle').on('click', function () {
        _mainFrame.Ts.System.logAction('Contact Detail - Toggle History View');
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
        _mainFrame.Ts.System.logAction('Contact Detail - Refresh History');
        LoadHistory(1);
    });

    $("#btnSaveReminder").click(function (e) {
        _mainFrame.Ts.System.logAction('Contact Detail - Save Reminder');
        if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
            _mainFrame.Ts.Services.System.EditReminder(null, _mainFrame.Ts.ReferenceTypes.Contacts, userID, $('#reminderDesc').val(), _mainFrame.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val(), function () { });
            $('#modalReminder').modal('hide');
        }
        else
            alert("Please fill in all the fields");
    });

    $('#taskContainer').on('click', 'a.tasklink', function (e) {
        e.preventDefault();
        var id = $(this).data('taskid');
        parent.Ts.System.logAction('Tasks Page - View Task');
        parent.Ts.MainPage.openNewTask(id);
    });

    $('#taskContainer').on('click', '.change-task-status', function (e) {
        var id = $(this).data('taskid');
        var checkbox = $(this);
        var checked = $(this).prop("checked");
        parent.Ts.System.logAction('Contact Page - Change Task Status');

        parent.Ts.Services.Task.SetTaskIsCompleted(id, checked, function (data) {
            if (data.IncompleteSubtasks) {
                checkbox.prop("checked", false);
                alert('There are subtasks pending completion, please finish them before completing the parent task.')
            }
            else if (checked) {
                _completeCommentTaskID = id;
                $('#modalTaskComment').modal('show');
            }
        });
    });

    $('#contactDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this contact?')) {
            parent.privateServices.DeleteUser(userID, function (e) {
                _mainFrame.Ts.System.logAction('Contact Detail - Delete  Contact');
                if (window.parent.document.getElementById('iframe-mniCustomers'))
                    window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
                _mainFrame.Ts.MainPage.closeNewContactTab(userID);
                //_mainFrame.Ts.MainPage.closeNewContact(userID);
            });


        }
    });

    function GetUser() {
        _mainFrame.Ts.Services.Customers.GetUser(userID, function (user) {
            var firstLast = user.FirstName + " " + user.LastName;
            $('#contactName').text(user.FirstName + " " + user.LastName);

            var hasCustomerInsights = _mainFrame.Ts.System.Organization.IsCustomerInsightsActive;

            if (hasCustomerInsights) {
                var userAvatarPath = "../../../dc/" + user.OrganizationID + "/contactavatar/" + userID + "/48";
                $('#contactAvatar').attr("src", userAvatarPath);
            }
            else {
                $('#contactAvatar').hide();
            }

            $('.userProperties #fieldName').text(firstLast.length > 1 ? user.FirstName + " " + user.LastName : "Unassigned");
            $('.userProperties #fieldName').attr("first", user.FirstName);
            $('.userProperties #fieldName').attr("middle", user.MiddleName);
            $('.userProperties #fieldName').attr("last", user.LastName);
            parent.privateServices.SetUserSetting('SelectedOrganizationID', user.OrganizationID);
            parent.privateServices.SetUserSetting('SelectedContactID', user.UserID);
            if (user.Email != '') {
                LoadEmails();
            }
        });
    }


    $('#reminderDate').datetimepicker({});

    var execGetCompany = null;
    function getCompany(request, response) {
        if (execGetCompany) { execGetCompany._executor.abort(); }
        execGetCompany = _mainFrame.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
    }

    //$('#contactName').click(function (e) {
    $('.userProperties').on('click', '#fieldName', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        else {
            e.preventDefault();
            var fname, mname, lastname;
            var header = $(this).hide();
            _mainFrame.Ts.Services.Customers.GetUser(userID, function (user) {

                _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Name');
                var container = $('<form>')
                    .addClass('form-inline')
                    .insertAfter(header);

                var container1 = $('<div>')
                    .addClass('form-group')
                    .appendTo(container);

                $('<input type="text">')
                    .addClass('form-control')
                    .val(user.FirstName)
                    .attr('placeholder', 'First Name')
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

                        _mainFrame.Ts.Services.Customers.SetContactName(userID, $(this).prev().prev().prev().val(), $(this).prev().prev().val(), $(this).prev().val(), function (result) {
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

            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Email');
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
                    _mainFrame.Ts.Services.Customers.SetContactEmail(userID, $(this).prev().find('input').val(), function (result) {
                        header.text(result);
                        $('#contactEdit').removeClass("disabled");
                        if (result != 'Empty') {
                            $('#contactEmailButton').show();
                        }
                        else {
                            $('#contactEmailButton').hide();
                        }
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
    $('.userProperties').on('click', '#fieldLinkedIn', function (e) {
        if ($(this).hasClass('link')) {
            window.open($(this).text(), '_blank');
            return;
        }
        else {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;

            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact LinkedIn');
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
                    _mainFrame.Ts.Services.Customers.SetContactLinkedIn(userID, $(this).prev().find('input').val(), function (result) {
                        header.text(result);
                        $('#contactEdit').removeClass("disabled");
                    },
                        function (error) {
                            header.show();
                            alert('There was an error saving the customer linkedin.');
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

        _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Title');
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
                _mainFrame.Ts.Services.Customers.SetContactTitle(userID, $(this).prev().find('input').val(), function (result) {
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
        _mainFrame.Ts.Services.Customers.SetContactActive(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldActive').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Active State');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer active.');
            });
    });

    $('.userProperties').on('click', '#fieldPortalUser', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPortalUser(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPortalUser').text((result == 0 ? 'No' : 'Yes'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Portal User');
            if (result == 1 && (_isAdmin || _mainFrame.Ts.System.User.CanEditContact)) {
                $('#passwordResetBtnGroup').show();
            }
            else {
                $('#passwordResetBtnGroup').hide();
            }

            if (result == 2)
                if (confirm("This users company does not have portal access enabled. Would you like to enable it now?"))
                    _mainFrame.Ts.Services.Customers.SetCompanyPortalAccessUser(userID);

        },
            function (error) {
                header.show();
                alert('There was an error saving the customer portal user status.');
            });
    });

    $('.userProperties').on('click', '#fieldPortalViewOnly', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPortalViewOnly(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPortalViewOnly').text((result == 0 ? 'No' : 'Yes'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Portal View Only');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer portal view only status.');
            });
    });

    $('.userProperties').on('click', '#fieldDisableOrganizationTicketsViewonPortal', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPortalLimitOrgTickets(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldDisableOrganizationTicketsViewonPortal').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Portal Limit Org Tickets');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer Portal Limit Org Tickets status.');
            });
    });

    $('.userProperties').on('click', '#fieldDisableOrganizationChildrenTicketsViewonPortal', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPortalLimitOrgChildrenTickets(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldDisableOrganizationChildrenTicketsViewonPortal').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Portal Limit Org Children Tickets');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer Portal Limit Org Children Tickets status.');
            });
    });

    $('.userProperties').on('click', '#fieldCompany', function (e) {
        if ($(this).hasClass('link')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Open Contacts Company');
            _mainFrame.Ts.MainPage.openNewCustomer($(this).attr('orgID'));
            return;
        }
        else {

            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;

            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Company');
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
                    if (neworgID != undefined) {
                        _mainFrame.Ts.Services.Customers.SetContactCompany2(userID, neworgID, function (result) {
                            if (result == 'email already exists') {
                                header.show();
                                alert('A contact with the same email already exists in the new company.');
                                $('#contactEdit').removeClass("disabled");
                                return;
                            }
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
    $('.userProperties').on('click', '#fieldPreventemailfromcreatingandupdatingtickets', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPreventEmail(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPreventemailfromcreatingandupdatingtickets').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Prevent Email From Creating And Updating Tickets');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer block email status.');
            });
    });
    $('.userProperties').on('click', '#fieldPreventemailfromcreatingbutallowupdatingtickets', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactPreventEmailCreatingOnly(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldPreventemailfromcreatingbutallowupdatingtickets').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Prevent Email From Creating Only');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer block email creating only.');
            });
    });
    $('.userProperties').on('click', '#fieldSystemAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactSystemAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldSystemAdministrator').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Is Contact System Administrator');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer system admin status.');
            });
    });
    $('.userProperties').on('click', '#fieldFinancialAdministrator', function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        _mainFrame.Ts.Services.Customers.SetContactFinancialAdmin(userID, ($(this).text() !== 'Yes'), function (result) {
            $('#fieldFinancialAdministrator').text((result === true ? 'Yes' : 'No'));
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Contact Financial Administrator');
        },
            function (error) {
                header.show();
                alert('There was an error saving the customer financial admin status.');
            });
    });



    $('#noteToggle').click(function (e) {
        $('#noteForm').toggle();
        $('#fieldNoteTitle').focus();
        _mainFrame.Ts.System.logAction('Contact Detail - Toggle Note Form');
    });

    $('#fileToggle').click(function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Toggle File Form');
        $('#fileForm').toggle();
    });

    $('#productToggle').click(function (e) {
        _mainFrame.Ts.System.logAction('Contact Detail - Toggle Product Form');
        $('#productForm').toggle();
    });

    $("input[type=text], textarea").autoGrow();

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Tickets")
            $('#ticketIframe').attr("src", "tickettabs.html?ContactID=" + userID);
        else if (e.target.innerHTML == "Notes") {
            LoadNotes();
            LoadNotesAdditional();
        }
        else if (e.target.innerHTML == "Files")
            LoadFiles();
        else if (e.target.innerHTML == "Products")
            LoadProducts();
        else if (e.target.innerHTML == "Inventory")
            LoadInventory();
        else if (e.target.innerHTML == "Ratings")
            LoadRatings('', 1);
        else if (e.target.innerHTML == "Tasks")
            LoadTasks();
    })

    $('#emailPanel').on('click', '.delEmail', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this email?')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Delete Email');
            parent.privateServices.DeleteEmail($(this).attr('id'), function (e) {
                LoadEmails(1);
            });
        }
    });

    $("#emailPanel").on("click", '.editEmail', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Email');
        _mainFrame.Ts.Services.Customers.LoadEmail($(this).attr('id'), function (email) {
            $('#email').val(email[0].Email);
            $('#emailID').val(email[0].Id);
            $('#modalEmail').modal('show');
        });
    });

    $('#phonePanel').on('click', '.delphone', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this phone number?')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Delete Phone Number');
            parent.privateServices.DeletePhone($(this).attr('id'), function (e) {
                LoadPhoneNumbers(1);
            });
        }
    });

    $("#phonePanel").on("click", '.editphone', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Phone Number');
        _mainFrame.Ts.Services.Customers.LoadPhoneNumber($(this).attr('id'), function (phone) {
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
            _mainFrame.Ts.System.logAction('Contact Detail - Delete Address');
            parent.privateServices.DeleteAddress($(this).attr('id'), function (e) {
                LoadAddresses(1);
            });

        }
    });

    $("#addressPanel").on("click", '.editaddress', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Address');
        _mainFrame.Ts.Services.Customers.LoadAddress($(this).attr('id'), function (phone) {
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

    $("#btnEmailSave").click(function (e) {
        var emailInfo = new Object();
        _mainFrame.Ts.System.logAction('Contact Detail - Save Email');
        emailInfo.Email = $('#email').val();
        emailInfo.EmailID = $('#emailID').val() != "" ? $('#emailID').val() : "-1";
        var inEditmode = $('#contactEdit').hasClass("btn-success")
        _mainFrame.Ts.Services.Customers.SaveEmail(parent.JSON.stringify(emailInfo), userID, _mainFrame.Ts.ReferenceTypes.Users, function (f) {
            $('#email').val('');
            $('#emailID').val('-1');
            $('#modalEmail').modal('hide');
            if (inEditmode)
                LoadEmails(1);
            else
                LoadEmails();
        }, function () {
            alert('There was an error saving this phone number.  Please try again.');
        });

    });

    $("#btnPhoneSave").click(function (e) {
        var phoneInfo = new Object();
        _mainFrame.Ts.System.logAction('Contact Detail - Save Phone Number');
        phoneInfo.PhoneTypeID = $('#phoneType').val();
        phoneInfo.Number = $('#phoneNumber').val();
        phoneInfo.Extension = $('#phoneExt').val();
        phoneInfo.PhoneID = $('#phoneID').val() != "" ? $('#phoneID').val() : "-1";
        var inEditmode = $('#contactEdit').hasClass("btn-success")
        _mainFrame.Ts.Services.Customers.SavePhoneNumber(parent.JSON.stringify(phoneInfo), userID, _mainFrame.Ts.ReferenceTypes.Users, function (f) {
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
        _mainFrame.Ts.System.logAction('Contact Detail - Save Address');
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

        _mainFrame.Ts.Services.Customers.SaveAddress(parent.JSON.stringify(addressInfo), userID, _mainFrame.Ts.ReferenceTypes.Users, function (f) {

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
        _mainFrame.Ts.System.logAction('Contact Detail - View File Attachment');
        _mainFrame.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
    });

    $('#tblFiles').on('click', '.delFile', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this attachment?')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Delete File Attachment');
            parent.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
                LoadFiles();
            });

        }
    });

    $('#tblNotes').on('click', '.editNote', function (e) {
        e.stopPropagation();
        //$(this).prop('disabled', true);
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Note');
        _mainFrame.Ts.Services.Customers.LoadNote($(this).parent().parent().attr('id'), function (note) {
            $('#fieldNoteTitle').val(note.Title);
            var desc = note.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //$('#fieldNoteDesc').val(desc);
            $('#fieldNoteID').val(note.NoteID);
            $('#noteContactAlert').prop('checked', note.IsAlert);
            $('#btnNotesSave').text("Save");
            $('#btnNotesCancel').show();
            $('#noteForm').show();
            $('#fieldNoteDesc').tinymce().setContent(desc);
            $('#fieldNoteDesc').tinymce().focus();
            if (note.ProductFamilyID) {
                $('#ddlNoteProductFamily').val(note.ProductFamilyID);
            }
            else {
                $('#ddlNoteProductFamily').val(-1);
            }
        });
    });

    $('#tblNotes').on('click', '.deleteNote', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this note?')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Delete Note');
            parent.privateServices.DeleteNote($(this).parent().parent().attr('id'), function () {
                ;
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
        _mainFrame.Ts.System.logAction('Contact Detail - View Note');
    });

    $('#tblNotesAdditional').on('click', '.viewNote', function (e) {
        e.preventDefault();
        var desc = $(this).data('description');
        $('#tblNotesAdditional tbody tr').removeClass("active");

        $(this).addClass("active");
        $('.noteDesc').toggle();
        $('.noteDesc').html("<strong>Description</strong> <p>" + desc + "</p>");
        _mainFrame.Ts.System.logAction('Contact Detail - View Note');
    });

    $("#btnNotesCancel").click(function (e) {
        e.preventDefault();
        $('#fieldNoteTitle').val('');
        $('#fieldNoteDesc').val('');
        $('#fieldNoteID').val('-1');
        $('#noteContactAlert').prop('checked', false);
        $('#btnNotesSave').text("Save Note");
        $('#noteForm').toggle();
        _mainFrame.Ts.System.logAction('Contact Detail - Cancel Note Edit / Add');
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
        _mainFrame.Ts.System.logAction('Contact Detail - Save Note');
        var productFamilyID = $("#ddlNoteProductFamily").val();
        _mainFrame.Ts.Services.Customers.SaveNote(title, description, noteID, userID, _mainFrame.Ts.ReferenceTypes.Users, isAlert, productFamilyID, function (note) {
            $('#fieldNoteTitle').val('');
            $('#fieldNoteDesc').val('');
            $('#fieldNoteID').val('-1');
            $('#ddlNoteProductFamily').val('-1');
            $('#noteContactAlert').prop('checked', false);
            $('#btnNotesSave').text("Save Note");
            LoadNotes();
            $('#noteForm').toggle();
            $("#btnNotesSave").removeProp('disabled');
        });
    });

    $("#btnFilesCancel").click(function (e) {
        $('.upload-queue').empty();
        $('#attachmentDescription').val('');
        $('#ddlFileProductFamily').val('-1');
        $('#fileForm').toggle();
        _mainFrame.Ts.System.logAction('Contact Detail - Cancel File Upload');
    });

    $('#btnFilesSave').click(function (e) {
        $(this).prop('disabled', true);
        _mainFrame.Ts.System.logAction('Contact Detail - Save Files');
        if ($('.upload-queue li').length > 0) {
            $('.upload-queue li').each(function (i, o) {
                var data = $(o).data('data');
                data.formData = {
                    description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n"),
                    productFamilyID: $("#ddlFileProductFamily").val()
                };
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
                    .text(data.files[i].name + '  (' + _mainFrame.Ts.Utils.getSizeString(data.files[i].size) + ')')
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
            $('#ddlFileProductFamily').val('-1');
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
        _mainFrame.Ts.System.logAction('Contact Detail - Send Welcome Message');
        _mainFrame.Ts.Services.Customers.SendWelcome(userID, function (msg) {
            alert(msg);
        });
    });

    $('#hubPasswordResetList').on('click', 'a#btnSendNewPW', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Send New Password');
        _mainFrame.Ts.Services.Customers.PasswordReset(userID, function (msg) {
            alert(msg);
        });
    });

    $('#hubPasswordResetList').on('click', 'a.btnResetChubPW', function (e) {
        e.preventDefault();
        var productFamilyID = $(this).data('familyid');
        window.parent.parent.Ts.System.logAction('Contact Detail - Reset Hub Password');
        window.parent.parent.Ts.Services.Customers.ChubPasswordReset(userID, productFamilyID, function (msg) {
            alert(msg);
        });
    })

    _mainFrame.Ts.Services.Customers.GetContactTickets(userID, 0, function (e) {
        $('#openTicketCount').text("Open Tickets: " + e);
    });

    function LoadNotes() {
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadNotesByUserRights(userID, _mainFrame.Ts.ReferenceTypes.Users, false, _userOrgID, function (note) {
                $('#tblNotes tbody').empty();
                var html;
                for (var i = 0; i < note.length; i++) {
                    if (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditContact)
                        html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    else
                        html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';

                    if (note[i].ProductFamilyID != null) {
                        html += '<td>' + note[i].ProductFamily + '</td>';
                    }
                    else {
                        html += '<td>Unassigned</td>';
                    }

                    $('<tr>').addClass("viewNote")
                        .attr("id", note[i].NoteID)
                        .html(html)
                        .data("description", note[i].Description)
                        .appendTo('#tblNotes > tbody:last');
                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<strong>Description</strong> <p>" + note[i].Description + "</p>");
                        $('.noteDesc').show();
                    }
                }
            });
        }
        else {
            _mainFrame.Ts.Services.Customers.LoadNotes(userID, _mainFrame.Ts.ReferenceTypes.Users, function (note) {
                $('#tblNotes tbody').empty();
                var html;
                for (var i = 0; i < note.length; i++) {

                    if (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditContact)
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
    }

    //Load notes for organization
    function LoadNotesAdditional() {
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadNotesByUserRights(_userOrgID, _mainFrame.Ts.ReferenceTypes.Organizations, false, _userOrgID, function (note) {
                $('#tblNotesAdditional tbody').empty();                var html;                for (var i = 0; i < note.length; i++) {
                    html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';                    if (note[i].ProductFamilyID != null) {
                        html += '<td>' + note[i].ProductFamily + '</td>';
                    }                    else {
                        html += '<td>Unassigned</td>';
                    }
                    $('<tr>').addClass("viewNote")                    .attr("id", note[i].NoteID)                    .html(html)                    .data("description", note[i].Description)                    .appendTo('#tblNotesAdditional > tbody:last');                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<strong>Description</strong> <p>" + note[i].Description + "</p>");                        $('.noteDesc').show();
                    }
                }
            });
        }        else {
            _mainFrame.Ts.Services.Customers.LoadNotes2(_userOrgID, _mainFrame.Ts.ReferenceTypes.Organizations, false, function (note) {
                $('#tblNotesAdditional tbody').empty();                var html;                for (var i = 0; i < note.length; i++) {
                    if (!_isParentView && (_isAdmin || note[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditCompany))                        html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';                    else                        html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                    $('<tr>').addClass("viewNote")                    .attr("id", note[i].NoteID)                    .html(html)                    .data("description", note[i].Description)                    .appendTo('#tblNotesAdditional > tbody:last');
                    //$('#tblNotes > tbody:last').append('<tr id=' + note[i].NoteID + ' class="viewNote"><td><i class="glyphicon glyphicon-edit editNote"></i></td><td><i class="glyphicon glyphicon-trash deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td></tr>').data('description',note[i].Description);
                    if (noteID != null && noteID == note[i].NoteID) {
                        $('.noteDesc').html("<strong>Description</strong> <p>" + note[i].Description + "</p>");                        $('.noteDesc').show();
                    }
                }
            });
        }
    }

    function LoadFiles() {
        $('#tblFiles tbody').empty();
        if (_mainFrame.Ts.System.Organization.UseProductFamilies) {
            _mainFrame.Ts.Services.Customers.LoadFilesByUserRights(userID, _mainFrame.Ts.ReferenceTypes.Users, false, function (files) {
                var html;
                for (var i = 0; i < files.length; i++) {

                    if (_isAdmin || files[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditContact)
                        html = '<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';
                    else
                        html = '<td></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>';

                    if (files[i].ProductFamilyID != null) {
                        html += '<td>' + files[i].ProductFamily + '</td>';
                    }
                    else {
                        html += '<td>Unassigned</td>';
                    }

                    var tr = $('<tr>')
                        .attr('id', files[i].AttachmentID)
                        .html(html)
                        .appendTo('#tblFiles > tbody:last');


                    //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
                }
            });
        }
        else {
            _mainFrame.Ts.Services.Customers.LoadFiles(userID, _mainFrame.Ts.ReferenceTypes.Users, function (files) {
                var html;
                for (var i = 0; i < files.length; i++) {

                    if (_isAdmin || files[i].CreatorID == _mainFrame.Ts.System.User.UserID || _mainFrame.Ts.System.User.CanEditContact)
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
    }

    function LoadEmails(reload) {
        $('#emailPanel').empty();
        _mainFrame.Ts.Services.Customers.LoadEmails(userID, _mainFrame.Ts.ReferenceTypes.Users, function (emails) {
            for (var i = 0; i < emails.length; i++) {
                var emailNumber = i + 2;
                $('#emailPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>Email " + emailNumber + "</label> \
                                        <div class='col-md-5 '> \
                                            <p class='form-control-static '>" + emails[i].Email + "</p> \
                                        </div> \
                                        <div id='editmenu' class='col-md-2 hiddenmenu'> \
                                            <p class='form-control-static'> \
                                            <a href='' id='" + emails[i].Id + "' class='editEmail'><span class='fa fa-pencil'></span></a>\
                                            <a href='' id='" + emails[i].Id + "' class='delEmail'><span class='fa fa-trash-o'></span></a/>\
                                            </p> \
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#emailPanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadPhoneNumbers(reload) {
        $('#phonePanel').empty();
        _mainFrame.Ts.Services.Customers.LoadPhoneNumbers(userID, _mainFrame.Ts.ReferenceTypes.Users, function (phone) {
            for (var i = 0; i < phone.length; i++) {
                $('#phonePanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + phone[i].PhoneTypeName + "</label> \
                                        <div class='col-md-5 '> \
                                            <p class='form-control-static '><a href='tel:" + phone[i].Number + "'>" + phone[i].Number + "</a>" + ((phone[i].Extension != null && phone[i].Extension != '') ? ' Ext:' + phone[i].Extension : '') + "</p> \
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
        _mainFrame.Ts.Services.Customers.LoadAddresses(userID, _mainFrame.Ts.ReferenceTypes.Users, function (address) {
            for (var i = 0; i < address.length; i++) {
                $('#addressPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + address[i].Description + "</label> \
                                        <div class='col-md-5'> \
                                            " + ((address[i].Addr1 != null) ? "<p class='form-control-static'><a href='" + address[i].MapLink + "' target='_blank' id='" + address[i].AddressID + "' class='mapphone'><span class='fa fa-map-marker'></span></a> " + address[i].Addr1 + "</p>" : "") + " \
                                            " + ((address[i].Addr2 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr2 + "</p>" : "") + " \
                                            " + ((address[i].Addr3 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr3 + "</p>" : "") + " \
                                            " + ((address[i].City != null) ? "<p class='form-control-static pt0'>" + address[i].City + ((address[i].State != null) ? ", " + address[i].State : "") + ((address[i].Zip != null) ? " " + address[i].Zip : "") + "</p>" : "") + " \
                                            " + ((address[i].Country != null && address[i].Country.length > 0) ? "<p class='form-control-static pt0'>" + address[i].Country + "</p>" : "") + " \
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
        _mainFrame.Ts.Services.Customers.LoadPhoneTypes(_mainFrame.Ts.System.User.OrganizationID, function (pt) {
            for (var i = 0; i < pt.length; i++) {
                $('<option>').attr('value', pt[i].PhoneTypeID).text(pt[i].Name).data('o', pt[i]).appendTo('#phoneType');
            }
        });
    }

    function LoadProperties() {
        _mainFrame.Ts.Services.Customers.LoadContactProperties(userID, function (user) {

            $('#userInfo').html(user[0]);
            $('#userProp').html(user[1]);

            $('.userProperties p').toggleClass("editable");

            if ($('#fieldPortalUser').text() == "No")
                $('#passwordResetBtnGroup').hide();
            else if ($('#fieldPortalUser').text() == "Yes" || _isAdmin || _mainFrame.Ts.System.User.CanEditContact)
                $('#passwordResetBtnGroup').show();
            else
                $('#passwordResetBtnGroup').hide();

            if ($('#fieldEmail').text() != "Empty") {
                $('.userProperties #fieldEmail').attr('mailto', $('#fieldEmail').text());
                $('.userProperties #fieldEmail').addClass("link");
                $('#contactEmailButton').show();
            }

            _mainFrame.Ts.Services.Customers.GetUser(userID, function (user1) {
                $('.userProperties #fieldCompany').attr('orgID', user1.OrganizationID);
                $('.userProperties #fieldCompany').addClass("link");
            });

            if ($('#fieldLinkedIn').text() != "Empty") {
                $('.userProperties #fieldLinkedIn').addClass("link");
            }
        });

    }

    function LoadCustomProperties() {
        _mainFrame.Ts.Services.Customers.GetCustomValues(userID, _mainFrame.Ts.ReferenceTypes.Contacts, function (html) {
            //$('#customProperties').append(html);
            appendCustomValues(html);

        });
    }

    function UpdateRecentView() {
        _mainFrame.Ts.Services.Customers.UpdateRecentlyViewed("u" + userID, function (resultHtml) {
            if (window.parent.document.getElementById('iframe-mniCustomers'))
                window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
        });

    }

    function LoadReminderUsers() {
        var users = _mainFrame.Ts.Cache.getUsers();
        if (users != null) {
            for (var i = 0; i < users.length; i++) {
                var option = $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('#reminderUsers');
                if (_mainFrame.Ts.System.User.UserID === users[i].UserID) { option.attr('selected', 'selected'); }
            }
        }
    }

    $('.customProperties, #customProductsControls').on('keydown', '.number', function (event) {
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
        _mainFrame.Ts.Services.Customers.LoadAgentRatings2(userID, ratingOption, $('#tblRatings tbody > tr').length + 1, _mainFrame.Ts.ReferenceTypes.Users, $('#ddlRatingProductFamily').val(), function (ratings) {
            var agents = "";
            for (var i = 0; i < ratings.length; i++) {
                for (var j = 0; j < ratings[i].users.length; j++) {
                    if (j != 0)
                        agents = agents + ", ";

                    agents = agents + '<a href="#" target="_blank" onclick="_mainFrame.Ts.MainPage.openUser(' + ratings[i].users[j].UserID + '); return false;">' + ratings[i].users[j].FirstName + ' ' + ratings[i].users[j].LastName + '</a>';
                }

                var tr = $('<tr>')
                    //.html('<td><a href="' + _mainFrame.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="_mainFrame.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">Ticket ' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td>' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .html('<td><a href="' + _mainFrame.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="_mainFrame.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td><a href="#" onclick="_mainFrame.Ts.MainPage.openNewContact(' + ratings[i].reporter.UserID + '); return false;">' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</a></td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                    .appendTo('#tblRatings > tbody:last');

                agents = "";
            }

        });

        _mainFrame.Ts.Services.Organizations.GetAgentRatingOptions(_mainFrame.Ts.System.Organization.OrganizationID, function (o) {
            if (o != null) {
                if (o.PositiveImage)
                    $('#positiveImage').attr('src', o.PositiveImage);
                if (o.NeutralImage)
                    $('#neutralImage').attr('src', o.NeutralImage);
                if (o.NegativeImage)
                    $('#negativeImage').attr('src', o.NegativeImage);
            }
        });

        _mainFrame.Ts.Services.Customers.LoadRatingPercents2(userID, _mainFrame.Ts.ReferenceTypes.Users, $('#ddlRatingProductFamily').val(), function (results) {
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

    $('#ddlRatingProductFamily').change(function () {
        LoadRatings(ratingFilter, 1);
    });

    function LoadTasks() {
        parent.Ts.Services.Task.GetContactTasks(0, 20, userID, function (tasks) {
            var data = { taskList: tasks };
            var source = $("#contact-tasks-template").html();
            var template = Handlebars.compile(source);
            $("#tasks").html(template(data));
        });
    }

    _mainFrame.Ts.Services.Tickets.Load5MostRecentByContactID(userID, function (tickets) {
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

                    _mainFrame.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
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
        _mainFrame.Ts.Services.Customers.LoadContactHistory(userID, start, function (history) {
            for (var i = 0; i < history.length; i++) {
                $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
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

        _mainFrame.Ts.Services.Customers.LoadContactChartData(userID, true, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            var openCount = 0;

            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Open Tickets", 1]);
                //$('#openChart').text("No Open Tickes").addClass("text-center");
                $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center chart-header").attr("title", "No Open Tickets");
            }
            else {
                for (var i = 0; i < chartData.length; i++) {
                    openCount = openCount + chartData[i][1];
                }
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
                        text: 'Open Tickets ' + openCount,
                        style: {
                            "fontSize": "14px"
                        }
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

        _mainFrame.Ts.Services.Customers.LoadContactChartData(userID, false, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            var closedCount = 0;

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
            else {
                for (var i = 0; i < chartData.length; i++) {
                    closedCount = closedCount + chartData[i][1];
                }
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
                        text: 'Closed Tickets ' + closedCount,
                        style: {
                            "fontSize": "14px"
                        }
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

    _mainFrame.Ts.Services.Customers.LoadAlerts(userID, _mainFrame.Ts.ReferenceTypes.Users, function (notes) {
        for (var i = 0; i < notes.length; i++) {
            var note = notes[i];
            var description = $('<div>').html(note.Description);
            var buttons = [
                {
                    text: "Close",
                    click: function () {
                        $(this).dialog("close");
                    }
                },
                {
                    text: "Snooze",
                    click: function () {
                        _mainFrame.Ts.Services.Customers.SnoozeAlertByID($(this).data('noteId'), _mainFrame.Ts.ReferenceTypes.Users);
                        $(this).dialog("close");
                    }
                }
            ]

            if (!_mainFrame.Ts.System.Organization.HideDismissNonAdmins || _mainFrame.Ts.System.User.IsSystemAdmin) {
                buttons.push({
                    text: "Dismiss",
                    click: function () {
                        _mainFrame.Ts.Services.Customers.DismissAlertByID($(this).data('noteId'), _mainFrame.Ts.ReferenceTypes.Users);
                        $(this).dialog("close");
                    }
                });
            }

            var alert = $('<div>').prop('title', 'Alert message').data('noteId', note.NoteID).append(description).appendTo(document.body);
            alert.dialog({
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
        _mainFrame.Ts.Services.Customers.SnoozeAlert(userID, _mainFrame.Ts.ReferenceTypes.Users);
        $('#modalAlert').modal('hide');
        _mainFrame.Ts.System.logAction('Contact Detail - Snooze Alert');
    });

    $('#alertDismiss').click(function (e) {
        _mainFrame.Ts.Services.Customers.DismissAlert(userID, _mainFrame.Ts.ReferenceTypes.Users);
        _mainFrame.Ts.System.logAction('Contact Detail - Dismiss Alert');
        $('#modalAlert').modal('hide');
    });

    $('.tab-content').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            LoadRatings(ratingFilter, $('#tblRatings tbody > tr').length + 1);
        }
    });

    $('#productContact').val(userID);

    // Why do we have an autocomplete on a hidden field in customer page?
    //$('#productContact').autocomplete({
    //    minLength: 2,
    //    source: getCompany,
    //    select: function (event, ui) {
    //        $(this)
    //        .data('item', ui.item)
    //        .removeClass('ui-autocomplete-loading')
    //    }
    //});

    _mainFrame.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        $('.datepicker').attr("data-format", dateformat);
        $('.datepicker').datetimepicker({ pickTime: false });

        $('#productExpiration').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({});
    });

    $('#productProduct').change(function () {
        LoadProductVersions($(this).val(), -1);
    });

    $('#btnProductSave').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        _mainFrame.Ts.System.logAction('Contact Detail - Save Product');
        var productInfo = new Object();
        var hasError = 0;
        productInfo.UserID = $("#productContact").val();
        productInfo.ProductID = $("#productProduct").val();
        productInfo.Version = $("#productVersion").val();
        productInfo.SupportExpiration = $("#productExpiration").val();
        productInfo.UserProductID = $('#fieldProductID').val();

        productInfo.Fields = new Array();
        $('.customField:visible').each(function () {
            var field = new Object();
            field.CustomFieldID = $(this).attr("id");

            if ($(this).hasClass("required") && ($(this).val() === null || $.trim($(this).val()) === '')) {
                $(this).parent().addClass('has-error');
                hasError = 1;
            }
            else {
                $(this).parent().removeClass('has-error');
            }

            switch ($(this).attr("type")) {
                case "checkbox":
                    field.Value = $(this).prop('checked');
                    break;
                //case "date":
                //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate($(this).val());
                //    break;
                //case "time":
                //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
                //    break;
                //case "datetime":
                //    field.Value = $(this).val() == "" ? null : _mainFrame.Ts.Utils.getMsDate($(this).val());
                //    break;
                default:
                    field.Value = $(this).val();
            }
            productInfo.Fields[productInfo.Fields.length] = field;
        });

        if (hasError == 0) {
            _mainFrame.Ts.Services.Customers.SaveContactProduct(parent.JSON.stringify(productInfo), function (prod) {
                LoadProducts();
                $('#btnProductSave').text("Save Product");
                $('#productExpiration').val('');
                $('#fieldProductID').val('-1');
                $('#btnProductSave').text("Associate Product");
                $('.customField:visible').each(function () {
                    switch ($(this).attr("type")) {
                        case "checkbox":
                            $(this).prop('checked', false);
                            break;
                        default:
                            $(this).val('');
                    }
                });
                $('#productForm').toggle();
            }, function () {
                alert('There was an error saving this product association. Please try again.');
            });
        }

    });

    $('#tblProducts').on('click', '.productEdit', function (e) {
        e.preventDefault();
        var product = $(this).parent().parent().attr('id');
        var userProductID;
        _mainFrame.Ts.System.logAction('Contact Detail - Edit Product');
        _mainFrame.Ts.Services.Customers.LoadContactProduct(product, function (prod) {
            userProductID = prod.UserProductID;
            LoadProductVersions(prod.ProductID, prod.VersionNumber);
            $('#productProduct').val(prod.ProductID);
            $('#productExpiration').val(prod.SupportExpiration);
            $('#fieldProductID').val(userProductID);
            $('#btnProductSave').text("Save");
            _mainFrame.Ts.Services.Customers.LoadCustomContactProductFields(product, function (custField) {
                for (var i = 0; i < custField.length; i++) {
                    if (custField[i].FieldType == 2)
                        $('#' + custField[i].CustomFieldID).attr('checked', custField[i].Value);
                    //else if (custField[i].FieldType == 5)
                    //{
                    //    var date = field.value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);
                    //    $('#' + custField[i].CustomFieldID).val(date.localeFormat(_mainFrame.Ts.Utils.getDatePattern()));
                    //}

                    else
                        $('#' + custField[i].CustomFieldID).val(custField[i].Value);
                }
            });
        });

        $('#productForm').show();



    });

    $('#tblProducts').on('click', '.productHeader', function (e) {
        e.preventDefault();
        _productsSortColumn = $(this).text();
        var sortIcon = $(this).children(i);
        if (sortIcon.length > 0) {
            if (sortIcon.hasClass('fa-sort-asc')) {
                _productsSortDirection = 'DESC'
            }
            else {
                _productsSortDirection = 'ASC'
            }
            sortIcon.toggleClass('fa-sort-asc fa-sort-desc');
        }
        else {
            $('.productHeader').children(i).remove();
            var newSortIcon = $('<i>')
                .addClass('fa fa-sort-asc')
                .appendTo($(this));
            _customersSortDirection = 'ASC';
            switch (_productsSortColumn.toLowerCase()) {
                case "version":
                case "support expiration":
                case "released date":
                case "date created":
                    newSortIcon.toggleClass('fa-sort-asc fa-sort-desc');
                    _productsSortDirection = 'DESC';

            }
        }
        LoadProducts();
    });

    $("#btnProductCancel").click(function (e) {
        e.preventDefault();
        LoadProductTypes();
        _mainFrame.Ts.System.logAction('Contact Detail - Cancel Product Edit');
        $('#productExpiration').val('');
        $('#fieldProductID').val('-1');
        $('#btnProductSave').text("Associate Product");
        $('.customField:visible').each(function () {
            switch ($(this).attr("type")) {
                case "checkbox":
                    $(this).prop('checked', false);
                    break;
                default:
                    $(this).val('');
            }
        });
        $('#productForm').toggle();
    });

    $('#tblProducts').on('click', '.productDelete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this product association?')) {
            _mainFrame.Ts.System.logAction('Contact Detail - Delete Product');
            parent.privateServices.DeleteUserProduct($(this).parent().parent().attr('id'), function (e) {
                LoadProducts();
            });

        }
    });

    $('#tblProducts').on('click', '.productView', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - View Product');
        _mainFrame.Ts.MainPage.openUserProduct($(this).parent().parent().attr('id'))
        //parent.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    $('#tblProducts').on('click', '.productVersionView', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - View Product Version');
        _mainFrame.Ts.MainPage.openUserProductVersion($(this).parent().parent().attr('id'))
        //parent.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    function LoadProductTypes() {
        $('#productProduct').empty();
        _mainFrame.Ts.Services.Customers.LoadProductTypes(function (pt) {
            for (var i = 0; i < pt.length; i++) {
                if (i == 0)
                    LoadProductVersions(pt[i].ProductID, -1);
                $('<option>').attr('value', pt[i].ProductID).text(pt[i].Name).data('o', pt[i]).appendTo('#productProduct');
            }
        });
    }

    function LoadProductVersions(productID, selVal) {
        $("#productVersion").empty();

        _mainFrame.Ts.Services.Customers.LoadProductVersions(productID, function (pt) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#productVersion');
            for (var i = 0; i < pt.length; i++) {
                var opt = $('<option>').attr('value', pt[i].ProductVersionID).text(pt[i].VersionNumber).data('o', pt[i]);
                if (pt[i].ProductVersionID == selVal)
                    opt.attr('selected', 'selected');
                opt.appendTo('#productVersion');
            }
        });
    }

    $('.asset-action-assign').click(function (e) {
        e.preventDefault();
    });

    $("#dateShipped").datetimepicker();

    $('.assetList').on('click', '.assetLink', function (e) {
        e.preventDefault();
        _mainFrame.Ts.System.logAction('Contact Detail - Open Asset From List');
        _mainFrame.Ts.MainPage.openNewAsset(this.id);
    });

    var getAssets = function (request, response) {
        if (_execGetAsset) { _execGetAsset._executor.abort(); }
        _execGetAsset = _mainFrame.Ts.Services.Organizations.GetWarehouseAssets(request.term, function (result) { response(result); });
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

            _mainFrame.Ts.Services.Assets.AssignAsset($('#inputAsset').data('item').id, parent.JSON.stringify(assetAssignmentInfo), function (assetHtml) {
                _mainFrame.Ts.System.logAction('Contact Detail - Asset Assigned');
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
        //      _mainFrame.Ts.Services.System.EditReminder(null, _mainFrame.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), _mainFrame.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
        //      $('#modalReminder').modal('hide');
        //    }
        //    else
        //      alert("Please fill in all the fields");
    });

    function LoadProducts() {

        if (!_productHeadersAdded) {
            _mainFrame.Ts.Services.Customers.LoadcustomContactProductHeaders(function (headers) {
                for (var i = 0; i < headers.length; i++) {
                    $('#tblProducts th:last').after('<th>' + headers[i] + '</th>');
                }
                _productHeadersAdded = true;
                if (headers.length > 5) {
                    $('#productsContainer').addClass('expandProductsContainer');
                }
            });
        }

        $('#tblProducts tbody').empty();
        _mainFrame.Ts.Services.Customers.LoadContactProducts(userID, _productsSortColumn, _productsSortDirection, function (product) {
            for (var i = 0; i < product.length; i++) {
                var customfields = "";
                for (var p = 0; p < product[i].CustomFields.length; p++) {
                    customfields = customfields + "<td>" + product[i].CustomFields[p] + "</td>";
                }

                var html;

                if (_mainFrame.Ts.System.User.CanEditCompany || _isAdmin) {
                    html = '<td><i class="fa fa-edit productEdit"></i></td><td><i class="fa fa-trash-o productDelete"></i></td><td><a href="#" class="productView">' + product[i].ProductName + '</a></td><td><a href="#" class="productVersionView">' + product[i].VersionNumber + '</a></td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td>' + product[i].DateCreated + '</td>' + customfields;
                }
                else {
                    html = '<td></td><td></td><td><a href="#" class="productView">' + product[i].ProductName + '</a></td><td><a href="#" class="productVersionView">' + product[i].VersionNumber + '</a></td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td>' + product[i].DateCreated + '</td>' + customfields;
                }
                var tr = $('<tr>')
                    .attr('id', product[i].UserProductID)
                    .html(html)
                    .appendTo('#tblProducts > tbody:last');


                //$('#tblProducts > tbody:last').append('<tr><td><a href="#" id='+ product.ProductID +'><i class="glyphicon glyphicon-edit productEdit"></i></td><td><i class="glyphicon glyphicon-trash productDelete"></i></td><td><i class="fa fa-folder-open productView"></i></td><td>' + product[i].ProductName + '</td><td>' + product[i].VersionNumber + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td></td></tr>');
            }

            $('.products-loading').hide();
            $('.products-empty').hide();
            if (product.length == 0) {
                $('.products-empty').show();
            }
        });

    }

    function LoadCustomControls(refType) {
        _mainFrame.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
            $('#customProductsControls').append(html);

            $('#customProductsControls .datepicker').datetimepicker({ pickTime: false });
            $('#customProductsControls .datetimepicker').datetimepicker({});
            $('#customProductsControls .timepicker ').datetimepicker({ pickDate: false });
        });
    }

    function LoadInventory() {
        $('.assetList').empty();
        _mainFrame.Ts.Services.Customers.LoadAssets(userID, _mainFrame.Ts.ReferenceTypes.Contacts, function (assets) {
            $('.assetList').append(assets)
            //for (var i = 0; i < users.length; i++) {
            //    $('<a>').attr('class', 'list-group-item').text(users[i].FirstName + ' ' + users[i].LastName).appendTo('.userList');
            //}
        });
    }

    function LoadHubs() {
        _mainFrame.Ts.Services.Customers.LoadCustomerHubsByContactID(userID, function (hubs) {
            source = $("#hub-password-dropdown-template").html();

            var template = Handlebars.compile(source);
            data = { hubList: hubs };

            $("#hubPasswordResetList").html(template(data));
        })
    }

    Handlebars.registerHelper("formatDate", function (datetime) {
        if (datetime != null) {
            return parent.Ts.Utils.getMsDate(datetime).localeFormat(parent.Ts.Utils.getDatePattern());
        }
        else return null;
    });

    Handlebars.registerHelper("formatRow", function (task) {
        var cssClasses = null;

        if (task.DueDate != null) {
            if (task.IsComplete != true && new Date() > new Date(task.DueDate)) {
                cssClasses = 'over-due';
            }
            else {
                return null;
            }
        }

        return cssClasses;
    });

    Handlebars.registerHelper("taskComplete", function (isComplete) {
        return isComplete == true ? ' checked="checked"' : '';
    });

    Handlebars.registerHelper("mapAssociation", function (association) {
        var result = '';
        var functionName = '';
        var associationName = '';
        var iconClass = '';

        switch (association.RefType) {
            case 6:
                associationName = association.Group;
                iconClass = "fa-users";
                functionName = 'window.parent.parent.Ts.MainPage.openGroup(' + association.RefID + '); return false;';
                break;
            case 9:
                associationName = association.Company;
                iconClass = "fa-building";
                functionName = 'window.parent.parent.Ts.MainPage.openNewCustomer(' + association.RefID + '); return false;';
                break;
            case 13:
                associationName = association.Product;
                iconClass = "fa-truck";
                functionName = 'window.parent.parent.Ts.MainPage.openNewProduct(' + association.RefID + '); return false;';
                break;
            case 17:
                associationName = association.TicketName;
                iconClass = "fa-ticket";
                functionName = 'window.parent.parent.Ts.MainPage.openTicketByID(' + association.RefID + '); return false;'
                break;
            case 22:
                associationName = association.User;
                iconClass = "fa-user-circle-o";
                functionName = 'window.parent.parent.Ts.MainPage.openUser(' + association.RefID + '); return false;'
                break;
            case 32:
                associationName = association.Contact;
                iconClass = "fa-user";
                functionName = 'window.parent.parent.Ts.MainPage.openNewContact(' + association.RefID + '); return false;'
                break;
            default:
                functionName = null;
        }

        if (functionName != null) {
            result = '<span class="associations"><i target="_blank" class="ts-link fa ' + iconClass + '" href="#" onclick="' + functionName + '" title="' + associationName + '"></i></span>'
        }

        return new Handlebars.SafeString(result);
    });

    $('#btnTaskCompleteComment').on('click', function (e) {
        e.preventDefault();
        if ($('#taskCompleteComment').val() == '') {
            alert('Please type your comments before clicking on the Yes button.');
        }
        else {
            window.parent.parent.Ts.System.logAction('Task - Add Task Complete Comment');
            window.parent.parent.Ts.Services.Task.AddTaskCompleteComment(_completeCommentTaskID, $('#taskCompleteComment').val(), function (result) {
                if (result.Value) {
                    $('#taskCompleteComment').val('');
                    $('#modalTaskComment').modal('hide');
                }
                else {
                    alert('There was an error saving your comment. Please try again.')
                }
            });
        }
    });

    Handlebars.registerHelper("pluralize", function (hubs) {
        if (hubs.hubList.length <= 1) return;
        else return "(s)";
    });
});

var initEditor = function (element, init) {
    _mainFrame.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor",
            toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
            branding: false,
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
            moxiemanager_image_settings: {
                moxiemanager_rootpath: "/" + _mainFrame.Ts.System.Organization.OrganizationID + "/images/",
                extensions: 'gif,jpg,jpeg,png'
            },
            paste_data_images: true,
            images_upload_url: "/Services/UserService.asmx/SaveTinyMCEPasteImage",
            setup: function (ed) {
                ed.on('init', function (e) {
                    _mainFrame.Ts.System.refreshUser(function () {
                        if (_mainFrame.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(_mainFrame.Ts.System.User.FontFamily));
                        }
                        else if (_mainFrame.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(_mainFrame.Ts.System.Organization.FontFamily));
                        }

                        if (_mainFrame.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, _mainFrame.Ts.System.User.FontSizeDescription);
                        }
                        else if (_mainFrame.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, _mainFrame.Ts.System.Organization.FontSizeDescription);
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
            case _mainFrame.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
            case _mainFrame.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
            case _mainFrame.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
            case _mainFrame.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
            case _mainFrame.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
            case _mainFrame.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
            case _mainFrame.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
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
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Combobox');
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
                _mainFrame.Ts.System.logAction('Contact Detail - Save Custom Edit Change');
                _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
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
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Number');
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
                    _mainFrame.Ts.System.logAction('Contact Detail - Save Custom Number Edit');
                    _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
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
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Boolean Value');
            var parent = $(this);
            var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
            _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
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
                _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Textbox');
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
                        _mainFrame.Ts.System.logAction('Contact Detail - Save Custom Textbox Edit');
                        _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
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
    var date = field.Value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
        .addClass('col-xs-8')
        .appendTo(element);

    var result = $('<p>')
        .text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDatePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Date');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
                .addClass('col-xs-9')
                .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue === null ? '' : fieldValue.localeFormat(_mainFrame.Ts.Utils.getDatePattern()))
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
                    var value = _mainFrame.Ts.Utils.getMsDate(input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        result.parent().addClass('has-error');
                    }
                    else {
                        result.parent().removeClass('has-error');
                    }
                    _mainFrame.Ts.System.logAction('Contact Detail - Save Custom Date Change');
                    _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                        parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDatePattern())))
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
    var date = field.Value == null ? null : _mainFrame.Ts.Utils.getMsDate(field.Value);

    var div = $('<div>')
        .addClass('col-xs-8')
        .appendTo(element);

    var result = $('<p>')
        .text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom DateTime');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
                .addClass('col-xs-9')
                .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue === null ? '' : fieldValue.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern()))
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
                    var value = _mainFrame.Ts.Utils.getMsDate(input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        result.parent().addClass('has-error');
                    }
                    else {
                        result.parent().removeClass('has-error');
                    }
                    _mainFrame.Ts.System.logAction('Contact Detail - Save Custom DateTime');
                    _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                        parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getDateTimePattern())))
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
        .text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getTimePattern())))
        .addClass('form-control-static editable')
        .appendTo(div)
        .click(function (e) {
            e.preventDefault();
            if (!$(this).hasClass('editable'))
                return false;
            var parent = $(this).hide();
            _mainFrame.Ts.System.logAction('Contact Detail - Edit Custom Time');
            var container = $('<div>')
                .insertAfter(parent);

            var container1 = $('<div>')
                .addClass('col-xs-9')
                .appendTo(container);

            var fieldValue = parent.closest('.form-group').data('field').Value;
            var input = $('<input type="text">')
                .addClass('col-xs-10 form-control')
                .val(fieldValue === null ? '' : fieldValue.localeFormat(_mainFrame.Ts.Utils.getTimePattern()))
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
                    var value = _mainFrame.Ts.Utils.getMsDate("1/1/1900 " + input.val());
                    container.remove();
                    if (field.IsRequired && (value === null || $.trim(value) === '')) {
                        result.parent().addClass('has-error');
                    }
                    else {
                        result.parent().removeClass('has-error');
                    }
                    _mainFrame.Ts.System.logAction('Contact Detail - Save Custom Time');
                    _mainFrame.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                        parent.closest('.form-group').data('field', result);
                        var date = result.Value === null ? null : _mainFrame.Ts.Utils.getMsDate(result.Value);
                        parent.text((date === null ? 'Unassigned' : date.localeFormat(_mainFrame.Ts.Utils.getTimePattern())))
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
    _mainFrame.Ts.Services.Customers.LoadNote(noteID, function (note) {
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

function LoadProductFamilies() {
    _mainFrame.Ts.Services.Organizations.LoadOrgProductFamilies(_mainFrame.Ts.System.Organization.OrganizationID, function (productFamilies) {
        for (var i = 0; i < productFamilies.length; i++) {
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlNoteProductFamily');
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlFileProductFamily');
            $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlRatingProductFamily');
        }
    });
}