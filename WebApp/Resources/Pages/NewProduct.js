/// <reference path="ts/ts.js" />
/// <reference path="ts/parent.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />


$(document).ready(function () {
  var _organizatinID = -1;
  var _isAdmin = parent.Ts.System.User.IsSystemAdmin && (_organizatinID != parent.Ts.System.User.OrganizationID);

  if (parent.Ts.System.Organization.UseProductFamilies) {
      LoadProductFamilies();
      $('#productFamilyRow').show();
  }

  if (parent.Ts.Cache.GetIsJiraLinkActiveForOrganization()) {
    //alert('vv1');
    $('#jiraProjectKeyRow').show();
    $('#productVersionJiraProjectKeyRow').show();
  }

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

  var defaultTab = parent.Ts.Utils.getQueryValue("open", window);
  var defaultProduct = parent.Ts.Utils.getQueryValue("productID", window);

  $(".maincontainer").on("keypress", "input", (function (evt) {
    //Determine where our character code is coming from within the event
    var charCode = evt.charCode || evt.keyCode;
    if (charCode == 13) { //Enter key's keycode
      return false;
    }
  }));

  if (defaultTab) {
    $('#productTabs a:first').tab('show');
    $('#productTabs a:last').tab('show');
    $('#productTabs a:first').hide();
    $('#productsList').hide();
}
  else {
    $('#productTabs a:first').tab('show');
    $('#productTabs a:last').hide();
}

  //initEditor($('#inputDescription'), function (ed) {
  //  $('#inputDescription').tinymce().focus();
  //});

  LoadProducts();
  LoadStatuses();

  

  LoadCustomControls();
  LoadVersionCustomControls();

  function LoadProducts() {
    parent.Ts.Services.Products.GetProducts(function (products) {

        for (var i = 0; i < products.length; i++) {
            if (defaultProduct == products[i].ProductID) {
                $('<option>').attr('value', products[i].ProductID).attr('selected', 'selected').text(products[i].Name).data('o', products[i]).appendTo('#ddlProduct');
            }
            else {
                $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#ddlProduct');
            }
        }
    });
  }

  function LoadProductFamilies() {
      parent.Ts.Services.Organizations.LoadOrgProductFamilies(parent.Ts.System.Organization.OrganizationID, function (productFamilies) {
          for (var i = 0; i < productFamilies.length; i++) {
              $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlProductFamily');
          }
      });
  }

  function LoadStatuses() {
    var productVersionStatuses = parent.Ts.Cache.getProductVersionStatuses();
    for (var i = 0; i < productVersionStatuses.length; i++) {
      $('<option>').attr('value', productVersionStatuses[i].ProductVersionStatusID).text(productVersionStatuses[i].Name).data('o', productVersionStatuses[i]).appendTo('#ddlStatus');
    }
  }

  function LoadCustomControls() {
    parent.Ts.Services.Assets.LoadCustomControls(parent.Ts.ReferenceTypes.Products, function (html) {
      if (html.length < 31) {
        $('#productCustomInfoBox').hide();
      }
      else {
        $('#productCustomInfo').append(html);
        $('.customField:visible').each(function () {
          var maskValue = $(this).attr("placeholder");
          if (maskValue) {
            $(this).mask(maskValue);
          }
        });
        parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
            $('.datepicker').attr("data-format", dateformat);
            
            $('.timepicker').datetimepicker({ pickDate: false });
            $('.datetimepicker').datetimepicker({});
            $('.datepicker').datetimepicker({ pickTime: false });
            $('#inputExpectedRelease').datetimepicker({ pickTime: false, format: dateformat });
        });
      }
    });
  }

  function LoadVersionCustomControls() {
    parent.Ts.Services.Assets.LoadCustomControls(parent.Ts.ReferenceTypes.ProductVersions, function (html) {
      if (html.length < 31) {
        $('#versionCustomInfoBox').hide();
      }
      else {
        $('#versionCustomInfo').append(html);
        $('.customField').each(function () {
          var maskValue = $(this).attr("placeholder");
          if (maskValue) {
            $(this).mask(maskValue);
          }
        });
        $('.datepicker').datetimepicker({ pickTime: false });
        $('.timepicker').datetimepicker({ pickDate: false });
        $('.datetimepicker').datetimepicker({});
      }
    });
  }

  $('#productSaveBtn').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var isValid = $("#productForm").valid();

    if ($("#inputName").val().length < 1) {
      alert("Please enter a name");
      return;
    }

    if (isValid) {
      $('#productSaveBtn').prop("disabled", true);
      var productInfo = new Object();
      parent.Ts.System.logAction('New Product Page - Added New Product');
      productInfo.Name = $("#inputName").val();
      productInfo.Description = $("#Description").val();
      productInfo.JiraProjectKey = $("inputJiraProjectKey").val();

      if ($("#ddlProductFamily").val() != -1) {
          productInfo.ProductFamilyID = $("#ddlProductFamily").val();
      }

      productInfo.Fields = new Array();
      $('.customField:visible').each(function () {
        var field = new Object();
        field.CustomFieldID = $(this).attr("id");
        switch ($(this).attr("type")) {
          case "checkbox":
            field.Value = $(this).prop('checked');
            break;
          case "_date":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate($(this).val());
            break;
          case "_time":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
            break;
          case "_datetime":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate($(this).val());
            break;
          default:
            field.Value = $(this).val();
        }
        productInfo.Fields[productInfo.Fields.length] = field;
      });


      parent.Ts.Services.Products.SaveProduct(parent.JSON.stringify(productInfo), function (f) {
          $('#productSaveBtn').prop("disabled", false);
        parent.Ts.MainPage.openNewProduct(f);
        parent.Ts.MainPage.closenewProductTab();
      }, function () {
          $('#productSaveBtn').prop("disabled", false);
          alert('There was an error saving this product.  Please try again.');
      });
    }
  });

  $('#productVersionSaveBtn').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    var isValid = $("#versionForm").valid();

    if ($("#inputVersionNumber").val().length < 1) {
      alert("Please enter a version number.");
      return;
    }

    if (isValid) {
        $('#productVersionSaveBtn').prop("disabled", true);
      var versionInfo = new Object();
      parent.Ts.System.logAction('New Product Page - Added New Product Version');
      versionInfo.VersionNumber = $("#inputVersionNumber").val();
      versionInfo.ProductID = $("#ddlProduct").val();
      versionInfo.ProductVersionStatusID = $("#ddlStatus").val();
      if ($("#inputExpectedRelease").val() != "")
      	versionInfo.ReleaseDate = moment($("#inputExpectedRelease").val(), dateFormat).format('MM/DD/YYYY');
      else
      	versionInfo.ReleaseDate = "";
      versionInfo.IsReleased = $("#cbReleased").prop('checked');
      versionInfo.Description = $("#inputDescription").val();
      versionInfo.JiraProjectKey = $("#inputProductVersionJiraProjectKey").val();

      versionInfo.Fields = new Array();
      $('.customField:visible').each(function () {
        var field = new Object();
        field.CustomFieldID = $(this).attr("id");
        switch ($(this).attr("type")) {
          case "checkbox":
            field.Value = $(this).prop('checked');
            break;
          case "_date":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate($(this).val());
            break;
          case "_time":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
            break;
          case "_datetime":
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate($(this).val());
            break;
          default:
            field.Value = $(this).val();
        }
        versionInfo.Fields[versionInfo.Fields.length] = field;
      });


      parent.Ts.Services.Products.SaveProductVersion(parent.JSON.stringify(versionInfo), function (f) {
          $('#productVersionSaveBtn').prop("disabled", false);
        parent.Ts.MainPage.openNewProductVersion(f);
        parent.Ts.MainPage.closenewProductTab();
      }, function () {
          $('#productVersionSaveBtn').prop("disabled", false);
        alert('There was an error saving this product version.  Please try again.');
      });
    }
  });

  $('#productCancelBtn, #productVersionCancelBtn').click(function (e) {
    parent.Ts.MainPage.closenewProductTab();
  });

});
