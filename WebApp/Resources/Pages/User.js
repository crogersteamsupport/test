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
    userPage.refresh();
}

function ShowDialog(wnd) {
    wnd.add_close(DialogClosed);
    wnd.show();
}


UserPage = function () {
    $('#btnRefresh')
  .click(function (e) {
      e.preventDefault();
      window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    $('button').button();
    $('a').addClass('ui-state-default ts-link');
    var userID = top.Ts.Utils.getQueryValue("userid", window);
    var orgID;

    top.Ts.Services.Users.GetUser(userID, function (user) {
        orgID = user.OrganizationID;

        $('.user-displayname').text(user.FirstName + ' ' + user.LastName);
        $('#userEmail').html('<a href="mailto:' + user.Email + '">' + user.Email + '</a>');
        $('#userTitle').text(user.Title);
        $('#userTimeZone').text(user.TimeZoneID);
        $('#userLastLogin').text(user.LastLogin);
        $('#userActive').text(user.IsActive);
        $('#userEmailNotify').text(user.ReceiveTicketNotifications);
        $('#userSubscribeTickets').text(user.SubscribeToNewTickets);
        $('#userSubscribeActions').text(user.SubscribeToNewActions);
        $('#userAutoSubscribe').text(user.DoNotAutoSubscribe);
        $('#userGroupNotify').text(user.ReceiveAllGroupNotifications);
        $('#userDateFormat').text(user.CultureName);
        $('#userSysAdmin').text(user.IsSystemAdmin);
        $('#userFinanceAdmin').text(user.IsFinanceAdmin);
        $('#userInformation').append(user.Information);
    });


    top.Ts.Services.Users.GetUserAddresses(userID, function (addresses) {
        var addresslist = '';

        for (var i = 0; i < addresses.length; i++) {
            addresslist += '<div class="user-address">';
            addresslist += '<h3 class="user-address-desc">' + addresses[i].Description + '</h3>';
            addresslist += '<div class="user-address-st1">' + addresses[i].Addr1 + '</div>';
            addresslist += '<div class="user-address-st2">' + addresses[i].Addr2 + '</div>';
            addresslist += '<div class="user-address-st3">' + addresses[i].Addr3 + '</div>';
            addresslist += '<div class="user-address-csz">' + addresses[i].City + ', ' + addresses[i].State + ' ' + addresses[i].Zip + '</div>';
            addresslist += '<div class="user-address-country">' + addresses[i].Country + '</div>';
            addresslist += '</div>';
        }

        $('#userAddressList').prepend(addresslist);
    });

    top.Ts.Services.Users.GetUserPhoneNumbers(userID, function (phones) {
        var phonelist = '';

        for (var i = 0; i < phones.length; i++) {
            phonelist += '<div class="user-phone"><span class="property">' + phones[i].PhoneTypeName + ':</span>';
            phonelist += '<span class="value">' + phones[i].Number + '</span></div>';
        }

        $('#userPhoneList').prepend(phonelist);
    });

    top.Ts.Services.Users.GetUserGroups(userID, function (groups) {
        var groupslist = '';

        for (var i = 0; i < groups.length; i++) {
            groupslist += groups[i].Name;

            if (i != groups.length - 1) {
                groupslist += ', ';
            }
        }

        $('#userGroupsList').append(groupslist);
    });

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
        if (icon.hasClass('ui-icon-triangle-1-s')) {
            icon.removeClass('ui-icon-triangle-1-s').addClass('ui-icon-triangle-1-e');
            $(this).next().hide();
        }
        else {
            icon.removeClass('ui-icon-triangle-1-e').addClass('ui-icon-triangle-1-s');
            $(this).next().show();
        }
    });

    var canEdit = top.Ts.System.User.UserID === userID || top.Ts.System.User.IsSystemAdmin;
    if (canEdit) {
        $('.ts-icon-edit').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            ShowDialog(top.GetUserDialog(orgID, userID));
        });

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
    }
    else {
        $('.ts-icon-edit').remove();
        $('.ts-add').remove();
    }

    $('.loading-section').hide().next().show();
};


UserPage.prototype = {
  constructor: UserPage,
  refresh: function () {

  }
};
