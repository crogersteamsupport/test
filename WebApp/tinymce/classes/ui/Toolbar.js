
define("tinymce/ui/Toolbar", [
	"tinymce/ui/Container"
], function(Container) {
	"use strict";

	return Container.extend({
		Defaults: {
			role: 'toolbar',
			layout: 'flow'
		},

		/**
		 * Constructs a instance with the specified settings.
		 *
		 * @constructor
		 * @param {Object} settings Name/value object with settings.
		 */
		init: function(settings) {
			var self = this;

			self._super(settings);
			self.classes.add('toolbar');
		},

		/**
		 * Called after the control has been rendered.
		 *
		 * @method postRender
		 */
		postRender: function() {
			var self = this;

			self.items().each(function(ctrl) {
				ctrl.classes.add('toolbar-item');
			});

			return self._super();
		}
	});
});
