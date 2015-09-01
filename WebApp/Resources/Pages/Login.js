$(document).ready(function () {
  $('#signIn').click(function (e) {
    debugger
    //attempt to sign the user in and process the return request
    e.preventDefault();
    $.ajax({
      type: "POST",
      url: "/LoginService.asmx/SignIn",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data) {
        
      }
    });
  })

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