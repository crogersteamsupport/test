
define("tinymce/ui/StackLayout", [
	"tinymce/ui/FlowLayout"
], function(FlowLayout) {
	"use strict";

	return FlowLayout.extend({
		Defaults: {
			containerClass: 'stack-layout',
			controlClass: 'stack-layout-item',
			endClass: 'break'
		},

		isNative: function() {
			return true;
		}
	});
});
