$(document).ready(function () {
    //apiKey = "45228242";

    SetupChatRequests();

    $('.page-loading').hide().next().show();

    function SetupChatRequests() {
        parent.Ts.Services.Chat.GetChats(function (data) {
            //console.log(data);
            for (i = 0; i < data.length; i++)
            {
                var chat = data[i].Chat;
                var user = data[i].Initiator;
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
                if (i == 0) anchor.trigger("click");
            }

        });
    }

    function CloseRequestAnchor() {
        $('.open-request').html($('.open-request > .userName').text()).removeClass('open-request');
    }

    function AcceptRequest(ChatRequestID, innerString, parentEl)  {
        parent.Ts.Services.Chat.AcceptRequest(ChatRequestID, function (chatId) {
            parentEl.remove();
            MoveAcceptedRequest(innerString, chatId);
        });
    }

    function MoveAcceptedRequest(innerString, chatID) {
        var anchor = $('<a href="#" class="list-group-item">' + innerString + '</a>');
        $('#chats-accepted').append(anchor);
    }

});