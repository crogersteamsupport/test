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

function SetupTOK(chatID) {
    $('#chat-tok-screenshare').click(function (e) {
        top.Ts.System.logAction('Ticket - Video Recording Button Clicked');
        if (OT.checkSystemRequirements() == 1) {
            var dynamicPub = $("#publisher");

            $("#videoRecordingContainer").show();
            dynamicPub.show();
            dynamicPub.attr("id", "tempContainer");
            dynamicPub.attr("width", "400px");
            dynamicPub.attr("height", "400px");

            if (dynamicPub.length == 0)
                dynamicPub = $("#tempContainer");

            top.Ts.Services.Chat.GetSessionInfo(function (resultID) {
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
            });

        }
        else {
            alert("Your client does not support video recording.")
        }

    });
}

function startVideoRecording(e) {
    parent.Ts.System.logAction('Chat - Video Recording Start Clicked');
    top.Ts.Services.Chat.StartArchiving(sessionId, function (resultID) {
        $('#startVideoRecording').hide();
        $('#stopVideoRecording').show();
        $('#canceltok').hide();
        archiveID = resultID;
        $('#statusText').text("Currently Recording ...");
    });
};

$scope.stopVideoRecording = function (e) {
    parent.Ts.System.logAction('Ticket - Video Recording Stop Clicked');
    $('#statusText').text("Processing...");
    top.Ts.Services.Chat.StopArchiving(archiveID, function (result) {
        $('#startVideoRecording').show();
        $('#stopVideoRecording').hide();
        $('#insertVideoRecording').show();
        $('#canceltok').show();
        tokurl = result;
        $('#statusText').text("Recording Stopped");
    });
};