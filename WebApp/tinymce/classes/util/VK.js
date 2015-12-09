
define("tinymce/util/VK", [
	"tinymce/Env"
], function(Env) {
	return {
		BACKSPACE: 8,
		DELETE: 46,
		DOWN: 40,
		ENTER: 13,
		LEFT: 37,
		RIGHT: 39,
		SPACEBAR: 32,
		TAB: 9,
		UP: 38,

		modifierPressed: function(e) {
			return e.shiftKey || e.ctrlKey || e.altKey || this.metaKeyPressed(e);
		},

		metaKeyPressed: function(e) {
			// Check if ctrl or meta key is pressed. Edge case for AltGr on Windows where it produces ctrlKey+altKey states
			return (Env.mac ? e.metaKey : e.ctrlKey && !e.altKey);
		}
	};
});
