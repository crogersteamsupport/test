(function ($) {
    function ReportsModel(reportID) {
        // private
        var PAGESIZE = 50;
        var data = { length: 0 };
        var _reportID = reportID;
        var searchstr = "";
        var sortcol = null;
        var sortdir = 1;
        var h_request = null;
        var req = null; // ajax request

        // events
        var onDataLoading = new Slick.Event();
        var onDataLoaded = new Slick.Event();


        function init() {
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
        }


        function ensureData(from, to, loadedCallback) {
            if (req) {
                req.abort();
                for (var i = req.fromPage; i <= req.toPage; i++)
                    data[i * PAGESIZE] = undefined;
            }

            if (from < 0) { from = 0; }

            if (data.length > 0) { to = Math.min(to, data.length - 1); }

            var fromPage = Math.floor(from / PAGESIZE);
            var toPage = Math.floor(to / PAGESIZE);

            while (data[fromPage * PAGESIZE] !== undefined && fromPage < toPage)
                fromPage++;

            while (data[toPage * PAGESIZE] !== undefined && fromPage < toPage)
                toPage--;

            if (fromPage > toPage || ((fromPage == toPage) && data[fromPage * PAGESIZE] !== undefined)) {
                // TODO:  look-ahead
                onDataLoaded.notify({ from: from, to: to });
                return;
            }

            if (h_request != null) {
                clearTimeout(h_request);
            }

            h_request = setTimeout(function () {
                for (var i = fromPage; i <= toPage; i++)
                    data[i * PAGESIZE] = null; // null indicates a 'requested but not available yet'

                onDataLoading.notify({ from: from, to: to });

                var params = { "reportID": _reportID, "from": fromPage * PAGESIZE, "to": (toPage * PAGESIZE) + PAGESIZE - 1, "sortField": sortcol, "isDesc": (sortdir < 1) };
                //console.log('REQUEST: From: ' + fromPage * PAGESIZE + ', To: ' + ((fromPage * PAGESIZE) + PAGESIZE-1) + "  Page: " + fromPage);
                req = $.ajax({
                    type: "POST",
                    url: "/Services/ReportService.asmx/GetReportData",
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (resp) {
                        onSuccess(resp)
                        if (loadedCallback) loadedCallback();
                    },
                    error: function () {
                        onError(fromPage, toPage);
                        if (loadedCallback) loadedCallback();
                    }

                });
                req.fromPage = fromPage;
                req.toPage = toPage;
            }, 100);
        }


        function onError(fromPage, toPage) {
            //alert("error loading pages " + fromPage + " to " + toPage);
        }

        function onSuccess(resp) {
            var from = resp.d.From, to = resp.d.To, results = JSON.parse(resp.d.Data);
            //console.log('RESPONSE: From: ' + from + ', To: ' + to);

            if (results.length > 0) {

                data.length = results[0].TotalRows || results.length;
                //console.log('count: ' + results.length);
                for (var i = 0; i < results.length; i++) {
                    var item = results[i];
                    data[from + i] = item;
                    data[from + i].index = from + i;
                }
            }

            req = null;
            onDataLoaded.notify({ from: from, to: to });
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

        init();

        return {
            // properties
            "data": data,
            "reportID": reportID,

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

    $.extend(true, window, { TeamSupport: { DataModels: { Reports: ReportsModel}} });
})(jQuery);
