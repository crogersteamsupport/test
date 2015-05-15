$(document).ready(function () {
  LoadOrder();
    CreateDOMEvents();
});

function LoadOrder() {
    //get ticket categories and append them to list
    top.Ts.Services.TicketPage.GetTicketPageOrder('TicketFieldsOrder', function (items) {
      jQuery.each(items, function (i, val) {
        if (val.Disabled == "false") {
          CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', val);
        }
        else {
          CreateOrderElement('.admin-ticket-page-fields-disabled', 'admin-ticket-page-field', val);
        }
      });
    });

    top.Ts.Services.TicketPage.GetTicketPageOrder('NewTicketFieldsOrder', function (items) {
      jQuery.each(items, function (i, val) {
        if (val.Disabled == "false") {
          CreateOrderElement('.admin-newticket-page-fields', 'admin-newticket-page-field', val);
        }
        else {
          CreateOrderElement('.admin-newticket-page-fields-disabled', 'admin-newticket-page-field', val);
        }
      });
    });

    //now make them sortable
    $('.admin-ticket-page-fields, .admin-ticket-page-fields-disabled').sortable({
      items: '.admin-ticket-page-field', connectWith: '.ticket-page-sortable', placeholder: 'ui-state-highlight admin-to-cat ui-corner-all', update: function (e, ui) {
            //save when updated
        SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
        }
    });

    $('.admin-newticket-page-fields, .admin-newticket-page-fields-disabled').sortable({
      items: '.admin-newticket-page-field', connectWith: '.newticket-page-sortable', placeholder: 'ui-state-highlight admin-to-cat ui-corner-all', update: function (e, ui) {
          //save when updated
          SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
        }
    });
};

function CreateOrderElement(parent, cssclass, item) {
    $(parent).append('<div data-catid="' + item.CatID + '" class="admin-to-cat ' + cssclass + '"><span class="admin-to-cat-name">' + item.CatName + '</span></div>');
};

function CreateDOMEvents() {
    //refresh page
    $('#btnRefresh').click(function (e) {
        e.preventDefault();
        window.location = window.location;
    });
    $('button').button();
};


function SaveOrder(parent, classname, group) {
  var items = new top.Array();

  $(parent + ' > ' + classname).each(function (index) {
        var item = new Object();
        item.CatID = $(this).data('catid');
        item.CatName = $(this).find('.admin-to-cat-name').text();
        item.Disabled = "false";
        items.push(item);
    });
  $(parent + '-disabled > ' + classname).each(function (index) {
      var item = new Object();
      item.CatID = $(this).data('catid');
      item.CatName = $(this).find('.admin-to-cat-name').text();
      item.Disabled = "true";
      items.push(item);
    });
    top.Ts.System.logAction('Admin Ticket Order - ' + group + ' Order Saved');
    top.Ts.Services.TicketPage.SaveTicketPageOrder(group, JSON.stringify(items));

};
