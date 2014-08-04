//TeamSupport Marketing

/*!
* jQuery Cookie Plugin v1.4.1
* https://github.com/carhartl/jquery-cookie
*
* Copyright 2013 Klaus Hartl
* Released under the MIT license
*/
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // CommonJS
        factory(require('jquery'));
    } else {
        // Browser globals
        factory(jQuery);
    }
} (function (jQuery) {

    var pluses = /\+/g;

    function encode(s) {
        return config.raw ? s : encodeURIComponent(s);
    }

    function decode(s) {
        return config.raw ? s : decodeURIComponent(s);
    }

    function stringifyCookieValue(value) {
        return encode(config.json ? JSON.stringify(value) : String(value));
    }

    function parseCookieValue(s) {
        if (s.indexOf('"') === 0) {
            // This is a quoted cookie as according to RFC2068, unescape...
            s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
        }

        try {
            // Replace server-side written pluses with spaces.
            // If we can't decode the cookie, ignore it, it's unusable.
            // If we can't parse the cookie, ignore it, it's unusable.
            s = decodeURIComponent(s.replace(pluses, ' '));
            return config.json ? JSON.parse(s) : s;
        } catch (e) { }
    }

    function read(s, converter) {
        var value = config.raw ? s : parseCookieValue(s);
        return jQuery.isFunction(converter) ? converter(value) : value;
    }

    var config = jQuery.cookie = function (key, value, options) {

        // Write

        if (value !== undefined && !jQuery.isFunction(value)) {
            options = jQuery.extend({}, config.defaults, options);

            if (typeof options.expires === 'number') {
                var days = options.expires, t = options.expires = new Date();
                t.setTime(+t + days * 864e+5);
            }

            return (document.cookie = [
				encode(key), '=', stringifyCookieValue(value),
				options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
				options.path ? '; path=' + options.path : '',
				options.domain ? '; domain=' + options.domain : '',
				options.secure ? '; secure' : ''
			].join(''));
        }

        // Read

        var result = key ? undefined : {};

        // To prevent the for loop in the first place assign an empty array
        // in case there are no cookies at all. Also prevents odd result when
        // calling $.cookie().
        var cookies = document.cookie ? document.cookie.split('; ') : [];

        for (var i = 0, l = cookies.length; i < l; i++) {
            var parts = cookies[i].split('=');
            var name = decode(parts.shift());
            var cookie = parts.join('=');

            if (key && key === name) {
                // If second argument (value) is a function it's a converter...
                result = read(cookie, value);
                break;
            }

            // Prevent storing a cookie that we couldn't decode.
            if (!key && (cookie = read(cookie)) !== undefined) {
                result[name] = cookie;
            }
        }

        return result;
    };

    config.defaults = {};

    jQuery.removeCookie = function (key, options) {
        if (jQuery.cookie(key) === undefined) {
            return false;
        }

        // Must not alter options, thus extending a fresh object...
        jQuery.cookie(key, '', jQuery.extend({}, options, { expires: -1 }));
        return !jQuery.cookie(key);
    };

}));

jQuery(document).ready(function () {
    function getURLParameter(name) {
        var val = (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1];
        return val ? decodeURIComponent(val) : null;
    }

    var params = new Object();
    params.Source = getURLParameter("utm_source");
    if (params.Source) {
        params.Medium = getURLParameter("utm_medium");
        params.Term = getURLParameter("utm_term");
        params.Content = getURLParameter("utm_content");
        params.Campaign = getURLParameter("utm_campaign");
        jQuery.cookie("_tsm", JSON.stringify(params), { expires: 7, path: '/', domain: 'teamsupport.com' });
    }

    if (!jQuery.cookie("_tsmi")) {
        var ga = jQuery.cookie("__utmz");
        if (ga) {
            function parseGAString(key) {
                var i = ga.indexOf(key + '=');
                if (i < 0) return null;
                var s = ga.substr(i + key.length + 1);
                var j = s.indexOf("|");
                if (j > -1) { s = s.substr(0, j); }
                return decodeURIComponent(s);
            }

            params = new Object();
            params.Campaign = parseGAString("utmccn");
            params.Content = parseGAString("utmcct");
            params.Term = parseGAString("utmctr");
            params.Medium = parseGAString("utmcmd");
            params.Source = parseGAString("utmcsr");

            /*if (parseGAString("utmgclid") != "") {
                params.Source = "Google";
                params.Medium = "cpc";
            }
            */

            jQuery.cookie("_tsmi", JSON.stringify(params), { expires: 7, path: '/', domain: 'teamsupport.com' });
        }
    }
});
