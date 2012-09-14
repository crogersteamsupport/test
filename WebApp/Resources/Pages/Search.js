var _firstItemIndex         = 0;
var _pageSize               = 20;
var _advancedSearchOptions  = null;
var _standardFilterID       = null;
var _addingFilter           = false;
var _addingSorter           = false;

$(document).ready(function () {

  layout = $('body').layout({
    applyDefaultStyles: true
    , east__size: 283
    , east__resizable: false
    , east__closable: false
    , spacing_open: 0
  });

  LoadAdvancedOptions();

  $('#search-results-loading').hide();

  $('#search-button').click(function () { _firstItemIndex = 0; GetSearchResults(); });

  $('#search-results-pane').bind('scroll', function () {
    if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
      GetSearchResultsNextPage();
    }
  });

  // The include-all is also a checkbox. When clicked first will handle the IncludeAllClickEvent and afterwards the StandardFilter click event. 
  $('#include-all').click(function () { HandleIncludeAllClickEvent(); });
  $('.checkbox').click(function () { HandleStandardFilterClickEvent(); });
  $('#search-options-add-filter').click(function () { HandleAddFilterClickEvent(); });
  $('#search-options-add-sorter').click(function () { HandleAddSorterClickEvent(); });
});

function LoadAdvancedOptions() {
  if (_advancedSearchOptions == null) {
    top.Ts.Services.Search.GetAdvancedSearchOptions(function (advancedSearchOptions) {

      _advancedSearchOptions  = advancedSearchOptions;
      _standardFilterID       = advancedSearchOptions.StandardFilterID;

      LoadStandardFilters();
      LoadCustomFilters();
      LoadCustomSorters();

    });
  }
  else {
    LoadStandardFilters();
    LoadCustomFilters();
    LoadCustomSorters();
  }
}

function LoadStandardFilters() {
  if (_standardFilterID != null) {
    $('#include-tickets').attr('checked', _advancedSearchOptions.Tickets);
    $('#include-knowledge-base').attr('checked', _advancedSearchOptions.KnowledgeBase);
    $('#include-wikis').attr('checked', _advancedSearchOptions.Wikis);
    if ($('#include-tickets').attr('checked') && $('#include-knowledge-base').attr('checked') && $('#include-wikis').attr('checked')) {
      $('#include-all').attr('checked', true);
    }
  }
  else {
    $('.checkbox').attr('checked', true);
  }
}

function LoadCustomFilters() {

  if (_advancedSearchOptions.CustomFilters.length > 0) {
    for (var i = 0; i < _advancedSearchOptions.CustomFilters.length; i++) {

      var customFilterID      = _advancedSearchOptions.CustomFilters[i].CustomFilterID;
      var reportTableFieldID  = _advancedSearchOptions.CustomFilters[i].FieldID;
      var measure             = _advancedSearchOptions.CustomFilters[i].Measure;
      var testValue           = _advancedSearchOptions.CustomFilters[i].TestValue;

      AddCustomFilter(customFilterID, reportTableFieldID, measure, testValue);

    }
  }
  else {
    AddCustomFilter();
    _addingFilter = true;
  }
}

function LoadCustomSorters() {

  if (_advancedSearchOptions.Sorters.length > 0) {

    for (var i = 0; i < _advancedSearchOptions.Sorters.length; i++) {

      var sorterID    = _advancedSearchOptions.Sorters[i].SorterID;
      var fieldID     = _advancedSearchOptions.Sorters[i].FieldID;
      var descending  = _advancedSearchOptions.Sorters[i].Descending;
      AddCustomSorter(sorterID, fieldID, descending);

    }
  }
  else {
    AddCustomSorter();
    _addingSorter = true;
  }
}

