(function ($) {
    function GridModel(getData, getItemMetadata) {
        // private
        var PAGESIZE = 50;
        var data = { length: 0 };
        var searchstr = "";
        var sortcol = null;
        var sortdir = 1;
        var h_request = null;
        var req = null; // ajax request

        // events
        var onDataLoading = new Slick.Event();
        var onDataLoaded = new Slick.Event();

        if (getItemMetadata) {
            data.getItemMetadata = function (index) {
                return getItemMetadata(index, data);
            }
        }


        function isDataLoaded(from, to) {
            for (var i = from; i <= to; i++) {
                if (data[i] == undefined || data[i] == null) {
                    return false;
                }
            }

            return true;
        }


        function clear() {
            for (var key in data) {
                delete data[key];
            }
            data.length = 0;
            if (getItemMetadata) {
                data.getItemMetadata = function (index) {
                    return getItemMetadata(index, data);
                }
            }
        }


        function ensureData(from, to, loadedCallback) {
            if (req) {
                if (req.get_executor()) req.get_executor().abort(); else req.abort();
                for (var i = req.from; i <= req.to; i++)
                    data[i] = undefined;
            }

            to = to + 2;
            from = from - 2;
            if (from < 0) { from = 0; }

            while (data[from] !== undefined && from < to) { from++; }
            while (data[to] !== undefined && from < to) { to--; }
            if (from >= to) { return; }


            if (h_request != null) {
                clearTimeout(h_request);
            }

            h_request = setTimeout(function () {
                for (var i = from; i < to - 1; i++) data[i] = null; // null indicates a 'requested but not available yet'

                onDataLoading.notify({ from: from, to: to });
                //console.log("FromReq: " + from + ", ToReq: " + to);

                req = getData(from, to, sortcol, (sortdir < 1),
                function (resp) {
                    onSuccess(resp);
                    if (loadedCallback) loadedCallback();

                });
                req.from = from;
                req.to = to;
            }, 100);
        }


        function onSuccess(resp) {
            if (resp.d) resp = resp.d;
            var from = resp.From, to = resp.To;

            var results = typeof resp.Data == 'string' ? JSON.parse(resp.Data) : resp.Data;
            //console.log('RESPONSE: From: ' + from + ', To: ' + to);
            data.length = resp.Total;

            if (results.length > 0) {

                //console.log('count: ' + results.length);
                for (var i = 0; i < results.length; i++) {
                    var item = results[i];
                    data[from + i] = item;
                    data[from + i].index = from + i;
                }
            }

            req = null;
            onDataLoaded.notify({ 'from': from, 'to': to, 'total': resp.Total });
        }


        function reloadData(from, to) {
            for (var i = from; i <= to; i++)
                delete data[i];

            ensureData(from, to);
        }


        function setSort(column, dir) {
            sortcol = column;
            sortdir = dir;
            clear();
        }

        function setSearch(str) {
            searchstr = str;
            clear();
        }

        return {
            // properties
            "data": data,

            // methods
            "clear": clear,
            "isDataLoaded": isDataLoaded,
            "ensureData": ensureData,
            "reloadData": reloadData,
            "setSort": setSort,
            "setSearch": setSearch,

            // events
            "onDataLoading": onDataLoading,
            "onDataLoaded": onDataLoaded
        };
    }

    $.extend(true, window, { TeamSupport: { DataModel: GridModel} });
})(jQuery);
