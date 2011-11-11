/// <reference path="ts/ts.services.js" />
/// <reference path="ts/ts.system.js" />
/// <reference path="ts/ts.utils.js" />
/// <reference path="~/Default.aspx" />
/// <reference path="ts/ts.pages.main.js" />

$(document).ready(function () {
  $('button').button();
  $('.refresh').click(function (e) {
    e.preventDefault();
    location = location;
  });
  $('.page-loading').hide().next().show();

  //$('.reminders').hide();

  top.Ts.Services.System.GetUserReminders(top.Ts.Utils.getQueryValue('userid', window), function (reminders) {
    for (var i = 0; i < reminders.length; i++) {
      var reminder = reminders[i];
      var reminderDiv = $('<div>')
        .addClass('reminder')
        .data('o', reminder)
        .appendTo('.reminders');

      $('<input>')
        .attr('type', 'checkbox')
        .appendTo(reminderDiv);
      $('<span>')
        .html(reminder.DueDate + '&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp')
        .appendTo(reminderDiv);
      $('<span>')
        .text((reminder.DueDate.getTime() - (new Date()).getTime())/ (1000 * 60 * 60))
        .appendTo(reminderDiv);
    }

    function getWeekNumber(d) {
      // Copy date so don't modify original
      d = new Date(d);
      // Set to nearest Thursday: current date + 4 - current day number
      // Make Sunday's day number 7
      d.setDate(d.getDate() + 4 - (d.getDay() || 7));
      // Get first day of year
      var yearStart = new Date(d.getFullYear(), 0, 1);
      // Calculate full weeks to nearest Thursday
      var weekNo = Math.ceil((((d - yearStart) / 86400000) + 1) / 7)
      // Return array of year and week number
      return [d.getFullYear(), weekNo];
    }
  });

  function getReminderDiv(date) { 
  
  
  }
});




