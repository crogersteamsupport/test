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
            <div class="panel panel-default">
                <div class="panel-heading">How can we help you today?</div>
                <div class="panel-body">
                    <form id="newChatForm">
                        <div class="form-group">
                            <label for="userFirstName">First Name</label>
                            <input type="text" class="form-control" id="userFirstName" placeholder="First Name">
                        </div>
                        <div class="form-group">
                            <label for="userLastName">Last Name</label>
                            <input type="text" class="form-control" id="userLastName" placeholder="Last Name">
                        </div>
                        <div class="form-group">
                            <label for="userEmail">Email address</label>
                            <input type="email" class="form-control" id="userEmail" placeholder="Email">
                        </div>
                        <div class="form-group">
                            <label for="userIssue">What is your questions?</label>
                            <textarea class="form-control" id="userIssue" rows="5"></textarea>
                        </div>
                        <button type="submit" class="btn btn-default">Submit</button>
                    </form>
                    <ul id="message-list" class="list-group" style="display:none;">
<%--                        <li class="list-group-item message-bubble">
                            <p class="text-muted">Matt Townsen</p>
                            <p>Why is yo shit so broke?</p>
                        </li>
                        <li class="list-group-item message-bubble">
                            <p class="text-muted">Matt Townsen</p>
                            <p>Why is yo shit so broke?</p>
                        </li>
                        <li class="list-group-item message-bubble">
                            <p class="text-muted">Matt Townsen</p>
                            <p>Why is yo shit so broke?</p>
                        </li>--%>
                    </ul>
                </div>
                <div class="panel-footer form-footer" style="display:none;">
                    <form id="message-form" action="">
                        <div class="input-group">
                            <input id="message" type="text" class="form-control" />
                            <span class="input-group-btn">
                                <button class="btn btn-default" type="submit">Send</button>
                            </span>
                        </div>
                    </form>

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
