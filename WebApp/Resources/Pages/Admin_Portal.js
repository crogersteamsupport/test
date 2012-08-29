/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />

var adminPortal = null;
$(document).ready(function () {
  adminPortal = new AdminPortal();
  adminPortal.refresh();
});

function onShow() {
  adminPortal.refresh();
};


AdminPortal = function () {
  $('#btnRefresh')
  .click(function (e) {
    e.preventDefault();
    window.location = window.location;
  })
  .toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

  if (top.Ts.System.Organization.ProductType == top.Ts.ProductType.Express) {
    $('#tab-community').remove();
    $('#tabs-community').hide();
    $('#tab-advanced').remove();
    $('#tabs-advanced').hide();
  }


  //$('a').addClass('ui-state-default ts-link');
  $('button').button();
  $('#tabs').tabs().find('.ui-tabs-panel').addClass('ui-corner-all');
  $('.loading-section').hide().next().show();

  $('.tabs-modifiable input.text, .tabs-modifiable input.checkbox, .tabs-modifiable textarea').click(function (e) {
    $('.portal-save-panel').show();
  });

  $('.com-cat-properties input.text, .com-cat-properties input.checkbox, .com-cat-properties textarea').click(function (e) {
    $('.com-cat-save-panel').show();
  });

  var enablingCommunity = false;
  $('.com-enable').click(function (e) {
    e.preventDefault();
    if (enablingCommunity) return;
    enablingCommunity = true;
    top.Ts.Services.Organizations.UpdateUseCommunity($(this).text().indexOf('Enable') > -1, function (result) {
      if (result != null) {
        $('.com-enable').button('option', 'label', (result == true ? "Disable Community" : "Enable Community"));
      }
      enablingCommunity = false;
    });
  });

  $('.portal-save-panel').hide();
  $('.portal-save').click(function (e) { saveValues(_portalOption); });
  $('.portal-cancel').click(function (e) {
    e.preventDefault();
    $('.portal-save-panel').hide();
    getData();
  });


  loadCombos();

  function loadCombos() {
    var groups = top.Ts.Cache.getGroups();
    $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#portal_def_group');
    for (var i = 0; i < groups.length; i++) {
      $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('#portal_def_group');
    }

    $('#portal_def_group').combobox({ selected: function (e, ui) { $('.portal-save-panel').show(); } });

    $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_group');
    for (var i = 0; i < groups.length; i++) {
      $('<option>').attr('value', groups[i].GroupID).text(groups[i].Name).data('o', groups[i]).appendTo('#com_cat_group');
    }
    $('#com_cat_group').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });

    var ticketTypes = top.Ts.Cache.getTicketTypes();
    //$('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_tickettype');
    for (var i = 0; i < ticketTypes.length; i++) {
      $('<option>').attr('value', ticketTypes[i].TicketTypeID).text(ticketTypes[i].Name).data('o', ticketTypes[i]).appendTo('#com_cat_tickettype');
    }
    $('#com_cat_tickettype').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });

    var products = top.Ts.Cache.getProducts();
    $('<option>').attr('value', -1).text('Unassigned').data('o', null).appendTo('#com_cat_product');
    for (var i = 0; i < products.length; i++) {
      $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#com_cat_product');
    }
    $('#com_cat_product').combobox({ selected: function (e, ui) { $('.com-cat-save-panel').show(); } });


  }
  $('#portal_theme').combobox({ selected: function (e, ui) { $('.portal-save-panel').show(); } });

  var _portalOption = null;

  function getData() {
    var organization = null;
    top.Ts.Services.Organizations.GetOrganization(top.Ts.System.Organization.OrganizationID, function (org) {
      organization = org;
      top.Ts.Services.Organizations.GetPortalOption(organization.OrganizationID, function (po) {
        top.Ts.Settings.clearCache();
        top.Ts.Settings.Organization.read('ExternalPortalLink', '', function (externalLink) {
          _portalOption = po;
          loadValues(organization, _portalOption, externalLink);
        });
      });
    });
  }

  getData();

  function loadValues(organization, portalOption, externalLink) {
    $('#portal_name').val(portalOption.PortalName);
    $('#portal_deflection').prop('checked', portalOption.DeflectionEnabled == null ? false : portalOption.DeflectionEnabled);
    $('#portal_def_group').combobox('setValue', organization.DefaultPortalGroupID);
    $('#portal_show_grouplist').prop('checked', portalOption.DisplayGroups == null ? false : portalOption.DisplayGroups);
    $('#portal_show_product').prop('checked', portalOption.DisplayProducts == null ? false : portalOption.DisplayProducts);
    $('#portal_show_version').prop('checked', portalOption.DisplayProductVersion == null ? false : portalOption.DisplayProductVersion);
    $('#portal_theme').combobox('setValue', portalOption.Theme);
    $('#portal_header').val(portalOption.PortalHTMLHeader);
    $('#portal_footer').val(portalOption.PortalHTMLFooter);
    $('#portal_screen_rec').prop('checked', portalOption.EnableScreenr);
    $('#portal_twocolumn').prop('checked', portalOption.TwoColumnFields);

    $('#portal_adv_url').text('https://portal.teamsupport.com/' + portalOption.PortalName).attr('href', 'https://portal.teamsupport.com/' + portalOption.PortalName);
    $('#portal_landing_enabled').prop('checked', portalOption.DisplayLandingPage);
    $('#portal_landing_html').val(portalOption.LandingPageHtml);
    $('#portal_external_link').val(externalLink);
    $('#portal_adv_width').val(portalOption.AdvPortalWidth);
    $('#portal_show_user').prop('checked', portalOption.HideUserAssignedTo == null ? true : !portalOption.HideUserAssignedTo);
    $('#portal_show_group').prop('checked', portalOption.HideGroupAssignedTo == null ? true : !portalOption.HideGroupAssignedTo);
    $('#portal_adv_kb').prop('checked', portalOption.DisplayAdvKB == true);
    $('#portal_adv_wiki').prop('checked', portalOption.DisplayAdvArticles == true);
    $('#portal_adv_enable_toc').prop('checked', portalOption.DisplayTandC == true);
    $('#portal_adv_toc').val(portalOption.TermsAndConditions);
    $('#portal_adv_autoregister').prop('checked', portalOption.AutoRegister == true);

    

    $('#portal_captcha').prop('checked', portalOption.UseRecaptcha == null ? false : portalOption.UseRecaptcha);
    $('#portal_basic_header').val(portalOption.BasicPortalDirections);
    $('#portal_basic_width').val(portalOption.BasicPortalColumnWidth);
    $('#portal_ticket_url').text('https://ticket.teamsupport.com/' + portalOption.PortalName).attr('href', 'https://ticket.teamsupport.com/' + portalOption.PortalName);
    $('#portal_use_company').prop('checked', portalOption.UseCompanyInBasic == null ? false : portalOption.UseCompanyInBasic);
    $('#portal_company_required').prop('checked', portalOption.CompanyRequiredInBasic == null ? false : portalOption.CompanyRequiredInBasic);
    $('#portal_kb_url').text('https://kb.teamsupport.com/' + portalOption.PortalName).attr('href', 'https://kb.teamsupport.com/' + portalOption.PortalName);
    $('#portal_allow_kb').prop('checked', portalOption.KBAccess == null ? false : portalOption.KBAccess);
    $('#portal_wiki_url').text('https://articles.teamsupport.com/' + portalOption.PortalName).attr('href', 'https://articles.teamsupport.com/' + portalOption.PortalName);
    $('#portal_allow_wiki').prop('checked', organization.IsPublicArticles);
    $('#portal_landing_url').text('https://publicportal.teamsupport.com/' + portalOption.PortalName).attr('href', 'https://publicportal.teamsupport.com/' + portalOption.PortalName);
    $('#portal_public_landing_body').val(portalOption.PublicLandingPageBody);
    $('#portal_public_landing_header').val(portalOption.PublicLandingPageHeader);

    $('#portal_com_enabled').prop('checked', portalOption.DisplayForum == null ? false : portalOption.DisplayForum);

    $('.com-enable').button('option', 'label', (portalOption.DisplayForum == true ? "Disable Community" : "Enable Community"));

  }

  function saveValues(portalOption) {
    portalOption.PortalName = $('#portal_name').val();
    portalOption.DeflectionEnabled = $('#portal_deflection').prop('checked');
    //organization.DefaultPortalGroupID = $('#portal_def_group').val();
    portalOption.DisplayGroups = $('#portal_show_grouplist').prop('checked');
    portalOption.DisplayProducts = $('#portal_show_product').prop('checked');
    portalOption.DisplayProductVersion = $('#portal_show_version').prop('checked');
    portalOption.Theme = $('#portal_theme').val();
    portalOption.PortalHTMLHeader = $('#portal_header').val();
    portalOption.PortalHTMLFooter = $('#portal_footer').val();
    portalOption.EnableScreenr = $('#portal_screen_rec').prop('checked');
    portalOption.TwoColumnFields = $('#portal_twocolumn').prop('checked');

    portalOption.DisplayLandingPage = $('#portal_landing_enabled').prop('checked');
    portalOption.LandingPageHtml = $('#portal_landing_html').val();
    //$('#portal_external_link').val();
    portalOption.AdvPortalWidth = $('#portal_adv_width').val();
    portalOption.HideUserAssignedTo = !$('#portal_show_user').prop('checked');
    portalOption.HideGroupAssignedTo = !$('#portal_show_group').prop('checked');
    portalOption.DisplayAdvKB = $('#portal_adv_kb').prop('checked');
    portalOption.DisplayAdvArticles = $('#portal_adv_wiki').prop('checked');
    portalOption.DisplayTandC = $('#portal_adv_enable_toc').prop('checked');
    portalOption.AutoRegister = $('#portal_adv_autoregister').prop('checked');

    portalOption.TermsAndConditions = $('#portal_adv_toc').val();

    portalOption.UseRecaptcha = $('#portal_captcha').prop('checked');
    portalOption.BasicPortalDirections = $('#portal_basic_header').val();
    portalOption.BasicPortalColumnWidth = $('#portal_basic_width').val();
    portalOption.UseCompanyInBasic = $('#portal_use_company').prop('checked');
    portalOption.CompanyRequiredInBasic = $('#portal_company_required').prop('checked');
    portalOption.KBAccess = $('#portal_allow_kb').prop('checked');
    //organization.IsPublicArticles = $('#portal_allow_wiki').prop('checked');
    portalOption.PublicLandingPageBody = $('#portal_public_landing_body').val();
    portalOption.PublicLandingPageHeader = $('#portal_public_landing_header').val();
    portalOption.DisplayForum = $('#portal_com_enabled').prop('checked');
    // show some indicator




    top.Ts.Services.Organizations.SetPortalOption(portalOption, $('#portal_external_link').val(), $('#portal_allow_wiki').prop('checked'), $('#portal_def_group').val() == -1 ? null : $('#portal_def_group').val(), function (result) {
      if (result != null) {
        alert(result);
      }
      else {
        $('.portal-save-panel').hide();
      }
    });
  }

  getForumCats()
  function getForumCats() {
    top.Ts.Services.Admin.GetForumCategories(function (cats) {
      $('.com-cat-list').empty();
      for (var i = 0; i < cats.length; i++) {
        var cat = cats[i].Category;
        createCategory(cat);
        var subs = cats[i].Subcategories;
        for (var j = 0; j < subs.length; j++) {
          var sub = subs[j];
          createSubCategory(sub);
        }
      }
      if ($('.com-cat').length < 1) {
        $('.no-cat').show().next().hide();
      }
      else {
        $('.no-cat').hide().next().show();
        $('.com-cat:first').click();
      }

    });
  }

  function createCategory(cat) {
    var container = $('<div>')
      .addClass('com-cats');

    var el = $('<div>')
      .addClass('com-cat com-cat-main ui-corner-all com-cat-' + cat.CategoryID)
      .text(cat.CategoryName)
      .data('cat', cat)
      .appendTo(container);

    $('<a>')
      .addClass('ui-state-default ts-link com-new-sub')
      .attr('href', '#')
      .text('Add a new subcategory')
      .appendTo($('<div>').appendTo(container));

    $('.com-cat-list').append(container);

    setSortable();
    return el;
  }

  function createSubCategory(cat) {
    var container = $('.com-cat-list').find('.com-cat-' + cat.ParentID);
    if (container.length < 1) return;
    container = container.parent().find('.com-new-sub');

    var el = $('<div>')
      .addClass('com-cat com-cat-sub ui-corner-all com-cat-' + cat.CategoryID)
      .text(cat.CategoryName)
      .data('cat', cat)
      .insertBefore(container);

    setSortable();
    return el;
  }

  $('.com-cat-list').delegate('.com-cat', 'mouseover mouseout', function (e) {
    $('.com-cat-list .com-cat').removeClass('ui-state-hover');
    if (e.type == 'mouseover') { $(this).addClass('ui-state-hover'); }
  });

  $('.com-cat-list').delegate('.com-cat', 'click', function (e) {
    $('.com-cat-list .com-cat').removeClass('ui-state-active');
    $(this).addClass('ui-state-active');

    var cat = $(this).data('cat');
    $('#com_cat_name').val(cat.CategoryName);
    $('#com_cat_description').val(cat.CategoryDesc);
    $('#com_cat_tickettype').combobox('setValue', cat.TicketType == null ? -1 : cat.TicketType);
    $('#com_cat_group').combobox('setValue', cat.GroupID == null ? -1 : cat.GroupID);
    $('#com_cat_product').combobox('setValue', cat.ProductID == null ? -1 : cat.ProductID);

    if (cat.ParentID < 0) {
      $('.com-delete-cat').text('Delete this category and all its subcategories');
      $('.com-sub-only').hide();
    }
    else {
      $('.com-delete-cat').text('Delete this subcategory');
      $('.com-sub-only').show();
    }
  });

  $('.com-delete-cat').click(function (e) {
    e.preventDefault();
    var el = $('.com-cat-list .ui-state-active.com-cat');
    if (el.length < 1) return;
    var cat = el.data('cat');
    var isMain = el.hasClass('com-cat-main') == true;

    if (isMain == true) {
      if (!confirm("Are you sure you would like to delete the category '" + cat.CategoryName + "' and ALL its subcategories?")) return;
    }
    else {
      if (!confirm("Are you sure you would like to delete subcategory '" + cat.CategoryName + "'?")) return;
    }


    top.Ts.Services.Admin.DeleteForumCategory(cat.CategoryID, function (result) {
      if (result == false) return;

      if (isMain == true) {
        el.parent().remove();
      }
      else {
        el.remove();
      }


      if ($('.com-cat').length < 1) {
        $('.no-cat').show().next().hide();
      }
      else {
        $('.no-cat').hide().next().show();
        $('.com-cat:first').click();
      }


    });
  });

  $('.com-save-cat').click(function (e) {
    e.preventDefault();
    var el = $('.com-cat-list .ui-state-active.com-cat');
    if (el.length < 1) return;
    var cat = el.data('cat');

    top.Ts.Services.Admin.UpdateForumCategory(
      cat.CategoryID,
      $('#com_cat_name').val(),
      $('#com_cat_description').val(),
      ($('#com_cat_tickettype').val() < 0 ? null : $('#com_cat_tickettype').val()),
      ($('#com_cat_group').val() < 0 ? null : $('#com_cat_group').val()),
      ($('#com_cat_product').val() < 0 ? null : $('#com_cat_product').val()),
      function (result) {
        if (result == null) return;
        $('.com-cat-save-panel').hide();
        updateCat(result);
      });
  });

  function updateCat(cat) {
    var el = $('.com-cat-list').find('.com-cat-' + cat.CategoryID);
    if (el.length < 1) return;
    el.data('cat', cat);
    el.text(cat.CategoryName);
  }

  $('.com-new-cat').click(function (e) {
    e.preventDefault();
    top.Ts.Services.Admin.AddForumCategory(null, function (result) {
      if (result != null) {
        var el = createCategory(result);
        el.click();
        $('.no-cat').hide().next().show();
      }
    });
  });

  $('.com-cat-list').delegate('.com-new-sub', 'click', function (e) {
    e.preventDefault();
    var parentCat = $(this).closest('.com-cats').find('.com-cat-main').data('cat');
    top.Ts.Services.Admin.AddForumCategory(parentCat.CategoryID, function (result) {
      if (result != null) {
        var el = createSubCategory(result);
        el.click();
        $('.no-cat').hide().next().show();
        $('.com-cat-save-panel').show();
        $('#com_cat_name').focus().select();
      }
    });
  });

  function setSortable() {
    $('.com-cats').sortable({ items: '.com-cat-sub', connectWith: '.com-cats', placeholder: 'ui-state-highlight com-cat-sub ui-corner-all', update: function (e, ui) {
      savePositions();
    }
    });

    $('.com-cat-list').sortable({ items: '.com-cats', placeholder: 'ui-state-highlight ui-widget-content ui-corner-all ts-section .com-cats', update: function (e, ui) {
      savePositions();
    }
    });
  }

  function savePositions() {
    var orders = new top.Array();
    $('.com-cats').each(function () {

      var item = new Object(); // top.TSWebServices.ForumCategoryOrder();
      item.CategoryIDs = new top.Array();
      orders[orders.length] = item;

      var cat = $(this).find('.com-cat-main').data('cat');

      item.ParentID = cat.CategoryID;

      $(this).find('.com-cat-sub').each(function () {
        var sub = $(this).data('cat');
        item.CategoryIDs[item.CategoryIDs.length] = sub.CategoryID

      });
    });

    top.Ts.Services.Admin.UpdateForumCategoryOrder(JSON.stringify(orders));
  }
};


AdminPortal.prototype = {
  constructor: AdminPortal,
  refresh: function () {

  }
};
