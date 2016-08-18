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
                var anchor = $('<a href="#" class="list-group-item chat-request">' + innerString + '</a>').click(function (e) {
                    e.preventDefault();

                    //TODO:  Need to find old name for closing element;
                    $('.open-request').html('Matthew Townsen - TeamSupport').removeClass('open-request')

                    $(this).html('<p>' + user.FirstName + ' ' + user.LastName + '</p>' +
                                     '<p>Email:  ' + user.Email + '</p>' +
                                     '<p>Time:  ' + chat.DateCreated + '</p>' +
                                     '<p>Message:  ' + chat.Message + '</p>' +
                                     '<button class="btn btn-default">Accept</button>')
                                     .addClass('open-request');
                });
                $('#chat-requests').append(anchor);
                if (i == 0) anchor.trigger("click");
            }

        });
    }

    function CreateRequestHTML(request)
    {

    }

});