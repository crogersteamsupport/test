var wikiPage = null;
var _wikiArticles = null;
var _wikiID = null;
var _wikiParentID = null;
var _wikiInternalLLinkBase = top.Ts.System.AppDomain + "?articleid={ArticleID}";
var _wikiExternalLinkBase = top.Ts.System.AppDomain + "/wiki/justarticle.aspx?Organizationid={ORGID}&ArticleID={ArticleID}";
var _wikiInternalLink = null;
var _wikiExternalLink = null;
var _wikiTitle = null;
var _wikiBody = null;
var _wikiSubArticleList = null;
var _wikiPortalView = false;
var _wikiPublicView = false;
var _wikiPrivateView = false;
var _wikiVersion = 0;
var _wikiModifiedDate = null;
var _wikiParentsList = null;
var _wikiMenuLITemplate = '<li class="wiki-menu-item"><a id="{ID}" href="#">{Title}</a>';
var _wikiMenuLIWithChildrenTemplate = '<li class="wiki-menu-item"><a id="{ID}" href="#">{Title}<span class="caret wiki-sidebar-caret wiki-sidebar-caret-right"></span></a>';
var _wikiSubMenuULTemplate = '<ul class="nav wiki-sidebar-subitem">';
var _wikiSubMenuLITemplate = '<li class="wiki-menu-subitem"><a id="{ID}" href="#">{Title}<span class="caret wiki-sidebar-caret  wiki-sidebar-caret-right"></span></a></li>';
var _editingWiki = false;
var _isWikiOwner = false;
var _canDeleteWiki = false;
var _isCreatingNewWiki = false;

$(document).ready(function () {
    wikiPage = new WikiPage();
    wikiPage.refresh();
});

function onShow() {
};

WikiPage = function () {
    this.refresh = function () {
        BuildWikiPage();
        BuildWikiEditEvents();
    }
}

//Build Page Functions
function BuildWikiPage() {
    top.Ts.Services.Wiki.GetWikiMenuItems(function (menuItems) {
        _wikiArticles = menuItems;
        if (menuItems !== null) {
            if (_wikiID == null) {
                var articleID = top.Ts.Utils.getQueryValue("ArticleID", window);
                if (articleID == null) {
                    top.Ts.Services.Wiki.GetDefaultWikiID(function (wikiID) {
                        if (wikiID == null) {
                            _wikiID = menuItems[0].ID
                        }
                        else {
                            _wikiID = wikiID;
                        }
                        GetWiki(_wikiID);
                        BuildWikiMenuItems();
                    });
                }
                else {
                    GetWiki(articleID);
                    BuildWikiMenuItems();
                }
            }
            else {
                GetWiki(_wikiID);
                BuildWikiMenuItems();
            };
            $("#EditWiki").show();
        }
        else {
            $("#EditWiki").hide();
        };
    });
    top.Ts.System.logAction('Wiki - Wiki Viewed');
};


function BuildWikiView() {
    var internalLink = new ZeroClipboard(document.getElementById("wiki-internal-link"));
    $("#wiki-internal-link").attr("data-clipboard-text", _wikiInternalLink);


    if (!_wikiPublicView) {
        $("#wiki-external-link").hide();
    }
    else {
        var externalLink = new ZeroClipboard(document.getElementById("wiki-external-link"));
        $("#wiki-external-link").attr("data-clipboard-text", _wikiExternalLink);

        $("#wiki-external-link").show();
    }

    $(".wiki-tools").tooltip({ placement: 'bottom', animation: false })

    $("#Wiki-Title").text(_wikiTitle);
    $("#Wiki-Body").html(_wikiBody);
    $("#Wiki-Edit-Title").val(_wikiTitle);
    $("#Wiki-Edit-Parent").val(_wikiParentID);
    $("#Wiki-Edit-PublicView").prop('checked', _wikiPublicView);
    $("#Wiki-Edit-PrivateView").prop('checked', _wikiPrivateView);
    $("#Wiki-Edit-PortalView").prop('checked', _wikiPortalView);
    $("#Wiki-Revisions").val(_wikiVersion);
    $("#WikiViewArea").show();
    $("#WikiEditArea").hide();
    $("#wiki-view-toolbar").show();
    $("#wiki-edit-toolbar").hide();
    $('#Wiki-Title').show();
    $("#wiki-title-edit").hide();

    $('a[href*="JustArticle.aspx"]').click(function (e) {
        e.preventDefault();
        var url = ($(this).attr('href'));
        var ArticleID = getURLParameter(url, 'ArticleID');
        top.Ts.MainPage.openWiki(ArticleID, true)
    });

    var element = $('body');
    tinymce.execCommand('mceRemoveControl', true, '#Wiki-Edit-Body');
    initEditor(element.find('#Wiki-Edit-Body'), function (ed) {
        element.find('#Wiki-Edit-Body').tinymce().focus();
    });
};

