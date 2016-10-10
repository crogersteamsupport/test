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

function SetupTOK() {
    $('#chat-tok-video').click(function (e) {
        if (OT.checkSystemRequirements() == 1) {
            var dynamicPub = $("#publisher");

            $("#videoRecordingContainer").show();
            dynamicPub.show();
            dynamicPub.attr("id", "tempContainer");
            dynamicPub.attr("width", "400px");
            dynamicPub.attr("height", "400px");

            if (dynamicPub.length == 0)
                dynamicPub = $("#tempContainer");

            var data = { chatID: _activeChatID };
            IssueAjaxRequest("GetTOKSessionInfoClient", data,
            function (resultID) {
                $('.panel-body').height('calc(50vh - 95px)');
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
            },
            function (error) {
                consol.log(error.message)
            });
        }
        else {
            alert("Your client does not support video recording.")
        }

    });

    $('#chat-tok-screen').click(function (e) {
        $('.panel-body').height('calc(95vh - 95px)');
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
                    IssueAjaxRequest("GetTOKSessionInfo",
                    function (resultID) {
                        sessionId = resultID[0];
                        token = resultID[1];
                        apiKey = resultID[2];
                        session = OT.initSession(apiKey, sessionId);
                        var pubOptions = { publishAudio: true, publishVideo: false };
                        publisher = OT.initPublisher('screenTwo', pubOptions);

                        session.connect(token, function (error) {
                            session.publish(publisher);
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

    });
}

function startVideoRecording(e) {
    var data = { sessionId: sessionId };
    IssueAjaxRequest("StartArchivingClient", data,
    function (resultID) {
        $('#startVideoRecording').hide();
        $('#stopVideoRecording').show();
        $('#canceltok').hide();
        archiveID = resultID;
        $('#statusText').text("Currently Recording ...");
    },
    function (error) {
        console.log(error.message);
    });
};

function stopVideoRecording(e) {
    var data = { archiveId: archiveID };
    IssueAjaxRequest("StopArchivingClient", data,
    function (result) {
        $('#startVideoRecording').show();
        $('#stopVideoRecording').hide();
        $('#insertVideoRecording').show();
        $('#canceltok').show();
        tokurl = result;
        $('#statusText').text("Recording Stopped");
    },
    function (error) {
        console.log(error.message);
    });
    $('#statusText').text("Processing...");
};

function insertVideoRecording(e) {
    var videoHTML = '<video width="400" height="400" controls poster="https://app.teamsupport.com/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
    var messageData = { channelName: 'presence-' + _activeChatID, message: videoHTML, chatID: _activeChatID, userID: _participantID };

    IssueAjaxRequest("AddMessage", messageData,
    function (result) {
        $('#message').val('');
    },
    function (error) {

    });

    session.unpublish(publisher);
    $('#startVideoRecording').show();
    $('#stopVideoRecording').hide();
    $('#insertVideoRecording').hide();
    $('#videoRecordingContainer').hide();
    $('#statusText').text("");
    $('.panel-body').height('calc(100vh - 95px)');
};

function cancelVideoRecording(e) {
    $('.panel-body').height('calc(100vh - 95px)');
    if (archiveID) {
        $('#statusText').text("Cancelling Recording ...");
        var data = { archiveID: archiveID };
        IssueAjaxRequest("DeleteArchiveClient", data,
        function (resultID) {
            $('#startVideoRecording').show();
            $('#stopVideoRecording').hide();
            $('#insertVideoRecording').hide();
            session.unpublish(publisher);
            $('#videoRecordingContainer').hide();
            $('#statusText').text("");
        },
        function (error) {

        });
    }
    else {
        session.unpublish(publisher);
        $('#videoRecordingContainer').hide();
    }
    $('#statusText').text("");
};

function muteVideoRecording(e) {
    publisher.publishAudio(false);
    $('#unmuteVideoRecording').show();
    $('#muteVideoRecording').hide();
};

function unmuteVideoRecording(e) {
    publisher.publishAudio(true);
    $('#muteVideoRecording').show();
    $('#unmuteVideoRecording').hide();
};

function startScreenRecording(e) {
    var data = { sessionId: sessionId };
    IssueAjaxRequest("StartArchivingClient", data,
    function (resultID) {
        $('#startScreenRecording').hide();
        $('#stopScreenRecording').show();
        $('#cancelScreenRecording').hide();
        $('#muteScreenRecording').show();
        archiveID = resultID;
        $('#tokScreenRecordingCountdown').show();
        setTimeout(function () {
            updateTimer($('#tokScreenRecordingCountdown'));
        }, 1000);

        $('#screenRecordingStatusText').text("Currently Recording Screen...");
    },
    function (error) {
        console.log(error.message);
    });
};

function updateTimer(parentElement) {
    var myTime = $('#tokScreenRecordingCountdown').html();
    var ss = myTime.split(":");
    var dt = new Date();
    dt.setHours(0);
    dt.setMinutes(ss[0]);
    dt.setSeconds(ss[1]);

    var dt2 = new Date(dt.valueOf() + 1000);
    var temp = dt2.toTimeString().split(" ");
    var ts = temp[0].split(":");

    if (temp[0] == "05") {
        stopScreenRecording(parentElement);
        return;
    }

    $("#tokScreenRecordingCountdown").html(ts[1] + ":" + ts[2]);
    tokTimer = setTimeout(function () {
        updateTimer(parentElement);
    }, 1000);
}

function stopScreenRecording(e) {
    $('#screenRecordingStatusText').text("Processing...");

    var data = { archiveID: archiveID };
    IssueAjaxRequest("StopArchiving", data,
    function (resultID) {
        $('#startScreenRecording').show();
        $('#stopScreenRecording').hide();
        $('#insertScreenRecording').show();
        $('#cancelScreenRecording').show();
        tokurl = result;
        $('#screenRecordingStatusText').text("Recording Stopped");
        clearTimeout(tokTimer);
        $("#tokScreenRecordingCountdown").html("0:00");
        $("#tokScreenRecordingCountdown").hide();
    },
    function (error) {
        console.log(error.message);
    });
};

function insertScreenRecording (e) {
    parent.Ts.System.logAction('Chat - Screen Recording Insert Clicked');
    $('.panel-body').height('calc(100vh - 95px)');
    var videoHTML = '<video width="400" height="400" controls poster="' + window.parent.Ts.System.AppDomain + '/dc/1078/images/static/videoview1.jpg"><source src="' + tokurl + '" type="video/mp4"><a href="' + tokurl + '">Please click here to view the video.</a></video>';
    submitMessage(videoHTML);

    session.unpublish(publisher);

    $('#startScreenRecording').show();
    $('#stopScreenRecording').hide();
    $('#insertScreenRecording').hide();
    $('#screenRecordingContainer').hide();
    $('#screenRecordingStatusText').text("");
};

function cancelScreenRecording(e) {
    $('.panel-body').height('calc(100vh - 95px)');
    if (archiveID) {
        $('#screenRecordingStatusText').text("Canceling Recording ...");
        var data = { archiveID: archiveID };
        IssueAjaxRequest("DeleteArchive", data,
        function (resultID) {
                $("#tokScreenRecordingCountdown").html("0:00");
                $("#tokScreenRecordingCountdown").hide();

                $('#startScreenRecording').show();
                $('#stopScreenRecording').hide();
                $('#insertScreenRecording').hide();
                session.unpublish(publisher);
                $('#screenRecordingContainer').hide();
                $('#screenRecordingStatusText').text("");
        },
        function (error) {
            console.log(error.message);
        });
    }
    else {
        session.unpublish(publisher);
        $('#screenRecordingContainer').hide();
    }
    $('#screenRecordingStatusText').text("");
};

function muteScreenRecording(e) {
    publisher.publishAudio(false);
    $('#unmuteScreenRecording').show();
    $('#muteScreenRecording').hide();
};

function unmuteScreenRecording(e) {
    publisher.publishAudio(true);
    $('#muteScreenRecording').show();
    $('#unmuteScreenRecording').hide();
};

function installChromePlugin()
{
    chrome.webstore.install("https://chrome.google.com/webstore/detail/laehkaldepkacogpkokmimggbepafabg",
function () { }, function (e) { console.log(e) });
}