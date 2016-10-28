$(document).ready(function () {
    var sessionID = Ts.Utils.getQueryValue("sessionid", window);
    var token = Ts.Utils.getQueryValue("token", window);
    debugger


    //IssueAjaxRequest("GetTOKSessionInfoClient",
    //function (result) {

    //},
    //function (error) {

    //});

    var data = { chatID: 1 };
    IssueAjaxRequest("GetTOKSessionInfoClient", data,
    function (resultID) {
        //token = resultID[1];
        apiKey = resultID[2];

        var dynamicPub = $("#screenStream");
        dynamicPub.show();
        dynamicPub.attr("id", "tempContainer");
        dynamicPub.attr("width", "400px");
        dynamicPub.attr("height", "400px");

        if (dynamicPub.length == 0)
            dynamicPub = $("#tempContainer");

        var stream = OT.initSession(apiKey, sessionID);
        stream.connect(token, function (error) {
            stream.on('streamCreated', function (event) {
                stream.subscribe(event.stream, dynamicPub.attr('id'), {
                    insertMode: 'append',
                    width: '100%',
                    height: '100%'
                });
            });
            stream.on('streamDestroyed', function (event) {
                window.close();
            });
        });
    });
});

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
