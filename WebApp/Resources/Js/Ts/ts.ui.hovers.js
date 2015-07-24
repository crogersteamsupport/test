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
	      //container: 'body',
	      trigger: 'hover',
	      delay: { "show": 1, "hide": 1 },
	      content: function () {
	        return $.ajax({
	          url: '../../../Tips/User.aspx?UserID=' + userid + '&TicketID=' + ticketid,
	          dataType: 'html',
	          async: false
	        }).responseText;
	      }
	    }).popover('show');
	})
  .on("click", ".UserAnchor", function (event) {
    var self = $(this);
    var userid = self.data('userid');
    top.Ts.MainPage.openUser(userid);
  })

	$("body").on("mouseenter", ".AssetAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var userid = e.data('userid');
	  var assetid = e.data('assetid');
	  e.popover({
	    html: true,
	    container: '#ticket-properties-area',
	    trigger: 'hover',
	    delay: { "show": 1, "hide": 1 },
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Asset.aspx?AssetID=' + assetid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	})
  .on("click", ".AssetAnchor", function (event) {
    var self = $(this);
    var assetid = self.data('assetid');
    top.Ts.MainPage.openAsset(assetid);
  })

	$("body").on("mouseenter", ".OrgAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var orgid = e.data('orgid');
	  var ticketid = e.data('ticketid');
	  e.popover({
	    html: true,
	    //container: 'body',
	    trigger: 'hover',
	    delay: { "show": 1, "hide": 1 },
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Customer.aspx?CustomerID=' + orgid + '&TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	})
  .on("click", ".OrgAnchor", function (event) {
    var self = $(this);
    var orgid = self.data('orgid');
    top.Ts.MainPage.openNewCustomer(orgid);
  })

	$("body").on("mouseenter", ".SLAAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var ticketid = e.data('ticketid');
	  e.popover({
	    html: true,
	    //container: 'body',
	    trigger: 'hover',
	    delay: { "show": 1, "hide": 1 },
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
	    //container: 'body',
	    trigger: 'hover',
	    delay: { "show": 1, "hide": 1 },
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Ticket.aspx?TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	})
  .on("click", ".TicketAnchor", function (event) {
    var self = $(this);
    var ticketid = self.data('ticketid');
    top.Ts.MainPage.openTicketByID(ticketid, true);
  })

	$("body").on("mouseenter", ".ProductAnchor", function (event) {
	  var e = $(this);
	  e.unbind('hover');
	  var ticketid = e.data('ticketid');
	  var productid = e.data('productid');
	  e.popover({
	    html: true,
	    container: 'body',
	    trigger: 'hover',
	    delay: { "show": 1, "hide": 1000 },
	    content: function () {
	      return $.ajax({
	        url: '../../../Tips/Product.aspx?ProductID=' + productid + '&TicketID=' + ticketid,
	        dataType: 'html',
	        async: false
	      }).responseText;
	    }
	  }).popover('show');
	});
});