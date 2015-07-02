var importPage = null;
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
      $('.import-section').removeClass('hidden');
      $('#import-new').addClass('hidden');
    }
  });

  $("#btnCancel").click(function (e) {
    top.Ts.System.logAction('Import - Cancel Upload');
    $('.upload-queue').empty();
    $('.import-section').removeClass('hidden');
    $('#import-new').addClass('hidden');
  });

  $('#btnImport').click(function (e) {
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


});

function onShow() {
    importPage.refresh();
};

ImportPage = function () {
    $('.action-new').click(function (e) {
        e.preventDefault();
        $('.import-section').addClass('hidden');
        $('#import-new').removeClass('hidden');
    });
}

