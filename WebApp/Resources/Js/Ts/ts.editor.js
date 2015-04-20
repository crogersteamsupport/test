var initEditor = function (element, shouldResize, init) {
    top.Ts.Settings.System.read('EnableScreenR', 'True', function (enableScreenR) {
        var resizePluginCode = '';
        if (shouldResize)
        {
            resizePluginCode = 'autoresize';
        }
        var editorOptions = {
            plugins: "paste link code textcolor image moxiemanager table " + resizePluginCode,
            toolbar1: "insertPasteImage insertKb insertTicket image insertimage insertDropBox recordScreen insertUser | link unlink | undo redo removeformat | cut copy paste pastetext | outdent indent | bullist numlist",
            toolbar2: "alignleft aligncenter alignright alignjustify | forecolor backcolor | fontselect fontsizeselect | bold italic underline strikethrough blockquote | code | table",
            statusbar: true,
            gecko_spellcheck: true,
            extended_valid_elements: "a[accesskey|charset|class|coords|dir<ltr?rtl|href|hreflang|id|lang|name|onblur|onclick|ondblclick|onfocus|onkeydown|onkeypress|onkeyup|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|rel|rev|shape<circle?default?poly?rect|style|tabindex|title|target|type],script[charset|defer|language|src|type],table[class=table|border:1]",
            content_css: "../Css/jquery-ui-latest.custom.css,../Css/editor.css",
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
            paste_data_images: true,
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
                        top.Ts.System.logAction('Ticket - Ticket Inserted');

                        top.Ts.MainPage.selectTicket(null, function (ticketID) {
                            top.Ts.Services.Tickets.GetTicket(ticketID, function (ticket) {
                                ed.focus();
                                top.Ts.Services.Tickets.AddRelated(_ticketID, ticketID, null, function (tickets) {
                                    appendRelated(tickets);
                                    //window.top.ticketSocket.server.ticketUpdate(_ticketNumber, "addrelationship", userFullName);
                                }, function (error) {
                                    //container.remove();
                                    alert(error.get_message());
                                });
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
                                var html = '<a href=' + files[0].link + '>' + files[0].name + '</a>';
                                ed.selection.setContent(html);
                                ed.execCommand('mceAutoResize');
                                ed.focus();
                                top.Ts.System.logAction('Ticket - Dropbox Added');
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
                                top.Ts.System.logAction('Ticket - KB Inserted');
                                //needs to resize or go to end

                            }, function () {
                                alert('There was an error inserting your knowledgebase ticket.');
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
}


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
}