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
            var display = percentage * 100;
            var color = getColor(reverse);
            console.log(data.TicketSentimentScore + ' / ' + color);
            $('#ticketSentiment').css({ 'color':color });
            $('#health-ticket').text('Ticket Health: ' + display + '%');
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
            $('#health-organization').css({ 'color':color });
            $('#health-score').text('Health: ' + display + '%');
        }
    });
}
