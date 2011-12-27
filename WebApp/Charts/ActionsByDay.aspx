﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActionsByDay.aspx.cs" Inherits="Charts_ActionsByDay" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <style type="text/css">
    body { background: #fff; }
    #datepicker {text-align: center; background-color: #ECF3FC;font-family: "Lucida Grande",Helvetica,Arial,Verdana,sans-serif; font-size: 12px; color: #15428B;}
  </style>

  <script type="text/javascript" src="https://www.google.com/jsapi"></script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <% if (VariableDateRange)
     { %>
     <div id="datepicker">
        <br />
                Start Date:
        <telerik:RadDateTimePicker ID="StartDatePick" Runat="server" Skin="Outlook">
            <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>
            <TimeView CellSpacing="-1"></TimeView>
            <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" 
                            ViewSelectorText="x" Skin="Outlook"></Calendar>
            <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
        </telerik:RadDateTimePicker>
        &nbsp;
        End Date:<telerik:RadDateTimePicker ID="EndDatePick" Runat="server" Skin="Outlook">
            <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>
            <TimeView CellSpacing="-1"></TimeView>
            <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" 
                            ViewSelectorText="x" Skin="Outlook"></Calendar>
            <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
        </telerik:RadDateTimePicker>
        <br />
        <asp:Button ID="Button1" runat="server" Text="Generate Graph" OnClick="Button1_Click" />
        <br /><br />
    </div>
  <%} %>
  <div>
    <div id="chart_div">
    </div>
  </div>
  </form>
</body>

<% if (!VariableDateRange || IsPostBack)
   { %>
  <script type="text/javascript">
      if (google != null) {
          google.load("visualization", "1", { packages: ["corechart"] });
          google.setOnLoadCallback(drawChart);
      }

      function drawChart() {
          PageMethods.GetData('<%=StartDate.HasValue ? StartDate : DateTime.Today.AddDays(DateRangeMin) %>','<%=EndDate.HasValue ? EndDate : DateTime.Today %>',function (result) {
              var data = new google.visualization.DataTable();
              data.addColumn('string', 'Date');
              data.addColumn('number', 'Actions Added');
              data.addRows(result.length);
              for (var i = 0; i < result.length; i++) {
                  data.setValue(i, 0, result[i].ActionDate);
                  data.setValue(i, 1, result[i].Count);
              }

              var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
              chart.draw(data, { width: 260, height: 250, titleFontSize: 0, vAxis: { title: 'Number of Actions Added'}, legend: 'none' });
          });
      }
    
  </script>
  <%} %>
</html>
