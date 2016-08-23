function getMainFrame(wnd) {
    if (!wnd) wnd = window;
    var result = wnd;
    var cnt = 0;
    while (!(result.Ts && result.Ts.Services)) {
        result = result.parent;
        cnt++;
        if (cnt > 5) return null;
    }
    return result;
}

(function () {
    var mainFrame = getMainFrame();

    window.pendo_options = mainFrame.pendo_options;

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.async = true;
    script.src = ('https:' === document.location.protocol ? 'https://' : 'http://') + 'd3accju1t3mngt.cloudfront.net/js/pa.min.js';
    var firstScript = document.getElementsByTagName('script')[0];
    firstScript.parentNode.insertBefore(script, firstScript);
})();

document.onkeydown = function (event) {

    if (!event) { /* This will happen in IE */
        event = window.event;
    }

    var keyCode = event.keyCode;

    if (keyCode == 8 &&
		((event.target || event.srcElement).tagName != "TEXTAREA") &&
		((event.target || event.srcElement).tagName != "INPUT")) {

        if (navigator.userAgent.toLowerCase().indexOf("msie") == -1) {
            event.stopPropagation();
        } else {
            alert("prevented");
            event.returnValue = false;
        }

        return false;
    }
};
