<%@ Page Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="false"
  CodeFile="Login.aspx.cs" Inherits="Login" Title="TeamSupport - Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    select, input.text { width: 250px; }
    
    .caption { font-size: 1.2em; padding-bottom: 3px; }
    a.login-link { text-decoration: underline !important; font-size: 1.4em; }
  </style>

  <script type="text/javascript">

    $(document).ready(function () {
      setInterval("window.location=window.location", 300000)
      $('#companies').combobox();
      $('button').button();

      var BrowserDetect = {
        init: function () {
          this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
          this.version = this.searchVersion(navigator.userAgent)
			|| this.searchVersion(navigator.appVersion)
			|| "an unknown version";
          this.OS = this.searchString(this.dataOS) || "an unknown OS";
        },
        searchString: function (data) {
          for (var i = 0; i < data.length; i++) {
            var dataString = data[i].string;
            var dataProp = data[i].prop;
            this.versionSearchString = data[i].versionSearch || data[i].identity;
            if (dataString) {
              if (dataString.indexOf(data[i].subString) != -1)
                return data[i].identity;
            }
            else if (dataProp)
              return data[i].identity;
          }
        },
        searchVersion: function (dataString) {
          var index = dataString.indexOf(this.versionSearchString);
          if (index == -1) return;
          return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
        },
        dataBrowser: [
		{
		  string: navigator.userAgent,
		  subString: "Chrome",
		  identity: "Chrome"
		},
		{ string: navigator.userAgent,
		  subString: "OmniWeb",
		  versionSearch: "OmniWeb/",
		  identity: "OmniWeb"
		},
		{
		  string: navigator.vendor,
		  subString: "Apple",
		  identity: "Safari",
		  versionSearch: "Version"
		},
		{
		  prop: window.opera,
		  identity: "Opera"
		},
		{
		  string: navigator.vendor,
		  subString: "iCab",
		  identity: "iCab"
		},
		{
		  string: navigator.vendor,
		  subString: "KDE",
		  identity: "Konqueror"
		},
		{
		  string: navigator.userAgent,
		  subString: "Firefox",
		  identity: "Firefox"
		},
		{
		  string: navigator.vendor,
		  subString: "Camino",
		  identity: "Camino"
		},
		{		// for newer Netscapes (6+)
		  string: navigator.userAgent,
		  subString: "Netscape",
		  identity: "Netscape"
		},
		{
		  string: navigator.userAgent,
		  subString: "MSIE",
		  identity: "Explorer",
		  versionSearch: "MSIE"
		},
		{
		  string: navigator.userAgent,
		  subString: "Gecko",
		  identity: "Mozilla",
		  versionSearch: "rv"
		},
		{ 		// for older Netscapes (4-)
		  string: navigator.userAgent,
		  subString: "Mozilla",
		  identity: "Netscape",
		  versionSearch: "Mozilla"
		}
	],
        dataOS: [
		{
		  string: navigator.platform,
		  subString: "Win",
		  identity: "Windows"
		},
		{
		  string: navigator.platform,
		  subString: "Mac",
		  identity: "Mac"
		},
		{
		  string: navigator.userAgent,
		  subString: "iPhone",
		  identity: "iPhone/iPod"
		},
		{
		  string: navigator.platform,
		  subString: "Linux",
		  identity: "Linux"
		}
	]

      };
      BrowserDetect.init();


      $('#btnLogin').click(function () {
        $('#divAlert').hide();
        $.cookie('LastCompany_' + $('#email').val(), $('#companies').val());
        PageMethods.SignIn($('#email').val(), $('#password').val(), $('#companies').val(), $('#remember').attr('checked'),
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
        if (cookie.length > 0) {
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
          <button id="btnLogin">Login</button>
        </div>
        <div style="clear: both;">
          &nbsp</div>
        <br />
        <a class="ts-link login-link" href="ResetPassword.aspx?reason=forgot">Forgot your
          password?</a>
        <br />
        <br />
        <a class="ts-link login-link" href="SignUp.aspx">Don't have an account? Sign up for
          free.</a>
      </div>
    </div>
  </div>
</asp:Content>
