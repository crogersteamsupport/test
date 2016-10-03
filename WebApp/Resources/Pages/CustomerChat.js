$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("chatid", window);
    var participantID = Ts.Utils.getQueryValue("pid", window);
    var chatObject;
    var channel;
    var chatInfoObject = {};

    setupChat(chatID, participantID, function (channelObject) {
        channel = channelObject;
    });
    loadInitialMessages(chatID);
    SetupChatUploads(chatID, participantID);

    $("#message-form").submit(function (e) {
        e.preventDefault();
        console.log(channel.members.me);
        var messageData = { channelName: 'presence-' + chatID, message: $('#message').val(), chatID: chatID, userID: participantID };
        
        IssueAjaxRequest("AddMessage", messageData,
        function (result) {
            $('#message').val('');
        },
        function (error) {

        });

    });


  
});

function createMessage(message)
{
    $('.chat-intro').append('<p>'+ message +'</p>');
}

function createMessageElement(messageData, direction) {
    var userAvatar = '../vcr/1_9_0/images/blank_avatar.png';
    if (messageData.CreatorID !== null) userAvatar = '../dc/' + chatInfoObject.OrganizationID + '/UserAvatar/' + messageData.CreatorID + '/48/1469829040429';

    $('#chat-body').append('<div class="answer ' + direction + '"> <div class="avatar"> <img src="'+ userAvatar +'" alt="User name">  </div>' +
                        '<div class="name">' + messageData.CreatorDisplayName + '</div>  <div class="text">' + messageData.Message + '</div> <div class="time">' + moment(messageData.DateCreated).format('DD/MM/YYYY hh:mm A') + '</div></div>');
}

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
    var pressenceChannel = pusher.subscribe(channelName);

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

    pressenceChannel.bind('agent-joined', function (data) {
        $('#operator-message').remove();
    });

    pressenceChannel.bind('new-comment', function (data) {
        //console.log('new-comment-user');
        //console.log(data)
        createMessageElement(data, (data.CreatorType == 0) ? 'left' : 'right');
        $(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
    });

    callback(pressenceChannel);
}

function loadInitialMessages(chatID) {
    var chatObject = { chatID: chatID };

    IssueAjaxRequest("GetChatInfo", chatObject,
    function (result) {
        chatInfoObject = result;
        createMessage('Initiated On: ' + moment(result.DateCreated).format('DD/MM/YYYY hh:mm A'));
        createMessage('Initiated By: ' + result.InitiatorDisplayName);

        //for (i = 0; i < result.Messages.length; i++) {
        //    console.log(result.Messages[i])
        //    createMessageElement(result.Messages[i], (result.Messages[i].CreatorType == 0) ? 'left' : 'right');
        //}

        //$(".panel-body").animate({ scrollTop: $('.panel-body').prop("scrollHeight") }, 1000);
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