<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatInit.aspx.cs" Inherits="Chat_ChatInit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%-- CSS --%>
    <link href="../vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" type="text/css" />
    <link href="../vcr/1_9_0/Css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="../vcr/1_9_0/Pages/CustomerChatInit.css" rel="stylesheet" />

</head>
<body>
            <div class="panel panel-default chatRequestForm" style="display:none;">
                <div class="panel-heading">How can we help you today?</div>
                <div class="panel-body">
                    
                    <form id="newChatForm">
                        <div class="alert alert-info chatRequestLabel" role="alert">
                            <strong>A chat operator is not available.</strong><br />
                            Please submit your question and we will get back with you.
                        </div>
                        <div class="form-group">
                            <label for="userFirstName">First Name</label>
                            <input type="text" class="form-control" id="userFirstName" placeholder="First Name" value="Matt">
                        </div>
                        <div class="form-group">
                            <label for="userLastName">Last Name</label>
                            <input type="text" class="form-control" id="userLastName" placeholder="Last Name" value="Townsen">
                        </div>
                        <div class="form-group">
                            <label for="userEmail">Email address</label>
                            <input type="email" class="form-control" id="userEmail" placeholder="Email" value="thunderfan1984@gmail.com">
                        </div>
                        <div class="form-group">
                            <label for="userIssue">What is your questions?</label>
                            <textarea class="form-control" id="userIssue" rows="5"></textarea>
                        </div>
                        <button type="submit" class="btn btn-default">Submit</button>
                    </form>
                </div>
            </div>
</body>
    <%-- JS --%>
    <script src="https://js.pusher.com/3.1/pusher.min.js"></script>
    <script src="../vcr/1_9_0/Js/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../vcr/1_9_0/Js/jquery.placeholder.js" type="text/javascript"></script>
    <script src="../vcr/1_9_0/Js/bootstrap3.min.js" type="text/javascript"></script>
    <script src="../vcr/1_9_0/Js/Ts/ts.utils.js"></script>
    <script src="../vcr/1_9_0/Pages/CustomerChatInit.js"></script>
</html>
