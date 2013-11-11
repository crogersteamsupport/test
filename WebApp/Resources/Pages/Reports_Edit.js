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
      if ($('#input-name').val() == '') {
        $('.input-name').popover('show').parent('.form-group').addClass('has-error');
        return;
      }
      _report.Name = $('.input-name').val();
      _report.Description = $('.report-description').val();
      $('.input-name').popover('hide').parent('.form-group').removeClass('has-error');
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
        _report.Fields.push($(this).data('field'));
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

    if ($('.report-fields-selected li').length < 1) {
      $('.action-next').addClass('disabled').prop('disabled', true);
      $('.report-fields-hint-order').hide();
      $('.report-fields-hint-add').show();
    }
    else {
      $('.action-next').removeClass('disabled').prop('disabled', false);
      $('.report-fields-hint-order').show();
      $('.report-fields-hint-add').hide();
    }

    if (_catID == catID && _subID == subID) return;
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
    location = "reports.html";
  });

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
    clone.appendTo(list).hide().fadeIn();
  }

  $('.filter').on('click', '.filter-add-group', function (e) {
    e.preventDefault();
    var subs = $(this).closest('.filter-content').find('.filter-subs:first');
    var list = $('.filter-template-body table').clone().appendTo(subs).hide().fadeIn().find('.filter-conds:first');
    addCondition(list);
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
    $(this).closest('li').find('.filter-value').show();
    var field = $(this).find(':selected').data('field');

    switch (field.DataType) {
      case 'datetime': $('.filter-template-comps .filter-comp-datetime').clone().insertAfter($(this)); break;
      case 'bool': $('.filter-template-comps .filter-comp-bool').clone().insertAfter($(this)); break;
      case 'number': $('.filter-template-comps .filter-comp-number').clone().insertAfter($(this)); break;
      default: $('.filter-template-comps .filter-comp-text').clone().insertAfter($(this)); break;
    }
  });

  $('.filter').on('change', '.filter-comp', function (e) {
    e.preventDefault();
    $(this).closest('li').find('.filter-field').find(':selected').data('field');
    //alert($(this).find(':selected').data('argtype-1'));
    $('<input type="text">').insertAfter($(this));

  });

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
