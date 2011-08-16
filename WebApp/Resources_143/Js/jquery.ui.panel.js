
(function ($, undefined) {

  var uiPanelClasses = "ui-panel-content ui-widget ui-widget-content ";

  $.widget("ui.panel", {
    options: {
      buttons: {},
      panelClass: "",
      title: ""
    },

    _create: function () {
      this.originalTitle = this.element.attr("title");
      if (typeof this.originalTitle !== "string") {
        this.originalTitle = "";
      }

      this.options.title = this.options.title || this.originalTitle;
      var self = this,
			options = self.options,

			title = options.title || "&#160;",
			titleId = $.ui.panel.getTitleId(self.element),

			uiPanel = (self.uiPanel = $(this.element))
        .wrap('<div />')
        .parent()
        .addClass('ui-widget ui-panel'),

			uiPanelContent = self.element
				.show()
				.removeAttr("title")
				.addClass(uiPanelClasses)
				.appendTo(uiPanel),

			uiPanelTitlebar = (self.uiPanelTitlebar = $("<div>"))
				.addClass("ui-panel-titlebar  ui-widget-header  " +
					"ui-corner-top  ui-helper-clearfix")
				.prependTo(uiPanel),

			uiPanelTitle = $("<span>")
				.addClass("ui-panel-title")
				.attr("id", titleId)
				.text(title)
				.prependTo(uiPanelTitlebar);

      uiPanelTitlebar.find("*").add(uiPanelTitlebar).disableSelection();

      self._createButtons(options.buttons);
    },

    _init: function () {
    },

    _destroy: function () {
      var self = this;

      self.uiPanel.hide();
      self.element
			.removeClass("ui-panel-content ui-widget-content")
			.hide()
			.appendTo("body");
      self.uiPanel.remove();

      if (self.originalTitle) {
        self.element.attr("title", self.originalTitle);
      }
    },

    widget: function () {
      return this.uiPanel;
    },


    _createButtons: function (buttons) {
      var self = this,
			hasButtons = false,
			uiPanelButtonPane = $("<div>")
				.addClass("ui-panel-buttonpane ui-helper-clearfix");

      // if we already have a button pane, remove it
      self.uiPanel.find(".ui-panel-buttonpane").remove();

      if (typeof buttons === "object" && buttons !== null) {
        $.each(buttons, function () {
          return !(hasButtons = true);
        });
      }
      if (hasButtons) {
        $.each(buttons, function (name, props) {
          var button = $("<div>")
          .addClass('ui-panel-button ui-corner-all ' + (props.buttonClass ? props.buttonClass : ""))
          .hover(function () { button.addClass('ui-state-hover'); }, function () { button.removeClass('ui-state-hover'); });
          if (props.click) {
            button.click(function () { props.click.apply(self.element[0], arguments); });
          }

          if (props.iconClass) {
            $("<span>")
            .addClass('ui-panel-button-icon ' + props.iconClass)
            .appendTo(button);
          }

          if (props.text) {
            $("<span>")
            .addClass('ui-panel-button-text')
            .text(props.text)
            .appendTo(button);
          }

          button.appendTo(uiPanelButtonPane);
        });

        uiPanelButtonPane.appendTo(self.uiPanelTitlebar);
      }
    },


    _setOptions: function (options) {
      var self = this;

      $.each(options, function (key, value) {
        self._setOption(key, value);
      });
    },

    _setOption: function (key, value) {
      var self = this,
			uiPanel = self.uiPanel;

      switch (key) {
        case "buttons":
          self._createButtons(value);
          break;
        case "panelClass":
          uiPanel
					.removeClass(self.options.panelClass)
					.addClass(uiPanelClasses + value);
          break;
        case "title":
          // convert whatever was passed in o a string, for html() to not throw up
          $(".ui-panel-title", self.uiPanelTitlebar)
					.html("" + (value || "&#160;"));
          break;
      }

      this._super("_setOption", key, value);
    },

    setTitle: function (title) {
      this.uiPanelTitle.text(title);
    }

  });

  $.extend($.ui.panel, {
    version: "@VERSION",

    uuid: 0,
    maxZ: 0,

    getTitleId: function ($el) {
      var id = $el.attr("id");
      if (!id) {
        this.uuid += 1;
        id = this.uuid;
      }
      return "ui-panel-title-" + id;
    }

  });



} (jQuery));