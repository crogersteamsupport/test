/// <reference path="ts/ts.js" />
/// <reference path="ts/top.Ts.Services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="ts/ts.pages.main.js" />
/// <reference path="~/Default.aspx" />

var waterCoolerPage = null;
$(document).ready(function () {
  waterCoolerPage = new WaterCoolerPage();
  $("#messagecontents").autoGrow();
});

function onShow() {
  waterCoolerPage.refresh();
};


WaterCoolerPage = function () {
    // Get the top 25 WC threads and display them
    // this funciton is on the webservice in the AppCode dir.  I just returns a WC object in that file
    top.Ts.Services.WaterCooler.GetThreads(function (threads) {
        var threadContainer = $('<div>');



        // insert the threads into the div
        $('.wc-threads').empty().append(threadContainer);
        $('.loading-section').hide().next().show();
    });


    // delete link event
    $('.wc-threads').delegate('.wc-delete-link', 'click', function (e) {
        var parent = $(this).closest('.wc-message');
        var message = parent.data('message');
        top.Ts.Services.WaterCooler.DeleteMessage(message.MessageID, function (result) {
            if (result == true) parent.remove();
        });

    });

    $('#faketext').click(function (e) {
        $('#faketextcontainer').hide();
        $('#commentcontainer').show();
    });
    $('#faketextcontainer').click(function (e) {
        $(this).hide();
        $('#commentcontainer').show();
    });


    $(document).click(function (e) {
        if ($(e.target).is('#faketextcontainer, #commentcontainer *, #faketext')) return;
        $('#commentcontainer').hide();
        $('#faketextcontainer').show();
    });


    $('#addatt').click(function (e) {
        e.preventDefault();
        $("#ticketcontainer").hide();
        $("#groupcontainer").hide();
        $("#attcontainer").slideToggle("fast");
    });
    $('#addticket').click(function (e) {
        e.preventDefault();
        $("#attcontainer").hide();
        $("#groupcontainer").hide();
        $("#ticketcontainer").slideToggle("fast");
    });
    $('#addgroup').click(function (e) {
        e.preventDefault();
        $("#attcontainer").hide();
        $("#ticketcontainer").hide();
        $("#groupcontainer").slideToggle("fast");
    });

    // set up the refresh button so we can just click that to see our dev changes
    $('#btnRefresh').click(function (e) { e.preventDefault(); window.location = window.location; }).toggle(window.location.hostname.indexOf('127.0.0.1') > -1);

    // change the style of some stuff
    $('button').button();
    $('a').addClass('ui-state-default ts-link');


};


WaterCoolerPage.prototype = {
  constructor: WaterCoolerPage,
  refresh: function () {

  }
};

jQuery.fn.autoGrow = function () {
    return this.each(function () {
        // Variables
        var colsDefault = this.cols;
        var rowsDefault = this.rows;

        //Functions
        var grow = function () {
            growByRef(this);
        }

        var growByRef = function (obj) {
            var linesCount = 0;
            var lines = obj.value.split('\n');

            for (var i = lines.length - 1; i >= 0; --i) {
                linesCount += Math.floor((lines[i].length / colsDefault) + 1);
            }

            if (linesCount >= rowsDefault)
                obj.rows = linesCount + 1;
            else
                obj.rows = rowsDefault;
        }

        var characterWidth = function (obj) {
            var characterWidth = 0;
            var temp1 = 0;
            var temp2 = 0;
            var tempCols = obj.cols;

            obj.cols = 1;
            temp1 = obj.offsetWidth;
            obj.cols = 2;
            temp2 = obj.offsetWidth;
            characterWidth = temp2 - temp1;
            obj.cols = tempCols;

            return characterWidth;
        }

        // Manipulations
        //this.style.width = "auto";
        this.style.height = "auto";
        this.style.overflow = "hidden";
        //this.style.width = ((characterWidth(this) * this.cols) + 6) + "px";
        this.onkeyup = grow;
        this.onfocus = grow;
        this.onblur = grow;
        growByRef(this);
    });
};