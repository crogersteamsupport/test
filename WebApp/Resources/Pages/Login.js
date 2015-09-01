//Unknown = 0,
//Success = 1,
//Fail = 2,
//VerificationNeeded = 3

$(document).ready(function () {
  $('#signIn').click(function (e) {
    //attempt to sign the user in and process the return request
    e.preventDefault();
    debugger
    var email = $('#inputEmail').val();
    var org = $('#orgSelect').val();
    if (org == "") org = null;
    var pw = $('#inputPassword').val();

    $.ajax({
      type: "POST",
      url: "/Services/LoginService.asmx/SignIn",
      data: JSON.stringify({ email: email, password: pw, organizationId: org, verificationRequired: false }),
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data) {
        debugger
      }
    });
  });

  $('#inputEmail').keyup(function (e) {
    
  });

  $('#forgotPW').click(function (e) {
    //Route them to forgot PW form

    //Populate orgSelect
  });

  $('#createAccount').click(function (e) {
    //redirect to signup form
  });

  function CheckEmailForOrgs() {
    //Code to lookup the email address entered and check for a valid user with multiple orgs.  
  }
});