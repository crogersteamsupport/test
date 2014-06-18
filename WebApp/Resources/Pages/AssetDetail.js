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

  LoadProperties();
  LoadCustomProperties();

  $(".maincontainer").on("keypress", "input", (function (evt) {
    //Deterime where our character code is coming from within the event
    var charCode = evt.charCode || evt.keyCode;
    if (charCode == 13) { //Enter key's keycode
      return false;
    }
  }));

  $('#historyToggle').on('click', function () {
    if (historyLoaded == 0) {
      historyLoaded = 1;
      LoadHistory(1);
    }
  });

  $('#historyRefresh').on('click', function () {
    LoadHistory(1);
  });

  $('#assetTabs a:first').tab('show');

  $('#assetEdit').click(function (e) {
    $('.assetProperties p').toggleClass("editable");
    $('.customProperties p').toggleClass("editable");
    $(this).toggleClass("btn-primary");
    $(this).toggleClass("btn-success");
    $('#assetTabs a:first').tab('show');
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

  top.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
    $('.datepicker').attr("data-format", dateformat);
    $('.datepicker').datetimepicker({ pickTime: false });

    $('#productExpiration').attr("data-format", dateformat);
    $('.datetimepicker').datetimepicker({});
  });

  $('#assetRefresh').click(function (e) {
    window.location = window.location;
  });

  $('#assetAssign').click(function (e) {
    $('#modalAssignTitle').text('Assign asset');
    $('#btnSaveReturn').hide();
    $('#btnSaveAssign').show();
    $('#comments').attr('placeholder', 'Comments about assigning this asset.');
  });

  $('#returnAsset').click(function (e) {
    $('#modalAssignTitle').text('Return asset');
    $('#btnSaveAssign').hide();
    $('#btnSaveReturn').show();
    $('#inputCustomerDiv').hide();
    $('#comments').attr('placeholder', 'Comments about returning this asset.');
  });

  $('#fileToggle').click(function (e) {
    top.Ts.System.logAction('Asset Detail - Toggle File Form');
    $('#fileForm').toggle();
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

  $('#btnJunkSave').click(function (e) {
    top.Ts.Services.Assets.JunkAsset(_assetID, $('#junkingComments').val(), function (assetID) {
      top.Ts.System.logAction('Asset Junked');
      $('#modalJunk').modal('hide');
      window.location = window.location;
    }, function () {
      alert('There was an error assigning this asset to the Junkyard.  Please try again.');
    });
  });

  $("#btnFilesCancel").click(function (e) {
    top.Ts.System.logAction('Asset Detail - Cancel File Form');
    $('.upload-queue').empty();
    $('#attachmentDescription').val('');
    $('#fileForm').toggle();
  });

  $('#btnFilesSave').click(function (e) {
    top.Ts.System.logAction('Asset Detail - Save Files');
    if ($('.upload-queue li').length > 0) {
      $('.upload-queue li').each(function (i, o) {
        var data = $(o).data('data');
        data.formData = { description: $('#attachmentDescription').val().replace(/<br\s?\/?>/g, "\n") };
        data.url = '../../../Upload/AssetAttachments/' + _assetID;
        data.jqXHR = data.submit();
        $(o).data('data', data);
      });
    }
    //$('#fileForm').toggle();
  });

  $('.file-upload').fileupload({
    namespace: 'asset_attachment',
    dropZone: $('.file-upload'),
    add: function (e, data) {
      for (var i = 0; i < data.files.length; i++) {
        var item = $('<li>')
                  .appendTo($('.upload-queue'));

        data.context = item;
        item.data('data', data);

        var bg = $('<div>')
                  .addClass('ts-color-bg-accent')
                  .appendTo(item);

        $('<div>')
                  .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
                  .addClass('filename')
                  .appendTo(bg);

        $('<span>')
                  .addClass('icon-remove')
                  .click(function (e) {
                    e.preventDefault();
                    $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                  })
                  .appendTo(bg);

        $('<span>')
                  .addClass('icon-remove')
                  .hide()
                  .click(function (e) {
                    e.preventDefault();
                    var data = $(this).closest('li').data('data');
                    data.jqXHR.abort();
                  })
                  .appendTo(bg);

        var progress = $('<div>')
                  .addClass('progress progress-striped active')
                  .hide();

        $('<div>')
                    .addClass('progress-bar')
                    .attr('role', 'progressbar')
                    .appendTo(progress);

        progress.appendTo(bg);
      }

    },
    send: function (e, data) {
      if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
        data.context.find('.progress-bar').css('width', '50%');
      }
    },
    fail: function (e, data) {
      if (data.errorThrown === 'abort') return;
      alert('There was an error uploading "' + data.files[0].name + '".');
    },
    progress: function (e, data) {
      data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
    },
    start: function (e, data) {
      $('.progress').show();
      $('.upload-queue .ui-icon-close').hide();
      $('.upload-queue .ui-icon-cancel').show();
    },
    stop: function (e, data) {
      //data.context.find('.progress-bar').css('width', '100%');
      LoadFiles();
      $('.upload-queue').empty();
      $('#attachmentDescription').val('');
      $('#fileForm').toggle();
    }
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

  function LoadCustomProperties() {
    top.Ts.Services.Assets.GetCustomValues(_assetID, function (html) {
      //$('#customProperties').append(html);
      appendCustomValues(html);

    });
  }

  function LoadProperties() {
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
  }

  function LoadHistory(start) {

    if (start == 1)
      $('#tblHistory tbody').empty();

    top.Ts.Services.Assets.LoadHistory(_assetID, start, function (history) {
      for (var i = 0; i < history.length; i++) {
        var nameAssignedFromValue = history[i].NameAssignedFrom == null ? '' : history[i].NameAssignedFrom;
        var nameAssignedToValue = history[i].NameAssignedTo == null ? '' : history[i].NameAssignedTo;
        $('<tr>').html('<td>' +
        history[i].ActionDescription + '</td><td>' +
        history[i].ActionTime.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' +
        history[i].ActorName + '</td><td>' +
        history[i].Comments + '</td><td>' +
        nameAssignedFromValue + '</td><td>' +
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

  $("#dateShipped").datetimepicker();

  $('.customProperties, .assetProperties').on('keydown', '.number', function (event) {
    // Allow only backspace and delete
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
      // let it happen, don't do anything
    }
    else {
      // Ensure that it is a number and stop the keypress
      if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
      }
    }
  });

  $('#asset-overview').on('keydown', '.number', function (event) {
    // Allow only backspace and delete
    if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 190 || event.keyCode == 109 || event.keyCode == 173 || (event.keyCode >= 96 && event.keyCode <= 105)) {
      // let it happen, don't do anything
    }
    else {
      // Ensure that it is a number and stop the keypress
      if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
      }
    }
  });

  $('.assetProperties p').toggleClass("editable");
});

var getUrls = function (input) {
  var source = (input || '').toString();
  var url;
  var matchArray;
  var result = '';

  // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
  var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;

  // Iterate through any URLs in the text.
  while ((matchArray = regexToken.exec(source)) !== null) {
    url = matchArray[0];
    if (url.length > 2 && url.substring(0, 3) == 'www') {
      url = 'http://' + url;
    }
    result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '">' + matchArray[0] + '</a>'
  }

  return result == '' ? input : result;
}

var appendCustomValues = function (fields) {
  if (fields === null || fields.length < 1) {
    $('.customProperties').empty();
    return;
  }
  var containerL = $('#customPropertiesL').empty();
  var containerR = $('#customPropertiesR').empty();


  for (var i = 0; i < fields.length; i++) {
    var item = null;

    var field = fields[i];

    var div = $('<div>').addClass('form-group').data('field', field);
    $('<label>')
          .addClass('col-md-4 control-label')
          .text(field.Name)
          .appendTo(div);

    switch (field.FieldType) {
      case top.Ts.CustomFieldType.Text: appendCustomEdit(field, div); break;
      case top.Ts.CustomFieldType.Date: appendCustomEditDate(field, div); break;
      case top.Ts.CustomFieldType.Time: appendCustomEditTime(field, div); break;
      case top.Ts.CustomFieldType.DateTime: appendCustomEditDateTime(field, div); break;
      case top.Ts.CustomFieldType.Boolean: appendCustomEditBool(field, div); break;
      case top.Ts.CustomFieldType.Number: appendCustomEditNumber(field, div); break;
      case top.Ts.CustomFieldType.PickList: appendCustomEditCombo(field, div); break;
      default:
    }

    if (i % 2)
      containerR.append(div);
    else
      containerL.append(div);

  }
  $('.customProperties p').toggleClass("editable");
  //$('#contactName').toggleClass("editable");
}

var appendCustomEditCombo = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Contact Detail - Edit Custom Combobox');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var select = $('<select>').addClass('form-control').attr('id', field.Name.replace(/\W/g, '')).appendTo(container1);

        var items = field.ListValues.split('|');
        for (var i = 0; i < items.length; i++) {
          var option = $('<option>').text(items[i]).appendTo(select);
          if (fieldValue === items[i]) { option.attr('selected', 'selected'); }
        }

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#customerEdit').removeClass("disabled");
            })
            .insertAfter(container1);

        $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
          var value = $(this).val();
          container.remove();

          if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
            result.parent().addClass('has-error');
          }
          else {
            result.parent().removeClass('has-error');
          }
          top.Ts.System.logAction('Contact Detail - Save Custom Edit Change');
          top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
            parent.closest('.form-group').data('field', result);
            parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
            parent.show();
            $('#contactEdit').removeClass("disabled");
          }, function () {
            alert("There was a problem saving your contact property.");
            $('#contactEdit').removeClass("disabled");
          });
        });

        $('#contactEdit').addClass("disabled");
      });
  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('has-error');
  }
}

