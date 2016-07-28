$(document).ready(function () {
    var chatID = Ts.Utils.getQueryValue("uid", window);

    var chatGuid = { chatGuid: chatID };
    IssueAjaxRequest("CheckChatStatus", { chatGuid: chatID },
    function (result) {
        //console.log(result)
        if (result) $('.chatRequestForm').show();
        else {
            $('.chatRequestLabel').show();
            $('.chatRequestForm').show();
        }
    },
    function (error) {
        console.log(error)
    });


    $("#newChatForm").submit(function (e) {
        e.preventDefault();

        var contactInfo = { chatGuid: chatID, fName: $('#userFirstName').val(), lName: $('#userLastName').val(), email: $('#userEmail').val(), description: $('#userIssue').val() };

        IssueAjaxRequest("RequestChat", contactInfo,
        function (result) {
            console.log(result)
            window.location.replace('Chat.html?chatid=' + result.ChatID + '&pid=' + result.RequestorID);
        },
        function (error) { 
            console.log(error)
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