function getURLParameter(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}

function BuildWikiMenuItems() {
    $('#Wiki-Edit-Parent').find('option').remove().end().append($("<option></option>"));
    $("#wiki-sidebar").empty();

    top.Ts.Services.Wiki.GetWikis(function (wikis) {
        $.each(wikis, function (index, value) {
            $('#Wiki-Edit-Parent')
                .append($("<option></option>")
                .attr("value", value.ID)
                .text(value.Title));
        })
    });

    top.Ts.Services.Wiki.GetWikiParents(function (wikis) {
        if (wikis !== null) {
            var menuParents = "";
            var subMenuItems = "";
            $.each(wikis, function (key, parent) {
                $("#wiki-sidebar").append(_wikiMenuLIWithChildrenTemplate.replace("{ID}", parent.ID).replace("{Title}", parent.Title));
                recursiveFunction(key, parent);
                $("#" + parent.ID).on('click', function (e) {
                    e.preventDefault();
                    SidebarFunction($(this));
                });
            });
        }
    });
};

function recursiveFunction(key, parent) {
    top.Ts.Services.Wiki.GetWikiAndChildren(parent.ID, function (children) {
        if (children.SubArticles !== null) {
            $("#" + parent.ID).parent().append(_wikiSubMenuULTemplate);
            $.each(children.SubArticles, function (key, child) {
                $("#" + parent.ID).parent().children("ul").append(_wikiSubMenuLITemplate.replace("{ID}", child.ID).replace("{Title}", child.Title));
                recursiveFunction(key, child)
                $("#wiki-sidebar").append("</li>");
                $("#" + child.ID).on('click', function (e) {
                    e.preventDefault();
                    SidebarFunction($(this));
                });
            });
        }
        else {
            $("#" + parent.ID + " > span.wiki-sidebar-caret").remove();
        }
        if (parent.ID == _wikiID) {
            var wikiMenuItem = $("#" + _wikiID);
            wikiMenuItem.addClass('active');
            wikiMenuItem.closest('li').children("ul").show();
            wikiMenuItem.find("span.wiki-sidebar-caret").removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('li.wiki-menu-item').children('a').children('span.wiki-sidebar-caret-right').removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('li.wiki-menu-subitem').children('a').children('span.wiki-sidebar-caret-right').removeClass('wiki-sidebar-caret-right');
            wikiMenuItem.parents('.wiki-sidebar-subitem').show();
        }
    });
}

function SidebarFunction(element) {
    $('.wiki-menu-item').children("a").removeClass('active');
    $('.wiki-menu-subitem').children("a").removeClass('active');
    element.addClass('active');
    element.closest('li').children("ul").toggle();
    element.closest('li').find("span.wiki-sidebar-caret").first().toggleClass('wiki-sidebar-caret-right');
    GetWiki(element[0].id);
};

