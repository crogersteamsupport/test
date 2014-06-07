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

    top.Ts.Services.Search.SearchAssets($('#searchString').val(), start, 20, searchAssigned, searchWarehouse, searchJunkyard, function (items) {
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

  function appendItem(container, item) {
    var el = $('<tr>');
    var circle = $('<i>').addClass('fa fa-circle fa-stack-2x');
    var icon = $('<i>').addClass('fa fa-stack-1x fa-inverse');

    $('<td>').addClass('result-icon').append(
          $('<span>').addClass('fa-stack fa-2x').append(circle).append(icon)
        ).appendTo(el);

    var div = $('<div>')
          .addClass('assetinfo');
    //          .append(
    //            $('<div>')
    //              .addClass('pull-right')
    //              .append($('<p>').text(item.openTicketCount + ' open tickets'))
    //          );

        $('<td>').append(div).appendTo(el);

    switch (item.location) {
      case "1":
        circle.addClass('color-green');
        icon.addClass('fa-truck');
        break;
      case "2":
        circle.addClass('color-yellow');
        icon.addClass('fa-home');
        break;
      case "3":
        circle.addClass('color-red');
        icon.addClass('fa-trash-o');
        break;
    }
    appendAsset(div, item);

    el.appendTo(container);
  }

  function appendAsset(el, item) {
    var displayName = item.name;
    if (isNullOrWhiteSpace(displayName)) {
      displayName = item.serialNumber;
      if (isNullOrWhiteSpace(displayName)) {
        displayName = item.assetID;
      }
    }

    $('<a>')
          .attr('href', '#')
          .addClass('assetlink')
          .data('assetID', item.assetID)
          .text(displayName)
          .appendTo($('<h4>').appendTo(el));

    var list = $('<ul>').appendTo(el);

    $('<a>')
          .attr('target', '_blank')
          .text(item.productName)
          .appendTo($('<li>').appendTo(list));

    if (!isNullOrWhiteSpace(item.productVersionNumber)) {
      $('<a>')
            .attr('target', '_blank')
            .text(item.productVersionNumber)
            .appendTo($('<li>').appendTo(list));
    }
  }


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

  $('.searchresults, .recent-container').on('click', '.assetlink', function (e) {
    e.preventDefault();

    var id = $(this).data('assetID');
    top.Ts.System.logAction('Inventory Page - View Recent Asset');
    top.Ts.MainPage.openNewAsset(id);

    top.Ts.Services.Customers.UpdateRecentlyViewed('o' + id, function (resultHtml) {
      $('.recent-container').empty();
      $('.recent-container').html(resultHtml);
    });

  });


  fetchItems();
});