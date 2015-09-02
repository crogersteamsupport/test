var loginService = '/Services/LoginService.asmx/';
var resourcesURL = '/vcr/1_9_0/Pages/';
var returnURL = '/';

$(document).ready(function () {
  returnURL = top.Ts.Utils.getQueryValue("ReturnUrl", window);
  getRememberMe();
  $('#signIn').click(function (e) {
    e.preventDefault();
    var email = $('#inputEmail').val();
    var org = $('#orgSelect').val();
    if (org == "") org = null;
    var signInData = { email: email, password: $('#inputPassword').val(), organizationId: org, verificationRequired: false };

    IssueAjaxRequest(loginService, "SignIn", signInData,
    function (result) {
      switch (result.Result) {//Unknown = 0, Success = 1, Fail = 2, VerificationNeeded = 3, VerificationSetupNeeded = 4
        case 1:
          window.location = returnURL;
          break;
        case 3:
          window.location = resourcesURL + 'LoginTwoStep.html/?UserID=' + result.UserId;
          break;
        case 4:
          window.location = resourcesURL + 'LoginTwoStepSetup.html/?UserID=' + result.UserId;
          break;
        default:
          $('#loginError').text(result.Error).show();
          break;
      }
    },
    function (error) {
      alert('There was a error signing you in.  Please try again. ')
    });
  });

  $('#inputEmail').change(function (e) {
    CheckEmailForOrgs($(this).val())
  });

  $('#createAccount').click(function (e) {
    //redirect to signup form
  });

function CheckEmailForOrgs(email) { 
  //Code to lookup the email address entered and check for a valid user with multiple orgs. 
  var emailLookupData = { email: email }
  IssueAjaxRequest(loginService, "GetCompanies", emailLookupData,
    function (result) {
      //success
      LoadCompanies(result);
    },
    function (error) {
        
    });
  }
});

function LoadCompanies(companies) {
  var companySelect = $('#orgSelect');
  companySelect.empty();

  if (companies && companies.length > 0) {
    for (var i = 0; i < companies.length; i++) {
      $('<option>').attr('value', companies[i].ID).text(companies[i].Label).appendTo(companySelect);
    }
    companySelect.show();
  }
  else companySelect.hide();
};

function getRememberMe() {
  var cookie = Ts.Utils.getCookie('rememberme', 'sessionid');
  if (cookie != null && cookie.length > 0) {
    var RememberMeData = { userID: cookie };
    IssueAjaxRequest(loginService, "GetEmail", RememberMeData,
    function (result) {
      $('#inputEmail').val(result);
      $('#rememberMe').attr('checked', 'checked')
    },
    function (error) {

    });
  }
}

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
      //alert(jsonResult.UserId + ' ' + jsonResult.OrganizationId + ' ' + jsonResult.Error + ' ' + jsonResult.Result + ' ' + jsonResult.ResultValue);
    },
    error: function (error, errorStatus, errorThrown) {
      errorCallback(error);
    }
  });
}