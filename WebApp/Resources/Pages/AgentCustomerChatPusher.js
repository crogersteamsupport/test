function setupChat(pusherKey, chatID, newCommentCallback, callback) {
    var channelName = 'presence-' + chatID;
    var service = '/Services/ChatService.asmx/';
    var pusher = new Pusher(pusherKey, {
        authEndpoint: service + 'AgentAuth', auth: {
            params: {
                chatID: chatID
            }
        }
    });
    var channel = pusher.subscribe(channelName);

    channel.bind('pusher:subscription_succeeded', function () {
        //console.log(channel.members);
    });

    channel.bind('pusher:member_added', function (member) {
        //$('#scopeMessage').remove();
        //createMessage(member.info.name + ' joined the chat.')
    });

    channel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    channel.bind('new-comment', function (data) {
        newCommentCallback(data, true);
    });

    channel.bind('user-typing', function (data) {
        console.log(data);
        $('#typing').text(data).show();
        //alert('yo typing')
    });

    channel.bind('user-stop-typing', function (data) {
        $('#typing').hide();
        //alert('yo NOT typing')
    });


    var typingTimer;                //timer identifier
    var doneTypingInterval = 5000;  //time in ms, 5 second for example
    var $input = $('#message');

    //on keyup, start the countdown
    $input.on('keyup', function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(doneTyping, doneTypingInterval);
    });

    //on keydown, clear the countdown 
    $input.on('keydown', function () {
        clearTimeout(typingTimer);

        parent.Ts.Services.Chat.AgentTyping('presence-' + _activeChatID, true, function (data) {

        });
    });

    //user is "finished typing," do something
    function doneTyping() {
        $('#typing').hide();

        parent.Ts.Services.Chat.AgentTyping('presence-' + _activeChatID, false, function (data) {

        });
    }

    callback(channel);
}

function subscribeToNewChatRequest(pusherKey, newRequestCallback) {
    var chatGUID = top.Ts.System.Organization.ChatID;
    var pusher = new Pusher(pusherKey);
    var request_channel = pusher.subscribe('chat-requests-' + chatGUID);

    request_channel.bind('new-chat-request', function (data) {
        newRequestCallback(data);
    });
}