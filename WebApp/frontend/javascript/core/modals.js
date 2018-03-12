// CLOSE MODAL.
$(document).on('click tap', '#overlay, .fa.close, #btn-cancel', function(e) {
    teamsupport.modals.overlay.hide();
    $('#modal').empty().slideToggle('fast');
});

teamsupport.module('modals', function() {

    this.module('overlay', function() {
        this.show = function() {
            if ($('#overlay').is(':hidden')) {
                var loading = $('<span>').attr('id','loading').addClass('fa fa-magic fa-spin fa-3x fa-fw').css('margin-top','70px').css('color','white').fadeIn('fast');
                $('#overlay').fadeIn("fast");
                $('#overlay').html(loading);
            }
        }
        this.hide = function() {
            if ($('#overlay').is(':visible')) {
                $('#overlay').fadeOut("fast");
            }
        }
    });

    this.module('modal', function() {
        this.show = function(id) {
            if ($(id).is(':hidden')) {
                $(id).slideToggle();
                if ($(id).is(':visible')) {
                    teamsupport.modals.overlay.show();
                }
            }
        }
        this.hide = function(id) {
            if ($(id).is(':visible')) {
                $(id).slideToggle();
            }
        }
    });

});
