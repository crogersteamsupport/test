/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />


$(document).ready(function () {

    var _isAdmin = top.Ts.System.User.IsSystemAdmin;
    if (!top.Ts.System.User.CanCreateCompany && !top.Ts.System.User.CanCreateContact && !_isAdmin) {
        $('.action-new').hide();
    }
    $('input, textarea').placeholder();

    $('.customer-add-new').click(function (e) {
        e.preventDefault();
        alert("Go to new Customer/Contact page");
    });

    function showLoadingIndicator() {
        _isLoading = true;
        $('.results-loading').show();
    }

    function insertSearchResults(html) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();
        $('.searchresults').empty().fadeTo(0, 1);
        if (html == '') {
            $('.results-empty').show();
        }
        else {
            $('.searchresults').html(html);
        }
        _isLoading = false;
    }

    function appendSearchResults(html) {
        $('.results-loading').hide();
        $('.results-empty').hide();
        $('.results-done').hide();
        if (html == '') {
            $('.results-done').show();
        } else {
            $('.searchresults').append(html);
        }
        _isLoading = false;
    }


    $('.customers-filter-all').click(function (e) {
        e.preventDefault();
        $(this).parents(':eq(1)').find('li').removeClass('active');
        $(this).parent().addClass('active');
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        top.Ts.Services.Customers.GetSearchResults($('#searchString').val(), 0, function (resultHtml) {
            insertSearchResults(resultHtml);
        });
    });

    $('.customers-filter-customers').click(function (e) {
        e.preventDefault();
        $(this).parents(':eq(1)').find('li').removeClass('active');
        $(this).parent().addClass('active');
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        top.Ts.Services.Customers.GetCompanies($('#searchString').val(), 0, function (resultHtml) {
            insertSearchResults(resultHtml);
        });
    });

    $('.action-new').click(function (e) {
        e.preventDefault();
        top.Ts.MainPage.newCustomer();

    });

    $('.customers-filter-contacts').click(function (e) {
        e.preventDefault();
        $(this).parents(':eq(1)').find('li').removeClass('active');
        $(this).parent().addClass('active');
        showLoadingIndicator();
        $('.searchresults').fadeTo(200, 0.5);
        top.Ts.Services.Customers.GetContacts($('#searchString').val(), 0, function (resultHtml) {
            insertSearchResults(resultHtml);
        });
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
    showLoadingIndicator();
    top.Ts.Services.Customers.GetSearchResults("", 0, function (resultHtml) {
        insertSearchResults(resultHtml);
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

    var delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();


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

            function loadResults(html) {
                $('.frame-container').animate({ scrollTop: 1 }, 600);
                insertSearchResults(html);
            }

            if ($('.customers-filter-all').parent().hasClass('active')) {
                top.Ts.Services.Customers.GetSearchResults($('#searchString').val(), 0, loadResults);
            }
            else if ($('.customers-filter-customers').parent().hasClass('active')) {
                top.Ts.Services.Customers.GetCompanies($('#searchString').val(), 0, loadResults);
            }
            else if ($('.customers-filter-contacts').parent().hasClass('active')) {
                top.Ts.Services.Customers.GetContacts($('#searchString').val(), 0, loadResults);
            }
            else {
                _isLoading = false;
            }
        }

        _tmrSearch = setTimeout(getResults, 500);

    });

    var _isLoading = false;
    $('.frame-container').bind('scroll', function () {
        if (_isLoading == true) return;
        if ($('.results-done').is(':visible')) return;

        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            var filterType = getSearchFilter();

            showLoadingIndicator();
            top.Ts.Services.Customers.GetMoreResults(filterType, $('#searchString').val(), $('.peopleinfo').length, function (results) {
                appendSearchResults(results);
            });
        }

        if ($(this).scrollTop() > 100) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').click(function () {
        $('.frame-container').animate({ scrollTop: 0 }, 600);
        return false;
    });

    function getSearchFilter() {
        if ($('.customers-filter-all').parent().hasClass('active'))
            return "all";
        if ($('.customers-filter-customers').parent().hasClass('active'))
            return "customers"
        if ($('.customers-filter-contacts').parent().hasClass('active'))
            return "contacts";

    }


});

function refreshPage() {
    top.Ts.Services.Customers.GetRecentlyViewed(function (resultHtml) {
        $('.recent-container').empty();
        $('.recent-container').html(resultHtml);
    });
}