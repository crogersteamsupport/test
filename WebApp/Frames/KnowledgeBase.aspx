<%@ Page Language="C#" MasterPageFile="~/Frames/Frame.master" AutoEventWireup="true" CodeFile="KnowledgeBase.aspx.cs" Inherits="Frames_KnowledgeBase" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script type="text/javascript" language="javascript">
    var _lastRefreshTime = new Date();

    function refreshData() {
      if (_lastRefreshTime != null) {
        var now = new Date();
        var diff = (now - _lastRefreshTime) / 1000;
        if (diff < 300) return;
      }

      _lastRefreshTime = new Date();

      window.location = window.location;

    }
  
  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <iframe id="ticketsFrame" runat="server" class="contentFrame" scrolling="no" src="" frameborder="0" height="100%" width="100%"></iframe>
    
   

</asp:Content>

