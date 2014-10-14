var _productVersionID = null;
var _productID = null;
var _productName = null;
var _versionNumber = null;
var _headersLoaded = false;

$(document).ready(function () {
  _productVersionID = top.Ts.Utils.getQueryValue("productversionid", window);

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

  top.Ts.Services.Products.GetVersion(_productVersionID, function (productVersion) {
    _productID = productVersion.ProductID;
    _productName = productVersion.ProductName;
    _versionNumber = productVersion.VersionNumber;
    $('#productVersionNumber').text(productVersion.ProductName + " - " + productVersion.VersionNumber);
    $('#fieldDescription').html(productVersion.Description != null && productVersion.Description != "" ? productVersion.Description : "Empty");
    $('#fieldProduct').html(productVersion.ProductName);
    $('#fieldStatus').html(productVersion.VersionStatus);
    $('#fieldReleased').text((productVersion.IsReleased === true ? 'True' : 'False'));
    $('#fieldReleaseDate').text(top.Ts.Utils.getMsDate(productVersion.ReleaseDate).localeFormat(top.Ts.Utils.getDatePattern()));
  });

  $('.product-version-tooltip').tooltip({ placement: 'bottom', container: 'body' });

  $('.productVersionProperties p').toggleClass("editable");
  $('.productVersionProperties span').toggleClass("editable");

  $('#productVersionEdit').click(function (e) {
    top.Ts.System.logAction('Product Version Detail - Product Version Edit');
    $('.productVersionProperties p').toggleClass("editable");
    $('.productVersionProperties span').toggleClass("editable");
    $('.customProperties p').toggleClass("editable");

    $(this).toggleClass("btn-primary");
    $(this).toggleClass("btn-success");
    $('#productVersionTabs a:first').tab('show');
  });

  $('#productVersionRefresh').click(function (e) {
    top.Ts.System.logAction('Product Version Detail - Refresh Page');
    window.location = window.location;
  });


  $('#productVersionDelete').click(function (e) {
    if (confirm('Are you sure you would like to remove this product version?')) {
      top.Ts.System.logAction('Product Version Detail - Delete Product Version');
      top.privateServices.DeleteVersion(_productVersionID, function (e) {
        if (window.parent.document.getElementById('iframe-mniProducts'))
          window.parent.document.getElementById('iframe-mniProducts').contentWindow.refreshPage();
        top.Ts.MainPage.closeNewProductVersionTab(_productVersionID);
      });
    }
  });

  $('#productVersionTabs a:first').tab('show');

  $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    if (e.target.innerHTML == "Details") {
      createTestChart();
      LoadCustomProperties();
    }
    else if (e.target.innerHTML == "Customers")
      LoadCustomers();
    else if (e.target.innerHTML == "Tickets")
      $('#ticketIframe').attr("src", "../../../Frames/TicketTabsAll.aspx?tf_ProductID=" + _productID);
    else if (e.target.innerHTML == "Watercooler")
      $('#watercoolerIframe').attr("src", "WaterCooler.html?pagetype=1&pageid=" + _productID);
    else if (e.target.innerHTML == "Inventory")
      LoadInventory();
  })

  function createTestChart() {
    var greenLimit, yellowLimit;

    top.Ts.Services.Products.LoadVersionChartData(_productVersionID, true, function (chartString) {

        var chartData = [];
        var dummy = chartString.split(",");
        var openCount=0;

        for (var i = 0; i < dummy.length; i++) {
            chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
            i++
        }

        if (dummy.length == 1) {
            //chartData.pop();
            //chartData.push(["No Open Tickets", 0]);
            //$('#openChart').text("No Open Tickes").addClass("text-center");
            $('#openChart').html("No Open Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Open Tickets");
        }
        else {
            for (var i = 0; i < chartData.length; i++) {
                openCount = openCount + chartData[i][1];
            }
        $('#openChart').highcharts({
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                height: 250,
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Open Tickets ' + openCount,
                style: {
                    "fontSize": "14px"
                }
            },
            tooltip: {
                pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    }
                }
            },
            series: [{
                type: 'pie',
                name: 'Open Tickets',
                data: []
            }]
        });

        var chart = $('#openChart').highcharts();
        chart.series[0].setData(chartData);



        }
    });

    top.Ts.Services.Products.LoadVersionChartData(_productVersionID, false, function (chartString) {

        var chartData = [];
        var dummy = chartString.split(",");
        var closedCount = 0;

        for (var i = 0; i < dummy.length; i++) {
            chartData.push([dummy[i], parseFloat(dummy[i + 1])]);
            i++
        }

        if (dummy.length == 1) {
            //chartData.pop();
            //chartData.push(["No Closed Tickets", 0]);
            //$('#closedChart').text("No Closed Tickets").addClass("text-center");
            $('#closedChart').html("No Closed Tickets<br/><img class='img-responsive' src=../Images/nochart.jpg>").addClass("text-center  chart-header").attr("title", "No Closed Tickets");
        }
        else {
            for (var i = 0; i < chartData.length; i++) {
                closedCount = closedCount + chartData[i][1];
            }
        $('#closedChart').highcharts({
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                height: 250,
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Closed Tickets ' + closedCount,
                style: {
                    "fontSize": "14px"
                }

            },
            tooltip: {
                pointFormat: '{series.name}: {point.y} - <b>{point.percentage:.0f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    }
                }
            },
            series: [{
                type: 'pie',
                name: 'Closed Tickets',
                data: []
            }]
        });

        var chart = $('#closedChart').highcharts();
        chart.series[0].setData(chartData);
        }
    });
  }

  top.Ts.Services.Products.GetProductVersionTickets(_productVersionID, 0, function (e) {
      $('#openTicketCount').text("Open Tickets: " + e);
  });

  top.Ts.Services.Tickets.Load5MostRecentByProductVersionID(_productVersionID, function (tickets) {
      var max = 5;
      if (tickets.length < 5)
          max = tickets.length;


      for (var i = 0; i < max; i++) {
          var div = $('<div>')
        .data('o', tickets[i])
        .addClass('ticket');

          $('<span>')
        .addClass('ts-icon ts-icon-info')
        .attr('rel', '../Tips/Ticket.aspx?TicketID=' + tickets[i].TicketID)
        .appendTo(div);

          var caption = $('<span>')
        .addClass('ticket-name')
        .appendTo(div);

          $('<a>')
        .addClass('ts-link ui-state-defaultx')
        .attr('href', '#')
        .text(tickets[i].TicketNumber + ': ' + ellipseString(tickets[i].Name, 50))
        .appendTo(caption)
        .click(function (e) {

            top.Ts.MainPage.openTicket($(this).closest('.ticket').data('o').TicketNumber, true);
        });


          div.appendTo(tickets[i].IsClosed == false ? '#openTickets' : '#closedTickets');
      }

      if ($('#openTickets .ticket').length < 1) {
          $('<div>')
          .addClass('no-tickets')
          .text('There are no recent tickets to display')
          .appendTo('#openTickets');
      }
  });

  var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

  function LoadCustomProperties() {
    top.Ts.Services.Assets.GetCustomValues(_productVersionID, top.Ts.ReferenceTypes.ProductVersions, function (html) {
      appendCustomValues(html);
    });
  }

  var historyLoaded = 0;

  $('#historyToggle').on('click', function () {
      top.Ts.System.logAction('Product Version Detail - History Toggle');
      if (historyLoaded == 0) {
          historyLoaded = 1;
          LoadHistory(1);
      }
  });

  $('#historyRefresh').on('click', function () {
      top.Ts.System.logAction('Product Version Detail - History Refresh');
          LoadHistory(1);
  });

  function LoadHistory(start) {

      if(start == 1)
          $('#tblHistory tbody').empty();

          top.Ts.Services.Products.LoadVersionHistory(_productVersionID, start, function (history) {
              for (var i = 0; i < history.length; i++) {
                  $('<tr>').html('<td>' + history[i].DateCreated.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td>')
                  .appendTo('#tblHistory > tbody:last');
                  //$('#tblHistory tr:last').after('<tr><td>' + history[i].DateCreated.toDateString() + '</td><td>' + history[i].CreatorName + '</td><td>' + history[i].Description + '</td></tr>');
              }
              if(history.length == 50)
                  $('<button>').text("Load More").addClass('btn-link')
                  .click(function (e){
                      LoadHistory($('#tblHistory tbody > tr').length+1);
                      $(this).remove();
                  })
                  .appendTo('#tblHistory > tbody:last');
          });
  }

  $('#productVersionNumber').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;

      top.Ts.System.logAction('Product Version Detail - Edit Version Number');
      var header = $(this).hide();
      var container = $('<div>')
        .insertAfter(header);

      var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

      $('<input type="text">')
        .addClass('col-xs-10 form-control')
        .val(_versionNumber)
        .appendTo(container1)
        .focus();

      $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#productVersionEdit').removeClass("disabled");
        })
        .insertAfter(container1);
      $('<i>')
        .addClass('col-xs-1 fa fa-check')
        .click(function (e) {
            top.Ts.System.logAction('Product Version Detail - Save Version Number Edit');
            top.Ts.Services.Products.SetVersionNumber(_productVersionID, $(this).prev().find('input').val(), function (result) {
                header.text(result);
                _versionNumber = result;
                $('#productVersionNumber').text(_productName + " - " + result);
                $('#productVersionEdit').removeClass("disabled");
            },
            function (error) {
                header.show();
                alert('There was an error saving the product version number.');
                $('#productVersionEdit').removeClass("disabled");
            });
            $('#productVersionEdit').removeClass("disabled");
            $(this).closest('div').remove();
            header.show();
        })
        .insertAfter(container1);
      $('#productVersionEdit').addClass("disabled");
  });

