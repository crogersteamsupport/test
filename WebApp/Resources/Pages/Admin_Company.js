/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminPortal = null;
$(document).ready(function () {
  adminPortal = new AdminPortal();
  adminPortal.refresh();
});

function onShow() {
  adminPortal.refresh();
};


AdminPortal = function () {
  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  var height = $(window).height();
  $('iframe').css('height', height * 0.88 | 0);

  $(function () {
      $(".slider").slider({
          range: "min",
          value: 2,
          min: 0,
          max: 10,
          slide: function (event, ui) {
              $(this).next().text("Overall Weight: " + (ui.value*10) + "%");
          },
          stop: function (event, ui) {
              var total = 0;
              var result;

              $('.slider').each(function () {
                  total = total + $(this).slider("value");
              });

              if (total == 10)
              {
                  result = "100%";
                  $('#cdi-total').removeClass('red');
                  $('#recalculate-cdi').removeAttr("disabled");
                  $('.portal-save-panel').show();
              }
                  
              if (total > 10)
              {
                  result = "is greater than 100%, please reconfigure your weights";
                  $('#cdi-total').addClass('red');
                  $('#recalculate-cdi').attr("disabled", "disabled");
                  $('.portal-save-panel').hide();
              }
                  
              if (total < 10)
              {
                  result = "is less than 100%, please reconfigure your weights";
                  $('#cdi-total').addClass('red');
                  $('#recalculate-cdi').attr("disabled", "disabled");
                  $('.portal-save-panel').hide();
              }

              $('#cdi-total').text("Total Weight: " + result);
              
          }
      });

      $("#cdi-green").slider({
          range: "min",
          value: 70,
          min: 0,
          max: 100,
          slide: function (event, ui) {
              $(this).next().text("Upper Limit: " + (ui.value));
          },
          stop: function (event,ui)
          {
              if (ui.value > $("#cdi-yellow").slider('value'))
              {
                  $(this).next().addClass("red");
                  $('.portal-save-panel').hide();
              }
              else{
                  $("#cdi-green").next().removeClass("red");
                  $('.portal-save-panel').show();
              }
          }
      });

      $("#cdi-yellow").slider({
          range: "min",
          value: 85,
          min: 0,
          max: 100,
          slide: function (event, ui) {
              $(this).next().text("Upper Limit: " + (ui.value));
          },
          stop: function (event,ui)
          {
              if (ui.value < $("#cdi-green").slider("value"))
              {
                  $(this).next().addClass("red");
                  $('.portal-save-panel').hide();
              }
              else {
                  $("#cdi-yellow").next().removeClass("red");
                  $(this).next().removeClass("red");
                  $('.portal-save-panel').show();
              }
          }
      });
  });


  //$('a').addClass('ui-state-default ts-link');
  $('button').button();
  $('#tabs').tabs().find('.ui-tabs-panel').addClass('ui-corner-all');
  $('.loading-section').hide().next().show();

  $('.tabs-modifiable input.text, .tabs-modifiable input.checkbox, .tabs-modifiable textarea').click(function (e) {
    $('.portal-save-panel').show();
  });

  $('.portal-save-panel').hide();
  $('.portal-save').click(function (e) { saveValues();  });
  $('.portal-cancel').click(function (e) {
    e.preventDefault();
    $('.portal-save-panel').hide();
    getData();
  });



  function saveValues() {

    var _cdiOption = new top.TeamSupport.Data.CDI_SettingProxy();
    _cdiOption.TotalTicketsWeight = ($('#ttw-slider').slider('value') * .1).toFixed(1);
    _cdiOption.OpenTicketsWeight = $('#otw-slider').slider('value') * .1;
    _cdiOption.Last30Weight = $('#last30-slider').slider('value') * .1;
    _cdiOption.AvgDaysOpenWeight = $('#avgopen-weight').slider('value') * .1;
    _cdiOption.AvgDaysToCloseWeight = $('#avgclose-weight').slider('value') * .1;
    _cdiOption.GreenUpperRange = $("#cdi-green").slider('value');
    _cdiOption.YellowUpperRange = $("#cdi-yellow").slider('value');

    top.Ts.Services.Organizations.SaveCDISettings(_cdiOption, function (result) {
        top.Ts.System.logAction('Admin Company - CDI Settings Saved');
        $('.portal-save-panel').hide();
    });

  }


  loadCDISettings();
  function loadCDISettings()
  {
      top.Ts.Services.Organizations.LoadCDISettings(top.Ts.System.Organization.OrganizationID, function (cdi) {

          if (cdi != null)
            {
              var ttwvalue = cdi.TotalTicketsWeight == null ? '2' : cdi.TotalTicketsWeight * 10;
              $('#ttw-slider').slider('value', ttwvalue);
              $('#ttw-slider').next().text("Overall Weight: " + (ttwvalue * 10) + "%");

              var last30slider = cdi.Last30Weight == null ? '2' : cdi.Last30Weight * 10;
              $('#last30-slider').slider('value', last30slider);
              $('#last30-slider').next().text("Overall Weight: " + (last30slider * 10) + "%");

              var otwslider = cdi.OpenTicketsWeight == null ? '2' : cdi.OpenTicketsWeight * 10;
              $('#otw-slider').slider('value', otwslider);
              $('#otw-slider').next().text("Overall Weight: " + (otwslider * 10) + "%");

              var avgopenweight = cdi.AvgDaysOpenWeight == null ? '2' : cdi.AvgDaysOpenWeight * 10;
              $('#avgopen-weight').slider('value', avgopenweight);
              $('#avgopen-weight').next().text("Overall Weight: " + (avgopenweight * 10) + "%");

              var avgcloseweight = cdi.AvgDaysToCloseWeight == null ? '2' : cdi.AvgDaysToCloseWeight * 10;
              $('#avgclose-weight').slider('value', avgcloseweight);
              $('#avgclose-weight').next().text("Overall Weight: " + (avgcloseweight * 10) + "%");

              var greenlimit = cdi.GreenUpperRange == null ? '70' : cdi.GreenUpperRange;
              $('#cdi-green').slider('value', greenlimit);
              $('#cdi-green').next().text("Upper Limit: " + greenlimit);

              var yellowlimit = cdi.YellowUpperRange == null ? '85' : cdi.YellowUpperRange;
              $('#cdi-yellow').slider('value', yellowlimit);
              $('#cdi-yellow').next().text("Upper Limit: " + yellowlimit);

              var cdistatus = cdi.LastCompute == null ? 'Never' : top.Ts.Utils.getMsDate(cdi.LastCompute);
              
              $('#cdiStatus').html("The CDI runs once per day, and the last time your account processed was: <strong>" + cdistatus.localeFormat(top.Ts.Utils.getDateTimePattern()) + "</strong> To force an update now, please click the force update button below.");
          }
      });

  }

  $('#recalculate-cdi').click(function () {
      top.Ts.Services.Organizations.ResetCDI();
  });
};


AdminPortal.prototype = {
  constructor: AdminPortal,
  refresh: function () {

  }
};
