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

  $('#customerRefresh').click(function (e) {
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
    $('#fieldProduct').text(asset.ProductName);
    $('#fieldProductVersion').text(asset.ProductVersionNumber);
    $('#fieldSerialNumber').text(asset.SerialNumber);
    $('#fieldWarrantyExpiration').text(top.Ts.Utils.getMsDate(asset.WarrantyExpiration).localeFormat(top.Ts.Utils.getDatePattern()));
    $('#fieldNotes').text(asset.Notes);

    $('#fieldAssetID').text(asset.AssetID);
    $('#fieldCreator').text(asset.CreatorName);
    $('#fieldDateCreated').text(top.Ts.Utils.getMsDate(asset.DateCreated).localeFormat(top.Ts.Utils.getDateTimePattern()));
    $('#fieldModifier').text(asset.ModifierName);
    $('#fieldDateModified').text(top.Ts.Utils.getMsDate(asset.DateModified).localeFormat(top.Ts.Utils.getDateTimePattern()));
  });

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