function BuildWikiEditEvents() {
    $("#EditWiki").click(function () {
        $('#Wiki-Title').hide();
        $("#wiki-title-edit").val(_wikiTitle).show();
        $("#Wiki-Edit-Parent").val(_wikiParentID);
        $("#Wiki-Update-Comment").val('');
        $("#wiki-sidebar-div").hide();
        $("#wiki-view-toolbar").hide();
        $("#wiki-edit-toolbar").show();
        $("#WikiViewArea").hide();
        $("#WikiEditArea").show();
        $("#wiki-edit-title-div").show();
        $("#wiki-title-div").hide();
        if (_isWikiOwner) {
            $("#Wiki-Edit-PrivateView").removeAttr("disabled");
        } else {
            $("#Wiki-Edit-PrivateView").attr("disabled", true);
        }
        if (_canDeleteWiki) {
            $("#wiki-edit-delete").show();
        } else {
            $("#wiki-edit-delete").hide();
        }
        _editingWiki = true;
        _isCreatingNewWiki = false;
        tinyMCE.activeEditor.setContent(_wikiBody);
    });

    $("#wiki-edit-cancel").click(function (e) {
        e.preventDefault();
        $("#wiki-view-toolbar").show();
        $("#wiki-edit-toolbar").hide();
        $("#WikiViewArea").show();
        $("#WikiEditArea").hide();
        $('#Wiki-Title').show();
        $("#wiki-title-edit").hide();
        $("#wiki-sidebar-div").show();
        $("#wiki-edit-title-div").hide();
        $("#wiki-title-div").show();
        _editingWiki = false;
        _isCreatingNewWiki = false;
    });

    $("#wiki-edit-delete").click(function (e) {
        e.preventDefault();
        var confirmation = confirm("Are you sure you want to delete this wiki article?");
        if (confirmation) {
            DeleteWiki(_wikiID);
            $("#wiki-view-toolbar").show();
            $("#wiki-edit-toolbar").hide();
            $("#WikiViewArea").show();
            $("#WikiEditArea").hide();
            $('#Wiki-Title').show();
            $("#wiki-title-edit").hide();
            $("#wiki-sidebar-div").show();
            $("#wiki-edit-title-div").hide();
            $("#wiki-title-div").show();
            _editingWiki = false;
            _isCreatingNewWiki = false;
        }
    });



    $("#wiki-edit-save").click(function () {
        var comment = $("#Wiki-Update-Comment").val();
        var title = $("#wiki-title-edit").val();
        var body = $("#Wiki-Edit-Body").html();
        var public = $("#Wiki-Edit-PublicView").is(':checked');
        var private = $("#Wiki-Edit-PrivateView").is(':checked');
        var portal = $("#Wiki-Edit-PortalView").is(':checked');
        var parent = $('#Wiki-Edit-Parent').val();
        var wikiID;

        if (_isCreatingNewWiki) {
            wikiID = 0;
        }
        else {
            wikiID = _wikiID;
        }
        if (wikiID.toString() !== parent) {
            SaveWiki(wikiID, parent, body, title, public, private, portal, comment);
            $('#Wiki-Comment-Modal').modal('hide')
            $("#wiki-view-toolbar").show();
            $("#wiki-edit-toolbar").hide();
            $("#WikiViewArea").show();
            $("#WikiEditArea").hide();
            $('#Wiki-Title').show();
            $("#wiki-title-edit").hide();
            $("#wiki-sidebar-div").show();
            $("#wiki-edit-title-div").hide();
            $("#wiki-title-div").show();
            _editingWiki = false;
        }
        else { alert('Please select a parent article other than the one you are editing.') };

    });

    $("#NewWiki").click(function () {
        _isCreatingNewWiki = true;
        $("#Wiki-Edit-Title").val(null);
        $("#wiki-title-edit").val("");
        $("#Wiki-Edit-Parent").val(0);
        $("#Wiki-Edit-PublicView").prop('checked', false);
        $("#Wiki-Edit-PrivateView").prop('checked', false);
        $("#Wiki-Edit-PortalView").prop('checked', false);
        $('.wiki-revision-history tbody').empty();
        $('.wiki-revision-div').hide();
        $("#wiki-view-toolbar").hide();
        $("#wiki-edit-toolbar").show();
        $('#Wiki-Title').hide();
        $("#wiki-title-edit").show();
        $("#WikiViewArea").hide();
        $("#WikiEditArea").show();
        $("#wiki-sidebar-div").hide();
        $("#wiki-edit-title-div").show();
        $("#wiki-title-div").hide();
        $("#wiki-edit-delete").hide();
        $("#Wiki-Edit-PrivateView").removeAttr("disabled");
        if (tinyMCE.activeEditor !== null) {
            tinyMCE.activeEditor.remove();
        }
        var element = $('body').find('#Wiki-Edit-Body');
        initEditor(element, function (ed) {
            ed.setContent('');
            element.find('#Wiki-Edit-Body').tinymce();
        });
        $("#wiki-title-edit").show().focus();
        top.Ts.System.logAction('Wiki - Wiki Created');
    });

    $("#WikiLink").click(function (e) { e.preventDefault(); });

    $("#wiki-title-edit").on('keydown', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode == 9) {
            e.preventDefault();
            $("#Wiki-Edit-Body").tinymce().focus();
        }
    });

    top.Ts.Services.Settings.SetMoxieManagerSessionVariables();

    $('#Wiki-Comment-Modal').on('shown.bs.modal', function (e) {
        $("#Wiki-Update-Comment").focus();
    });
};