var appendCustomEditNumber = function (field, element) {
  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Contact Detail - Edit Custom Number');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-md-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-md-10 form-control number')
            .val(fieldValue)
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-md-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-md-1 fa fa-check')
            .click(function (e) {
              var value = input.val();
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                result.parent().addClass('has-error');
              }
              else {
                result.parent().removeClass('has-error');
              }
              top.Ts.System.logAction('Contact Detail - Save Custom Number Edit');
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                parent.closest('.form-group').data('field', result);
                parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                $('#contactEdit').removeClass("disabled");
              }, function () {
                alert("There was a problem saving your contact property.");
                $('#contactEdit').removeClass("disabled");
              });
              parent.show();
              $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('#contactEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
  }
}

var appendCustomEditBool = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .text((field.Value === null || $.trim(field.Value) === '' ? 'False' : field.Value))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        //$('.form-group').prev().show().next().remove();
        top.Ts.System.logAction('Contact Detail - Edit Custom Boolean Value');
        var parent = $(this);
        var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
          parent.closest('.form-group').data('field', result);
          parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
        }, function () {
          alert("There was a problem saving your contact property.");
        });
      });
}

var appendCustomEdit = function (field, element) {

  var div = $('<div>')
    .addClass('col-md-8')
    .appendTo(element);

  var result = $('<p>')
      .html((field.Value === null || $.trim(field.Value) === '' ? 'Unassigned' : getUrls(field.Value)))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        if ($(this).has('a') && !$(this).hasClass('editable')) {
          return;
        }
        else {
          e.preventDefault();
          if (!$(this).hasClass('editable'))
            return false;
          var parent = $(this).hide();
          top.Ts.System.logAction('Contact Detail - Edit Custom Textbox');
          var container = $('<div>')
                .insertAfter(parent);

          var container1 = $('<div>')
              .addClass('col-md-9')
              .appendTo(container);

          var fieldValue = parent.closest('.form-group').data('field').Value;
          var input = $('<input type="text">')
                .addClass('col-md-10 form-control')
                .val(fieldValue)
                .appendTo(container1)
                .focus();

          if (field.Mask) {
            input.mask(field.Mask);
            input.attr("placeholder", field.Mask);
          }

          $('<i>')
                .addClass('col-md-1 fa fa-times')
                .click(function (e) {
                  $(this).closest('div').remove();
                  parent.show();
                  $('#contactEdit').removeClass("disabled");
                })
                .insertAfter(container1);
          $('<i>')
                .addClass('col-md-1 fa fa-check')
                .click(function (e) {
                  var value = input.val();
                  container.remove();
                  if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    result.parent().addClass('has-error');
                  }
                  else {
                    result.parent().removeClass('has-error');
                  }
                  top.Ts.System.logAction('Contact Detail - Save Custom Textbox Edit');
                  top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                    parent.closest('.form-group').data('field', result);
                    parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                    $('#contactEdit').removeClass("disabled");
                  }, function () {
                    alert("There was a problem saving your contact property.");
                    $('#contactEdit').removeClass("disabled");
                  });
                  parent.show();
                })
                .insertAfter(container1);
          $('#contactEdit').addClass("disabled");
        }
      });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
  }
}

