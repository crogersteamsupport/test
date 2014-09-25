$(document).ready(function () {
  top.Ts.System.logAction('New Asset - Started');
  $('body').layout({
    defaults: {
      spacing_open: 0,
      closable: false
    },
    north: {
      spacing_open: 1,
      size: 100
    },
    center: {
      maskContents: true
    }
  });

  function LoadProducts() {
    var products = top.Ts.Cache.getProducts();
    for (var i = 0; i < products.length; i++) {
      $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#ddlProduct');
    }
  }

  function LoadProductVersions() {
    $('#ddlProductVersion').empty();
    var product = top.Ts.Cache.getProduct($('#ddlProduct').val());
    for (var i = 0; i < product.Versions.length; i++) {
      $('<option>').attr('value', product.Versions[i].ProductVersionID).text(product.Versions[i].VersionNumber).data('version', product.Versions[i]).appendTo('#ddlProductVersion');
    }
    if (product.Versions.length == 0) {
      $('<option>').text(product.Name + ' has no versions.').appendTo('#ddlProductVersion');
      $('#ddlProductVersion').prop("disabled", true);
    }
    else {
      $('#ddlProductVersion').prop("disabled", false);
    }
  }

  LoadProducts();
  LoadProductVersions();
  LoadCustomControls();

  $('#ddlProduct').change(function (e) {
    LoadProductVersions();
  });

  var userDateFormat = top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern.replace("yyyy", "yy");
  $("#inputWarrantyExpiration").datepicker({ dateFormat: userDateFormat });

  $('#assetSaveBtn').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    if ($("#assetForm").valid()) {
      var assetInfo = new Object();

      assetInfo.Name = $("#inputName").val();
      assetInfo.ProductID = $("#ddlProduct").val();
      if ($('#ddlProductVersion').val().isNumeric) {
        assetInfo.ProductVersionID = $('#ddlProductVersion').val();
      }
      assetInfo.SerialNumber = $("#inputSerialNumber").val();
      assetInfo.WarrantyExpiration = $("#inputWarrantyExpiration").val();
      assetInfo.Notes = $("#Notes").val();

      assetInfo.Fields = new Array();
      $('.customField:visible').each(function () {
        var field = new Object();
        field.CustomFieldID = $(this).attr("id");
        switch ($(this).attr("type")) {
          case "checkbox":
            field.Value = $(this).prop('checked');
            break;
          case "date":
            //    var dt = $(this).find('input').datepicker('getDate');
            field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
            break;
          case "time":
            //    var time = new Date("January 1, 1970 00:00:00");
            //    time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
            //    time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
            field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
            break;
          case "datetime":
            //    //field.Value = top.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
            //    var dt = $(this).find('input').datetimepicker('getDate');
            //    field.Value = dt == null ? null : dt.toUTCString();
            field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
            break;
          default:
            field.Value = $(this).val();
        }
        assetInfo.Fields[assetInfo.Fields.length] = field;
      });


      top.Ts.Services.Assets.SaveAsset(top.JSON.stringify(assetInfo), function (assetID) {
        top.Ts.System.logAction('Asset Created');
        top.Ts.MainPage.openNewAsset(assetID);
        top.Ts.MainPage.closenewAssetTab();
      }, function () {
        alert('There was an error saving this asset.  Please try again.');
      });
    }
  });

  $('#assetCancelBtn').click(function (e) {
    top.Ts.System.logAction('New Asset - Cancelled');
    top.Ts.MainPage.closenewAssetTab();
  });

  function LoadCustomControls() {
    top.Ts.Services.Assets.LoadCustomControls(top.Ts.ReferenceTypes.Assets, function (html) {
      $('#customerCustomInfo').append(html);
      $('.customField:visible').each(function () {
        var maskValue = $(this).attr("placeholder");
        if (maskValue) {
          $(this).mask(maskValue);
        }
      });
    });
  }

  top.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
    //$('.datepicker').datepicker({ format: dateformat });
    //$('.datepicker').datetimepicker({ pickTime: false });
    //The line below breaks the page when the format is different than us
    //    $('.datepicker').attr("data-format", dateformat);
    //    $('.datepicker').attr("data-format", "M/d/yyyy");
    //    $('.datepicker').datepicker("option", "dateFormat", dateformat);
    //    $('.datepicker').datepicker({ dateFormat: dateformatValue });
    //    $('.datepicker').datepicker("option", "altFormat", dateformat);

    $('.datepicker').datetimepicker({ pickTime: false });
    $('.timepicker').datetimepicker({ pickDate: false });
    $('.datetimepicker').datetimepicker({});
  });
});