
$(document).ready(function () {
  var _layout = null;
  var _reportID = Ts.Utils.getQueryValue('ReportID', window);
  var _grid;
  var datamodel = new TeamSupport.DataModels.Reports(_reportID);

  $('.reports-tabview-back').click(function (e) {
    e.preventDefault();
    window.location.assign('reports.html');
  });

  $('.reports-tabview-refresh').click(function (e) {
    e.preventDefault();
    window.location = window.location;
  });

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
      size: 55,
      spacing_open: 0,
      resizable: false
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

  Ts.Utils.webMethod("ReportService", "GetReportColumnNames", { "reportID": _reportID }, function (names) {
    var columns = new Array();
    var idx = new Object();
    idx.id = "index";
    idx.name = "index";
    idx.field = "index";
    columns.push(idx);

    for (var i = 0; i < names.length; i++) {
      var column = new Object();
      column.id = names[i];
      column.name = names[i];
      column.field = names[i];
      column.width = 250;
      columns.push(column);
    }
    initGrid(columns);
  });

  var tmrDelayIndicator = null;
  var tmrHideLoading = null;
  var loadingIndicator = null;
  showLoadingIndicator();
  function showLoadingIndicator(delay) {
    if (!delay) {
      if (!loadingIndicator) {
        loadingIndicator = $("<div class='grid-loading'></div>").appendTo(document.body);
        loadingIndicator.position({ my: "center center", at: "center center", of: _layout.panes.center, collision: "none" });
      }
      loadingIndicator.show();
    }
    else {
      if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
      tmrDelayIndicator = setTimeout(function () { showLoadingIndicator(); }, delay);
    }
    if (tmrHideLoading) { clearTimeout(tmrHideLoading); }
    tmrHideLoading = setTimeout(function () { hideLoadingIndicator(); }, 3000);
  }

  function hideLoadingIndicator() {
    tmrHideLoading = null;
    if (tmrDelayIndicator) clearTimeout(tmrDelayIndicator);
    tmrDelayIndicator = null;
    if (loadingIndicator) loadingIndicator.fadeOut();
  }


  function initGrid(columns) {
    var options = {
      rowHeight: 32,
      editable: false,
      enableAddRow: false,
      enableCellNavigation: true,
      multiSelect: false,
      enableColumnReorder: true,
      forceFitColumns: false
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
      showLoadingIndicator(250);
    });

    datamodel.onDataLoaded.subscribe(function (e, args) {
      for (var i = args.from; i <= args.to; i++) {
        _grid.invalidateRow(i);
      }

      _grid.updateRowCount();
      _grid.render();

      hideLoadingIndicator();
    });
    /*
    $("#txtSearch").keyup(function (e) {
    if (e.which == 13) {
    datamodel.setSearch($(this).val());
    var vp = _grid.getViewport();
    datamodel.ensureData(vp.top, vp.bottom);
    }
    });
    */
    //datamodel.setSearch($("#txtSearch").val());
    //datamodel.setSort("create_ts", -1);
    //grid.setSortColumn("date", false);

    // load the first page
    _grid.onViewportChanged.notify();
  }
});