var appendCustomEditDate = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Contact Detail - Edit Custom Date');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getDatePattern()))
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                result.parent().addClass('has-error');
              }
              else {
                result.parent().removeClass('has-error');
              }
              top.Ts.System.logAction('Contact Detail - Save Custom Date Change');
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                parent.closest('.form-group').data('field', result);
                var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                $('#contactEdit').removeClass("disabled");
              }, function () {
                alert("There was a problem saving your contact property.");
                $('#contactEdit').removeClass("disabled");
              });
              parent.show();
            })
            .insertAfter(container1);
        $('#contactEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
  }

}

var appendCustomEditDateTime = function (field, element) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Contact Detail - Edit Custom DateTime');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getDateTimePattern()))
            .datetimepicker({
            })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                result.parent().addClass('has-error');
              }
              else {
                result.parent().removeClass('has-error');
              }
              top.Ts.System.logAction('Contact Detail - Save Custom DateTime');
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                parent.closest('.form-group').data('field', result);
                var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                $('#contactEdit').removeClass("disabled");
              }, function () {
                alert("There was a problem saving your contact property.");
                $('#contactEdit').removeClass("disabled");
              });
              parent.show();
            })
            .insertAfter(container1);
        $('#contactEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
  }

}

var appendCustomEditTime = function (field, element) {
  var date = field.Value == null ? null : field.Value;

  var div = $('<div>')
    .addClass('col-xs-8')
    .appendTo(element);

  var result = $('<p>')
      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
      .addClass('form-control-static editable')
      .appendTo(div)
      .click(function (e) {
        e.preventDefault();
        if (!$(this).hasClass('editable'))
          return false;
        var parent = $(this).hide();
        top.Ts.System.logAction('Contact Detail - Edit Custom Time');
        var container = $('<div>')
            .insertAfter(parent);

        var container1 = $('<div>')
          .addClass('col-xs-9')
          .appendTo(container);

        var fieldValue = parent.closest('.form-group').data('field').Value;
        var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val(fieldValue === null ? '' : fieldValue.localeFormat(top.Ts.Utils.getTimePattern()))
            .datetimepicker({ pickDate: false })

            .appendTo(container1)
            .focus();

        $('<i>')
            .addClass('col-xs-1 fa fa-times')
            .click(function (e) {
              $(this).closest('div').remove();
              parent.show();
              $('#contactEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                result.parent().addClass('has-error');
              }
              else {
                result.parent().removeClass('has-error');
              }
              top.Ts.System.logAction('Contact Detail - Save Custom Time');
              top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, userID, value, function (result) {
                parent.closest('.form-group').data('field', result);
                var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                $('#contactEdit').removeClass("disabled");
              }, function () {
                alert("There was a problem saving your contact property.");
                $('#contactEdit').removeClass("disabled");
              });
              parent.show();
            })
            .insertAfter(container1);
        $('#contactEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
  }

}

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

