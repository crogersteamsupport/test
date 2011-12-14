/// <reference path="ts/ts.js" />
/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />


$(document).ready(function () {
  var self = this;

  $('button').button();
  $('#divProgressBar').progressbar({ value: 0 });
  var versionID = top.Ts.Utils.getQueryValue('versionid', window);

  $('#file-upload').attr('action', 'Upload/Products/' + versionID);
  var loadTimer = null;
  loadVersion();

  function showLoading() { $('.ts-loading').show().next().hide(); }

  function loadVersion() {
    loadTimer = setTimeout(showLoading, 500);
    top.Ts.Services.Products.GetVersion(versionID, function (version) {
      top.Ts.Services.CustomFields.GetValues(14, versionID, function (values) {
        loadAttachments(function () {
          var div = $('#divVersionInfo').html('');
          $('<h1>').text(version.ProductName + ' - ' + version.VersionNumber).appendTo(div);
          var table = $('<table>');
          $('<tr>').append('<td>Version:</td><td>' + version.VersionNumber + '</td>').appendTo(table);
          $('<tr>').append('<td>Status:</td><td>' + version.VersionStatus + '</td>').appendTo(table);
          $('<tr>').append('<td>Released:</td><td>' + (version.IsReleased === false ? 'No' : 'Yes') + '</td>').appendTo(table);
          if (version.ReleaseDate) $('<tr>').append('<td>Date:</td><td>' + version.ReleaseDate.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern) + '</td>').appendTo(table);
          for (var i = 0; i < values.length; i++) {
            $('<tr>').append('<td>' + values[i].Name + ':</td><td>' + values[i].Value + '</td>').appendTo(table);
          }

          table.appendTo(div);
          $('#divDescription').html(version.Description == '' ? 'No Description' : version.Description);

          if (loadTimer) clearTimeout(loadTimer);
          $('.ts-loading').hide().next().show();
        });
      });
    });
  }

  function loadAttachments(callback) {
    top.Ts.Services.Products.GetAttachments(versionID, function (attachments) {
      var list = $('<ul>');

      for (var i = 0; i < attachments.length; i++) {
        $('<li>')
          .html('<span class="ts-icon ts-icon-attachment"></span><a href="../../dc/attachments/' + attachments[i].AttachmentID + '/" target="_blank">' + attachments[i].FileName + '</a><span class="ui-icon ui-icon-close"></span>')
          .data('id', attachments[i].AttachmentID)
          .data('fileName', attachments[i].FileName)
          .appendTo(list)
          .find('.ui-icon')
          .click(function (e) {
            var item = $(this).parent();
            if (!confirm('Are you sure you would like to remove ' + item.data('fileName') + '?')) return;
            top.Ts.Services.Products.DeleteAttachment(item.data('id'), function () {
              item.remove();
            });

          });

      }
      $('#divAttachments').empty().append(list);
      if (callback) callback();
    });
  }

  var fileUpload = $('#file-upload').fileupload({
    namespace: 'product_versions',
    send: function (event, files, index, xhr, handler) {
      if (!xhr.upload) {
        $('#divProgressBar').progressbar({ value: 50 });
      }
      else {
        $('#divProgressBar').progressbar({ value: 0 });
      }

      $('#divUploadProgress').show();
      $('#file-upload').hide();
    },
    onLoad: function (event, files, index, xhr, handler) {
      loadAttachments();
      $('#divUploadProgress').hide();
      $('#file-upload').show();
    },
    onProgress: function (event, files, index, xhr, handler) {
      $('#divProgressBar').progressbar({ value: parseInt(event.loaded / event.total * 100, 10) });
    }


  });


});