function AddCustomFilter(customFilterID, reportTableFieldID, measure, testValue) {
  if (_addingFilter) {
    return;
  }

  var div = $('<div>').addClass('condition');
  var displayFieldSet = $('<fieldset>').addClass('display-condition');

  var fieldText = $('<p>');
  var measureText = $('<p>');
  var valueText = $('<p>');
  
  if (customFilterID) {
    div.addClass('filter-' + customFilterID);
    displayFieldSet.addClass('filter-' + customFilterID);
  }

  var fields = $('<select>').addClass('condition-field').appendTo(div).width('200px');
  loadComboFields(fields, true).combobox({
    selected: function (e, ui) {
      createConditionValue(div, fields.find('option:selected').data('field'));
    }
  });

  if (reportTableFieldID) {
    fields.combobox('setValue', reportTableFieldID);
    fieldText.html(fields.find('option:selected').text());
  }

  var measures = $('<select>').addClass('condition-measure').appendTo(div).width('150px');
  loadComboMeasures(measures).combobox({
    selected: function (e, ui) {
    }
  });
  
  if (measure) {
    measures.combobox('setValue', measure);
    measureText.html(measures.find('option:selected').text());
  }

  $('<span>').addClass('condition-value-container').appendTo(div);

  if (testValue) {
    createConditionValue(div, fields.find('option:selected').data('field'), testValue);
    valueText.html(testValue)
  }
  else {
    createConditionValue(div, fields.find('option:selected').data('field'));
  }

  var deleteIcon = $('<span>')
    .addClass('ui-icon ui-icon-close')
    .click(function (e) {
      var clickedCustomFilterID = Ts.Utils.getIdFromElement('filter', $(this).parent());
      top.Ts.Services.Search.DeleteCustomFilter(clickedCustomFilterID, function () { });
      div.remove();
      displayFieldSet.remove();
      $('#search-button').trigger('click');
    });
  deleteIcon.css('float', 'right');
  deleteIcon.appendTo(displayFieldSet);

  var editIcon = $('<span>')
    .addClass('ui-icon ui-icon-pencil')
    .click(function (e) {
      div.show();
      displayFieldSet.hide();
    });
  editIcon.css('float', 'right');
  editIcon.appendTo(displayFieldSet);

  fieldText.appendTo(displayFieldSet);
  measureText.appendTo(displayFieldSet);
  valueText.appendTo(displayFieldSet);


  var iconsDiv = $('<div>');
  iconsDiv.css('margin-top', '3px');

  var saveIcon = $('<span>')
    .addClass('ts-icon ts-icon-save')
    .click(function (e) {
      var fieldID = $(this).parent().parent().find('.condition-field').val();
      var measure = $(this).parent().parent().find('.condition-measure').val();
      var value = $(this).parent().parent().find('.condition-value').val();
      var isCustom = $(this).parent().parent().find('.condition-field option:selected').data('field').IsCustom;
      var clickedCustomFilterID = Ts.Utils.getIdFromElement('filter', $(this).parent().parent());
      if (clickedCustomFilterID == null) {
        top.Ts.Services.Search.AddCustomFilter(fieldID, measure, value, isCustom, function (result) {
          div.addClass('filter-' + result);
          displayFieldSet.addClass('filter-' + result);
          _addingFilter = false;
        },
          function () {
            alert('An error occurred saving filter. Please try again later.');
          });
      }
      else {
        top.Ts.Services.Search.UpdateCustomFilter(clickedCustomFilterID, fieldID, measure, value, isCustom);
      }
      fieldText.html(fields.find('option:selected').text());
      measureText.html(measures.find('option:selected').text());
      valueText.html(value);

      if (clickedCustomFilterID == null) {
        $(div).hide().appendTo('#search-options-filters');
        displayFieldSet.show("slow");
      }
      else {
        div.hide();
        displayFieldSet.show();
      }

      $('#search-button').trigger('click');
    });
  saveIcon.css('float', 'left');
  saveIcon.css('margin-left', '5px');
  saveIcon.css('margin-right', '5px');
  saveIcon.appendTo(iconsDiv);

  var cancelIcon = $('<span>')
    .addClass('ts-icon ts-icon-cancel')
    .click(function (e) {
      if (Ts.Utils.getIdFromElement('filter', $(this).parent().parent()) == null) {
        $(this).parent().parent().remove();
        _addingFilter = false;
      }
      else {
        fields.combobox('setValue', reportTableFieldID);
        measures.combobox('setValue', measure);
        createConditionValue(div, fields.find('option:selected').data('field'), testValue);

        div.hide();
        displayFieldSet.show();
      }
    });
  cancelIcon.css('float', 'left');
  cancelIcon.css('margin-right', '5px');
  cancelIcon.appendTo(iconsDiv);

  iconsDiv.appendTo(div);

  $(displayFieldSet).appendTo('#search-options-filters');

  if (reportTableFieldID) {
    $(div).appendTo('#search-options-filters').hide();
    displayFieldSet.show();
  }
  else {
    $(div).appendTo('#search-options-new-filter').show();
    displayFieldSet.hide();
  }
}

