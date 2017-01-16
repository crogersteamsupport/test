var _activeChatID = null;
var dateFormat;
var _intervalUpdateActiveChats = null;
$(document).ready(function () {
    //apiKey = "45228242";
    var chatInfoObject = {};
    var pusherKey = null;
    var contactID = null;
    var uploadPath = '../../../Upload/ChatAttachments/';

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

    top.Ts.Settings.System.read('PusherKey', '1', function (key) {
        pusherKey = key;
        SetupChatRequests();
        subscribeToNewChatRequest(pusherKey, function (request) {
            SetupPendingRequest(request.chatRequest, true);
        });

        $('.page-loading').hide().next().show();
    });

    GetChatSettings();
    SetupToolbar();
    SetupTOK();

    function GetChatSettings() {
        parent.Ts.Services.Chat.GetAgentChatProperties(function (data) {
            if (!data.TOKScreenEnabled)
                $('#chat-tok-screen').hide();
            if (!data.TOKVideoEnabled)
                $('#chat-tok-video').hide();
            if (!data.TOKVoiceEnabled)
                $('#chat-tok-audio').hide();
        });
    }

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
            $(this).addClass('list-group-item-info');

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
        var initiator = chat.InitiatorDisplayName;

        if (typeof chat.CompanyName !== "undefined" && chat.CompanyName) {
            initiator = initiator + ' - ' + chat.CompanyName;
        }

        var anchor = $('<a id="active-chat_' + chat.ChatID + '" href="#" class="list-group-item">' + initiator + '</a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info');

            _activeChatID = chat.ChatID;
            SetActiveChat(_activeChatID);
        });

        $('#chats-accepted').append(anchor);
        if (shouldTrigger) anchor.trigger("click");

        setupChat(pusherKey, chat.ChatID, createMessageElement, function (channel) {
            //console.log(channel);
        });

    }

    function doneTyping() {
        //$('#typing').hide();
        if (channel !== null)
            var triggered = channel.trigger('client-agent-stop-typing', channel.members.me.info.name + ' is typing...');
        isTyping = false;
    }

    function CloseRequestAnchor() {
        $('.open-request').html($('.open-request > .userName').text()).removeClass('open-request').removeClass('list-group-item-info');
    }

    function AcceptRequest(ChatRequestID, innerString, parentEl)  {
        parent.Ts.Services.Chat.AcceptRequest(ChatRequestID, function (chatId) {
            setupChat(pusherKey, chatId, createMessageElement, function (channel) {
                //console.log(channel);
            });

            parentEl.remove();
            MoveAcceptedRequest(innerString, chatId);

            _activeChatID = chatId;
            SetActiveChat(_activeChatID);
        });
    }

    function MoveAcceptedRequest(innerString, chatID) {
        $('.list-group-item-success').removeClass('list-group-item-success');
        var anchor = $('<a id="active-chat_' + chatID + '" href="#" class="list-group-item list-group-item-success">' + innerString + '</a>').click(function (e) {
            e.preventDefault();

            $('.list-group-item-success').removeClass('list-group-item-success');
            $(this).addClass('list-group-item-success')
                    .removeClass('list-group-item-info');

            _activeChatID = chatID;
            SetActiveChat(_activeChatID);
        });

        $('#chats-accepted').append(anchor);
    }

    function createMessageElement(messageData, scrollView) {
        //console.log(messageData)
        if (messageData.ChatID == _activeChatID) {
            var messageTemplate = $("#message-template").html();
            var compiledTemplate = messageTemplate
                                    .replace('{{MessageDirection}}', 'left')
                                    .replace('{{UserName}}', messageData.CreatorDisplayName)
                                    .replace('{{Avatar}}', (messageData.CreatorID !== null)
                                                                    ? '../../../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1470773158079'
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
            $('.chat-intro').append('<p>Initiated On: ' + moment(chat.DateCreated).format(dateFormat + ' hh:mm A') + '</p>');
            $('.chat-intro').append('<p>Initiated By: ' + chat.InitiatorMessage + '</p>');

            for(i = 0; i <  chat.Messages.length; i++)
            {
                createMessageElement(chat.Messages[i], false);
            }
            ScrollMessages(false);

            if ($('#Ticket-Create').hasClass("disabled")) {
                $('#Ticket-Create').removeClass("disabled");
            }
            _intervalUpdateActiveChats = setInterval('EnableDisableTicketMenu();', 5200);
        });
    }

    function ScrollMessages(animated) {
        if (animated)
            $(".current-chat-area").animate({ scrollTop: $('.current-chat-area').prop("scrollHeight") }, 1000);
        else
            $(".current-chat-area").scrollTop($('.current-chat-area').prop("scrollHeight"));
    }

    $("#message-form").submit(function (e) {
        e.preventDefault();
        $('#new-message').prop("disabled", true);
        doneTyping();
        parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, $('#message').val(), _activeChatID, function (data) {
            $('#message').val('');
            $('#new-message').prop("disabled", false);
        });

    });

    function SetupToolbar() {
        //Leave Chat and remove from list of active chats
        $('#chat-leave').click(function (e) {
            e.preventDefault();
            if (confirm('Are you sure you want to leave this chat?')) {
                parent.Ts.Services.Chat.CloseChat('presence-' + _activeChatID, _activeChatID, function (success) {
                    if (success) {
                        $('#active-chat_' + _activeChatID).remove();
                        $('.media-list').empty();
                        $('.chat-intro').empty();
                        _activeChatID = null;
                    }
                    else console.log('Error closing chat.')
                });
            } else {
                // Do nothing!
            }
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
            parent.Ts.Services.Chat.RequestInvite(_activeChatID, userID, function (data) {
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
            parent.Ts.Services.Chat.RequestTransfer(_activeChatID, userID, function (data) {
                $('#chat-transfer-user-modal').modal('hide');
            });
        });

        $('#chat-customer').click(function (e) {
            e.preventDefault();
            parent.Ts.MainPage.openContact(contactID);
        });

        var _suggestedSolutionDefaultInput = '';
        $('#chat-suggestions').click(function (e) {
            e.preventDefault();
            suggestedSolutions(function (ticketID, isArticle) {
                //console.log(ticketID + ' ' + isArticle);

                if (isArticle) {
                    top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                        if (result === null) {
                            alert('There was an error inserting your suggested solution ticket.');
                            return;
                        }
                        var ticket = result[0];
                        var actions = result[1];

                        var html = '<div>';

                        if (actions.length == 0) {
                            alert('The selected ticket has no knowledgebase actions.');
                        }

                        for (var i = 0; i < actions.length; i++) {
                            html = html + '<div>' + actions[i].Description + '</div></br>';
                        }
                        html = html + '</div>';

                        parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, html, _activeChatID, function (data) {
;
                        });

                        top.Ts.System.logAction('Chat - Suggested Solution Inserted');
                    }, function () {
                        alert('There was an error inserting your suggested solution ticket.');
                    });
                }
                else {
                    top.Ts.Services.Admin.GetHubURLwithCName(function (url) {
                        var link = "https://" + url + "/knowledgeBase/" + ticketID;
                        var html = '<a href="' + link + '" target="_blank">' + link + '</a></br>';

                        parent.Ts.Services.Chat.AddAgentMessage('presence-' + _activeChatID, html, _activeChatID, function (data) {

                        });
                        top.Ts.System.logAction('Chat - Suggested Solution Link Inserted');
                    });

                }
            });
        });

        $('#hiddenAttachmentInput').fileupload({
            dropZone: $('.current-chat'),
            add: function (e, data) {
                data.url = uploadPath + _activeChatID;
                var jqXHR = data.submit()
                    .success(function (result, textStatus, jqXHR) {
                        var attachment = JSON.parse(result)[0];
                        parent.Ts.Services.Chat.AddAgentUploadtMessage('presence-' + _activeChatID, _activeChatID, attachment.id);
                    })
                    .error(function (jqXHR, textStatus, errorThrown) { console.log(textStatus) })
            },
        });

        $('#chat-attachment').click(function (e) {
            e.preventDefault();
            $('#hiddenAttachmentInput').click();
        });

        //Create a ticket with the associated chat in it.
        $('#Ticket-Create').click(function (e) {
            e.preventDefault();
            parent.Ts.MainPage.newTicket('?ChatID=' + _activeChatID);
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
            var ticketID = $('#chat-add-ticket').data('item').data;
            parent.Ts.Services.Chat.AddTicket(_activeChatID, ticketID, function () {
                parent.Ts.MainPage.openTicketByID(ticketID, false);
                $('#chat-add-ticket-modal').modal('hide');
            });
        });

        //Open the Ticket associated with this chat
        $('#Ticket-Open').click(function (e) {
            e.preventDefault();
            parent.Ts.Services.Chat.GetTicketID(_activeChatID, function (ticketID) {
                if (ticketID) {
                    parent.Ts.MainPage.openTicketByID(ticketID, false);
                }
                else console.log('Error opening associated ticket.')
            });
        });



    }

    var execSuggestedSolutions = null;
    var execSelectTicket = null;
    var selectTicket = function (request, response) {
        if (execSelectTicket) { execSelectTicket._executor.abort(); }
        var filter = $(this.element).data('filter');
        if (filter === undefined) {
            execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
        }
        else {
            execSelectTicket = window.parent.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
        }
    }

    function suggestedSolutions(callback) {
        $('.dialog-select-ticket2').find('input').val('');
        $('#SuggestedSolutionsModal').modal('show');
        $('#SuggestedSolutionsModal').on('shown.bs.modal', function () {
            $("#dialog-select-ticket2-input").focus();
        })
        if (execSuggestedSolutions) {
            return;
        }
        execSuggestedSolutions = true;
        $('#SuggestedSolutionsIFrame').attr('src', '/vcr/1_9_0/Pages/SuggestedSolutionsChat.html');

        $('.afterSearch').show();

        filter = new top.TeamSupport.Data.TicketLoadFilter();
        filter.IsKnowledgeBase = true;
        $('.dialog-select-ticket2').find('input').data('filter', filter);

        $(".dialog-select-ticket2 input").autocomplete({
            minLength: 2,
            source: selectTicket,
            select: function (event, ui) {
                $(this).data('item', ui.item).removeClass('ui-autocomplete-loading')
                top.Ts.Services.Tickets.GetKBTicketAndActions(ui.item.data, function (result) {
                    var html = '<div>';

                    var actions = result[1];
                    if (actions.length == 0) {
                        html = html + '<h2>The selected ticket has no knowledgebase actions.</h2>';
                    }
                    else {
                        for (var i = 0; i < actions.length; i++) {
                            html = html + '<div>' + actions[i].Description + '</div></br>';
                        }
                    }
                    html = html + '</div>';
                    //clickedItem.find('.previewHtml').attr("value", html);
                    window.frames[0].document.getElementById("TicketPreviewIFrame").contentWindow.writeHtml(html);
                });
            },
            position: {
                my: "right top",
                at: "right bottom",
                collision: "fit flip"
            }
        });

        $('#InsertSuggestedSolutions').click(function (e) {
            e.preventDefault();

            if ($(".dialog-select-ticket2 input").data('item')) {
                callback($(".dialog-select-ticket2 input").data('item').data, true);
                $('#SuggestedSolutionsModal').modal('hide');
                top.Ts.System.logAction('Inserted kb');
            }
            else {
                var id = document.getElementById("SuggestedSolutionsIFrame").contentWindow.GetSelectedID();
                if (id) {
                    callback(id, true);
                    $('#SuggestedSolutionsModal').modal('hide');
                    top.Ts.System.logAction('Inserted suggested solution');
                }
                else {
                    alert('Select a knowledgebase article.');
                }
            }
        });

        $('#InsertSuggestedSolutionsLink').click(function (e) {
            e.preventDefault();

            if ($(".dialog-select-ticket2 input").data('item')) {
                callback($(".dialog-select-ticket2 input").data('item').data, false);
                $('#SuggestedSolutionsModal').modal('hide');
                top.Ts.System.logAction('Inserted kb');
            }
            else {
                var id = document.getElementById("SuggestedSolutionsIFrame").contentWindow.GetSelectedID();
                if (id) {
                    callback(id, false);
                    $('#SuggestedSolutionsModal').modal('hide');
                    top.Ts.System.logAction('Inserted suggested solution');
                }
                else {
                    alert('Select a knowledgebase article.');
                }
            }
        });
    }

    _intervalUpdateActiveChats = setInterval('EnableDisableTicketMenu();', 5200);
});

function EnableDisableTicketMenu() {
    parent.Ts.Services.Chat.GetTicketID(_activeChatID, function (ticketID) {
        if (ticketID && ticketID > 0) {
            if (!$('#Ticket-Create').hasClass("disabled")) {
                $('#Ticket-Create').addClass("disabled");
                clearInterval(_intervalUpdateActiveChats);
            }
        }
    });
}

$(document).bind('dragover', function (e) {
    var dropZone = $('.current-chat'),
        timeout = window.dropZoneTimeout;
    if (!timeout) {
        dropZone.addClass('in');
    } else {
        clearTimeout(timeout);
    }
    var found = false,
        node = e.target;
    do {
        if (node === dropZone[0]) {
            found = true;
            break;
        }
        node = node.parentNode;
    } while (node != null);
    if (found) {
        dropZone.addClass('hover');
    } else {
        dropZone.removeClass('hover');
    }
    window.dropZoneTimeout = setTimeout(function () {
        window.dropZoneTimeout = null;
        dropZone.removeClass('in hover');
    }, 100);
});