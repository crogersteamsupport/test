/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var userPage = null;
$(document).ready(function () {
  userPage = new UserPage();
  userPage.refresh();
});

function onShow() {
  userPage.refresh();
};

function DialogClosed(sender, args) {
    sender.remove_close(DialogClosed);
    window.location = window.location;
}

function ShowDialog(wnd) {
    wnd.add_close(DialogClosed);
    wnd.show();
}

UserPage = function () {
    var _user = null;

    $('button').button();

    $('a').addClass('ui-state-default ts-link');

    $('.ts-section').hover(function (e) {
        e.preventDefault();
        $(this).find('.user-profile-edit').show();
    }, function (e) {
        e.preventDefault();
        $('.user-profile-edit').hide();
    });

    $('.ts-section .collapsable')
    .prepend('<span class="ui-icon ui-icon-triangle-1-e">')
    .click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        var icon = $(this).find('.ui-icon');
        if (icon.hasClass('ui-icon-triangle-1-e')) {
            icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
            $(this).next().hide();
        }
        else {
            icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
            $(this).next().show();
        }
    });

    var userID = top.Ts.Utils.getQueryValue("userid", window);
    var orgID;

    top.Ts.Services.Users.GetUser(userID, function (user) {
        _user = user;
        orgID = user.OrganizationID;

        $('.user-displayname').html(user.FirstName + ' ' + user.LastName);
        $('#userEmail').html('<a class=fleft href="mailto:' + user.Email + '">' + user.Email + '</a>');
        var email = $('#userEmail').parent()
        .hover(function () {
            $(this).find('.ts-icon-edit').show();
        }, function () {
            $(this).find('.ts-icon-edit').hide();
        });

        $('#userTitle').html(user.Title == '' ? 'None' : user.Title);
        $('#userTimeZone').html(user.timeZoneDisplay);
        $('#userLastLogin').text(user.LastLogin.toDateString());
        $('#userActive').text((user.IsActive == true ? 'Yes' : 'No'));
        $('#userEmailNotify').text((user.ReceiveTicketNotifications == true ? 'Yes' : 'No'));
        $('#userSubscribeTickets').text((user.SubscribeToNewTickets == true ? 'Yes' : 'No'));
        $('#userSubscribeActions').text((user.SubscribeToNewActions == true ? 'Yes' : 'No'));
        $('#userAutoSubscribe').text((user.DoNotAutoSubscribe == true ? 'Yes' : 'No'));
        $('#userGroupNotify').text((user.ReceiveAllGroupNotifications == true ? 'Yes' : 'No'));
        $('#userDateFormat').text(user.CultureDisplay);
        $('#userSysAdmin').text((user.IsSystemAdmin == true ? 'Yes' : 'No'));
        $('#chatUser').text((user.IsChatUser == true ? 'Yes' : 'No'));
        $('#activatedOn').text(user.ActivatedOn.toDateString());
        $('#userInfo').html((user.UserInformation == '' ? 'No Additional Information' : user.UserInformation.replace(/\n\r?/g, '<br />')));

        top.Ts.Services.Users.GetUserPhoto(userID, function (att) {
            $('#userPhoto').attr("src", att);
        });

    });

    var canEdit = top.Ts.System.User.UserID == userID || top.Ts.System.User.IsSystemAdmin;
    var isSysAdmin = top.Ts.System.User.IsSystemAdmin;
    if (canEdit) {

        if (!isSysAdmin) {
            $('#userActive').removeClass('ui-state-default ts-link');
            $('#userActive').addClass('disabledlink');
            $('#userSysAdmin').removeClass('ui-state-default ts-link');
            $('#userSysAdmin').addClass('disabledlink');
            $('#chatUser').removeClass('ui-state-default ts-link');
            $('#chatUser').addClass('disabledlink');
        }


        $('.user-address-add').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            ShowDialog(top.GetAddressDialog(userID, 22));
        });

        $('.user-phone-add').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            ShowDialog(top.GetPhoneDialog(userID, 22));

        });

        if (!isSysAdmin) {
            $('.user-group-add').remove();
        }
        else {
            $('.user-group-add').click(function (e) {
                e.preventDefault();
                e.stopPropagation();
                ShowDialog(top.GetSelectGroupDialog(userID, 22));
            });
        }

        $('#userPhotoEdit')
      .click(function (e) {
          e.preventDefault();
          ShowDialog(top.GetProfileImageDialog(orgID, userID));
          //            $('.dialog-avatar').dialog({
          //              autoOpen: true,
          //              modal: true,
          //              height: 400,
          //              width: 400
          //            });
      });

        $('#UserName')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
        e.preventDefault();
        var header = $(this).parent().hide();
        var container = $('<div>')
        .addClass('user-name-edit')
        .css('marginTop', '1em')
        .insertAfter(header);

        $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val(_user.FirstName)
        .appendTo(container)
        .focus();

        $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val(_user.MiddleName)
        .appendTo(container)
        .focus();

        $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val(_user.LastName)
        .appendTo(container)
        .focus();

        $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show().find('img').show();
            var fname = $(this).prev().prev().prev().val();
            var mname = $(this).prev().prev().val();
            var lname = $(this).prev().val();

            if (fname == '' || lname == '') {
                header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                alert('The name you have specified is invalid.  Please enter your name.');
                return;
            }

            top.Ts.Services.Users.SaveUserName(_user.UserID, fname, mname, lname, function (result) {
                header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                $('#UserName').html(result);
                _user.FirstName = fname;
                _user.MidddleName = mname;
                _user.LastName = lname;
            },
          function (error) {
              header.show().find('img').hide();
              alert('There was an error saving the username.');
          });
        })
        .appendTo(container)

        $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
        })
        .appendTo(container)
    });

        $('#userInfo')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
        e.preventDefault();
        var header = $(this).parent().hide();
        $(this).find('.ts-icon-edit').hide();
        var container = $('<div>')
        .addClass('user-info-edit')
        .insertAfter(header);

        $('<textarea cols=50 rows=5>')
        .addClass('ui-widget-content ui-corner-all')
        .val($(this).parent().find('a').html().replace(/<br>/g, '\n'))
        .appendTo(container)
        .focus();

        $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show().find('img').show();
            top.Ts.Services.Users.SaveUserInfo(_user.UserID, $(this).prev().val(), function (result) {
                header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                if (result != '') {
                    $('#userInfo').html(result.replace(/\n\r?/g, '<br />'));
                } else {
                    $('#userInfo').html('No Additional Information');
                }
            },
          function (error) {
              header.show().find('img').hide();
              alert('There was an error saving the users information.');
          });
        })
        .appendTo(container)

        $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
        })
        .appendTo(container)
    });

        $('#userTitle')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
        e.preventDefault();
        var header = $(this).parent().hide();
        var container = $('<div>')
        .addClass('user-title-edit')
        .insertAfter(header);

        $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val($(this).text())
        .appendTo(container)
        .focus();

        $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show().find('img').show();
            top.Ts.Services.Users.SaveUserTitle(_user.UserID, $(this).prev().val(), function (result) {
                header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                $('#userTitle').html(result);
            },
          function (error) {
              header.show().find('img').hide();
              alert('There was an error saving the users title.');
          });
        })
        .appendTo(container)

        $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
        })
        .appendTo(container)
    });

        $('#editEmail')
    .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
    .click(function (e) {
        e.preventDefault();
        var header = $(this).parent().hide();
        $(this).find('.ts-icon-edit').hide();
        var container = $('<div>')
        .addClass('user-email-edit')
        .insertAfter(header);

        $('<input type="text">')
        .addClass('ui-widget-content ui-corner-all')
        .val($('#userEmail').text())
        .appendTo(container)
        .focus();

        $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show().find('img').show();
            top.Ts.Services.Users.SaveUserEmail(_user.UserID, $(this).prev().val(), function (result) {
                header.show().find('img').hide().next().show().delay(800).fadeOut(400);
                if (result.substring(0, 6) == "_error")
                    alert("The email you have specified is invalid.  Please choose another email.");
                else
                    $('#userEmail').html('<a href="mailto:' + result + '">' + result + '</a>');
            },
          function (error) {
              header.show().find('img').hide();
              alert('There was an error saving the email.');
          });
        })
        .appendTo(container)

        $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
        })
        .appendTo(container)
    });

        $('#userActive')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);

          if (isSysAdmin) {
              item.next().show();
              top.Ts.Services.Users.SetIsActive(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user active status.');
              item.next().hide();
          });
          }
      });

        $('#userEmailNotify')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          item.next().show();
          top.Ts.Services.Users.SetEmailNotify(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user email notification status.');
              item.next().hide();
          });
      });


        $('#userSubscribeTickets')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          item.next().show();
          top.Ts.Services.Users.SetSubscribeTickets(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user ticket subscription status.');
              item.next().hide();
          });
      });

        $('#userSubscribeActions')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          item.next().show();
          top.Ts.Services.Users.SetSubscribeActions(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user ticket action subscription status.');
              item.next().hide();
          });
      });

        $('#userAutoSubscribe')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          item.next().show();
          top.Ts.Services.Users.SetAutoSubscribe(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user auto subscribe status.');
              item.next().hide();
          });
      });

        $('#userGroupNotify')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          item.next().show();
          top.Ts.Services.Users.SetGroupNotify(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user group notification subscription status.');
              item.next().hide();
          });
      });

        $('#userSysAdmin')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          if (isSysAdmin) {
              item.next().show();
              top.Ts.Services.Users.SetSysAdmin(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              item.text((result === true ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user system admin status.');
              item.next().hide();
          });
          }
      });

        $('#chatUser')
      .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
      .click(function (e) {
          e.preventDefault();
          var item = $(this);
          if (isSysAdmin) {
              item.next().show();
              top.Ts.Services.Users.SetChatUser(_user.UserID, (item.text() !== 'Yes'),
          function (result) {
              if (result == 'error') {
                  item.text('No').next().hide().next().show().delay(800).fadeOut(400);
                  alert("You have exceeded your chat licenses.  Please purchase more seats to add additional chat users.");
              }
              else
                  item.text((result == "True" ? 'Yes' : 'No')).next().hide().next().show().delay(800).fadeOut(400);
          },
          function (error) {
              alert('There was an error saving the user chat status.');
              item.next().hide();
          });
          }
      });


        $('#userTimeZone')
        .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
        .click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var tz = top.Ts.Cache.getTimeZones();
            for (var i = 0; i < tz.length; i += 2) {
                var option = $('<option>').text(tz[i]).data('type', tz[i + 1]).appendTo(select);
                if ($.trim(value) == $.trim(tz[i])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function (e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    top.Ts.Services.Users.SetTimezone(_user.UserID, type, function (result) {
                        if (result !== null) {
                            $('#userTimeZone').html(result);
                            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        }
                        else {
                            parent.show().find('img').hide();
                        }
                    },
            function (error) {
                parent.show().find('img').hide();
                alert('There was an error setting user timezone.');
            });
                    container.remove();
                },
                close: function (e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

        $('#userDateFormat')
        .after('<img src="../Images/loading/loading_small2.gif" /><span class="ts-icon ts-icon-saved"></span>')
        .click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            removeComboBoxes();
            var value = $(this).text();
            var parent = $(this).parent().hide();
            var container = $('<div>').addClass('ticket-combobox').insertAfter(parent);
            var select = $('<select>').appendTo(container);

            var culture = top.Ts.Cache.getCultures();
            for (var i = 0; i < culture.length; i++) {
                var displayname = culture[i].split("_");
                var option = $('<option>').text(displayname[0]).appendTo(select).data('type', displayname[1]);
                if ($.trim(value) == $.trim(displayname[0])) {
                    option.attr('selected', 'selected');
                }
            }

            select.combobox({
                selected: function (e, ui) {
                    parent.show().find('img').show();
                    var type = $(ui.item).data('type');
                    top.Ts.Services.Users.SetCulture(_user.UserID, type, function (result) {
                        if (result !== null) {
                            $('#userDateFormat').html(result);
                            parent.show().find('img').hide().next().show().delay(800).fadeOut(400);
                        }
                        else {
                            parent.show().find('img').hide();
                        }
                    },
            function (error) {
                parent.show().find('img').hide();
                alert('There was an error setting user date format.');
            });
                    container.remove();
                },
                close: function (e, ui) {
                    removeComboBoxes();
                }
            });
            select.combobox('search', '');
        });

    }
    else {
        $('.ts-icon-edit').remove();
        $('.ts-add').remove();
        $('#userPhotoEdit').remove();
        $('#UserName').removeClass('ui-state-default ts-link');
        $('#UserName').addClass('disabledlink');
        $('#userTitle').removeClass('ui-state-default ts-link');
        $('#userTitle').addClass('disabledlink');
        $('#userTimeZone').removeClass('ui-state-default ts-link');
        $('#userTimeZone').addClass('disabledlink');
        $('#userLastLogin').removeClass('ui-state-default ts-link');
        $('#userLastLogin').addClass('disabledlink');
        $('#userActive').removeClass('ui-state-default ts-link');
        $('#userActive').addClass('disabledlink');
        $('#userEmailNotify').removeClass('ui-state-default ts-link');
        $('#userEmailNotify').addClass('disabledlink');
        $('#userSubscribeTickets').removeClass('ui-state-default ts-link');
        $('#userSubscribeTickets').addClass('disabledlink');
        $('#userSubscribeActions').removeClass('ui-state-default ts-link');
        $('#userSubscribeActions').addClass('disabledlink');
        $('#userAutoSubscribe').removeClass('ui-state-default ts-link');
        $('#userAutoSubscribe').addClass('disabledlink');
        $('#userGroupNotify').removeClass('ui-state-default ts-link');
        $('#userGroupNotify').addClass('disabledlink');
        $('#userDateFormat').removeClass('ui-state-default ts-link');
        $('#userDateFormat').addClass('disabledlink');
        $('#userDateFormat').removeClass('ui-state-default ts-link');
        $('#userDateFormat').addClass('disabledlink');
        $('#userSysAdmin').removeClass('ui-state-default ts-link');
        $('#userSysAdmin').addClass('disabledlink');
        $('#chatUser').removeClass('ui-state-default ts-link');
        $('#chatUser').addClass('disabledlink');
        $('#userInfo').removeClass('ui-state-default ts-link');
        $('#userInfo').addClass('disabledlink');
    }

    GetAddresses();
    GetPhoneNumbers();
    GetGroups();


    function removeComboBoxes() {
        $('.ticket-combobox').prev().show().next().remove();
    }

    function GetAddresses() {
        top.Ts.Services.Users.GetUserAddresses(userID, function (addresses) {
            var addresslist = '';

            if (addresses.length === 0) {
                addresslist = '<div>There are no addresses to display.</div>';
            }

            for (var i = 0; i < addresses.length; i++) {
                var header = $('<div>').addClass('user-address')
                            .hover(function () {
                                $(this).find('.ts-icon').show();
                            }, function () {
                                $(this).find('.ts-icon').hide();
                            })
                                   .data('data', addresses[i].AddressID);
                var desc = $('<span>')
                            .addClass('link-strong')
                            .text(addresses[i].Description)
                            .appendTo(header);
                if (canEdit) {
                    $('<span>')
                .addClass('fleft ts-icon ts-icon-edit')
                .click(function (e) {
                    var item = $(this).parent();
                    var data = item.data('data');
                    ShowDialog(top.GetAddressDialog(data));
                }).hide()
                .appendTo(header);


                    $('<span>')
                .addClass('fleft ts-icon ts-icon-delete')
                .click(function (e) {
                    if (confirm('Are you sure you would like to remove this address?')) {
                        var item = $(this).parent();
                        var data = item.data('data');
                        top.privateServices.DeleteAddress(data);
                        window.location = window.location;
                    }
                    else {
                        $(this).prev().hide();
                        $(this).hide();
                    }
                }).hide()
                .appendTo(header);
                }

                var addr1 = $('<div>').addClass('user-address-st1')
                                .text(addresses[i].Addr1)
                                .appendTo(header);

                var addr2 = $('<div>').addClass('user-address-st2')
                                .text(addresses[i].Addr2)
                                .appendTo(header);
                var addr3 = $('<div>').addClass('user-address-st3')
                                .text(addresses[i].Addr3)
                                .appendTo(header);
                var csz = $('<div>').addClass('user-address-csz')
                                .text(addresses[i].City + ', ' + addresses[i].State + ' ' + addresses[i].Zip)
                                .appendTo(header);
                var country = $('<div>').addClass('ser-address-country')
                                .text(addresses[i].Country)
                                .appendTo(header);
                $('#userAddressList').append(header);
            }

            $('#userAddressList').prepend(addresslist);
        });

    }

    function GetPhoneNumbers() {
        top.Ts.Services.Users.GetUserPhoneNumbers(userID, function (phones) {
            var phonelist = '';

            if (phones.length === 0) {
                phonelist = '<div>There are no phone numbers to display.</div>';
            }
            for (var i = 0; i < phones.length; i++) {
                var header = $('<div>').addClass('user-phone')
                        .hover(function () {
                            $(this).find('.ts-icon').show();
                        }, function () {
                            $(this).find('.ts-icon').hide();
                        })
                        .data('data', phones[i].PhoneID);

                var link = $('<span>')
                                .addClass('fleft')
                                .html('<strong>' + phones[i].PhoneTypeName + '</strong>: ' + phones[i].Number + '  ' + (phones[i].Extension == '' ? '' : '<strong>Ext:</strong> ' + phones[i].Extension))
                                .appendTo(header);
                if (canEdit) {
                    $('<span>')
                .addClass('fleft ts-icon ts-icon-edit')
                .click(function (e) {
                    var item = $(this).parent();
                    var data = item.data('data');
                    ShowDialog(top.GetPhoneDialog(data));
                }).hide()
                .appendTo(header);

                    $('<span>')
                .addClass('fleft ts-icon ts-icon-delete')
                .click(function (e) {
                    if (confirm('Are you sure you would like to remove this phone number?')) {
                        var item = $(this).parent();
                        var data = item.data('data');
                        top.privateServices.DeletePhone(data);
                        window.location = window.location;
                    }
                    else {
                        $(this).prev().hide();
                        $(this).hide();
                    }
                }).hide()
                .appendTo(header);
                }
                $('#userPhoneList').append(header);
            }
            $('#userPhoneList').prepend(phonelist);
        });
    }

    function GetGroups() {
        top.Ts.Services.Users.GetUserGroups(userID, function (groups) {
            var groupslist = '';
            if (groups.length === 0) {
                groupslist = '<div>There are no groups to display.</div>';
            }

            for (var i = 0; i < groups.length; i++) {
                var header = $('<div>').addClass('user-group')
                            .hover(function () {
                                $(this).find('.ts-icon').show();
                            }, function () {
                                $(this).find('.ts-icon').hide();
                            })
                        .data('data', groups[i].GroupID);
                var link = $('<span>')
                                .addClass('group-name')
                                .text(groups[i].Name)
                                .appendTo(header);
                if (isSysAdmin) {
                    $('<span>')
                .addClass('fleft ts-icon ts-icon-delete')
                .click(function (e) {
                    if (confirm('Are you sure you would like to remove this group?')) {
                        var item = $(this).parent();
                        var data = item.data('data');
                        top.privateServices.DeleteGroupUser(data, _user.UserID);
                        window.location = window.location;
                    }
                    else
                        $(this).hide();
                }).hide()
                .appendTo(header);
                }

                $('#userGroups').append(header);
            }
            $('#userGroups').prepend(groupslist);
        });
    }

    $('.loading-section').hide().next().show();

};

UserPage.prototype = {
  constructor: UserPage,
  refresh: function () {

  }
};
