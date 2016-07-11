<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatInit.aspx.cs" Inherits="Chat_ChatInit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%-- CSS --%>
    <link href="../Resources/Css/bootstrap3.min.css" rel="stylesheet" type="text/css" />
    <link href="../Resources/Css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="../Resources/Pages/CustomerChat.css" rel="stylesheet" />

</head>
<body>
<%--    <form id="form1" runat="server">
        <div>
    
        </div>
    </form>--%>
            <div class="panel panel-default">
                <div class="panel-heading">How can we help you today?</div>
                <div class="panel-body">
                    <div class="row message-bubble">
                        <p class="text-muted">Matt Townsen</p>
                        <p>Why is yo shit so broke?</p>
                    </div>
                    <div class="row message-bubble">
                        <p class="text-muted">Matt Townsen</p>
                        <p>It Isn't'</p>
                    </div>
                    <div class="row message-bubble">
                        <p class="text-muted">Matt Townsen</p>
                        <p>Umm yes it is</p>
                    </div>
                    <div class="row message-bubble">
                        <p class="text-muted">Matt Townsen</p>
                        <p>Test message</p>
                    </div>
                </div>
                <div class="panel-footer">
                    <div class="input-group">
                        <input type="text" class="form-control" />
                        <span class="input-group-btn">
                        <button class="btn btn-default" type="button">Send</button>
                        </span>
                    </div>
                </div>
            </div>
</body>
    <%-- JS --%>
    <script src="https://js.pusher.com/3.1/pusher.min.js"></script>
    <script src="../Resources/Js/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../Resources/Js/jquery.placeholder.js" type="text/javascript"></script>
    <script src="../Resources/Js/bootstrap3.min.js" type="text/javascript"></script>
    <script src="../Resources/Pages/CustomerChat.js"></script>
</html>
