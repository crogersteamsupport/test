$(document).ready(function () {

    //check if the calendar is on a sub section or not
    var pageType = top.Ts.Utils.getQueryValue("pagetype", window);
    var pageID = top.Ts.Utils.getQueryValue("pageid", window);

    if (pageType == null)
        pageType = -1;

    if (pageID == null)
        pageID = -1;

    $('body').layout({
        applyDemoStyles: true
    });

    //test function
    var date = new Date();
    var day = date.getDate();        // yields day
    var month = date.getMonth()+1;    // yields month
    var year = date.getFullYear();  // yields year
    // After this construct a string with the above results as below
    var time = day + "/" + month + "/" + year;
    
    //var e = top.Ts.Services.Users.GetCalendarEvents($.fullCalendar.moment('2014-05-01'));
    
    //initialize the calendar
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        editable: true,
        eventLimit: true, 
        height: '200',
        aspectRatio: 2.3,
        dayClick: function (date, jsEvent, view) {
            clearModal();
            $('#inputStartTime').val(moment(date).format('MM/DD/YYYY hh:mm'));
            $('#fullCalModal').modal();

            //alert('Clicked on: ' + date.format());
            //alert('Current view: ' + view.name);
            // change the day's background color just for fun
            //$(this).css('background-color', 'orange');

        },  
        timezone: 'local',
        ignoreTimezone: false,
        events: function (start, end, timezone, callback) {
            var thestart = start.toISOString();
            $.ajax({
                type: "POST",
                url: '/Services/UserService.asmx/GetCalendarEvents',
                data: JSON.stringify({ 'startdate': thestart, 'pageType' : pageType, 'pageID' : pageID }),
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var events = [];
                    $(data.d).each(function () {
                        var thestart = top.Ts.Utils.getMsDate(parseJsonDate(this.start));
                        var test = thestart.localeFormat();
                        var test2 = new Date(thestart);
                        events.push({
                            title: this.title,
                            start: this.start,
                            textColor: this.color,
                            color: '#fff',
                            type: this.type,
                            id: this.id,
                            description: this.description
                            //end: this.end
                        });
                    });
                    callback(events);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('There was an error loading calendar events');
                }
            });
        },
        eventDrop: function (event, delta, revertFunc) {
            top.Ts.Services.Users.ChangeEventDate(event.id, event.start.format(), event.type)

        },
        eventRender: function (event, element) {
            element.popover({
                title: event.title,
                placement:'auto',
                html:true,
                trigger : 'click',
                animation: 'true',
                content: buildPopover(event, element),
                container: 'body'
        });
        $('body').on('click', function (e) {
            if (!element.is(e.target) && element.has(e.target).length === 0 && $('.popover').has(e.target).length === 0)
                element.popover('hide');
        });
        
    },

    });


    //Convert buttons to bootstrap classes
    $('.fc-toolbar').find('.fc-button').addClass('btn btn-default');

    //build popover content
    function buildPopover(event, element)
    {
        var theTime = $("<div>")
            .text(moment(event.start).format('MM/DD/YYYY hh:mm'));
        var lb = $("<hr>")
            .appendTo(theTime);
        var goButton = $("<button>")
            .addClass("btn btn-sm")
            .click(function (e) {
                alert("test");
                //if (event.type == "ticket") {
                //    top.Ts.MainPage.openTicket(event.id);
                //}
                //else if (event.type == "reminder-org")
                //{
                //    //top.Ts.MainPage.openNewCustomer(event.id);
                    
                //}
                //else if (event.type == "reminder-user") {
                //    //top.Ts.MainPage.openNewContact(event.id);
                //}
                //else if (event.type == "cal")
                //{
                //    element.popover('hide');
                //    clearModal();
                //    $('#inputTitle').val(event.title);
                //    $('#inputStartTime').val(moment(event.start).format('MM/DD/YYYY hh:mm'));
                //    $('#inputEndTime').val("");
                //    $('#inputDescription').val(event.description);
                //    $('#fullCalModal').modal('show');
                //}
            })
            .appendTo(theTime);
        var icon = $("<i>")
            .addClass("fa fa-arrow-up")
            .appendTo(goButton);

        if (top.Ts.System.User.IsSystemAdmin && event.type == "cal")
        {
            var delButton = $("<button>")
                .addClass("btn btn-sm pull-right")
                .click(function (e) {
                    if (confirm("Are you sure you want to delete this event"))
                    {
                        top.Ts.Services.Users.DeleteCalEvent(event.id, function () {
                            element.popover('hide');
                            $("#calendar").fullCalendar('refetchEvents');
                        });
                    }
                })
            .appendTo(theTime);
            var icon = $("<i>")
                .addClass("fa fa-trash-o")
                .appendTo(delButton);
        }

        return theTime;
    }

    //default setup functions
    $('.datetimepicker').datetimepicker({ format: 'MM/DD/YYYY hh:mm' });
    $('#searchGroup').hide();

    //handle the event association dropdown
    $('#calAssociation').change(function () {
        var searchbox = $('#associationSearch');
        switch (this.value)
        {
            case "-1":
                $('#searchGroup').hide();
                break;
            case "0":
                $('#searchGroup').show();
                $('#associationSearch').attr("placeholder", "Search Tickets").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2, source: getTicketsByTerm, delay: 300,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Ticket')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.ticket-queue')).data('Ticket', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value,20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "1":
                $('#searchGroup').show();
                $('#associationSearch').attr("placeholder", "Search Users").val("");
                searchbox.autocomplete({
                    minLength: 3, source: getUsers, delay: 300,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('User')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                            .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                            .appendTo($('#associationQueue').find('.user-queue')).data('User', ui.item.id);


                                $('<span>')
                            .text(ellipseString(ui.item.value, 20))
                            .addClass('filename')
                            .appendTo(bg);

                                $('<span>')
                            .addClass('ui-icon ui-icon-close')
                            .click(function (e) {
                                e.preventDefault();
                                $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                            })
                            .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "2":
                $('#searchGroup').show();
                $('#associationSearch').attr("placeholder", "Search Companies").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getCustomers,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Company')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.customer-queue')).data('Company', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "3":
                $('#searchGroup').show();
                $('#associationSearch').attr("placeholder", "Search Groups").val("");
                $('#associationSearch').autocomplete({
                    minLength: 2,
                    source: getGroupsByTerm,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Group')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.group-queue')).data('Group', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
            case "4":
                $('#searchGroup').show();
                $('#associationSearch').attr("placeholder", "Search Products").val("");
                $('#associationSearch').autocomplete({
                    minLength: 3,
                    source: getProductByTerm,
                    select: function (event, ui) {
                        if (ui.item) {
                            var isDupe;
                            $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
                                if (ui.item.id == $(this).data('Product')) {
                                    isDupe = true;
                                }
                            });
                            if (!isDupe) {
                                var bg = $('<div>')
                                .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                                .appendTo($('#associationQueue').find('.product-queue')).data('Product', ui.item.id);


                                $('<span>')
                                .text(ellipseString(ui.item.value, 20))
                                .addClass('filename')
                                .appendTo(bg);

                                $('<span>')
                                .addClass('ui-icon ui-icon-close')
                                .click(function (e) {
                                    e.preventDefault();
                                    $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                                })
                                .appendTo(bg);
                                this.value = "";
                                return false;
                            }
                        }
                    }
                });
                break;
        }
    });

    //save the new calendar event
    $('#btnSaveEvent').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var calendarinfo = new Object();

        calendarinfo.title = $('#inputTitle').val();
        calendarinfo.start = $('#inputStartTime').val();
        calendarinfo.end = $('#inputEndTime').val();
        calendarinfo.description = $('#inputDescription').val();
        calendarinfo.PageType = pageType;
        calendarinfo.PageID = pageID;

        calendarinfo.Tickets = new Array();
        $('#associationQueue').find('.ticket-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Tickets[calendarinfo.Tickets.length] = $(this).data('Ticket');
        });

        calendarinfo.Groups = new Array();
        $('#associationQueue').find('.group-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Groups[calendarinfo.Groups.length] = $(this).data('Group');
        });

        calendarinfo.Products = new Array();
        $('#associationQueue').find('.product-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Products[calendarinfo.Products.length] = $(this).data('Product');
        });

        calendarinfo.Company = new Array();
        $('#associationQueue').find('.customer-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.Company[calendarinfo.Company.length] = $(this).data('Company');
        });

        calendarinfo.User = new Array();
        $('#associationQueue').find('.user-queue').find('.ticket-removable-item').each(function () {
            calendarinfo.User[calendarinfo.User.length] = $(this).data('User');
        });

        if (calendarinfo.Tickets.length > 0) top.Ts.System.logAction('Calendar Event - Ticket Inserted');
        if (calendarinfo.Groups.length > 0) top.Ts.System.logAction('Calendar Event - Group Inserted');
        if (calendarinfo.Products.length > 0) top.Ts.System.logAction('Calendar Event - Product Inserted');
        if (calendarinfo.Company.length > 0) top.Ts.System.logAction('Calendar Event - Company Inserted');
        if (calendarinfo.User.length > 0) top.Ts.System.logAction('Calendar Event - User Inserted');

        top.Ts.Services.Users.SaveCalendarEvent(top.JSON.stringify(calendarinfo), function () {
            $('#fullCalModal').modal('hide');
            clearModal();
            $("#calendar").fullCalendar('refetchEvents');
        });


    });


    //search functions for the associations
    var execGetCustomer = null;
    function getCustomers(request, response) {
        if (execGetCustomer) { execGetCustomer._executor.abort(); }
        execGetCustomer = top.Ts.Services.Organizations.WCSearchOrganization(request.term, function (result) {
            response(result);
        });
    }

    var execGetUsers = null;
    function getUsers(request, response) {
        if (execGetUsers) { execGetUsers._executor.abort(); }
        execGetUsers = top.Ts.Services.Users.SearchUsers(request.term, function (result) { response(result); });
    }

    var execGetTicket = null;
    function getTicketsByTerm(request, response) {
        if (execGetTicket) { execGetTicket._executor.abort(); }
        //execGetTicket = Ts.Services.Tickets.GetTicketsByTerm(request.term, function (result) { response(result); });
        execGetTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) {
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            response(result);
        });

    }

    var execGetGroups = null;
    function getGroupsByTerm(request, response) {
        if (execGetGroups) { execGetGroups._executor.abort(); }
        execGetTicket = top.Ts.Services.WaterCooler.GetGroupsByTerm(request.term, function (result) { response(result); });
    }

    var execGetProducts = null;
    function getProductByTerm(request, response) {
        if (execGetProducts) { execGetProducts._executor.abort(); }
        execGetProducts = top.Ts.Services.WaterCooler.GetProductsByTerm(request.term, function (result) { response(result); });
    }

    //text shortening
    function ellipseString(text, max) {
        return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text;
    };

    //json date parser
    function parseJsonDate(jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    }

    //clears the modal
    function clearModal()
    {
        $('#inputTitle').val("");
        $('#inputStartTime').val("");
        $('#inputEndTime').val("");
        $('#inputDescription').val("");
        $('#calAssociation').val("-1");
        $('#associationQueue').find('.upload-queue').empty();
        $('#associationQueue').find('.ticket-queue').empty();
        $('#associationQueue').find('.group-queue').empty();
        $('#associationQueue').find('.customer-queue').empty();
        $('#associationQueue').find('.user-queue').empty();
        $('#associationQueue').find('.product-queue').empty();
    }

}); 