/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

$(document).ready(function () {
    var pageType = top.Ts.Utils.getQueryValue("pagetype", window);
    var pageID = top.Ts.Utils.getQueryValue("pageid", window);

    if (pageType == null)
        pageType = -1;

    if (pageID == null)
        pageID = -1;

    if (pageType != -1) {
        $('#leftcontent').removeClass();
        $('#leftcontent').addClass("span12");
        $('#sidebar').remove();
    }

    $(window).focus(function () {
        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(false);
        newMsg = 1;
        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setCaption("Water Cooler");
    });

    // delete link event
    $('.wc-threads').delegate('.wc-delete-link', 'click', function (e) {
        var parent = $(this).closest('.wc-message');
        var message = parent.data('message');
        top.Ts.Services.WaterCooler.DeleteMessage(message.MessageID, function (result) {
            if (result == true)
                parent.remove();
            window.top.chatHubClient.deleteMessage(message.MessageID);
        });

    });

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

    $('.user-search').autocomplete({
        minLength: 4,
        source: getUsers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('User')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.user-queue')).data('User', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.company-search').autocomplete({
        minLength: 4,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Company')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.customer-queue')).data('Company', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
            //.next().show();
        }
    });

    $('.main-quick-ticket').autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.ticket-queue')).data('Ticket', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

    $('.insert-ticket').autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                $('#messagecontents').val($('#messagecontents').val() + " &ticket" + ui.item.id);
            }
            $('.insert-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

    $('.group-search').autocomplete({
        minLength: 2,
        source: getGroupsByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Group')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.group-queue')).data('Group', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.product-search').autocomplete({
        minLength: 4,
        source: getProductByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Product')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.product-queue')).data('Product', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

    $('.user-search')
    .focusin(function () { $(this).val('').removeClass('user-search-blur'); })
    .focusout(function () { $(this).val('Search for a user...').addClass('user-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('user-search-blur'); })
    .val('Search for a user...');

    $('.company-search')
    .focusin(function () { $(this).val('').removeClass('company-search-blur'); })
    .focusout(function () { $(this).val('Search for a company...').addClass('company-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('company-search-blur'); })
    .val('Search for a company...');

    $('.main-quick-ticket')
    .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
    .val('Search for a ticket...');

    $('.insert-ticket')
    .focusin(function () { $(this).val('').removeClass('insert-ticket-blur'); })
    .focusout(function () { $(this).val('insert ticket link...').addClass('insert-ticket-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('insert-ticket-blur'); })
    .val('insert ticket link...');

    $('.group-search')
    .focusin(function () { $(this).val('').removeClass('group-search-blur'); })
    .focusout(function () { $(this).val('Search for a group...').addClass('group-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('group-search-blur'); })
    .val('Search for a group...');

    $('.product-search')
    .focusin(function () { $(this).val('').removeClass('product-search-blur'); })
    .focusout(function () { $(this).val('Search for a product...').addClass('product-search-blur').removeClass('ui-autocomplete-loading'); })
    .click(function () { $(this).val('').removeClass('product-search-blur'); })
    .val('Search for a product...');

    top.Ts.Services.WaterCooler.GetOnlineChatUsers(top.Ts.System.User.OrganizationID, function (users) {
        var name;
        var chatID;
        for (var i = 0; i < users.length; i++) {
            name = users[i].FirstName + ' ' + users[i].LastName;
            chatID = users[i].AppChatID;
            var onlineuser = $('<li>')
        .data('ChatID', chatID)
        .data('Name', name)
        .addClass('onlineUser')
        .click(function () {
            window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
        })
        .html('<a class="ui-state-default ts-link" href="#"><i class="icon-user"></i>' + users[i].FirstName + ' ' + users[i].LastName + '</a>')
        .appendTo($('.sidebarusers'));
        }
    });

    top.Ts.Services.Users.GetUserPhoto(-99, function (att) {
        $('.mainavatarlrg').attr("src", att);
    });

    //Gets the top 10 threads on initial page load
    top.Ts.Services.WaterCooler.GetThreads(pageType, pageID, function (threads) {
        var threadContainer = $('#maincontainer');
        if (threads.length > 0) {
            for (var i = 0; i < threads.length; i++) {
                var div = createThread(threads[i]);
                threadContainer.append(div);
            }
        }
        else {
            var placeholder = $('<div>')
            .addClass("placeholder");

            switch(pageType)
            {
            case -1:
                placeholder.html('The WaterCooler allows you to have conversations with colleagues about tickets, customers, and even products - Collaborate with your entire team to better support your customers!');
                break;
            case "0":
                placeholder.html('Start a conversation about this ticket in the WaterCooler.  If you have questions or comments that are for a broader audience than the ticket actions allow, just post a comment here and you co-workers will see it in their WaterCooler.');
                break;
            case "1":
                placeholder.html('Share information or ask questions about products through the WaterCooler!');
                break;
            case "2":
                placeholder.html('Share information or ask questions about customers through the WaterCooler!');
                break;
            case "4":
                placeholder.html('Have conversation just with the people in this specific group through the WaterCooler.  Only members of this group will be able to see the conversations.');
            }

            $('#maincontainer').append(placeholder);
        }

        $('.loading-section').hide().next().show();
    });

    // set up the refresh button so we can just click that to see our dev changes
    $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; }).toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    //$('span').tooltip('show');
    $("input[type=text], textarea").autoGrow();

    $('#newcomment').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        if ($('#messagecontents').val().length > 0) {
            var commentinfo = new Object();
            commentinfo.Description = $('#messagecontents').val();
            commentinfo.Attachments = new Array();
            commentinfo.ParentTicketID = -1;

            commentinfo.PageType = pageType;
            commentinfo.PageID = pageID;

            commentinfo.Tickets = new Array();
            $('#commentatt:first').find('.ticket-queue').find('.ticket-removable-item').each(function () {
                commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
            });

            commentinfo.Groups = new Array();
            $('#commentatt:first').find('.group-queue').find('.ticket-removable-item').each(function () {
                commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
            });

            commentinfo.Products = new Array();
            $('#commentatt:first').find('.product-queue').find('.ticket-removable-item').each(function () {
                commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
            });

            commentinfo.Company = new Array();
            $('#commentatt:first').find('.customer-queue').find('.ticket-removable-item').each(function () {
                commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
            });

            commentinfo.User = new Array();
            $('#commentatt:first').find('.user-queue').find('.ticket-removable-item').each(function () {
                commentinfo.User[commentinfo.User.length] = $(this).data('User');
            });

            top.Ts.Services.WaterCooler.NewComment(top.JSON.stringify(commentinfo), function (MessageID) {
                if ($('.postcontainer').find('.upload-queue div').length > 0) {
                    $('.postcontainer').find('.upload-queue div').each(function (i, o) {
                        var data = $(o).data('data');
                        data.url = '../../../Upload/WaterCooler/' + MessageID;
                        data.jqXHR = data.submit();
                        $(o).data('data', data);
                    });
                }
                window.top.chatHubClient.newThread(MessageID);
                $('.commentcontainer').hide();
                $('.faketextcontainer').show();
                $('#messagecontents').val('');
                resetDisplay();
            });
            $('.frame-content').animate({ scrollTop: 0 }, 600);
        }
    });
    $('.faketext').click(function (e) {
        $(this).parent().hide();
        var comment = $(this).parent().parent().find('.commentcontainer').show();
        comment.find('#messagecontents').focus();
    });
    $('.faketextcontainer').click(function (e) {
        $(this).hide();
        $(this).parent().find('.commentcontainer').show();
        $(this).parent().find('.commentcontainer').find(".ticketcontainer").hide();
        $(this).parent().find('.commentcontainer').find(".groupcontainer").hide();
        //$(this).parent().find('.commentcontainer').find(".attcontainer").hide();
        $(this).parent().find("#ticketinput").hide();
        $(this).parent().find("#groupinput").hide();
        $(this).parent().find("#customerinput").hide();
        $(this).parent().find("#productinput").hide();
        $(this).parent().find("#userinput").hide();
        $(this).parent().find("#attachmentinput").show();
        $(this).parent().find("#ticketinsert").show();
        $(this).parent().find(".arrow-up").css('left', '7px');
        $(this).parent().find('#messagecontents').focus();
    });
    $(document).click(function (e) {
        if ($(e.target).is('.faketextcontainer, .commentcontainer *, .faketext, .ui-autocomplete *, ui-combobox *')) return;
        $('.commentcontainer').hide();
        $('.faketextcontainer').show();
        //resetDisplay();
    });
    $('.addatt').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").show();
        $(this).parent().parent().find("#ticketinsert").show();
        $(this).parent().find(".arrow-up").css('left', '7px');
    })
    .tooltip();
    $('.addticket').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").show();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '30px');
    })
    .tooltip();
    $('.adduser').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").show();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '53px');
    })
    .tooltip();
    $('.addcustomer').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").show();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '78px');
    })
    .tooltip();
    $('.addgroup').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").show();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").hide();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '104px');
    })
    .tooltip();
    $('.addproduct').click(function (e) {
        e.preventDefault();
        $(this).parent().parent().find("#ticketinput").hide();
        $(this).parent().parent().find("#groupinput").hide();
        $(this).parent().parent().find("#customerinput").hide();
        $(this).parent().parent().find("#productinput").show();
        $(this).parent().parent().find("#userinput").hide();
        $(this).parent().parent().find("#attachmentinput").hide();
        $(this).parent().parent().find("#ticketinsert").hide();
        $(this).parent().find(".arrow-up").css('left', '125px');

    })
    .tooltip();
    $('.file-upload').fileupload({
        namespace: 'new_ticket',
        dropZone: $('.file-upload'),
        add: function (e, data) {
            for (var i = 0; i < data.files.length; i++) {

                var bg = $('<div>')
          .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
          .appendTo($(this).parent().parent().find('.upload-queue'));

                data.context = bg;
                bg.data('data', data);

                $('<span>')
          .text(ellipseString(data.files[i].name, 20) + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
          .addClass('filename')
          .appendTo(bg);

                //                $('<div>')
                //          .addClass('progress')
                //          .hide()
                //          .appendTo(bg);

                $('<span>')
          .addClass('ui-icon ui-icon-close')
          .click(function (e) {
              e.preventDefault();
              $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
          })
          .appendTo(bg);

                $('<span>')
          .addClass('ui-icon ui-icon-cancel')
          .hide()
          .click(function (e) {
              e.preventDefault();
              var data = $(this).closest('li').data('data');
              data.jqXHR.abort();
          })
          .appendTo(bg);
            }

        },
        send: function (e, data) {
            if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                //data.context.find('.progress').progressbar('value', 50);
            }
        },
        fail: function (e, data) {
            if (data.errorThrown === 'abort') return;
            alert('There was an error uploading "' + data.files[0].name + '".');
        },
        progress: function (e, data) {
            //data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
        },
        start: function (e, data) {
            //$(this).parent().parent().find('.progress').progressbar().show();
            //$(this).parent().parent().find('.upload-queue .ui-icon-close').hide();
            //$(this).parent().parent().find('.upload-queue .ui-icon-cancel').show();
        },
        stop: function (e, data) {
            $(this).parent().parent().find('.progress').progressbar('value', 100);
        }
    });
    $('.testbtn').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var firstpost = $('#maincontainer').find('.postcontainer:first');
        for (var i = 0; i < 5; i++) {
            firstpost.after(i).fadeIn(3000);
        }

        // insert the threads into the div

        $('.loading-section').hide().next().show();

    });
    $('.ui-autocomplete-input').css('width', '200px');
    $('a').addClass('ui-state-default ts-link');

    $('.frame-content').bind('scroll', function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            top.Ts.Services.WaterCooler.GetMoreThreads(pageType, pageID, $('#maincontainer').find('.topic_container').length, function (newthreads) {
                var threadContainer = $('#maincontainer');

                for (var i = 0; i < newthreads.length; i++) {
                    var div1 = createThread(newthreads[i]);
                    threadContainer.append(div1);
                }
            });

        }

        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').click(function () {
        $('.frame-content').animate({ scrollTop: 0 }, 600);
        return false;
    });

});

