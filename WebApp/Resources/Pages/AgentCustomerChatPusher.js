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

    var typeTemplate;
    channel.bind('client-user-typing', function (data) {
        var messageTemplate = $("#typing-template").html();
        typeTemplate = messageTemplate
                                .replace('{{MessageDirection}}', 'left')
                                .replace('{{UserName}}', data.userName)
                                .replace('{{Avatar}}', '../images/blank_avatar.png')
                                .replace('{{message}}', data.userName + ' is typing...')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'));

        $('.media-list').append(typeTemplate);
        ScrollMessages(true);
    });

    channel.bind('client-user-stop-typing', function (data) {
        $('#typing').remove();
    });

    channel.bind('client-tok-screen-user', function (data) {
        var messageTemplate = $("#tok-screen-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to share their screen with you. ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        ScrollMessages(true);
    });

    channel.bind('client-tok-video-user', function (data) {
        var messageTemplate = $("#tok-video-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have a video call with you.  ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        ScrollMessages(true);
    });

    channel.bind('client-tok-audio-user', function (data) {
        var messageTemplate = $("#tok-audio-template").html();
        var compiledTemplate = messageTemplate
                                .replace('{{message}}', data.userName + ' wants to have a audio call with you. ')
                                .replace('{{Date}}', moment().format(dateFormat + ' hh:mm A'))
                                .replace('{{UserName}}', data.userName);
        $('.media-list').append(compiledTemplate);

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        ScrollMessages(true);
    });

    channel.bind('client-tok-audio-user-accept', function (data) {
        debugger
        //console.log(data);
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open('https://release-chat.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
        
        //Account for popup blockers
        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                //First Checking Condition Works For IE & Firefox
                //Second Checking Condition Works For Chrome
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {

            }
        }, 25);
    });

    channel.bind('client-tok-video-user-accept', function (data) {
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open('https://release-chat.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');

        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                //First Checking Condition Works For IE & Firefox
                //Second Checking Condition Works For Chrome
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {

            }
        }, 25);
    });


    //channel.bind('client-tok-ended', function (data) {
    //    stopTOKStream();
    //});


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

function ScrollMessages(animated) {
    if (animated)
        $(".current-chat-area").animate({ scrollTop: $('.current-chat-area').prop("scrollHeight") }, 1000);
    else
        $(".current-chat-area").scrollTop($('.current-chat-area').prop("scrollHeight"));
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

    request_channel.bind('chat-request-accepted', function (data) {
        $('#chats-requests > #' + data).remove();
    });
}