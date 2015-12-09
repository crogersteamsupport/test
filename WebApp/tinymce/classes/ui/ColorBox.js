
define("tinymce/ui/ColorBox", [
	"tinymce/ui/ComboBox"
], function(ComboBox) {
	"use strict";

	return ComboBox.extend({
		/**
		 * Constructs a new control instance with the specified settings.
		 *
		 * @constructor
		 * @param {Object} settings Name/value object with settings.
		 */
		init: function(settings) {
			var self = this;

			settings.spellcheck = false;

			if (settings.onaction) {
				settings.icon = 'none';
			}

			self._super(settings);

			self.classes.add('colorbox');
			self.on('change keyup postrender', function() {
				self.repaintColor(self.value());
			});
		},

		repaintColor: function(value) {
			var elm = this.getEl().getElementsByTagName('i')[0];

			if (elm) {
				try {
					elm.style.background = value;
				} catch (ex) {
					// Ignore
				}
			}
		},

		bindStates: function() {
			var self = this;

			self.state.on('change:value', function(e) {
				if (self._rendered) {
					self.repaintColor(e.value);
				}
			});

			return self._super();
		}
	});
});
