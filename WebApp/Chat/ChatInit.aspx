
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat_ChatInit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/frontend/css/core/flexbox.css" rel="stylesheet" />
    <link href="/frontend/css/features/customerchat.css" rel="stylesheet" />
    <script src="https://js.pusher.com/3.1/pusher.min.js"></script>
    <script src="/frontend/library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="/frontend/library/jquery.placeholder.js" type="text/javascript"></script>
    <script src="/frontend/javascript/features/deflector.js?10" type="text/javascript"></script>
    <script src="../vcr/1_9_0/Js/Ts/ts.utils.js"></script>
    <script src="/frontend/javascript/features/customerchat.js"></script>
</head>

<body>

    <div style="width:100%;">

        <div class="flexbox column">
            <div cla="flex" style="padding:5px 10px;background-color:#2e3f52;color:white;">
                <div>Welcome to our live chat!</div>
            </div>
            <div cla="flex">
                <div class="alert alert-info chatOfflineWarning" role="alert" style="display:none;">
                    Our live chat is not available at this time. Please submit a ticket request in the form below, and a member of our team will follow up with you as soon as possible.<br />Thank you!
                </div>
            </div>
        </div>

        <form id="newChatForm" class="container" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server" ScriptMode="Release" EnableScriptGlobalization="True">
                <services>
                    <asp:ServiceReference Path="~/Services/DeflectorService.asmx" />
                </services>
            </asp:ScriptManager>

            <div class="flexbox" style="width:100%;">
                <div class="flex1" style="padding:5px;">
                    <div>
                        <input type="text" id="userFirstName" placeholder="First Name" style="width:100%;">
                    </div>
                </div>
                <div class="flex1">
                    <div style="padding:5px;">
                        <input type="text" id="userLastName" placeholder="Last Name" style="width:100%;">
                    </div>
                </div>
            </div>

            <div class="flexbox column" style="width:100%;">
                <div class="flex">
                    <div style="padding:5px;">
                        <input type="text" id="userEmail" placeholder="Email Address" style="width:100%;">
                    </div>
                </div>
                <div class="flex">
                    <div style="padding:5px;">
                        <textarea id="userIssue" rows="5" placeholder="How can we help you?" style="width:100%;"></textarea>
                    </div>
                </div>
            </div>

            <div class="flexbox column" style="width:100%;">
                <div class="flex">
                    <div>
                        <div class="list-group" id="deflection-results" style="margin-top:20px;"></div>
                    </div>
                </div>
            </div>

            <div class="flexbox column" style="width:100%;">
                <div class="flex">
                    <div style="text-align:right;">
                        <button type="submit" class="btn btn-default" style="margin-top:10px;">Submit New Ticket</button>
                    </div>
                </div>
            </div>

        </form>

    </div>

</body>

</html>
