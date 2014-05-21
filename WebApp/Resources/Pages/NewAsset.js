$(document).ready(function () {
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

  LoadProducts();

  function LoadProducts() {
    var products = top.Ts.Cache.getProducts();
    for (var i = 0; i < products.length; i++) {
      $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#ddlProduct');
    }
  }

  var userDateFormat = top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern.replace("yyyy", "yy");
  $("#inputWarrantyExpiration").datepicker({ dateFormat: userDateFormat });

  $('#assetSaveBtn').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var assetInfo = new Object();

    assetInfo.Name = $("#inputName").val();
    assetInfo.ProductID = $("#ddlProduct").val();
    assetInfo.SerialNumber = $("#inputSerialNumber").val();
    assetInfo.WarrantyExpiration = $("#inputWarrantyExpiration").val();
    assetInfo.Notes = $("#Notes").val();

    //    assetInfo.Fields = new Array();
    //    $('.customField:visible').each(function () {
    //      var field = new Object();
    //      field.CustomFieldID = $(this).attr("id");
    //      switch ($(this).attr("type")) {
    //        case "checkbox":
    //          field.Value = $(this).prop('checked');
    //          break;
    //        case "date":
    //          //    var dt = $(this).find('input').datepicker('getDate');
    //          field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
    //          break;
    //        case "time":
    //          //    var time = new Date("January 1, 1970 00:00:00");
    //          //    time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
    //          //    time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
    //          field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
    //          break;
    //        case "datetime":
    //          //    //field.Value = top.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
    //          //    var dt = $(this).find('input').datetimepicker('getDate');
    //          //    field.Value = dt == null ? null : dt.toUTCString();
    //          field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
    //          break;
    //        default:
    //          field.Value = $(this).val();
    //      }
    //      customerInfo.Fields[customerInfo.Fields.length] = field;
    //    });


    top.Ts.Services.Assets.SaveAsset(top.JSON.stringify(assetInfo), function (assetID) {
      top.Ts.MainPage.openNewAsset(assetID);
      //      top.Ts.MainPage.closenewCustomerTab();
    }, function () {
      alert('There was an error saving this customer.  Please try again.');
    });
  });
  $('#assetCancelBtn').click(function (e) {
    top.Ts.MainPage.closenewCustomerTab();
  });

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