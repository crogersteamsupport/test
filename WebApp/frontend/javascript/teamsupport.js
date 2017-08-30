// AUTOGROW.
$.fn.autogrow = function (e) {
    return this.each(function (e) {
        if ($(this).hasClass('autogrow') && this.scrollHeight > this.clientHeight) {
            $(this).css('height', this.scrollHeight + 'px');
        }
    });
}
$(document).on('input', 'textarea.autogrow', function (e) {
    $(this).autogrow();
});


$(document).ready(function() {
    // AUTOGROW.
    $('textarea.autogrow').autogrow();

    $('.dropdown-submenu a.expand-submenu').on("click", function(e){
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
});
