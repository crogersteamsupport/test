
/*global jQuery:true */

/*
 * Fake Sizzle using jQuery.
 */
define("tinymce/dom/Sizzle", [], function() {
	// Detect if jQuery is loaded
	if (!window.jQuery) {
		throw new Error("Load jQuery first");
	}

	return jQuery.find;
});
