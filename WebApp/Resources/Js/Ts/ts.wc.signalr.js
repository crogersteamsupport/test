
var chatHubClient = $.connection.socket;
var notify = false;

$(document).ready(function () {

    $("#jquery_jplayer_1").jPlayer({
        ready: function () {
            $(this).jPlayer("setMedia", {
                mp3: "vcr/142/Audio/chime.mp3"
            });
        },
        loop: false,
        swfPath: "vcr/142/Js"
    });

    //Debug reasons
    $.connection.hub.logging = true;
    $.connection.hub.url = "signalr/signalr";
    // Start the connection only if on main wc page

    $.connection.hub.start(function () {
        chatHubClient.login(top.Ts.System.User.UserID);
    });

    chatHubClient.chatMessage = function (message, chatID, chatname) {

        chatWith(chatname, chatID);
        chatAddMsg(chatID, message, chatname);

        if (notify) {
            $("#jquery_jplayer_1").jPlayer("play", 0);
        }
    };

    chatHubClient.addMessage = function (data) {
        //    $("#mainWC").contents().find("#messages").append('<li>' + data + '</li>');
        //    $(".ticketIframe").contents().find("#watercoolerIframe").contents().find("#messages").append('<li>' + data + '</li>');

        //$("#mainWC").contents().teststring(data);
        //$("#mainWC").contentWindow.teststring(data);
        var mainWC = $("#mainWC");
        mainWC[0].contentWindow.teststring(data);
        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        for (var i = 0; i < ticketWC.length; i++) {
            ticketWC[i].contentWindow.teststring(data);
        }
    };

    chatHubClient.addThread = function (message) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.addThread(message);
        }
        catch (err) { }

        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.addThread(message);
            }
        }
        catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.addThread(message);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.addThread(message);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.addThread(message);
        } catch (err) { }


    };

    chatHubClient.addComment = function (message) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.addComment(message);
        }
        catch (err) { }

        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.addComment(message);
            }
        }
        catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.addComment(message);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.addComment(message);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.addComment(message);
        } catch (err) { }
    };

    chatHubClient.deleteMessage = function (messageID, parentID) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.deleteMessage(messageID, parentID);
        }
        catch (err) { }

        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.deleteMessage(messageID, parentID);
            }
        }
        catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.deleteMessage(messageID, parentID);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.deleteMessage(messageID, parentID);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.deleteMessage(messageID, parentID);
        } catch (err) { }

    };

    chatHubClient.updateLikes = function (likes, messageID, messageParentID) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        }
        catch (err) { }
        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.updateLikes(likes, messageID, messageParentID);
            }
        }
        catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }
    };

    chatHubClient.updateattachments = function (message) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.updateattachments(message);
        } catch (err) { }
        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.updateattachments(message);
            }
        } catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.updateLikes(likes, messageID, messageParentID);
        } catch (err) { }
    };

    chatHubClient.disconnect = function (windowid) {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.disconnect(windowid);
        } catch (err) { }

        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.disconnect(windowid);
            }
        } catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.disconnect(windowid);
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.disconnect(windowid);
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.disconnect(windowid);
        } catch (err) { }
    };

    chatHubClient.updateUsers = function () {
        var mainWC = $("#iframe-mniWC2");
        try {
            mainWC[0].contentWindow.updateUsers();
        } catch (err) { }

        var ticketWC = $(".ticketIframe").contents().find("#watercoolerIframe");
        try {
            for (var i = 0; i < ticketWC.length; i++) {
                ticketWC[i].contentWindow.updateUsers();
            }
        } catch (err) { }

        var groupWC = $("#iframe-mniGroups").contents().find("#ctl00_ContentPlaceHolder1_groupContentFrame");
        try {
            groupWC[0].contentWindow.updateUsers();
        } catch (err) { }

        var customerWC = $("#iframe-mniCustomers").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            customerWC[0].contentWindow.updateUsers();
        } catch (err) { }

        var productWC = $("#iframe-mniProducts").contents().find("#ctl00_ContentPlaceHolder1_frmOrganizations");
        try {
            productWC[0].contentWindow.updateUsers();
        } catch (err) { }
    };

    originalTitle = document.title;

    $(window).blur(function () {
        notify = true;
    });

    $(window).focus(function () {
        notify = false;
    });





});

function openChat(name, chatid) {
    chatWith(name, chatid);
}



