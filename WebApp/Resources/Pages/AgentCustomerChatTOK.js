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

var sharedSessionID;
var sharedApiKey;
var sharedToken;

function SetupTOK() {
    $('#chat-tok-video').click(function (e) {
        top.Ts.System.logAction('Chat - Video Call Button Clicked');
        publishTOKVideo();
    });

    $('#chat-tok-audio').click(function (e) {
        top.Ts.System.logAction('Chat -Audio Call Button Clicked');
        publishTOKAudio();
    });

    $('#chat-tok-screen').click(function (e) {
        top.Ts.System.logAction('Chat - Share Screen Button Clicked');
        publishTOKScreen();
    });
}


function publishTOKVideo() {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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
            startVideoStreaming();
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKAudio() {
    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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
            startAudioStreaming();
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
            top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
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

                //// Screen sharing is available. Publish the screen.
                //// Create an element, but do not display it in the HTML DOM:
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
            });
        }
    });
}

function subscribeToVideoStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=500,height=500');
    publichTOKVideo();
}

function subscribeToAudioStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=500,height=500');
    publichTOKAudio();
}

function subscribeToScreenStream() {
    var tokenURI = encodeURIComponent(sharedToken);
    window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=500,height=1000');
}

function startVideoStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    channel.trigger('client-tok-video', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function startAudioStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    channel.trigger('client-tok-audio', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function startScreenStreaming() {
    //Send a signal over Pusher to any parties to notify of screen sharing stream.
    channel.trigger('client-tok-screen', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
};

function installChromePlugin() {
    chrome.webstore.install("https://chrome.google.com/webstore/detail/laehkaldepkacogpkokmimggbepafabg",
    function () { }, function (e) { console.log(e) });
}