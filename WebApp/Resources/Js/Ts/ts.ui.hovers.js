$(document).ready(function () {
	var originalLeave = $.fn.popover.Constructor.prototype.leave;
	$.fn.popover.Constructor.prototype.leave = function (obj) {
	    var self = obj instanceof this.constructor ?
	        obj : $(obj.currentTarget)[this.type](this.getDelegateOptions()).data('bs.' + this.type)
	    var container, timeout;

	    originalLeave.call(this, obj);

	    if (obj.currentTarget) {
	        container = $(obj.currentTarget).siblings('.popover')
	        timeout = self.timeout;
	        container.one('mouseenter', function () {
	            //We entered the actual popover – call off the dogs
	            clearTimeout(timeout);
	            //Let's monitor popover content instead
	            container.one('mouseleave', function () {
	                $.fn.popover.Constructor.prototype.leave.call(self, self);
	            });
	        })
	    }
	};

	$("body").on("mouseenter", ".UserAnchor", function (event) {
	    var e = $(this);
	    e.unbind('hover');
	    var userid = e.data('userid');
	    var ticketid = e.data('ticketid');
	    e.popover({
	      html: true,
	      trigger: 'hover',
	      content: function () {
	        return $.ajax({
	          url: '../../../Tips/User.aspx?UserID=' + userid + '&TicketID=' + ticketid,
	          dataType: 'html',
	          async: false
	        }).responseText;
	      }
	    }).popover('show');
	});

	$("body").on("mouseenter", ".AssetAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var userid = e.data('userid');
	  var assetid = e.data('assetid');
	  e.popover({
	    html: true,
	    trigger: 'hover',
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Asset.aspx?AssetID=' + assetid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	});

	$("body").on("mouseenter", ".OrgAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var orgid = e.data('orgid');
	  var ticketid = e.data('ticketid');
	  e.popover({
	    html: true,
	    trigger: 'hover',
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Customer.aspx?CustomerID=' + orgid + '&TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	});

	$("body").on("mouseenter", ".SLAAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var ticketid = e.data('ticketid');
	  e.popover({
	    html: true,
	    trigger: 'hover',
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Sla.aspx?TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	});

	$("body").on("mouseenter", ".TicketAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var ticketid = e.data('ticketid');
	  e.popover({
	    html: true,
	    trigger: 'hover',
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Ticket.aspx?TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	});
});