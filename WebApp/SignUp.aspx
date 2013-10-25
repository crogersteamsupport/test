<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SignUp.aspx.cs" Inherits="SignUp" EnableViewState="false" Title="TeamSupport - Sign Up" %>

<head>
  <link rel="SHORTCUT ICON" href="~/favicon.ico" />
  <link href="SignUp/css/style.css" rel="stylesheet" type="text/css" />
  <link href="SignUp/css/uniform.default.css" rel="stylesheet" type="text/css" />
  <style type="text/css">
    body { background: #fff;  }
    tr { height: 2em;}
    td input { width: 200px;}
    select{ width:160px;}
    .error { color: Red; display:none;}
    .ui-helper-hidden { display:none;}


  </style>
  <script src="vcr/1_7_0/Js/jquery-latest.min.js" type="text/javascript"></script>
  <script src="SignUp/jquery.uniform.min.js" type="text/javascript"></script>

  <script type="text/javascript">
    var _resultID = -1;
    function getID() { return 34; } // _resultID; }

    $(document).ready(function () {

      $('#priceBoxes').click(function () {


      });
      var _userID = -1;

      $("*").find("input, textarea, select, button").uniform();
      $('.error').hide();
      $('#thanks').hide();
      function checkEnabled() {
        if ($('#checkAgree').prop('checked') == true) {
          $('#btnSubmit').prop('disabled', false);
        }
        else {
          $('#btnSubmit').prop('disabled', true);
        }
        $.uniform.update();
      }

      checkEnabled();
      $('#checkAgree').change(checkEnabled);

      var submitting = false;

      $('#btnSubmit').click(function (e) {
        e.preventDefault();
        if (submitting == true) return;
        if ($('#checkAgree').prop('checked') != true) return;

        $('.error').hide();
        var flag = false;

        $('input.required').each(function (index, el) {
          if ($.trim($(el).val()) == '') {
            $(el).parent().next().find('.error.required').show();
            flag = true;
          }
        });

        var pw = $.trim($('#textPassord').val());

        if (pw.length < 6) {
          $('#textPassord').parent().next().find('.error.required').hide();
          $('#textPassord').parent().next().find('.error.invalid').show();
          flag = true;
        }

        if (pw != $('#textConfirm').val()) {
          $('#textConfirm').parent().next().find('.error.invalid').show();
          flag = true;
        }

        if (flag == false) {

          PageMethods.IsCompanyValid($('#textCompany').val(), function (result) {
            if (result == false) {
              $('#textCompany').parent().next().find('.error.invalid').show();
            }
            else {
              //public static bool SignMeUp(string firstName, string lastName, string email, string company, string phone, int version, string password, string promo, string interest, string seats, string process)
              submitting = true;
              $('#btnSubmit').hide();

              PageMethods.SignMeUp(
                $('#textFirstName').val(),
                $('#textLastName').val(),
                $('#textEmail').val(),
                $('#textCompany').val(),
                $('#textPhone').val(),
                $('#selVersion').val(),
                $('#textPassord').val(),
                $('#textPromo').val(),
                $('#selInterest').val(),
                $('#selSeats').val(),
                $('#selEval').val(),
                function (result) {
                  if (result > 0) {
                    // window.location = 'SignUpThanks.html?userid=' + result;
                    window.top.location = 'http://www.teamsupport.com/thank-you-for-trying-teamsupport/?userid=' + result;
                  }
                  else {
                    submitting = false;
                    $('#btnSubmit').show();

                  }

                });
            }
          });

        }

      });
    });
  </script>
</head>


<html>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePageMethods="true"></asp:ScriptManager>

  <div style="text-align:center;>
    <div id="thecontent" style="margin-top: 20px;">
    <div style="width: 600px; float: left; text-align: left; padding-left: 10px;">
      <table cellpadding="0" cellspacing="10" border="0" width="100%">
        <tr>
          <td>First Name: </td>
          <td>
            <input class="required" id="textFirstName" type="text" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
        <tr>
          <td>Last Name: </td>
          <td>
            <input class="required" id="textLastName" type="text" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
        <tr>
          <td>Email Address: </td>
          <td>
            <input class="required" id="textEmail" type="text" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
            <span class="error invalid">* Email is invalid</span>
          </td>
        </tr>
        <tr>
          <td>Company Name: </td>
          <td>
            <input class="required" id="textCompany" type="text" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
            <span class="error invalid">* Already in use</span>
          </td>
        </tr>
        <tr>
          <td>Phone Number: </td>
          <td>
            <input class="required" id="textPhone" type="text" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
        <tr>
          <td>TeamSupport Version: </td>
          <td>
            <select id="selVersion">
              <option value="2" selected="selected">Enterprise</option>
              <option value="1">Support Desk</option>
              <option value="0">Express</option>
            </select>
          </td>
        </tr>
        <tr>
          <td>Password: </td>
          <td>
            <input class="required" id="textPassord" type="password" maxlength="250"/>
          </td>
          <td>
            <span class="error required">* Required</span>
            <span class="error invalid">* Not long enough</span>
          </td>
        </tr>
        <tr>
          <td>Confirm Password: </td>
          <td>
            <input class="required" id="textConfirm" type="password" maxlength="250"/>
          </td>
          <td>
            <span class="error invalid">* Doesn't match</span>
          </td>
        </tr>
        <tr style="">
          <td>Promotional Code: </td>
          <td>
            <input id="textPromo" type="text" maxlength="250"/>
          </td>
        </tr>
        <tr class="ui-helper-hidden">
          <td>Primary Interest:</td>
          <td>
            <select id="selInterest">
              <option>Internal help desk/support</option>
              <option>Customer facing support</option>
              <option>Both</option>
            </select>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
        <tr class="ui-helper-hidden">
          <td>Number of Seats:</td>
          <td>
            <select id="selSeats">
              <option>1-5</option>
              <option>5-20</option>
              <option>20+</option>
            </select>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
        <tr class="ui-helper-hidden">
          <td>Evaluation process: </td>
          <td>
            <select id="selEval">
              <option>Determining needs and requirements</option>
              <option>Actively evaluating vendors for purchase</option>
              <option>Ready to purchase</option>
            </select>
          </td>
          <td>
            <span class="error required">* Required</span>
          </td>
        </tr>
      </table>
      <div style="padding-top: 20px; text-align: left;">
        <button id="btnSubmit" disabled="disabled">Sign Up</button>
        <br />
        <input type="checkbox" id="checkAgree" checked="checked" class="ui-helper-hidden"/>
        <br />
        <span>By clicking Sign Up, you agree to the <a href="http://www.teamsupport.com/help-desk-subscription-use-service-terms/" target="TSTermsOfService">
          Terms of Service</a> & <a href="http://www.teamsupport.com/privacy/" target="TSPrivacyPolicy">
            Privacy Policy</a> </span>
        <br />
        <br />
      </div>
    </div>
    <div style="float: left; width: 250px; text-align: left;">
      <div style="text-align: center; margin: 0 auto; display:none">
        <a href="#" onclick="window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');">
          <img src="https://app.teamsupport.com/dc/1078/chat/image" /></a></div>
      <div style="background-color: #F1F2F2; padding: 15px 10px;">
        <h3>No Credit Card Required</h3>
        <p style="border-bottom: solid 1px; padding-bottom: 10px;">
        Before your trial expires,
          we will notify you by email at which time you may elect to sign up and pay for your
          account. Otherwise, your account will expire 14 days from today. </p>

        <div style="border-bottom: solid 1px; padding-bottom: 10px;">
          <blockquote>
            <p>
            "I've used lots of different ticketing tools and I can say hands down that TeamSupport is the best I've used. If you're looking for quick and easy setup, flexibility and overall ease of use for support reps TeamSupport is for you."
            </p>
          </blockquote>
          <cite>Sean Wilson - PrintFleet Inc</cite>
          </div>

        <div style="padding-bottom: 10px;">
          <blockquote>
            <p>
            "Delight your customers with excellent service using TeamSupport - it's customer support software done right!"
            </p>
          </blockquote>
          <cite>Eric Pita - ClinicSource</cite>
          </div>

      </div>
    </div>
    </div>
    <div id="thanks">
        <div style="font-size: 30px; padding: 50px 0 50px 0; line-height: 50px; text-align:center;">
        You have successfully signed up. <br /> 
        Thank you for trying TeamSupport!<br />
    <button id="btnContinue">Login Now</button>

  </div>
    </div>

    <div style="clear: both;">
    </div>
  </div>

  </div>
  </form>
</body>
</html>
