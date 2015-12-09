
define("tinymce/ui/MenuBar", [
	"tinymce/ui/Toolbar"
], function(Toolbar) {
	"use strict";

	return Toolbar.extend({
		Defaults: {
			role: 'menubar',
			containerCls: 'menubar',
			ariaRoot: true,
			defaults: {
				type: 'menubutton'
			}
		}
	});
});
