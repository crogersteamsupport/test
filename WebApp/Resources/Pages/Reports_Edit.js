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
  var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
  var _tempReport = new Object();
  var location = window.location;
  var _report = null;
  var _cats = null;
  var _fields = null;
  var _subID = -1;

  $('.report-filter').reportFilter();

  if (_reportID) {
    top.Ts.Utils.webMethod("ReportService", "GetReport", { "reportID": _reportID }, function (report) {
      _report = report;

      $('.report-name').val(report.Name);
      $('.report-description').val(report.Description);
      switch (report.ReportType) {
        case 0:
          $('#radioTabular').prop('checked', true);
          _report.Def = JSON.parse(report.ReportDef);
          break;
        case 1:
          $('#radioChart').prop('checked', true);
          _report.Def = JSON.parse(report.ReportDef);
          break;
        case 2:
          $('#radioExternal').prop('checked', true);
          $('#external-url').val(report.ReportDef);
          break;
        case 4:
          $('#radioSummary').prop('checked', true);
          _report.Def = JSON.parse(report.ReportDef);
          break;
        default:
      }
      initReport();
    });
  }
  else {
    initReport();
  }


  function initReport() {
    top.Ts.Services.Reports.GetCategories(loadCats);
    $('.report-info').show();


    $('.action-cancel').click(function (e) {
      e.preventDefault();
      var returnURL = top.Ts.Utils.getQueryValue('ReturnURL', window);
      if (returnURL == null) returnURL = '/vcr/1_7_0/pages/reports.html';
      location.assign(returnURL);
    });

    $('.action-next').click(function (e) {
      e.preventDefault();
      var visibleSection = $('.report-section:visible');
      var reportType = $(".report-type input[type='radio']:checked").val();
      if (visibleSection.hasClass('report-info')) {
        if ($('.report-name').val() == '') {
          $('.report-name').popover('show').parent('.form-group').addClass('has-error');
          return;
        }
        $('.report-name').popover('hide').parent('.form-group').removeClass('has-error');
        $('.action-back').removeClass('disabled').prop('disabled', false);
        $('.report-type').show();
      }
      else if (visibleSection.hasClass('report-type')) {
        if (reportType == 2) $('.report-external').show(); else $('.report-tables').show();
      }
      else if (visibleSection.hasClass('report-charttype')) {
        $('.report-summary').show();
      }
      else if (visibleSection.hasClass('report-tables')) {

        $('.action-next').addClass('disabled').prop('disabled', true);
        if (_report && _report.Def.Subcategory != $('#selectSubCat').val()) { _report = null; }

        if (!(_tempReport.Subcategory && _tempReport.Subcategory == $('#selectSubCat').val())) {
          loadFields();
        }

        switch (reportType) {
          case "0":
            $('.report-fields').show();
            break;
          case "1":
            $('.action-next').removeClass('disabled').prop('disabled', false);
            $('.report-charttype').show();
            break;
          case "4":
            $('.action-next').removeClass('disabled').prop('disabled', false);
            $('.report-summary').show();
            break;
          default:

        }

      }
      else if (visibleSection.hasClass('report-summary')) {
        initFilters();
        $('.report-filters').show();
      }
      else if (visibleSection.hasClass('report-fields')) {
        initFilters();
        $('.report-filters').show();
      }
      visibleSection.hide();

      if ($('.report-section:visible').hasClass('report-wizard-end')) {
        $('.action-next').hide();
        $('.action-save').show();
      }
    });

    $('.action-back').click(function (e) {
      e.preventDefault();
      var reportType = $(".report-type input[type='radio']:checked").val();

      $('.action-save').hide();
      $('.action-next').show();
      $('.action-next').removeClass('disabled').prop('disabled', false);

      var visibleSection = $('.report-section:visible');
      visibleSection.hide();

      if (visibleSection.hasClass('report-type')) {
        $('.report-info').show();
        $('.action-back').addClass('disabled').prop('disabled', true);
      }
      else if (visibleSection.hasClass('report-external')) {
        $('.report-type').show();
      }
      else if (visibleSection.hasClass('report-tables')) {
        $('.report-type').show();
      }
      else if (visibleSection.hasClass('report-summary')) {
        if (reportType == "1") $('.report-charttype').show(); else $('.report-tables').show();
      }
      else if (visibleSection.hasClass('report-charttype')) {
        $('.report-tables').show();
      }
      else if (visibleSection.hasClass('report-fields')) {
        $('.report-tables').show();
      }
      else if (visibleSection.hasClass('report-filters')) {
        switch (reportType) {
          case "0":
            $('.report-fields').show();
            break;
          case "1":
            $('.report-summary').show();
            break;
          case "4":
            $('.report-summary').show();
            break;
          default:
        }
      }
    });

    function loadCats(cats) {
      _cats = cats;
      for (var i = 0; i < cats.length; i++) {
        var option = $('<option/>').attr('value', cats[i].ReportTableID).text(cats[i].Name);
        if (_report != null && findSubCat(cats[i].Subcategories, _report.Def.Subcategory) != null) {
          option.prop('selected', true);
        }
        $('#selectCat').append(option);
      }
      loadSubCats();
    }

    function findSubCat(subcats, subcatID) {
      for (var i = 0; i < subcats.length; i++) {
        if (subcats[i].SubCatID == subcatID) return subcats[i];
      }
      return null;
    }

    $('#selectCat').change(loadSubCats);

    function loadSubCats() {
      var reportTableID = $('#selectCat').val();
      $('#selectSubCat').empty();
      //$('#selectSubCat').append($('<option/>').attr('value', '-1').text('None'));
      for (var i = 0; i < _cats.length; i++) {
        if (_cats[i].ReportTableID == reportTableID) {
          for (var j = 0; j < _cats[i].Subcategories.length; j++) {
            var option = $('<option/>').attr('value', _cats[i].Subcategories[j].SubCatID).text(_cats[i].Subcategories[j].Name);
            if (_report != null && _cats[i].Subcategories[j].SubCatID == _report.Def.Subcategory) {
              option.prop('selected', true);
            }
            $('#selectSubCat').append(option);
          }
        }
      }
    }

    function loadFields() {
      var subID = $('#selectSubCat').val();

      if ($('.report-fields-selected li').length > 0) {
        $('.action-next').removeClass('disabled').prop('disabled', false);
        $('.report-fields-hint-order').show();
        $('.report-fields-hint-add').hide();
      }

      if (_subID == subID) return;
      $('.action-next').addClass('disabled').prop('disabled', true);
      $('.report-fields-hint-order').hide();
      $('.report-fields-hint-add').show();

      $('.report-fields-selected ul').empty();

      $('#selectSummaryField').empty();
      $('#selectSummaryCalc').empty();

      _subID = subID;
      _fields = null;
      var list = $('.report-fields-available ul').empty();
      top.Ts.Services.Reports.GetFields(subID, function (fields) {
        var tableName = "";
        var optGroup = null;
        var optGroupx = null;
        var optGroupy = null;
        for (var i = 0; i < fields.length; i++) {
          delete fields[i]['__type'];
          $('<li>')
          .addClass(getUniqueFieldClass(fields[i]))
          .data('field', fields[i])
          .append($('<div>', { class: 'checkbox' }).append($('<label>', { html: ' <span class="text-muted">' + fields[i].Table + '.</span>' + fields[i].Name }).prepend('<input type="checkbox" />')))
          .appendTo(list);
          if (tableName != fields[i].Table) {
            tableName = fields[i].Table;
            optGroupx = $('<optgroup>').attr('label', tableName).appendTo('#selectSummaryField');
            optGroupy = $('<optgroup>').attr('label', tableName).appendTo('#selectSummaryCalc');
          }

          $('<option>').text(fields[i].Name).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupx);
          $('<option>').text(fields[i].Name).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupy);
        }
        _fields = fields;
        if (_report != null) loadSelectedFields();
        $('.report-filter').reportFilter('loadFields', fields);
        
      });
    }

    function loadSelectedFields() {
      for (var i = 0; i < _report.Def.Fields.length; i++) {
        $('.' + getUniqueFieldClass(_report.Def.Fields[i]) + ' input').prop('checked', true).trigger('change');
      }
    }

    function getUniqueFieldClass(field) {
      var id = field.ID ? field.ID : field.FieldID;
      if (field.IsCustom)
        return 'report-field-id-c' + id;
      else
        return 'report-field-id-s' + id;
    }

    $('.report-fields-available ul').on('change', 'input', function (e) {
      e.preventDefault();
      var el = $(this).closest('li');
      if ($(this).is(":checked") == true) addSelectedField(el); else removeSelectedField(el);
    });

    $('.report-fields-selected ul').on('change', 'input', function (e) {
      e.preventDefault();
      var li = $(this).closest('li');
      var field = li.data('field');
      $('.report-fields-available .' + getUniqueFieldClass(field)).find('input').prop('checked', false);
      li.fadeOut(function () { $(this).remove(); });
    });

    function addSelectedField(el) {
      var list = $('.report-fields-selected ul');
      var field = el.data('field');
      $('<li>')
          .addClass(getUniqueFieldClass(field))
          .data('field', field)
          .html('<span class="text-muted">' + field.Table + '.</span>' + field.Name)
          .prepend($('<i class="icon-ellipsis-vertical">'))
          .appendTo(list);

      list.sortable({ axis: 'y' }).disableSelection();
      $('.report-fields-hint-add').hide();
      $('.report-fields-hint-order').show();
      $('.action-next').removeClass('disabled').prop('disabled', false);

    }

    function removeSelectedField(el) {
      $('.report-fields-selected .' + getUniqueFieldClass(el.data('field'))).fadeOut(function () {
        $(this).remove();
        if ($('.report-fields-selected li').length < 1) {
          $('.action-next').addClass('disabled').prop('disabled', true);
          $('.report-fields-hint-order').hide();
          $('.report-fields-hint-add').show();
        }
      });
    }

    $('.action-save').click(function (e) {
      e.preventDefault();
      var reportType = $(".report-section.report-type input[type='radio']:checked").val();
      switch (reportType) {
        case "0":
          var tabData = new Object();
          tabData.Fields = new Array();
          $('.report-fields-selected li').each(function (index) {
            var field = $(this).data('field');
            var fieldItem = new Object();
            fieldItem.FieldID = field.ID;
            fieldItem.IsCustom = field.IsCustom;
            tabData.Fields.push(fieldItem);
          });
          tabData.Subcategory = $('#selectSubCat').val();
          tabData.Filters = $('.report-filter').reportFilter('getObject');
          

          top.Ts.Utils.webMethod("ReportService", "SaveReport",
            {
              "reportID": _reportID,
              "name": $('.report-name').val(),
              "description": $('.report-description').val(),
              "reportType": reportType,
              "data": JSON.stringify(tabData)
            },
            function (reportID) {
              var returnURL = top.Ts.Utils.getQueryValue('ReturnURL', window);
              if (returnURL == null) returnURL = '/vcr/1_7_0/pages/reports.html';
              if (returnURL.toLowerCase().indexOf('reportid=') < 0) {
                returnURL = returnURL + (returnURL.indexOf('?') > -1 ? '&' : '?') + 'ReportID=' + reportID;
              }
              location.assign(returnURL);
            },
            function (error) { alert(error.get_message()); }
            );

          break;
        case "4":
          break;
        case "2":
          top.Ts.Utils.webMethod("ReportService", "SaveReport",
            {
              "reportID": _reportID,
              "name": $('.report-name').val(),
              "description": $('.report-description').val(),
              "reportType": reportType,
              "data": $('#external-url').val()
            }
            , function (reportID) {
              var returnURL = top.Ts.Utils.getQueryValue('ReturnURL', window);
              if (returnURL == null) returnURL = '/vcr/1_7_0/pages/reports.html';
              if (returnURL.toLowerCase().indexOf('reportid=') < 0) {
                returnURL = returnURL + (returnURL.indexOf('?') > -1 ? '&' : '?') + 'ReportID=' + reportID;
              }
              location.assign(returnURL);
            }
           , function (error) { alert(error.get_message()); }
           );
          break;
        case "1":
          break;
        default:
          break;
      }

    });



    function initFilters() {
      if ($('.filter .filter-group').length < 1) {
        if (_report != null && _report.Def.Filters && _report.Def.Filters.length > 0) {
          $('.report-filter').reportFilter('loadFilters', _report.Def.Filters);
        }
      }
    }
  }


  $('.fa-bar-chart-o').click(function (e) {
    e.preventDefault();
    location.assign(location.href);
  });

});


