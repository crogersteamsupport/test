$(document).ready(function () {
    //apiKey = "45228242";
    var activeChatID = 0;
    var dateFormat;
    var chatInfoObject = {};
    var pusherKey = null;

    window.parent.Ts.Services.Customers.GetDateFormat(false, function (format) {
        dateFormat = format.replace("yyyy", "yy");
        if (dateFormat.length < 8) {
            var dateArr = dateFormat.split('/');
            if (dateArr[0].length < 2) {
                dateArr[0] = dateArr[0] + dateArr[0];
            }
            if (dateArr[1].length < 2) {
                dateArr[1] = dateArr[1] + dateArr[1];
            }
            if (dateArr[2].length < 2) {
                dateArr[1] = dateArr[1] + dateArr[1];
            }
            dateFormat = dateArr[0] + "/" + dateArr[1] + "/" + dateArr[2];
        }
    });

    subscribeToNewChatRequest(function (request) {
        SetupPendingRequest(request.chatRequest, false);
    });

    top.Ts.Settings.System.read('PusherKey', '1', function (key) {
        pusherKey = key;
        SetupChatRequests();

        $('.page-loading').hide().next().show();
    });


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
                             '<p>Time:  ' + moment(chat.DateCreated).format(dateFormat + ' hh:mm A') + '</p>' +
                             '<p>Message:  ' + chat.Description + '</p>')
                             .append(acceptBtn)
                             .addClass('open-request');
        });

        $('#chats-requests').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

    }

    function SetupActiveRequest(chat, shouldTrigger) {
        var anchor = $('<a id="active-chat_' + chat.ChatID + '" href="#" class="list-group-item">' + chat.InitiatorDisplayName + '</a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info');

            activeChatID = chat.ChatID;
            SetActiveChat(activeChatID);
        });

        $('#chats-accepted').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

        setupChat(pusherKey, chat.ChatID, createMessageElement, function (channel) {
            //console.log(channel);
        });

    }

    function CloseRequestAnchor() {
        $('.open-request').html($('.open-request > .userName').text()).removeClass('open-request');
    }

    function AcceptRequest(ChatRequestID, innerString, parentEl)  {
        parent.Ts.Services.Chat.AcceptRequest(ChatRequestID, function (chatId) {
            setupChat(pusherKey, chatId, createMessageElement, function (channel) {
                //console.log(channel);
            });

            parentEl.remove();
            MoveAcceptedRequest(innerString, chatId);

            activeChatID = chatId;
            SetActiveChat(activeChatID);
        });
    }

    function MoveAcceptedRequest(innerString, chatID) {
        $('.list-group-item-success').removeClass('list-group-item-success');
        var anchor = $('<a id="active-chat_' + chatID + '" href="#" class="list-group-item list-group-item-success">' + innerString + '</a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info');

            activeChatID = chatID;
            SetActiveChat(activeChatID);
        });

        $('#chats-accepted').append(anchor);
    }

    function createMessageElement(messageData, scrollView) {
        console.log(messageData)
        if (messageData.ChatID == activeChatID) {
            var messageTemplate = $("#message-template").html();
            var compiledTemplate = messageTemplate
                                    .replace('{{MessageDirection}}', 'left')
                                    .replace('{{UserName}}', messageData.CreatorDisplayName)
                                    .replace('{{Avatar}}', (messageData.CreatorID !== null)
                                                                    ? 'https://app.teamsupport.com/dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1470773158079'
                                                                    : '../images/blank_avatar.png')
                                    .replace('{{Message}}', messageData.Message)
                                    .replace('{{Date}}', moment(messageData.DateCreated).format(dateFormat + ' hh:mm A'));

            $('.media-list').append(compiledTemplate);
            if (scrollView) ScrollMessages(true);
        }
        else 
        {
            $('#active-chat_' + messageData.ChatID).addClass('list-group-item-info');
        }
    }

    function SetActiveChat(chatID) {
        parent.Ts.Services.Chat.GetChatDetails(chatID, function (chat) {
            chatInfoObject = chat;
            console.log(chat);
            $('.media-list').empty();
            $('.chat-intro').empty();
            $('.chat-intro').append('<p>Initiated On: ' + chat.DateCreated + '</p>');
            $('.chat-intro').append('<p>Initiated By: ' + chat.InitiatorMessage + '</p>');

            for(i = 0; i <  chat.Messages.length; i++)
            {
                createMessageElement(chat.Messages[i], false);
            }
            ScrollMessages(false);
        });
    }

    function ScrollMessages(animated) {
        if (animated)
            $(".current-chat-area").animate({ scrollTop: $('.current-chat-area').prop("scrollHeight") }, 1000);
        else
            $(".current-chat-area").scrollTop($('.current-chat-area').prop("scrollHeight"));
    }

    $("#new-message").click(function (e) {
        e.preventDefault();
        parent.Ts.Services.Chat.AddAgentMessage('presence-' + activeChatID, $('#message').val(), activeChatID, function (data) {
            $('#message').val('');
        });

    });

});