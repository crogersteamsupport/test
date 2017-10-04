var notify = false;
var _pressenceChannel = false;

$(document).ready(function () {
    loadPusher();
});

function loadPusher() {
    $("#jquery_jplayer_1").jPlayer({
        ready: function () {
            $(this).jPlayer("setMedia", { mp3: "vcr/1_9_0/Audio/chime.mp3" });
        },
        loop: false,
        swfPath: "vcr/1_9_0/Js"
    });

    var pusher = null;
    var ticket_channel = null;
    var watercooler_channel = null;
    var pressenceChannel = null;

    var service = '../Services/DispatchService.asmx/';
    top.Ts.Settings.System.read('PusherKey', '1', function (key) {
        var orgID = top.Ts.System.Organization.OrganizationID;
        var userID = top.Ts.System.User.UserID;

        top.Ts.Pusher = new Pusher(key, {
            authEndpoint: service + 'Auth',
            auth: {
                params: { userID: top.Ts.System.User.UserID }
            }
        });

        console.log(top.Ts.Pusher);

        ticket_channel = top.Ts.Pusher.subscribe('ticket-dispatch-' + orgID);

        ticket_channel.bind('addThread', function (data) {
            var windows = getChildWindows();
            for (var i = 0; i < windows.length; i++) {
                try { if (windows[i].addThread) windows[i].addThread(data); } catch (err) { }
            }
        });

        ticket_channel.bind('addComment', function (data) {
            var windows = getChildWindows();
            for (var i = 0; i < windows.length; i++) {
                try { if (windows[i].addComment) windows[i].addComment(data); } catch (err) { }
            }
        });

        ticket_channel.bind('updateattachments', function (data) {
            var windows = getChildWindows();
            for (var i = 0; i < windows.length; i++) {
                try { if (windows[i].updateattachments) windows[i].updateattachments(data); } catch (err) { }
            }
        });

        ticket_channel.bind('deleteMessage', function (data) {
            var windows = getChildWindows();
            for (var i = 0; i < windows.length; i++) {
                try { if (windows[i].deleteMessage) windows[i].deleteMessage(data.msgID, data.parentID); } catch (err) { }
            }
        });

        ticket_channel.bind('updateLikes', function (data) {
            var windows = getChildWindows();
            for (var i = 0; i < windows.length; i++) {
                try { if (windows[i].updateLikes) windows[i].updateLikes(data.like, data.message, data.messageParent); } catch (err) { }
            }
        });

        ticket_channel.bind('getTicketViewing', function (data) {
            if ($('.main-ticket-' + data).length > 0) {
                if ($('.main-ticket-' + data).is(":visible")) {
                    // mainFrame.Ts.Services.Dispatch.ticketViewingAdd(data, top.Ts.System.User.UserID);
                    $('.main-ticket-' + data).find('iframe')[0].contentWindow.SetupPusher();
                }
            }
        });

        ticket_channel.bind('DisplayTicketUpdate', function (data) {
            var mergeticket;

            if (data.ticket.indexOf(',') != -1) {
                var mergeTickets = data.ticket.split(',');
                var losingTicket = mergeTickets[0];
                mergeticket = 1;
                top.Ts.MainPage.AppNotify("Ticket " + data.ticket, data.update);

                if ($('.main-ticket-' + losingTicket).length > 0) {
                    if (!$('.main-ticket-' + losingTicket).is(":visible")) {
                        top.Ts.MainPage.closeTicketTab(losingTicket);
                        top.Ts.MainPage.AppNotify("Ticket " + data.ticket, data.update);
                    }
                    else {
                        top.Ts.MainPage.AppNotify("Ticket " + data.ticket, data.update, "error");
                    }
                }
                data.ticket = mergeTickets[1];
            }

            if ($('.main-ticket-' + data.ticket).length > 0) {
                if (!$('.main-ticket-' + data.ticket).is(":visible") && mergeticket != 1) {
                    top.Ts.MainPage.AppNotify("Ticket " + data.ticket, data.update);

                    if (data.update.indexOf('delete') != -1) {
                        top.Ts.MainPage.closeTicketTab(data.ticket);
                    }
                }
                else {
                    if (data.update.indexOf('delete') != -1) {
                        top.Ts.MainPage.AppNotify("Ticket " + data.ticket, data.update, "error");
                    }
                }
                if (data.update.indexOf(top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName) < 0)
                $('.main-ticket-' + data.ticket).find('iframe')[0].contentWindow.loadTicket(data.ticket, 0);
            }
        });

        ticket_channel.bind('ticketRefreshSla', function (data) {
            if ($('.main-ticket-' + data).length > 0) {
                $('.main-ticket-' + data).find('iframe')[0].contentWindow.resetSLAInfo();
            }
        });

        ticket_channel.bind('chatMessage', function (data) {
            //chatHubClient.client.chatMessage = function (message, chatID, chatname) {
            if (data.reciever == top.Ts.System.User.UserID) {
                chatWith(data.chatname, data.chatID);
                chatAddMsg(data.chatID, data.message, data.chatname);

                if (notify) {
                    $("#jquery_jplayer_1").jPlayer("setMedia", {
                        mp3: "vcr/1_9_0/Audio/chime.mp3"
                    }).jPlayer("play", 0);
                }
            }
        });

    });



    function getChildWindows()  {
        var result = [];
        function addWindow(element) {
            for (var i = 0; i < element.length; i++) {
                try {
                    if (element[i].contentWindow && element[i].contentWindow != null) result.push(element[i].contentWindow);
                } catch (e) {}
            }
        }
        addWindow($("#iframe-mniWC2"));
        addWindow($("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame"));
        addWindow($(".customerIframe").contents().find("#watercoolerIframe"));
        addWindow($("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
        addWindow($(".ticketIframe").contents().find("#watercoolerIframe"));
        return result;
    }

    function getTicketWindows() {
        var result = [];
        function addWindow(element) {
            for (var i = 0; i < element.length; i++) {
                try {
                    if (element[i].contentWindow && element[i].contentWindow != null) result.push(element[i].contentWindow);
                } catch (e) { }
            }
        }
        addWindow($("#iframe-mniWC2"));
        addWindow($("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame"));
        addWindow($(".customerIframe").contents().find("#watercoolerIframe"));
        addWindow($("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations"));
        addWindow($(".ticketIframe").contents().find("#watercoolerIframe"));
        return result;
    }

    originalTitle = document.title;

    $(window).blur(function () {
        notify = true;
    });

    $(window).focus(function () {
        notify = false;
    });

}

function openChat(name, chatid) {
    chatWith(name, chatid);
}

function chime(chimeType) {
    $("#jquery_jplayer_1").jPlayer("setMedia", {
        mp3: "vcr/1_9_0/Audio/drop.mp3"
    }).jPlayer("play", 0);
}

function SetupPusher() {
    console.log("setup pusher");
    var pressenceChannel = null;
    var service = '/Services/DispatchService.asmx/';
    top.Ts.TicketViewing = _ticketNumber;
    top.Ts.Settings.System.read('PusherKey', '1', function (key) {
        var orgID = top.Ts.System.Organization.OrganizationID;
        var userID = top.Ts.System.User.UserID;

        var presenceChannelName = 'presence-ticket-' + _ticketNumber + '-org-' + orgID;

        console.log(presenceChannelName);
        pressenceChannel = top.Ts.Pusher.subscribe(presenceChannelName);

        pressenceChannel.bind('pusher:subscription_succeeded', function (members) {
            try {
                addUsersViewing(members);
                console.log("sub succeeded");
            } catch (err) { }
        });

        pressenceChannel.bind('pusher:member_added', function (member) {
            try {
                console.log("add user viewing");
                addUserViewing(member.id);
            } catch (err) { }
        });

        pressenceChannel.bind('pusher:member_removed', function (member) {
            try {
                console.log("removing user");
                removeUserViewing(member.id);
            } catch (err) { }
        });

        pressenceChannel.bind('ticketViewingRemove', function (data) {
            console.log("ticketViewingRemove pusher");
            top.Ts.Pusher.unsubscribe(data.chan);
        });
    });
}

function addUsersViewing (members) {
    members.each(function (member) {
        addUserViewing(member.id);
    });
}

function addUserViewing (userID) {
    if (userID != top.Ts.System.User.UserID) {
        $('#ticket-now-viewing').show();
        if ($('.ticket-viewer[data-ChatID="' + userID + '"]').length < 1) {
            window.parent.Ts.Services.Users.GetUser(userID, function (user) {
                $('.ticket-viewer[data-ChatID="' + user.UserID + '"]').remove();
                var fullName = user.FirstName + " " + user.LastName;
                var viewuser = $('<a>').data('ChatID', user.UserID).data('Name', fullName).addClass('ticket-viewer').click(function () {
                    window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
                    window.parent.Ts.System.logAction('Now Viewing - Chat Opened');
                }).html('<img class="user-avatar ticket-viewer-avatar" src="../../../dc/' + user.OrganizationID + '/useravatar/' + user.UserID + '/48">' + fullName + '</a>').appendTo($('#ticket-viewing-users'));
            });
        }
    }
}

function removeUserViewing (userID) {
    if ($('.ticket-viewer[data-ChatID="' + userID + '"]').length > 0) {
        $('.ticket-viewer[data-ChatID="' + userID + '"]').remove();
        if ($('.ticket-viewer').length < 1) {
            $('#ticket-now-viewing').hide();
        }
    }
}
