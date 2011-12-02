/**
* AQUANTUM Demo Application 1.0
*
* Copyright 2010, Sebastian Tschan, AQUANTUM
* http://www.aquantum.de
*/

/*jslint browser: true, regexp: false */
/*global $, File */

var Application = function (settings, locale) {
  'use strict';

  var tmplHelper,

        TemplateHelper = function (locale, settings) {
          this.locale = locale;
          this.settings = settings;
          this.formatFileSize = function (bytes) {
            if (typeof bytes !== 'number' || bytes === null) {
              return '';
            }
            if (bytes >= 1000000000) {
              return (bytes / 1000000000).toFixed(2) + ' GB';
            }
            if (bytes >= 1000000) {
              return (bytes / 1000000).toFixed(2) + ' MB';
            }
            return (bytes / 1000).toFixed(2) + ' KB';
          };
          this.formatFileName = function (fileName) {
            // Remove any path information:
            return fileName.replace(/^.*[\/\\]/, '');
          };
        },

        getAuthenticityToken = function (singleValue) {
          var name = settings.authenticity_token.name,
                parts = $.cookie(name).split('|'),
                obj;
          if (singleValue) {
            obj = {};
            obj[name] = parts[0];
            return obj;
          }
          return { name: name, value: parts[0] };
        },

        addUrlParams = function (url, data) {
          return url + (/\?/.test(url) ? '&' : '?') + $.param(data);
        },

        getFileNode = function (key) {
          return $('#file_' + key);
        },

        deleteItem = function (node, url, callBack) {
          var dialog = $('#dialog_confirm_delete'),
                options,
                form;
          if (!dialog.length) {
            dialog = $('#template_confirm_delete').tmpl(locale).attr('id', 'dialog_confirm_delete');
            options = {
              modal: true,
              show: 'fade',
              hide: 'fade',
              width: 400,
              buttons: {}
            };
            options.buttons[locale.buttons.destroy] = function () {
              $(this).find('form:first').submit();
            };
            options.buttons[locale.buttons.cancel] = function () {
              $(this).dialog('close');
            };
            dialog.dialog(options);
          }
          form = dialog.find('form').bind('submit', function () {
            dialog.dialog('close');
            $('#loading').fadeIn();
            $.ajax({
              url: addUrlParams(url, getAuthenticityToken(true)),
              type: 'DELETE',
              success: function (data) {
                $('#loading').fadeOut();
                callBack(data);
              }
            });
            return false;
          });
          node.addClass('ui-state-highlight');
          dialog.bind('dialogclose', function () {
            $(this).find('form').unbind('submit').unbind('dialogclose');
            node.removeClass('ui-state-highlight');
          }).dialog('open');
        },

        deleteFile = function (key) {
          var node = getFileNode(key);
          deleteItem(node, '/file-upload/files/' + key + '.json', function (data) {
            node.fadeOut(function () {
              $(this).remove();
              if (!$('#files .file_delete:first').length) {
                $('#file_delete').fadeOut();
              }
            });
          });
        },

        enableDragToDesktop = function () {
          var link = $(this),
                url = link.get(0).href,
                regExp = /:/g,
                name = decodeURIComponent(url.split('/').pop()).replace(regExp, '-'),
                type = link.attr('data-type').replace(regExp, '-');
          link.bind('dragstart', function (event) {
            try {
              event.originalEvent.dataTransfer
                        .setData('DownloadURL', [type, name, url].join(':'));
            } catch (e) { }
          });
        },

        fileUploadOptions = {
          uploadTable: $('#files'),
          progressAllNode: $('#file_upload_progress div'),
          buildMultiUploadRow: function (files, handler) {
            var rows = $('<tbody style="display:none;"/>');
            $.each(files, function (index, file) {
              var row = handler.buildUploadRow(files, index, handler).show(),
                        cells = row.find(
                            '.file_upload_progress, .file_upload_start, .file_upload_cancel'
                        );
              if (index) {
                cells.remove();
              } else {
                cells.attr('rowspan', files.length);
              }
              rows.append(row);
            });
            return rows;
          },
          buildUploadRow: function (files, index, handler) {
            if (typeof index !== 'number') {
              return handler.buildMultiUploadRow(files, handler);
            }
            return $('#template_upload').tmpl(files[index], tmplHelper);
          },
          buildMultiDownloadRow: function (files, handler) {
            var rows = $('<tbody style="display:none;"/>');
            $.each(files, function (index, file) {
              rows.append(handler.buildDownloadRow(file, handler).show());
            });
            return rows;
          },
          buildDownloadRow: function (data, handler) {
            if ($.isArray(data)) {
              return handler.buildMultiDownloadRow(data, handler);
            }
            var downloadRow = $('#template_download').tmpl(data, tmplHelper);
            if (data.error) {
              setTimeout(function () {
                downloadRow.fadeOut(function () {
                  downloadRow.remove();
                });
              }, 10000);
            }
            downloadRow.find('a').each(enableDragToDesktop);
            return downloadRow;
          },
          beforeSend: function (event, files, index, xhr, handler, callBack) {
            var fileSize = null,
                    urlRequest;
            if (typeof index === 'undefined') {
              fileSize = 0;
              $.each(files, function (index, file) {
                if (file.size > fileSize) {
                  fileSize = file.size;
                }
              });
            } else {
              fileSize = files[index].size;
            }
            if (fileSize > settings.max_file_size || fileSize === 0) {
              setTimeout(function () {
                handler.onAbort(event, files, index, xhr, handler);
              }, 10000);
              return;
            }
            $('#file_upload_cancel').fadeIn();
            handler.uploadRow.find(handler.cancelSelector).click(function (e) {
              if (urlRequest) {
                urlRequest.abort();
              }
            });
            urlRequest = $.get('/file-upload/upload' + (xhr.upload ? '.json' : ''), function (data) {
              handler.url = data.replace(/http(s)?:\/\/[^\/]+/, '');
              callBack();
            });
          },
          onCompleteAll: function (list) {
            if (!$('#files .file_upload_progress:first').length) {
              $('#file_upload_start, #file_upload_cancel').fadeOut();
            }
            if ($('#files .file_delete:first').length) {
              $('#file_delete').fadeIn();
            }
          },
          previewSelector: null
        },

        uploadDemos = {
          auto: {
            beforeSend: fileUploadOptions.beforeSend,
            previewSelector: null
          },
          queue: {
            beforeSend: function (event, files, index, xhr, handler, callBack) {
              fileUploadOptions.beforeSend(event, files, index, xhr, handler, function () {
                $('#file_upload_start').fadeIn();
                handler.uploadRow.find('.file_upload_start button').click(function () {
                  $(this).fadeOut(function () {
                    if (!$('#files .file_upload_start button:visible:first').length) {
                      $('#file_upload_start').fadeOut();
                    }
                  });
                  callBack();
                  return false;
                });
              });
            },
            previewSelector: '.file_upload_preview'
          }
        },

        initEventHandlers = function () {
          var getKey = function (node) {
            return node.attr('id').replace(/\w+?_/, '');
          };
          $('.file_upload_cancel button').live('click', function () {
            setTimeout(function () {
              if (!$('#files .file_upload_progress:first').length) {
                $('#file_upload_start, #file_upload_cancel').fadeOut();
              }
            }, 500);
          });
          $('.file_delete button').live('click', function () {
            deleteFile(getKey($(this).closest('tr')));
            return false;
          });
          $('button.ui-state-default').live(
                'mouseenter mouseleave',
                function () {
                  $(this).toggleClass('ui-state-hover');
                }
            );
          $('#radio input').click(function (e) {
            var val = $(this).val();
            settings.autoUpload = val === 'auto';
            $('#file_upload').fileUploadUI('option', uploadDemos[val]);
          });
          $('#file_upload_start, #file_upload_cancel, #file_delete').click(function () {
            $('#files .' + $(this).attr('id') + ' button').click();
            return false;
          });
          // Enable drag to Desktop for existing files:
          $('#files a').each(enableDragToDesktop);
          // Open download dialogs via iframes, to prevent aborting current uploads:
          $('#files a:not([target="_blank"])').live('click', function () {
            $('<iframe style="display:none;"/>')
                    .attr('src', this.href)
                    .appendTo('body');
            return false;
          });
        };

  this.initialize = function () {
    settings.autoUpload = true;
    tmplHelper = new TemplateHelper(locale, settings);
    initEventHandlers();
    $('#tabs').tabs();
    $('#radio').fadeIn().buttonset();
    $('#file_upload').fileUploadUI(fileUploadOptions);
  };
};