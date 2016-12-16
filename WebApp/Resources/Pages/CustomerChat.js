var _activeChatID = null;
var _participantID = null;
var _timer;
var showAvatars = true;
$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("chatid", window);
    _activeChatID = chatID;
    var participantID = Ts.Utils.getQueryValue("pid", window);
    _participantID = participantID;
    var chatObject;
    var channel;
    var chatInfoObject = {};

    setupChat(chatID, participantID, function (channelObject) {
        channel = channelObject;

        _timer = setTimeout(function () {
            var data = { chatID: chatID }

            IssueAjaxRequest("MissedChat", data,
            function (result) {
                //console.log(result)
                window.location.replace('ChatThankYou.html');
            },
            function (error) {
                console.log(error)
            });
        }, 120000);

        channel.bind('agent-joined', function (data) {
            $('#operator-message').remove();
            clearTimeout(_timer);
        });
        

    });

    GetChatSettings(chatID);
    loadInitialMessages(chatID);
    SetupChatUploads(chatID, participantID);
    SetupTOK();

    $("#message-form").submit(function (e) {
        e.preventDefault();
        if ($('#message').val() !== '') {
            $('#send-message').prop("disabled", true);
            doneTyping();
            var messageData = { channelName: 'presence-' + chatID, message: $('#message').val(), chatID: chatID, userID: participantID };

            IssueAjaxRequest("AddMessage", messageData,
            function (result) {
                $('#message').val('');
                $('#send-message').prop("disabled", false);
            },
            function (error) {

            });
        }
    });

    //TODO:  Not centering correclty
    //$('#chat-tok-audio').tooltip({
    //    container: 'body'
    //});
});

function GetChatSettings(chatID) {
    var chatObject = { chatID: chatID };
    
    IssueAjaxRequest("GetClientChatPropertiesByChatID", chatObject,
    function (result) {
        console.log(result)
        if (!result.TOKScreenEnabled)
            $('#chat-tok-screen').hide();
        if (!result.TOKVideoEnabled)
            $('#chat-tok-video').hide();
        if (!result.TOKVoiceEnabled)
            $('#chat-tok-audio').hide();

            showAvatars = result.ChatAvatarsEnabled
        $('.panel-heading').text(result.ChatIntro);
    },
    function (error) {

    });
}

function createMessage(message)
{
    $('.chat-intro').append('<p>'+ message +'</p>');
}

function createMessageElement(messageData, direction) {
    var userAvatar = '../vcr/1_9_0/images/blank_avatar.png';
    if (messageData.CreatorID !== null) userAvatar = '../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1469829040429';

    $('#chat-body').append('<div class="answer ' + direction + '"> <div class="avatar"> <img src="'+ userAvatar +'" alt="User name">  </div>' +
                        '<div class="name">' + messageData.CreatorDisplayName + '</div>  <div class="text">' + messageData.Message + '</div> <div class="time">' + moment(messageData.DateCreated).format('MM/DD/YYYY hh:mm A') + '</div></div>');
}

