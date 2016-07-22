﻿$(document).ready(function () {
    var channelName = 'presence-test';
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', { authEndpoint: service + 'Auth' });
    var channel = pusher.subscribe(channelName);
    var chatID = Ts.Utils.getQueryValue("uid", window);

    channel.bind('pusher:subscription_succeeded', function () {
        console.log(channel.members);
    });

    channel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    channel.bind('new-comment', function (data) {
        console.log(data);
        createMessageElement(data);
    });
    
    $("#newChatForm").submit(function (e) {
        e.preventDefault();

        var contactInfo = { chatGuid: chatID, fName: $('#userFirstName').val(), lName: $('#userLastName').val(), email: $('#userEmail').val() };

        IssueAjaxRequest("GetContact", contactInfo,
        function (result) {debugger
            console.log(result)
        },
        function (error) {
            debugger
            console.log(error)
        });
    });


    $("#message-form").submit(function (e) {
        e.preventDefault();

        var messageData = { channelName: 'presence-test', message: $('#message').val() };
        
        IssueAjaxRequest("AddMessage", messageData,
        function (result) {

        },
        function (error) {

        });

    });
  
});

function createMessageElement(messageData) {
    $('#message-list').append('<li class="list-group-item message-bubble"> ' +
                            '<p class="text-muted">' + messageData.userName + '</p> ' +
                            '<p>' + messageData.message + '</p></li>');
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
