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

    channel.bind('pusher:member_removed', function (member) {
        parent.Ts.Services.Chat.RemoveUser('presence-' + _activeChatID, _activeChatID, member.id, function (success) { });
    });

    channel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    channel.bind('new-comment', function (data) {
        newCommentCallback(data, true);
    });

    channel.bind('client-user-typing', function (data) {
        console.log(data);
        $('#typing').text(data).show();
        //alert('yo typing')
    });

    channel.bind('client-user-stop-typing', function (data) {
        $('#typing').hide();
        //alert('yo NOT typing')
    });


    var isTyping = false;
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
        if (!isTyping) {
            isTyping = true;
            clearTimeout(typingTimer);
            var triggered = channel.trigger('client-agent-typing', channel.members.me.info.name + ' is typing...');
        }
    });

    //user is "finished typing," do something
    function doneTyping() {
        $('#typing').hide();
        var triggered = channel.trigger('client-agent-stop-typing', channel.members.me.info.name + ' is typing...');
        isTyping = false;
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