
define("tinymce/ui/Radio", [
	"tinymce/ui/Checkbox"
], function(Checkbox) {
	"use strict";

	return Checkbox.extend({
		Defaults: {
			classes: "radio",
			role: "radio"
		}
	});
});
