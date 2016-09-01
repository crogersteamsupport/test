﻿function setupChat(chatID, newCommentCallback, callback) {
    var channelName = 'presence-' + chatID;
    var service = '/Services/ChatService.asmx/';
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', {
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
        console.log('new-comment');
        console.log(data);
        newCommentCallback(data);
    });

    callback(channel);
}