$(document).ready(function () {
    //apiKey = "45228242";
    var activeChatID = 0;
    var dateFormat;
    var chatInfoObject = {};
    var pusherKey = null;
    var contactID = null;

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

    SetupToolbar();

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
            //console.log(chat);
            chatInfoObject = chat;
            if (chat.InitiatorUserID !== null) {
                contactID = chat.InitiatorUserID;
                $('#chat-customer').show();
            }
            else {
                contactID = null;
                $('#chat-customer').hide();
            }

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

    function SetupToolbar() {
        //Leave Chat and remove from list of active chats
        $('#chat-leave').click(function (e) {
            e.preventDefault();
            parent.Ts.Services.Chat.CloseChat(activeChatID, function (success) {
                if (success) {
                    $('#active-chat_' + activeChatID).remove();
                    $('.media-list').empty();
                    $('.chat-intro').empty();
                    activeChatID = null;
                }
                else console.log('Error closing chat.')
            });
        });

        $('#chat-invite').click(function (e) {
            e.preventDefault();
            $('#chat-add-user-modal').modal('show');
        });

        var _execGetCustomer = null;
        var getUsers = function (request, response) {
            if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
            _execGetCustomer = parent.Ts.Services.Chat.GetUsers(request.term, function (result) { response(result); });
        };

        $('.chat-user-list').autocomplete({
            minLength: 2,
            source: getUsers,
            defaultDate: new Date(),
            select: function (event, ui) {
                $(this).data('item', ui.item);
            }
        });

        $('#add-user-save').click(function (e) {
            e.preventDefault();
            var userID = $('#chat-invite-user').data('item').id;
            parent.Ts.Services.Chat.RequestInvite(activeChatID, userID, function (data) {
                $('#chat-add-user-modal').modal('hide');
            });
        });

        $('#chat-transfer').click(function (e) {
            e.preventDefault();
            $('#chat-transfer-user-modal').modal('show');
        });

        $('#transfer-user-save').click(function (e) {
            e.preventDefault();
            var userID = $('#chat-transfer-user').data('item').id;
            parent.Ts.Services.Chat.RequestTransfer(activeChatID, userID, function (data) {
                $('#chat-transfer-user-modal').modal('hide');
            });
        });

        $('#chat-customer').click(function (e) {
            e.preventDefault();
            parent.Ts.MainPage.openContact(contactID);
        });

        //TODO: Port over suggestion code
        $('#chat-suggestions').click(function (e) {
            e.preventDefault();
            //parent.Ts.Services.Chat.CloseChat(activeChatID, function (success) {
            //    if (success) {

            //    }
            //    else console.log('Error closing chat.')
            //});
        });

        //TODO: Create Attachment upload logic
        $('#chat-attachment').click(function (e) {
            e.preventDefault();
            //parent.Ts.Services.Chat.CloseChat(activeChatID, function (success) {
            //    if (success) {

            //    }
            //    else console.log('Error closing chat.')
            //});
        });

        //TODO:  TOK Integration
        $('#chat-record').click(function (e) {
            e.preventDefault();
            //parent.Ts.Services.Chat.CloseChat(activeChatID, function (success) {
            //    if (success) {

            //    }
            //    else console.log('Error closing chat.')
            //});
        });

        //Create a ticket with the associated chat in it.
        $('#Ticket-Create').click(function (e) {
            e.preventDefault();
            parent.Ts.MainPage.newTicket('?ChatID=' + activeChatID);
            parent.Ts.System.logAction('Chat - Ticket Created');
        });

        var execGetRelated = null;
        var getTickets = function (request, response) {
            if (execGetRelated) { execGetRelated._executor.abort(); }
            execGetRelated = parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
        }

        $('.chat-tickets-list').autocomplete({
            minLength: 2,
            source: getTickets,
            defaultDate: new Date(),
            select: function (event, ui) {
                $(this).data('item', ui.item);
            }
        });

        $('#Ticket-Add').click(function (e) {
            e.preventDefault();
            $('#chat-add-ticket-modal').modal('show');
        });

        $('#ticket-add-save').click(function (e) {
            e.preventDefault();
            var ticketID = $('#chat-add-ticket').data('item').id;
            parent.Ts.Services.Chat.AddTicket(activeChatID, ticketID, function () {
                
                $('#chat-add-ticket-modal').modal('hide');
            });
        });

        //Open the Ticket assocaited with this chat
        $('#Ticket-Open').click(function (e) {
            e.preventDefault();
            parent.Ts.Services.Chat.GetTicketID(activeChatID, function (ticketID) {
                if (ticketID) {
                    parent.Ts.MainPage.openTicketByID(ticketID, false);
                }
                else console.log('Error opening associated ticket.')
            });
        });



    }
});