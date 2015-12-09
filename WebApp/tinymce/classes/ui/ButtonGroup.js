
define("tinymce/ui/ButtonGroup", [
	"tinymce/ui/Container"
], function(Container) {
	"use strict";

	return Container.extend({
		Defaults: {
			defaultType: 'button',
			role: 'group'
		},

		/**
		 * Renders the control as a HTML string.
		 *
		 * @method renderHtml
		 * @return {String} HTML representing the control.
		 */
		renderHtml: function() {
			var self = this, layout = self._layout;

			self.classes.add('btn-group');
			self.preRender();
			layout.preRender(self);

			return (
				'<div id="' + self._id + '" class="' + self.classes + '">' +
					'<div id="' + self._id + '-body">' +
						(self.settings.html || '') + layout.renderHtml(self) +
					'</div>' +
				'</div>'
			);
		}
	});
});
