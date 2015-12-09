
define("tinymce/util/Fun", [], function() {
	function constant(value) {
		return function() {
			return value;
		};
	}

	return {
		constant: constant
	};
});
