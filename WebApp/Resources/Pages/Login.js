//Unknown = 0,
//Success = 1,
//Fail = 2,
//VerificationNeeded = 3

var loginService = '/Services/LoginService.asmx/';

$(document).ready(function () {
  $('#signIn').click(function (e) {
    //attempt to sign the user in and process the return request
    e.preventDefault();
    var email = $('#inputEmail').val();
    var org = $('#orgSelect').val();
    if (org == "") org = null;
    var signInData = { email: email, password: $('#inputPassword').val(), organizationId: 1078, verificationRequired: false };

    IssueAjaxRequest(loginService, "SignIn", signInData,
    function (result) {
      //success
      debugger
      alert(result);
    },
    function (error) {
      alert('There was a error signing you in.  Please try again. ')
    });
  });

  $('#inputEmail').keyup(function (e) {
    CheckEmailForOrgs($(this).val())
  });

  $('#forgotPW').click(function (e) {
    //Route them to forgot PW form

    //Populate orgSelect
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
        debugger
      },
      function (error) {
        
      });
    }
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
      //alert(jsonResult.UserId + ' ' + jsonResult.OrganizationId + ' ' + jsonResult.Error + ' ' + jsonResult.Result + ' ' + jsonResult.ResultValue);
    },
    error: function (error, errorStatus, errorThrown) {
      errorCallback(error);
    }
  });
}