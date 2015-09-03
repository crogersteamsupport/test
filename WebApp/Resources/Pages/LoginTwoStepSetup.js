var loginService = '/Services/LoginService.asmx/';
var returnURL = '/';

$(document).ready(function () {
  $('#update').click(function (e) {
    e.preventDefault();
    var phoneNumb = $('#twoStepNumber').val();
    var userId = top.Ts.Utils.getQueryValue("UserID", window);
    if (phoneNumb) {
      var userData = { userId: userId, phoneNumber: phoneNumb };
      IssueAjaxRequest(loginService, "SetupVerificationPhoneNumber", userData,
      function (result) {
        window.location = '/';
      },
      function (error) {
        $('#pageError').text('There was a issue updating your profile.  Please try again.').show();
      });
    }
    else {
      $('#pageError').text('Please enter a valid phone number.').show();
    }
  });
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