//  initEditor($('#fieldDesc'), function (ed) {
//      $('#fieldDesc').tinymce().focus();
//  });

  $('#fieldDescription').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;
      var header = $(this).hide();
      top.Ts.System.logAction('Product Version Detail - Edit Description');
      top.Ts.Services.Products.GetVersion(_productVersionID, function (productVersion) {
        var desc = productVersion.Description;
        desc = desc.replace(/<br\s?\/?>/g, "\n");
//        $('#fieldDesc').tinymce().setContent(desc);
//        $('#fieldDesc').tinymce().focus();
        $('#fieldDesc').html(desc);
        $('#descriptionForm').show();
      });

      $('#btnDescriptionCancel').click(function (e) {
        e.preventDefault();
        $('#descriptionForm').hide();
        header.show();
        $('#productVersionEdit').removeClass("disabled");
      });

      $('#btnDescriptionSave').click(function (e) {
        e.preventDefault();
        top.Ts.System.logAction('Product Version Detail - Save Description Edit');
        top.Ts.Services.Products.SetVersionDescription(_productVersionID, $(this).prev().find('textarea').val(), function (result) {
            header.html(result);
            $('#productVersionEdit').removeClass("disabled");
        },
        function (error) {
            header.show();
            alert('There was an error saving the product version description.');
            $('#productVersionEdit').removeClass("disabled");
        });
        $('#descriptionForm').hide();
        header.show();
      })
      $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldProduct').click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Product Version Detail - View Product');
    top.Ts.MainPage.openNewProduct(_productID);

    top.Ts.Services.Products.UpdateRecentlyViewed('p' + _productID, function (resultHtml) {
    });
  });

  $('#fieldStatus').click(function (e) {
      e.preventDefault();
      if (!$(this).hasClass('editable'))
          return false;
      var header = $(this).hide();
      top.Ts.System.logAction('Product Version Detail - Edit Status');
      var container = $('<div>')
        .insertAfter(header);

      var container1 = $('<div>')
          .addClass('col-xs-9')
        .appendTo(container);

      var select = $('<select>').addClass('form-control').attr('id', 'ddlStatus').appendTo(container1);
      top.Ts.Services.Products.LoadVersionStatuses(function (statuses) {
          $('<option>').attr('value', '-1').text('Unassigned').appendTo(select);
          for (var i = 0; i < statuses.length; i++) {
              var opt = $('<option>').attr('value', statuses[i].ProductVersionStatusID).text(statuses[i].Name).data('o', statuses[i]);
              if (header.data('field') == statuses[i].ProductVersionStatusID)
                  opt.attr('selected', 'selected');
              opt.appendTo(select);
          }
      });


      $('<i>')
        .addClass('col-xs-1 fa fa-times')
        .click(function (e) {
            $(this).closest('div').remove();
            header.show();
            $('#productVersionEdit').removeClass("disabled");
        })
        .insertAfter(container1);
      $('#ddlStatus').on('change', function () {
          var value = $(this).val();
          var name = this.options[this.selectedIndex].innerHTML;
          container.remove();
          top.Ts.System.logAction('Product Version Detail - Save Status');
          top.Ts.Services.Products.SetProductVersionStatus(_productVersionID, value, function (result) {
              header.data('field', result);
              header.text(name);
              header.show();
              $('#productVersionEdit').removeClass("disabled");
          }, function () {
              alert("There was a problem saving your status property.");
              $('#productVersionEdit').removeClass("disabled");
          });
      });
      $('#productVersionEdit').addClass("disabled");
  });

  $('#fieldReleased').click(function (e) {
      if (!$(this).hasClass('editable'))
          return false;
      top.Ts.Services.Products.SetProductVersionReleased(_productVersionID, ($(this).text() !== 'true'), function (result) {
          top.Ts.System.logAction('Product Version Detail - Toggle Released State');
          $('#fieldReleased').text((result === true ? 'True' : 'False'));
      },
      function (error) {
          header.show();
          alert('There was an error saving the product version released.');
      });
  });

  $('#fieldReleaseDate').click(function (e) {
    e.preventDefault();
    if (!$(this).hasClass('editable'))
      return false;
    top.Ts.System.logAction('Product Version Detail - Date Released clicked');
    var parent = $(this).hide();
    var container = $('<div>')
          .insertAfter(parent);

    var container1 = $('<div>')
            .addClass('col-xs-9')
          .appendTo(container);

    var input = $('<input type="text">')
            .addClass('col-xs-10 form-control')
            .val($(this).val())
            .datetimepicker({ pickTime: false })
            .appendTo(container1)
            .focus();

    $('<i>')
          .addClass('col-xs-1 fa fa-times')
          .click(function (e) {
            $(this).closest('div').remove();
            parent.show();
            $('#productVersionEdit').removeClass("disabled");
            top.Ts.System.logAction('Product Version Detail - Date Released change cancelled');
          })
          .insertAfter(container1);
    $('<i>')
          .addClass('col-xs-1 fa fa-check')
          .click(function (e) {
            var value = top.Ts.Utils.getMsDate(input.val());
            container.remove();
            top.Ts.Services.Products.SetReleaseDate(_productVersionID, value, function (result) {
              var date = result === null ? null : top.Ts.Utils.getMsDate(result);
              parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
              $('#productVersionEdit').removeClass("disabled");
              top.Ts.System.logAction('Product Version Detail - Date Released Change');
            },
            function (error) {
              parent.show();
              alert('There was an error saving the Product Version Date Released.');
              $('#productVersionEdit').removeClass("disabled");
            });
            $('#productVersionEdit').removeClass("disabled");
            $(this).closest('div').remove();
            parent.show();
          })
          .insertAfter(container1);
    $('#productVersionEdit').addClass("disabled");
  });

  function LoadCustomers() {

      if(!_headersLoaded){
        
          top.Ts.Services.Customers.LoadcustomProductHeaders(function (headers) {
              for (var i = 0; i < headers.length; i++) {
                  var header = headers[i];
                  if(header == 'Product Name') {
                    header = 'Customer';
                  }
                  $('#tblCustomers th:last').after('<th>' + header + '</th>');
              }
              _headersLoaded = true;
          });
          }

      $('#tblCustomers tbody').empty();

      top.Ts.Services.Products.LoadVersionCustomers(_productVersionID, function (product) {
          for (var i = 0; i < product.length; i++) {
              var customfields = "";
              for (var p = 0; p < product[i].CustomFields.length; p++)
              {
                  customfields = customfields + "<td>" + product[i].CustomFields[p]  + "</td>";
              }

              var html;

              if(top.Ts.System.User.CanEditCompany || _isAdmin)
              {
                  html = '<td><i class="fa fa-edit customerEdit"></i></td><td><i class="fa fa-trash-o customerDelete"></i></td><td><i class="fa fa-folder-open customerView"></i></td><td>' + product[i].Customer + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td>' + customfields;
              }
              else
              {
                  html = '<td></td><td></td><td><i class="fa fa-folder-open customerView"></i></td><td>' + product[i].Customer + '</td><td>' + product[i].SupportExpiration + '</td><td>' + product[i].VersionStatus + '</td><td>' + product[i].IsReleased + '</td><td>' + product[i].ReleaseDate + '</td>' + customfields
              }
              var tr = $('<tr>')
              .attr('id', product[i].OrganizationProductID)
              .html(html)
              .appendTo('#tblCustomers > tbody:last');


          }
      });

  }

  var getCustomers = function (request, response) {
    if (_execGetCustomer) { _execGetCustomer._executor.abort(); }
    _execGetCustomer = top.Ts.Services.Organizations.GetOrganizationForTicket(request.term, function (result) { response(result); });
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

  top.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
      $('.datepicker').attr("data-format", dateformat);
      $('.datepicker').datetimepicker({ pickTime: false });

      $('#productExpiration').attr("data-format", dateformat);
      $('.datetimepicker').datetimepicker({ });
  });

  LoadCustomControls(top.Ts.ReferenceTypes.OrganizationProducts);
  function LoadCustomControls(refType) {
      top.Ts.Services.Customers.LoadCustomControls(refType, function (html) {
          $('#customProductsControls').append(html);

          $('#customProductsControls .datepicker').datetimepicker({ pickTime: false });
          $('#customProductsControls .datetimepicker').datetimepicker({});
          $('#customProductsControls .timepicker ').datetimepicker({ pickDate: false });
      });
  }

  $('#btnCustomerSave').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      top.Ts.System.logAction('Product Version Detail - Save Customer');
      var productInfo = new Object();
      var hasError = 0;
      productInfo.OrganizationID = $('#inputCustomer').data('item').id;
      productInfo.ProductID = _productID;
      productInfo.Version = _productVersionID;
      productInfo.SupportExpiration = $("#supportExpiration").val();
      productInfo.OrganizationProductID = $('#fieldOrganizationProductID').val();

      productInfo.Fields = new Array();
      $('.customField:visible').each(function () {
          var field = new Object();
          field.CustomFieldID = $(this).attr("id");

          if ($(this).hasClass("required") && ($(this).val() === null || $.trim($(this).val()) === '')) {
              $(this).parent().addClass('has-error');
              hasError = 1;
          }
          else {
              $(this).parent().removeClass('has-error');
          }

          switch ($(this).attr("type")) {
              case "checkbox":
                  field.Value = $(this).prop('checked');
                  break;
              //case "date":
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
              //    break;
              //case "time":
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
              //    break;
              //case "datetime":
              //    field.Value = $(this).val() == "" ? null : top.Ts.Utils.getMsDate($(this).val());
              //    break;
              default:
                  field.Value = $(this).val();
          }
          productInfo.Fields[productInfo.Fields.length] = field;
      });

      if (hasError == 0)
      {
          top.Ts.Services.Customers.SaveProduct(top.JSON.stringify(productInfo), function (prod) {
              LoadCustomers();
              $('#productExpiration').val('');
              $('#fieldOrganizationProductID').val('-1');
              $('.customField:visible').each(function () {
                  switch ($(this).attr("type")) {
                      case "checkbox":
                          $(this).prop('checked', false);
                          break;
                      default:
                          $(this).val('');
                  }
              });
              $('#customerForm').toggle();
          }, function () {
              alert('There was an error saving this product version association. Please try again.');
          });
      }

  });

  $("#btnCustomerCancel").click(function (e) {
      e.preventDefault();
      top.Ts.System.logAction('Product Version Detail - Cancel Customer Edit');
      $('#productExpiration').val('');
      $('#fieldOrganizationProductID').val('-1');
      $('.customField:visible').each(function () {
          switch ($(this).attr("type")) {
              case "checkbox":
                  $(this).prop('checked',false);
                  break;
              default:
                  $(this).val('');
          }
      });
      $('#customerForm').toggle();
  });

  $('#tblCustomers').on('click', '.customerEdit', function (e) {
      e.preventDefault();
      var organizationProductID = $(this).parent().parent().attr('id');
      //var orgproductID;
      top.Ts.System.logAction('Product Version Detail - Edit Customer');
      top.Ts.Services.Products.LoadCustomer(organizationProductID, function (organizationProduct) {
          //orgproductID = prod.OrganizationProductID;
          var item = new Object();
          item.label = organizationProduct.Customer;
          item.value = organizationProduct.Customer;
          item.id = organizationProduct.OrganizationID;
          item.data = "o";
          $('#inputCustomer').data('item', item);
          $('#inputCustomer').val(organizationProduct.Customer);
          $('#supportExpiration').val(organizationProduct.SupportExpiration);
          $('#fieldOrganizationProductID').val(organizationProductID);
          top.Ts.Services.Customers.LoadCustomProductFields(organizationProductID, function (custField) {
              for (var i = 0; i < custField.length; i++) {
                  if (custField[i].FieldType == 2)
                      $('#' + custField[i].CustomFieldID).attr('checked', custField[i].Value);
                  //else if (custField[i].FieldType == 5)
                  //{
                  //    var date = field.value == null ? null : top.Ts.Utils.getMsDate(field.Value);
                  //    $('#' + custField[i].CustomFieldID).val(date.localeFormat(top.Ts.Utils.getDatePattern()));
                  //}
                        
                  else
                      $('#' + custField[i].CustomFieldID).val(custField[i].Value);
              }
          });
      });
      $('#customerForm').show();
  });

  $('#tblCustomers').on('click', '.customerDelete', function (e) {
      e.preventDefault();
      if (confirm('Are you sure you would like to remove this customer association?')) {
          top.Ts.System.logAction('Product Version Detail - Delete Customer');
          top.privateServices.DeleteOrganizationProduct($(this).parent().parent().attr('id'), function (e) {
              LoadCustomers();
          });
            
      }
  });

  $('#tblCustomers').on('click', '.customerView', function (e) {
      e.preventDefault();
      top.Ts.System.logAction('Product Version Detail - View Customer');
      top.Ts.MainPage.openProductOrganization($(this).parent().parent().attr('id'))
      //top.location = "../../../Default.aspx?OrganizationProductID=" + ;

  });

  function LoadInventory() {
      $('.assetList').empty();
      top.Ts.Services.Products.LoadAssets(_productID, function (assets) {
          $('.assetList').append(assets)
      });
  }

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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Combobox');
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);

        $('#' + field.Name.replace(/\W/g, '')).on('change', function () {
          var value = $(this).val();
          container.remove();

          if (field.IsRequired && field.IsFirstIndexSelect == true && $(this).find('option:selected').index() < 1) {
            alert("This field is required and the first value is not a valid selection for a required field.");
          }
          else {
            top.Ts.System.logAction('Product Version Detail - Save Custom Edit Change');
            top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
              parent.closest('.form-group').data('field', result);
              parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
              $('#productEdit').removeClass("disabled");
            }, function () {
              alert("There was a problem saving your product property.");
              $('#productEdit').removeClass("disabled");
            });
            result.parent().removeClass('has-error');
            result.removeClass('form-control');
            result.parent().children('.help-block').remove();
          }
          parent.show();
          $('#productEdit').removeClass("disabled");
        });

        $('#productEdit').addClass("disabled");
      });
  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Number');
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-md-1 fa fa-check')
            .click(function (e) {
              var value = input.val();
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Number Edit');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  parent.text((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : result.Value));
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Boolean Value');
        var parent = $(this);
        var value = $(this).text() === 'No' || $(this).text() === 'False' ? true : false;
        top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
          parent.closest('.form-group').data('field', result);
          parent.text((result.Value === null || $.trim(result.Value) === '' ? 'False' : result.Value));
        }, function () {
          alert("There was a problem saving your product property.");
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
          top.Ts.System.logAction('Product Version Detail - Edit Custom Textbox');
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
                  $('#productEdit').removeClass("disabled");
                })
                .insertAfter(container1);
          $('<i>')
                .addClass('col-md-1 fa fa-check')
                .click(function (e) {
                  var value = input.val();
                  container.remove();
                  if (field.IsRequired && (value === null || $.trim(value) === '')) {
                    alert("This field is required");
                  }
                  else {
                    top.Ts.System.logAction('Product Version Detail - Save Custom Textbox Edit');
                    top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
                      parent.closest('.form-group').data('field', result);
                      parent.html((result.Value === null || $.trim(result.Value) === '' ? 'Unassigned' : getUrls(result.Value)));
                      $('#productEdit').removeClass("disabled");
                    }, function () {
                      alert("There was a problem saving your product property.");
                      $('#productEdit').removeClass("disabled");
                    });
                    result.parent().removeClass('has-error');
                    result.removeClass('form-control');
                    result.parent().children('.help-block').remove();
                  }
                  parent.show();
                })
                .insertAfter(container1);
          $('#productEdit').addClass("disabled");
        }
      });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Date');
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Date Change');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom DateTime');
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate(input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom DateTime');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
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
        top.Ts.System.logAction('Product Version Detail - Edit Custom Time');
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
              $('#productEdit').removeClass("disabled");
            })
            .insertAfter(container1);
        $('<i>')
            .addClass('col-xs-1 fa fa-check')
            .click(function (e) {
              var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
              container.remove();
              if (field.IsRequired && (value === null || $.trim(value) === '')) {
                // Currently there is no way to clear a Date.
                // If ever implemented this alert will prevent clearing a required date.
                alert("This field is required");
              }
              else {
                top.Ts.System.logAction('Product Version Detail - Save Custom Time');
                top.Ts.Services.System.SaveCustomValue(field.CustomFieldID, _productID, value, function (result) {
                  parent.closest('.form-group').data('field', result);
                  var date = result.Value === null ? null : top.Ts.Utils.getMsDate(result.Value);
                  parent.text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                  $('#productEdit').removeClass("disabled");
                }, function () {
                  alert("There was a problem saving your product property.");
                  $('#productEdit').removeClass("disabled");
                });
                result.parent().removeClass('has-error');
                result.removeClass('form-control');
                result.parent().children('.help-block').remove();
              }
              parent.show();
            })
            .insertAfter(container1);
        $('#productEdit').addClass("disabled");
      });
  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    result.parent().addClass('has-error');
    result.addClass('form-control');
    if (result.parent().children('.help-block').length == 0) {
      $('<label>')
        .addClass('help-block')
        .text('This field is required')
        .appendTo(result.parent());
    }
  }

}

