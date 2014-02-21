$(document).ready(function () {

    var _isAdmin = top.Ts.System.User.IsSystemAdmin;
    if (!top.Ts.System.User.CanCreateCompany && !top.Ts.System.User.CanCreateContact && !_isAdmin) {
        $('.action-new').hide();
    }
    $('input, textarea').placeholder();

    function fetchItems(start) {
        start = start || 0;
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        var term = $('#searchString').val();

        var searchCompanies = false;
        var searchContacts = false;
        if ($('.customers-filter-all').parent().hasClass('active')) {
            searchContacts = true;
            searchCompanies = true;
        } else if ($('.customers-filter-customers').parent().hasClass('active')) {
            searchCompanies = true;
        } else if ($('.customers-filter-contacts').parent().hasClass('active')) {
            searchContacts = true;
        }

        top.Ts.Services.Search.SearchCompaniesAndContacts($('#searchString').val(), start, 20, searchCompanies, searchContacts, function (items) {
            $('.searchresults').fadeTo(0, 1);

            if (start == 0) {
                insertSearchResults(items);
                /*$('.frame-container').animate({
                scrollTop: 1
                }, 600);*/
            } else {
                appendSearchResults(items);
            }

        });
    }

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(items) {
        $('.searchresults').empty();

        if (items.length < 1) {
            $('.results-empty').show();
        } else {
            appendSearchResults(items);
        }
        _isLoading = false;
    }

    function appendSearchResults(items) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();

        if (items.length < 1) {
            $('.results-done').show();
        } else {
            var container = $('.searchresults');
            for (var i = 0; i < items.length; i++) {
                appendItem(container, JSON.parse(items[i]));
            }
        }
        _isLoading = false;
    }

    function appendItem(container, item) {
        var el = $('<tr>');
        var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
        var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

        $('<td>').addClass('result-icon').append(
          $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
        ).appendTo(el);

        var div = $('<div>')
          .addClass('peopleinfo')
          .append(
            $('<div>')
              .addClass('pull-right')
              .append($('<p>').text(item.openTicketCount + ' open tickets'))
              .append($('<p>').text(item.isPortal ? 'Has portal access' : 'Does not have portal access'))
          );

        $('<td>').append(div).appendTo(el);

        if (item.organizationID) {
            circle.addClass('color-green');
            icon.addClass('fa-building-o');
            appendCompany(div, item);
        }
        else {
            circle.addClass('color-orange');
            icon.addClass('fa-user');
            appendContact(div, item);
        }
        el.appendTo(container);
    }

    function appendCompany(el, item) {
        

        $('<a>')
          .attr('href', '#')
          .addClass('companylink')
          .data('id', item.organizationID)
          .text(!isNullOrWhiteSpace(item.name) ? item.name : 'Unnamed Company')
          .appendTo($('<h4>').appendTo(el));

        var list = $('<ul>').appendTo(el);
        // check http
        if (item.website && item.website != '') {
            $('<a>')
                .attr('target', '_blank')
                .attr('href', item.website)
                .text(item.website)
                .appendTo($('<li>').appendTo(list));
        }

        var phones = $('<li>');
        appendPhones(phones, item);
        phones.appendTo(list);
    }

    function appendContact(el, item) {
        $('<a>')
          .attr('href', '#')
          .addClass('contactlink')
          .text(isNullOrWhiteSpace(item.fName) && isNullOrWhiteSpace(item.lName) ? 'Unnamed Contact' : item.fName + ' ' + item.lName)
          .data('id', item.userID)
          .appendTo($('<h4>').appendTo(el));

    }

    function appendPhones(el, item) {
        //type number ext 
        for (var i = 0; i < item.phones.length; i++) {
            var phone = item.phones[i];
            if (!isNullOrWhiteSpace(phone.number)) {
                $('<span>').text(phone.type + " ").appendTo(el);
                $('<a>')
                    .attr('href', 'tel:' + phone.number)
                    .attr('target', '_blank')
                    .text(phone.number)
                    .appendTo(el);
                if (!isNullOrWhiteSpace(phone.ext)) {
                    $('<span>').text(" Ext:" + phone.ext).appendTo(el);
                }
            }
        }
    }

    function isNullOrWhiteSpace(str) {
        return str === null || str.match(/^ *$/) !== null;
    }


    $('.customers-filter').on('click', 'a', function (e) {
        e.preventDefault();
        $('.customers-filter li.active').removeClass('active');
        $(this).parent().addClass('active');
        fetchItems();
    });


    $('.action-new').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.newCustomer();

    });


    $('.searchresults, .recent-container').on('click', '.contactlink', function (e) {
        e.preventDefault();

        var id = e.target.id.substring(1);
        top.Ts.MainPage.openNewContact(id);
    });

    $('.searchresults').on('click', '.viewOrg', function (e) {
        e.preventDefault();

        var id = e.target.id;
        top.Ts.MainPage.openNewCustomer(id);
    });

    $('.searchresults, .recent-container').on('click', '.companylink', function (e) {
        e.preventDefault();

        var id = e.target.id.substring(1);
        top.Ts.MainPage.openNewCustomer(id);
    });

    top.Ts.Services.Customers.GetRecentlyViewed(function (resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });

    $('.searchresults')
        .click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($(e.target).is('a.contactlink') || $(e.target).is('a.companylink')) {
                top.Ts.Services.Customers.UpdateRecentlyViewed(e.target.id, function (resultHtml) {
                    $('.recent-container').empty();
                    $('.recent-container').html(resultHtml);
                });
            }
        });


    var _tmrSearch = null;
    $('#searchString').keyup(function () {
        if (_tmrSearch != null) {
            clearTimeout(_tmrSearch);
        }
        $('.searchresults').fadeTo(200, 0.5);

        function getResults() {
            _isLoading = true;
            $('.results-loading').hide();
            $('.results-empty').hide();
            $('.results-done').hide();

            fetchItems();
        }

        _tmrSearch = setTimeout(getResults, 500);

    });

    var _isLoading = false;
    $('.frame-container').bind('scroll', function () {
        if (_isLoading == true) return;
        if ($('.results-done').is(':visible')) return;

        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            fetchItems($('.peopleinfo').length);
        }

        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').click(function () {
        $('.frame-container').animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    fetchItems();

});

function refreshPage() {
    top.Ts.Services.Customers.GetRecentlyViewed(function (resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });
}