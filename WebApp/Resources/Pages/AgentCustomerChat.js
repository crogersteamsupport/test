$(document).ready(function () {
    //apiKey = "45228242";
    var activeChatID = 0;

    SetupChatRequests();

    $('.page-loading').hide().next().show();

    function SetupChatRequests() {
        parent.Ts.Services.Chat.GetChatRequests(function (data) {
            //console.log(data);
            for (i = 0; i < data.length; i++)
            {
                SetupPendingRequest(data[i], (i == 0));
            }

        });

        parent.Ts.Services.Chat.GetActiveChats(function (data) {
            //console.log(data);
            for (a = 0; a < data.length; a++) {
                SetupActiveRequest(data[a], (a == 0));
            }
        });
    }

    function SetupPendingRequest(chat, shouldTrigger) {
        var innerString = chat.InitiatorDisplayName;
        var anchor = $('<a id="' + chat.ChatRequestID + '" href="#" class="list-group-item chat-request">' + innerString + '</a>').click(function (e) {
            e.preventDefault();

            CloseRequestAnchor();

            var acceptBtn = $('<button class="btn btn-default">Accept</button>').click(function (e) {
                var parentEl = $(this).parent();
                AcceptRequest(chat.ChatRequestID, innerString, parentEl);
            });

            $(this).html('<p class="userName">' + innerString + '</p>' +
                             '<p>Email:  ' + chat.InitiatorEmail + '</p>' +
                             '<p>Time:  ' + chat.DateCreated + '</p>' +
                             '<p>Message:  ' + chat.Description + '</p>')
                             .append(acceptBtn)
                             .addClass('open-request');
        });

        $('#chats-requests').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

    }

    function SetupActiveRequest(chat, shouldTrigger) {
        var innerString = chat.InitiatorDisplayName;

        var anchor = $('<a id="' + chat.ChatID + '" href="#" class="list-group-item">' + innerString + '</a>').click(function (e) {
            e.preventDefault();
            activeChatID = chat.ChatID;
            SetActiveChat(activeChatID);
        });

        $('#chats-accepted').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

        setupChat(chat.ChatID, createMessageElement, function (channel) {
            console.log(channel)
        });

    }

    function CloseRequestAnchor() {
        $('.open-request').html($('.open-request > .userName').text()).removeClass('open-request');
    }

    function AcceptRequest(ChatRequestID, innerString, parentEl)  {
        parent.Ts.Services.Chat.AcceptRequest(ChatRequestID, function (chatId) {
            setupChat(chatId, createMessageElement, function (channel) {
                console.log(channel)
            });

            parentEl.remove();
            MoveAcceptedRequest(innerString, chatId);
        });
    }

    function MoveAcceptedRequest(innerString, chatID) {
        var anchor = $('<a href="#" class="list-group-item">' + innerString + '</a>');
        $('#chats-accepted').append(anchor);
    }

    function createMessageElement(messageData) {
        var messageTemplate = $("#message-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{MessageDirection}}', 'left')
                                .replace('{{UserName}}', messageData.userName)
                                .replace('{{Avatar}}', (messageData.userID !== null)
                                                                ? 'https://app.teamsupport.com/dc/' + messageData.OrganizationID + '/UserAvatar/' + messageData.userID + '/48/1470773158079'
                                                                : 'https://app.teamsupport.com/dc/1078/UserAvatar/1839999/48/1470773158079')
                                .replace('{{Message}}', messageData.message)
                                .replace('{{Date}}', '1 min ago');

        $('.media-list').append(compiledTemplate);
    }

    function SetActiveChat(chatID) {
        parent.Ts.Services.Chat.GetChatDetails(chatID, function (chat) {
            //console.log(chat);
            $('.media-list').empty();
            $('.chat-intro').empty();
            $('.chat-intro').append('<p>Initiated On: ' + chat.DateCreated + '</p>');
            $('.chat-intro').append('<p>Initiated By: ' + chat.InitiatorMessage + '</p>');

            for(i = 0; i <  chat.Messages.length; i++)
            {
                var message = chat.Messages[i];
                var messageData = {};
                messageData.userName = message.CreatorDisplayName;
                messageData.userID = message.CreatorID;
                messageData.OrganizationID = chat.OrganizationID;
                messageData.message = message.Message;
                createMessageElement(messageData);
            }
        });
    }

    $("#new-message").click(function (e) {
        e.preventDefault();
        alert('clicked');
        parent.Ts.Services.Chat.AddAgentMessage('presence-' + activeChatID, $('#message').val(), activeChatID, function (data) {
            //alert('posted')
        });

    });

});