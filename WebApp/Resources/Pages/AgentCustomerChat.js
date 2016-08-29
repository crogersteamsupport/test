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
                SetupPendingRequest(data[i].Chat, data[i].Initiator, (i == 0));
            }

        });

        parent.Ts.Services.Chat.GetActiveChats(function (data) {
            //console.log(data);
            for (a = 0; a < data.length; a++) {
                SetupActiveRequest(data[a].Chat, data[a].Initiator, (a == 0));
            }
        });
    }

    function SetupPendingRequest(chat, user, shouldTrigger) {
        var innerString = user.FirstName + ' ' + user.LastName + ' - ' + user.CompanyName
        var anchor = $('<a id="' + chat.ChatRequestID + '" href="#" class="list-group-item chat-request">' + innerString + '</a>').click(function (e) {
            e.preventDefault();

            CloseRequestAnchor();

            var acceptBtn = $('<button class="btn btn-default">Accept</button>').click(function (e) {
                var parentEl = $(this).parent();
                AcceptRequest(chat.ChatRequestID, innerString, parentEl);
            });

            $(this).html('<p class="userName">' + innerString + '</p>' +
                             '<p>Email:  ' + user.Email + '</p>' +
                             '<p>Time:  ' + chat.DateCreated + '</p>' +
                             '<p>Message:  ' + chat.Message + '</p>')
                             .append(acceptBtn)
                             .addClass('open-request');
        });

        $('#chats-requests').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

    }

    function SetupActiveRequest(chat, user, shouldTrigger) {
        var innerString = user.FirstName + ' ' + user.LastName + ' - ' + user.CompanyName

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
            console.log(chat);
            $('.media-list').empty();
            $('.chat-intro').empty();
            $('.chat-intro').append('<p>Initiated On: ' + chat.Chat.DateCreated + '</p>');
            $('.chat-intro').append('<p>Initiated By: ' + chat.Initiator.FirstName + ' ' + chat.Initiator.LastName + ', ' + chat.Initiator.CompanyName + ' (' + chat.Initiator.Email + ')</p>');

            for(i = 0; i <  chat.Messages.length; i++)
            {
                var message = chat.Messages[i];
                var messageData = {};
                messageData.userName = "matt townsen";
                messageData.userID = message.PosterID;
                messageData.OrganizationID = chat.Chat.OrganizationID;
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