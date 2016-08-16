$(document).ready(function () {
    //apiKey = "45228242";

    SetupChatRequests();

    $('.page-loading').hide().next().show();

    $(".chat-request").click(function (e) {
        e.preventDefault();
        $('.open-request').html('Matthew Townsen - TeamSupport').removeClass('open-request')
        $(this).html('<p>Name:  Matthew Townsen</p>' +
                         '<p>Email:  mtownsen@teamsupport.com</p>' +
                         '<p>Time:  2:47 PM</p>' +
                         '<p>Message:  Its all broken</p>' +
                         '<button class="btn btn-default">Accept</button>')
                         .addClass('open-request');
    });

    function SetupChatRequests() {
        parent.Ts.Services.Chat.GetChats(function (data) {
            console.log(data);
        });
    }

});