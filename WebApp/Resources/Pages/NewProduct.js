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
  var _organizatinID = -1;
  var _isAdmin = top.Ts.System.User.IsSystemAdmin && (_organizatinID != top.Ts.System.User.OrganizationID);

  if (top.Ts.System.Organization.UseProductFamilies) {
      LoadProductFamilies();
      $('#productFamilyRow').show();
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

  var defaultTab = top.Ts.Utils.getQueryValue("open", window);
  var defaultProduct = top.Ts.Utils.getQueryValue("productID", window);

  $(".maincontainer").on("keypress", "input", (function (evt) {
    //Deterime where our character code is coming from within the event
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

  initEditor($('#inputDescription'), function (ed) {
    $('#inputDescription').tinymce().focus();
  });

  LoadProducts();
  LoadStatuses();

  $('#inputExpectedRelease').datetimepicker({ pickTime: false });

  LoadCustomControls();
  LoadVersionCustomControls();

  function LoadProducts() {
    top.Ts.Services.Products.GetProducts(function (products) {

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
      top.Ts.Services.Organizations.LoadOrgProductFamilies(top.Ts.System.Organization.OrganizationID, function (productFamilies) {
          for (var i = 0; i < productFamilies.length; i++) {
              $('<option>').attr('value', productFamilies[i].ProductFamilyID).text(productFamilies[i].Name).data('o', productFamilies[i]).appendTo('#ddlProductFamily');
          }
      });
  }

  function LoadStatuses() {
    var productVersionStatuses = top.Ts.Cache.getProductVersionStatuses();
    for (var i = 0; i < productVersionStatuses.length; i++) {
      $('<option>').attr('value', productVersionStatuses[i].ProductVersionStatusID).text(productVersionStatuses[i].Name).data('o', productVersionStatuses[i]).appendTo('#ddlStatus');
    }
  }

  function LoadCustomControls() {
    top.Ts.Services.Assets.LoadCustomControls(top.Ts.ReferenceTypes.Products, function (html) {
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
        $('.datetimepicker').datetimepicker({});
      }
    });
  }

  function LoadVersionCustomControls() {
    top.Ts.Services.Assets.LoadCustomControls(top.Ts.ReferenceTypes.ProductVersions, function (html) {
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
      var productInfo = new Object();
      top.Ts.System.logAction('New Product Page - Added New Product');
      productInfo.Name = $("#inputName").val();
      productInfo.Description = $("#Description").val();

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
        productInfo.Fields[productInfo.Fields.length] = field;
      });


      top.Ts.Services.Products.SaveProduct(top.JSON.stringify(productInfo), function (f) {
        top.Ts.MainPage.openNewProduct(f);
        top.Ts.MainPage.closenewProductTab();
      }, function () {
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
      var versionInfo = new Object();
      top.Ts.System.logAction('New Product Page - Added New Product Version');
      versionInfo.VersionNumber = $("#inputVersionNumber").val();
      versionInfo.ProductID = $("#ddlProduct").val();
      versionInfo.ProductVersionStatusID = $("#ddlStatus").val();
      versionInfo.ReleaseDate = $("#inputExpectedRelease").val();
      versionInfo.IsRelease = $("#cbReleased").prop('checked');
      versionInfo.Description = $("#inputDescription").val();

      versionInfo.Fields = new Array();
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
        versionInfo.Fields[versionInfo.Fields.length] = field;
      });


      top.Ts.Services.Products.SaveProductVersion(top.JSON.stringify(versionInfo), function (f) {
        top.Ts.MainPage.openNewProductVersion(f);
        top.Ts.MainPage.closenewProductTab();
      }, function () {
        alert('There was an error saving this product version.  Please try again.');
      });
    }
  });

  $('#productCancelBtn, #productVersionCancelBtn').click(function (e) {
    top.Ts.MainPage.closenewProductTab();
  });

});

var initEditor = function (element, init) {
  top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
    var editorOptions = {
      plugins: "autoresize paste link code textcolor",
      toolbar1: "link unlink | undo redo removeformat | cut copy paste pastetext | code | outdent indent | bullist numlist",
      toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote",
      statusbar: false,
      gecko_spellcheck: true,
      extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
      content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
      body_class: "ui-widget ui-widget-content",

      convert_urls: true,
      remove_script_host: false,
      relative_urls: false,
      template_external_list_url: "tinymce/jscripts/template_list.js",
      external_link_list_url: "tinymce/jscripts/link_list.js",
      external_image_list_url: "tinymce/jscripts/image_list.js",
      media_external_list_url: "tinymce/jscripts/media_list.js",
      menubar: false,
      moxiemanager_image_settings: {
          moxiemanager_rootpath: "/" + top.Ts.System.Organization.OrganizationID + "/images/",
          extensions: 'gif,jpg,jpeg,png'
      },
      setup: function (ed) {
        ed.on('init', function (e) {
          top.Ts.System.refreshUser(function () {
            if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
              ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
            }
            else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
              ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
            }

            if (top.Ts.System.User.FontSize != "0") {
              ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
            }
            else if (top.Ts.System.Organization.FontSize != "0") {
              ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSizeDescription);
            }
          });
        });

        ed.on('paste', function (ed, e) {
          setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
        });
      }
            , oninit: init
    };
    $(element).tinymce(editorOptions);
  });
}

function GetTinyMCEFontName(fontFamily) {
  var result = '';
  switch (fontFamily) {
    case 1:
      result = "'andale mono', times";
      break;
    case 2:
      result = "arial, helvetica, sans-serif";
      break;
    case 3:
      result = "'arial black', 'avant garde'";
      break;
    case 4:
      result = "'book antiqua', palatino";
      break;
    case 5:
      result = "'comic sans ms', sans-serif";
      break;
    case 6:
      result = "'courier new', courier";
      break;
    case 7:
      result = "georgia, palatino";
      break;
    case 8:
      result = "helvetica";
      break;
    case 9:
      result = "impact, chicago";
      break;
    case 10:
      result = "symbol";
      break;
    case 11:
      result = "tahoma, arial, helvetica, sans-serif";
      break;
    case 12:
      result = "terminal, monaco";
      break;
    case 13:
      result = "'times new roman', times";
      break;
    case 14:
      result = "'trebuchet ms', geneva";
      break;
    case 15:
      result = "verdana, geneva";
      break;
    case 16:
      result = "webdings";
      break;
    case 17:
      result = "wingdings, 'zapf dingbats'";
      break;
  }
  return result;
}