jQuery(document).ready(function () {
    jQuery("span.timeago").timeago();
});

jQuery.fn.autoGrow = function () {
    return this.each(function () {
        // Variables
        var colsDefault = 130; //this.cols;
        var rowsDefault = this.rows;

        //Functions
        var grow = function () {
            growByRef(this);
        }

        var growByRef = function (obj) {
            var linesCount = 0;
            var lines = obj.value.split('\n');

            for (var i = lines.length - 1; i >= 0; --i) {
                linesCount += Math.floor((lines[i].length / colsDefault) + 1);
            }

            if (linesCount > rowsDefault)
                obj.rows = linesCount + 1;
            else
                obj.rows = rowsDefault;
        }

        var characterWidth = function (obj) {
            var characterWidth = 0;
            var temp1 = 0;
            var temp2 = 0;
            var tempCols = obj.cols;

            obj.cols = 1;
            temp1 = obj.offsetWidth;
            obj.cols = 2;
            temp2 = obj.offsetWidth;
            characterWidth = temp2 - temp1;
            obj.cols = tempCols;

            return characterWidth;
        }

        // Manipulations
        //this.style.width = "auto";
        this.style.height = "auto";
        this.style.overflow = "hidden";
        //this.style.width = ((characterWidth(this) * this.cols) + 6) + "px";
        this.onkeyup = grow;
        this.onfocus = grow;
        this.onblur = grow;
        growByRef(this);
    });
};

