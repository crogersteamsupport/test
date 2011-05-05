<%@ Page Title="" Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="NotePreview.aspx.cs" Inherits="Frames_NotePreview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    body {background-color: #ffffff;}
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div style="height:100%; width:100%; padding: 10px 10px; background-color:#fff; overflow:auto;">
  <div id="divContent" runat="server"  style="height:100%; width:97%;">
  
  </div>
  </div>
</asp:Content>

