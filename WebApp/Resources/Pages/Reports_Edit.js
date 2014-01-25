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
            list.find('li:last').clone().appendTo(list);
        });

        $('.summary-fields').on('click', 'li a', function (e) {
            e.preventDefault();
            $(this).closest('li').fadeOut(function () { remove(); });
        });

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
                    $('<li>')
                                .addClass('report-field ' + getUniqueFieldClass(fields[i]))
                                .data('field', fields[i])
                                .append($('<div>', {
                                    class: 'checkbox'
                                }).append($('<label>', {
                                    html: ' <span class="text-muted">' + fields[i].Table + '.</span>' + fields[i].Name
                                }).prepend('<input type="checkbox" />')))
                                .appendTo(list);
                    if (tableName != fields[i].Table) {
                        tableName = fields[i].Table;
                        optGroupx = $('<optgroup>').attr('label', tableName).appendTo('.summary-desc .summary-field');
                        optGroupy = $('<optgroup>').attr('label', tableName).appendTo('.summary-calc .summary-field');
                    }

                    $('<option>').text(fields[i].Name).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupx);
                    $('<option>').text(fields[i].Name).data('field', fields[i]).addClass(getUniqueFieldClass(fields[i])).appendTo(optGroupy);
                }
                _fields = fields;
                if (_report != null) loadSelectedFields();
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

            switch (_reportType) {
                case 0: // tabular
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


                    top.Ts.Utils.webMethod("ReportService", "SaveReport", {
                        "reportID": _reportID,
                        "name": $('.report-name').val(),
                        "reportType": _reportType,
                        "data": JSON.stringify(tabData)
                    },
                    closeReport,
                    function (error) { alert(error.get_message()); }
                    );

                    break;
                case 1: // chart
                    break;
                case 4: // summary
                    break;
                case 2: // external
                    top.Ts.Utils.webMethod("ReportService", "SaveReport", {
                        "reportID": _reportID,
                        "name": $('.report-name').val(),
                        "reportType": _reportType,
                        "data": $('#external-url').val()
                    }, closeReport,
                    function (error) { alert(error.get_message()); }
                    );
                    break;
                default:
                    break;
            }
        });

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
    }


    $('.nav-brand').click(function (e) {
        e.preventDefault();
        location.assign(location.href);
    });

});