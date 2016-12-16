$(document).ready(function () {
  LoadOrder();
  CreateDOMEvents();
  if (window.parent.parent.parent.Ts.System.Organization.OrganizationID != 1078
      && window.parent.parent.parent.Ts.System.Organization.OrganizationID != 13679
      && window.parent.parent.parent.Ts.System.Organization.OrganizationID != 1088
      && window.parent.parent.parent.Ts.System.Organization.OrganizationID != 362372)
  {
      $('#btnAddTicketPlugin').remove();

  }
});

var _pluginID = -1;



function LoadOrder() {
    //get ticket categories and append them to list
    window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPageOrder('TicketFieldsOrder', function (items) {
      jQuery.each(items, function (i, val) {
        if (val.Disabled == "false") {
          CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', val);
        }
        else {
          CreateOrderElement('.admin-ticket-page-fields-disabled', 'admin-ticket-page-field', val);
        }
      });
    });

    window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPageOrder('NewTicketFieldsOrder', function (items) {
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
        items: '.admin-ticket-page-field', connectWith: '.ticket-page-sortable', placeholder: 'admin-to-cat-placeholder', handle: '.fa-bars', update: function (e, ui) {
            //save when updated
        SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
        }
    });

    $('.admin-newticket-page-fields, .admin-newticket-page-fields-disabled').sortable({
        items: '.admin-newticket-page-field', connectWith: '.newticket-page-sortable', placeholder: 'admin-to-cat-placeholder', handle: '.fa-bars', update: function (e, ui) {
          //save when updated
          SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
        }
    });

    $('body').on('click', '.admin-to-cat.admin-linebreak .fa-times', function (e) {
        $(this).parent().remove();
        SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
        SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
    });
};

function CreateOrderElement(parent, cssclass, item) {
    if (item.ItemID) {
        $(parent).append('<div data-itemid="' + item.ItemID + '" data-catid="' + item.CatID + '" class="admin-to-cat admin-plugin ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span><i class="fa fa-pencil color-darkgray"></i></div>');
    }
    else {
        if (item.CatID == "hr") {
            $(parent).append('<div data-catid="' + item.CatID + '" class="admin-to-cat admin-linebreak ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span><i class="fa fa-times color-darkgray"></i></div>');
        }
        else {
            $(parent).append('<div data-catid="' + item.CatID + '" class="admin-to-cat ' + cssclass + '"><i class="fa fa-bars color-lightgray"></i><span class="admin-to-cat-name">' + item.CatName + '</span></div>');
        }
    }
};

function CreateDOMEvents() {
    //refresh page
    $('#btnRefresh').click(function (e) {
        e.preventDefault();
        window.location = window.location;
    });
    $('button').button();
    $('#btnAddTicketPlugin').click(function (e) {
        e.preventDefault();
        _pluginID = -1;
        $('#plugin-name').val('');
        $('#plugin-code').val('');
        $('#plugin-name').closest('.form-group').removeClass('has-error');
        $('#modal-plugin').modal('show');

    });

    $('body').on('click', '.admin-to-cat .fa-pencil', function (e) {
        _pluginID = $(this).closest('div').data('itemid');
        window.parent.parent.parent.Ts.Services.TicketPage.GetTicketPagePlugin(_pluginID, function (result) {
            $('#plugin-name').val(result.Name);
            $('#plugin-code').val(result.Code);
            $('#plugin-name').closest('.form-group').removeClass('has-error');
            $('#modal-plugin').modal('show');
        });
    });

    $('#plugin-variables').hide();
    $('#plugin-show-variables').click(function (e) {
        e.preventDefault();
        $('#plugin-show-variables').hide();
        $('#plugin-variables').show();
    });

    $('#plugin-hide-variables').click(function (e) {
        e.preventDefault();
        $('#plugin-show-variables').show();
        $('#plugin-variables').hide();
    });

    $('#btnPluginSave').click(function (e) {
        e.preventDefault();
        if ($('#plugin-name').val().trim() == '')
        {
            $('#plugin-name').closest('.form-group').addClass('has-error');
            return;
        }

        window.parent.parent.parent.Ts.Services.TicketPage.SaveTicketPagePlugin(_pluginID, $('#plugin-name').val(), $('#plugin-code').val(), function (result) {
            if (_pluginID < 0)
            {
                var item = {};
                item.CatID = "plugin";
                item.CatName = result.Name;
                item.ItemID = result.PluginID;
                item.Disabled = "false";
                CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', item);
                $('.admin-ticket-page-fields, .admin-ticket-page-fields-disabled').sortable("refresh");
                SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
            }
        });
        $('#modal-plugin').modal('hide');
    });
    
    $('#btnPluginDelete').click(function (e) {
        e.preventDefault();
        if (confirm('Are you sure you would like to delete this plugin?')) {
            window.parent.parent.parent.Ts.Services.TicketPage.DeleteTicketPagePlugin(_pluginID);
            $("body").find('[data-itemid="' + _pluginID + '"]').remove();
            $('#modal-plugin').modal('hide');
            SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
        }

    });

    $('#btnAddSpacerNewTicket').click(function (e) {
      var item = Object();
      item.CatID = 'hr';
      item.CatName = 'Line Break'
      CreateOrderElement('.admin-newticket-page-fields', 'admin-newticket-page-field', item);
      SaveOrder('.admin-newticket-page-fields', '.admin-newticket-page-field', 'NewTicketFieldsOrder');
    });
    $('#btnAddSpacerExistingTicket').click(function (e) {
      var item = Object();
      item.CatID = 'hr';
      item.CatName = 'Line Break'
      CreateOrderElement('.admin-ticket-page-fields', 'admin-ticket-page-field', item);
      SaveOrder('.admin-ticket-page-fields', '.admin-ticket-page-field', 'TicketFieldsOrder');
    });
};


function SaveOrder(parent, classname, group) {
  var items = new window.parent.parent.parent.Array();

  $(parent + ' > ' + classname).each(function (index) {
        var item = new Object();
        item.CatID = $(this).data('catid');
        item.ItemID = $(this).data('itemid');
        item.CatName = $(this).find('.admin-to-cat-name').text();
        item.Disabled = "false";
        items.push(item);
    });
  $(parent + '-disabled > ' + classname).each(function (index) {
      var item = new Object();
      item.CatID = $(this).data('catid');
      item.ItemID = $(this).data('itemid');
      item.CatName = $(this).find('.admin-to-cat-name').text();
      item.Disabled = "true";
      items.push(item);
    });
    window.parent.parent.parent.Ts.System.logAction('Admin Ticket Order - ' + group + ' Order Saved');
    window.parent.parent.parent.Ts.Services.TicketPage.SaveTicketPageOrder(group, JSON.stringify(items));

};
