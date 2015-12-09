
define("tinymce/ui/AbsoluteLayout", [
	"tinymce/ui/Layout"
], function(Layout) {
	"use strict";

	return Layout.extend({
		Defaults: {
			containerClass: 'abs-layout',
			controlClass: 'abs-layout-item'
		},

		/**
		 * Recalculates the positions of the controls in the specified container.
		 *
		 * @method recalc
		 * @param {tinymce.ui.Container} container Container instance to recalc.
		 */
		recalc: function(container) {
			container.items().filter(':visible').each(function(ctrl) {
				var settings = ctrl.settings;

				ctrl.layoutRect({
					x: settings.x,
					y: settings.y,
					w: settings.w,
					h: settings.h
				});

				if (ctrl.recalc) {
					ctrl.recalc();
				}
			});
		},

		/**
		 * Renders the specified container and any layout specific HTML.
		 *
		 * @method renderHtml
		 * @param {tinymce.ui.Container} container Container to render HTML for.
		 */
		renderHtml: function(container) {
			return '<div id="' + container._id + '-absend" class="' + container.classPrefix + 'abs-end"></div>' + this._super(container);
		}
	});
});