this.imagePreview = function () {
    /* CONFIG */

    xOffset = 10;
    yOffset = 30;

    // these 2 variable determine popup's distance from the cursor
    // you might want to adjust to get the right result

    /* END CONFIG */
    $("a.preview").hover(function (e) {
        this.t = this.title;
        this.title = "";
        var c = (this.t != "") ? "<br/>" + this.t : "";
        $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' />" + c + "</p>");
        $("#preview")
			.css("top", (e.pageY - xOffset) + "px")
			.css("left", (e.pageX + yOffset) + "px")
			.fadeIn("fast");
    },
	function () {
	    this.title = this.t;
	    $("#preview").remove();
	});
    $("a.preview").mousemove(function (e) {
        $("#preview")
			.css("top", (e.pageY - xOffset) + "px")
			.css("left", (e.pageX + yOffset) + "px");
    });
};

function teststring(str) {
    $("#messages").append('<li>' + str + '</li>');
}

var pageType = top.Ts.Utils.getQueryValue("pagetype", window);
var pageID = top.Ts.Utils.getQueryValue("pageid", window);
var newMsg = 1;

if (pageType == null)
    pageType = -1;

if (pageID == null)
    pageID = -1;

function placeHolder() {
    var topics = $('#maincontainer').find('.topic_container');

    if (topics.length > 0) {
        $('#maincontainer').find('.placeholder').remove();
    }
    else {

        var placeholder = $('<div>')
            .addClass("placeholder");

        switch (pageType) {
            case -1:
                placeholder.html('The WaterCooler allows you to have conversations with colleagues about tickets, customers, and even products - Collaborate with your entire team to better support your customers!');
                break;
            case "0":
                placeholder.html('Start a conversation about this ticket in the WaterCooler.  If you have questions or comments that are for a broader audience than the ticket actions allow, just post a comment here and you co-workers will see it in their WaterCooler.');
                break;
            case "1":
                placeholder.html('Share information or ask questions about products through the WaterCooler!');
                break;
            case "2":
                placeholder.html('Share information or ask questions about customers through the WaterCooler!');
                break;
            case "4":
                placeholder.html('Have conversation just with the people in this specific group through the WaterCooler.  Only members of this group will be able to see the conversations.');
        }

        $('#maincontainer').append(placeholder);
    }

};


function addThread(message) {
        var firstpost = $('#maincontainer');

        top.Ts.Services.WaterCooler.IsValid(pageType, pageID, message.Message.MessageID, function (valid) {
            if (valid) {
                var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + message.Message.MessageID + ')');
                if (parentThread.length == 0) {
                    var nmdiv = createThread(message);
                    firstpost.prepend(nmdiv.fadeIn(1500));
                    if (message.Message.UserID != top.Ts.System.User.UserID) {
                        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(true);
                        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setCaption("Water Cooler (" + newMsg++ + ")");
                    }

                }
                placeHolder();
            }

        });

    };

function addComment (message) {
        var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + message.Message.MessageParent + ')');
        var commentcount = $(parentThread).find('.treplycontainer').length + 1;
        var comments = $(parentThread).find('.treplycontainer');
        var firstpost = $('#maincontainer');
        top.Ts.Services.WaterCooler.IsValid(pageType, pageID, message.Message.MessageParent, function (valid) {
            if (valid) {
                if (parentThread) {
                    //If over 6 comments compact it
                    if (commentcount > 6) {
                        var topicComments = parentThread.find('.treplycontainer:first').find('.topicComments');

                        if (topicComments.length > 0) {
                            topicComments.html(commentcount - 1 + " comments");
                            var moverply = $(parentThread).find('.treplycontainer').get(commentcount - 3);
                            $(moverply).appendTo(parentThread.find('.hiddenComments'));

                        }
                        else {
                            var movedivs = $(parentThread).find('.treplycontainer').slice(0, commentcount - 2);

                            var trepCont = $('<div>')
                            .addClass('treplycontainer')
                            .insertAfter(parentThread.find('.topictext:first'));

                            var tpcommenttxt = $('<div>')
                            .addClass('topicComments slim')
                            .html(commentcount + " comments")
                            .click(function (e) {
                                e.preventDefault();
                                $(this).parent().parent().find('.hiddenComments').slideToggle("fast");
                            })
                            .appendTo(trepCont);

                            var hidden = $('<div>')
                            .addClass('hiddenComments')
                            .insertAfter(trepCont);


                            $(movedivs).appendTo(hidden);
                        }

                    }

                    var replydiv = createReply(message.Message);
                    var lastreply = $(parentThread).find('.treplycontainer:last');
                    if (lastreply.length > 0) {
                        lastreply.after(replydiv.fadeIn(1500));
                    }
                    else {
                        $(parentThread).find('.topictext').after(replydiv.fadeIn(1500));
                    }

                    if (message.Message.UserID != top.Ts.System.User.UserID) {
                        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setIsHighlighted(true);
                        top.Ts.MainPage.MainMenu.find('mniWC2', 'wc2').setCaption("Water Cooler (" + newMsg++ + ")");
                    }

                    firstpost.prepend(parentThread.fadeIn(1500));
                }
            }
            else {
                parentThread.hide();
            }
            placeHolder();
        });


    };

function deleteMessage (messageID, parentID) {
        if (parentID == -1) {
            var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + messageID + ')');
            parentThread.remove();
        } else {
            var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + parentID + ')');
            parentThread.find('.treplycontainer:data(MessageID=' + messageID + ')').remove();
        }
        placeHolder();
    }

function updateLikes (likes, messageID, messageParentID) {
        if (messageParentID == -1) {

            var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + messageID + ')');
        }
        else {
            var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + messageParentID + ')').find('.treplycontainer:data(MessageID=' + messageID + ')');
        }

        var likeUsers = '';
        for (var i = 0; i < likes.length; i++) {
            if (likes[i].UserID == top.Ts.System.User.UserID)
                likeUsers += "You<br/>";
            else
                likeUsers += likes[i].UserName + "<br/>";
        }

        parentThread.find('.likestar:first').attr('title', likeUsers)
        .tipTip({ defaultPosition: "top", edgeOffset: 7 });

        parentThread.find('.likeCounter:first').text(likes.length);

        //        $('<span>')
        //                    .addClass('likeCounter someClass')
        //                    .attr('id', 'likeCounter')
        //                    .text(likes.length)
        //                    .appendTo(parent);
    };

