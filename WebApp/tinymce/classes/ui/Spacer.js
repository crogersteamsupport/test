
define("tinymce/ui/Spacer", [
	"tinymce/ui/Widget"
], function(Widget) {
	"use strict";

	return Widget.extend({
		/**
		 * Renders the control as a HTML string.
		 *
		 * @method renderHtml
		 * @return {String} HTML representing the control.
		 */
		renderHtml: function() {
			var self = this;

			self.classes.add('spacer');
			self.canFocus = false;

			return '<div id="' + self._id + '" class="' + self.classes + '"></div>';
		}
	});
});