function MapWikiProperties(wiki) {
    _wikiID = wiki.ArticleID;
    _wikiParentID = wiki.ParentID;
    _wikiTitle = wiki.ArticleName;
    _wikiBody = wiki.Body;
    _wikiVersion = wiki.Version;
    _wikiPortalView = wiki.PortalView;
    _wikiPublicView = wiki.PublicView;
    _wikiPrivateView = wiki.Private;
    _wikiModifiedDate = wiki.ModifiedDate;
    _wikiExternalLink = _wikiExternalLinkBase.replace("{ORGID}", wiki.OrganizationID).replace("{ArticleID}", wiki.ArticleID);
    _wikiInternalLink = _wikiInternalLLinkBase.replace("{ArticleID}", wiki.ArticleID);
    _editingWiki = false;
    _isWikiOwner = wiki.IsOwner;
    _canDeleteWiki = wiki.CanDelete;
};

//GET-POST Wiki Functions
function GetWiki(wikiID) {
    top.Ts.Services.Wiki.GetWiki(wikiID, function (wiki) {
        //if not null, this is my wiki article object.
        if (wiki !== null) {
            MapWikiProperties(wiki);
            GetWikiHistory(wikiID);
            BuildWikiView();
        }
        else {
            alert('You do not have access to this wiki or the wiki no longer exists.  Check your default wiki settings to ensure you are not using a old deleted wiki or select a wiki from the menu below. ')
        }
    });
};

function GetWikiHistory(wikiID) {
    top.Ts.Services.Wiki.GetWikiHistory(wikiID, function (wikiHistory) {
        $('.wiki-revision-history tbody').empty();
        if (wikiHistory !== null) {
            $('.wiki-revision-div').show();
            $.each(wikiHistory, function (key, value) {
                $(".wiki-revision-history tbody").append('<tr><td>' + value.RevisionNumber + '</td><td>' + value.RevisedDate.localeFormat(top.Ts.Utils.getDateTimePattern()) + '</td><td>' + value.RevisedBy + '</td><td>' + value.Comment + '</td><td><button data-id="' + value.HistoryID + '" class="btn btn-primary btn-xs wiki-restore">Preview</button></td></tr>');
            });
        }
        else {
            $('.wiki-revision-div').hide();
        }
        $(".wiki-restore").click(function () {
            var wikiRevisionID = $(this).data("id");
            $('.wiki-revision-history tbody > tr').removeClass('active');
            $(this).closest('tr').addClass('active');

            if (wikiRevisionID !== "") {
                top.Ts.Services.Wiki.GetWikiRevision(wikiRevisionID, function (revision) {
                    $("#wiki-title-edit").val(revision.ArticleName);
                    $("#Wiki-Edit-Body").html(revision.Body);
                    $("#Wiki-Edit-PublicView").prop('checked', false);
                    $("#Wiki-Edit-PrivateView").prop('checked', false);
                    $("#Wiki-Edit-PortalView").prop('checked', false);
                });
            }
            else {
                $("#wiki-title-edi").val(_wikiTitle);
                $("#Wiki-Edit-Body").html(_wikiBody);
                $("#Wiki-Edit-PublicView").prop('checked', _wikiPublicView);
                $("#Wiki-Edit-PrivateView").prop('checked', _wikiPrivateView);
                $("#Wiki-Edit-PortalView").prop('checked', _wikiPortalView);
            };
            top.Ts.System.logAction('Wiki - Viewed Revision');
        });
    });
};


function SaveWiki(wikiID, parent, wikiBody, wikiTitle, publicView, privateView, portalView, comment) {
    top.Ts.Services.Wiki.SaveWiki(wikiID, parent, wikiBody, wikiTitle, publicView, privateView, portalView, comment, function (wiki) {
        MapWikiProperties(wiki);
        BuildWikiPage();
        top.Ts.System.logAction('Wiki - Wiki Saved');
    });

};

function DeleteWiki(wikiID) {
    top.Ts.Services.Wiki.DeleteWiki(wikiID, function () {
        _wikiID = null;
        BuildWikiPage();
        top.Ts.System.logAction('Wiki - Wiki Deleted');
    });

};


