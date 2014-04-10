/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var customerDetailPage = null;
var organizationID = null;

$(document).ready(function () {
    customerDetailPage = new CustomerDetailPage();
    customerDetailPage.refresh();
    $('.customer-tooltip').tooltip({ placement: 'bottom', container: 'body' });


    initEditor($('#fieldNoteDesc'), function (ed) {
        $('#fieldNoteDesc').tinymce().focus();
    });

    $('input, textarea').placeholder();
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

    organizationID = top.Ts.Utils.getQueryValue("organizationid", window);
    noteID = top.Ts.Utils.getQueryValue("noteid", window);
    var _isAdmin = top.Ts.System.User.IsSystemAdmin && (organizationID != top.Ts.System.User.OrganizationID);
    var historyLoaded = 0;
    top.privateServices.SetUserSetting('SelectedOrganizationID', organizationID);
    top.privateServices.SetUserSetting('SelectedContactID', -1);

    LoadNotes();
    //LoadHistory();
    LoadFiles();
    LoadPhoneTypes();
    LoadPhoneNumbers();
    LoadAddresses();
    LoadProperties();
    LoadCustomProperties();
    LoadContacts();
    LoadProducts();
    LoadProductTypes();
    LoadCustomControls(top.Ts.ReferenceTypes.OrganizationProducts);
    LoadReminderUsers();
    UpdateRecentView();

    top.Ts.Services.Customers.LoadAlert(organizationID, top.Ts.ReferenceTypes.Organizations, function (note) {
        if (note != null) {
            $('#modalAlertMessage').html(note.Description);
            //$('#modalAlert').modal('show');
            $("#dialog").dialog({
                resizable: false,
                width: 'auto',
                height: 'auto',
                modal: true,
                buttons: {
                    "Close": function () {
                        $(this).dialog("close");
                    },
                    "Snooze": function () {
                        top.Ts.Services.Customers.SnoozeAlert(organizationID, top.Ts.ReferenceTypes.Organizations);
                        $(this).dialog("close");
                    },
                    "Dismiss": function () {
                        top.Ts.Services.Customers.DismissAlert(organizationID, top.Ts.ReferenceTypes.Organizations);
                        $(this).dialog("close");
                    }
                }
            });

        }
    });


    if (!top.Ts.System.User.CanEditCompany && !_isAdmin) 
    {
        $('#productToggle').hide();
    }

    if (!top.Ts.System.Organization.IsInventoryEnabled)
    {
        $('#companyTabs a[href="#company-products"]').hide();
        $('#companyTabs a[href="#company-inventory"]').hide();
    }
    $(".maincontainer").on("keypress", "input",(function (evt) {
        //Deterime where our character code is coming from within the event
        var charCode = evt.charCode || evt.keyCode;
        if (charCode == 13) { //Enter key's keycode
            return false;
        }
    }));

    $('#historyToggle').on('click', function () {
        if (historyLoaded == 0) {
            historyLoaded = 1;
            LoadHistory(1);
        }
    });

    $('#historyRefresh').on('click', function () {
            LoadHistory(1);
    });

    if (noteID != null)
    {
        $('#companyTabs a:first').tab('show');
        $('#companyTabs a[href="#company-notes"]').tab('show');
        openNote(noteID);
    }
    else {
        $('#companyTabs a:first').tab('show');
    }

    if (!_isAdmin && !top.Ts.System.User.CanEditCompany) {
        $('#fieldActive').removeClass('editable');
        $('#groupAPI').hide();
        $('#customerEdit').hide();
        $('#customerPhoneBtn').hide();
        $('#customerAddressBtn').hide();
        $('#customerDelete').hide();
        $('#fileToggle').hide();
    }

    if (!_isAdmin && !top.Ts.System.User.CanCreateContact) {
        $('.contact-action-add').hide();
    }


    if (top.Ts.System.User.OrganizationID == organizationID)
    {
        $('#groupSupportUser').hide();
        $('#groupSupportGroup').hide();
    }else{
        $('#groupTimezone').hide();
        $('#groupPortalGroup').hide();
    }
    
    $('#customerEdit').click(function (e) {
        $('.userProperties p').toggleClass("editable");
        $('.customProperties p').toggleClass("editable");
        $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        $("#addressPanel #editmenu").toggleClass("hiddenmenu");
        $(this).toggleClass("btn-primary");
        $(this).toggleClass("btn-success");
        $('#companyTabs a:first').tab('show');
        if ((!_isAdmin && !top.Ts.System.User.HasPortalRights) || !top.Ts.System.User.CanEditCompany) {
            $('#fieldPortalAccess').removeClass('editable');
        }

    });

    $('#fieldName').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetCompanyName(organizationID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#companyName').text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company name.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $('#customerEdit').removeClass("disabled");
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldWebsite').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetCompanyWeb(organizationID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company website.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDomains').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetCompanyDomain(organizationID, $(this).prev().find('input').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company domain.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSupportHours').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<input type="text">')
          .addClass('col-xs-10 form-control number')
          .val($(this).text())
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              var value = $(this).prev().find('input').val();
              top.Ts.Services.Customers.SetCompanySupportHours(organizationID, value != "" ? value : 0 , function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company support hours.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDescription').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "Empty" ? "" : $(this).html().replace(/<br\s?\/?>/g, "\n"))
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetCompanyDescription(organizationID, $(this).prev().find('textarea').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company description.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldInactive').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).html().replace(/<br\s?\/?>/g, "\n"))
          .appendTo(container1)
          .focus();

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              top.Ts.Services.Customers.SetCompanyInactive(organizationID, $(this).prev().find('textarea').val(), function (result) {
                  header.text(result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company inactive reason.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSAED').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();
        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var theinput = $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text() == "[None]" ? "" : $(this).text())
            .datetimepicker({ pickTime: false })
          .appendTo(container1)
          .focus();
       

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        

        $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
              //var value = top.Ts.Utils.getMsDate($(this).prev().find('input').val());
              top.Ts.Services.Customers.SetCompanySAE(organizationID, $(this).prev().find('input').val(), function (result) {
                  //var date = result === null ? null : top.Ts.Utils.getMsDate(result);
                  header.text(result == "" ? "[None]" : result);
                  $('#customerEdit').removeClass("disabled");
              },
                            function (error) {
                                header.show();
                                alert('There was an error saving the company Service Agreement Expiration Date.');
                                $('#customerEdit').removeClass("disabled");
                            });
              $(this).closest('div').remove();
              header.show();
          })
          .insertAfter(container1);
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldPrimaryContact').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryContact').appendTo(container1);
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
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryContact').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanyPrimaryContact(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDefaultUser').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlPrimaryDefaultUser').appendTo(container1);
        top.Ts.Services.Customers.LoadOrgSupportUsers(organizationID, function (contacts) {
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
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlPrimaryDefaultUser').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanyDefaultSupportUser(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldTimeZone').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlTimezone').appendTo(container1);
        top.Ts.Services.Customers.LoadTimeZones(function (timeZones) {
            for (var i = 0; i < timeZones.length; i++) {
                var opt = $('<option>').attr('value', timeZones[i].Id).text(timeZones[i].DisplayName).data('o', timeZones[i]).appendTo(select);
                if (header.data('field') == timeZones[i].Id)
                    opt.attr('selected', 'selected');
            }
        });

        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlTimezone').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanyTimezone(organizationID, value, function (result) {
                header.data('field', result);
                header.text(result);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company timezone.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDefaultGroup').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldDefaultGroup').appendTo(container1);
        top.Ts.Services.Customers.LoadOrgGroups(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].GroupID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].GroupID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldDefaultGroup').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanyDefaultSupportGroup(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldDefaultPortalGroup').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldDefaultPortalGroup').appendTo(container1);
        top.Ts.Services.Customers.LoadOrgGroups(organizationID, function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].GroupID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].GroupID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldDefaultPortalGroup').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanyDefaultPortalGroup(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldSLA').click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
            return false;
        var header = $(this).hide();

        var container = $('<div>')
          .insertAfter(header);

        var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

        var select = $('<select>').addClass('form-control').attr('id', 'ddlfieldSLA').appendTo(container1);
        top.Ts.Services.Customers.LoadOrgSlas(function (contacts) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
            for (var i = 0; i < contacts.length; i++) {
                var opt = $('<option>').attr('value', contacts[i].SlaLevelID).text(contacts[i].Name).data('o', contacts[i]);
                if (header.data('field') == contacts[i].SlaLevelID)
                    opt.attr('selected', 'selected');
                opt.appendTo(select);
            }
        });


        $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
              $(this).closest('div').remove();
              header.show();
              $('#customerEdit').removeClass("disabled");
          })
          .insertAfter(container1);
        $('#ddlfieldSLA').on('change', function () {
            var value = $(this).val();
            var name = this.options[this.selectedIndex].innerHTML;
            container.remove();

            top.Ts.Services.Customers.SetCompanySLA(organizationID, value, function (result) {
                header.data('field', result);
                header.text(name);
                header.show();
                $('#customerEdit').removeClass("disabled");
            }, function () {
                alert("There was a problem saving your company property.");
                $('#customerEdit').removeClass("disabled");
            });
        });
        $('#customerEdit').addClass("disabled");
    });

    $('#fieldActive').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetCompanyActive(organizationID, ($(this).text() !== 'true'), function (result) {
            $('#fieldActive').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
    });

    $('#fieldAPIEnabled').click(function (e) {
        if (!$(this).hasClass('editable'))
            return false;
        top.Ts.Services.Customers.SetCompanyAPIEnabled(organizationID, ($(this).text() !== 'true'), function (result) {
            $('#fieldAPIEnabled').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer active.');
        });
    });

    $('#fieldPortalAccess').click(function (e) {
        if (!$(this).hasClass('editable') || (!_isAdmin && !top.Ts.System.User.HasPortalRights) || !top.Ts.System.User.CanEditCompany)
            return false;
        top.Ts.Services.Customers.SetCompanyPortalAccess(organizationID, ($(this).text() !== 'true'), function (result) {
            $('#fieldPortalAccess').text((result === true ? 'true' : 'false'));
        },
        function (error) {
            header.show();
            alert('There was an error saving the customer portal access.');
        });
    });

    $('#productToggle').click(function (e) {
        $('#productForm').toggle();
    });

    $('#noteToggle').click(function (e) {
        $('#noteForm').toggle();
        $('#fieldNoteTitle').focus();
    });

    $('#fileToggle').click(function (e) {
        $('#fileForm').toggle();
    });

    $('#productCustomer').val(organizationID);

    top.Ts.Services.Organizations.GetOrganization(organizationID, function (org) {
        $('#companyName').html(org.Name);
    });

    $("input[type=text], textarea").autoGrow();
  
    $.valHooks.textarea = {
        get: function (elem) {
            return elem.value.replace(/\r\n|\r|\n/g, "<br />");
        }
    };

    //$('#companyTabs a:first').tab('show');

    $('.contact-action-add').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.newCustomer("customer",organizationID);
    });

    $('#productProduct').change(function () {
        LoadProductVersions($(this).val(),-1);
    });

    $('#btnProductSave').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var productInfo = new Object();
        productInfo.OrganizationID = $("#productCustomer").val();
        productInfo.ProductID = $("#productProduct").val();
        productInfo.Version = $("#productVersion").val();
        productInfo.SupportExpiration = $("#productExpiration").val();
        productInfo.OrganizationProductID = $('#fieldProductID').val();

        productInfo.Fields = new Array();
        $('.customField:visible').each(function () {
            var field = new Object();
            field.CustomFieldID = $(this).attr("id");
            switch ($(this).attr("type")) {
                case "checkbox":
                    field.Value = $(this).prop('checked');
                    break;
                default:
                    field.Value = $(this).val();
            }
            productInfo.Fields[productInfo.Fields.length] = field;
        });


        top.Ts.Services.Customers.SaveProduct(top.JSON.stringify(productInfo), function (prod) {
            LoadProducts(true);
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

    });

    top.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
        $('.datepicker').attr("data-format", dateformat);
        $('.datepicker').datetimepicker({ pickTime: false });

        $('#productExpiration').attr("data-format", dateformat);
        $('.datetimepicker').datetimepicker({ });
    });

    $('.userList').on('click', '.contactlink', function (e) {
        e.preventDefault();
        top.Ts.MainPage.openNewContact(this.id);
    });

    $('#tblProducts').on('click', '.productEdit', function (e) {
        e.preventDefault();
        var product = $(this).parent().parent().attr('id');
        var orgproductID;

        top.Ts.Services.Customers.LoadProduct(product, function (prod) {
            orgproductID = prod.OrganizationProductID;
            LoadProductVersions(prod.ProductID, prod.VersionNumber);
            $('#productProduct').val(prod.ProductID);
            $('#productExpiration').val(prod.SupportExpiration);
            $('#fieldProductID').val(orgproductID);
            $('#btnProductSave').text("Save");
            top.Ts.Services.Customers.LoadCustomProductFields(product, function (custField) {
                for (var i = 0; i < custField.length; i++) {
                    if (custField[i].FieldType == 2)
                        $('#' + custField[i].CustomFieldID).attr('checked',custField[i].Value);
                    else
                        $('#' + custField[i].CustomFieldID).val(custField[i].Value);
                }
            });
        });

        $('#productForm').show();



    });

    $("#btnProductCancel").click(function (e) {
        e.preventDefault();
        LoadProductTypes();
        $('#productExpiration').val('');
        $('#fieldProductID').val('-1');
        $('#btnProductSave').text("Associate Product");
        $('.customField:visible').each(function () {
            switch ($(this).attr("type")) {
                case "checkbox":
                    $(this).prop('checked',false);
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
            top.privateServices.DeleteOrganizationProduct($(this).parent().parent().attr('id'), function (e) {
                LoadProducts(true);
            });
            
        }
    });

    $('#tblProducts').on('click', '.productView', function (e) {
        e.preventDefault();
        top.Ts.MainPage.openOrganizationProduct($(this).parent().parent().attr('id'))
        //top.location = "../../../Default.aspx?OrganizationProductID=" + ;

    });

    top.privateServices.IsSubscribed(9, organizationID, issubbed);

    function issubbed(result) {
        if (result)
            $('#customerSubscribe').html('Unsubscribe');
        else
            $('#customerSubscribe').html('<i class="fa fa-rss"></i>');
    }

    $('#customerSubscribe').click(function (e) {
        top.privateServices.Subscribe(9, organizationID);
        top.privateServices.IsSubscribed(9, organizationID, issubbed);
    });

    $('#customerRefresh').click(function (e) {
        window.location = window.location;
    });


    $('#customerDelete').click(function (e) {
        if (confirm('Are you sure you would like to remove this organization?')) {
            top.privateServices.DeleteOrganization(organizationID, function (e) {
                if (window.parent.document.getElementById('iframe-mniCustomers'))
                    window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
                top.Ts.MainPage.closeNewCustomerTab(organizationID);
            });
        }
    });

    $('#phonePanel').on('click', '.delphone', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to remove this phone number?')) {
            top.privateServices.DeletePhone($(this).attr('id'), function (e) {
                LoadPhoneNumbers(1);
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
            });

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
            top.privateServices.DeleteAddress($(this).attr('id'), function (e) {
                LoadAddresses(1);
                $("#addressPanel #editmenu").toggleClass("hiddenmenu");
            });

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

    //$('#reminderDate').datetimepicker();

    $("#btnSaveReminder").click(function (e) {
        if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
            top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), $('#reminderDate').val(), $('#reminderUsers').val());
            $('#modalReminder').modal('hide');
        }
        else
            alert("Please fill in all the fields");
    });


    $("#btnPhoneSave").click(function (e) {
        var phoneInfo = new Object();

        phoneInfo.PhoneTypeID = $('#phoneType').val();
        phoneInfo.Number = $('#phoneNumber').val();
        phoneInfo.Extension = $('#phoneExt').val();
        phoneInfo.PhoneID = $('#phoneID').val() != "" ? $('#phoneID').val() : "-1";
        var inEditmode = $('#customerEdit').hasClass("btn-success")

        top.Ts.Services.Customers.SavePhoneNumber(top.JSON.stringify(phoneInfo), organizationID, top.Ts.ReferenceTypes.Organizations, function (f) {
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

        top.Ts.Services.Customers.SaveAddress(top.JSON.stringify(addressInfo), organizationID, top.Ts.ReferenceTypes.Organizations, function (f) {

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

    $('#tblNotes').on('click', '.editNote', function (e) {
        e.stopPropagation();
        top.Ts.Services.Customers.LoadNote($(this).parent().parent().attr('id'), function (note) {
            $('#fieldNoteTitle').val(note.Title);
            var desc = note.Description;
            desc = desc.replace(/<br\s?\/?>/g, "\n");
            //$('#fieldNoteDesc').val(desc);
            $('#fieldNoteID').val(note.NoteID);
            $('#noteCustomerAlert').prop('checked', note.IsAlert);
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
            top.privateServices.DeleteNote($(this).parent().parent().attr('id'), function(){
                LoadNotes();    
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
    });

    $('#tblFiles').on('click', '.viewFile', function (e) {
        e.preventDefault();
        top.Ts.MainPage.openNewAttachment($(this).parent().attr('id'));
    });

    $('#tblFiles').on('click', '.delFile', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (confirm('Are you sure you would like to remove this attachment?')) {
            top.privateServices.DeleteAttachment($(this).parent().parent().attr('id'), function (e) {
                LoadFiles();
            });
            
        }
    });

    $("#btnNotesCancel").click(function (e) {
        e.preventDefault();
        $('#fieldNoteTitle').val('');
        $('#fieldNoteDesc').val('');
        $('#fieldNoteID').val('-1');
        $('#noteCustomerAlert').prop('checked', false);
        $('#btnNotesSave').text("Save Note");
        $('#noteForm').toggle();
    });

    $("#btnNotesSave").click(function (e) {
        e.preventDefault();
        var title = $('#fieldNoteTitle').val();
        var description = $('#fieldNoteDesc').val();
        var noteID = $('#fieldNoteID').val();
        var isAlert = $('#noteCustomerAlert').prop('checked');
        if ((title.length || description.length) < 1){
            alert("Please fill in all the required information");
            return;
        }
        $(this).prop('disabled', true);
        top.Ts.Services.Customers.SaveNote(title, description, noteID, organizationID, top.Ts.ReferenceTypes.Organizations, isAlert, function (note) {
            $('#fieldNoteTitle').val('');
            $('#fieldNoteDesc').val('');
            $('#fieldNoteID').val('-1');
            $('#noteCustomerAlert').prop('checked',false);
            $('#btnNotesSave').text("Save Note");
            LoadNotes();
            $('#noteForm').toggle();
            $("#btnNotesSave").removeProp('disabled');
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
                data.formData = { description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n") };
                data.url = '../../../Upload/OrganizationAttachments/' + organizationID;
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
                  .addClass('icon-remove')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

                $('<span>')
                  .addClass('icon-remove')
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
            data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) +'%');
        },
        start: function (e, data) {
            $('.progress').show();
            $('.upload-queue .ui-icon-close').hide();
            $('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            //data.context.find('.progress-bar').css('width', '100%');
            LoadFiles();
            $('.upload-queue').empty();
            $('#attachmentDescription').val('');
            $('#fileForm').toggle();
        }
    });

    var execGetCompany = null;
    function getCompany(request, response) {
        if (execGetCompany) { execGetCompany._executor.abort(); }
        execGetCompany = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) { response(result); });
    }

    $('#productCustomer').autocomplete({
        minLength: 2,
        source: getCompany,
        select: function (event, ui) {
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading')
        }
    });

    $("#modalPhone").on('hidden.bs.modal', function () {
        $('#modalPhone input').val('');
    });
    $("#modalAddress").on('hidden.bs.modal', function () {
        $('#modalAddress input').val('');
        $('#modalAddress #addressID').val('-1');
    });
    $("#modalReminder").on('hidden.bs.modal', function () {
        $('#modalReminder input').val('');
    });

    top.Ts.Services.Tickets.Load5MostRecentByOrgID(organizationID, function (tickets) {
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

    function LoadCustomProperties() {
        top.Ts.Services.Customers.GetCustomValues(organizationID, top.Ts.ReferenceTypes.Organizations, function (html) {
            //$('#customProperties').append(html);
            appendCustomValues(html);
            });
    }

    function LoadProperties() {
        top.Ts.Services.Customers.GetProperties(organizationID, function (result) {
            $('#fieldName').text(result.orgproxy.Name);
            $('#fieldWebsite').text(result.orgproxy.Website != null && result.orgproxy.Website != "" ? result.orgproxy.Website : "Empty");
            $('#fieldDomains').text(result.orgproxy.CompanyDomains != "" ? result.orgproxy.CompanyDomains : "Empty");
            $('#fieldActive').text(result.orgproxy.IsActive);
            $('#fieldPortalAccess').text(result.orgproxy.HasPortalAccess);
            $('#fieldAPIEnabled').text(result.orgproxy.IsApiActive && result.orgproxy.IsApiEnabled);
            $('#fieldSAED').text(result.SAED == null ? "Empty" : result.SAED);
            $('#fieldSLA').text(result.SLA);
            $('#fieldSLA').data('field', result.orgproxy.SlaLevelID);
            $('#fieldSupportHours').text(result.orgproxy.SupportHoursMonth);
            $('#fieldDescription').html(result.orgproxy.Description != null && result.orgproxy.Description != ""? result.orgproxy.Description : "Empty");
            $('#fieldAPIToken').text(result.orgproxy.WebServiceID);
            $('#fieldOrgID').text(result.orgproxy.OrganizationID);
            $('#fieldPrimaryContact').text(result.PrimaryUser == "" ? "Empty" : result.PrimaryUser);
            $('#fieldPrimaryContact').data('field', result.orgproxy.PrimaryUserID);
            $('#fieldDefaultUser').text(result.DefaultSupportUser);
            $('#fieldDefaultUser').data('field', result.orgproxy.DefaultSupportUserID);
            $('#fieldDefaultGroup').text(result.SupportGroup);
            $('#fieldDefaultGroup').data('field', result.orgproxy.DefaultSupportGroupID);
            $('#fieldInactive').html(result.orgproxy.InActiveReason != null && result.orgproxy.InActiveReason != "" ? result.orgproxy.InActiveReason : "Empty");

            $('#fieldTimeZone').text(result.orgproxy.TimeZoneID == "" ? "Central Standard Time" : result.orgproxy.TimeZoneID);
            $('#fieldTimeZone').data('field', result.orgproxy.TimeZoneID);

            if (!_isAdmin || result.orgproxy.IsActive == false)
            {
                $('#groupInactive').hide();
            }

            if (result.orgproxy.Name == "_Unknown Company") {
                $('#customPropRow').hide();
                $('#customerEdit').hide();
                $('#customerDelete').hide();
            }


        });
    }

    function LoadAddresses(reload) {
        $('#addressPanel').empty();
        top.Ts.Services.Customers.LoadAddresses(organizationID,top.Ts.ReferenceTypes.Organizations, function (address) {
            for (var i = 0; i < address.length; i++) {
                $('#addressPanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + address[i].Description + "</label> \
                                        <div class='col-xs-5'> \
                                            " + ((address[i].Addr1 != null) ? "<p class='form-control-static'><a href='" + address[i].MapLink + "' target='_blank' id='" + address[i].AddressID + "' class='mapphone'><span class='fa fa-map-marker'></span></a> " + address[i].Addr1 + "</p>" : "") + " \
                                            " + ((address[i].Addr2 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr2 + "</p>" : "") + " \
                                            " + ((address[i].Addr3 != null) ? "<p class='form-control-static pt0'>" + address[i].Addr3 + "</p>" : "") + " \
                                            <p class='form-control-static pt0'> \
                                            " + ((address[i].City != null) ? "<p class='form-control-static pt0'>" + address[i].City : "") + " \
                                            " + ((address[i].State != null && address[i].State != '') ? ", " + address[i].State : "") + " \
                                            " + ((address[i].Zip != null && address[i].Zip != '') ? " " + address[i].Zip : "") + " </p> \
                                            " + ((address[i].Country != null) ? "<p class='form-control-static pt0'>" + address[i].Country + "</p>" : "") + " \
                                        </div> \
                                        <div id='editmenu' class='col-xs-2 hiddenmenu'> \
                                            <a href='#' id='" + address[i].AddressID + "' class='editaddress'><span class='fa fa-pencil'></span></a>\
                                            <a href='#' id='" + address[i].AddressID + "' class='deladdress'><span class='fa fa-trash-o'></span></a/></p>\
                                        </div> \
                                    </div>");
            }
            if (reload != undefined)
                $("#addressPanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadNotes() {
        top.Ts.Services.Customers.LoadNotes(organizationID,top.Ts.ReferenceTypes.Organizations, function (note) {
            $('#tblNotes tbody').empty();
            var html;
            for (var i = 0; i < note.length; i++) {
                if (_isAdmin || note[i].CreatorID == top.Ts.System.User.UserID || top.Ts.System.User.CanEditCompany)
                    html = '<td><i class="fa fa-edit editNote"></i></td><td><i class="fa fa-trash-o deleteNote"></i></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';
                else
                    html = '<td></td><td></td><td>' + note[i].Title + '</td><td>' + note[i].CreatorName + '</td><td>' + note[i].DateCreated.toDateString() + '</td>';

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

    var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

    top.Ts.Services.Customers.GetOrganizationTickets(organizationID, 0, function (e) {
        $('#openTicketCount').text("Open Tickets: " + e);
    });

    function LoadHistory(start) {

        if(start == 1)
            $('#tblHistory tbody').empty();

            top.Ts.Services.Customers.LoadHistory(organizationID, start, function (history) {
                for (var i = 0; i < history.length; i++) {
                    $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                    .appendTo('#tblHistory > tbody:last');
                    //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
                }
                if(history.length == 50)
                    $('<button>').text("Load More").addClass('btn-link')
                    .click(function (e){
                        LoadHistory($('#tblHistory tbody > tr').length+1);
                        $(this).remove();
                    })
                   .appendTo('#tblHistory > tbody:last');
            });
    }

    function LoadFiles() {
        $('#tblFiles tbody').empty();
        top.Ts.Services.Customers.LoadFiles(organizationID,top.Ts.ReferenceTypes.Organizations, function (files) {
            for (var i = 0; i < files.length; i++) {
                var tr = $('<tr>')
                .attr('id', files[i].AttachmentID)
                .html('<td><i class="fa fa-trash-o delFile"></i></td><td class="viewFile">' + files[i].FileName + '</td><td>' + files[i].Description + '</td><td>' + files[i].CreatorName + '</td><td>' + files[i].DateCreated.toDateString() + '</td>')
                .appendTo('#tblFiles > tbody:last');


                //$('#tblFiles > tbody:last').appendTo('<tr id=' +  + '></tr>');
            }
        });
    }

    function LoadPhoneNumbers(reload)
    {
        $('#phonePanel').empty();
        top.Ts.Services.Customers.LoadPhoneNumbers(organizationID,top.Ts.ReferenceTypes.Organizations, function (phone) {
            for (var i = 0; i < phone.length; i++) {
                $('#phonePanel').append("<div class='form-group content'> \
                                        <label for='inputName' class='col-xs-4 control-label'>" + phone[i].PhoneTypeName + "</label> \
                                        <div class='col-xs-5 '> \
                                            <p class='form-control-static '><a href='tel:"+phone[i].Number+"'>" + phone[i].Number + "</a>" + ((phone[i].Extension != null && phone[i].Extension != '') ? ' Ext:' + phone[i].Extension : '') + "</p> \
                                        </div> \
                                        <div id='editmenu' class='col-xs-2 hiddenmenu'> \
                                            <p class='form-control-static'> \
                                            <a href='' id='" + phone[i].PhoneID + "' class='editphone'><span class='fa fa-pencil'></span></a>\
                                            <a href='' id='" + phone[i].PhoneID + "' class='delphone'><span class='fa fa-trash-o'></span></a/>\
                                            </p> \
                                        </div> \
                                    </div>");
            }
            if(reload != undefined)
                $("#phonePanel #editmenu").toggleClass("hiddenmenu");
        });
    }

    function LoadPhoneTypes() {
        top.Ts.Services.Customers.LoadPhoneTypes(organizationID, function (pt) {
            for (var i = 0; i < pt.length; i++) {
                $('<option>').attr('value', pt[i].PhoneTypeID).text(pt[i].Name).data('o', pt[i]).appendTo('#phoneType');
            }
        });
    }

    function LoadProductTypes() {
        $('#productProduct').empty();
        top.Ts.Services.Customers.LoadProductTypes(function (pt) {
            for (var i = 0; i < pt.length; i++) {
                if (i == 0)
                    LoadProductVersions(pt[i].ProductID,-1);
                $('<option>').attr('value', pt[i].ProductID).text(pt[i].Name).data('o', pt[i]).appendTo('#productProduct');
            }
        });
    }

    function LoadProductVersions(productID, selVal) {
        $("#productVersion").empty();
        
        top.Ts.Services.Customers.LoadProductVersions(productID, function (pt) {
            $('<option>').attr('value', '-1').text('Unassigned').appendTo('#productVersion');
            for (var i = 0; i < pt.length; i++) {
                var opt = $('<option>').attr('value', pt[i].ProductVersionID).text(pt[i].VersionNumber).data('o', pt[i]);
                if (pt[i].ProductVersionID == selVal)
                    opt.attr('selected', 'selected');
                opt.appendTo('#productVersion');
            }
        });
    }

    function LoadContacts() {
        $('.userList').empty();
        top.Ts.Services.Customers.LoadContacts(organizationID, function (users) {
            $('.userList').append(users)
            //for (var i = 0; i < users.length; i++) {
            //    $('<a>').attr('class', 'list-group-item').text(users[i].FirstName + ' ' + users[i].LastName).appendTo('.userList');
            //}
        });
    }

    function LoadProducts(noheaders) {

        if(!noheaders){
            top.Ts.Services.Customers.LoadcustomProductHeaders(organizationID, function (headers) {
                for (var i = 0; i < headers.length; i++) {
                    $('#tblProducts th:last').after('<th>' + headers[i] + '</th>');
                }
            });
            }

        $('#tblProducts tbody').empty();
        top.Ts.Services.Customers.LoadProducts(organizationID, function (product) {
            for (var i = 0; i < product.length; i++) {
                var customfields = "";
                for (var p = 0; p < product[i].CustomFields.length; p++)
                {
                    customfields = customfields + "<td>" + product[i].CustomFields[p]  + "</td>";
                }

                var html;

                if(top.Ts.System.User.CanEditCompany || _isAdmin)
                {
                    html = '<td><i class="fa fa-edit productEdit"></i></td><td><i class="fa fa-trash-o productDelete"></i></td><td><i class="fa fa-folder-open productView"></i></td><td>' + product[i].ProductName + '</td><td>' + product[i].VersionNumber + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td>' + customfields;
                }
                else
                {
                    html = '<td></td><td></td><td><i class="fa fa-folder-open productView"></i></td><td>' + product[i].ProductName + '</td><td>' + product[i].VersionNumber + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td>' + customfields
                }
                var tr = $('<tr>')
                .attr('id', product[i].OrganizationProductID)
                .html(html)
                .appendTo('#tblProducts > tbody:last');


                //$('#tblProducts > tbody:last').append('<tr><td><a href="#" id='+ product.ProductID +'><i class="glyphicon glyphicon-edit productEdit"></i></td><td><i class="glyphicon glyphicon-trash productDelete"></i></td><td><i class="fa fa-folder-open productView"></i></td><td>' + product[i].ProductName + '</td><td>' + product[i].VersionNumber + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td><td></td></tr>');
            }
        });

    }

    function LoadCustomControls(refType) {
        top.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
            $('#customProductsControls').append(html);
        });
    }

    function LoadReminderUsers() {
        var users = top.Ts.Cache.getUsers();
        if (users != null) {
            for (var i = 0; i < users.length; i++) {
                var option = $('<option>').attr('value', users[i].UserID).text(users[i].Name).data('o', users[i]).appendTo('#reminderUsers');
                if (top.Ts.System.User.UserID === users[i].UserID) { option.attr('selected', 'selected'); }
            }
        }
    }

    function UpdateRecentView() {
        top.Ts.Services.Customers.UpdateRecentlyViewed("o" + organizationID, function (resultHtml) {
            if (window.parent.document.getElementById('iframe-mniCustomers'))
                window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
        });

    }

    createTestChart();
    function createTestChart() {

        top.Ts.Services.Customers.LoadChartData(organizationID, true, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Open Tickets", 0]);
                //$('#openChart').text("No Open Tickes").addClass("text-center");
                $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Open Tickets");
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

        top.Ts.Services.Customers.LoadChartData(organizationID, false, function (chartString) {

            var chartData = [];
            var dummy = chartString.split(",");
            for (var i = 0; i < dummy.length; i++) {
                chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
                i++
            }

            if (dummy.length == 1) {
                //chartData.pop();
                //chartData.push(["No Closed Tickets", 0]);
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

        //top.Ts.Services.Customers.LoadCDI(organizationID, function (cdiValue) {
        //    var chartData = [];
        //    chartData.push(cdiValue);
        //    $('#csiChart').highcharts({

        //        chart: {
        //            type: 'gauge',
        //            plotBackgroundColor: null,
        //            plotBackgroundImage: null,
        //            plotBorderWidth: 0,
        //            plotShadow: false,
        //            height: 250,
        //        },

        //        title: {
        //            text: 'CDI'
        //        },

        //        pane: {
        //            startAngle: -150,
        //            endAngle: 150,
        //            background: [{
        //                backgroundColor: {
        //                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
        //                    stops: [
        //                        [0, '#FFF'],
        //                        [1, '#333']
        //                    ]
        //                },
        //                borderWidth: 0,
        //                outerRadius: '109%'
        //            }, {
        //                backgroundColor: {
        //                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
        //                    stops: [
        //                        [0, '#333'],
        //                        [1, '#FFF']
        //                    ]
        //                },
        //                borderWidth: 1,
        //                outerRadius: '107%'
        //            }, {
        //                // default background
        //            }, {
        //                backgroundColor: '#DDD',
        //                borderWidth: 0,
        //                outerRadius: '105%',
        //                innerRadius: '103%'
        //            }]
        //        },

        //        // the value axis
        //        yAxis: {
        //            min: 0,
        //            max: 100,

        //            minorTickInterval: 'auto',
        //            minorTickWidth: 1,
        //            minorTickLength: 10,
        //            minorTickPosition: 'inside',
        //            minorTickColor: '#666',

        //            tickPixelInterval: 30,
        //            tickWidth: 2,
        //            tickPosition: 'inside',
        //            tickLength: 10,
        //            tickColor: '#666',
        //            labels: {
        //                step: 2,
        //                rotation: 'auto'
        //            },
        //            title: {
        //                text: ''
        //            },
        //            plotBands: [{
        //                from: 0,
        //                to: 60,
        //                color: '#55BF3B' // green
        //            }, {
        //                from: 60,
        //                to: 80,
        //                color: '#DDDF0D' // yellow
        //            }, {
        //                from: 80,
        //                to: 100,
        //                color: '#DF5353' // red
        //            }]
        //        },
        //        credits: {
        //            enabled: false
        //        },
        //        series: [{
        //            name: 'CDI',
        //            data: [],
        //            tooltip: {
        //                valueSuffix: ' Rating'
        //            }
        //        }]

        //    },
        //    function (chart) {
        //        if (!chart.renderer.forExport) {

        //        }
        //    });

        //    var chart = $('#csiChart').highcharts();
        //    chart.series[0].setData(chartData);
        //}); 
    }
    $("[rel='tooltip']").tooltip();
    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "Tickets")
            $('#ticketIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_CustomerID=" + organizationID);
        else if (e.target.innerHTML == "Watercooler")
            $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=2&pageid=" + organizationID);
        else if (e.target.innerHTML == "Details")
            createTestChart();
        else if (e.target.innerHTML == "Contacts")
            LoadContacts();
        else if (e.target.innerHTML == "Notes")
            LoadNotes();
        else if (e.target.innerHTML == "Files")
            LoadFiles();
    })

    $('#inventoryIframe').attr("src", "../../../Inventory/CustomerInventory.aspx?CustID=" + organizationID);
    $("input[type=text], textarea").autoGrow();

    $('.customProperties, .userProperties').on('keydown', '.number', function (event) {
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

    $('.userProperties p').toggleClass("editable");

    $('#alertSnooze').click(function (e) {
        top.Ts.Services.Customers.SnoozeAlert(organizationID, top.Ts.ReferenceTypes.Organizations);
        $('#modalAlert').modal('hide');
    });

    $('#alertDismiss').click(function (e) {
        top.Ts.Services.Customers.DismissAlert(organizationID, top.Ts.ReferenceTypes.Organizations);
        $('#modalAlert').modal('hide');
    });
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

var appendCustomValues = function (fields) {

    top.Ts.Services.Customers.GetCustomFieldCategories(function (categories) {
        var noCatfields = 0;
        for (var i = 0; i < fields.length; i++) {

            if (fields[i].CustomFieldCategoryID == -1) {
                noCatfields++;
                var item = null;
                var field = fields[i];
                var div = $('<div>').addClass('form-group').data('field', field);

                $('<label>')
                  .addClass('col-xs-4 control-label')
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


                //if (i < (fields.length / 2))
                //    containerL.append(div);
                //else
                if (noCatfields % 2)
                    $('#customPropertiesL').append(div);
                else
                    $('#customPropertiesR').append(div);
            }
        }

        if (noCatfields == 0) {
            $('#customPropRow').hide();
        }

        for (var c = 0; c < categories.length; c++)
        {
            var custom = $('<div>').insertBefore($('#customPropRow'));

            var box = $('<div>').addClass('box').appendTo(custom);
            var header = $('<div>').addClass('box-header').appendTo(box);
            var h3title = $('<h3>').text(categories[c].Category).appendTo(header);
            var boxcontent = $('<div>').addClass('box-content').appendTo(box);
            var boxrow = $('<div>').addClass('row').appendTo(boxcontent);
            var formh = $('<form>').addClass('form-horizontal').appendTo(boxrow);
            var colxsL = $('<div>').addClass('col-xs-6 customProperties').appendTo(formh);
            var colxsR = $('<div>').addClass('col-xs-6 customProperties').appendTo(formh);
            var fieldcount = 0;

            for (var i = 0; i < fields.length; i++) {

                if (categories[c].CustomFieldCategoryID == fields[i].CustomFieldCategoryID)
                {
                    var item = null;
                    var field = fields[i];
                    var div = $('<div>').addClass('form-group').data('field', field);

                    $('<label>')
                      .addClass('col-xs-4 control-label')
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

                    if (fieldcount % 2)
                        colxsR.append(div);
                    else
                        colxsL.append(div);
                    fieldcount++;
                    //if (i < (fields.length / 2))
                    //    containerL.append(div);
                    //else
                    //    containerR.append(div);
                }
            }
        }
        $('.customProperties p').toggleClass("editable");
    });


    if (fields === null || fields.length < 1) {
        $('#customPropertiesL').empty();
        //$('#customPropertiesR').empty();
        return;
    }
    //var containerL = $('#customPropertiesL').empty();
    //var containerR = $('#customPropertiesR').empty();
    
    
}

var appendCustomEditCombo = function (field, element) {
    var div = $('<div>')
    .addClass('col-xs-8')
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
          .addClass('col-xs-9')
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
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  parent.show();
                  $('#customerEdit').removeClass("disabled");
              }, function () {
                  alert("There was a problem saving your contact property.");
                  $('#customerEdit').removeClass("disabled");
              });
          });
          $('#customerEdit').addClass("disabled");
      });
    var items = field.ListValues.split('|');
    if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditNumber = function (field, element) {
    var div = $('<div>')
    .addClass('col-xs-8')
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
          .addClass('col-xs-9')
          .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
            .addClass('col-xs-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }
}

var appendCustomEditBool = function (field, element) {

    var div = $('<div>')
    .addClass('col-xs-8')
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
          top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
          }, function () {
              alert("There was a problem saving your contact property.");
          });
      });
}

var appendCustomEdit = function (field, element) {

    var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

    var result = $('<p>')
      .html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        if ($(this).has('a') && !$(this).hasClass('editable'))
        {
            return;
        }
        else
            {
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
            .val(fieldValue == "Empty" ? "" : fieldValue)
            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);
          $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
                var value = input.val();
                container.remove();
                if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                }
                else {
                    result.parent().removeClass('has-error');
                }
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
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
                $('#customerEdit').removeClass("disabled");
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
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
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
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
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
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your customer property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
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
            .datetimepicker({pickDate: false})

            .appendTo(container1)
            .focus();

          $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
                $(this).closest('div').remove();
                parent.show();
                $('#customerEdit').removeClass("disabled");
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
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, organizationID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                    parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                    $('#customerEdit').removeClass("disabled");
                }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#customerEdit').removeClass("disabled");
                });
                parent.show();
            })
            .insertAfter(container1);
          $('#customerEdit').addClass("disabled");
      });
    if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
        result.parent().addClass('has-error');
    }

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

function onShow() {
    customerDetailPage.refresh();
};

CustomerDetailPage = function () {

};

CustomerDetailPage.prototype = {
    constructor: CustomerDetailPage,
    refresh: function () {

    }
};

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

function openTicketWindow(ticketID) {
    top.Ts.MainPage.openTicketByID(ticketID, true);
}

function openNote(noteID) {
    top.Ts.Services.Customers.LoadNote(noteID, function (note) {
        var desc = note.Description;
        desc = desc.replace(/<br\s?\/?>/g, "\n");
        $('.noteDesc').show();
        $('.noteDesc').html("<strong>Description</strong> <p>" + desc + "</p>");

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