function updateattachments (message) {
        var parentThread = $('#maincontainer').find('.topic_container:data(MessageID=' + message.Message.MessageID + ')');
        var tixHasAtt = false;
        parentThread.find('.topicHistory').remove();

        var tixatt = message.Tickets;
        var tixattstr = "";
        if (tixatt.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixatt.length; i++) {
                tixattstr = tixattstr + ' ' + tixatt[i].CreatorName + ' added ticket <a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + tixatt[i].AttachmentID + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + tixatt[i].AttachmentID + '); return false;">' + tixatt[i].TicketName + '</a><br/>';
            }
        }

        var tixgrp = message.Groups;
        var tixgrpstr = "";
        if (tixgrp.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixgrp.length; i++) {
                tixgrpstr = tixgrpstr + ' ' + tixgrp[i].CreatorName + ' added group ' + tixgrp[i].GroupName + '<br/>';
            }
        }

        var tixprod = message.Products;
        var tixprodstr = "";
        if (tixprod.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixprod.length; i++) {
                tixprodstr = tixprodstr + ' ' + tixprod[i].CreatorName + ' added product <a href="#" target="_blank" onclick="top.Ts.MainPage.openProduct(' + tixprod[i].AttachmentID + '); return false;">' + tixprod[i].ProductName + '</a><br/>';
            }
        }

        var tixcompany = message.Company;
        var tixcompanystr = "";
        if (tixcompany.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixcompany.length; i++) {
                tixcompanystr = tixcompanystr + ' ' + tixcompany[i].CreatorName + ' added company <a href="#" target="_blank" onclick="top.Ts.MainPage.openCustomer(' + tixcompany[i].AttachmentID + '); return false;">' + tixcompany[i].CompanyName + '</a><br/>';
            }
        }

        var tixuser = message.User;
        var tixuserstr = "";
        if (tixuser.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixuser.length; i++) {
                tixuserstr = tixuserstr + ' ' + tixuser[i].CreatorName + ' added user <a href="#" target="_blank" onclick="top.Ts.MainPage.openUser(' + tixuser[i].AttachmentID + '); return false;">' + tixuser[i].UserName + '</a><br/>';
            }
        }

        if (tixHasAtt == true) {
            $('<span>')
            .addClass('topicHistory someClass')
            .attr('title', tixattstr + tixgrpstr + tixprodstr + tixcompanystr + tixuserstr)
            .tipTip({ defaultPosition: "top", edgeOffset: 7, keepAlive: true })
            .prependTo(parentThread.find('.topicAttachments'));

        }
    };

function disconnect (windowid) {
        if (pageType == -1) {
            $('.sidebarusers').find('.onlineUser:data(ChatID=' + windowid + ')').remove();
        }
        //chatAddMsg(windowid, "User is currently offline", "system");
    };

function updateUsers () {
        if (pageType == -1) {
            top.Ts.Services.WaterCooler.GetOnlineChatUsers(top.Ts.System.User.OrganizationID, function (users) {
                var name;
                var chatID;
                for (var i = 0; i < users.length; i++) {
                    name = users[i].FirstName + ' ' + users[i].LastName;
                    chatID = users[i].AppChatID;

                    var user = $('.sidebarusers').find('.onlineUser:data(ChatID=' + chatID + ')');

                    if (user.length > 0) {
                        user.data('ChatID', chatID);
                    }
                    else {
                        var onlineuser = $('<li>')
                    .data('ChatID', chatID)
                    .data('Name', name)
                    .addClass('onlineUser')
                    .click(function () {
                        window.parent.openChat($(this).data('Name'), $(this).data('ChatID'));
                    })
                    .html('<a class="ui-state-default ts-link" href="#"><i class="icon-user"></i>' + users[i].FirstName + ' ' + users[i].LastName + '</a>')
                    .appendTo($('.sidebarusers'));
                    }
                }
            });
        }
    };

function resetDisplay() {
        $('#commentatt').find('.upload-queue').empty();
        $('#commentatt').find('.ticket-queue').empty();
        $('#commentatt').find('.group-queue').empty();
        $('#commentatt').find('.customer-queue').empty();
        $('#commentatt').find('.user-queue').empty();
        $('#commentatt').find('.product-queue').empty();
        $(".newticket-group").val(-1);
        $(".newticket-product").val(-1);
    }

