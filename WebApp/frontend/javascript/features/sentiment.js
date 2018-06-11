function getColor(value){
    //value from 0 to 1
    var hue=((1-value)*120).toString(10);
    return ["hsl(",hue,",100%,50%)"].join("");
}

function WatsonTicket(ticketid) {
    window.parent.Ts.Services.TicketPage.WatsonTicket(ticketid, function(result) {
        console.log(result);
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);
            var color = getColor(data.TicketSentimentScor);
            console.log(data.TicketSentimentScore + ' / ' + color);
        } else {
            var color = getColor(0.100);
            console.log(color);
        }

        $('#ticketSentiment').css({ 'color':color });
    });
}
