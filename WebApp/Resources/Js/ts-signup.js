$(document).ready(function () {
    function getURLParameter(name) {
        var val = (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search) || [, null])[1];
        return val ? decodeURIComponent(val) : null;
    }

    if (getURLParameter("suerrors") != null) { alert("There was an error processing your sign up request.  Please review your information and try again."); }

    $('#submit').click(function (e) {
        validateCompany();
        validateEmail();
        validateName();

        if ($('.form-group.has-error').length > 0) {
            e.preventDefault();
            return;
        }

        var btn = $(this).hide();
        if ($('.img-busy').length < 1) {
            $('<img>').attr('src', 'http://www.teamsupport.com/images/dots64.gif').insertAfter(btn);
        }
        else {
            $('.img-busy').show();
        }




    });

    $('input[name="name"]').keyup(function (e) { validateName(); }).change(function (e) { validateName(); });
    $('input[name="email"]').keyup(function (e) { validateEmail(); }).change(function (e) { validateEmail(); });
    $('input[name="company"]').keyup(function (e) { validateCompany(); }).change(function (e) { validateCompany(); });

    function validateName() {
        var el = $('input[name="name"]');
        var nameReg = /\w+\s+\w+/;
        var name = $.trim(el.val());
        if (!nameReg.test(name)) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }

    function validateEmail() {
        var el = $('input[name="email"]');
        var emailReg = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        var email = $.trim(el.val());
        if (!emailReg.test(email) || email == '') {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }
    }

    function validateCompany(callback) {
        var el = $('input[name="company"]');
        var company = $.trim(el.val());
        var group = el.closest('.form-group');
        var help = group.find('.help-block');
        if (company == '') {
            group.addClass('has-error');
            help.text("Please enter a company name.");
            return;
        }


        $.ajax({
            type: "GET",
            url: "http://trunk.tsdev.com/signup/fn/validatecompany",
            data: { name: company },
            crossDomain: true,
            dataType: 'jsonp'
        })
            .done(function (result) {
                if (result.isValid == false) {
                    group.addClass('has-error');
                    help.text("That company name alread exists.  Please choose another one.");
                }
                else {
                    group.removeClass('has-error');
                }
                if (callback) callback();
            });
    }

    function validatePhone() {
        var el = $('input[name="phone"]');
        var intRegex = /[0-9 -()+]+$/;
        var phone = $.trim(el.val());
        if ((phone.length < 6) || (!intRegex.test(phone))) {
            el.closest('.form-group').addClass('has-error');
        } else {
            el.closest('.form-group').removeClass('has-error');
        }

    }




});

//window.top.location = 'http://www.teamsupport.com/thank-you-for-trying-teamsupport/?userid=' + result;
