var loginService = '/Services/LoginService.asmx/';
var returnURL = '/';
var resourcesURL = '/vcr/1_9_0/Pages/';

$(document).ready(function () {
  $("#mobile-number").intlTelInput({
    defaultCountry: "auto",
    geoIpLookup: function (callback) {
      $.get('http://ipinfo.io', function () { }, "jsonp").always(function (resp) {
        var countryCode = (resp && resp.country) ? resp.country : "";
        callback(countryCode);
      });
    },
    utilsScript: "../../Resources/js/utils.js" // just for formatting/placeholders etc
  });

  $('#update').click(function (e) {
    e.preventDefault();
    //var phoneNumb = $('#twoStepNumber').val();
    var phoneNumb = $("#mobile-number").intlTelInput("getNumber");
    var userId = top.Ts.Utils.getQueryValue("UserID", window);
    if (phoneNumb) {
      var userData = { userId: userId, phoneNumber: phoneNumb };
      IssueAjaxRequest(loginService, "SetupVerificationPhoneNumber", userData,
      function (result) {
        window.location = resourcesURL + 'LoginTwoStep.html?UserID=' + userId;
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