function createThread(thread) {
        var tc = $('<div>')
            .addClass('topic_container')
            .data('MessageID', thread.Message.MessageID);

        var tp = $('<div>')
            .addClass('topic')
            .appendTo(tc);

        var ta = $('<div>')
            .addClass('topicavatar')
            .appendTo(tp);

        var avaimg = $('<img>')
            .addClass('topicavatarlrg')
            .appendTo(ta);

        top.Ts.Services.Users.GetUserPhoto(thread.Message.UserID, function (att) {
            avaimg.attr("src", att);
        });

        var tpic = $('<div>')
            .addClass('tpic')
            .appendTo(tp);

        var dv = $('<div>')
            .hover(function () {
                $(this).find('.close').show();
            }, function () {
                $(this).find('.close').hide();
            })
            .appendTo(tpic);

        var sptpic = $('<span>')
            .addClass('topicln')
            .appendTo(dv);

        var namea = $('<a>')
            .addClass('ts-linkb ts-link ui-state-default')
            .attr('href', '#')
            .text(thread.Message.UserName)
            .click(function (e) {
                e.preventDefault();
                top.Ts.MainPage.openUser(thread.Message.UserID);
            })
            .appendTo(sptpic);

        var sptm = $('<span>')
            .addClass('topictm timeago')
            .attr('title', thread.Message.TimeStamp)
            .text(jQuery.timeago(thread.Message.TimeStamp))
            .timeago()
            .appendTo(dv);

        var splike = $('<span>')
            .attr('id', 'spnlike')
            .appendTo(dv);

        top.Ts.Services.WaterCooler.GetLikes(thread.Message.MessageID, function (likes) {
            var likeUsers = '';
            var userLike = false;

            for (var i = 0; i < likes.length; i++) {
                if (likes[i].UserID == top.Ts.System.User.UserID) {
                    likeUsers += "You<br/>";
                    userLike = true;
                }
                else
                    likeUsers += likes[i].UserName + "<br/>";
            }

            var star = $('<img>')
            .addClass('likestar')
            .attr('src', '../images/icons/star_icon.png')
            .attr('title', likeUsers)
            .tipTip({ defaultPosition: "top", edgeOffset: 7 })
            .appendTo(splike);

            $('<span>')
                .addClass('likeCounter someClass')
                .attr('id', 'likeCounter')
                .text(likes.length)
                .appendTo(splike);


            if (userLike == false) {
                var splikelink = $('<a>')
            .addClass('topiclike ts-link ui-state-default')
            .text("like")
            .click(function (e) {
                e.preventDefault();
                var parent = $(this).parent();
                //parent.find('#likeCounter').remove();
                $(this).hide();
                top.Ts.Services.WaterCooler.AddCommentLike(thread.Message.MessageID, function (likes) {
                    window.top.chatHubClient.addLike(likes, thread.Message.MessageID, thread.Message.MessageParent);
                });

            })
            .appendTo(splike);
            }
        });


        var tixatt = thread.Tickets;
        var tixattstr = "";
        var tixHasAtt = false;
        if (tixatt.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixatt.length; i++) {
                tixattstr = tixattstr + ' ' + tixatt[i].CreatorName + ' added ticket <a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + tixatt[i].AttachmentID + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + tixatt[i].AttachmentID + '); return false;">' + tixatt[i].TicketName + '</a><br/>';
            }
        }

        var tixgrp = thread.Groups;
        var tixgrpstr = "";
        if (tixgrp.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixgrp.length; i++) {
                tixgrpstr = tixgrpstr + ' ' + tixgrp[i].CreatorName + ' added group <a href="#" target="_blank" onclick="top.Ts.MainPage.openGroup(' + tixgrp[i].AttachmentID + '); return false;">' + tixgrp[i].GroupName + '</a><br/>';
                //tixgrpstr = tixgrpstr + ' ' + tixgrp[i].CreatorName + ' added group ' + tixgrp[i].GroupName + '<br/>';
            }
        }

        var tixprod = thread.Products;
        var tixprodstr = "";
        if (tixprod.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixprod.length; i++) {
                tixprodstr = tixprodstr + ' ' + tixprod[i].CreatorName + ' added product <a href="#" target="_blank" onclick="top.Ts.MainPage.openProduct(' + tixprod[i].AttachmentID + '); return false;">' + tixprod[i].ProductName + '</a><br/>';
            }
        }

        var tixcompany = thread.Company;
        var tixcompanystr = "";
        if (tixcompany.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixcompany.length; i++) {
                tixcompanystr = tixcompanystr + ' ' + tixcompany[i].CreatorName + ' added company <a href="#" target="_blank" onclick="top.Ts.MainPage.openCustomer(' + tixcompany[i].AttachmentID + '); return false;">' + tixcompany[i].CompanyName + '</a><br/>';
            }
        }

        var tixuser = thread.User;
        var tixuserstr = "";
        if (tixuser.length > 0) {
            tixHasAtt = true;
            for (var i = 0; i < tixuser.length; i++) {
                tixuserstr = tixuserstr + ' ' + tixuser[i].CreatorName + ' added user <a href="#" target="_blank" onclick="top.Ts.MainPage.openUser(' + tixuser[i].AttachmentID + '); return false;">' + tixuser[i].UserName + '</a><br/>';
                //tixuserstr = tixuserstr + ' ' + tixuser[i].CreatorName + ' added user ' + tixuser[i].UserName + '<br/>';
            }
        }

        var sptprel = $('<span>')
            .addClass('topicrel topicAttachments')
            .appendTo(dv);

        if (tixHasAtt == true) {
            $('<span>')
            .addClass('topicHistory someClass')
            .attr('title', tixattstr + tixgrpstr + tixprodstr + tixcompanystr + tixuserstr)
            .tipTip({ defaultPosition: "top", edgeOffset: 7, keepAlive: true })
            .appendTo(sptprel);

        }

        var canEdit = top.Ts.System.User.UserID == thread.Message.UserID || top.Ts.System.User.IsSystemAdmin;

        if (canEdit) {
            var spdelete = $('<span>')
            .addClass('topicrel close')
            .click(function (e) {
                if (confirm('Are you sure you would like to remove this post?')) {
                    top.Ts.Services.WaterCooler.DeleteMessage(thread.Message.MessageID, function () {
                    });
                    window.top.chatHubClient.del(thread.Message.MessageID);
                }
            }).hide()
            .text('x')
            .appendTo(dv);

        }

        var tptxt = $('<div>')
        .addClass('topictext')
        .html(thread.Message.Message.replace(/\n\r?/g, '<br />'))
        .appendTo(tpic);

        top.Ts.Services.WaterCooler.GetAttachments(thread.Message.MessageID, function (attachments) {
            if (attachments.length > 0) {
                var attdiv = $('<div>')
                .addClass('attachment-list')
                .appendTo(tptxt);

                var atticon = $('<span>')
                .addClass('ts-icon ts-icon-attachment')
                .appendTo(attdiv);
            }
            for (var i = 0; i < attachments.length; i++) {

                $('<a>')
                .attr('target', '_blank')
                .attr('filetype', attachments[i].FileType)
                .text(ellipseString(attachments[i].FileName, 20))
                .addClass('attfilename ui-state-default ts-link preview')
                .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                .hover(function (e) {
                    if ($(this).attr('filetype').indexOf('image') >= 0) {
                        $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                        $("#preview")
			        .css("top", (e.pageY - 10) + "px")
			        .css("left", (e.pageX + 30) + "px")
			        .fadeIn("fast");
                    }
                },
	            function () {
	                $("#preview").remove();
	            })
                .appendTo(attdiv);

            }
        });

        if (thread.Replies.length > 6) {
            var trc = $('<div>')
            .addClass('treplycontainer')
            .appendTo(tpic);

            var tpcommenttxt = $('<div>')
            .addClass('topicComments slim')
            .html(thread.Replies.length + " comments")
            .click(function (e) {
                e.preventDefault();
                $(this).parent().parent().find('.hiddenComments').slideToggle("fast");
            })
            .appendTo(trc);
            var comcontainer = $('<div>')
            .addClass('hiddenComments')
            .appendTo(tpic);
        }
        for (var j = 0; j < thread.Replies.length; j++) {
            var div = createReply(thread.Replies[j]);

            if (thread.Replies.length > 6 && j < (thread.Replies.length - 2)) {
                comcontainer.append(div);
            }
            else {
                tpic.append(div);
            }
        }

        var repcontainer = createCommentContainer(thread.Message.MessageID);

        repcontainer.appendTo(tpic);

        return tc;
    }

