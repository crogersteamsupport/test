<%@ Page Title="" Language="C#" MasterPageFile="~/StandardForm.master" AutoEventWireup="true" CodeFile="SiteDown.aspx.cs" Inherits="SiteDown"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    button { padding: 1em 2em;}
  </style>

  <script language="javascript" type="text/javascript">
    setInterval(tryLogin, 20000);

    function tryLogin() {
      window.location = 'Login.aspx';
    }
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="font-size: 2em; padding: 20px 0 20px 0;">We have updated our servers, sorry for any inconvenience.</div>
  <button onclick="tryLogin(); return false;">Continue</button>
  <br />
  <br />
  <br />
  <br />
</asp:Content>

