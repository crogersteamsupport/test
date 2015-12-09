
define("tinymce/ui/FitLayout", [
	"tinymce/ui/AbsoluteLayout"
], function(AbsoluteLayout) {
	"use strict";

	return AbsoluteLayout.extend({
		/**
		 * Recalculates the positions of the controls in the specified container.
		 *
		 * @method recalc
		 * @param {tinymce.ui.Container} container Container instance to recalc.
		 */
		recalc: function(container) {
			var contLayoutRect = container.layoutRect(), paddingBox = container.paddingBox;

			container.items().filter(':visible').each(function(ctrl) {
				ctrl.layoutRect({
					x: paddingBox.left,
					y: paddingBox.top,
					w: contLayoutRect.innerW - paddingBox.right - paddingBox.left,
					h: contLayoutRect.innerH - paddingBox.top - paddingBox.bottom
				});

				if (ctrl.recalc) {
					ctrl.recalc();
				}
			});
		}
	});
});
