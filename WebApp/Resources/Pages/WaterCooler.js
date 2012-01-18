/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var waterCoolerPage = null;
$(document).ready(function () {
  waterCoolerPage = new WaterCoolerPage();
  waterCoolerPage.refresh();
});

function onShow() {
  waterCoolerPage.refresh();
};


WaterCoolerPage = function () {
  // Get the top 25 WC threads and display them
  // this funciton is on the webservice in the AppCode dir.  I just returns a WC object in that file
  top.Ts.Services.WaterCooler.GetThreads(function (threads) {
    var threadContainer = $('<div>');
    
    for (var i = 0; i < threads.length; i++) {
      var div = createThread(threads[i]);
      threadContainer.append(div);
    }

    // insert the threads into the div
    $('.wc-threads').empty().append(threadContainer);
    $('.loading-section').hide().next().show();
  });


  //create a thread div
  // you could use this on a timer to check for new threads and just load it from a web service so you don't have to load the whole page,
  // you could also have a thread object returned to you when you create a new one so you don't have to update the page
  function createThread(thread) {
    var threadDiv = $('<div>')
        .addClass('wc-thread wc-message')
        .data('message', thread.Message);

    $('<h1>')
        .text(thread.Message.UserName)
        .appendTo(threadDiv);

    $('<div>')
        .html(thread.Message.Message)
        .appendTo(threadDiv);

    for (var j = 0; j < thread.Replies.length; j++) {
      var div = createReply(thread.Replies[j]);
      threadDiv.append(div);
    }

    var linkDiv = $('<div>').appendTo(threadDiv);
    $('<a>')
      .addClass('wc-reply-link ui-state-default ts-link')
      .text('Reply')
      .appendTo(linkDiv);

    $('<a>')
      .addClass('wc-delete-link ui-state-default ts-link')
      .text('Delete')
      .appendTo(linkDiv);

    return threadDiv;
  }


  // create a reply div
  function createReply(reply) {
    var replyDiv = $('<div>')
          .addClass('wc-reply wc-message ts-color-bg-accent ui-widget ui-corner-all')
          .html(reply.UserName + ': ' + reply.Message)
          .data('message', reply);

    return replyDiv;
  }


  // REPLY LINK Event
  // when some one clicks on the reply link
  $('.wc-threads').delegate('.wc-reply-link', 'click', function (e) {
    e.preventDefault();
    e.stopPropagation();

    $(this).hide();

    var container = $('<div>')
      .addClass('wc-edit')
      .appendTo($(this).parent());

    // load the editor
    var editor = $('<textarea>').width('100%').appendTo(container);
    initEditor(editor, function (editor) {
      // if you want to init some text, do it here
    });


    $('<button>')
      .text('OK')
      .click(function (e) {
        // Lets save it here

        // get the data stored in the closest parent .wc-thread div
        var parent = $(this).closest('.wc-thread');
        var data = parent.data('message');
        top.Ts.Services.WaterCooler.AddMessage(data.MessageID, $(editor).html(), function (newMessage) {
          var div = createReply(newMessage);
          div.hide().insertBefore(parent.find('.wc-reply-link')).fadeIn('slow');
        });
        parent.find('.wc-reply-link').show();
        parent.find('.wc-edit').remove();
      })
      .appendTo(container)
      .button();

    $('<button>')
      .text('Cancel')
      .click(function (e) {
        var parent = $(this).closest('.wc-thread');
        parent.find('.wc-reply-link').show();
        parent.find('.wc-edit').remove();
      })
      .appendTo(container)
      .button();
  });
  //-- end reply link


  // delete link event
  $('.wc-threads').delegate('.wc-delete-link', 'click', function (e) {
    var parent = $(this).closest('.wc-message');
    var message = parent.data('message');
    top.Ts.Services.WaterCooler.DeleteMessage(message.MessageID, function (result) {
      if (result == true) parent.remove();
    });

  });


  // set up the refresh button so we can just click that to see our dev changes
  $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; }).toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  // change the style of some stuff
  $('button').button();
  $('a').addClass('ui-state-default ts-link');


  // function to init the editor
  function initEditor(element, init) {
    var editorOptions = {
      theme: "advanced",
      skin: "o2k7",
      plugins: "autoresize,paste,table,spellchecker,inlinepopups,table",
      theme_advanced_buttons1: "insertTicket,insertKb,|,link,unlink,|,undo,redo,removeformat,|,cut,copy,paste,pastetext,pasteword,|,cleanup,code,|,outdent,indent,|,bullist,numlist",
      theme_advanced_buttons2: "forecolor,backcolor,fontselect,fontsizeselect,bold,italic,underline,strikethrough,blockquote,|,spellchecker",
      theme_advanced_buttons3: "",
      theme_advanced_buttons4: "",
      theme_advanced_toolbar_location: "top",
      theme_advanced_toolbar_align: "left",
      theme_advanced_statusbar_location: "none",
      theme_advanced_resizing: true,
      autoresize_bottom_margin: 10,
      autoresize_on_init: true,
      spellchecker_rpc_url: "../../../TinyMCEHandler.aspx?module=SpellChecker",
      gecko_spellcheck: true,
      extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type]",
      convert_urls: true,
      remove_script_host: false,
      relative_urls: false,
      content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
      body_class: "ui-widget ui-widget-content",

      template_external_list_url: "tinymce/jscripts/template_list.js",
      external_link_list_url: "tinymce/jscripts/link_list.js",
      external_image_list_url: "tinymce/jscripts/image_list.js",
      media_external_list_url: "tinymce/jscripts/media_list.js",
      setup: function (ed) {
        ed.addButton('insertTicket', {
          title: 'Insert Ticket',
          image: '../images/nav/16/tickets.png',
          onclick: function () {
            top.Ts.MainPage.selectTicket(null, function (ticketID) {
              top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                ed.focus();

                var html = '<a href="' + top.Ts.System.AppDomain + '?TicketNumber=' + ticket.TicketNumber + '" target="_blank" onclick="top.Ts.MainPage.openTicket(' + ticket.TicketNumber + '); return false;">Ticket ' + ticket.TicketNumber + '</a>';
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();
              }, function () {
                alert('There was a problem inserting the ticket link.');
              });
            });
          }
        });
        ed.addButton('insertKb', {
          title: 'Insert Knowledgebase',
          image: '../images/nav/16/knowledge.png',
          onclick: function () {
            filter = new top.TeamSupport.Data.TicketLoadFilter();
            filter.IsKnowledgeBase = true;
            top.Ts.MainPage.selectTicket(filter, function (ticketID) {
              top.Ts.Services.Tickets.GetKBTicketAndActions(ticketID, function (result) {
                if (result === null) {
                  alert('There was an error inserting your knowledgebase ticket.');
                  return;
                }
                var ticket = result[0];
                var actions = result[1];

                var html = '<div><h2>' + ticket.Name + '</h2>';

                for (var i = 0; i < actions.length; i++) {
                  html = html + '<div>' + actions[i].Description + '</div></br>';
                }
                html = html + '</div>';

                ed.focus();
                ed.selection.setContent(html);
                ed.execCommand('mceAutoResize');
                ed.focus();

                //needs to resize or go to end

              }, function () {
                alert('There was an error inserting your knowledgebase ticket.');
              });
            });
          }
        });
      }
    , oninit: init
    };
    $(element).tinymce(editorOptions);
  }

};


WaterCoolerPage.prototype = {
  constructor: WaterCoolerPage,
  refresh: function () {

  }
};
