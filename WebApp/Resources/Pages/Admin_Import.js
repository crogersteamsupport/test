var importPage = null;
var _uploadedFileName = '';
$(document).ready(function () {
    importPage = new ImportPage();

    LoadImports(1);

    function LoadImports(start) {
      showLoadingIndicator();
      $('.results-empty').hide();
      $('.results-done').hide();

      if (start == 1)
        $('#tblImports tbody').empty();

      top.Ts.Services.Organizations.LoadImports(start, function (imports) {
        hideLoadingIndicator();
        if (imports.length < 1) {
          if (start == 1) {
            $('.results-empty').show();
          }
          else {
            $('.results-done').show();
          }
        } else {
          for (var i = 0; i < imports.length; i++) {
            var dateStarted = null;
            if (imports[i].DateStarted == null) {
              dateStarted = 'Not started';
            }
            else {
              dateStarted = imports[i].DateStarted.toLocaleString();
            }
            var status = 0;
            if (imports[i].CompletedRows > 0) {
              status = (imports[i].CompletedRows / imports[i].TotalRows) * 100;
            }

            var tr = $('<tr>')
                    .attr('id', imports[i].ImportID)
                    .html('\
                    <td>' + imports[i].FileName + '</td>\
                    <td>' + imports[i].RefTypeString + '</td>\
                    <td>' + dateStarted + '</td>\
                    <td>' + status + '%</td>')
                    .appendTo('#tblImports > tbody:last');
          }
        }
      });
    }

    function LoadFields(refType) {
      top.Ts.Services.Organizations.LoadImportFields(refType, function (importFields) {
        $('.available-field-list').empty();
        for (var i = 0; i < importFields.length; i++) {
          var li = $('<li>')
            .data('ImportFieldID', importFields[i].ImportFieldID)
            .data('FieldName', importFields[i].FieldName)
            .data('SourceName', importFields[i].SourceName)
            .appendTo('.available-field-list:last');

          var firstRow = $('<div>')
            .addClass('available-field-name')
            .html(importFields[i].FieldName)
            .appendTo(li);

          var typeAndSizeSpan = $('<span>')
            .addClass('text-muted available-field-type pull-right')
            .html(importFields[i].DataType + ', ' + importFields[i].Size + ' bytes')
            .appendTo(firstRow);

          var secondRow = $('<div>')
            .addClass('available-field-desc')
            .html(importFields[i].Description)
            .appendTo(li);

          var formcontainer = $('<div>').addClass('form-horizontal').appendTo(li);
          var groupContainer = $('<div>').addClass('form-group form-group-sm')
                                  .appendTo(formcontainer)
                                  .append($('<label>').addClass('col-sm-4 control-label select-label').text('Source Name:'));
          var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
          var inputGroupContainer = $('<div>').addClass('input-group').appendTo(inputContainer);
          var input = $('<input>')
            .addClass('form-control ticket-simple-input muted-placeholder col-sm-8')
            .attr("placeholder", importFields[i].FieldName)
            .val(importFields[i].SourceName)
            .appendTo(inputGroupContainer)

          input.change(function (e) {
            var value = input.val();
            if (value != li.data('FieldName')) {
              li.data('SourceName', value);
            }
            else {
              li.data('SourceName', '');
            }
          });
        }
      });
    }

    $('.action-new').click(function (e) {
      e.preventDefault();
      $('.import-section').addClass('hidden');
      $('#import-new').removeClass('hidden');
      $('#importUpload').removeClass('hidden');
      $('#csvColumnsPanel').addClass('hidden');
      $('#importButtons').addClass('hidden');
      LoadFields($('#import-type').val());
    });

    $('#import-type').change(function () {
      LoadFields($('#import-type').val());
    });

    var _isLoading = false;
    $('.frame-container').bind('scroll', function () {
      if (_isLoading == true) return;
      if ($('.results-done').is(':visible')) return;

      if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
        LoadImports($('#tblImports > tbody > tr').length);
      }

      if ($(this).scrollTop() > 100) {
        $('.scrollup').fadeIn();
      } else {
        $('.scrollup').fadeOut();
      }
    });

    $('.scrollup').click(function () {
      $('.frame-container').animate({
        scrollTop: 0
      }, 600);
      return false;
    });

    function showLoadingIndicator() {
      _isLoading = true;
      $('.results-loading').show();
    }

    function hideLoadingIndicator() {
      _isLoading = false;
      $('.results-loading').hide();
    }

    $('#btnSelectFile').click(function (e) {
      // Simulate a click on the file input button
      // to show the file browser dialog
      e.preventDefault();
      $(this).parent().find('input').click();
    });

    $('.import-upload').fileupload({
    namespace: 'import_file',
    dropZone: $('.import-upload'),
    add: function (e, data) {
      var goUpload = true;

      for (var i = 0; i < data.files.length; i++) {
        if (!(/\.(csv)$/i).test(data.files[i].name)) {
          alert('You must select csv files only.');
          goUpload = false;
        }
      }

      if (goUpload) {
        for (var i = 0; i < data.files.length; i++) {
          var item = $('<li>')
                    .appendTo($('.upload-queue'));

          data.context = item;
          item.data('data', data);

          var bg = $('<div>')
                    .addClass('ts-color-bg-accent')
                    .appendTo(item);

          $('<div>')
                    .text(data.files[i].name + '  (' + top.Ts.Utils.getSizeString(data.files[i].size) + ')')
                    .addClass('filename')
                    .appendTo(bg);

          $('<span>')
                    .addClass('icon-remove')
                    .click(function (e) {
                      e.preventDefault();
                      $(this).closest('li').fadeOut(500, function () { $(this).remove(); });
                    })
                    .appendTo(bg);

          $('<span>')
                    .addClass('icon-remove')
                    .hide()
                    .click(function (e) {
                      e.preventDefault();
                      var data = $(this).closest('li').data('data');
                      data.jqXHR.abort();
                    })
                    .appendTo(bg);

          var progress = $('<div>')
                    .addClass('progress progress-striped active')
                    .hide();

          $('<div>')
                      .addClass('progress-bar')
                      .attr('role', 'progressbar')
                      .appendTo(progress);

          progress.appendTo(bg);
        }
      }
    },
    send: function (e, data) {
      if (data.context && data.dataType && data.dataType.substr(0, 6) === 'iframe') {
        data.context.find('.progress-bar').css('width', '50%');
      }
    },
    fail: function (e, data) {
      if (data.errorThrown === 'abort') return;
      alert('There was an error uploading "' + data.files[0].name + '".');
    },
    progress: function (e, data) {
      data.context.find('.progress-bar').css('width', parseInt(data.loaded / data.total * 100, 10) + '%');
    },
    start: function (e, data) {
      $('.progress').show();
      $('.upload-queue .ui-icon-close').hide();
      $('.upload-queue .ui-icon-cancel').show();
    },
    stop: function (e, data) {
      //data.context.find('.progress-bar').css('width', '100%');
      LoadImports(1);
      $('.upload-queue').empty();
      //No yet. This will be done once import button is pushed.
      //$('.import-section').removeClass('hidden');
      //$('#import-new').addClass('hidden');
      $('#importUpload').addClass('hidden');
      $('#csvColumnsPanel').removeClass('hidden');
      $('#importButtons').removeClass('hidden');
    },
    done: function (e, data) {
      //var fields = [];
      //$('.available-field-list li').each(function (i, o) {
      //  if ($(o).data('SourceName') != '') {
      //    var field = new Object();
      //    field.ImportFieldID = $(o).data('ImportFieldID');
      //    field.SourceName = $(o).data('SourceName');
      //    fields.push(field);
      //  }
      //});
      var result = JSON.parse(data.result);
      //top.Ts.Services.Organizations.SaveImportFieldMaps(result[0].id, JSON.stringify(fields), function (importFields) {});
      _uploadedFileName = result[0].name;
      top.Ts.Services.Organizations.GetImportPanels(_uploadedFileName, $('#import-type').val(), function (panels) {
        var fieldList = $('<select>')
          .addClass('form-control');

        var skippedOption = $('<option>')
          .val(-1)
          .text('Skipped')
          .appendTo(fieldList);

        panels = JSON.parse(panels);
        for (i = 0; i < panels.ImportFields.length; i++) {
          var option = $('<option>')
            .val(panels.ImportFields[i].ImportFieldID)
            .text(panels.ImportFields[i].FieldName)
            .appendTo(fieldList);
        }

        $('#csvColumnsPanel').empty();
        for (i = 0; i < panels.ImportFieldMap.length; i++) {
          var panel = $('<div>')
            .addClass('col-xs-6')
            .appendTo($('#csvColumnsPanel'));

          var panelDoc = $('<div>')
            .addClass('panel panel-primary field-panel')
            .appendTo(panel);

          var panelHeading = $('<div>')
            .addClass('panel-heading')
            .appendTo(panelDoc);

          var panelTitle = $('<h3>')
            .addClass('panel-title')
            .text(panels.ImportFieldMap[i].SourceName)
            .appendTo(panelHeading);

          var panelBody = $('<div>')
            .addClass('panel-body')
            .appendTo(panelDoc);

          var panelForm = $('<form>')
            .appendTo(panelBody);

          var fieldListClone = fieldList.clone();

          if (panels.ImportFieldMap[i].ImportFieldID != 0)
          {
            fieldListClone.val(panels.ImportFieldMap[i].ImportFieldID);
          }
          else
          {
            fieldListClone.find('option').filter(function () { return $(this).text() == panels.ImportFieldMap[i].SourceName; }).attr("selected", "selected");
          }
          fieldListClone.appendTo(panelForm);
          panelTitle.data("MappedFieldName", fieldListClone.find(":selected").text());

          fieldListClone.change(function () {
            panelTitle.data("MappedFieldName", $(this).find(":selected").text());
          });

          var exampleValue = $('<div>')
            .addClass('well well-sm')
            .appendTo(panelForm);

          var exampleValueLabel = $('<div>')
            .addClass('text-muted')
            .text('Value: ')
            .appendTo(exampleValue);

          var exampleValueContent = $('<div>')
            .text(panels.ImportFieldMap[i].ExampleValue)
            .appendTo(exampleValue);
        }
      });
    }
  });

  $(".btnCancel").click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Import - Cancel Upload');
    $('.upload-queue').empty();
    $('.import-section').removeClass('hidden');
    $('#import-new').addClass('hidden');
  });

  $('#btnUpload').click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Import - Upload Files');
    if ($('.upload-queue li').length > 0) {
      $('.upload-queue li').each(function (i, o) {
        var data = $(o).data('data');
        data.formData = { refType: $('#import-type').val() };
        data.url = '../../../Upload/Imports';
        data.jqXHR = data.submit();
        $(o).data('data', data);
      });
    }
  });

  $('#btnImport').click(function (e) {
    e.preventDefault();
    top.Ts.System.logAction('Import - Create import.');
    var fields = [];
    $('.panel-title').each(function (i, o) {
      if ($(o).text() != 'Available Fields' && $(o).data('MappedFieldName') != 'Skipped' && $(o).data('MappedFieldName') != $(o).text()) {
        var field = new Object();
        field.ImportFieldName = $(o).data('MappedFieldName');
        field.SourceName = $(o).text();
        fields.push(field);
      }
    });
    top.Ts.Services.Organizations.SaveImport(_uploadedFileName, $('#import-type').val(), JSON.stringify(fields), function (importFields) {
      $('.import-section').removeClass('hidden');
      $('#import-new').addClass('hidden');
      $('#importUpload').addClass('hidden');
      $('#csvColumnsPanel').removeClass('hidden');
      $('#importButtons').removeClass('hidden');
    });
  });

});

function onShow() {
    importPage.refresh();
};

ImportPage = function () {
}