function AddCustomSorter(sorterID, fieldID, descending) {
  if (_addingSorter) {
    return;
  }

  var div = $('<div>').addClass('condition');
  var displayFieldSet = $('<fieldset>').addClass('display-condition');

  var fieldText = $('<p>');
  var directionText = $('<p>');

  if (sorterID) {
    div.addClass('sorter-' + sorterID);
    displayFieldSet.addClass('sorter-' + sorterID);
  }

  var fields = $('<select>').addClass('condition-field').appendTo(div).width('200px');
  loadComboFields(fields, false).combobox({
    selected: function (e, ui) {
    }
  });

  if (fieldID) {
    fields.combobox('setValue', fieldID);
    fieldText.html(fields.find('option:selected').text());
  }

  var directions = $('<select>').addClass('condition-direction').appendTo(div);
  loadComboDirections(directions).combobox({
    selected: function (e, ui) {
    }
  });

  if (descending) {
    directions.combobox('setValue', descending);
    directionText.html('Descending');
  }
  else {
    directionText.html('Ascending');
  }

  var deleteIcon = $('<span>')
    .addClass('ui-icon ui-icon-close')
    .click(function (e) {
      var clickedSorterID = Ts.Utils.getIdFromElement('sorter', $(this).parent());
      top.Ts.Services.Search.DeleteSorter(clickedSorterID, function () { });
      div.remove();
      displayFieldSet.remove();
      $('#search-button').trigger('click');
    });
  deleteIcon.css('float', 'right');
  deleteIcon.appendTo(displayFieldSet);

  var editIcon = $('<span>')
    .addClass('ui-icon ui-icon-pencil')
    .click(function (e) {
      div.show();
      displayFieldSet.hide();
    });
  editIcon.css('float', 'right');
  editIcon.appendTo(displayFieldSet);

  fieldText.appendTo(displayFieldSet);
  directionText.appendTo(displayFieldSet);

  var iconsDiv = $('<div>').addClass('condition-sorter-icons');

  var saveIcon = $('<span>')
        .addClass('ts-icon ts-icon-save')
        .click(function (e) {
          var fieldID = $(this).parent().parent().find('.condition-field').val();
          var descending = $(this).parent().parent().find('.condition-direction').val();
          var isCustom = $(this).parent().parent().find('.condition-field option:selected').data('field').IsCustom;
          var clickedSorterID = Ts.Utils.getIdFromElement('sorter', $(this).parent().parent());
          if (clickedSorterID == null) {
            top.Ts.Services.Search.AddSorter(fieldID, descending, isCustom, function (result) {
              div.addClass('sorter-' + result);
              displayFieldSet.addClass('sorter-' + result);
              _addingSorter = false;
            },
            function () {
              alert('An error occurred saving sorter. Please try again later.');
            });
          }
          else {
            top.Ts.Services.Search.UpdateSorter(clickedSorterID, fieldID, descending, isCustom);
          }

          fieldText.html(fields.find('option:selected').text());
          if (descending) {
            directionText.html('Descending');
          }
          else {
            directionText.html('Ascending');
          }

          if (clickedSorterID == null) {
            $(div).hide().appendTo('#search-options-sorters');
            displayFieldSet.show("slow");
          }
          else {
            div.hide();
            displayFieldSet.show();
          }

          $('#search-button').trigger('click');
        });
  saveIcon.css('float', 'left');
  saveIcon.css('margin-right', '5px');
  saveIcon.appendTo(iconsDiv);

  var cancelIcon = $('<span>')
        .addClass('ts-icon ts-icon-cancel')
        .click(function (e) {
          if (Ts.Utils.getIdFromElement('sorter', $(this).parent().parent()) == null) {
            $(this).parent().parent().remove();
            _addingSorter = false;
          }
          else {
            fields.combobox('setValue', fieldID);
            directions.combobox('setValue', descending);

            div.hide();
            displayFieldSet.show();

          }
        });

  cancelIcon.css('float', 'left');
  cancelIcon.css('margin-right', '5px');
  cancelIcon.appendTo(iconsDiv);

  iconsDiv.appendTo(div);

  $(displayFieldSet).appendTo('#search-options-sorters');

  if (fieldID) {
    $(div).appendTo('#search-options-sorters').hide();
    displayFieldSet.show();
  }
  else {
    $(div).appendTo('#search-options-new-sorter').show();
    displayFieldSet.hide();
  }
}

