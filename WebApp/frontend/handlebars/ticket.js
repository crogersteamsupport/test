(function() {
  var template = Handlebars.template, templates = Handlebars.templates = Handlebars.templates || {};
templates['action'] = template({"1":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=container.lambda, alias2=container.escapeExpression;

  return "    <li data-id=\""
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.RefID : stack1), depth0))
    + "\" data-isWC=\""
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsWC : stack1), depth0))
    + "\" data-action=\""
    + alias2(((helper = (helper = helpers.ActionData || (depth0 != null ? depth0.ActionData : depth0)) != null ? helper : helpers.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : (container.nullContext || {}),{"name":"ActionData","hash":{},"data":data}) : helper)))
    + "\" class=\"ticket-action pinned\">\r\n";
},"3":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=container.lambda, alias2=container.escapeExpression;

  return "    <li data-id=\""
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.RefID : stack1), depth0))
    + "\" data-iswc=\""
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsWC : stack1), depth0))
    + "\" data-action=\""
    + alias2(((helper = (helper = helpers.ActionData || (depth0 != null ? depth0.ActionData : depth0)) != null ? helper : helpers.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : (container.nullContext || {}),{"name":"ActionData","hash":{},"data":data}) : helper)))
    + "\" class=\"ticket-action\">\r\n";
},"5":function(container,depth0,helpers,partials,data) {
    var stack1, helper, options, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=helpers.blockHelperMissing, buffer = 
  "        <label class=\"ticket-action-number\">"
    + container.escapeExpression(((helper = (helper = helpers.ActionNumber || (depth0 != null ? depth0.ActionNumber : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"ActionNumber","hash":{},"data":data}) : helper)))
    + "</label>\r\n        <div class=\"timeline-indicators\">\r\n"
    + ((stack1 = helpers["if"].call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsKnowledgeBase : stack1),{"name":"if","hash":{},"fn":container.program(6, data, 0),"inverse":container.program(8, data, 0),"data":data})) != null ? stack1 : "")
    + " "
    + ((stack1 = helpers["if"].call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsPinned : stack1),{"name":"if","hash":{},"fn":container.program(10, data, 0),"inverse":container.program(12, data, 0),"data":data})) != null ? stack1 : "")
    + "        </div>\r\n        <span class=\"pull-right action-options\">\r\n            <span class=\"fa fa-ellipsis-h action-options-icon text-muted\" aria-hidden=\"true\"></span>\r\n        <span class=\"action-option-items\">\r\n\r\n";
  stack1 = ((helper = (helper = helpers.CanEdit || (depth0 != null ? depth0.CanEdit : depth0)) != null ? helper : alias2),(options={"name":"CanEdit","hash":{},"fn":container.program(14, data, 0),"inverse":container.noop,"data":data}),(typeof helper === alias3 ? helper.call(alias1,options) : helper));
  if (!helpers.CanEdit) { stack1 = alias4.call(depth0,stack1,options)}
  if (stack1 != null) { buffer += stack1; }
  stack1 = ((helper = (helper = helpers.CanPin || (depth0 != null ? depth0.CanPin : depth0)) != null ? helper : alias2),(options={"name":"CanPin","hash":{},"fn":container.program(16, data, 0),"inverse":container.noop,"data":data}),(typeof helper === alias3 ? helper.call(alias1,options) : helper));
  if (!helpers.CanPin) { stack1 = alias4.call(depth0,stack1,options)}
  if (stack1 != null) { buffer += stack1; }
  stack1 = ((helper = (helper = helpers.CanKB || (depth0 != null ? depth0.CanKB : depth0)) != null ? helper : alias2),(options={"name":"CanKB","hash":{},"fn":container.program(21, data, 0),"inverse":container.noop,"data":data}),(typeof helper === alias3 ? helper.call(alias1,options) : helper));
  if (!helpers.CanKB) { stack1 = alias4.call(depth0,stack1,options)}
  if (stack1 != null) { buffer += stack1; }
  stack1 = ((helper = (helper = helpers.CanMakeVisible || (depth0 != null ? depth0.CanMakeVisible : depth0)) != null ? helper : alias2),(options={"name":"CanMakeVisible","hash":{},"fn":container.program(23, data, 0),"inverse":container.noop,"data":data}),(typeof helper === alias3 ? helper.call(alias1,options) : helper));
  if (!helpers.CanMakeVisible) { stack1 = alias4.call(depth0,stack1,options)}
  if (stack1 != null) { buffer += stack1; }
  return buffer + "</span>\r\n                </span>\r\n";
},"6":function(container,depth0,helpers,partials,data) {
    return "            <a class=\"glyphicon glyphicon-bookmark TeamSupportBlue ticket-action-kb\" aria-hidden=\"true\"></a>\r\n";
},"8":function(container,depth0,helpers,partials,data) {
    return "            <a class=\"glyphicon glyphicon-bookmark TeamSupportBlue ticket-action-kb hidden\" aria-hidden=\"true\"></a>\r\n            ";
},"10":function(container,depth0,helpers,partials,data) {
    return "\r\n            <a class=\"glyphicon glyphicon-pushpin TeamSupportBlue ticket-action-pinned\" aria-hidden=\"true\"></a>\r\n";
},"12":function(container,depth0,helpers,partials,data) {
    return "            <a class=\"glyphicon glyphicon-pushpin TeamSupportBlue ticket-action-pinned hidden\" aria-hidden=\"true\"></a>\r\n";
},"14":function(container,depth0,helpers,partials,data) {
    return "        <a href=\"#\" class=\"action-option-edit\"><i class=\"glyphicon glyphicon-pencil action-option-item\"></i>Edit</a>\r\n        <a href=\"#\" class=\"action-option-delete\"><i class=\"glyphicon glyphicon-trash action-option-item\"></i>Delete</a>\r\n";
},"16":function(container,depth0,helpers,partials,data) {
    var stack1;

  return ((stack1 = helpers["if"].call(depth0 != null ? depth0 : (container.nullContext || {}),((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsPinned : stack1),{"name":"if","hash":{},"fn":container.program(17, data, 0),"inverse":container.program(19, data, 0),"data":data})) != null ? stack1 : "");
},"17":function(container,depth0,helpers,partials,data) {
    return "        <a href=\"#\" class=\"action-option-pin\"><i class=\" glyphicon glyphicon-pushpin action-option-item\"></i>Unpin</a>\r\n";
},"19":function(container,depth0,helpers,partials,data) {
    return "        <a href=\"#\" class=\"action-option-pin\"><i class=\" glyphicon glyphicon-pushpin action-option-item\"></i>Pin</a>\r\n";
},"21":function(container,depth0,helpers,partials,data) {
    return "<a href=\"#\" class=\"action-option-kb\"><i class=\"glyphicon glyphicon-bookmark action-option-item\"></i>KB</a>\r\n";
},"23":function(container,depth0,helpers,partials,data) {
    return "<a href=\"#\" class=\"action-option-visible\"><i class=\"glyphicon glyphicon-eye-open action-option-item\"></i>Visible</a>\r\n";
},"25":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "                    <a target=\"_blank\" href=\"../../../dc/1/attachments/"
    + alias4(((helper = (helper = helpers.AttachmentID || (depth0 != null ? depth0.AttachmentID : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"AttachmentID","hash":{},"data":data}) : helper)))
    + "\"><i class=\"glyphicon glyphicon-paperclip\"></i>"
    + alias4(((helper = (helper = helpers.FileName || (depth0 != null ? depth0.FileName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"FileName","hash":{},"data":data}) : helper)))
    + "</a>\r\n                    <a data-attachmentid=\""
    + alias4(((helper = (helper = helpers.AttachmentID || (depth0 != null ? depth0.AttachmentID : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"AttachmentID","hash":{},"data":data}) : helper)))
    + "\" data-name=\""
    + alias4(((helper = (helper = helpers.FileName || (depth0 != null ? depth0.FileName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"FileName","hash":{},"data":data}) : helper)))
    + "\" class=\"remove-attachment\"><i class=\"glyphicon glyphicon-remove\" style=\"font-size:10px; margin-left:5px; cursor:pointer;\"></i></a> ";
},"27":function(container,depth0,helpers,partials,data) {
    var stack1, alias1=depth0 != null ? depth0 : (container.nullContext || {});

  return "                <div id=\"timeline-wc-responses\" class=\"timeline-wc-responses\">\r\n"
    + ((stack1 = helpers.each.call(alias1,(depth0 != null ? depth0.WaterCoolerReplies : depth0),{"name":"each","hash":{},"fn":container.program(28, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "                </div>\r\n                <div class=\"wc-options\">\r\n"
    + ((stack1 = helpers["if"].call(alias1,(depth0 != null ? depth0.DidLike : depth0),{"name":"if","hash":{},"fn":container.program(31, data, 0),"inverse":container.program(33, data, 0),"data":data})) != null ? stack1 : "")
    + "                    <div class=\"wc-option-replyarea\">\r\n                        <span class=\"wc-faux-textarea text-muted\">post comment...</span>\r\n                    </div>\r\n                    <div class=\"wc-textarea\" style=\"display:none;\">\r\n                        <textarea class=\"form-control wc-reply-text\" rows=\"3\"></textarea>\r\n                        <button type=\"button\" class=\"btn btn-primary wc-textarea-send\">Reply</button>\r\n                    </div>\r\n                </div>\r\n";
},"28":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=container.lambda, alias2=container.escapeExpression, alias3=depth0 != null ? depth0 : (container.nullContext || {}), alias4=helpers.helperMissing, alias5="function";

  return "                    <div data-messageid=\""
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.MessageID : stack1), depth0))
    + "\" class=\"timeline-wc-reply\">\r\n                        <img class=\"user-avatar pull-left wc-avatar\" src=\"../../../dc/"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.OrganizationID : stack1), depth0))
    + "/UserAvatar/"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.UserID : stack1), depth0))
    + "/120\">\r\n                        <div>\r\n                            <a class=\"TeamSupportBlue\">"
    + alias2(alias1(((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.UserName : stack1), depth0))
    + "</a><small class=\"text-muted\">  "
    + alias2((helpers.FormatDateTime || (depth0 && depth0.FormatDateTime) || alias4).call(alias3,((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.LastModified : stack1),{"name":"FormatDateTime","hash":{},"data":data}))
    + "  </small>\r\n\r\n                            <a data-liked=\""
    + alias2(((helper = (helper = helpers.DidLike || (depth0 != null ? depth0.DidLike : depth0)) != null ? helper : alias4),(typeof helper === alias5 ? helper.call(alias3,{"name":"DidLike","hash":{},"data":data}) : helper)))
    + "\" href=\"#\" class=\"wc-reply-like-link\">\r\n                                <span class=\"wc-reply-like-total\">"
    + alias2(((helper = (helper = helpers.WCLikes || (depth0 != null ? depth0.WCLikes : depth0)) != null ? helper : alias4),(typeof helper === alias5 ? helper.call(alias3,{"name":"WCLikes","hash":{},"data":data}) : helper)))
    + "</span>\r\n                                <span>\r\n"
    + ((stack1 = helpers.unless.call(alias3,(depth0 != null ? depth0.DidLike : depth0),{"name":"unless","hash":{},"fn":container.program(29, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "                            </span>\r\n                            </a>\r\n                            <p>"
    + ((stack1 = alias1(((stack1 = (depth0 != null ? depth0.WaterCoolerReplyProxy : depth0)) != null ? stack1.Message : stack1), depth0)) != null ? stack1 : "")
    + "</p>\r\n                        </div>\r\n                    </div>\r\n";
},"29":function(container,depth0,helpers,partials,data) {
    return "                                like\r\n";
},"31":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "                    <span data-liked=\""
    + alias4(((helper = (helper = helpers.DidLike || (depth0 != null ? depth0.DidLike : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"DidLike","hash":{},"data":data}) : helper)))
    + "\" class=\"wc-option wc-option-likes wc-liked\">+<span>"
    + alias4(((helper = (helper = helpers.Likes || (depth0 != null ? depth0.Likes : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Likes","hash":{},"data":data}) : helper)))
    + "</span></span>\r\n";
},"33":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "                    <span data-liked=\""
    + alias4(((helper = (helper = helpers.DidLike || (depth0 != null ? depth0.DidLike : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"DidLike","hash":{},"data":data}) : helper)))
    + "\" class=\"wc-option wc-option-likes\">+<span>"
    + alias4(((helper = (helper = helpers.Likes || (depth0 != null ? depth0.Likes : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Likes","hash":{},"data":data}) : helper)))
    + "</span></span>\r\n";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.lambda, alias5=container.escapeExpression;

  return ((stack1 = helpers["if"].call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsPinned : stack1),{"name":"if","hash":{},"fn":container.program(1, data, 0),"inverse":container.program(3, data, 0),"data":data})) != null ? stack1 : "")
    + "    <div class=\"timeline-panel\">\r\n"
    + ((stack1 = helpers.unless.call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsWC : stack1),{"name":"unless","hash":{},"fn":container.program(5, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "                <div class=\"ticket-badge\">\r\n                    "
    + ((stack1 = ((helper = (helper = helpers.TimeLineLabel || (depth0 != null ? depth0.TimeLineLabel : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"TimeLineLabel","hash":{},"data":data}) : helper))) != null ? stack1 : "")
    + " "
    + ((stack1 = ((helper = (helper = helpers.WaterCoolerRelationships || (depth0 != null ? depth0.WaterCoolerRelationships : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"WaterCoolerRelationships","hash":{},"data":data}) : helper))) != null ? stack1 : "")
    + "\r\n                </div>\r\n                <div class=\"timeline-header\">\r\n                    "
    + ((stack1 = ((helper = (helper = helpers.UserImageTag || (depth0 != null ? depth0.UserImageTag : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"UserImageTag","hash":{},"data":data}) : helper))) != null ? stack1 : "")
    + "\r\n                    <span class=\"pull-left\" style=\"line-height: 15px;\">\r\n<a data-userid=\""
    + alias5(alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.CreatorID : stack1), depth0))
    + "\" data-ticketid=\""
    + alias5(alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.TicketID : stack1), depth0))
    + "\" data-name=\""
    + alias5(alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.CreatorName : stack1), depth0))
    + "\" class=\"TeamSupportBlue UserAnchor\">"
    + alias5(alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.CreatorName : stack1), depth0))
    + "</a>\r\n<br>\r\n<small class=\"text-muted\">\r\nadded a\r\n"
    + alias5(alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.MessageType : stack1), depth0))
    + "\r\non "
    + alias5((helpers.FormatDateTime || (depth0 && depth0.FormatDateTime) || alias2).call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.DateCreated : stack1),{"name":"FormatDateTime","hash":{},"data":data}))
    + "\r\n</small>\r\n<br />\r\n<small class=\"text-muted\">"
    + alias5(((helper = (helper = helpers.TimeSpent || (depth0 != null ? depth0.TimeSpent : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"TimeSpent","hash":{},"data":data}) : helper)))
    + "</small>\r\n<br />\r\n</span> "
    + ((stack1 = ((helper = (helper = helpers.Applause || (depth0 != null ? depth0.Applause : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"Applause","hash":{},"data":data}) : helper))) != null ? stack1 : "")
    + "\r\n                    <!--<span class=\"pull-right p50r\">\r\n<br />\r\n<small class=\"text-muted\">"
    + alias5(((helper = (helper = helpers.TimeSpent || (depth0 != null ? depth0.TimeSpent : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"TimeSpent","hash":{},"data":data}) : helper)))
    + "</small>\r\n</span>-->\r\n                </div>\r\n                <div class=\"timeline-body\">\r\n                    "
    + ((stack1 = alias4(((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.Message : stack1), depth0)) != null ? stack1 : "")
    + "\r\n                </div>\r\n                <div class=\"timeline-attachments\">\r\n"
    + ((stack1 = helpers.each.call(alias1,(depth0 != null ? depth0.Attachments : depth0),{"name":"each","hash":{},"fn":container.program(25, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "\r\n                </div>\r\n"
    + ((stack1 = helpers["if"].call(alias1,((stack1 = (depth0 != null ? depth0.item : depth0)) != null ? stack1.IsWC : stack1),{"name":"if","hash":{},"fn":container.program(27, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "            </div>\r\n        </li>\r\n";
},"useData":true});
templates['new-action-template'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "<div id=\"ticket-title-panel\" class=\"panel panel-default\" style=\"position: relative; z-index: 1; display:table; margin-left:auto; margin-right:auto; margin-bottom:10px;\">\r\n\r\n    <p id=\"ticket-title-label\" class=\"text-center\" style=\"margin-bottom: 0px; padding: 5px; font-weight: 500;\"></p>\r\n\r\n</div>\r\n\r\n<div id=\"ticket-title-input-panel\" class=\"panel panel-default\" style=\"position:relative; z-index: 1; margin-bottom:10px; display:none; \">\r\n\r\n    <div class=\"row\">\r\n\r\n        <div class=\"col-sm-12\">\r\n\r\n            <div class=\"input-group\">\r\n\r\n                <input id=\"ticket-title-input\" type=\"text\" class=\"form-control\" placeholder=\"Enter name for ticket...\">\r\n\r\n                <span class=\"input-group-btn\">\r\n\r\n                        <button id=\"ticket-title-save\" class=\"btn btn-primary\" type=\"button\">Save</button>\r\n\r\n                        <button id=\"ticket-title-cancel\" class=\"btn btn-default\" type=\"button\">Cancel</button>\r\n\r\n                    </span>\r\n\r\n            </div>\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n</div>\r\n\r\n<li class=\"action-new\">\r\n\r\n    <img id=\"new-action-avatar\" class=\"user-avatar pull-left\" style=\"height:40px;\" src=\"../../../dc/"
    + alias4(((helper = (helper = helpers.OrganizationID || (depth0 != null ? depth0.OrganizationID : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"OrganizationID","hash":{},"data":data}) : helper)))
    + "/UserAvatar/"
    + alias4(((helper = (helper = helpers.UserID || (depth0 != null ? depth0.UserID : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"UserID","hash":{},"data":data}) : helper)))
    + "/120\" />\r\n\r\n    <div class=\"panel panel-default\">\r\n\r\n        <p class=\"text-muted\">add a <a id=\"action-add-public\" href=\"\"> public action </a> / <a id=\"action-add-private\" href=\" \"> private action  /</a> <a id=\"action-add-wc\" href=\" \"> watercooler</a>.</p>\r\n\r\n    </div>\r\n\r\n    <div class=\"action-new-area\" style=\"display:none;\">\r\n\r\n        <div class=\"divScreenRecorderMessages\" style=\"display: none\">\r\n\r\n            <p class=\"pAllowPluginsToRunInstructions\">Please verify java is supported and allowed to run in your browser.</p>\r\n\r\n            <p>Finally, the first run of the screen recorder takes about 40 seconds to load on a 2 Mbps internet connection.</p>\r\n\r\n        </div>\r\n\r\n        <div id=\"public-badge\" class=\"ticket-badge\">\r\n\r\n            <div class=\"bgcolor-green\"><span class=\"bgcolor-green\">&nbsp;</span><a href=\"#\" class=\"new-action-option-visible\">Public</a></div>\r\n\r\n        </div>\r\n\r\n        <div id=\"private-badge\" class=\"ticket-badge\">\r\n\r\n            <div class=\"bgcolor-orange\"><span class=\"bgcolor-orange\">&nbsp;</span><a href=\"#\" class=\"new-action-option-visible\">Private</a></div>\r\n\r\n        </div>\r\n\r\n        <!--<textarea id=\"action-new-editor\"></textarea>-->\r\n        <div id=\"action-new-editor\"></div>\r\n\r\n        <div id=\"screenRecordDiv\">\r\n\r\n            <div id=\"ourPubTest\" style=\"position: absolute !important; z-index: -99;\"></div>\r\n\r\n            <div id=\"ourPubTest2\" style=\"position: absolute; left: -9999px;\"></div>\r\n\r\n        </div>\r\n\r\n        <div style=\"text-align:center;display:none;\" id=\"recordScreenContainer\" class=\"action-new-toolbar\">\r\n\r\n            <div id=\"statusTextScreen\"></div>\r\n\r\n            <div id=\"tokScreenCountdown\">0:00</div>\r\n\r\n            <button id=\"rcdtokScreen\" class=\"btn btn-primary\" title=\"Record Screen\"><i class=\"fa fa-video-camera\"></i></button>\r\n\r\n            <button id=\"muteTokScreen\" class=\"btn btn-primary\" title=\"Mute Audio\"><i class=\"fa fa-microphone\"></i></button>\r\n\r\n            <button id=\"unmuteTokScreen\" class=\"btn btn-danger\" title=\"Record Audio\"><i class=\"fa fa-microphone-slash\"></i></button>\r\n\r\n            <button id=\"stoptokScreen\" class=\"btn btn-danger\" title=\"Stop Screen Recording\"><i class=\"fa fa-stop\"></i></button>\r\n\r\n            <button id=\"canceltokScreen\" title=\"Cancel Screen Recording\" class=\"btn btn-danger\"><i class=\"fa fa-eject\"></i></button>\r\n\r\n        </div>\r\n\r\n        <div style=\"text-align:center;display:none;\" id=\"recordVideoContainer\" class=\"action-new-toolbar\">\r\n\r\n            <div id=\"statusText\"></div>\r\n\r\n            <div id=\"publisher\"></div>\r\n\r\n            <button id=\"rcdtok\" class=\"btn btn-primary\" title=\"Record\"><i class=\"fa fa-video-camera\"></i></button>\r\n\r\n            <button id=\"stoptok\" class=\"btn btn-danger\" title=\"Stop\"><i class=\"fa fa-stop\"></i></button>\r\n\r\n            <button id=\"inserttok\" class=\"btn btn-success\" title=\"Insert Video\"><i class=\"fa fa-cloud-upload\"></i></button>\r\n\r\n            <button id=\"canceltok\" title=\"Cancel\" class=\"btn btn-danger\"><i class=\"fa fa-eject\"></i></button>\r\n\r\n        </div>\r\n\r\n        <div class=\"row action-new-toolbar\">\r\n            <div>\r\n\r\n                <div class=\"ticket-action-form-timespent pull-left\">\r\n\r\n                    <div class=\"form-group\" style=\"width:210px;\">\r\n\r\n                        <label for=\"action-new-date-started\">Date Started:</label>\r\n\r\n                        <div class='input-group date'>\r\n\r\n                            <input id=\"action-new-date-started\" type='text' class=\"form-control\" />\r\n\r\n                            <span id=\"action-new-timer\" class=\"input-group-addon\" data-hasstarted=\"false\">\r\n\r\n                            <span class=\"fa fa-clock-o color-red\"></span>\r\n\r\n                            </span>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n\r\n                        <label for=\"action-new-hours\">Hours:</label>\r\n\r\n                        <div class=\"input-group spinner\">\r\n\r\n                            <input id=\"action-new-hours\" type=\"text\" class=\"form-control\" value=\"0\">\r\n\r\n                            <div class=\"input-group-btn-vertical\">\r\n\r\n                                <button class=\"btn btn-default\"><i class=\"fa fa-caret-up\"></i></button>\r\n\r\n                                <button class=\"btn btn-default\"><i class=\"fa fa-caret-down\"></i></button>\r\n\r\n                            </div>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n\r\n                        <label for=\"action-new-minutes\">Minutes:</label>\r\n\r\n                        <div class=\"input-group spinner\">\r\n\r\n                            <input id=\"action-new-minutes\" type=\"text\" class=\"form-control\" value=\"0\">\r\n\r\n                            <div class=\"input-group-btn-vertical\">\r\n\r\n                                <button class=\"btn btn-default\"><i class=\"fa fa-caret-up\"></i></button>\r\n\r\n                                <button class=\"btn btn-default\"><i class=\"fa fa-caret-down\"></i></button>\r\n\r\n                            </div>\r\n\r\n                        </div>\r\n\r\n                    </div>\r\n\r\n\r\n                    <div class=\"form-group\">\r\n\r\n                        <label for=\"action-new-type\">Type:</label>\r\n\r\n                        <select id=\"action-new-type\" class=\"form-control\"></select>\r\n\r\n                    </div>\r\n\r\n                    <div class=\"form-group\">\r\n\r\n                        <label for=\"action-new-KB\">KnowledgeBase:</label>\r\n\r\n                        <input id=\"action-new-KB\" type=\"checkbox\">\r\n\r\n                    </div>\r\n\r\n                </div>\r\n\r\n            </div>\r\n\r\n        </div>\r\n\r\n        <div class=\"row action-new-toolbar\">\r\n\r\n            <div class=\"col-sm-6\">\r\n\r\n                <form id=\"action-file-upload\" class=\"file-upload\" action=\"Upload/Actions/555\" method=\"POST\" enctype=\"multipart/form-data\" title=\"You can drag and drop your attachments here.\">\r\n\r\n                    <input type=\"file\" name=\"file[]\">\r\n\r\n                    <a class=\"ts-link\" href=\"#\" title=\"You can drag and drop your attachments here.\">Attach File >></a>\r\n\r\n                </form>\r\n\r\n                <ul class=\"upload-queue\"></ul>\r\n\r\n            </div>\r\n\r\n            <div class=\"col-sm-12\">\r\n\r\n                <div id=\"action-save-alert\" class=\"alert alert-danger\" role=\"alert\" style=\"display:none;\">Please enter the time you worked on this action.</div>\r\n\r\n                <button id=\"action-new-cancel\" type=\"button\" class=\"btn btn-default pull-right\">Cancel</button>\r\n\r\n                <div class=\"btn-group dropup action-save-group pull-right\">\r\n\r\n                    <button id=\"action-new-save\" type=\"button\" class=\"btn btn-primary\">Create</button>\r\n\r\n                    <button id=\"action-new-save-element\" type=\"button\" class=\"btn btn-primary dropdown-toggle\" data-toggle=\"dropdown\" aria-expanded=\"false\"><span class=\"caret\"></span> <span class=\"sr-only\">Toggle Dropdown</span></button>\r\n\r\n                    <ul id=\"action-new-saveoptions\" class=\"dropdown-menu\" role=\"menu\"></ul>\r\n\r\n                </div>\r\n\r\n            </div>\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n</li>\r\n\r\n<li class=\"watercooler-new-area\" style=\"display:none; padding-bottom: 10px;\">\r\n    <div class=\"ticket-badge\">\r\n        <div class=\"bgcolor-blue\"><span class=\"bgcolor-blue\">&nbsp;</span>\r\n            <label>WC</label>\r\n        </div>\r\n    </div>\r\n    <div class=\"container\" style=\"padding-top: 25px;\">\r\n        <div class=\"form-group\">\r\n            <textarea id=\"inputDescription\" rows=\"5\" class=\"form-control\" placeholder=\"Share To Watercooler\"></textarea>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            <span data-toggle=\"tooltip\" data-original-title=\"Associate Ticket\" class=\"addticket\" title=\"Associate Ticket\"></span>\r\n            <span data-toggle=\"tooltip\" data-original-title=\"Associate User\" class=\"adduser\" title=\"Associate User\"></span>\r\n            <span data-toggle=\"tooltip\" data-original-title=\"Associate Company\" class=\"addcustomer\" title=\"Associate Company\"></span>\r\n            <span data-toggle=\"tooltip\" data-original-title=\"Associate Group\" class=\"addgroup\" title=\"Associate Group\"></span>\r\n            <span data-toggle=\"tooltip\" data-original-title=\"Associate Product\" class=\"addproduct\" title=\"Associate Product\"></span>\r\n            <div style=\"left: 7px;\" class=\"arrow-up\"></div>\r\n            <input type=\"text\" class=\"form-control\" id=\"associationSearch\" placeholder=\"Search...\" style=\"width:250px;\" />\r\n        </div>\r\n\r\n        <div class=\"form-group\" style=\"margin-bottom: 0px;\">\r\n            <div class=\"row\">\r\n                <div class=\"col-sm-6\">\r\n                    <div id=\"associationQueue\">\r\n                        <div class=\"wc-attachments\"></div>\r\n                        <div class=\"ticket-queue\"></div>\r\n                        <div class=\"user-queue\"></div>\r\n                        <div class=\"group-queue\"></div>\r\n                        <div class=\"customer-queue\"></div>\r\n                        <div class=\"product-queue\"></div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <div class=\"form-group\">\r\n            <div class=\"row\">\r\n                <div class=\"col-sm-6\">\r\n                    <form id=\"wc-file-upload\" class=\"file-upload\" action=\"Upload/Actions/555\" method=\"POST\" enctype=\"multipart/form-data\" title=\"You can drag and drop your attachments here.\">\r\n                        <input type=\"file\" name=\"file[]\">\r\n                        <a class=\"ts-link\" href=\"#\" title=\"You can drag and drop your attachments here.\">Attach File >></a>\r\n                    </form>\r\n                </div>\r\n                <div class=\"col-sm-6\">\r\n                    <span class=\"pull-right\">\r\n                        <button id=\"newcomment\" class=\"btn btn-primary\">Save</button>\r\n                        <button id=\"newcommentcancel\" type=\"button\" class=\"btn btn-default\">Cancel</button>\r\n                    </span>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</li>\r\n\r\n<div class=\"hidden action-placeholder\"></div>\r\n";
},"useData":true});
templates['task-record'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1, helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "<div class=\"checkbox\">\r\n    <input id=\"task-"
    + alias4(((helper = (helper = helpers.id || (depth0 != null ? depth0.id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data}) : helper)))
    + "\" class=\"change-task-status\" type=\"checkbox\" data-taskid=\""
    + alias4(((helper = (helper = helpers.id || (depth0 != null ? depth0.id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data}) : helper)))
    + "\" "
    + alias4((helpers.taskComplete || (depth0 && depth0.taskComplete) || alias2).call(alias1,(depth0 != null ? depth0.IsComplete : depth0),{"name":"taskComplete","hash":{},"data":data}))
    + "/>\r\n    <label for=\"task-"
    + alias4(((helper = (helper = helpers.id || (depth0 != null ? depth0.id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data}) : helper)))
    + "\"><a href=\"#\" class=\"tasklink\" data-taskid=\""
    + alias4(((helper = (helper = helpers.id || (depth0 != null ? depth0.id : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"id","hash":{},"data":data}) : helper)))
    + "\">"
    + ((stack1 = ((helper = (helper = helpers.value || (depth0 != null ? depth0.value : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"value","hash":{},"data":data}) : helper))) != null ? stack1 : "")
    + "</a></label>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-assignedto'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-assigned\" class=\"col-sm-4 control-label select-label\">Assigned</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-assigned\" class=\"form-control hidden-select muted-placeholder\" placeholder=\"Assign User\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-associatedtickets'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n    <!--<label for=\"ticket-AssociatedTickets\" class=\"TagCategoryLabel\">Associated Tickets:&nbsp;</label>-->\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-AssociatedTickets-Input\" class=\"tagInput hidden-select\" type=\"text\" placeholder=\"Add Related Ticket\">\r\n        <span id=\"ticket-AssociatedTickets\" style=\"z-index:100;\"></span>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-attachements'] = template({"1":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=helpers.helperMissing, alias3="function", alias4=container.escapeExpression;

  return "        <a target=\"_blank\" href=\"../../../dc/1/attachments/"
    + alias4(((helper = (helper = helpers.AttachmentID || (depth0 != null ? depth0.AttachmentID : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"AttachmentID","hash":{},"data":data}) : helper)))
    + "\"><i class=\"glyphicon glyphicon-paperclip\"></i>"
    + alias4(((helper = (helper = helpers.FileName || (depth0 != null ? depth0.FileName : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"FileName","hash":{},"data":data}) : helper)))
    + "</a>\r\n        <br /> ";
},"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    var stack1;

  return "<div class=\"form-group ticket-group\">\r\n    <div class=\"tagContainer\">\r\n        <label class=\"col-sm-4 control-label select-label attachment-label\">Attachments</label>\r\n"
    + ((stack1 = helpers.each.call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? depth0.Attachments : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0),"inverse":container.noop,"data":data})) != null ? stack1 : "")
    + "\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-community'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-Community\" class=\"col-sm-4 control-label select-label\">Community</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-Community\" class=\"form-control hidden-select muted-placeholder\" placeholder=\"Assign Community\"></select>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"form-horizontal\" id=\"ticket-CommunityInfo-RO\" style=\"display:none;\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-Community-RO\" class=\"col-sm-4 control-label\">Community</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-Community-RO\" class=\"control-label\" style=\"color: #333333; text-align:left;\"></label>\r\n        </div>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-customers'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-Customers-Input\" class=\"tagInput hidden-select\" type=\"text\" placeholder=\"Add/Create Customer\">\r\n        <span id=\"ticket-Customer\" style=\"z-index:100;\">\r\n\r\n        </span>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-customfields'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div id=\"ticket-group-custom-fields\">\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-daysopened'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-DaysOpened\" class=\"col-sm-4 control-label\"></label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-DaysOpened\" class=\"control-label\"></label>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-duedate'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-DueDate\" class=\"col-sm-4 control-label\">Due Date</label>\r\n        <div id=\"ticket-duedate-container\" class=\"col-sm-8 ticket-input-container\" style=\"padding-top: 6px;\">\r\n            <!--<input id=\"ticket-DueDate\" class=\"form-control\" type=\"text\" />-->\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-group'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-group\" class=\"col-sm-4 control-label select-label\">Group</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-group\" class=\"form-control hidden-select muted-placeholder\" placeholder=\"Assign Group\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-hr'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<hr />\r\n";
},"useData":true});
templates['ticket-group-inventory'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n    <!--<label for=\"ticket-UserQueue\" class=\"TagCategoryLabel\">Inventory:&nbsp;</label>-->\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-Inventory-Input\" class=\"tagInput hidden-select\" type=\"text\" placeholder=\"Add Inventory\">\r\n        <span id=\"ticket-Inventory\" style=\"z-index:100;\"></span>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-jira'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div id=\"ticket-jirafields\" class=\"form-horizontal\">\r\n    <div id=\"issueKey\" class=\"form-group form-group-sm\">\r\n        <label for=\"issueKeyValue\" class=\"col-sm-4 control-label\">Jira Issue Key:</label>\r\n        <div id=\"ticket-jirakey-container\" class=\"col-sm-8 ticket-input-container\" style=\"padding-top: 6px;\">\r\n            <span id=\"issueKeyValue\"></span>\r\n            <i id=\"jiraUnlink\" class=\"glyphicon glyphicon-remove\" style=\"font-size:10px; cursor:pointer; position:relative; left:5px;\"></i>\r\n        </div>\r\n    </div>\r\n\r\n    <div id=\"enterIssueKey\" style=\"display:none;\">\r\n        <div class=\"ticket-jira-issue\">\r\n            <label>Existing Jira Issue Key:</label>\r\n            <input id=\"issueKeyInput\" type=\"text\" class=\"form-control\" style=\"margin-bottom:5px;\" />\r\n        </div>\r\n        <div style=\"text-align:right\">\r\n            <button id=\"saveIssueKeyButton\" class=\"btn btn-sm btn-default\">Save</button>\r\n            <button id=\"cancelIssueyKeyButton\" class=\"btn btn-sm btn-default\">Cancel</button>\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"ts-jira-buttons-container\">\r\n        <button id=\"newJiraIssue\" class=\"btn btn-default btn-block\">New Jira Issue</button>\r\n        <button id=\"existingJiraIssue\" class=\"btn btn-default btn-block\">Existing Jira Issue</button>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-knowledgebase'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-isKB\" class=\"col-sm-4 control-label\">KB</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <div class=\"checkbox\" style=\"padding-top:5px;\">\r\n                <input id=\"ticket-isKB\" type=\"checkbox\" style=\"margin-left: -15px;\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\" id=\"ticket-group-KBCat\">\r\n        <label for=\"ticket-KB-Category\" class=\"col-sm-4 control-label select-label\">Category</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-KB-Category\" class=\"form-control hidden-select muted-placeholder\" placeholder=\"Assign Category\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"form-horizontal\" id=\"ticket-KBVisible-RO\" style=\"display:none;\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-DueDate\" class=\"col-sm-4 control-label\">KB</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-isKB-RO\" class=\"control-label\"></label>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"form-horizontal\" id=\"ticket-KBCat-RO\" style=\"display:none;\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-KB-Category-RO\" class=\"col-sm-4 control-label\">KB</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-KB-Category-RO\" class=\"control-label\"></label>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-product'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-Product\" class=\"col-sm-4 control-label select-label\">Product</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-Product\" class=\"hidden-select muted-placeholder\" placeholder=\"\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-reminders'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\" id=\"ticket-group-Reminders\">\r\n    <div class=\"tagContainer\">\r\n        <a class=\"btn btn-link\" style=\"color: #428bca; padding-left: 3px; padding-bottom: 0px; font-size: 12px; margin-bottom:10px; text-align: left; width:100%;\" data-toggle=\"modal\" data-target=\"#RemindersModal\">Add Reminder</a>\r\n        <span id=\"ticket-reminder-span\"></span>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-reported'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-Product-Reported-Version\" class=\"col-sm-4 control-label select-label\">Reported <abbr title=\"Version\">Ver.</abbr></label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-Versions\" class=\"hidden-select\"></select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-resolved'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-Product-Resolved-Version\" class=\"col-sm-4 control-label select-label\">Resolved <abbr title=\"Version\">Ver.</abbr></label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-Resolved\" class=\"hidden-select\"></select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-severity'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-severity\" class=\"col-sm-4 control-label select-label\">Severity</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-severity\" class=\"form-control ticket-select hidden-select muted-placeholder\" placeholder=\"Assign Severity\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-slastatus'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-SLAStatus\" class=\"col-sm-4 control-label select-label\">SLA Status</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-SLAStatus\" class=\"ticket-sla-status pull-left control-label SLAAnchor\">\r\n                <i class=\"fa fa-bomb\"></i>\r\n            </label>\r\n            <label id=\"ticket-SLANote\" class=\"ticket-sla-status-note pull-left control-label\" style=\"line-height: 20px; padding-top: 3px; padding-left: 0px; color: #333333;\"></label>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-status'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label id=\"ticket-status-label\" for=\"ticket-status\" class=\"col-sm-4 control-label select-label\">Status</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-status\" class=\"form-control hidden-select muted-placeholder\" placeholder=\"Change Status\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-subscribedusers'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-SubscribedUsers-Input\" class=\"tagInput hidden-select\" type=\"text\" placeholder=\"Subscribe User\">\r\n        <span id=\"ticket-SubscribedUsers\" style=\"z-index:100;\"></span>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-tags'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n    <!--<label for=\"ticket-tags\" class=\"TagCategoryLabel\">Tags:&nbsp;</label>-->\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-tag-Input\" class=\"tagInput ui-autocomplete-input\" type=\"text\" placeholder=\"Add Tag\" style=\"font-size:12px; margin-bottom: 10px; padding-left: 5px; padding-top: 8px;\" autocomplete=\"off\"> <span id=\"ticket-tags\"></span>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-tasks'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\" id=\"ticket-group-Tasks\">\r\n    <div class=\"taskContainer\">\r\n        <a class=\"btn btn-link new-task\" style=\"color: #428bca; padding-left: 3px; padding-bottom: 0px; font-size: 12px; margin-bottom:10px; text-align: left; width:100%;\">Add Task</a>\r\n        <span id=\"ticket-task-span\"></span>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-tfs'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div id=\"ticket-tfsfields\" class=\"form-horizontal\">\r\n    <div id=\"workItemTitle\" class=\"form-group form-group-sm\">\r\n        <label for=\"workItemTitleValue\" class=\"col-sm-4 control-label\">Work Item Title:</label>\r\n        <div id=\"ticket-workItemTitle-container\" class=\"col-sm-8 ticket-input-container\" style=\"padding-top: 6px;\">\r\n            <span id=\"workItemTitleValue\"></span>\r\n            <i id=\"tfsUnlink\" class=\"glyphicon glyphicon-remove\" style=\"font-size:10px; cursor:pointer; position:relative; left:5px;\"></i>\r\n        </div>\r\n    </div>\r\n\r\n    <div id=\"enterWorkItemTitle\" style=\"display:none;\">\r\n        <div class=\"ticket-tfs-workItem\">\r\n            <label>Existing TFS Work Item ID:</label>\r\n            <input id=\"workItemIDInput\" type=\"text\" class=\"form-control\" style=\"margin-bottom:5px;\" />\r\n        </div>\r\n        <div style=\"text-align:right\">\r\n            <button id=\"saveWorkItemTitleButton\" class=\"btn btn-sm btn-default\">Save</button>\r\n            <button id=\"cancelWorkItemTitleButton\" class=\"btn btn-sm btn-default\">Cancel</button>\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"ts-tfs-buttons-container\">\r\n        <button id=\"newTFSWorkItem\" class=\"btn btn-default btn-block\">New TFS Work Item</button>\r\n        <button id=\"existingTFSWorkItem\" class=\"btn btn-default btn-block\">Existing TFS Work Item</button>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-totaltimespent'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-TimeSpent\" class=\"col-sm-4 control-label\">Time</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <label id=\"ticket-TimeSpent\" class=\"control-label\" style=\"color: #333333;\"></label>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-type'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-type\" class=\"col-sm-4 control-label select-label\">Type</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <select id=\"ticket-type\" class=\"form-control ticket-select hidden-select muted-placeholder\" placeholder=\"Change Type\">\r\n                <option>Unassigned</option>\r\n            </select>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-userqueue'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-group ticket-group\">\r\n    <!--<label for=\"ticket-UserQueue\" class=\"TagCategoryLabel\">User Queues:&nbsp;</label>-->\r\n    <div class=\"tagContainer\">\r\n        <input id=\"ticket-UserQueue-Input\" class=\"tagInput hidden-select\" type=\"text\" placeholder=\"Add to Queue\">\r\n        <span id=\"ticket-UserQueue\" style=\"z-index:100;\"></span>\r\n\r\n    </div>\r\n</div>\r\n";
},"useData":true});
templates['ticket-group-visibletocustomers'] = template({"compiler":[7,">= 4.0.0"],"main":function(container,depth0,helpers,partials,data) {
    return "<div class=\"form-horizontal\">\r\n    <div class=\"form-group form-group-sm\">\r\n        <label for=\"ticket-visible\" class=\"col-sm-4 control-label\">Visible</label>\r\n        <div class=\"col-sm-8 ticket-input-container\">\r\n            <div class=\"checkbox\" style=\"padding-top:5px;\">\r\n                <input id=\"ticket-visible\" type=\"checkbox\" style=\"margin-left: -15px;\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n";
},"useData":true});
})();