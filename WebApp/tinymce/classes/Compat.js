
define("tinymce/Compat", [
	"tinymce/dom/DOMUtils",
	"tinymce/dom/EventUtils",
	"tinymce/dom/ScriptLoader",
	"tinymce/AddOnManager",
	"tinymce/util/Tools",
	"tinymce/Env"
], function(DOMUtils, EventUtils, ScriptLoader, AddOnManager, Tools, Env) {
	var tinymce = window.tinymce;

	/**
	 * @property {tinymce.dom.DOMUtils} DOM Global DOM instance.
	 * @property {tinymce.dom.ScriptLoader} ScriptLoader Global ScriptLoader instance.
	 * @property {tinymce.AddOnManager} PluginManager Global PluginManager instance.
	 * @property {tinymce.AddOnManager} ThemeManager Global ThemeManager instance.
	 */
	tinymce.DOM = DOMUtils.DOM;
	tinymce.ScriptLoader = ScriptLoader.ScriptLoader;
	tinymce.PluginManager = AddOnManager.PluginManager;
	tinymce.ThemeManager = AddOnManager.ThemeManager;

	tinymce.dom = tinymce.dom || {};
	tinymce.dom.Event = EventUtils.Event;

	Tools.each(Tools, function(func, key) {
		tinymce[key] = func;
	});

	Tools.each('isOpera isWebKit isIE isGecko isMac'.split(' '), function(name) {
		tinymce[name] = Env[name.substr(2).toLowerCase()];
	});

	return {};
});

// Describe the different namespaces