function loadComboFields(select, withCustomFields) {
  select.html('');
  $('<option>').attr('value', '-1').text('-- Select a Field --').appendTo(select).attr('selected', 'selected');
  if (_advancedSearchOptions == null) {
    top.Ts.Services.Search.GetAdvancedSearchOptions(function (advancedSearchOptions) {
      _advancedSearchOptions = advancedSearchOptions;
      PopulateComboFields(select, withCustomFields);
    });
  }
  else {
    PopulateComboFields(select, withCustomFields);
  }

  return select;
}

function PopulateComboFields(select, withCustomFields) {
  for (var i = 0; i < _advancedSearchOptions.Fields.length; i++) {
    if (withCustomFields || !_advancedSearchOptions.Fields[i].IsCustom) {
      $('<option>').attr('value', _advancedSearchOptions.Fields[i].FieldID).text(_advancedSearchOptions.Fields[i].Alias).appendTo(select).data('field', _advancedSearchOptions.Fields[i]);
    }
  }
}

function loadComboMeasures(select) {
  select.html('');
  $('<option>').attr('value', '<').text('Less Than').appendTo(select);
  $('<option>').attr('value', '<=').text('Less Than or Equal To').appendTo(select);
  $('<option>').attr('value', '=').attr('selected', 'selected').text('Equal To').appendTo(select);
  $('<option>').attr('value', '<>').text('Not Equal To').appendTo(select);
  $('<option>').attr('value', '>=').text('Greater Than or Equal To').appendTo(select);
  $('<option>').attr('value', '>').text('Greater Than').appendTo(select);
  $('<option>').attr('value', 'contains').text('Contains').appendTo(select);
  return select;
}

function loadComboDirections(select) {
  select.html('');
  $('<option>').attr('value', false).attr('selected', 'selected').text('Ascending').appendTo(select);
  $('<option>').attr('value', true).text('Descending').appendTo(select);
  return select;
}

function createConditionValue(condition, field, value) {
  var container = condition.find('.condition-value-container').empty();
  if (!field) {
    var input = $('<input>')
      .addClass('text ui-corner-all ui-widget-content condition-value')
      .attr('type', 'text')
      .width('200px')
      .height('14px')
      .keydown(function () { isModified(true, condition); })
      .appendTo(container);
    return;
  }

  var execGetFieldValues = null;
  function getFieldValues(request, response) {
    if (execGetFieldValues) { execGetFieldValues._executor.abort(); }
    execGetFieldValues = top.Ts.Services.System.GetLookupDisplayNames(condition.find('.condition-field').val(), request.term, function (result) { response(result); $(this).removeClass('ui-autocomplete-loading'); });
  }

  if (field.DataType == 'bit') {
    var select = $('<select>')
        .addClass('condition-value')
        .width('125px')
        .height('14px')
        .appendTo(container);
    $('<option>').attr('value', 'true').text('True').appendTo(select);
    $('<option>').attr('value', 'false').text('False').appendTo(select);

    if (value && value == 'false') select.find('option:last').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
    select.combobox({ selected: function (e, ui) { isModified(true, condition); } })
  }
  else if (field.DataType == 'list') {
    var select = $('<select>')
        .addClass('condition-value')
        .height('14px')
        .appendTo(container);
    for (var i = 0; i < field.ListValues.length; i++) {
      $('<option>').attr('value', field.ListValues[i]).text(field.ListValues[i]).appendTo(select);
    }

    if (value) select.find('option[value="' + value + '"]').attr('selected', 'selected'); else select.find('option:first').attr('selected', 'selected');
    select.combobox({ selected: function (e, ui) { isModified(true, condition); } })
  }
  else {
    var input = $('<input>')
        .addClass('text ui-corner-all ui-widget-content condition-value')
        .attr('type', 'text')
        .width('200px')
        .height('14px')
        .keydown(function () { isModified(true, condition); })
        .appendTo(container)
        .val((value ? value : ""));
    if (field.LookupTableID != null) {
      input.autocomplete({ minLength: 2, source: getFieldValues, select: function (event, ui) { } });
    }
    else if (field.DataType == 'datetime') {
      input.datetimepicker().change(function () { isModified(true, condition); });
    }
  }
}

function isModified(value, div) {
}

$(document).keypress(function (e) {
  if (e.which == 13) {
    _firstItemIndex = 0;
    GetSearchResults();
    $(document).blur();
  }
});

