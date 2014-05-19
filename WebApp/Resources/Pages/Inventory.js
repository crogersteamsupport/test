$(document).ready(function () {

//  var _isAdmin = top.Ts.System.User.IsSystemAdmin;
//  if (!top.Ts.System.User.CanCreateCompany && !top.Ts.System.User.CanCreateContact && !_isAdmin) {
//    $('.action-new').hide();
//  }
  $('input, textarea').placeholder();

  function fetchItems(start) {
    start = start || 0;
    showLoadingIndicator();
    $('.searchresults').fadeTo(200, 0.5);
    var term = $('#searchString').val();

    var searchAssigned = false;
    var searchWarehouse = false;
    var searchJunkyard = false;

    if ($('.inventory-filter-all').parent().hasClass('active')) {
      searchAssigned = true;
      searchWarehouse = true;
      searchJunkyard = true;
    } else if ($('.inventory-filter-assigned').parent().hasClass('active')) {
      searchAssigned = true;
    } else if ($('.inventory-filter-warehouse').parent().hasClass('active')) {
      searchWarehouse = true;
    } else if ($('.inventory-filter-junkyard').parent().hasClass('active')) {
      searchJunkyard = true;
    }

//    top.Ts.Services.Search.SearchCompaniesAndContacts($('#searchString').val(), start, 20, searchCompanies, searchContacts, function (items) {
//      $('.searchresults').fadeTo(0, 1);

//      if (start == 0) {
//        insertSearchResults(items);
//        /*$('.frame-container').animate({
//        scrollTop: 1
//        }, 600);*/
//      } else {
//        appendSearchResults(items);
//      }

//    });
  }

  function showLoadingIndicator() {
    _isLoading = true;
    $('.results-loading').show();
  }

  function insertSearchResults(items) {
    $('.searchresults').empty();

    if (items.length < 1) {
      $('.results-loading').hide();
      $('.results-done').hide();
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

//  function appendItem(container, item) {
//    var el = $('<tr>');
//    var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
//    var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

//    $('<td>').addClass('result-icon').append(
//          $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
//        ).appendTo(el);

//    var div = $('<div>')
//          .addClass('assetinfo')
//          .append(
//            $('<div>')
//              .addClass('pull-right')
//              .append($('<p>').text(item.openTicketCount + ' open tickets'))
//          );

//    $('<td>').append(div).appendTo(el);

//    if (item.userID) {
//      circle.addClass('color-orange');
//      icon.addClass('fa-user');
//      appendContact(div, item);
//    }
//    else {
//      circle.addClass('color-green');
//      icon.addClass('fa-building-o');
//      appendCompany(div, item);
//    }
//    el.appendTo(container);
//  }

  function isNullOrWhiteSpace(str) {
    return str === null || str.match(/^ *$/) !== null;
  }

  $('.inventory-filter').on('click', 'a', function (e) {
    e.preventDefault();
    $('.inventory-filter li.active').removeClass('active');
    $(this).parent().addClass('active');
    fetchItems();
  });

  $('.action-new').click(function (e) {
    e.preventDefault();
    top.Ts.MainPage.newAsset();

  });

});