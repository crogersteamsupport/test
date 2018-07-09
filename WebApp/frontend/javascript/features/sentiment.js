var sentiments = {
    1: 'Sad',
    2: 'Frustrated',
    3: 'Satisfied',
    4: 'Excited',
    5: 'Polite',
    6: 'Impolite',
    7: 'Sympathetic'
}

function getColor(value){
    //value from 0 to 1
    if (value > 0) {
        var hue = ((1 - value) * 120).toString(10);
        return ["hsl(",hue,",100%,70%)"].join("");
    }
}

function WatsonTicket(ticketid) {
    window.parent.Ts.Services.WatsonTickets.Ticket(ticketid, function(result) {
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);
            var percentage = data.TicketSentimentScore / 1000;
            var reverse = 1 - percentage;
            var display = parseInt(percentage * 100);
            var color = getColor(reverse);
            $('#health-ticket').css({ 'background-color':color }).css({ 'text-align':'left' });
            $('#health-meter').css({ 'width':display + 'px' });
            $('#health-message').removeClass('disabled').addClass('enabled').text(display + '%');
        } else {
            var color = getColor(0.100);
        }
    });

    window.parent.Ts.Services.WatsonTickets.Summary(_ticketID, function(r) {
        if (r != 'negative' && r != 'nothing' && r != 'faults') {
            var data = jQuery.parseJSON(r);
            var highest = data.summary[0];
            var emotion = sentiments[highest.SentimentID];
            $('#health-emotions').text(emotion);
        }
    });
}

function WatsonCustomer(organizationID) {
    _mainFrame.Ts.Services.Customers.GetOrganizationSentiment(organizationID, function(e) {
        if (e.length > 0) {
            $('#organizationSentiment').show();
            var percentage = e / 1000;
            var reverse = 1 - percentage;
            var display = parseInt(percentage * 100);
            var color = getColor(reverse);
            $('#health-ticket').css({ 'background-color':color }).css({ 'text-align':'left' });
            $('#health-meter').css({ 'width':display + 'px' });
            $('#health-message').removeClass('disabled').addClass('enabled').text(display + '%');
        }
    });
}

function WatsonTicketField(ticketid) {
    window.parent.Ts.Services.WatsonTickets.Ticket(ticketid, function(result) {
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);
            var display = [];
            display.push(data.TicketSentimentScore + "% - ");
            // display.push("<strong>Emotions</strong><br>");
            if (data.Sad) display.push("Sad<br>");
            if (data.Frustrated) display.push("Frustrated<br>");
            if (data.Satisfied) display.push("Satisfied<br>");
            if (data.Excited) display.push("Excited<br>");
            if (data.Polite) display.push("Polite<br>");
            if (data.Impolite) display.push("Impolite<br>");
            if (data.Sympathetic) display.push("Sympathetic<br>");
            $('#ticket-Sentiment').append(display.join(' '));
        }
    });
}
