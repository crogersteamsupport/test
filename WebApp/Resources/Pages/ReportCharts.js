function addChartData(options, records) {
    options.colors = ['#3276B1', '#193b58', '#78A300', '#e72b19', '#008080', '#E57B3A', '#bd4cff', '#FFC312', '#BA55D3'];
    
    
    function fixRecordName(record, index) {
        if (record.fieldType == 'bool') {
            return record.name + (record.data[index] == true ? ' = True' : ' = False');
        }
        return record.data[index];
    }

    if (options.ts.chartType == 'pie') {
        if (records.length > 2) {
            return 'Please select only one descriptive field to plot a pie chart.';
        }

        var total = 0;
        for (var i = 0; i < records[1].data.length; i++) {
            total += parseInt(records[1].data[i]);
        }

        options.series = [{ type: 'pie', name: options.ts.seriesTitle, data: []}];

        for (var i = 0; i < records[1].data.length; i++) {
            var val = records[1].data[i] / total * 100;
            options.series[0].data.push([records[0].data[i], parseFloat(val.toFixed(2))]);
        }
    }
    else if ((records[0].fieldType == 'datetime' && records[0].format == 'date') || (records[1].fieldType == 'datetime' && records[1].format == 'date')) {
        options.series = [];
        options.xAxis = { type: 'datetime' };

        if (records.length == 3) {
            var series = null;
            for (var i = 0; i < records[0].data.length; i++) {
                if (!series || series.value != records[0].data[i]) {
                    series = { name: fixRecordName(records[0], i), value: records[0].data[i], data: [] };
                    options.series.push(series);
                }

                var item = [];
                item.push(Date.parse(records[1].data[i]));
                item.push(records[2].data[i]);
                series.data.push(item);

            }

        }
        else if (records.length = 2) {
            options.series.push({ name: records[1].name, data: [] });

            for (var i = 0; i < records[0].data.length; i++) {
                var item = [];
                item.push(Date.parse(records[0].data[i]));
                item.push(records[1].data[i]);
                options.series[0].data.push(item);
            }
        }

    }
    else {
        options.series = [];

        if (records.length == 3) {

            options.xAxis = { categories: [] };

            function indexOfCategory(name) {
                for (var i = 0; i < options.xAxis.categories.length; i++) {
                    if (options.xAxis.categories[i] == name) {
                        return i;
                    }
                }
                return -1;
            }

            for (var i = 0; i < records[1].data.length; i++) {
                var name = fixRecordName(records[1], i);
                if (indexOfCategory(name) < 0) {
                    options.xAxis.categories.push(name);
                }
            }

            function createDataArray() {
                var result = [];
                for (var i = 0; i < options.xAxis.categories.length; i++) {
                    result.push(0);
                }
                return result;
            }

            var series = null;
            for (var i = 0; i < records[0].data.length; i++) {
                if (!series || series.value != records[0].data[i]) {
                    series = { name: fixRecordName(records[0], i), value: records[0].data[i], data: createDataArray() };
                    options.series.push(series);
                }
                var catIndex = indexOfCategory(fixRecordName(records[1], i));
                if (records[2].data[i]) series.data[catIndex] = records[2].data[i];
            }

        }
        else if (records.length = 2) {
            options.xAxis = { categories: records[0].data };
            options.series.push({ name: records[1].name, data: records[1].data });
        }

    }

}
