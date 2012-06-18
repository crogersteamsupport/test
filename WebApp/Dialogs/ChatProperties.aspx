<%@ Page Title="Chat Properties" Language="C#" MasterPageFile="~/Dialogs/Dialog.master"
  AutoEventWireup="true" CodeFile="ChatProperties.aspx.cs" Inherits="Dialogs_ChatProperties" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

  <link href="../css_5/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="../js_5/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="../vcr/142/Js/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <script src="../js_5/ajaxupload.js" type="text/javascript"></script>
  <style type="text/css">
    .crmLabelDiv
    {
      padding-top: 10px;
    }
    .dialogContentDiv div
    {
      padding-bottom: 5px;
    }
    
    .label {}
    .image {}
    .button {}
    
  </style>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
       <div class="image">
         <asp:Image ID="imgAvailable" runat="server" />
       </div>
      <div class="label">
        Available Image Upload</div>
      <div>
        <telerik:RadUpload ID="upAvailable" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
       <div class="image">
         <asp:Image ID="imgUnavailable" runat="server" />
       </div>
      <div class="label">
        Unavailable Image Upload</div>
      <div>
        <telerik:RadUpload ID="upUnavailable" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
       <div class="image">
         <asp:Image ID="imgLogo" runat="server" />
       </div>
      <div class="label">
        Company Logo Upload (Side Banner)</div>
        <div>Recomended Size (w:100 x h:250)</div>
      <div>
        <telerik:RadUpload ID="upLogo" runat="server" MaxFileInputsCount="1" OverwriteExistingFiles="True"
          ControlObjectsVisibility="None">
        </telerik:RadUpload>
      </div>
    </div>
  </div>
  
  
</asp:Content>
