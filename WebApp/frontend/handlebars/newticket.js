!function(){var n=Handlebars.template,r=Handlebars.templates=Handlebars.templates||{};r["ticket-group-assignedto"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox" style="margin-top:20px;">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-assigned" class="select-label">Assigned</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-assigned" class="hidden-select muted-placeholder" placeholder="Assign User">\r\n\r\n            <option>Unassigned</option>\r\n\r\n        </select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-associatedtickets"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-AssociatedTickets-Input" class="tagInput hidden-select" type="text" placeholder="Add Ticket">\r\n\r\n    <span id="ticket-AssociatedTickets" style="z-index:100;"></span>\r\n\r\n  </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-community"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-Community" class="control-label select-label">Community</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-Community" class="form-control  hidden-select  muted-placeholder" placeholder="Change Community"><option>unassigned</option></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-customers"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-Customers-Input" class="tagInput hidden-select" type="text" placeholder="Add Customer">\r\n\r\n        <span id="ticket-Customer" style="z-index:100;"></span>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-customfields"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return""},useData:!0}),r["ticket-group-duedate"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-DueDate" class="control-label">Due Date</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <div id="ticket-duedate-container" class="ticket-input-container" style="padding-top:6px;">\r\n\r\n            \x3c!--<input id="ticket-DueDate" class="form-control" type="text" />--\x3e\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-group"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-group" class="control-label select-label">Group</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-group" class="form-control hidden-select muted-placeholder" placeholder="Assign Group"><option>Unassigned</option></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-hr"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return"<hr>\r\n"},useData:!0}),r["ticket-group-inventory"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-Inventory-Input" class="tagInput hidden-select" type="text" placeholder="Add Inventory">\r\n\r\n        <span id="ticket-Inventory" style="z-index:100;"></span>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-knowledgebase"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-isKB" class="control-label">KB</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <div class="checkbox" style="padding-top:5px;">\r\n\r\n            <input id="ticket-isKB" type="checkbox">\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n</div>\r\n\r\n\r\n<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label" id="ticket-group-KBCat">\r\n\r\n        <label for="ticket-KB-Category" class="control-label select-label">Category</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-KB-Category" class="form-control  hidden-select  muted-placeholder">\r\n\r\n            <option>Unassigned</option>\r\n\r\n        </select>\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-product"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-Product" class="control-label select-label">Product</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-Product" class=" hidden-select  muted-placeholder" placeholder="Choose Product">\r\n\r\n            <option>unassigned</option>\r\n\r\n        </select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-reminders"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group" id="ticket-group-Reminders">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <a class="btn btn-link" style="color: #428bca; padding-left: 3px; padding-bottom: 0px; font-size: 12px; margin-bottom:10px; text-align: left; width:100%;" data-toggle="modal" data-target="#RemindersModal">Add Reminder</a>\r\n\r\n        <span id="ticket-reminder-span"></span><br />\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-reported"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-Product-Reported-Version" class="control-label select-label">Reported <abbr title="Version">Ver.</abbr></label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-Versions" class=" hidden-select  muted-placeholder"></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-resolved"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-Product-Resolved-Version" class="control-label select-label">Resolved <abbr title="Version">Ver.</abbr></label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-Resolved" class=" hidden-select  muted-placeholder"></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-severity"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-severity" class="control-label select-label">Severity</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-severity" class="form-control ticket-select  hidden-select  muted-placeholder" placeholder="Add Severity"></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-status"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label id="ticket-status-label" for="ticket-status" class="control-label select-label" placeholder="Change Status">Status</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-status" class="form-control hidden-select muted-placeholder"></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-subscribedusers"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-SubscribedUsers-Input" class="tagInput hidden-select" type="text" placeholder="Subscribe User">\r\n\r\n        <span id="ticket-SubscribedUsers" style="z-index:100;"></span>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-tags"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-tag-Input" class="tagInput ui-autocomplete-input" type="text" placeholder="Add Tag" style="font-size:12px; margin-bottom: 10px; padding-left: 5px;padding-top: 8px;" autocomplete="off">\r\n\r\n        <span id="ticket-tags"></span>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-type"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-type" class="control-label select-label">Type</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <select id="ticket-type" class="form-control ticket-select hidden-select  muted-placeholder" placeholder="Change Type"></select>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-userqueue"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'\r\n<div class="form-group ticket-group">\r\n\r\n    <div class="tagContainer">\r\n\r\n        <input id="ticket-UserQueue-Input" class="tagInput hidden-select" type="text" placeholder="Add to Queue">\r\n\r\n        <span id="ticket-UserQueue" style="z-index:100;"></span>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-group-visibletocustomers"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){return'<div class="form-horizontal flexbox">\r\n\r\n    <div class="flexpull form-label">\r\n\r\n        <label for="ticket-visible" class="control-label">Visible</label>\r\n\r\n    </div>\r\n\r\n    <div class="flexpush form-input ticket-input-container">\r\n\r\n        <div class="checkbox" style="padding-top:5px;">\r\n\r\n            <input id="ticket-visible" type="checkbox">\r\n\r\n        </div>\r\n\r\n    </div>\r\n\r\n</div>\r\n'},useData:!0}),r["ticket-tag"]=n({compiler:[7,">= 4.0.0"],main:function(n,r,e,t,l){var i,s,a=null!=r?r:n.nullContext||{},o=e.helperMissing,c=n.escapeExpression;return'\r\n<div id="'+c((s=null!=(s=e.id||(null!=r?r.id:r))?s:o,"function"==typeof s?s.call(a,{name:"id",hash:{},data:l}):s))+'" class="'+c((s=null!=(s=e.css||(null!=r?r.css:r))?s:o,"function"==typeof s?s.call(a,{name:"css",hash:{},data:l}):s))+'">\r\n\r\n    <span class="tagRemove" aria-hidden="true">&times;</span>\r\n\r\n    '+(null!=(s=null!=(s=e.value||(null!=r?r.value:r))?s:o,i="function"==typeof s?s.call(a,{name:"value",hash:{},data:l}):s)?i:"")+"\r\n\r\n</div>\r\n"},useData:!0})}();