function createReply(thread) {
        var trc = $('<div>')
        .addClass('treplycontainer')
        .data('MessageID', thread.MessageID);

        var avaspn = $('<span>')
            .appendTo(trc);

        var avaimg = $('<img>')
            .addClass('topicavatarsm')
            .appendTo(avaspn);

        top.Ts.Services.Users.GetUserPhoto(thread.UserID, function (att) {
            avaimg.attr("src", att);
        });

        var tprpy = $('<div>')
        .addClass('topicrpy')
        .appendTo(trc);

        var dv = $('<div>')
            .hover(function () {
                $(this).find('.close').show();
            }, function () {
                $(this).find('.close').hide();
            })
            .appendTo(tprpy);

        var sptpic = $('<span>')
            .addClass('topicln')
            .appendTo(dv);

        var namea = $('<a>')
            .addClass('ts-linkb ts-link ui-state-default')
            .attr('href', '#')
            .text(thread.UserName)
            .click(function (e) {
                e.preventDefault();
                top.Ts.MainPage.openUser(thread.UserID);
            })
            .appendTo(sptpic);

        var sptm = $('<span>')
            .addClass('topictm timeago')
            .attr('title', thread.TimeStamp)
            .text(jQuery.timeago(thread.TimeStamp))
            .timeago()
            .appendTo(dv);

        var splike = $('<span>')
            .attr('id', 'spnlike')
            .appendTo(dv);

        top.Ts.Services.WaterCooler.GetLikes(thread.MessageID, function (likes) {
            var likeUsers = '';
            var userLike = false;

            for (var i = 0; i < likes.length; i++) {
                if (likes[i].UserID == top.Ts.System.User.UserID) {
                    likeUsers += "You<br/>";
                    userLike = true;
                }
                else
                    likeUsers += likes[i].UserName + "<br/>";
            }

            var star = $('<img>')
            .addClass('likestar')
            .attr('src', '../images/icons/star_icon.png')
            .attr('title', likeUsers)
            .tipTip({ defaultPosition: "top", edgeOffset: 7 })
            .appendTo(splike);

            $('<span>')
                .addClass('likeCounter someClass')
                .attr('id', 'likeCounter')
                .text(likes.length)
                .appendTo(splike);


            if (userLike == false) {
                var splikelink = $('<a>')
            .addClass('topiclike ts-link ui-state-default')
            .text("like")
            .click(function (e) {
                e.preventDefault();
                var parent = $(this).parent();
                //parent.find('#likeCounter').remove();
                $(this).hide();
                top.Ts.Services.WaterCooler.AddCommentLike(thread.MessageID, function (likes) {
                    window.top.chatHubClient.addLike(likes, thread.MessageID, thread.MessageParent);
                });

            })
            .appendTo(splike);
            }
        });

        var canEdit = top.Ts.System.User.UserID == thread.UserID || top.Ts.System.User.IsSystemAdmin;

        if (canEdit) {
            var spdelete = $('<span>')
            .addClass('topicrel close')
            .click(function (e) {
                if (confirm('Are you sure you would like to remove this reply?')) {
                    top.Ts.Services.WaterCooler.DeleteMessage(thread.MessageID, function () {
                    });
                    window.top.chatHubClient.del(thread.MessageID);
                }
            }).hide()
            .text('x')
            .appendTo(dv);
        }
        var tptxt = $('<div>')
        .addClass('topictext')
        .html(thread.Message.replace(/\n\r?/g, '<br />'))
        .appendTo(tprpy);


        top.Ts.Services.WaterCooler.GetAttachments(thread.MessageID, function (attachments) {
            if (attachments.length > 0) {
                var attdiv = $('<div>')
                .addClass('attachment-list')
                .appendTo(tptxt);

                var atticon = $('<span>')
                .addClass('ts-icon ts-icon-attachment')
                .appendTo(attdiv);
            }
            for (var i = 0; i < attachments.length; i++) {

                $('<a>')
                .attr('target', '_blank')
                .attr('filetype', attachments[i].FileType)
                .text(ellipseString(attachments[i].FileName, 20))
                .addClass('attfilename ui-state-default ts-link')
                .attr('href', '../../../dc/1/attachments/' + attachments[i].AttachmentID)
                .hover(function (e) {
                    if ($(this).attr('filetype').indexOf('image') >= 0) {
                        $("body").append("<p id='preview'><img src='" + this.href + "' alt='Image preview' style='max-width:400px' /></p>");
                        $("#preview")
			        .css("top", (e.pageY - 10) + "px")
			        .css("left", (e.pageX + 30) + "px")
			        .fadeIn("fast");
                    }
                },
	            function () {
	                $("#preview").remove();
	            })
                .appendTo(attdiv);

            }
        });

        return trc;
    }

