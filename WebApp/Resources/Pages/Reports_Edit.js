/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.ui.menutree.js" />
/// <reference path="ts/ts.ui.tabs.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="ts/ts.grids.models.tickets.js" />
/// <reference path="~/Default.aspx" />
$(document).ready(function () {
    var _reportID = top.Ts.Utils.getQueryValue('ReportID', window);
    var _reportType = top.Ts.Utils.getQueryValue('ReportType', window);
    var _chartData = null;
    if (_reportType != null) _reportType = parseInt(_reportType);
    var _tempReport = new Object();
    var _typeClass = '';
    var location = window.location;
    var _report = null;
    var _cats = null;
    var _fields = null;
    var _subID = -1;

    $('.report-filter').reportFilter();

    if (_reportID != null) {
        top.Ts.Utils.webMethod("ReportService", "GetReport", {
            "reportID": _reportID
        }, function (report) {
            _report = report;
            $('.report-name').val(report.Name);
            initReport();
        });
    } else if (_reportType != null) {
        initReport();
    } else {
        return;
    }


    function initReport() {
        $('.report-class-item').hide();
        if (_reportType == null) { _reportType = _report.ReportType; }
        var type = _reportType;

        switch (type) {
            case 0:
                _typeClass = 'report-class-tabular';
                $('.reports-header i').addClass('fa-table color-red');
                $('.report-title').text(_report ? _report.Name : 'New Tabular Report');
                break;
            case 1:
                _typeClass = 'report-class-chart';
                $('.reports-header i').addClass('fa-bar-chart-o color-green');
                $('.report-title').text(_report ? _report.Name : 'New Chart');
                $('.report-type-radio-chart').prop('checked', true);
                $('.summary-add-calcfield').hide();
                break;
            case 2:
                _typeClass = 'report-class-external';
                $('.reports-header i').addClass('fa-globe color-blue');
                $('.report-title').text(_report ? _report.Name : 'New External Report');
                break;
            case 4:
                _typeClass = 'report-class-summary';
                $('.reports-header i').addClass('fa-tasks color-yellow');
                $('.report-title').text(_report ? _report.Name : 'New Summary Report');
                break;
            default:
        }
        $('.report-class-item.' + _typeClass).show();
        $('.report-setup').show();

        if (type == 2) {
            if (_report && _report.ReportDef) {
                $('#external-url').val(_report.ReportDef);
            }
            $('.action-back, .action-next').hide();
            $('.action-save').show();
            $('.action-cancel').css('float', 'none');
        } else {
            if (_report && _report.ReportDef) {
                _report.Def = JSON.parse(_report.ReportDef);
            }
            top.Ts.Services.Reports.GetCategories(loadCats);
        }

        $('.action-cancel').click(function (e) {
            e.preventDefault();
            closeReport();
        });

        function validateSection(el) {
            if (el.hasClass('report-setup')) {
                if ($('.report-name').val() == '') {
                    $('.report-name').popover('show').parent('.form-group').addClass('has-error');
                    return false;
                }
                else {
                    $('.report-name').popover('hide').parent('.form-group').removeClass('has-error');
                }
                return null;
            }
            if (el.hasClass('report-fields')) {
                return $('li.report-field input:checked').length > 0 ? null : "Please select some field.";
            }
        }

        function loadNewSection(el, callback) {
            if (el.hasClass('report-fields') || el.hasClass('report-summary-fields')) {
                if (_report && _report.Def.Subcategory != $('#selectSubCat').val()) { _report = null; }
                if (!(_tempReport.Subcategory && _tempReport.Subcategory == $('#selectSubCat').val())) { loadFields(callback); }
            }
            else if (el.hasClass('report-filters')) {
                initFilters();
                if (callback) callback();
            }
            else if (el.hasClass('report-chartproperties')) {
                initChart(callback);
            }
            else {
                if (callback) callback();
            }
        }

        $('.action-next').click(function (e) {
            e.preventDefault();
            var currentSection = $('.report-section:visible');
            var validation = validateSection(currentSection);
            if (validation != null) {
                if (validation == "") {
                    return;
                }
                else {
                    alert(validation);
                    return;
                }
            }
            var nextSection = currentSection.nextAll('.' + _typeClass + ':first');

            loadNewSection(nextSection, function () {
                currentSection.fadeOut(200, function () {
                    $('.action-back').removeClass('disabled').prop('disabled', false);
                    $('.action-next').removeClass('disabled').prop('disabled', false);
                    if (nextSection.nextAll('.' + _typeClass + ':first').length < 1) {
                        $('.action-next').hide();
                        $('.action-save').show();
                    }
                    nextSection.show();
                });
            });

        });

        $('.action-back').click(function (e) {
            e.preventDefault();
            $('.action-save').hide();
            $('.action-next').show();

            var currentSection = $('.report-section:visible').hide();
            var prevSection = currentSection.prevAll('.' + _typeClass + ':first').show();

            if (prevSection.prevAll('.' + _typeClass + ':first').length < 1) {
                $('.action-back').addClass('disabled').prop('disabled', false);
            }

        });
        function loadCats(cats) {
            _cats = cats;
            for (var i = 0; i < cats.length; i++) {
                var option = $('<option/>').attr('value', cats[i].ReportTableID).text(cats[i].Name);
                if (_report != null && findSubCat(cats[i].Subcategories, _report.Def.Subcategory) != null) {
                    option.prop('selected', true);
                }
                $('#selectCat').append(option);
            }
            loadSubCats();
        }


        $('.summary-add-field').click(function (e) {
            e.preventDefault();
            var list = $(this).prev();
            initSummaryRow(list.find('li:first').clone(true).appendTo(list));
            if (_reportType == 1) $('.summary-add-descfield').hide();
        });

        $('.summary-fields').on('click', 'li a', function (e) {
            e.preventDefault();
            $(this).closest('li').fadeOut(function () { $(this).remove(); });
            if (_reportType == 1) $('.summary-add-descfield').show();
        });

        $('.summary-fields').on('change', '.summary-desc-field', function (e) {
            e.preventDefault();
            var item = $(this);
            var field = item.find(':selected').data('field');
            if (field.DataType == 'datetime') {
                item.next().show();
            }
            else {
                item.next().hide();
            }
        });

        $('.summary-fields').on('change', '.summary-calc-field', function (e) {
            e.preventDefault();
            var item = $(this);
            var field = item.find(':selected').data('field');
            console.log(JSON.stringify(field));
            if (field.DataType == 'number') {
                item.next().show().next().hide();
            }
            else {
                item.next().hide().next().show();
            }
        });

        $('.summary-fields').on('change', '.summary-calc-comp', function (e) {
            e.preventDefault();
            var value = $(this).val();
            if (value == 'none') {
                $(this).next().hide().next().hide();
            }
            else if (value == 'bet') {
                $(this).next().show().next().show();
            } else {
                $(this).next().show().next().hide();
            }
        });

        function initSummaryRow(el) {
            el.find('.summary-desc-field').trigger('change');
            el.find('.summary-calc-field').trigger('change');
            el.find('.summary-calc-comp').trigger('change');
        }

        function loadSummaryFields() {
            clearSummaryFields();
            var list = $('.summary-desc');
            for (var i = 0; i < _report.Def.Fields.Descriptive.length; i++) {
                var field = _report.Def.Fields.Descriptive[i];
                if (i > 0) list.find('li:first').clone(true).appendTo(list);
                var item = list.find('li:last');
                item.find('.' + getUniqueFieldClass(field.Field)).prop('selected', true).trigger('change');
                item.find('.summary-desc-val1').val(field.Value1);
            }

            list = $('.summary-calc');
            for (var i = 0; i < _report.Def.Fields.Calculated.length; i++) {
                var field = _report.Def.Fields.Calculated[i];
                if (i > 0) list.find('li:first').clone(true).appendTo(list);
                var item = list.find('li:last');
                item.find('.' + getUniqueFieldClass(field.Field)).prop('selected', true).trigger('change');
                item.find('.summary-calc-arg').val(field.Aggregate);
                item.find('.summary-calc-text-arg').val(field.Aggregate);
                item.find('.summary-calc-comp').val(field.Comparator).trigger('change');
                item.find('.summary-calc-val1').val(field.Value1);
                item.find('.summary-calc-val2').val(field.Value2);

            }

        }

        function clearSummaryFields() {
            $('.summary-calc li').not(':first').remove();
            $('.summary-desc li').not(':first').remove();
            $('.summary-add-descfield').show();
        }

        function findSubCat(subcats, subcatID) {
            for (var i = 0; i < subcats.length; i++) {
                if (subcats[i].SubCatID == subcatID) return subcats[i];
            }
            return null;
        }

        $('#selectCat').change(loadSubCats);

        function loadSubCats() {
            var reportTableID = $('#selectCat').val();
            $('#selectSubCat').empty();
            //$('#selectSubCat').append($('<option/>').attr('value', '-1').text('None'));
            for (var i = 0; i < _cats.length; i++) {
                if (_cats[i].ReportTableID == reportTableID) {
                    for (var j = 0; j < _cats[i].Subcategories.length; j++) {
                        var option = $('<option/>').attr('value', _cats[i].Subcategories[j].SubCatID).text(_cats[i].Subcategories[j].Name);
                        if (_report != null && _cats[i].Subcategories[j].SubCatID == _report.Def.Subcategory) {
                            option.prop('selected', true);
                        }
                        $('#selectSubCat').append(option);
                    }
                }
            }
        }

        function loadFields(callback) {
            var subID = $('#selectSubCat').val();

            if (_subID == subID) {
                if (callback) callback();
                return;
            }

            $('.summary-desc .summary-field').empty();
            $('.summary-calc .summary-field').empty();

            _subID = subID;
            _fields = null;
            $('.report-fields-available ul').empty();
            top.Ts.Services.Reports.GetFields(subID, function (fields) {
                var tableName = "";
                var optGroup = null;
                var optGroupx = null;
                var optGroupy = null;
                var max = Math.floor(fields.length / 3);
                var rem = fields.length % 3;
                var iteration = 1;
                var cnt = rem > 0 ? -1 : 0;
                var list = $('.report-fields-available  ul.report-fields-1');
                for (var i = 0; i < fields.length; i++) {
                    if (cnt > max - 1) {
                        iteration++;
                        cnt = rem == iteration ? -1 : 0;
                        list = $('.report-fields-available  ul.report-fields-' + iteration);
                    }
                    cnt++;


                    delete fields[i]['__type'];
                    var fieldName = fields[i].Name + (fields[i].AuxName ? " (" + fields[i].AuxName + ")" : "");

                    $('<li>')
                                .addClass('report-field ' + getUniqueFieldClass(fields[i]))
                                .data('field', fields[i])
                                .append($('<div>', {
                                    class: 'checkbox'
                                }).append($('<label>', {
                                    html: ' <span class="text-muted">' + fields[i].Table + '.</span>' + fieldName
                                }).prepend('<input type="checkbox" />')))
                                .appendTo(list);
                    if (tableName != fields[i].Table) {
                        tableName = fields[i].Table;
                        optGroupx = $('<optgroup>').attr('label', tableName).appendTo('.summary-desc .summary-field');
                        optGroupy = $('<optgroup>').attr('label', tableName).appendTo('.summary-calc .summary-field');
                    }

                    $('<option>').text(fieldName).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupx);
                    $('<option>').text(fieldName).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupy);
                }
                _fields = fields;

                clearSummaryFields();
                initSummaryRow($('.summary-fields'));
                if (_report != null) {
                    loadSelectedFields();
                    if (_reportType == 1 || _reportType == 4) loadSummaryFields();
                }
                $('.report-filter').reportFilter('loadFields', fields);
                if (callback) callback();
            });
        }

        function loadSelectedFields() {
            for (var i = 0; i < _report.Def.Fields.length; i++) {
                $('.' + getUniqueFieldClass(_report.Def.Fields[i]) + ' input').prop('checked', true).trigger('change');
            }
        }

        function getUniqueFieldClass(field) {
            var id = field.ID ? field.ID : field.FieldID;
            if (field.IsCustom)
                return 'report-field-id-c' + id;
            else
                return 'report-field-id-s' + id;
        }

        $('.action-save').click(function (e) {
            e.preventDefault();
            var data = null;
            switch (_reportType) {
                case 0: data = JSON.stringify(getTabularObject()); break;
                case 1: data = JSON.stringify(getChartObject()); break;
                case 4: data = JSON.stringify(getSummaryObject()); break;
                case 2:
                    data = $('#external-url').val();
                    if (data.indexOf('http://') < 0) data = 'http://' + data;

                    break;
                default: break;
            }

            top.Ts.Utils.webMethod("ReportService", "SaveReport", {
                "reportID": _reportID,
                "name": $('.report-name').val(),
                "reportType": _reportType,
                "data": data
            },
            closeReport,
            function (error) { alert(error.get_message()); });
        });

        function getTabularObject() {
            var tabData = new Object();
            tabData.Fields = new Array();
            $('li.report-field input:checked').each(function (index) {
                var field = $(this).closest('li').data('field');
                var fieldItem = new Object();
                fieldItem.FieldID = field.ID;
                fieldItem.IsCustom = field.IsCustom;
                tabData.Fields.push(fieldItem);
            });
            tabData.Subcategory = $('#selectSubCat').val();
            tabData.Filters = $('.report-filter').reportFilter('getObject');
            return tabData;
        }

        function getChartObject() {
            var chartData = new Object();
            chartData.Filters = $('.report-filter').reportFilter('getObject');
            chartData.Subcategory = $('#selectSubCat').val();
            chartData.Fields = getSummaryFieldsObject();
            chartData.Chart = getHighChartOptions();
            return chartData;
        }

        function getSummaryObject() {
            var sumData = new Object();
            sumData.Filters = $('.report-filter').reportFilter('getObject');
            sumData.Subcategory = $('#selectSubCat').val();
            sumData.Fields = getSummaryFieldsObject();
            return sumData;
        }

        function getSummaryFieldsObject() {
            var fields = new Object();
            descs = new Array();
            calcs = new Array();

            $('.summary-desc li').each(function () {
                var desc = new Object();
                var item = $(this);
                var data = item.find('.summary-desc-field option:selected').data('field');
                desc.Field = new Object();
                desc.Field.FieldID = data.ID;
                desc.Field.IsCustom = data.IsCustom;
                desc.Value1 = item.find('.summary-desc-val1').val();
                descs.push(desc);
            });

            $('.summary-calc li').each(function () {
                var calc = new Object();
                var item = $(this);
                var data = item.find('.summary-calc-field option:selected').data('field');
                calc.Field = new Object();
                calc.Field.FieldID = data.ID;
                calc.Field.IsCustom = data.IsCustom;
                calc.Aggregate = data.DataType == 'number' ? item.find('.summary-calc-arg').val() : item.find('.summary-calc-text-arg').val();
                calc.Comparator = item.find('.summary-calc-comp').val();
                calc.Value1 = item.find('.summary-calc-val1').val();
                calc.Value2 = item.find('.summary-calc-val2').val();
                calcs.push(calc);
            });


            fields.Descriptive = descs;
            fields.Calculated = calcs;
            return fields;
        }

        function closeReport(report) {
            var result = '/vcr/1_7_0/pages/';

            function getReportUrl(r) {
                switch (r.ReportType) {
                    case 0:
                        return 'Reports_View_Tabular.html?ReportID=' + r.ReportID;
                    case 1:
                        return 'Reports_View_Chart.html?ReportID=' + r.ReportID;
                    case 2:
                        return 'Reports_View_External.html?ReportID=' + r.ReportID;
                    case 4:
                        return 'Reports_View_Tabular.html?ReportID=' + r.ReportID;
                }
            }

            if (!report) // canceled
            {
                if (_report) // go back to report
                {
                    result = result + getReportUrl(_report);
                } else { // go back to list
                    result = result + 'reports.html';
                }
            } else { // saved
                if (_report) // go back to report
                {
                    result = result + getReportUrl(report);
                } else { // go back to list AND open tab
                    result = result + 'reports.html';
                    top.Ts.MainPage.openReport(report);
                }
            }

            location.assign(result);
        }

        function initFilters() {
            if ($('.filter .filter-group').length < 1) {
                if (_report != null && _report.Def.Filters && _report.Def.Filters.length > 0) {
                    $('.report-filter').reportFilter('loadFilters', _report.Def.Filters);
                }
            }
        }


        function getHighChartOptions(data) {
            var options = {};
            options.credits = { enabled: false }
            options.title = { text: $('.report-name').val(), x: -20 };
            options.subtitle = { text: $('#chart-subtitle').val(), x: -20 };
            options.yAxis = {};
            options.yAxis.title = { text: $('#chart-y-title').val() };
            options.yAxis.plotLines = [{ value: 0, width: 1, color: '#808080'}];
            options.tooltip = { valueSuffix: $('#chart-tip-suffix').val() };
            options.legend = {};
            options.legend.layout = $('#chart-legend-layout').val();
            options.legend.align = $('#chart-legend-align').val();
            options.legend.verticalAlign = $('#chart-legend-valign').val();
            options.tscharttype = $('#chart-type').val();
            switch ($('#chart-type').val()) {
                case 'line':
                    break;
                case 'area':
                    options.chart = { type: 'area' };
                    break;
                case 'stackedarea':
                    options.chart = { type: 'area' };
                    options.plotOptions = {};
                    options.plotOptions.area = { stacking: 'normal' };
                    break;
                case 'bar':
                    options.chart = { type: 'bar' };
                    options.plotOptions = {};
                    options.plotOptions.bar = { dataLabels: true };
                    break;
                case 'stackedbar':
                    options.chart = { type: 'bar' };
                    options.plotOptions = {};
                    options.plotOptions.bar = { stacking: 'normal' };
                    break;
                case 'column':
                    options.chart = { type: 'column' };
                    options.plotOptions = {};
                    options.plotOptions.column = { dataLabels: true };
                    break;
                case 'stackedcolumn':
                    options.chart = { type: 'column' };
                    options.plotOptions = {};
                    options.plotOptions.column = { stacking: 'normal' };
                    break;
                case 'pie':
                    options.chart = { type: 'pie' };
                    break;
                default:
            }

            if (data) {
                options.series = data.Series;
                options.xAxis = { categories: data.Categories };
            }

            return options;
        }

        function buildChart(data) {
            if (data.length < 1) {
                alert("There is no data to create a chart.");
                return;
            }
            $('.chart-container').highcharts(getHighChartOptions(data));
        }

        function getChartData(data, callback) {
            top.Ts.Utils.webMethod("ReportService", "GetChartData",
              { "summaryReportFields": data },
              callback,
              function (error) {
                  alert(error.get_message());
              });
        }

        function initChart(callback) {
            if (_report != null && _report.Def.Chart) {
                var options = _report.Def.Chart;
                $('#chart-type').val(options.tscharttype);
                $('#chart-subtitle').val(options.subtitle.text);
                $('#chart-y-title').val(options.yAxis.title.text);
                $('#chart-tip-suffix').val(options.tooltip.valueSuffix);
                $('#chart-legend-layout').val(options.legend.layout);
                $('#chart-legend-align').val(options.legend.align);
                $('#chart-legend-valign').val(options.legend.verticalAlign);
            }

            getChartData(JSON.stringify(getSummaryObject()),
                function (data) {
                    _chartData = data;
                    buildChart(JSON.parse(data));
                    if (callback) callback();
                });
        }

        $('.chart-generate').click(function (e) {
            e.preventDefault();
            buildChart(JSON.parse(_chartData));
        });

        $('.report-chartproperties input, .report-chartproperties select').change(function (e) {
            buildChart(JSON.parse(_chartData));
        });

    }

});