function GetSearchResults() {
  if (_firstItemIndex == 0) {
    $('#search-results').hide();
    $('#search-results-loading').show();
  }
  top.Ts.Services.Search.GetSearchResults($('#search-input').val(), _firstItemIndex, _pageSize, function (results) { showSearchResults(results); });
}

function GetSearchResultsNextPage() {
  _firstItemIndex += _pageSize;
  GetSearchResults();
}

var _ticketTypes = top.Ts.Cache.getTicketTypes();

function showSearchResults(results) {
  if (results == null) return;
  $('#search-results-summary').html(
  '<div class="resultsSummary">' +
      results.Count + ' items found.' +
    '</div>' +
    '<div class="ui-helper-clearfix" />')
  var html = '';
  for (var i = 0; i < results.Items.length; i++) {
    var iconPath = "";
    var onClickHandler = "";
    var subText = "";
    switch (results.Items[i].TypeID) {
      case 1://Tickets
        iconPath = "/vcr/142/images/nav/16/tickets.png";
        onClickHandler = "top.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
        subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
                  '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
        break;
      case 2://KnowledgeBase
        iconPath = "/vcr/142/images/nav/16/knowledge.png";
        onClickHandler = "top.Ts.MainPage.openTicket(" + results.Items[i].Number + ", true)";
        subText = '<h2>Status: ' + results.Items[i].Status + ' </h2>' +
                  '<h2>Severity: ' + results.Items[i].Severity + '</h2>';
        break;
      case 3://Wikis
        iconPath = "/vcr/142/images/nav/16/wiki.png";
        onClickHandler = "top.Ts.MainPage.openWiki(" + results.Items[i].ID + ", true)";
        subText = '<h2>Created by: ' + results.Items[i].Creator + ' </h2>' +
                  '<h2>Modified by: ' + results.Items[i].Modifier + '</h2>';
        break;
    }

    var text = results.Items[i].DisplayName;

    html = html +
      '<div class="resultItem">' +
        '<div class="resultItem-left">' +
          '<div class="resultItem-icon">' +
            '<img alt="Result item icon" src="' + iconPath + '" />' +
            '<h2>' + results.Items[i].ScorePercent + '%</h2>' +
          '</div>' +
          '<div class="resultItem-text">' +
            '<h1>' +
              '<a href="#" onclick="' + onClickHandler + '" class="ts-link">' + text + '</a>' +
            '</h1>' +
            subText +
          '</div>' +
        '</div>' +
        '<div class="ticket-right" />' +
        '<div class="ui-helper-clearfix" />' +
      '</div>';
  }

  if (_firstItemIndex == 0) {
    $('#search-results').html(html).show();
    $('#search-results-loading').hide();
  }
  else {
    $('#search-results').append(html);
  }
}

function getTicketIconUrl(ticketTypeID) {
  for (var i = 0; i < _ticketTypes.length; i++) {
    if (_ticketTypes[i].TicketTypeID == ticketTypeID) {
      return _ticketTypes[i].IconUrl;
    }
  }
  return '';
}

function HandleAddFilterClickEvent() {
  //  $('#search-options-filters').find('.ts-icon-save:visible').trigger('click');
  AddCustomFilter();
  _addingFilter = true;
}

function HandleAddSorterClickEvent() {
  //  $('#search-options-sorters').find('.ts-icon-save:visible').trigger('click');
  AddCustomSorter();
  _addingSorter = true;
}

function HandleIncludeAllClickEvent() {
  if ($('#include-all').attr('checked')) {
    $('.checkbox').attr('checked', true);
  }
  else {
    $('.checkbox').attr('checked', false);
  }
}

function HandleStandardFilterClickEvent() {
  if ($('#include-tickets').attr('checked') && $('#include-knowledge-base').attr('checked') && $('#include-wikis').attr('checked')) {
    $('#include-all').attr('checked', true)
  }
  else {
    $('#include-all').attr('checked', false)
  }

  var includeTickets        = $('#include-tickets').attr('checked');
  var includeKnowledgeBase  = $('#include-knowledge-base').attr('checked');
  var includeWikis          = $('#include-wikis').attr('checked');

  if (_standardFilterID != null) {
    top.Ts.Services.Search.UpdateStandardFilters(_standardFilterID, includeTickets, includeKnowledgeBase, includeWikis);
  }
  else {
    top.Ts.Services.Search.AddStandardFilters(includeTickets, includeKnowledgeBase, includeWikis, function (result) {
      _standardFilterID = result;
    });
  }

  $('#search-button').trigger('click');

}