function createCommentContainer(messageid) {
        var pc = $('<div>')
        .addClass('postcontainer');

        var ftc = $('<div>')
        .addClass('faketextcontainer')
        .click(function (e) {
            $(this).hide();
            $(this).parent().find('.commentcontainer').show();
            $(this).parent().find('.commentcontainer').find(".ticketcontainer").hide();
            $(this).parent().find('.commentcontainer').find(".groupcontainer").hide();
            //$(this).parent().find('.commentcontainer').find(".attcontainer").hide();
            $(this).parent().find('#messagecontents').focus();
            $(this).parent().find("#ticketinput").hide();
            $(this).parent().find("#groupinput").hide();
            $(this).parent().find("#customerinput").hide();
            $(this).parent().find("#productinput").hide();
            $(this).parent().find("#userinput").hide();
            $(this).parent().find("#attachmentinput").show();
            $(this).parent().find("#ticketinsert").show();

        })
        .appendTo(pc);

        var ft = $('<span>')
        .addClass('faketext')
        .attr('title', 'comment')
        .text('post comment')
        .click(function (e) {
            $(this).parent().hide();
            var comment = $(this).parent().parent().find('.commentcontainer').show();
            comment.find('#messagecontents').focus();
        })
        .appendTo(ftc);

        var cc = $('<div>')
        .addClass('commentcontainer')
        .appendTo(pc);

        var s = $('<span>')
        .appendTo(cc);

        var ta = $('<textarea>')
        .addClass('boxsizingBorder singlelinecomment')
        .attr({ rows: '2', id: 'messagecontents', placeholder: 'post comment' })
        .autoGrow()
        .appendTo(s);

        var attrow = $('<div>')
        .addClass('attachrow')
        .appendTo(cc);

        var attbtn = $('<span>')
        .addClass('addatt')
        .attr('title', 'Add Attachments')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").hide();
            $(this).parent().parent().find("#groupinput").hide();
            $(this).parent().parent().find("#customerinput").hide();
            $(this).parent().parent().find("#productinput").hide();
            $(this).parent().parent().find("#userinput").hide();
            $(this).parent().parent().find("#attachmentinput").show();
            $(this).parent().parent().find("#ticketinsert").show();
            $(this).parent().find(".arrow-up").css('left', '7px');
        })
        .tooltip()
        .appendTo(attrow);

        var tixbtn = $('<span>')
        .addClass('addticket')
        .attr('title', 'Associate Ticket')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").show();
            $(this).parent().parent().find("#groupinput").hide();
            $(this).parent().parent().find("#customerinput").hide();
            $(this).parent().parent().find("#productinput").hide();
            $(this).parent().parent().find("#userinput").hide();
            $(this).parent().parent().find("#attachmentinput").hide();
            $(this).parent().parent().find("#ticketinsert").hide();
            $(this).parent().find(".arrow-up").css('left', '30px');
        })
        .tooltip()
        .appendTo(attrow);

        var usrbtn = $('<span>')
        .addClass('adduser')
        .attr('title', 'Associate User')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").hide();
            $(this).parent().parent().find("#groupinput").hide();
            $(this).parent().parent().find("#customerinput").hide();
            $(this).parent().parent().find("#productinput").hide();
            $(this).parent().parent().find("#userinput").show();
            $(this).parent().parent().find("#attachmentinput").hide();
            $(this).parent().parent().find("#ticketinsert").hide();
            $(this).parent().find(".arrow-up").css('left', '53px');
        })
        .tooltip()
        .appendTo(attrow);

        var custbtn = $('<span>')
        .addClass('addcustomer')
        .attr('title', 'Associate Company')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").hide();
            $(this).parent().parent().find("#groupinput").hide();
            $(this).parent().parent().find("#customerinput").show();
            $(this).parent().parent().find("#productinput").hide();
            $(this).parent().parent().find("#userinput").hide();
            $(this).parent().parent().find("#attachmentinput").hide();
            $(this).parent().parent().find("#ticketinsert").hide();
            $(this).parent().find(".arrow-up").css('left', '78px');
        })
        .tooltip()
        .appendTo(attrow);

        var grpbtn = $('<span>')
        .addClass('addgroup')
        .attr('title', 'Associate Group')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").hide();
            $(this).parent().parent().find("#groupinput").show();
            $(this).parent().parent().find("#customerinput").hide();
            $(this).parent().parent().find("#productinput").hide();
            $(this).parent().parent().find("#userinput").hide();
            $(this).parent().parent().find("#attachmentinput").hide();
            $(this).parent().parent().find("#ticketinsert").hide();
            $(this).parent().find(".arrow-up").css('left', '104px');
        })
        .tooltip()
        .appendTo(attrow);

        var prodbtn = $('<span>')
        .addClass('addproduct')
        .attr('title', 'Associate Product')
        .click(function (e) {
            e.preventDefault();
            $(this).parent().parent().find("#ticketinput").hide();
            $(this).parent().parent().find("#groupinput").hide();
            $(this).parent().parent().find("#customerinput").hide();
            $(this).parent().parent().find("#productinput").show();
            $(this).parent().parent().find("#userinput").hide();
            $(this).parent().parent().find("#attachmentinput").hide();
            $(this).parent().parent().find("#ticketinsert").hide();
            $(this).parent().find(".arrow-up").css('left', '125px');
        })
        .tooltip()
        .appendTo(attrow);

        var postbtn = $('<button>')
        .attr({ id: 'newcomment', title: 'Post Comment' })
        .addClass("btn btn-primary")
        .text('comment')
        .click(function (e) {
            var msgtxtbox = $(this).parent().prev().find('#messagecontents');
            if (msgtxtbox.val().length > 0) {
                var commentinfo = new Object();
                commentinfo.Description = msgtxtbox.val();
                commentinfo.Attachments = new Array();
                commentinfo.ParentTicketID = messageid;

                commentinfo.Tickets = new Array();
                $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    commentinfo.Tickets[commentinfo.Tickets.length] = $(this).data('Ticket');
                });

                commentinfo.Groups = new Array();
                $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                    commentinfo.Groups[commentinfo.Groups.length] = $(this).data('Group');
                });

                commentinfo.Products = new Array();
                $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                    commentinfo.Products[commentinfo.Products.length] = $(this).data('Product');
                });

                commentinfo.Company = new Array();
                $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                    commentinfo.Company[commentinfo.Company.length] = $(this).data('Company');
                });

                commentinfo.User = new Array();
                $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                    commentinfo.User[commentinfo.User.length] = $(this).data('User');
                });


                var attcontainer = $(this).parent().parent().find('#commentatt').find('.upload-queue div');

                top.Ts.Services.WaterCooler.NewComment(top.JSON.stringify(commentinfo), function (MessageID) {
                    if (attcontainer.length > 0) {
                        attcontainer.each(function (i, o) {
                            var data = $(o).data('data');
                            data.url = '../../../Upload/WaterCooler/' + MessageID;
                            data.jqXHR = data.submit();
                            $(o).data('data', data);
                        });
                    }

                    window.top.chatHubClient.newThread(MessageID);

                    cc.hide();
                    ftc.show();
                    ta.val('');
                    cc.find('.upload-queue').empty();
                    cc.find('.group-queue').empty();
                    cc.find('.ticket-queue').empty();
                    cc.find('.product-queue').empty();
                    cc.find('.customer-queue').empty();
                    cc.find('.user-queue').empty();
                    cc.find(".arrow-up").css('left', '7px');
                });
                $('.frame-content').animate({ scrollTop: 0 }, 600);
            }
        })
        .appendTo(attrow);

        var arrowdiv = $('<div>')
        .addClass('arrow-up')
        .appendTo(attrow);

        var attcont = $('<div>')
        .addClass('attcontainer subcontainer')
        .attr('id', 'commentatt')
        .appendTo(cc);

        var ulqueue = $('<div>')
        .addClass('upload-queue')
        .appendTo(attcont);
        var tqueeue = $('<div>')
        .addClass('ticket-queue')
        .appendTo(attcont);
        var userqueue = $('<div>')
        .addClass('user-queue')
        .appendTo(attcont);
        var groupqueeue = $('<div>')
        .addClass('group-queue')
        .appendTo(attcont);
        var custqueue = $('<div>')
        .addClass('customer-queue')
        .appendTo(attcont);
        var prodqueue = $('<div>')
        .addClass('product-queue')
        .appendTo(attcont);

        var spnur = $('<span>')
        .addClass('ur')
        .appendTo(attcont);

        var ulform = $('<form>')
        .addClass('file-upload')
        .attr({ id: 'attachmentinput' })
        .appendTo(spnur);

        var fipt = $('<input>')
        .attr({ type: 'file', name: 'file[]' })
        .fileupload({
            add: function (e, data) {
                for (var i = 0; i < data.files.length; i++) {

                    var bg = $('<div>')
                  .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                  .appendTo(ulqueue);

                    data.context = bg;
                    bg.data('data', data);

                    $('<span>')
                  .text(ellipseString(data.files[i].name, 20) + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);

                    $('<span>')
                  .addClass('ui-icon ui-icon-close')
                  .click(function (e) {
                      e.preventDefault();
                      $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

                    $('<span>')
                  .addClass('ui-icon ui-icon-cancel')
                  .hide()
                  .click(function (e) {
                      e.preventDefault();
                      var data = $(this).closest('li').data('data');
                      data.jqXHR.abort();
                  })
                  .appendTo(bg);
                }

            },
            send: function (e, data) {
                if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
                    //data.context.find('.progress').progressbar('value', 50);
                }
            },
            fail: function (e, data) {
                if (data.errorThrown === 'abort') return;
                alert('There was an error uploading "' + data.files[0].name + '".');
            },
            progress: function (e, data) {
                //data.context.find('.progress').progressbar('value', parseInt(data.loaded / data.total * 100, 10));
            },
            start: function (e, data) {
                //$(this).parent().parent().find('.progress').progressbar().show();
                //$(this).parent().parent().find('.upload-queue .ui-icon-close').hide();
                //$(this).parent().parent().find('.upload-queue .ui-icon-cancel').show();
            },
            stop: function (e, data) {
                $(this).parent().parent().find('.progress').progressbar('value', 100);
            }
        })
        .appendTo(ulform)

        var atbtn = $('<a>')
        .addClass('ui-state-default ts-link')
        .text('+add attachment')
        .appendTo(ulform);

        var tinput = $('<input>')
        .addClass('main-quick-ticket')
        .attr({ type: 'text' })
        .attr({ id: 'ticketinput' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
        .focusout(function () { $(this).val('Search for a ticket...').addClass('main-quick-ticket-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('main-quick-ticket-blur'); })
        .val('Search for a ticket...')
        .autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    var isDupe;
                    $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                        if (ui.item.id == $(this).data('Ticket')) {
                            isDupe = true;
                        }
                    });
                    if (!isDupe) {
                        var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.ticket-queue')).data('Ticket', ui.item.id);


                        $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                        $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    }
                }
                $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        var inserttixinput = $('<input>')
        .addClass('insert-ticket')
        .attr({ type: 'text' })
        .attr({ id: 'ticketinsert' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('insert-ticket-blur'); })
        .focusout(function () { $(this).val('insert ticket link...').addClass('insert-ticket-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('insert-ticket-blur'); })
        .val('insert ticket link...')
        .autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    ta.val(ta.val() + " &ticket" + ui.item.id);
                }
                $('.insert-ticket').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        var userinput = $('<input>')
        .addClass('user-search ui-widget-content')
        .attr({ type: 'text' })
        .attr({ id: 'userinput' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('user-search-blur'); })
        .focusout(function () { $(this).val('Search for a user...').addClass('user-search-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('user-search-blur'); })
        .val('Search for a user...')
        .autocomplete({ minLength: 4, source: getUsers, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    var isDupe;
                    $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                        if (ui.item.id == $(this).data('User')) {
                            isDupe = true;
                        }
                    });
                    if (!isDupe) {
                        var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.user-queue')).data('User', ui.item.id);


                        $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                        $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    }
                }
                $('.user-search').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        var custinput = $('<input>')
        .addClass('company-search ui-widget-content')
        .attr({ type: 'text' })
        .attr({ id: 'customerinput' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('company-search-blur'); })
        .focusout(function () { $(this).val('Search for a company...').addClass('company-search-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('company-search-blur'); })
        .val('Search for a company...')
        .autocomplete({ minLength: 4, source: getCustomers, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    var isDupe;
                    $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                        if (ui.item.id == $(this).data('Company')) {
                            isDupe = true;
                        }
                    });
                    if (!isDupe) {
                        var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.customer-queue')).data('Company', ui.item.id);


                        $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                        $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    }
                }
                $('.company-search').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        var groupinput = $('<input>')
        .addClass('group-search ui-widget-content')
        .attr({ type: 'text' })
        .attr({ id: 'groupinput' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('group-search-blur'); })
        .focusout(function () { $(this).val('Search for a group...').addClass('group-search-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('group-search-blur'); })
        .val('Search for a group...')
        .autocomplete({ minLength: 2, source: getGroupsByTerm, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    var isDupe;
                    $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                        if (ui.item.id == $(this).data('Group')) {
                            isDupe = true;
                        }
                    });
                    if (!isDupe) {
                        var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.group-queue')).data('Group', ui.item.id);


                        $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                        $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    }
                }
                $('.group-search').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        var prodinput = $('<input>')
        .addClass('product-search ui-widget-content')
        .attr({ type: 'text' })
        .attr({ id: 'productinput' })
        .css('width', '200px')
        .autoGrow()
        .focusin(function () { $(this).val('').removeClass('product-search-blur'); })
        .focusout(function () { $(this).val('Search for a product...').addClass('product-search-blur').removeClass('ui-autocomplete-loading'); })
        .click(function () { $(this).val('').removeClass('product-search-blur'); })
        .val('Search for a product...')
        .autocomplete({ minLength: 4, source: getProductByTerm, delay: 300,
            select: function (event, ui) {
                if (ui.item) {
                    var isDupe;
                    $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                        if (ui.item.id == $(this).data('Product')) {
                            isDupe = true;
                        }
                    });
                    if (!isDupe) {
                        var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.product-queue')).data('Product', ui.item.id);


                        $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                        $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                    }
                }
                $('.product-search').removeClass('ui-autocomplete-loading');
                return false;
            }
        })
        .appendTo(spnur);

        return pc;
    }

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

$('.user-search').autocomplete({
        minLength: 4,
        source: getUsers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.user-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('User')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.user-queue')).data('User', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

$('.company-search').autocomplete({
        minLength: 4,
        source: getCustomers,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.customer-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Company')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.customer-queue')).data('Company', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
            //.next().show();
        }
    });

$('.main-quick-ticket').autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.ticket-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Ticket')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.ticket-queue')).data('Ticket', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $('.main-quick-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

$('.insert-ticket').autocomplete({ minLength: 2, source: getTicketsByTerm, delay: 300,
        select: function (event, ui) {
            if (ui.item) {
                $('#messagecontents').val($('#messagecontents').val() + " &ticket" + ui.item.id);
            }
            $('.insert-ticket').removeClass('ui-autocomplete-loading');
            return false;
        }
    });

$('.group-search').autocomplete({
        minLength: 2,
        source: getGroupsByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.group-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Group')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.group-queue')).data('Group', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

$('.product-search').autocomplete({
        minLength: 4,
        source: getProductByTerm,
        select: function (event, ui) {
            if (ui.item) {
                var isDupe;
                $(this).parent().parent().find('.product-queue').find('.ticket-removable-item').each(function () {
                    if (ui.item.id == $(this).data('Product')) {
                        isDupe = true;
                    }
                });
                if (!isDupe) {
                    var bg = $('<div>')
                    .addClass('ui-corner-all ts-color-bg-accent ticket-removable-item ulfn')
                    .appendTo($(this).parent().parent().find('.product-queue')).data('Product', ui.item.id);


                    $('<span>')
                    .text(ui.item.value)
                    .addClass('filename')
                    .appendTo(bg);

                    $('<span>')
                    .addClass('ui-icon ui-icon-close')
                    .click(function (e) {
                        e.preventDefault();
                        $(this).closest('div').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);
                }
            }
            $(this)
            .data('item', ui.item)
            .removeClass('ui-autocomplete-loading');
        }
    });

function ellipseString(text, max) {
    return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; 
 };





