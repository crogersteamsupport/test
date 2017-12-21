(function(g, r, o, w, t, h, _, s, d, k) {
    if (!g[w] || !g[w]._q) {
        for (; s < _.length;) t(h, _[s++]);
        d = r.createElement(o);
        d.async = 1;
        d.src = "https://d3ixbrge5tdwth.cloudfront.net/gs/v1.1/build.min.js";
        k = r.getElementsByTagName(o)[0];
        k.parentNode.insertBefore(d, k);
        g[w] = h
    }
})(window, document, "script", "growthscore", function(g, r) {
    g[r] = function() {
        g._q.push([r, arguments])
    }
}, {
    _q: [],
    _v: 1
}, "init widget discardSurvey submitSurvey survey testimonials renderTestimonials referralWidget getTerms getFriendsList".split(" "), 0);

var gs_appkey = '319b0d10584c41368947a81e016d835d';

growthscore.init(gs_appkey, {
    emailid: 'USER_EMAIL (required)',
    firstname: 'FIRST_NAME',
    lastname: 'LAST_NAME',
    signupdate: 'SIGNUP_DATE (yyyy-mm-dd)',
    storeid: 'USER_STOREID (External ID)',
    attributes: {
        "custom_1": "CUSTOM_1_VALUE",
        "custom_2": "CUSTOM_2_VALUE"
    }
}, function(err, data) {
    //Handle Error Data
});
