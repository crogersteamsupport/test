var _productVersionID = null;
var _productName = null;
var _versionNumber = null;

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
    _productName = productVersion.ProductName;
    _versionNumber = productVersion.VersionNumber;
    $('#productVersionNumber').text(productVersion.ProductName + " - " + productVersion.VersionNumber);
    $('#fieldDescription').html(productVersion.Description != null && productVersion.Description != "" ? productVersion.Description : "Empty");
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

  $('.version-action-add').click(function (e) {
      e.preventDefault();
      top.Ts.MainPage.newProduct("product", _productID);
  });

});

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
        top.Ts.System.logAction('Product Detail - Edit Custom Combobox');
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
            top.Ts.System.logAction('Product Detail - Save Custom Edit Change');
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
        top.Ts.System.logAction('Product Detail - Edit Custom Number');
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
                top.Ts.System.logAction('Product Detail - Save Custom Number Edit');
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
        top.Ts.System.logAction('Product Detail - Edit Custom Boolean Value');
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
          top.Ts.System.logAction('Product Detail - Edit Custom Textbox');
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
                    top.Ts.System.logAction('Product Detail - Save Custom Textbox Edit');
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
        top.Ts.System.logAction('Product Detail - Edit Custom Date');
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
                top.Ts.System.logAction('Product Detail - Save Custom Date Change');
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
        top.Ts.System.logAction('Product Detail - Edit Custom DateTime');
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
                top.Ts.System.logAction('Product Detail - Save Custom DateTime');
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
        top.Ts.System.logAction('Product Detail - Edit Custom Time');
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
                top.Ts.System.logAction('Product Detail - Save Custom Time');
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

