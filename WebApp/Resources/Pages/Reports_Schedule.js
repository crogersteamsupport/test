$(document).ready(function () {
	var _isEditing = false;
	var _reportIdToSchedule = parent.Ts.Utils.getQueryValue('ReportId', window);
	var _reportIdToScheduleName = parent.Ts.Utils.getQueryValue('ReportName', window);
	var _editingScheduledId = parent.Ts.Utils.getQueryValue('ScheduledReportId', window);
	var _returnTo = parent.Ts.Utils.getQueryValue('ReturnTo', window);

	if (_editingScheduledId != undefined
		&& _editingScheduledId != null
		&& _editingScheduledId != "") {
		_isEditing = true;
	}

	OnLoadSetup();

	$('.action-save').click(function (e) {
		e.preventDefault();

		var currentSection = $('.schedule-panel-body');
		var validation = ValidateSection(currentSection);

		if (validation != null) {
			if (validation == "") {
				return;
			}
			else {
				alert(validation);
				return;
			}
		}

		var emailSubject = $('.schedule-email-subject').val();
		var emailBody = $('.schedule-email-body').val();
		var emailAddresses = $('.schedule-email-addresses').val();
		var startOn = $('#StartDateTimePicker').val();
		var startOnValue = '';
		startOnValue = parent.Ts.Utils.getMsDate(startOn);
		var frequency = $('.frequencyList').val();
		var every = $('.numberList').val();
		var weekday = (($('.frequencyList').val() == 1) ? $('#weeksOn').val() : $('#dayOfMonth').val());
		var dayOfMonth = $('.monthDayList').val() || 0;
		var isActive = false;

		//3: Once
		if (frequency == 3) {
			every = 1;
			weekday = 0;
			dayOfMonth = 0;
		}

		if ($("#active").hasClass('fa-check-square-o')) {
			isActive = true;
		}

		parent.Ts.Services.Reports.SaveScheduledReport(_editingScheduledId || 0,
													_reportIdToSchedule || 0,
													emailSubject,
													emailBody,
													emailAddresses,
													startOnValue,
													frequency,
													every,
													weekday,
													dayOfMonth,
													isActive,
													BackToList(true));
	});

	$('.action-cancel').click(function (e) {
		e.preventDefault();
		BackToList(false);
	});

	$('.frequencyList').change(function (e) {
		e.preventDefault();
		
		switch (this.value)
		{
			case "1":
				SetWeeklyOptions();
				break;
			case "2":
				SetMonthlyOptions();
				break;
			case "3":
				SetOnceOptions();
				break;
		}
	});

	$('.monthDayList').change(function (e) {
		e.preventDefault();
		var onlyDayEntry = this.value > 4;

		PopulateWeekDayList(onlyDayEntry, false);
	});

	$(".schedule-email-subject").blur(function () {
		$(this).popover('hide').parent('.schedule-cond').removeClass('has-error');
	});

	$(".schedule-email-body").blur(function () {
		$(this).popover('hide').parent('.schedule-cond').removeClass('has-error');
	});

	$(".schedule-email-addresses").blur(function () {
		$(this).popover('hide').parent('.schedule-cond').removeClass('has-error');
	});

	$("#active").click(function (e) {
		e.preventDefault();
		var check = $(this);

		if (check.hasClass('fa-square-o')) {
			check.removeClass('fa-square-o').addClass('fa-check-square-o');
		} else {
			check.removeClass('fa-check-square-o').addClass('fa-square-o');
		}
	});

	function ValidateSection(el) {

		if ($('.schedule-email-subject').val() == '') {
			$('.schedule-email-subject').popover('show').parent('.schedule-cond').addClass('has-error');
			$('.schedule-email-subject').focus();
			return false;
		}
		else {
			$('.schedule-email-subject').popover('hide').parent('.schedule-cond').removeClass('has-error');
		}

		if ($('.schedule-email-body').val() == '') {
			$('.schedule-email-body').popover('show').parent('.schedule-cond').addClass('has-error');
			$('.schedule-email-body').focus();
			return false;
		}
		else {
			$('.schedule-email-body').popover('hide').parent('.schedule-cond').removeClass('has-error');
		}

		if ($('.schedule-email-addresses').val() == '') {
			$('.schedule-email-addresses').popover('show').parent('.schedule-cond').addClass('has-error');
			$('.schedule-email-addresses').focus();
			return false;
		}
		else {
			$('.schedule-email-addresses').popover('hide').parent('.schedule-cond').removeClass('has-error');
		}

		return null;
	}

	function OnLoadSetup() {
		$('#ReportTitleSpan').text(_reportIdToScheduleName);

		if (_isEditing) {
			parent.Ts.Services.Reports.GetScheduledReport(_editingScheduledId, LoadReportData);
		} else {
			SetScheduleOptions(1);

			var dateNow = new Date();
			dateNow.setHours(dateNow.getHours() + 1);

			$('#StartDateTimePicker').datetimepicker({
				defaultDate: dateNow
			});
		}

		PopulateFrequencyList();
	}

	function LoadReportData(report) {
		SetScheduleOptions(report.RecurrencyId);
		$('.schedule-email-subject').val(report.EmailSubject);
		$('.schedule-email-body').val(report.EmailBody);
		$('.schedule-email-addresses').val(report.EmailRecipients);
		var startOn = report.StartDate.localeFormat(parent.Ts.Utils.getDateTimePattern());
		$('#StartDateTimePicker').val(startOn);
		$('#StartDateTimePicker').datetimepicker({
			setDate: startOn,
			pickTime: true
		});
		$('.frequencyList').val(report.RecurrencyId);
		$('.numberList').val(report.Every);
		$('#weeksOn').val(report.Weekday);
		$('#dayOfMonth').val(report.Weekday || 0);
		$('.monthDayList').val(report.Monthday);

		if (report.IsActive) {
			$("#active").removeClass('fa-square-o').addClass('fa-check-square-o');
		} else {
			$("#active").removeClass('fa-check-square-o').addClass('fa-square-o');
		}
	}

	function BackToList(isNew) {
		var returnToOption = "";

		if (isNew) {
			_returnTo = "ScheduledReports";
		}

		if (_returnTo != undefined && _returnTo != null && _returnTo != "") {
			returnToOption = "?ReturnFrom=" + _returnTo;
		}

		var result = '/vcr/1_9_0/pages/reports.html' + returnToOption;
		location.assign(result);
	};

	function SetScheduleOptions(frequency) {
		switch (frequency) {
			case 1:
				SetWeeklyOptions();
				break;
			case 2:
				SetMonthlyOptions();
				break;
			case 3:
				SetOnceOptions();
				break;
		}
	}

	function SetWeeklyOptions() {
		$('#Every').show();
		$('#WeeklyOptions').show();
		$('#MonthlyOptions').hide();
		$('#WeekdayList').hide();
		PopulateWeekDayList(false, true);
	}

	function SetMonthlyOptions() {
		$('#Every').show();
		$('#WeeklyOptions').hide();
		$('#MonthlyOptions').show();
		$('#WeekdayList').show();
		PopulateWeekDayList(false, false);
		PopulateMonthDayList();
	}

	function SetOnceOptions() {
		$('#Every').hide();
		$('#WeeklyOptions').hide();
		$('#MonthlyOptions').hide();
		$('#WeekdayList').hide();
	}

	function PopulateFrequencyList() {
		$('.frequencyList').empty();
		var selectValues = { 3: "Once", 1: "Weekly", 2: "Monthly" };

		$.each(selectValues, function (key, value) {
			$('.frequencyList')
				.append($("<option></option>")
						   .attr("value", key)
						   .text(value));
		});
	}

	function PopulateWeekDayList(onlyDayEntry, onlyWeekDays) {
		$('.weekdayList').empty();
		var includeDivider = true;

		var selectValues = {
			"0": "Day",
			"1": "Sunday",
			"2": "Monday",
			"3": "Tuesday",
			"4": "Wednesday",
			"5": "Thursday",
			"6": "Friday",
			"7": "Saturday"
		};

		if (onlyDayEntry) {
			selectValues = {
				"0": "Day"
			};

			includeDivider = false;
		}

		if (onlyWeekDays) {
			selectValues = {
				"1": "Sunday",
				"2": "Monday",
				"3": "Tuesday",
				"4": "Wednesday",
				"5": "Thursday",
				"6": "Friday",
				"7": "Saturday"
			};

			includeDivider = false;
		}

		$.each(selectValues, function (key, value) {
			if (includeDivider && key == 1) {
				$('.weekdayList')
							.append($("<option disabled>──────────</option>"));
			}

			$('.weekdayList')
							.append($("<option></option>")
									   .attr("value", key)
									   .text(value));
		});
	}

	function PopulateMonthDayList() {
		$('.monthDayList').empty();
		var selectValues = {
			"1": "1st",
			"2": "2nd",
			"3": "3rd",
			"4": "4th",
			"5": "5th",
			"6": "6th",
			"7": "7th",
			"8": "8th",
			"9": "9th",
			"10": "10th",
			"11": "11th",
			"12": "12th",
			"13": "13th",
			"14": "14th",
			"15": "15th",
			"16": "16th",
			"17": "17th",
			"18": "18th",
			"19": "19th",
			"20": "20th",
			"21": "21st",
			"22": "22nd",
			"23": "23rd",
			"24": "24th",
			"25": "25th",
			"26": "26th",
			"27": "27th",
			"28": "28th",
			"29": "29th",
			"30": "30th",
			"31": "31st",
			"99": "Last"
		};

		$.each(selectValues, function (key, value) {
			$('.monthDayList')
				.append($("<option></option>")
						   .attr("value", key)
						   .text(value));
		});
	}
});