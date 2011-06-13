if (typeof Ts === "undefined") Ts = {};

(function () {

  function TsUtils() {

  }

  TsUtils.prototype =
  {
    constructor: TsUtils,
    getQueryValue: function (name, wnd) {
      if (!wnd) wnd = window;
      params = wnd.location.search.substring(1);
      name = name.toLowerCase();
      param = params.split("&");
      for (i = 0; i < param.length; i++) {
        value = param[i].split("=");
        if (value[0].toLowerCase() == name) { return unescape(value[1]); }
      }
      return null;
    },

    findClass: function (classString, term) {
      if (!classString || classString.length < 1) return '';
      classes = classString.split(' ');
      for (var i = 0; i < classes.length; i++) {
        if (classes[i].indexOf(term) === 0) {
          return classes[i];
        }
      }
      return '';
    },

    getIdFromElement: function (idClass, element) { return this.getNameParam(idClass, element, 0); },
    getTypeFromElement: function (idClass, element) { return this.getNameParam(idClass, element, 1); },
    getNameParam: function (idClass, element, indexFromEnd, splitter) {
      var item = '';
      if (!splitter) splitter = '-';
      idClass = idClass + splitter;
      if (element.id && element.id.indexOf(idClass) === 0) {
        item = element.id;
      }
      else {
        item = this.findClass($(element).attr('class'), idClass);
      }

      if (item === '') { return null; }
      var result = item.split(splitter);
      return result[result.length - (indexFromEnd + 1)];
    },

    ticketFilterToQuery: function (ticketFilter) {
      var result = '';
      for (property in ticketFilter) {
        if (property == '__type') continue;
        if (result != '') result = result + '&';
        result = result + 'tf_' + property + '=' + ticketFilter[property];
      }
      return result;
    },

    queryToTicketFilter: function (wnd) {
      var result = new TeamSupport.Data.TicketLoadFilter();

      if (!wnd) wnd = window;
      params = wnd.location.search.substring(1);
      param = params.split("&");
      for (i = 0; i < param.length; i++) {
        if (param[i].substr(0, 3) == 'tf_') {
          var item = param[i].split("=");
          var key = item[0].substr(3, item[0].length - 3);
          result[key] = unescape(item[1]);
        }
      }
      return result;
    },

    getDateTimePattern: function () {

      return top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern + ' ' + top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern
    
    },


    secondsToHms: function(seconds) {
      seconds = Number(seconds);
      var h = Math.floor(seconds / 3600);
      var m = Math.floor(seconds % 3600 / 60);
      var s = Math.floor(seconds % 3600 % 60);
	    return ((h > 0 ? h + ":" : "") + (m > 0 ? (h > 0 && m < 10 ? "0" : "") + m + ":" : "0:") + (s < 10 ? "0" : "") + s);
   },

   getTimeSpentText: function(hours)
   {
    var minutes = Math.floor(Number(hours) * 60);
     if (minutes > 59)
     {
      var h = Math.floor(minutes / 60);
      var m = Math.floor(minutes % 60);
      if (m == 0) return h + ' Hours';
      else return h + ' Hours ' + m + ' Minutes';
     }
     else {
       return minutes + ' Minutes';
     }
   },

   setCookie: function(key,subkey,value) {
     if (arguments.length > 2) {
     
     }
     else {
    
     }
   },

   getCookie: function(key, subkey) {
     var value = $.cookie(key);
     if (subkey === null)
     {
       return value == null ? '' : value;
     }

     var cookies = value.split('&');
     for (var i = 0; i < cookies.length; i++) {
       var cookie = cookies[i].split('=');
       if (cookie.length > 1) {
         if (cookie[0] == key) return cookie[1] == null ? '' : cookie[1];
       }
     }
     return '';
   }



  };

  Ts.Utils = new TsUtils();

})();

Ts.Utils.EventHandler = function() {
  this.handlers = [];

  this.subscribe = function (fn) {
    this.handlers.push(fn);
  };

  this.notify = function (args) {
    for (var i = 0; i < this.handlers.length; i++) {
      this.handlers[i].call(this, args);
    }
  };

  return this;
}

jQuery.cookie = function (key, value, options) {

  // key and value given, set cookie...
  if (arguments.length > 1 && (value === null || typeof value !== "object")) {
    options = jQuery.extend({}, options);

    if (value === null) {
      options.expires = -1;
    }

    if (typeof options.expires === 'number') {
      var days = options.expires, t = options.expires = new Date();
      t.setDate(t.getDate() + days);
    }

    return (document.cookie = [
            encodeURIComponent(key), '=',
            options.raw ? String(value) : encodeURIComponent(String(value)),
            options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
            options.path ? '; path=' + options.path : '',
            options.domain ? '; domain=' + options.domain : '',
            options.secure ? '; secure' : ''
        ].join(''));
  }

  // key and possibly options given, get cookie...
  options = value || {};
  var result, decode = options.raw ? function (s) { return s; } : decodeURIComponent;
  return (result = new RegExp('(?:^|; )' + encodeURIComponent(key) + '=([^;]*)').exec(document.cookie)) ? decode(result[1]) : null;
};
