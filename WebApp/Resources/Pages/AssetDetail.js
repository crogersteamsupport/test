/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var _assetDetailPage = null;
var _assetID = null;
var _execGetCustomer = null;
var _assetAssignments = null;

$(document).ready(function () {
  _assetDetailPage = new AssetDetailPage();
  _assetDetailPage.refresh();
  $('.asset-tooltip').tooltip({ placement: 'bottom', container: 'body' });

  $('body').layout({
    defaults: {
      spacing_open: 0,
      resizable: false,
      closable: false
    },
    north: {
      size: 100,
      spacing_open: 1
    },
    center: {
      maskContents: true,
      size: 'auto'
    }
  });

  _assetID = top.Ts.Utils.getQueryValue("assetid", window);
  var historyLoaded = 0;
  top.privateServices.SetUserSetting('SelectedAssetID', _assetID);

  $('#assetRefresh').click(function (e) {
    window.location = window.location;
  });

  //  As below lets do if in Junkyard then set delete button to delete for admins only
  //  if (!_isAdmin)
  //    $('#customerDelete').hide();


  // To implement once we've figure out the two types of asset functionality
  //  $('#customerDelete').click(function (e) {
  //    if (confirm('Are you sure you would like to remove this organization?')) {
  //      top.privateServices.DeleteOrganization(organizationID, function (e) {
  //        if (window.parent.document.getElementById('iframe-mniCustomers'))
  //          window.parent.document.getElementById('iframe-mniCustomers').contentWindow.refreshPage();
  //        top.Ts.MainPage.closeNewCustomerTab(organizationID);
  //      });
  //    }
  //  });

  top.Ts.Services.Assets.GetAsset(_assetID, function (asset) {
    if (asset.Name) {
      $('#assetTitle').html(asset.Name);
    }
    else if (asset.SerialNumber) {
      $('#assetTitle').html(asset.SerialNumber);
    }
    else {
      $('#assetTitle').html(asset.AssetID);
    }

    $('#fieldName').text(asset.Name);
    $('#fieldProduct').data('productID', asset.ProductID);
    $('#fieldProduct').text(asset.ProductName);
    $('#fieldProductVersion').data('productVersionID', asset.ProductVersionID);
    if (asset.ProductVersionID == null) {
      var product = top.Ts.Cache.getProduct(asset.ProductID);
      if (product.Versions.length == 0) {
        $('#fieldProductVersion').text('N/A');
        $('#fieldProductVersion').removeClass("editable");
      }
      else {
        $('#fieldProductVersion').text('Unassigned');
        $('#fieldProductVersion').addClass("editable");
      }
    }
    else {
      $('#fieldProductVersion').text(asset.ProductVersionNumber);
    }
    $('#fieldSerialNumber').text(asset.SerialNumber);
    $('#fieldWarrantyExpiration').text(top.Ts.Utils.getMsDate(asset.WarrantyExpiration).localeFormat(top.Ts.Utils.getDatePattern()));
    $('#fieldNotes').text(asset.Notes);

    $('#fieldAssetID').text(asset.AssetID);
    $('#fieldCreator').text(asset.CreatorName);
    $('#fieldDateCreated').text(top.Ts.Utils.getMsDate(asset.DateCreated).localeFormat(top.Ts.Utils.getDateTimePattern()));
    $('#fieldModifier').text(asset.ModifierName);
    $('#fieldDateModified').text(top.Ts.Utils.getMsDate(asset.DateModified).localeFormat(top.Ts.Utils.getDateTimePattern()));

    switch (asset.Location) {
      case '1':
        $('#locationHeader').text('Assigned');
        $('.assignedDetails').show();
        $('.junkyardDetails').hide();
        $('#assetToJunkyard').hide();
        top.Ts.Services.Assets.GetAssetAssignments(_assetID, function (assetAssignments) {
          _assetAssignments = assetAssignments;
          for (var i = 0; i < assetAssignments.length; i++) {
            $('<tr>').html('<td>' +
            assetAssignments[i].NameAssignedTo + '</td><td>' +
            top.Ts.Utils.getMsDate(assetAssignments[i].ActionTime).localeFormat(top.Ts.Utils.getDatePattern()) + '</td><td>' +
            assetAssignments[i].ActorName + '</td><td>' +
            assetAssignments[i].Comments + '</td><td>' +
            '</td>').appendTo('#assignedTable > tbody:last');
          }
        });
        break;
      case '2':
        $('#locationHeader').text('Warehouse');
        $('#locationDetails').hide();
        $('#returnAsset').hide();
        break;
      case '3':
        $('#locationHeader').text('Junkyard');
        $('.assignedDetails').hide();
        $('.junkyardDetails').show();
        $('#assetToJunkyard').hide();
        break;
    }
  });

  $('#assetEdit').click(function (e) {
    $('.userProperties p').toggleClass("editable");
    //    $('.customProperties p').toggleClass("editable");
    //    $("#phonePanel #editmenu").toggleClass("hiddenmenu");
    //    $("#addressPanel #editmenu").toggleClass("hiddenmenu");
    //    $(this).toggleClass("btn-primary");
    //    $(this).toggleClass("btn-success");
    //    $('#companyTabs a:first').tab('show');
    //    if ((!_isAdmin && !top.Ts.System.User.HasPortalRights) || !top.Ts.System.User.CanEditCompany) {
    //      $('#fieldPortalAccess').removeClass('editable');
    //    }

  });

  $('#assetAssign').click(function (e) {
    $('#modalAssignTitle').text('Assign asset');
    $('#btnSaveReturn').hide();
    $('#btnSaveAssign').show();
  });

  $('#returnAsset').click(function (e) {
    $('#modalAssignTitle').text('Return asset');
    $('#btnSaveAssign').hide();
    $('#btnSaveReturn').show();
    $('#inputCustomerDiv').hide();
  });

  $('#btnSaveAssign').click(function (e) {
    if ($('#inputCustomer').data('item') && $('#dateShipped').val()) {
      var refType = 9;
      if ($('#inputCustomer').data('item').data == 'u') {
        refType = 32;
      }

      var assetAssignmentInfo = new Object();

      assetAssignmentInfo.RefID = $('#inputCustomer').data('item').id;
      assetAssignmentInfo.RefType = refType;
      assetAssignmentInfo.DateShipped = $('#dateShipped').val();
      assetAssignmentInfo.TrackingNumber = $('#trackingNumber').val();
      assetAssignmentInfo.ShippingMethod = $('#shippingMethod').val();
      assetAssignmentInfo.ReferenceNumber = $('#referenceNumber').val();
      assetAssignmentInfo.Comments = $('#comments').val();

      top.Ts.Services.Assets.AssignAsset(_assetID, top.JSON.stringify(assetAssignmentInfo), function (assetID) {
        top.Ts.System.logAction('Asset Assigned');
        $('#modalAssign').modal('hide');
        window.location = window.location;
      }, function () {
        alert('There was an error assigning this asset.  Please try again.');
      });
    }
    else {
      if (!$('#inputCustomer').data('item')) {
        alert("Please select a valid customer or contact to assign this asset to.");
      }
      else {
        alert("Please enter a valid date shipped.");
      }
    }
    //    if ($('#reminderDesc').val() != "" && $('#reminderDate').val() != "") {
    //      top.Ts.Services.System.EditReminder(null, top.Ts.ReferenceTypes.Organizations, organizationID, $('#reminderDesc').val(), top.Ts.Utils.getMsDate($('#reminderDate').val()), $('#reminderUsers').val());
    //      $('#modalReminder').modal('hide');
    //    }
    //    else
    //      alert("Please fill in all the fields");
  });

  $('#btnSaveReturn').click(function (e) {
    if ($('#dateShipped').val()) {
      var confirmed = true;
      if (_assetAssignments && _assetAssignments.length > 1) {
        confirmed = confirm("Returning the asset will unassigned it from all customers. Are you sure you want to proceed?");
      }
      if (confirmed) {
        var assetReturnInfo = new Object();

        assetReturnInfo.DateShipped = $('#dateShipped').val();
        assetReturnInfo.TrackingNumber = $('#trackingNumber').val();
        assetReturnInfo.ShippingMethod = $('#shippingMethod').val();
        assetReturnInfo.ReferenceNumber = $('#referenceNumber').val();
        assetReturnInfo.Comments = $('#comments').val();

        top.Ts.Services.Assets.ReturnAsset(_assetID, top.JSON.stringify(assetReturnInfo), function (assetID) {
          top.Ts.System.logAction('Asset Returned');
          $('#modalAssign').modal('hide');
          window.location = window.location;
        }, function () {
          alert('There was an error returning this asset.  Please try again.');
        });
      }
      else {
        window.location = window.location;
      }
    }
    else {
      alert("Please enter a valid date shipped.");
    }
  });

  $('#fieldName').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Asset Detail - Name clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
            top.Ts.System.logAction('Asset Detail - Change Name cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            top.Ts.Services.Assets.SetAssetName(_assetID, $(this).prev().find('input').val(), function (result) {
              header.text(result);
              $('#assetTitle').text(result);
              $('#assetEdit').removeClass("disabled");
              top.Ts.System.logAction('Asset Detail - Name changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset name.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldProduct').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Asset Detail - Product clicked');
    var header = $(this).hide();

    var container = $('<div>')
        .insertAfter(header);

    var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

    var select = $('<select>').addClass('form-control').attr('id', 'ddlProduct').appendTo(container1);

    var products = top.Ts.Cache.getProducts();

    for (var i = 0; i < products.length; i++) {
      var opt = $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]);
      if (header.data('productID') == products[i].ProductID)
        opt.attr('selected', 'selected');
      opt.appendTo(select);
    }


    $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show();
          $('#customerEdit').removeClass("disabled");
          top.Ts.System.logAction('Asset Detail - Product changed cancelled');
        })
        .insertAfter(container1);
    $('#ddlProduct').on('change', function () {
      var value = $(this).val();
      var name = this.options[this.selectedIndex].innerHTML;
      container.remove();

      top.Ts.Services.Assets.SetAssetProduct(_assetID, value, function (result) {
        header.data('productID', result);
        header.text(name);
        header.show();
        $('#assetEdit').removeClass("disabled");
        top.Ts.System.logAction('Asset Detail - Product changed.');
        var product = top.Ts.Cache.getProduct(result);
        if (product.Versions.length == 0) {
          $('#fieldProductVersion').text('N/A');
          $('#fieldProductVersion').removeClass("editable");
        }
        else {
          $('#fieldProductVersion').text('Unassigned');
          $('#fieldProductVersion').addClass("editable");
        }
      });
    });
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldProductVersion').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Asset Detail - Product Version clicked');
    var header = $(this).hide();

    var container = $('<div>')
        .insertAfter(header);

    var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

    var select = $('<select>').addClass('form-control').attr('id', 'ddlProductVersion').appendTo(container1);

    var product = top.Ts.Cache.getProduct($('#fieldProduct').data('productID'));
    for (var i = 0; i < product.Versions.length; i++) {
      var opt = $('<option>').attr('value', product.Versions[i].ProductVersionID).text(product.Versions[i].VersionNumber).data('o', product.Versions[i]);
      if (header.data('productVersionID') == product.Versions[i].ProductVersionID)
        opt.attr('selected', 'selected');
      opt.appendTo(select);
    }


    $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
          $(this).closest('div').remove();
          header.show();
          $('#customerEdit').removeClass("disabled");
          top.Ts.System.logAction('Asset Detail - Product Version changed cancelled');
        })
        .insertAfter(container1);
    $('#ddlProductVersion').on('change', function () {
      var value = $(this).val();
      var name = this.options[this.selectedIndex].innerHTML;
      container.remove();

      top.Ts.Services.Assets.SetAssetProductVersion(_assetID, value, function (result) {
        header.data('productVersionID', result);
        header.text(name);
        header.show();
        $('#assetEdit').removeClass("disabled");
        top.Ts.System.logAction('Asset Detail - Product Version changed.');
      });
    });
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldSerialNumber').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Asset Detail - Serial Number clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<input type="text">')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
            top.Ts.System.logAction('Asset Detail - Serial Number change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            top.Ts.Services.Assets.SetAssetSerialNumber(_assetID, $(this).prev().find('input').val(), function (result) {
              header.text(result);
              $('#assetEdit').removeClass("disabled");
              top.Ts.System.logAction('Asset Detail - Serial Number changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset name.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  $('#fieldNotes').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Asset Detail - Notes clicked');
    var header = $(this).hide();
    var container = $('<div>')
          .insertAfter(header);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    $('<textarea>')
          .addClass('col-xs-10 form-control')
          .val($(this).text())
          .appendTo(container1)
          .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#customerEdit').removeClass("disabled");
            top.Ts.System.logAction('Asset Detail - Notes change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            top.Ts.Services.Assets.SetAssetNotes(_assetID, $(this).prev().find('textarea').val(), function (result) {
              header.text(result);
              $('#assetEdit').removeClass("disabled");
              top.Ts.System.logAction('Asset Detail - Notes changed');
            },
            function (error) {
              header.show();
              alert('There was an error saving the asset notes.');
              $('#assetEdit').removeClass("disabled");
            });
            $('#assetEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
          })
          .insertAfter(container1);
    $('#assetEdit').addClass("disabled");
  });

  var getCustomers = function (request, response) {
    if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
    _execGetCustomer = top.Ts.Services.Organizations.GetUserOrOrganizationForTicket(request.term, function (result) { response(result); });
  }

  $('#inputCustomer').autocomplete({
    open: function () {
      $('.ui-menu').width($('#inputCustomer').width());
    },
    minLength: 2,
    source: getCustomers,
    defaultDate: new Date(),
    select: function (event, ui) {
      $(this).data('item', ui.item);
    }
  });

  $("#dateShipped").datetimepicker();

  $('#historyToggle').on('click', function () {
    if (historyLoaded == 0) {
      historyLoaded = 1;
      LoadHistory(1);
    }
  });

  $('#historyRefresh').on('click', function () {
    LoadHistory(1);
  });

  function LoadHistory(start) {

    if (start == 1)
      $('#tblHistory tbody').empty();

    top.Ts.Services.Assets.LoadHistory(_assetID, start, function (history) {
      for (var i = 0; i < history.length; i++) {
        var nameAssignedToValue = history[i].NameAssignedTo == null ? '' : history[i].NameAssignedTo;
        $('<tr>').html('<td>' +
        history[i].ActionDescription + '</td><td>' +
        history[i].ActionTime.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' +
        history[i].ActorName + '</td><td>' +
        history[i].Comments + '</td><td>' +
        nameAssignedToValue + '</td><td>' +
        history[i].ShippingMethod + '</td><td>' +
        history[i].TrackingNumber + '</td><td>' +
        history[i].ReferenceNum + '</td><td>' +
        '</td>').appendTo('#tblHistory > tbody:last');
      }
      if (history.length == 50)
        $('<button>').text("Load More").addClass('btn-link')
                    .click(function (e) {
                      LoadHistory($('#tblHistory tbody > tr').length + 1);
                      $(this).remove();
                    })
                   .appendTo('#tblHistory > tbody:last');
    });
  }


});

function onShow() {
  _assetDetailPage.refresh();
};

AssetDetailPage = function () {

};

AssetDetailPage.prototype = {
  constructor: AssetDetailPage,
  refresh: function () {

  }
};

