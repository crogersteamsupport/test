jQuery(window).keydown(function(e) { if (e.keyCode == 123) debugger; });

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

// COPY TEXT TO CLIPBOARD.
$(document).on('click', '#clipboard', function (e) {
    var text = $(this).data('copy');
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val(text).select();
    document.execCommand("copy");
    $temp.remove();
    alert("Ticket URL has been copied.\n\n" + text);
});

$(document).on('click', '#file-browse', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#file-input').trigger('click');
});
