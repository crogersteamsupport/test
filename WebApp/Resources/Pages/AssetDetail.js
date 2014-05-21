/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var assetDetailPage = null;
var assetID = null;

$(document).ready(function () {
  assetDetailPage = new AssetDetailPage();
  assetDetailPage.refresh();
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

  assetID = top.Ts.Utils.getQueryValue("assetid", window);
  var historyLoaded = 0;
  top.privateServices.SetUserSetting('SelectedAssetID', assetID);

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

  top.Ts.Services.Assets.GetAsset(assetID, function (asset) {
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
    $('#fieldProduct').text(asset.ProductID);
    $('#fieldSerialNumber').text(asset.SerialNumber);
    $('#fieldWarrantyExpiration').text(asset.WarrantyExpiration);
    $('#fieldNotes').text(asset.Notes);

    $('#fieldAssetID').text(asset.AssetID);
    $('#fieldCreator').text(asset.CreatorID);
    $('#fieldDateCreated').text(asset.DateCreated);
    $('#fieldModifier').text(asset.ModifierID);
    $('#fieldDateModified').text(asset.DateModified);
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

    top.Ts.Services.Assets.LoadHistory(organizationID, start, function (history) {
      for (var i = 0; i < history.length; i++) {
        $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                    .appendTo('#tblHistory > tbody:last');
        //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
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
  assetDetailPage.refresh();
};

AssetDetailPage = function () {

};

AssetDetailPage.prototype = {
  constructor: AssetDetailPage,
  refresh: function () {

  }
};

