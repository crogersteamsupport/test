$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("chatid", window);
    var participantID = Ts.Utils.getQueryValue("pid", window);
    var chatObject;
    var channel;

    setupChat(chatID, participantID, function (channelObject) {
        channel = channelObject;
    });
    loadInitialMessages(chatID);

    $("#message-form").submit(function (e) {
        e.preventDefault();
        console.log(channel.members.me);
        var messageData = { channelName: 'presence-test', message: $('#message').val(), chatID: chatID, userID: participantID };
        
        IssueAjaxRequest("AddMessage", messageData,
        function (result) {
            $('#message').val('');
        },
        function (error) {

        });

    });
  
});

function createMessage(message)
{
    $('#message-list').append('<li class="list-group-item message-bubble"> <p>' + message + '</p></li>');
}

function createMessageElement(messageData) {
    $('#message-list').append('<li class="list-group-item message-bubble"> ' +
                            '<p class="text-muted">' + messageData.userName + '</p> ' +
                            '<p>' + messageData.message + '</p></li>');
}

function setupChat(chatID, participantID, callback) {
    var channelName = 'presence-test';
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', {
        authEndpoint: service + 'Auth', auth: {
            params: {
                chatID: chatID,
                participantID: participantID
            }
        }
    });
    var pressenceChannel = pusher.subscribe(channelName);

    pressenceChannel.bind('pusher:subscription_succeeded', function () {
        //console.log(channel.members);
    });

    pressenceChannel.bind('pusher:member_added', function (member) {
        $('#scopeMessage').remove();
        createMessage(member.info.name + ' joined the chat.')
    });

    pressenceChannel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    pressenceChannel.bind('agent-joined', function (data) {
        //console.log(data);
        $('#scopeMessage').remove();
    });

    pressenceChannel.bind('new-comment', function (data) {
        //console.log(data);
        createMessageElement(data);
    });

    callback(pressenceChannel);
}

function loadInitialMessages(chatID) {
    var chatObject = { chatID: chatID };

    IssueAjaxRequest("GetChatInfo", chatObject,
    function (result) {
        //console.log(result);
        chatObject = result;
        createMessage('Initiated On: ' + result.Chat.DateCreated);
        createMessage('Initiated By: ' + result.Initiator.FirstName + ' ' + result.Initiator.LastName + ', ' + result.Initiator.CompanyName + ' (' + result.Initiator.Email + ')');
        createMessage('Description:' + result.Chat.Message);
    },
    function (error) {

    });
}

var service = '../Services/ChatService.asmx/';
function IssueAjaxRequest(method, data, successCallback, errorCallback) {
    $.ajax({
        type: "POST",
        url: service + method,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        dataFilter: function (data) {
            var jsonResult = eval('(' + data + ')');
            if (jsonResult.hasOwnProperty('d'))
                return jsonResult.d;
            else
                return jsonResult;
        },
        success: function (jsonResult) {
            successCallback(jsonResult);
        },
        error: function (error, errorStatus, errorThrown) {
            if (errorCallback) errorCallback(error);
        }
    });
}
