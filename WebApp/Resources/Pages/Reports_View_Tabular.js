
$(document).ready(function () {
  var _layout = null;
  var _reportID = Ts.Utils.getQueryValue('ReportID', window);
  var _grid;
  var datamodel = new TeamSupport.DataModels.Reports(_reportID);

/*  $('.slick-header').click(function (e) {
    e.preventDefault();
    window.location.href = window.location.href;
  });*/

    _layout = $('#reports-tabview-layout').layout({
  //resizeNestedLayout: true,
  //maskIframesOnResize: true,
  defaults: {
  spacing_open: 5,
  closable: false
  },
center: { 
  
  onresize: resizeGrid,
  triggerEventsOnLoad: false,
  minSize: 500
  },
  north: {
  size: 50,
  spacing_open: 0,
  resizable: false
  },
  south: {
  spacing_open: 5,
  size: 20,
  closable: false
  }
  });


  function resizeGrid(paneName, paneElement, paneState, paneOptions, layoutName) {
    if (loadingIndicator) {
      loadingIndicator.remove();
      loadingIndicator = null;
    }
    try {
      var vp = _grid.getViewport();
      var t = vp.top;
      datamodel.clear();
      datamodel.ensureData(vp.top, vp.bottom + 50, function () {
        if (t > 10) _grid.scrollRowIntoView(t + 10, false);
        _grid.resizeCanvas();
      });
    } catch (e) {
      alert(e.message);
    }

  }

  var dateFormatter = function (row, cell, value, columnDef, dataContext) {
    return (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear();
  };

  Ts.Utils.webMethod("ReportService", "GetReportColumnNames", {"reportID": _reportID}, function (names) {
    var columns = new Array();
    for (var i = 0; i < names.length; i++) {
      var column = new Object();
      column.id = names[i];
      column.name = names[i];
      column.field = names[i];
      columns.push(column);
    }
    initGrid(columns);
  });
  /*
  var columns = [
  { id: "num", name: "#", field: "index", width: 40 },
  { id: "story", name: "Story", width: 520, field: "title" },
  { id: "date", name: "Date", field: "create_ts", width: 60, formatter: dateFormatter, sortable: true },
  { id: "points", name: "Points", field: "points", width: 60, sortable: true }
  ];*/


  var loadingIndicator = null;

  function initGrid(columns) {
    var options = {
      rowHeight: 22,
      editable: false,
      enableAddRow: false,
      enableCellNavigation: true,
      multiSelect: false,
      enableColumnReorder: true,
      forceFitColumns: true
    };

    _grid = new Slick.Grid("#reports-tabview-grid-container", datamodel.data, columns, options);

    _grid.onViewportChanged.subscribe(function (e, args) {
      var vp = _grid.getViewport();
      datamodel.ensureData(vp.top, vp.bottom);
    });

    _grid.onSort.subscribe(function (e, args) {
      datamodel.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
      var vp = _grid.getViewport();
      datamodel.ensureData(vp.top, vp.bottom);
    });

    datamodel.onDataLoading.subscribe(function () {
      if (!loadingIndicator) {
        loadingIndicator = $("<span class='loading-indicator'><label>Buffering...</label></span>").appendTo(document.body);
        var $g = $("#reports-tabview-grid-container");

        loadingIndicator
            .css("position", "absolute")
            .css("top", $g.position().top + $g.height() / 2 - loadingIndicator.height() / 2)
            .css("left", $g.position().left + $g.width() / 2 - loadingIndicator.width() / 2);
      }

      loadingIndicator.show();
    });

    datamodel.onDataLoaded.subscribe(function (e, args) {
      for (var i = args.from; i <= args.to; i++) {
        _grid.invalidateRow(i);
      }

      _grid.updateRowCount();
      _grid.render();

      loadingIndicator.fadeOut();
    });

    $("#txtSearch").keyup(function (e) {
      if (e.which == 13) {
        datamodel.setSearch($(this).val());
        var vp = _grid.getViewport();
        datamodel.ensureData(vp.top, vp.bottom);
      }
    });

    //datamodel.setSearch($("#txtSearch").val());
    //datamodel.setSort("create_ts", -1);
    //grid.setSortColumn("date", false);

    // load the first page
    _grid.onViewportChanged.notify();
  }
});