var pressenceChannel;
function setupChat(chatID, participantID, callback) {
    var channelName = 'presence-' + chatID;
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', {
        authEndpoint: service + 'Auth', auth: {
            params: {
                chatID: chatID,
                participantID: participantID
            }
        }
    });
    pressenceChannel = pusher.subscribe(channelName);

    pressenceChannel.bind('pusher:subscription_succeeded', function () {
        //console.log(channel.members);
    });

    pressenceChannel.bind('pusher:member_added', function (member) {
        $('#operator-message').remove();
        createMessage(member.info.name + ' joined the chat.')
    });


    pressenceChannel.bind('pusher:subscription_error', function (status) {
        console.log(status);
    });

    pressenceChannel.bind('new-comment', function (data) {
        $('#typing').remove();
        createMessageElement(data, (data.CreatorType == 0) ? 'left' : 'right');
        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
    });

    pressenceChannel.bind('client-tok-screen', function (data) {
        //console.log(data);
        $('#chat-body').append('<div class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to share a screen with you. <a onClick="subscribeToScreenStream()">Do you Accept?</a></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });

    pressenceChannel.bind('client-tok-video', function (data) {
        $('#chat-body').append('<div class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to share video with you. <a onClick="subscribeToVideoStream()">Do you Accept?</a></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });

    pressenceChannel.bind('client-tok-audio', function (data) {
        $('#chat-body').append('<div class="answer left"> <div class="avatar"> <img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name">  </div>' +
                    '<div class="name">' + data.userName + '</div>  <div class="text">' + data.userName + ' wants to share audio with you. <a onClick="subscribeToAudioStream()">Do you Accept?</a></div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');

        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
    });

    pressenceChannel.bind('client-tok-audio-accept', function (data) {
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                //First Checking Condition Works For IE & Firefox
                //Second Checking Condition Works For Chrome
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {

            }
        }, 25);
    });

    pressenceChannel.bind('client-tok-video-accept', function (data) {
        $('#tokStatusText').text(data.userName + ' has joined live session.');
        sharedApiKey = data.apiKey;
        sharedToken = data.token;
        sharedSessionID = data.sessionId;
        var tokenURI = encodeURIComponent(sharedToken);
        tokpopup = window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
        setTimeout(function () {
            if (!tokpopup || tokpopup.outerHeight === 0) {
                //First Checking Condition Works For IE & Firefox
                //Second Checking Condition Works For Chrome
                alert("Popup Blocker is enabled! Please add this site to your exception list.");
            } else {

            }
        }, 25);
    });

    pressenceChannel.bind('client-agent-stop-typing', function (data) {
        //console.log('received: client-agent-stoip-typing');
        $('#typing').remove();
    });

    pressenceChannel.bind('client-agent-typing', function (data) {
        $('#chat-body').append('<div id="typing" class="answer left"> <div class="avatar"><img src="../vcr/1_9_0/images/blank_avatar.png" alt="User name"></div>' +
                    '<div class="name">' + data + '</div>  <div class="text">' + data + '</div> <div class="time">' + moment().format('MM/DD/YYYY hh:mm A') + '</div></div>');
    });

    //pressenceChannel.bind('client-tok-ended', function (data) {
    //    stopTOKStream();
    //    channel.trigger('client-tok-ended', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    //});


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
            var triggered = pressenceChannel.trigger('client-user-typing', pressenceChannel.members.me.info.name + ' is typing...');
        }
    });

    callback(pressenceChannel);
}

function doneTyping() {
    var triggered = pressenceChannel.trigger('client-user-stop-typing', pressenceChannel.members.me.info.name + ' is typing...');
    isTyping = false;
}

function loadInitialMessages(chatID) {
    var chatObject = { chatID: chatID };

    IssueAjaxRequest("GetChatInfo", chatObject,
    function (result) {
        chatInfoObject = result;
        createMessage('Initiated On: ' + moment(result.DateCreated).format('MM/DD/YYYY hh:mm A'));
        createMessage('Initiated By: ' + result.InitiatorDisplayName);
    },
    function (error) {

    });
}

var service = '../Services/ChatService.asmx/';
function IssueAjaxRequest(method, data, successCallback, errorCallback) {
    $.ajax({
        type: "POST",
        url: service + method,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        dataFilter: function (data) {
            var jsonResult = eval('(' + data + ')');
            if (jsonResult.hasOwnProperty('d'))
                return jsonResult.d;
            else
                return jsonResult;
        },
        success: function (jsonResult) {
            successCallback(jsonResult);
        },
        error: function (error, errorStatus, errorThrown) {
            if (errorCallback) errorCallback(error);
        }
    });
}

function SetupChatUploads(chatID, participantID) {
    var uploadPath = '../../../Upload/ChatAttachments/';
    $('#hiddenAttachmentInput').fileupload({
        dropZone: $('.panel-default'),
        add: function (e, data) {
            data.url = uploadPath + chatID;
            var jqXHR = data.submit()
                .success(function (result, textStatus, jqXHR) {
                    var attachment = JSON.parse(result)[0];

                    var messageData = { channelName: 'presence-' + chatID, chatID: chatID, attachmentID: attachment.id, userID: participantID };

                    IssueAjaxRequest("AddUserUploadMessage", messageData,
                    function (result) {

                    },
                    function (error) {

                    });
                })
                .error(function (jqXHR, textStatus, errorThrown) { console.log(textStatus) })
        },
    });

    $('#chat-attachment').click(function (e) {
        e.preventDefault();
        $('#hiddenAttachmentInput').click();
    });
}

$(document).bind('dragover', function (e) {
    var dropZone = $('.panel-default'),
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