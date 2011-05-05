
(function ($) {
  $.widget("ui.combobox", {

    _create: function () {
      var self = this,
					select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "",

          div = this.div = $("<div>")
          .css("left", "-10000")
          .appendTo("body")
          .addClass('ui-combobox ui-widget'),

          table = this.table = $("<table>")
          .attr("cellpadding", "0")
          .attr("cellspacing", "0")
          .attr("border", "0")
          .appendTo(div)
          .addClass("ui-combobox-table"),
          row = this.row = $("<tr>")
          .appendTo(table),
          cellInput = this.cellInput = $("<td>")
          .appendTo(row),
          cellButton = this.cellButton = $("<td>")
          .appendTo(row),

          input = this.input = $("<input>")
					.appendTo(cellInput)
					.val(value)
          .width(select.width() < 50 ? "100px" : select.width())
          .click(function () { input.autocomplete("search", ""); })
					.removeClass("ui-corner-all")
					.autocomplete({
					  delay: 0,
					  minLength: 0,
					  source: function (request, response) {
					    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
					    response(select.children("option").map(function () {
					      var text = $(this).text();
					      if (this.value != null && (!request.term || matcher.test(text))) {
					        return {
					          label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
					          value: text,
					          option: this
					        };
					      }
					    }));
					  },

					  select: function (event, ui) {
					    ui.item.option.selected = true;
					    self._trigger("selected", event, {
					      item: ui.item.option
					    });
					  },

					  change: function (event, ui) {
					    if (!ui.item) {
					      var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
									valid = false;
					      select.children("option").each(function () {
					        if ($(this).text().match(matcher)) {
					          this.selected = valid = true;
					          return false;
					        }
					      });
					      if (!valid) {
					        // remove invalid value, as it didn't match anything
					        $(this).val("");
					        select.val("");
					        input.data("autocomplete").term = "";
					        return false;
					      }
					    }
					  }
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

      input.data("autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
      };

      this.button = $("<button>&nbsp;</button>")
					.attr("tabIndex", -1)
					.attr("title", "Show All Items")
					.appendTo(cellButton)
					.button({
					  icons: {
					    primary: "ui-icon-triangle-1-s"
					  },
					  text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
          .height(this.input.outerHeight())
          .width(this.input.outerHeight())
					.click(function (e) {
					  e.preventDefault();
					  // close if already visible
					  if (input.autocomplete("widget").is(":visible")) {
					    input.autocomplete("close");
					    return false;
					  }

					  // pass empty string as value to search for, displaying all results
					  input.autocomplete("search", "");
					  input.focus();
					});

      div.css("left", "").detach().insertAfter(select);


    },

    update: function () {
      var selected = this.element.children(":selected"),
					value = selected.val() ? selected.text() : "";
      this.input.val(value);
    },
    setValue: function (value) {
      this.element.children(":selected").removeAttr("selected");
      this.element.children('option[value="' + value + '"]').attr("selected", "selected");
      this.update();
    },

    destroy: function () {
      this.input.remove();
      this.button.remove();
      this.element.show();
      this.table.remove();
      $.Widget.prototype.destroy.call(this);
    }
  });

} (jQuery));