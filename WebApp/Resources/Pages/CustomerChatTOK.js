var session;
var token;
var recordingID;
var apiKey;
var sessionId;
var tokurl;
var publisher;
var screenSharingPublisher;
var videoURL;
var tokTimer;
var archiveID;
var stream;
var sharedSessionID;
var sharedApiKey;
var sharedToken;

function SetupTOK() {
    $('#chat-tok-video').click(function (e) {
        publishTOKVideo(function () {
            startVideoStreaming();
        });
    });

    $('#chat-tok-audio').click(function (e) {
        publishTOKAudio(function () {
            startAudioStreaming();
        });
    });

    $('#chat-tok-screen').click(function (e) {
        publishTOKScreen();
    });
}

function publishTOKVideo(callback) {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.panel-body').height('calc(100vh - 172px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');

        var data = { chatID: _activeChatID };
        IssueAjaxRequest("GetTOKSessionInfoClient", data,
        function (resultID) {
            sessionId = resultID[0];
            token = resultID[1];
            apiKey = resultID[2];
            session = OT.initSession(apiKey, sessionId);
            session.connect(token, function (error) {
                publisher = OT.initPublisher(dynamicPub.attr('id'), {
                    insertMode: 'append',
                    width: '100%',
                    height: '100%'
                });
                session.publish(publisher);
            });
            callback();
        },
        function (error) {
            consol.log(error.message)
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKAudio(callback) {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.panel-body').height('calc(100vh - 172px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');

        var data = { chatID: _activeChatID };
        IssueAjaxRequest("GetTOKSessionInfoClient", data,
        function (resultID) {
            sessionId = resultID[0];
            token = resultID[1];
            apiKey = resultID[2];
            session = OT.initSession(apiKey, sessionId);
            session.connect(token, function (error) {
                publisher = OT.initPublisher(dynamicPub.attr('id'), {
                    videoSource: null,
                    insertMode: 'append',
                    width: '100%',
                    height: '100%'
                });
                session.publish(publisher);
            });
            //startAudioStreaming();
            callback();
        },
        function (error) {
            consol.log(error.message)
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKScreen() {
    var dynamicPub = $("#screenShare");
    $("#screenRecordingContainer").show();
    dynamicPub.show();

    var clonedScreen = $('#screenOne').clone();
    clonedScreen.attr("id", "screenOneClone");
    var clonedVid = $('#screenTwo').clone();
    clonedVid.attr("id", "screenOneClone");


    OT.registerScreenSharingExtension('chrome', 'laehkaldepkacogpkokmimggbepafabg', 2);

    OT.checkScreenSharingCapability(function (response) {
        if (!response.supported || response.extensionRegistered === false) {
            alert("This browser does not support screen sharing");
        } else if (response.extensionInstalled === false && BrowserDetect.browser != "Mozilla") {
            // prompt to install the response.extensionRequired extension

            if (BrowserDetect.browser == "Chrome") {
                $('#ChromeInstallModal').modal('show');
            }
            else if (BrowserDetect.browser == "Firefox") {
                $('#FireFoxInstallModal').modal('show');
            }

            $('#screenRecordingContainer').hide();
        }
        else {
            $('.panel-body').height('calc(100vh - 172px)');
            $('#tokStreamControls').show();
            $('#tokStatusText').text('Sharing Screen.');
            var data = { chatID: _activeChatID };
            IssueAjaxRequest("GetTOKSessionInfoClient", data,
            function (resultID) {
                sessionId = resultID[0];
                token = resultID[1];
                apiKey = resultID[2];
                session = OT.initSession(apiKey, sessionId);
                var pubOptions = { publishAudio: false, publishVideo: false };
                publisher = OT.initPublisher('screenTwo', pubOptions);

                session.connect(token, function (error) {
                    session.publish(publisher);
                    startScreenStreaming();
                });

                // Screen sharing is available. Publish the screen.
                // Create an element, but do not display it in the HTML DOM:
                var screenContainerElement = document.createElement('div');
                screenSharingPublisher = OT.initPublisher(
                      'screenOne',
                      { videoSource: 'screen' },
                      function (error) {
                          if (error) {
                              console.log(error);
                          } else {
                              session.publish(
                                screenSharingPublisher,
                                function (error) {
                                    if (error) {
                                        alert('Screen Recording will not start because, ' + error.message);
                                    }
                                });
                          }
                      });
            },
            function (error) {

            });
        }
    });
}

function subscribeToVideoStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
    publishTOKVideo(function () {
        pressenceChannel.trigger('client-tok-video-user-accept', { userName: pressenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    });
}

function subscribeToAudioStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
    publishTOKAudio(function () {
        pressenceChannel.trigger('client-tok-audio-user-accept', { userName: pressenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
    });
}

function subscribeToScreenStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
}

function startVideoStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    pressenceChannel.trigger('client-tok-video-user', { userName: pressenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function startAudioStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    pressenceChannel.trigger('client-tok-audio-user', { userName: pressenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function startScreenStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    pressenceChannel.trigger('client-tok-screen-user', { userName: pressenceChannel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function stopVideoAudioStreaming() {
    session.unpublish(publisher);
}

function stopScreenStreaming() {
    session.unpublish(screenSharingPublisher);
}

function stopTOKStream(e) {
    $('#tokStatusText').text('Ending live session...');
    session.unpublish(publisher);
    $('#tokStreamControls').hide();
    $('.panel-body').height('calc(100vh - 95px)');
};

function muteTOKStream(e) {
    publisher.publishAudio(false);
    $('#unmuteStream').show();
    $('#muteStream').hide();
};

function unmuteTOKStream(e) {
    publisher.publishAudio(true);
    $('#muteStream').show();
    $('#unmuteStream').hide();
};