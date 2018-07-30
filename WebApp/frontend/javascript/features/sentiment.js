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
	if (_mainFrame.Ts.System.Organization.UseWatson) {
		window.parent.Ts.Services.Customers.GetOrganizationSentiment(organizationID, function (e) {
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
	} else {
		$("#health-properties").addClass("hide");
	}
}

function WatsonTicketField(ticketid) {
	if (_mainFrame.Ts.System.Organization.UseWatson) {
		window.parent.Ts.Services.WatsonTickets.Ticket(ticketid, function (result) {
        if (result != 'negative' && result != 'nothing' && result != 'hidden') {
            var data = jQuery.parseJSON(result);

            var percentage = data.TicketSentimentScore / 1000;
            var reverse = 1 - percentage;
            var display = parseInt(percentage * 100);

            var output = [];
            output.push(data.TicketSentimentScore + " - ");
            // output.push("<strong>Emotions</strong><br>");
            if (data.Sad) output.push("Sad<br>");
            if (data.Frustrated) output.push("Frustrated<br>");
            if (data.Satisfied) output.push("Satisfied<br>");
            if (data.Excited) output.push("Excited<br>");
            if (data.Polite) output.push("Polite<br>");
            if (data.Impolite) output.push("Impolite<br>");
            if (data.Sympathetic) output.push("Sympathetic<br>");
            $('#ticket-Sentiment').append(output.join(' '));
        }
    });
	} else {
		$("#health-properties").addClass("hide");
	}
}




$(document).on('click', '#ticket-Sentiment', function(e) {
    teamsupport.modals.overlay.show();
    $('#modal').html(Handlebars.templates['chart']).css({ 'width':'740px' });
    teamsupport.modals.modal.show('#modal');

    var c = document.getElementById("myCanvas");
    var ctx = c.getContext("2d");

    // CENTER LINES.

    ctx.beginPath();
    ctx.moveTo(200, 200);
    ctx.lineTo(350, 200);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#7ad714";
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(350, 200);
    ctx.lineTo(500, 200);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#7ad714";
    ctx.stroke();

   // BRANCHES LINES, START @ TOP LEFT.

    ctx.beginPath();
    ctx.moveTo(100, 100);
    ctx.lineTo(200, 200);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#7ad714";
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(500, 200);
    ctx.lineTo(600, 100);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#ec663c";
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(500, 200);
    ctx.lineTo(600, 300);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#ec663c";
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(100, 300);
    ctx.lineTo(200, 200);
    ctx.lineWidth=5;
    ctx.strokeStyle = "#7ad714";
    ctx.stroke();

    // CENTER CIRCLE.

    ctx.beginPath();
    ctx.arc(350, 200, 50, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(122,215,20,0.5)";
    ctx.lineWidth=20;
    ctx.stroke();
    ctx.fillStyle = "#7ad714";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Satisfied",350,290);

    ctx.beginPath();
    ctx.font = "16px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "#7ad714";
    ctx.fillText("78 / 100",350,315);

    // LEFT CIRCLE.

    ctx.beginPath();
    ctx.arc(200, 200, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(122,215,20,0.5)";
    ctx.lineWidth=10;
    ctx.stroke();
    ctx.fillStyle = "#7ad714";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("+",200,199);

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "left";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Polite",180,250);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "left";
    ctx.fillStyle = "#7ad714";
    ctx.fillText("78 / 100",180,270);

    // RIGHT CIRCLE.

    ctx.beginPath();
    ctx.arc(500, 200, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(229,99,66,0.5)";
    ctx.lineWidth=10;
    ctx.stroke();
    ctx.fillStyle = "#ec663c";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("-",500,199);

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "right";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Frustrated",520,250);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "right";
    ctx.fillStyle = "#ec663c";
    ctx.fillText("78 / 100",520,270);



    // BRANCH CIRCLES, START @ TOP LEFT.


    // BRANCH CIRCLE, TOP LEFT.

    ctx.beginPath();
    ctx.arc(100, 100, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(122,215,20,0.5)";
    ctx.lineWidth=10;
    ctx.stroke();
    ctx.fillStyle = "#7ad714";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Giddy",100,150);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "#7ad714";
    ctx.fillText("78 / 100",100,170);

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("+",100,99);

    // BRANCH CIRCLE, TOP RIGHT.

    ctx.beginPath();
    ctx.arc(600, 100, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(229,99,66,0.5)";
    ctx.lineWidth=10;
    ctx.stroke();
    ctx.fillStyle = "#ec663c";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Pissed",600,150);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "#ec663c";
    ctx.fillText("78 / 100",600,170);

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("-",600,99);

    // BRANCH CIRCLE, BOTTOM RIGHT.

    ctx.beginPath();
    ctx.arc(600, 300, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(229,99,66,0.5)";
    ctx.lineWidth = 10;
    ctx.stroke();
    ctx.fillStyle = "#ec663c";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Angry",600,350);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "#ec663c";
    ctx.fillText("78 / 100",600,370);

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("-",600,299);

    // BRANCH CIRCLE, BOTTOM LEFT.

    ctx.beginPath();
    ctx.arc(100, 300, 20, 0, 2 * Math.PI);
    ctx.strokeStyle = "rgba(122,215,20,0.5)";
    ctx.lineWidth=10;
    ctx.stroke();
    ctx.fillStyle = "#7ad714";
    ctx.fill();

    ctx.beginPath();
    ctx.font = "14px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "rgba(0,0,0,0.8)";
    ctx.fillText("Happy",100,350);

    ctx.beginPath();
    ctx.font = "12px Verdana";
    ctx.textAlign = "center";
    ctx.fillStyle = "#7ad714";
    ctx.fillText("78 / 100",100,370);

    ctx.beginPath();
    ctx.font = "20px Verdana";
    ctx.textAlign = "center";
    ctx.textBaseline = "middle";
    ctx.fillStyle = "rgba(255,255,255,0.8)";
    ctx.fillText("+",100,299);

});
