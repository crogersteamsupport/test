﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ServiceStatus.aspx.cs" Inherits="ServiceStatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="vcr/1_9_0/Css/bootstrap3.min.css" rel="stylesheet" />
    <script src="vcr/1_9_0/Js/jquery-latest.min.js"></script>
    <script src="vcr/1_9_0/Js/bootstrap3.min.js"></script>

     <script type="text/javascript" language="javascript">    
        $(document).ready(function () {

            $('.slack').click(function (e) {
                e.stopPropagation();
                e.preventDefault();
                //$('.queries').html('');
                var btn = $(this).button('loading')
                PageMethods.AckSlack(function (result) {
                    
                    btn.button('reset');
                });
            });
        });
    </script>

    <title></title>
</head>
<body>
    <form id="form1" runat="server">
                  <asp:ScriptManager ID="ScriptManagerMain"
            runat="server"
            EnablePageMethods="true" 
            ScriptMode="Release" 
            LoadScriptsBeforeUI="true">
    </asp:ScriptManager>
        <div class="container">
            <div class="page-header">
                <asp:Literal ID="litStatus" runat="server"></asp:Literal> <button type="button" class="btn btn-primary slack" data-loading-text="Loading...">Acknowlege Issue on Slack</button>
            </div>

            <div class="page-header">
                <h2>Service Statuses</h2>
            </div>

            <div>
                <table class="table table-hover table-striped">
                    <thead>
                        <tr>
                            <th>Service</th>
                            <th>Status</th>
                            <th>Last Time Checked</th>
                            <th>Health</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="litStatusRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
            </div>

            <div class="page-header">
                <h2>Table Count Statuses</h2>
            </div>

            <div>
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>Counted Item</th>
                            <th>Status</th>
                            <th>Max Rows</th>
                            <th>Row Count</th>
                            <th>Message</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Literal ID="litQueryRows" runat="server"></asp:Literal>
                    </tbody>
                </table>
            </div>



        </div>
    </form>
</body>
</html>
