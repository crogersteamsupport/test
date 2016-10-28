var isTyping = false;
var channel;
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
    channel = pusher.subscribe(channelName);

    //channel.bind('pusher:subscription_succeeded', function () {
    //    //console.log(channel.members);
    //});

    //channel.bind('pusher:member_added', function (member) {
    //    //$('#scopeMessage').remove();
    //    //createMessage(member.info.name + ' joined the chat.')
    //});

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

    channel.bind('client-tok-screen-user', function (data) {
        var messageTemplate = $("#tok-screen-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to share their screen with you. ')
        $('.media-list').append(compiledTemplate);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });

    channel.bind('client-tok-video-user', function (data) {
        var messageTemplate = $("#tok-video-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have a video call with you.  ')
        $('.media-list').append(compiledTemplate);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });

    channel.bind('client-tok-audio-user', function (data) {
        var messageTemplate = $("#tok-audio-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have a audio call with you. ')
        $('.media-list').append(compiledTemplate);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });


    var typingTimer;
    var doneTypingInterval = 5000;
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
            if (channel !== null)
                var triggered = channel.trigger('client-agent-typing', channel.members.me.info.name + ' is typing...');
        }
    });

    callback(channel);
}

function doneTyping() {
    //$('#typing').hide();
    if (channel !== null)
        var triggered = channel.trigger('client-agent-stop-typing', channel.members.me.info.name + ' is typing...');
    isTyping = false;
}

function subscribeToNewChatRequest(pusherKey, newRequestCallback) {
    var chatGUID = top.Ts.System.Organization.ChatID;
    var pusher = new Pusher(pusherKey);
    var request_channel = pusher.subscribe('chat-requests-' + chatGUID);

    request_channel.bind('new-chat-request', function (data) {
        newRequestCallback(data);
    });
}