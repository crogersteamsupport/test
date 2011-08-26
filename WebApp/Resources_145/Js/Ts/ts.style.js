/// <reference path="../jquery.color.js" />
/// <reference path="../jquery-latest.min.js" />

if (typeof Ts === "undefined") Ts = {};

(function () {

  function TsStyle() {

  }

  TsStyle.prototype =
  {
    constructor: TsStyle,
    content: function (selector) {
      var content = $(selector)
      .addClass('ui-widget-header ts-content');
      
      var color = content.cssColor('background-color').toHSL().modify([null, null, .96]).applyCSS(selector, 'background-color');
      //var color = content.cssColor('background-color').toHSL().adjust([0, 0, 0.075]).applyCSS(selector, 'background-color');
      var sections = content.find('.ts-section').addClass('ui-widget-content ui-corner-all');
      //sections.cssColor('border-top-color').toHSV().adjust([0, 0, .1]).fix();

      color
        .modify([null, null, .91])
        .applyCSS(sections, 'border-top-color')
        .applyCSS(sections, 'border-left-color')
        .applyCSS(sections, 'border-bottom-color')
        .applyCSS(sections, 'border-right-color');
    }

  };

  Ts.Style = new TsStyle();

})();

