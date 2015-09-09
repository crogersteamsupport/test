﻿var loginService = '/Services/LoginService.asmx/';
var resourcesURL = '/vcr/1_9_0/Pages/';
var returnURL = '/';

$(document).ready(function () {
	returnURL = top.Ts.Utils.getQueryValue("ReturnUrl", window);
	supportToken = top.Ts.Utils.getQueryValue("SupportToken", window);
	if (supportToken && supportToken != '')
	{
		IssueAjaxRequest(loginService, "SupportSignIn", { token: supportToken }, function (result) { window.location = '/'; }, function () { });
		return;
	}
  getRememberMe();
  $('#signIn').click(function (e) {
    e.preventDefault();
    var email = $('#inputEmail').val();
    var org = $('#orgSelect').val();
    if (org == "") org = null;
    var signInData = { email: email, password: $('#inputPassword').val(), organizationId: org, verificationRequired: true };

    IssueAjaxRequest(loginService, "SignIn", signInData,
    function (result) {debugger
      switch (result.Result) {//Unknown = 0, Success = 1, Fail = 2, VerificationNeeded = 3, VerificationSetupNeeded = 4, ExipredPassword = 5
        case 1:
          window.location = returnURL;
          break;
        case 3:
          window.location = resourcesURL + 'LoginTwoStep.html?UserID=' + result.UserId;
          break;
        case 4:
          window.location = resourcesURL + 'LoginTwoStepSetup.html?UserID=' + result.UserId;
          break;
			case 5:
				window.location = resourcesURL + result.RedirectURL;
      		break;
        default:
        	$('#loginError').text(result.Error).show();
        	if (result.LoginFailedAttempts > 0 && result.LoginFailedAttempts <= 10) {
        		$('#numbAttempts').text(result.LoginFailedAttempts).parent().show();
        	}
        	else $('#numbAttempts').parent().hide();
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

    if (companies.length > 1) {
    	companySelect.show();
    }
    else companySelect.hide();
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
      if (errorCallback) errorCallback(error);
    }
  });
}