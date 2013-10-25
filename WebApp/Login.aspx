<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="false" EnableViewState="false"
  CodeFile="Login.aspx.cs" Inherits="Login" Title="TeamSupport - Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

  <script src="vcr/1_7_0/Js/browser.js" type="text/javascript"></script>
  <style type="text/css">
    select, input.text { width: 250px; }
    a.login-link { font-size: 1em; color: #6D8093; display:block; margin-bottom: .5em;}
    .caption { font-size: 1.2em; padding-bottom: 3px; }
    .link-mobile { margin-left: 2em;}
  </style>

  <script type="text/javascript">

    $(document).ready(function () {
      setInterval("window.location=window.location", 300000)
      $('#companies').combobox();
      $('button').button();
      $("#email").focus();

      $('#btnLogin').click(function () {
        $('#divAlert').hide();
        $.cookie('LastCompany_' + $('#email').val(), $('#companies').val());
        PageMethods.SignIn($('#email').val(), $('#password').val(), $('#companies').val(), $('#remember').prop('checked'),
          BrowserDetect.browser, BrowserDetect.version, navigator.cookieEnabled, BrowserDetect.OS,
          navigator.userAgent, screen.height, screen.width, screen.pixelDepth ? screen.pixelDepth : -1,
          navigator.language ? navigator.language : 'undefined',

          function (result) {
            if (result[0] != null) {
              $('#divAlert').html(result[0]);
              $('#divAlert').show('fast');
            }
            else {
              window.location = result[1];
            }
          }
        );
        return false;

      });

      $('#email')
      .click(function () { loadCompanies($(this).val()); })
      .blur(function () { loadCompanies($(this).val()); })
      .keyup(function () { loadCompanies($(this).val()); });
      loadCompanies($("#email").val());

      function loadCompanies(email) {

        if (isEmailValid(email)) {
          PageMethods.GetCompanies(email, function (result) {
            if (result.length < 2) {
              $('#divCompany').hide();
              return;
            }

            $("#companies").html('');
            for (var i = 0; i < result.length; i++) {
              $("#companies").append('<option value="' + result[i].ID + '">' + result[i].Label + '</option>');
            }

            $('#companies option:selected').removeAttr('selected');

            var lastID = $('#companies option[value="' + $.cookie('LastCompany_' + $('#email').val()) + '"]').attr('selected', 'selected').html();
            if (lastID) $('#companies').combobox("update");

            $('#divCompany').show();
          });

        }
        else {
          $('#divCompany').hide();
        }
      }

      function isEmailValid(email) {
        var pattern = /^([a-zA-Z0-9_.-])+@([a-zA-Z0-9_.-])+\.([a-zA-Z])+([a-zA-Z])+/;
        return pattern.test(email);
      }

      function getRememberMe() {
        var cookie = Ts.Utils.getCookie('rememberme', 'sessionid');
        if (cookie != null && cookie.length > 0) {
          PageMethods.GetEmail(cookie, function (result) {
            $('#email').val(result);
            $('#password').val('rememberme');
            $('#remember').attr('checked', 'checked')
            loadCompanies($('#email').val())
          });
        }
        /*
        var cookies = $.cookie('rememberme').split('&');
        for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].split('=');
        if (cookie.length > 1) {
        if (cookie[0] == 'sessionid') {
        PageMethods.GetEmail(cookie[1], function (result) {
        $('#email').val(result);
        $('#password').val('rememberme');
        $('#remember').attr('checked', 'checked')
        loadCompanies($('#email').val())


        });
        }
        }
        }
        */
      }
      getRememberMe();

    });
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div style="float: left; margin: 20px 0 0 50px;">
    <img src="images/icons/Roles.png" /></div>
  <div style="float: left;">
    <div style="padding: 20px 0 20px 50px; text-align: left;">
      <div style="padding-bottom: 10px;">
        <div style="font-size: 2em; font-weight: normal; padding-bottom: 10px;">
          Login to TeamSupport
        </div>
        <div class="ui-widget ui-state-highlight ui-corner-all ui-helper-hidden" id="divAlert"
          style="padding: 5px 10px;">
        </div>
      </div>
      <div style="width: 450px;">
        <div class="caption">
          Email Address:</div>
        <div>
          <input type="text" id="email" class="text ui-widget ui-widget-content ui-corner-all" /></div>
        <br />
        <div class="caption">
          Password:</div>
        <div>
          <input type="password" id="password" class="text ui-widget ui-widget-content ui-corner-all" />
        </div>
        <div id="divCompany" class="ui-helper-hidden">
          <br />
          <div class="caption">
            Company:</div>
          <div>
            <select id="companies">
              <option value="-1">Select a company...</option>
            </select>
          </div>
          <div style="clear:both;"></div>
        </div>
        <br />
        <div>
          <input type="checkbox" id="remember" />Remember me on this computer</div>
        <br />
        <div>
          <button id="btnLogin">Login</button><a href="https://m.teamsupport.com" class="link-mobile ts-link ui-state-default">Visit Mobile Site</a>
        </div>
        <div style="clear: both;">
          &nbsp</div>
        <a class="ts-link login-link" href="ResetPassword.aspx?reason=forgot">Forgot your
          password?</a>
        <a class="ts-link login-link" href="SignUp.aspx">Don't have an account? Sign up for
          free.</a>
          
      </div>
    </div>
  </div>
  <a target="TSEarn25" href="http://www.teamsupport.com/web-support-software-contact-us/refer-your-friends-to-teamsupport-the-industrys-best-help-desk-software/"><img src="vcr/1_7_0/images/earn25.jpg" /></a>
</asp:Content>