$.fn.autoGrow = function () {
    return this.each(function () {
        // Variables
        var colsDefault = 130; //this.cols;
        var rowsDefault = this.rows;

        //Functions
        var grow = function () {
            growByRef(this);
        }

        var growByRef = function (obj) {
            var linesCount = 0;
            var lines = obj.value.split('\n');

            for (var i = lines.length - 1; i >= 0; --i) {
                linesCount += Math.floor((lines[i].length / colsDefault) + 1);
            }

            if (linesCount > rowsDefault)
                obj.rows = linesCount + 1;
            else
                obj.rows = rowsDefault;
        }

        var characterWidth = function (obj) {
            var characterWidth = 0;
            var temp1 = 0;
            var temp2 = 0;
            var tempCols = obj.cols;

            obj.cols = 1;
            temp1 = obj.offsetWidth;
            obj.cols = 2;
            temp2 = obj.offsetWidth;
            characterWidth = temp2 - temp1;
            obj.cols = tempCols;

            return characterWidth;
        }

        // Manipulations
        //this.style.width = "auto";
        this.style.height = "auto";
        this.style.overflow = "hidden";
        //this.style.width = ((characterWidth(this) * this.cols) + 6) + "px";
        this.onkeyup = grow;
        this.onfocus = grow;
        this.onblur = grow;
        growByRef(this);
    });
};