//Tiny_MCE Editor Setup
var initEditor = function (element, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var editorOptions = {
            plugins: "autoresize paste link code textcolor image moxiemanager table",
            toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox recordScreen insertUser insertWiki | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote | code | table",
            statusbar: false,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type],table[class=table|border:1]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
            body_class: "ui-widget ui-widget-content wiki-desc-editor",
            convert_urls: true,
            remove_script_host: false,
            relative_urls: false,
            template_external_list_url: "tinymce/jscripts/template_list.js",
            external_link_list_url: "tinymce/jscripts/link_list.js",
            external_image_list_url: "tinymce/jscripts/image_list.js",
            media_external_list_url: "tinymce/jscripts/media_list.js",
            menubar: false,
            moxiemanager_leftpanel: false,
            moxiemanager_fullscreen: false,
            moxiemanager_title: top.Ts.System.Organization.Name,
            moxiemanager_hidden_tools: (top.Ts.System.User.IsSystemAdmin == true) ? "" : "manage",
            paste_data_images: false,
            moxiemanager_image_settings: {
                moxiemanager_rootpath: "/" + top.Ts.System.Organization.OrganizationID + "/images/",
                extensions: 'gif,jpg,jpeg,png'
            },
            
            setup: function (ed) {
                ed.on('init', function (e) {
                    top.Ts.System.refreshUser(function () {
                        if (top.Ts.System.User.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.User.FontFamily));
                            ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.User.FontFamily);
                        }
                        else if (top.Ts.System.Organization.FontFamilyDescription != "Unassigned") {
                            ed.execCommand("FontName", false, GetTinyMCEFontName(top.Ts.System.Organization.FontFamily));
                            ed.getBody().style.fontFamily = GetTinyMCEFontName(top.Ts.System.Organization.FontFamily);
                        }

                        if (top.Ts.System.User.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.User.FontSizeDescription);
                            ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.User.FontSize + 1);
                        }
                        else if (top.Ts.System.Organization.FontSize != "0") {
                            ed.execCommand("FontSize", false, top.Ts.System.Organization.FontSize + 1);
                            ed.getBody().style.fontSize = GetTinyMCEFontSize(top.Ts.System.Organization.FontSize + 1);
                        }

                        if (_isCreatingNewWiki) {
                            $("#wiki-title-edit").focus();
                        }

                    });
                });

                ed.on('paste', function (ed, e) {
                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                });

                ed.addButton('insertTicket', {
                    title: 'Insert Ticket',
                    //image: '../images/nav/16/tickets.png',
                    icon: 'awesome fa fa-ticket',
                    onclick: function () {
                        top.Ts.System.logAction('Wiki - Ticket Inserted');

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

                ed.addButton('insertPasteImage', {
                    title: 'Insert Pasted Image',
                    //image: '../images/nav/16/imagepaste.png',
                    icon: 'awesome fa fa-paste',
                    onclick: function () {
                        if (BrowserDetect.browser == 'Safari' || BrowserDetect.browser == 'Explorer') {
                            alert("Sorry, this feature is not supported by " + BrowserDetect.browser);
                        }
                        else {
                            top.Ts.MainPage.pasteImage(null, function (result) {
                                ed.focus();
                                if (result != "") {
                                    var html = '<img src="' + top.Ts.System.AppDomain + '/dc/' + result + '"</a>&nbsp;<br/>';
                                    ed.selection.setContent(html);
                                    setTimeout(function () { ed.execCommand('mceAutoResize'); }, 1000);
                                    ed.execCommand('mceAutoResize');
                                    ed.focus();
                                }
                            });
                        }
                    }
                });

                ed.addButton('insertUser', {
                    title: 'Insert Userstamp',
                    icon: 'awesome fa fa-clock-o',
                    //image: '../images/icons/dropbox.png',
                    onclick: function () {
                        var html = Date(Date.UTC(Date.Now)) + ' ' + top.Ts.System.User.FirstName + ' ' + top.Ts.System.User.LastName + ' : ';
                        ed.selection.setContent(html);
                        ed.execCommand('mceAutoResize');
                        ed.focus();
                    }
                });

                ed.addButton('insertDropBox', {
                    title: 'Insert DropBox',
                    icon: 'awesome fa fa-dropbox',
                    //image: '../images/icons/dropbox.png',
                    onclick: function () {
                        var options = {
                            linkType: "preview",
                            success: function (files) {
                                ed.focus();
                                var html = '<a href=' + files[0].link + ' target="_blank">' + files[0].name + '</a>';
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Wiki - Dropbox Added');
                            },
                            cancel: function () {
                                alert('There was a problem inserting the dropbox file.');
                            }
                        };
                        Dropbox.choose(options);
                    }
                });
                ed.addButton('insertKb', {
                    title: 'Insert Knowledgebase',
                    //image: '../images/nav/16/knowledge.png',
                    icon: 'awesome fa fa-book',
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

                                var html = '<div>';

                                for (var i = 0; i < actions.length; i++) {
                                    html = html + '<div>' + actions[i].Description + '</div></br>';
                                }
                                html = html + '</div>';

                                ed.focus();
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Wiki - KB Inserted');
                                //needs to resize or go to end

                            }, function () {
                                alert('There was an error inserting your knowledgebase ticket.');
                            });
                        });
                    }
                });

                ed.addButton('insertWiki', {
                    title: 'Insert Wiki Article',
                    icon: 'awesome fa fa-file',
                    onclick: function () {
                        var bookmark = ed.selection.getBookmark(0);
                        top.Ts.MainPage.selectWiki(function (wikiID) {
                            top.Ts.Services.Wiki.GetWiki(wikiID, function (wiki) {
                                if (wiki === null) {
                                    alert('There was an error inserting your wiki article.');
                                    return;
                                }
                                var html = '<a href="' + top.Ts.System.AppDomain + '/wiki/JustArticle.aspx?Organizationid=' + wiki.OrganizationID + '&amp;ArticleID=' + wiki.ArticleID + '">' + wiki.ArticleName + '</a>';
                                ed.focus();
                                ed.selection.moveToBookmark(bookmark);
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Wiki - Wiki Inserted');
                            }, function () {
                                alert('There was an error inserting your wiki article.');
                            });
                        });
                    }
                });

                if (enableScreenR.toLowerCase() != 'false') {
                    ed.addButton('recordScreen', {
                        title: 'Record Screen',
                        //image: '../images/icons/Symbol_Record.png',
                        icon: 'awesome fa fa-circle',
                        onclick: function () {
                            //var x = '<div><iframe src="https://teamsupport.viewscreencasts.com/embed/e75084e0156749969d4c82ed05e35a9c" frameborder="0" width="650" height="400"><a href="http://google.com" target="_blank">Click here to view screen recording video</a></iframe>&nbsp;</div>';
                            top.Ts.MainPage.recordScreen(null, function (result) {
                                var link = '<a href="' + result.url + '" target="_blank">Click here to view screen recording video</a>';
                                var html = '<div><iframe src="https://teamsupport.viewscreencasts.com/embed/' + result.id + '" width="650" height="400" frameborder="0">' + link + '</iframe>&nbsp;</div>'
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Ticket - Screen Recorded');
                            });
                        }
                    });
                }
            }
        , oninit: init
        };
        $(element).tinymce(editorOptions);
    });
};

