var loginService = '/Services/LoginService.asmx/';
var returnURL = '/';

$(document).ready(function () {
  $('#verify').click(function (e) {
    e.preventDefault();
    var code = $('#inputVerificationCode').val();
    var userId = top.Ts.Utils.getQueryValue("UserID", window);
    if (code) {
      var userData = { userId: userId, codeEntered: code };
      IssueAjaxRequest(loginService, "CodeVerification", userData,
      function (result) {
        switch (result.Result) {//Unknown = 0, Success = 1, Fail = 2, VerificationNeeded = 3, VerificationSetupNeeded = 4
          case 1:
            window.location = '/';
            break;
          default:
            $('#pageError').text(result.Error).show();
            break;
        }
      },
      function (error) {
        $('#pageError').text('Verification Failed.  Please try again.').show();
      });
    }
    else {
      $('#pageError').text('Please enter a valid verification code.').show();
    }
  });

  $('#resendCode').click(function (e) {
    e.preventDefault();
    var userId = top.Ts.Utils.getQueryValue("UserID", window);
    var userData = { userId: userId };
    IssueAjaxRequest(loginService, "RegenerateCodeVerification", userData,
    function (result) {
    	$('#codeResent').show();
    },
    function (error) {
      $('#pageError').text('There was a issue resending your verification code.  Please try again.').show();
    });
  })
});

function IssueAjaxRequest(service, method, data, successCallback, errorCallback) {
  $.ajax({
    type: "POST",
    url: service + method,
    data: JSON.stringify(data),
    contentType: "application/json; charset=utf-8",
    dataType: "json",
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
      errorCallback(error);
    }
  });
}