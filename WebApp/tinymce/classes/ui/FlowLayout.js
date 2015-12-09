
define("tinymce/ui/FlowLayout", [
	"tinymce/ui/Layout"
], function(Layout) {
	return Layout.extend({
		Defaults: {
			containerClass: 'flow-layout',
			controlClass: 'flow-layout-item',
			endClass: 'break'
		},

		/**
		 * Recalculates the positions of the controls in the specified container.
		 *
		 * @method recalc
		 * @param {tinymce.ui.Container} container Container instance to recalc.
		 */
		recalc: function(container) {
			container.items().filter(':visible').each(function(ctrl) {
				if (ctrl.recalc) {
					ctrl.recalc();
				}
			});
		},

		isNative: function() {
			return true;
		}
	});
});