function GetTinyMCEFontSize(fontSize) {
    var result = '';
    switch (fontSize) {
        case 1:
            result = "8px";
            break;
        case 2:
            result = "10px";
            break;
        case 3:
            result = "12px";
            break;
        case 4:
            result = "14px";
            break;
        case 5:
            result = "18px";
            break;
        case 6:
            result = "24px";
            break;
        case 7:
            result = "36px";
            break;
    }
    return result;
}

function GetTinyMCEFontName(fontFamily) {
    var result = '';
    switch (fontFamily) {
        case 1:
            result = "'andale mono', times";
            break;
        case 2:
            result = "arial, helvetica, sans-serif";
            break;
        case 3:
            result = "'arial black', 'avant garde'";
            break;
        case 4:
            result = "'book antiqua', palatino";
            break;
        case 5:
            result = "'comic sans ms', sans-serif";
            break;
        case 6:
            result = "'courier new', courier";
            break;
        case 7:
            result = "georgia, palatino";
            break;
        case 8:
            result = "helvetica";
            break;
        case 9:
            result = "impact, chicago";
            break;
        case 10:
            result = "symbol";
            break;
        case 11:
            result = "tahoma, arial, helvetica, sans-serif";
            break;
        case 12:
            result = "terminal, monaco";
            break;
        case 13:
            result = "'times new roman', times";
            break;
        case 14:
            result = "'trebuchet ms', geneva";
            break;
        case 15:
            result = "verdana, geneva";
            break;
        case 16:
            result = "webdings";
            break;
        case 17:
            result = "wingdings, 'zapf dingbats'";
            break;
    }
    return result;
};