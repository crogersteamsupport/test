
define("tinymce/ui/FormItem", [
	"tinymce/ui/Container"
], function(Container) {
	"use strict";

	return Container.extend({
		Defaults: {
			layout: 'flex',
			align: 'center',
			defaults: {
				flex: 1
			}
		},

		/**
		 * Renders the control as a HTML string.
		 *
		 * @method renderHtml
		 * @return {String} HTML representing the control.
		 */
		renderHtml: function() {
			var self = this, layout = self._layout, prefix = self.classPrefix;

			self.classes.add('formitem');
			layout.preRender(self);

			return (
				'<div id="' + self._id + '" class="' + self.classes + '" hidefocus="1" tabindex="-1">' +
					(self.settings.title ? ('<div id="' + self._id + '-title" class="' + prefix + 'title">' +
						self.settings.title + '</div>') : '') +
					'<div id="' + self._id + '-body" class="' + self.bodyClasses + '">' +
						(self.settings.html || '') + layout.renderHtml(self) +
					'</div>' +
				'</div>'
			);
		}
	});
});
