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
        <div class="panel-heading">Welcome to our live chat!</div>
        <div class="panel-body">
            <form id="newChatForm" class="container">
                <div class="row">
                    <div class="col-xs-9 col-sm-10 col-md-11">                        
                        <div class="alert alert-info chatOfflineWarning" role="alert">
                            Our live chat is not available at this time.  Please submit a ticket request in the form below, and a member of our team will follow up with you as soon as possible.<br />Thank you!
                        </div>
                        <div class="form-group">
                            <label for="userFirstName">First Name</label>
                            <input type="text" class="form-control" id="userFirstName" placeholder="First Name" />
                        </div>
                        <div class="form-group">
                            <label for="userLastName">Last Name</label>
                            <input type="text" class="form-control" id="userLastName" placeholder="Last Name" />
                        </div>
                        <div class="form-group">
                            <label for="userEmail">Email address</label>
                            <input type="email" class="form-control" id="userEmail" placeholder="Email" />
                        </div>
                    </div>    
                    <div class="col-xs-3 col-sm-2 col-md-1">
                        <div class="chat-logo pull-right"></div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-12 col-sm-12 col-md-12">
                        <label for="userIssue" class="description-label">How can we help you?</label>
                        <textarea class="form-control" id="userIssue" rows="5"></textarea>
                    </div>
                </div>
                <button type="submit" class="btn btn-default" style="margin-top:10px;">Submit</button>
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
