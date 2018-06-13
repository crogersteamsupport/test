function getColor(value){
    //value from 0 to 1
    if (value > 0) {
        var hue = ((1 - value) * 120).toString(10);
        return ["hsl(",hue,",100%,50%)"].join("");
    }
}

function WatsonTicket(ticketid) {
    window.parent.Ts.Services.TicketPage.WatsonTicket(ticketid, function(result) {
        console.log(result);
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);
            var percentage = data.TicketSentimentScore / 1000;
            var reverse = 1 - percentage;
            var display = parseInt(percentage * 100);
            var color = getColor(reverse);
            console.log(data.TicketSentimentScore + ' / ' + color);
            $('#health-ticket').css({ 'background-color':color }).css({ 'text-align':'left' });
            $('#health-meter').css({ 'width':display + 'px' });
            $('#health-message').removeClass('disabled').addClass('enabled').text(display + '%');
        } else {
            var color = getColor(0.100);
            console.log(color);
        }
    });
}



function WatsonCustomer(organizationID) {
    _mainFrame.Ts.Services.Customers.GetOrganizationSentiment(organizationID, function(e) {
        if (e.length > 0) {
            $('#organizationSentiment').show();
            var percentage = e / 1000;
            var reverse = 1 - percentage;
            var display = percentage * 100;
            var color = getColor(reverse);
            console.log(e + ' / ' + color);
            $('#health-ticket').css({ 'background-color':color });
            $('#health-message').text('Health is ' + display + '%');
        }
    });
}



function meter () {
    var elem = document.getElementById("myBar");
    var width = 1;
    var id = setInterval(frame, 10);
    function frame() {
        if (width >= 100) {
            clearInterval(id);
        } else {
            width++;
            elem.style.width = width + '%';
        }
    }
}
