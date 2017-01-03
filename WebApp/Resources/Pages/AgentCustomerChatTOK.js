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
var tokpopup;

function SetupTOK() {
    if (BrowserDetect.browser == "Explorer" || BrowserDetect.browser == "Safari") {
        $('#chat-tok-video').hide();
        $('#chat-tok-audio').hide();
        $('#chat-tok-screen').hide();
    }

    $('#chat-tok-video').click(function (e) {
        top.Ts.System.logAction('Chat - Video Call Button Clicked');
        publishTOKVideo(function () {
            startVideoStreaming();
        });
    });

    $('#chat-tok-audio').click(function (e) {
        top.Ts.System.logAction('Chat -Audio Call Button Clicked');
        publishTOKAudio(function () {
            startAudioStreaming();
        });
    });

    $('#chat-tok-screen').click(function (e) {
        top.Ts.System.logAction('Chat - Share Screen Button Clicked');
        publishTOKScreen();
    });
}


function publishTOKVideo(callback) {
    //if (session !== undefined || publisher !== undefined)
    //    stopTOKStream();

    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.current-chat-area').height('calc(100vh - 225px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');

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

            session.on("connectionDestroyed", function (event) {
                stopTOKStream(event);
            });

            callback();
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKAudio(callback) {
    //if (session !== undefined || publisher !== undefined)
    //    stopTOKStream();

    if (OT.checkSystemRequirements() == 1) {
        var dynamicPub = $("#publisher");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        $('.current-chat-area').height('calc(100vh - 225px)');
        $('#tokStreamControls').show();
        $('#tokStatusText').text('Requesting Live Session...');

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

            session.on("connectionDestroyed", function (event) {
                debugger
                stopTOKStream(event);
            });

            callback();
        });
    }
    else {
        alert("Your client does not support video recording.")
    }
}

function publishTOKScreen() {
    //if (session !== undefined || publisher !== undefined)
    //    stopTOKStream();

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
            $('.current-chat-area').height('calc(100vh - 225px)');
            $('#tokStreamControls').show();
            $('#tokStatusText').text('Sharing Screen.');
            top.Ts.Services.Chat.GetTOKSessionInfo(function (resultID) {
                sessionId = resultID[0];
                token = resultID[1];
                apiKey = resultID[2];
                session = OT.initSession(apiKey, sessionId);
                var pubOptions = { publishAudio: true, publishVideo: false };
                publisher = OT.initPublisher('screenTwo', pubOptions);

                session.connect(token, function (error) {
                    session.publish(publisher);
                    startScreenStreaming();
                    muteTOKStream();
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

                session.on("connectionDestroyed", function (event) {
                    stopTOKStream(event);
                });
            });
        }
    });
}

function subscribeToVideoStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var tokenURI = encodeURIComponent(sharedToken);
    tokpopup = window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');
    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {
            publishTOKVideo(function () {
                channel.trigger('client-tok-video-accept', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
            });
        }
    }, 25);

}

function subscribeToAudioStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var tokenURI = encodeURIComponent(sharedToken);
    tokpopup = window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');

    //Account for popup blockers
    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {
            publishTOKAudio(function () {
                channel.trigger('client-tok-audio-accept', { userName: channel.members.me.info.name, apiKey: apiKey, token: token, sessionId: sessionId });
            });
        }
    }, 25);
}

function subscribeToScreenStream() {
    if (session !== undefined || publisher !== undefined)
        stopTOKStream();

    var tokenURI = encodeURIComponent(sharedToken);
    tokpopup = window.open('https://chat.alpha.teamsupport.com/screenshare/TOKSharedSession.html?sessionid=' + sharedSessionID + '&token=' + tokenURI, 'TSTOKSharedSession', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=1250,height=1000');

    //Account for popup blockers
    setTimeout(function () {
        if (!tokpopup || tokpopup.outerHeight === 0) {
            //First Checking Condition Works For IE & Firefox
            //Second Checking Condition Works For Chrome
            alert("Popup Blocker is enabled! Please add this site to your exception list.");
        } else {

        }
    }, 25);
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

function stopTOKStream(e) {
    debugger
    $('#tokStatusText').text('Ending live session...');
    session.unpublish(publisher);
    session.disconnect();
    publisher.destroy();
    session = undefined;
    publisher = undefined;

    $('#tokStreamControls').hide();
    $('.current-chat-area').height('height: calc(100vh - 155px);');

    if(tokpopup)
        tokpopup.close();
};

function muteTOKStream() {
    publisher.publishAudio(false);
    $('#unmuteStream').show();
    $('#muteStream').hide();
};

function unmuteTOKStream() {
    publisher.publishAudio(true);
    $('#muteStream').show();
    $('#unmuteStream').hide();
};

function installChromePlugin() {
    chrome.webstore.install("https://chrome.google.com/webstore/detail/laehkaldepkacogpkokmimggbepafabg",
    function () { }, function (e) { console.log(e) });
}