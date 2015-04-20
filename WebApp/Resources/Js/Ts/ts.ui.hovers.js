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
	    var name = e.data('name');
	    top.Ts.Services.Users.GetUserContactCard(userid, function (cardhtml) {
	        e.popover({
	            html: true,
	            title: name,
	            trigger: 'hover',
	            content: cardhtml,
	            delay: { "show": 500, "hide": 500 }
	        }).popover('show');
	    });
	});
});