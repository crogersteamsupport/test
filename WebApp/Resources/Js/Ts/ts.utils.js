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
    webMethod: function (service, method, params, onSuccess, onFailure) {
      return $.ajax({
        type: "POST",
        url: "/Services/" + service + ".asmx/" + method,
        data: JSON.stringify(params),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
          if (typeof (onSuccess) !== 'undefined') { onSuccess(result.d); }
        },
        error: onFailure
      });
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
    secondsToHms: function(seconds) {
      seconds = Number(seconds);
      var h = Math.floor(seconds / 3600);
      var m = Math.floor(seconds % 3600 / 60);
      var s = Math.floor(seconds % 3600 % 60);
	    return ((h > 0 ? h + ":" : "") + (m > 0 ? (h > 0 && m < 10 ? "0" : "") + m + ":" : "0:") + (s < 10 ? "0" : "") + s);
   },

   getTimeSpentText: function(hours)
   {
    var minutes = Math.ceil(Number(hours) * 60);
     if (minutes > 59)
     {
      var h = Math.floor(minutes / 60);
      var m = Math.floor(minutes % 60);
      var ht = (h === 1 ? ' Hour ' : ' Hours ');
      var hm = (m === 1 ? ' Minute' : ' Minutes');
      if (m == 0) return h + ht;
      else return h + ht + m + hm;
     }
     else {
       var hm = (m === 1 ? ' Minute' : ' Minutes');
       return minutes + hm;
     }
   },
 
   getDateString: function(date, showDate, showTime)
   {
     var msDate = this.getMsDate(date);
     if ((!showDate || showDate === true) && (!showTime || showTime === true))
     {
       return msDate.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern + ' ' + top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern);
     }
     else if (showDate || showDate === true){
       return msDate.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern);
     }
     else {
       return msDate.localeFormat(top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern);
      }

     
   },
   getDateTimePattern: function () {
     return top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern + ' ' + top.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortTimePattern
   },
   getMsDate: function(args)
   {
     if (args) return new top.Date(args);
     else  return new top.Date();
   },

   setCookie: function(key,subkey,value) {
     if (arguments.length > 2) {
     
     }
     else {
    
     }
   },

   getClueTipOptions: function(tipTimer)
   {
      return {
        mouseOutClose: true,
        width: 400,
        hoverIntent: {
          sensitivity: 4,
          interval: 150,
          timeout: 0
        },
        cluetipClass: 'ui-corner-all',
        showTitle: false,
        dropShadow: (!($.browser.msie)),
        dropShadowSteps: 10,
        sticky: true,
        ajaxProcess: function (data) {
          //data = data.replace(/<(style|title)[^<]+<\/(style|title)>/gm, '').replace(/<(link|meta)[^>]+>/g, '');
          return data;
        },
        onShow: function (ct, c) {
          if (tipTimer != null) clearTimeout(tipTimer);
          ct.addClass('ui-corner-all').find('.ui-cluetip-content').addClass('ui-corner-all').find('a').addClass('ts-link ui-state-default');
        },
        onActivate: function (ct, c) {
          $(this).mouseout(function () {
          });
        }
      };
   },

   getCookie: function(key, subkey) {
     var value = $.cookie(key);
     if (value === null) return '';
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
   },
   getSizeString: function (bytes) {
      var s = ['bytes', 'kb', 'MB', 'GB', 'TB', 'PB'];
      var e = Math.floor(Math.log(bytes)/Math.log(1024));
      return (bytes/Math.pow(1024, Math.floor(e))).toFixed(2)+" "+s[e];
    },

    getTicketSourceIcon: function(source) {
      var result = 'Images/icons/TeamSupportLogo16.png';
      if (!source || source == null) { return result; }
      source = source.toLowerCase();
        switch (source) {
          case 'chatoffline': result = 'Images/icons/chat_d.png'; break;
          case 'chat': result = 'Images/icons/chat.png'; break; 
          case 'email': result = 'Images/icons/MailBox.png'; break; 
          case 'facebook': result = 'Images/party3/facebook.png'; break; 
          case 'forum': result = 'Images/icons/forum.png'; break; 
          case 'mobile': result = 'Images/icons/mobile.png'; break; 
          case 'web': result = 'Images/icons/globe.png'; break; 
          default:
            result = 'Images/icons/TeamSupportLogo16.png';
        }

      return result;
    },

    ellipseString: function (text, max) { 
      return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; 
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

