$.fn.autogrow = function (e) {
    return this.each(function (e) {
        if ($(this).hasClass('autogrow') && this.scrollHeight > this.clientHeight) {
            $(this).css('height', this.scrollHeight + 'px');
        }
    });
}

$(document).ready(function() {
    // AUTOGROW.
    $('textarea.autogrow').autogrow();
    $('textarea.autogrow').on('input', function (e) {
        console.log('here');
        $(this).autogrow();
    });

    $('.dropdown-submenu a.expand-submenu').on("click", function(e){
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
});
