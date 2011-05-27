<%@ Page Language="C#" MasterPageFile="~/Standard.master" AutoEventWireup="true"
  CodeFile="SignUp.aspx.cs" Inherits="SignUp" Title="TeamSupport - Sign Up" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
  <style type="text/css">
    h2
    {
      border-bottom: Solid 1px #7F9DB9;
      padding-top: 20px;
    }
    li
    {
      padding-bottom: 8px;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div>
    <div style="width: 500px; float: left; text-align: left; padding-left: 10px;">
  <div style="font-size: 24px; padding: 0 0 10px 10px;">Setup your 14 day free trial</div>
      <table cellpadding="0" cellspacing="10" border="0" width="100%">
        <tr>
          <td>
            First Name:
          </td>
          <td>
            <telerik:RadTextBox ID="Ecom_BillTo_Postal_Name_First" runat="server" Width="175px"
              MaxLength="250">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="* First name is required"
              ControlToValidate="Ecom_BillTo_Postal_Name_First"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            Last Name:
          </td>
          <td>
            <telerik:RadTextBox ID="Ecom_BillTo_Postal_Name_Last" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="* Last name is required"
              ControlToValidate="Ecom_BillTo_Postal_Name_Last"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            Email Address:
          </td>
          <td>
            <telerik:RadTextBox ID="Ecom_BillTo_Online_Email" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="* Email address is invalid"
              ValidationExpression=".*@.*\..*" ControlToValidate="Ecom_BillTo_Online_Email" Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="* Email address is required"
              ControlToValidate="Ecom_BillTo_Online_Email" Display="Dynamic"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            Company Name:
          </td>
          <td>
            <telerik:RadTextBox ID="Ecom_BillTo_Postal_Company" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="* Company name is required"
              ControlToValidate="Ecom_BillTo_Postal_Company"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            Phone Number:
          </td>
          <td>
            <telerik:RadTextBox ID="Ecom_BillTo_Telecom_Phone_Number" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="* Phone number is required"
              ControlToValidate="Ecom_BillTo_Telecom_Phone_Number"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            TeamSupport Version:
          </td>
          <td>
            <telerik:RadComboBox ID="cmbVersion" runat="server" Width="175px">
            </telerik:RadComboBox>
          </td>
        </tr>
        <tr>
          <td>
            Password:
          </td>
          <td>
            <telerik:RadTextBox ID="textPassword" runat="server" Width="175px" TextMode="Password">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="* Must be at least 6 characters long"
              ValidationExpression=".{6,50}" ControlToValidate="textPassword" Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="* Password is required"
              ControlToValidate="textPassword" Display="Dynamic"></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td>
            Confirm Password:
          </td>
          <td>
            <telerik:RadTextBox ID="textPassword2" runat="server" Width="175px" TextMode="Password">
            </telerik:RadTextBox>
          </td>
          <td>
            <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="* Passwords do not match"
              ControlToCompare="textPassword2" ControlToValidate="textPassword"></asp:CompareValidator>
          </td>
        </tr>
        <tr style="display:none;">
          <td>
            How did you find us?:
          </td>
          <td>
            <telerik:RadTextBox ID="textHeard" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
        </tr>
        <tr style="display:none;">
          <td>
            Promotional Code:
          </td>
          <td>
            <telerik:RadTextBox ID="textPromoCode" runat="server" Width="175px">
            </telerik:RadTextBox>
          </td>
        </tr>
        <tr>
          <td>
            Primary Interest in TeamSupport?
          </td>
          <td>
            <telerik:RadComboBox ID="cmbInterest" runat="server" Width="175px" AllowCustomText="true" MarkFirstMatch="false">
              <Items>
                <telerik:RadComboBoxItem Text="Internal help desk/support" />
                <telerik:RadComboBoxItem Text="Customer facing support" />
                <telerik:RadComboBoxItem Text="Both" />
              </Items>
            </telerik:RadComboBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="* Required"
              ControlToValidate="cmbInterest"></asp:RequiredFieldValidator>
          
          </td>
        </tr>
        <tr>
          <td>
            Number of potential seats required?
          </td>
          <td>
            <telerik:RadComboBox ID="cmbSeats" runat="server" Width="175px" AllowCustomText="true" MarkFirstMatch="false">
              <Items>
                <telerik:RadComboBoxItem Text="1-5" />
                <telerik:RadComboBoxItem Text="5-20" />
                <telerik:RadComboBoxItem Text="20+" />
              </Items>
            </telerik:RadComboBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="* Required"
              ControlToValidate="cmbSeats"></asp:RequiredFieldValidator>
          
          </td>
        </tr>
        <tr>
          <td>
            Evaluation process
          </td>
          <td>
            <telerik:RadComboBox ID="cmbEval" runat="server" Width="175px" AllowCustomText="true" MarkFirstMatch="false">
              <Items>
                <telerik:RadComboBoxItem Text="Determining needs and requirements" />
                <telerik:RadComboBoxItem Text="Actively evaluating vendors for purchase" />
                <telerik:RadComboBoxItem Text="Ready to purchase" />
              </Items>
            </telerik:RadComboBox>
          </td>
          <td>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="* Required"
              ControlToValidate="cmbEval"></asp:RequiredFieldValidator>
          
          </td>
        </tr>
      </table>
      <div style="padding-top: 20px; text-align: left;">
        <asp:CheckBox ID="cbAgree" runat="server" AutoPostBack="true" />
        <span>I agree to the <a href="http://teamsupport.com/terms.php" target="TSTermsOfService">
          Terms of Service</a> & <a href="http://teamsupport.com/privacy.php" target="TSPrivacyPolicy">
            Privacy Policy</a> </span>
        <br />
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="Sign Up!" OnClick="btnSubmit_Click"
          Enabled="false" />
      </div>
    </div>
    <div style="float: left; width: 250px;  margin-top: 5px; text-align: left;">
      <div style="text-align:center; margin:0 auto;"><a href="#" onclick="window.open('https://app.teamsupport.com/Chat/ChatInit.aspx?uid=22bd89b8-5162-4509-8b0d-f209a0aa6ee9', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500');"><img src="https://app.teamsupport.com/dc/1078/chat/image" /></a></div>
      <div class="ui-corner-all" style="background-color: #C9D9F5; padding: 5px 10px; margin-top: 10px;">
      <h3>
        No Credit Card Required</h3>
      <p style="border-bottom: solid 1px; padding-bottom: 10px;">
        Before your trial expires, we will notify you by email at which time you may elect
        to sign up and pay for your account. Otherwise, your account will expire 14 days
        from today.
      </p>
      <h3>
        Billing</h3>
      <p>
        We accommodate monthly billing via credit card or annual payment via company check
        or credit card. We can also accept a company PO for invoicing.
      </p>
      <p style="border-bottom: solid 1px; padding-bottom: 10px;">
        *Discounts available for annual payments and large groups. Contact sales for more
        info.
      </p>
      <h3>Need help?</h3>
      <h3>Have Questions?</h3>
      <p>
        Contact us <a href="mailto:sales@teamsupport.com">here</a></p>
        
        </div>
    </div>
    <div style="clear: both;">
    </div>
  </div>
  <asp:HiddenField ID="fieldPostToken" runat="server" />

  <!-- Google Code for Sign Up Conversion Page -->

  <script type="text/javascript">
<!--
    var google_conversion_id = 1038685820;
    var google_conversion_language = "en_US";
    var google_conversion_format = "1";
    var google_conversion_color = "ffffff";
    var google_conversion_label = "fftdCI6XngEQ_Kyk7wM";
    var google_conversion_value = 0;
//-->
  </script>

  <script type="text/javascript" src="https://www.googleadservices.com/pagead/conversion.js">
  </script>




</asp:Content>
