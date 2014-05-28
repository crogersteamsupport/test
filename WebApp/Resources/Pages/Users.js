

var userID
$(document).ready(function () {
    $('body').layout({
        applyDemoStyles: true
    });

    /*initialize */
    userID = top.Ts.System.User.UserID;
    var activeID;

    var organizationID = top.Ts.System.User.OrganizationID;
    if (organizationID != 1078 && organizationID != 13679 && organizationID != 1088) {
        $('#ratingsTab').hide();
    }

    if (!top.Ts.System.User.IsSystemAdmin)
    {
        $('#historyTab').hide();
    }

    if (top.Ts.Utils.getQueryValue("UserID", window) != null) {
        userID = top.Ts.Utils.getQueryValue("UserID", window);
        top.Ts.Services.Users.GetUser(userID, function (user) {
            $('#userName').text(user.FirstName + " " + user.LastName);
        });
        
    }
    else
    {
        $('#userName').text(top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName);
    }
        
    $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
    $('#userTabs a:first').tab('show');
    Search();
    

    $('#infoIframe, #openIframe, #closedIframe, #allIframe, #queueIframe, #historyIframe, #userName').load(function () {
        $('.maincontainer').fadeTo(0, 1);
    });

    if (!top.Ts.System.User.IsSystemAdmin) {
        $('#userNew').hide();
        $('#userDelete').hide();
    }

    /* functions */

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        if (e.target.innerHTML == "User Information")
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
        else if (e.target.innerHTML == "Open")
            $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
        else if (e.target.innerHTML == "Closed")
            $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
        else if (e.target.innerHTML == "All Tickets")
            $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
        else if (e.target.innerHTML == "Ticket Queue")
            $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
        else if (e.target.innerHTML == "History")
            $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
        else if (e.target.innerHTML == "Ratings")
            LoadRatings();
    });

    var _tmrSearch = null;
    $('#searchString').keyup(function () {
        if (_tmrSearch != null) {
            clearTimeout(_tmrSearch);
        }
        $('.user-container').fadeTo(200, 0.5);

        _tmrSearch = setTimeout(Search, 500);

    });

    //function Search()
    //{
    //    top.Ts.Services.Users.GetUsersSearch(top.Ts.System.User.OrganizationID, $('#searchString').val(), function (html) {
    //        $('.user-container').empty();
    //        $('.user-container').fadeTo(0, 1);
    //        $('.user-container').append(html);
    //        $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });
            
    //    });
    //}

    function SearchDelete()
    {
        top.Ts.Services.Users.GetUsersSearch(top.Ts.System.User.OrganizationID, $('#searchString').val(), function (html) {
            $('.user-container').empty();
            $('.user-container').fadeTo(0, 1);
            $('.user-container').append(html);
            $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });

            userID = top.Ts.System.User.UserID;
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
            $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
            $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
            $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
            $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
            $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
            $('#userName').text(top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName);
        });
    }

    $('.user-container').on('click', '.user', function (e) {
        e.preventDefault();
        $('.maincontainer').fadeTo(200, 0.5);
        userID = $(this).attr('uid');
        $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
        $('#openIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=false&tf_UserID=" + userID);
        $('#closedIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsClosed=true&tf_UserID=" + userID);
        $('#allIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_UserID=" + userID);
        $('#queueIframe').attr("src", "/vcr/1_9_0/Pages/TicketGrid.html?tf_IsEnqueued=true&tf_ViewerID=" + userID);
        $('#historyIframe').attr("src", "../../../Frames/History.aspx?RefType=22&RefID=" + userID);
        $('#userName').text($(this).text());
        LoadRatings();
        activeID = userID;
        
    });

    $('.user-container').on('click', '.user-chat', function (e) {
        e.preventDefault();
        window.parent.openChat($(this).parent().parent().parent().find('strong').text(), $(this).attr('cid'));
        
    });

    $('#userNew').click(function () {
        top.Ts.Services.Users.GetNewUserMessage(function (msg) {
            if (msg != "")
                alert(msg);
            else
                ShowDialog(top.GetUserDialog(top.Ts.System.User.OrganizationID));
        });
    });


    $('#userDelete').click(function () {
        if (confirm("Are you sure you would like to PERMANENTLEY delete this user?")) {
            top.privateServices.DeleteUser(activeID, SearchDelete);
            top.Ts.System.logAction('Users - User Deleted');
        }
    });

    function DialogClosed(sender, args) {
        sender.remove_close(DialogClosed);
        window.location = window.location;
    }

    function ShowDialog(wnd) {
        wnd.add_close(DialogClosed);
        wnd.show();
    }

    $(window).bind('resize', function () {
        if ($("ul#userTabs li.active").text() == "User Information")
        {
            $('#infoIframe').attr("src", "/vcr/1_9_0/Pages/User.html?UserID=" + userID);
            autoResize();
        }
        

        ;
    });

    $('#ddlRatingFilter').on('change', function () {
        LoadRatings();
    });

    LoadRatings();

    function LoadRatings() {
        $('#tblRatings tbody').empty();
        top.Ts.Services.Customers.LoadAgentRatingsUser(userID, $('#ddlRatingFilter').val(), function (ratings) {
            var agents = "";
            for (var i = 0; i < ratings.length; i++) {
                for (var j = 0; j < ratings[i].users.length; j++) {
                    if (j != 0)
                        agents = agents + ", ";

                    agents = agents + '<a href="#" target="_blank" onclick="top.Ts.MainPage.openUser(' + ratings[i].users[j].UserID + '); return false;">' + ratings[i].users[j].FirstName + ' ' + ratings[i].users[j].LastName + '</a>';
                }

                var tr = $('<tr>')
                .html('<td><a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ratings[i].rating.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ratings[i].rating.TicketNumber + '); return false;">Ticket ' + ratings[i].rating.TicketNumber + '</a></td><td>' + agents + '</td><td>' + ratings[i].reporter.FirstName + ' ' + ratings[i].reporter.LastName + '</td><td>' + ratings[i].rating.DateCreated.toDateString() + '</td><td>' + ratings[i].rating.RatingText + '</td><td>' + (ratings[i].rating.Comment === null ? "None" : ratings[i].rating.Comment) + '</td>')
                .appendTo('#tblRatings > tbody:last');
            }
        });
    }

});

function Search() {
    top.Ts.Services.Users.GetUsersSearch(top.Ts.System.User.OrganizationID, $('#searchString').val(), function (html) {
        $('.user-container').empty();
        $('.user-container').fadeTo(0, 1);
        $('.user-container').append(html);
        $('.user-tooltip').tooltip({ placement: 'bottom', container: 'body' });

    });
}

function Update() {
    Search();
}

function autoResize() {
    $('#infoIframe').attr('height', $('.maincontainer').outerHeight() - $('.main-nav').outerHeight());
}