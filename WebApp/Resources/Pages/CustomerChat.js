$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("chatid", window);


    $("#message-form").submit(function (e) {
        e.preventDefault();

        var messageData = { channelName: 'presence-test', message: $('#message').val() };
        
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

function setupChat(email, chatID, name) {
    var channelName = 'presence-test';
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', {
        authEndpoint: service + 'Auth', auth: {
            params: {
                chatGuid: chatID,
                email: email,
                name: name
            }
        }
    });
    var channel = pusher.subscribe(channelName);

    channel.bind('pusher:subscription_succeeded', function () {
        console.log(channel.members);
    });

    channel.bind('pusher:member_added', function (member) {
        $('#scopeMessage').remove();
        createMessage(member.info.name + ' joined the chat.')
    });

    channel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    channel.bind('new-comment', function (data) {
        console.log(data);
        createMessageElement(data);
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
