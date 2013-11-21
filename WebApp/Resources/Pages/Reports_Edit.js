/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var reportEditPage = null;
$(document).ready(function () {
  reportEditPage = new ReportEditPage();
  reportEditPage.refresh();
  top.Ts.Services.Reports.GetCategories(loadCats);
  $('.report-info').show();


  var _cats = null;
  var _fields = null;
  var _report = new Object();
  var _catID = -1;
  var _subID = -1;

  $('.action-cancel').click(function (e) {
    e.preventDefault();
    location = "reports.html";
  });

  $('.action-next').click(function (e) {
    e.preventDefault();
    var visibleSection = $('.report-section:visible');
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
      var reportType = visibleSection.find("input[type='radio']:checked").val();
      _report.ReportType = reportType;
      switch (reportType) {
        case 'tabular':
          $('.report-tables').show();
          if (_cats == null) {
            top.Ts.Services.Reports.GetCategories(loadCats);
          }
          break;
        case 'chart':
          $('.report-tables').show();
          if (_cats == null) {
            top.Ts.Services.Reports.GetCategories(loadCats);
          }
          break;
        case 'summary':
          $('.report-tables').show();
          if (_cats == null) {
            top.Ts.Services.Reports.GetCategories(loadCats);
          }
          break;
        case 'external':
          $('.report-external').show();
          break;
        default:
      }
    }
    else if (visibleSection.hasClass('report-tables')) {
      _report.Category = $('#selectCat').val();
      _report.Subcategory = $('#selectSubCat').val();

      $('.report-fields').show();
      $('.action-next').addClass('disabled').prop('disabled', true);

      loadFields();
    }
    else if (visibleSection.hasClass('report-fields')) {
      _report.Fields = new Array();
      $('.report-fields-selected li').each(function (index) {
        var field = $(this).data('field');
        var fieldItem = new Object();
        fieldItem.FieldID = field.ID;
        fieldItem.IsCustom = field.IsCustom;
        _report.Fields.push(fieldItem);
      });

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
    else if (visibleSection.hasClass('report-fields')) {
      $('.report-tables').show();
    }
    else if (visibleSection.hasClass('report-filters')) {
      $('.report-fields').show();
    }
  });

  function loadCats(cats) {
    _cats = cats;
    for (var i = 0; i < cats.length; i++) {
      $('#selectCat').append($('<option/>').attr('value', cats[i].ReportTableID).text(cats[i].Name));
    }
    loadSubCats();
  }

  $('#selectCat').change(loadSubCats);

  function loadSubCats() {
    var reportTableID = $('#selectCat').val();
    $('#selectSubCat').empty();
    $('#selectSubCat').append($('<option/>').attr('value', '-1').text('None'));
    for (var i = 0; i < _cats.length; i++) {
      if (_cats[i].ReportTableID == reportTableID) {
        for (var j = 0; j < _cats[i].Subcategories.length; j++) {
          $('#selectSubCat').append($('<option/>').attr('value', _cats[i].Subcategories[j].SubCatID).text(_cats[i].Subcategories[j].Name));
        }
      }
    }
  }

  function loadFields() {
    var catID = $('#selectCat').val();
    var subID = $('#selectSubCat').val();

    if ($('.report-fields-selected li').length > 0) {
      $('.action-next').removeClass('disabled').prop('disabled', false);
      $('.report-fields-hint-order').show();
      $('.report-fields-hint-add').hide();
    }

    if (_catID == catID && _subID == subID) return;
    $('.action-next').addClass('disabled').prop('disabled', true);
    $('.report-fields-hint-order').hide();
    $('.report-fields-hint-add').show();

    $('.report-fields-selected ul').empty();
    $('.filter').empty();
    _catID = catID;
    _subID = subID;

    var list = $('.report-fields-available ul').empty();
    top.Ts.Services.Reports.GetFields(catID, subID, function (fields) {
      _fields = fields;
      $('.filter-template-cond .filter-field').empty();
      var tableName = "";
      var optGroup = null;
      for (var i = 0; i < fields.length; i++) {
        delete fields[i]['__type'];
        $('<li>')
          .addClass('report-field-id-' + fields[i].ID)
          .data('field', fields[i])
          .append($('<div>', { class: 'checkbox' }).append($('<label>', { html: ' <span class="text-muted">' + fields[i].Table + '.</span>' + fields[i].Name }).prepend('<input type="checkbox" />')))
          .appendTo(list);
        if (tableName != fields[i].Table) {
          tableName = fields[i].Table;
          optGroup = $('<optgroup>').attr('label', tableName).appendTo('.filter-template-cond .filter-field');
        }

        $('<option>').text(fields[i].Name).data('field', fields[i]).appendTo(optGroup);
      }
    });
  }

  $('.report-fields-available ul').on('change', 'input', function (e) {
    e.preventDefault();
    var el = $(this).closest('li');
    if ($(this).is(":checked") == true) addSelectedField(el); else removeSelectedField(el);
  });

  $('.report-fields-selected ul').on('change', 'input', function (e) {
    e.preventDefault();
    var li = $(this).closest('li');
    var id = li.data('field').ID;
    $('.report-fields-available .report-field-id-' + id).find('input').prop('checked', false);
    li.fadeOut(function () { $(this).remove(); });
  });

  function addSelectedField(el) {
    var list = $('.report-fields-selected ul');
    var field = el.data('field');
    $('<li>')
          .addClass('report-field-id-' + field.ID)
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
    $('.report-fields-selected .report-field-id-' + el.data('field').ID).fadeOut(function () {
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
    $('.filter-output').empty();
    getFilterObject($('.filter'), _report);
    var data = JSON.stringify(_report);
    top.Ts.Services.Reports.SaveReport(null, $('.report-name').val(), $('.report-description').val(), $(".report-section.report-type input[type='radio']:checked").val(), data, function (result) {
    });
    $('.filter-output').text(JSON.stringify(_report, undefined, 3));

    //location = "reports.html";
  });

  function getFilterObject(el, obj) {
    el = $(el);
    obj.Filters = new Array();

    el.find('>.filter-group').each(function () {
      var group = new Object();
      var table = $(this);
      group.Conjunction = table.find('.filter-conj:first').text().toUpperCase();
      group.Conditions = new Array();

      table.find('.filter-conds:first').find('.filter-cond').each(function () {
        var condition = new Object();
        var field = $(this).find(':selected').data('field');
        condition.FieldID = field.ID;
        condition.IsCustom = field.IsCustom;
        var comp = $(this).find('.filter-comp');
        condition.Comparator = comp.val().toUpperCase();
        var next = comp.next();
        if (!next.hasClass('filter-remove-cond')) {
          condition.Value1 = next.val();
        }
        else {
          condition.Value1 = null;
        }
        group.Conditions.push(condition);
      });

      obj.Filters.push(group);
      getFilterObject(table.find('.filter-subs:first'), group);
    });

    return obj;
  }

  $('.icon-bar-chart').click(function (e) {
    e.preventDefault();
    location = "reports_edit.html";
  });

  $('.filter').on('click', '.filter-conj', function (e) {
    e.preventDefault();
    var btn = $(this);
    if (btn.text() == 'And') btn.text('Or'); else btn.text('And');
  });

  $('.filter').on('click', '.filter-add-cond', function (e) {
    e.preventDefault();
    var list = $(this).closest('.filter-content').find('.filter-conds:first');
    addCondition(list);
  });

  function addCondition(list) {
    var clone = $('.filter-template-cond li.filter-cond').clone(true);
    clone.appendTo(list).hide();
    clone.find('.filter-field').trigger('change');
    clone.fadeIn();
  }

  $('.filter').on('click', '.filter-add-group', function (e) {
    e.preventDefault();
    var subs = $(this).closest('.filter-content').find('.filter-subs:first');
    var list = $('.filter-template-body table').clone().appendTo(subs).hide().fadeIn().find('.filter-conds:first');
    addCondition(list);
  });

  $('.filter').on('click', '.filter-remove-cond', function (e) {
    e.preventDefault();
    if ($(this).closest('ul').find('li').length <= 1) {
      $(this).closest('table').remove();
    }
    else {
      $(this).closest('li').remove();
    }
  });

  $('.filter').on('change', '.filter-field', function (e) {
    e.preventDefault();
    $(this).closest('li').find('.filter-comp').remove();
    $(this).closest('li').find('.filter-value').remove();
    var field = $(this).find(':selected').data('field');
    var comp = null;
    switch (field.DataType) {
      case 'datetime': comp = $('.filter-template-comps .filter-comp-datetime').clone().insertAfter($(this)); break;
      case 'bool': comp = $('.filter-template-comps .filter-comp-bool').clone().insertAfter($(this)); break;
      case 'number': comp = $('.filter-template-comps .filter-comp-number').clone().insertAfter($(this)); break;
      default: comp = $('.filter-template-comps .filter-comp-text').clone().insertAfter($(this)); break;
    }
    comp.trigger('change');
  });

  $('.filter').on('change', '.filter-comp', function (e) {
    e.preventDefault();
    var field = $(this).closest('li').find('.filter-field').find(':selected').data('field');
    $(this).closest('li').find('.filter-value').remove();
    var arg1type = $(this).find(':selected').data('argtype-1');
    if (arg1type) {
      switch (arg1type) {
        case 'int':
          $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).numeric({ 'decimal': false });
          break;
        case 'float':
          $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).numeric();
          break;
        case 'month':
          var months = $('<select>').addClass('form-control filter-value').insertAfter($(this));
          $('<option>').attr('value', 1).text('January').appendTo(months);
          $('<option>').attr('value', 2).text('February').appendTo(months);
          $('<option>').attr('value', 3).text('March').appendTo(months);
          $('<option>').attr('value', 4).text('April').appendTo(months);
          $('<option>').attr('value', 5).text('May').appendTo(months);
          $('<option>').attr('value', 6).text('June').appendTo(months);
          $('<option>').attr('value', 7).text('July').appendTo(months);
          $('<option>').attr('value', 8).text('August').appendTo(months);
          $('<option>').attr('value', 9).text('September').appendTo(months);
          $('<option>').attr('value', 10).text('October').appendTo(months);
          $('<option>').attr('value', 11).text('November').appendTo(months);
          $('<option>').attr('value', 12).text('December').appendTo(months);
          break;
        case 'day':
          var days = $('<select>').addClass('form-control filter-value').insertAfter($(this));
          $('<option>').attr('value', 1).text('Sunday').appendTo(days);
          $('<option>').attr('value', 2).text('Monday').appendTo(days);
          $('<option>').attr('value', 3).text('Tuesday').appendTo(days);
          $('<option>').attr('value', 4).text('Wednesday').appendTo(days);
          $('<option>').attr('value', 5).text('Thursday').appendTo(days);
          $('<option>').attr('value', 6).text('Friday').appendTo(days);
          $('<option>').attr('value', 7).text('Saturday').appendTo(days);
          break;
        case 'date':
          $('<input type="text">').addClass('form-control filter-value').insertAfter($(this)).datetimepicker(
            {
              icons: { time: "fa fa-clock-o", date: "fa fa-calendar", up: "fa fa-arrow-up", down: "fa fa-arrow-down" },
              pickTime: false
            }
          );
          break;
        default:
          var input = $('<input type="text">').addClass('form-control filter-value').insertAfter($(this));
          if (field.LookupTableID) {
            input.autocomplete({ minLength: 2, source: getFieldValues, select: function (event, ui) { } });
            input.data('fieldid', field.ID);
          }
      }
    }
  });

  var execGetFieldValues = null;
  function getFieldValues(request, response) {
    if (execGetFieldValues) { execGetFieldValues._executor.abort(); }
    execGetFieldValues = top.Ts.Services.System.GetLookupDisplayNames($(this.element).data('fieldid'), request.term, function (result) { response(result); $(this).removeClass('ui-autocomplete-loading'); });
  }


  function initFilters() {
    addCondition($('.filter-template-body table').clone().appendTo('.filter').find('.filter-conds:first'));
  }
});


function onShow() {
  reportEditPage.refresh();
};

ReportEditPage = function () {
  $('.loading-section').hide().next().show();
};

ReportEditPage.prototype = {
  constructor: ReportEditPage,
  refresh: function () {

  }
};
