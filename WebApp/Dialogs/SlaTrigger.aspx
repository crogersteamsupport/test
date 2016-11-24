<%@ Page Title="SLA Trigger" Language="C#" MasterPageFile="~/Dialogs/Dialog.master" AutoEventWireup="true" CodeFile="SlaTrigger.aspx.cs" Inherits="Dialogs_SlaTrigger" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <style type="text/css">
    .col1{ text-align:right; padding-right: 5px;}

    .HiddenPicker td
    {
	    padding:0 !important;
    }

    .HiddenTextBox
    {
	    width:1px !important;
	    border:0 !important;
	    margin:0 !important;
	    background:none transparent !important;
	    visibility:hidden !important;
        display: none;
    }

    .SlaStartEnd
    {
        width: 40% !important;
    }

    .daysToPause
    {
        vertical-align: top !important;
    }
  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="dialogContentWrapperDiv">
    <div class="dialogContentDiv">
      <table cellpadding="0" cellspacing="5" border="0">
        <tr>
          <td class="col1">SLA Name:</td><td><asp:Label ID="lblSla" runat="server" Text="Label" Font-Bold="true"></asp:Label></td>
        </tr>
        <tr><td class="col1">Ticket Type:</td><td><asp:Label ID="lblTicketType" runat="server" Text="Label" Font-Bold="true"></asp:Label></td></tr>
        <tr><td class="col1">Ticket Severity:</td><td colspan="3"><telerik:RadComboBox ID="cmbSeverities" runat="server"></telerik:RadComboBox></td></tr>
        <tr><td class="col1">Warning Time Before Violation:</td><td>
          <telerik:RadNumericTextBox ID="numWarning" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbWarningMin" runat="server" Text="Minutes" GroupName="WarningTime" />
            <asp:RadioButton ID="rbWarningHour" runat="server" Text="Hours" GroupName="WarningTime" Checked="true"/>
            <asp:RadioButton ID="rbWarningDay" runat="server" Text="Days" GroupName="WarningTime"/></td></tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Pause</div></td></tr>    
        <tr>
          <td class="col1">Pause on Company Holidays</td><td>
            <asp:CheckBox ID="cbPauseOnOrganizationHolidays" runat="server" /></td>
        </tr>
        <tr>
            <td class="col1" style="vertical-align: top;">Pause on Specific Dates</td>
        </tr>
        <tr>
            <td class="col1"><asp:ListBox ID="daysToPauseList" runat="server" Rows="6" Width="100px" SelectionMode="Multiple" CssClass="daysToPause"></asp:ListBox>
                <input type="hidden" runat="server" id="DaysToPauseHidden" name="DaysToPayseHidden" />
                <telerik:RadDatePicker ID="PauseOnDates" runat="server" Width="1px" CssClass="HiddenPicker">
                    <ClientEvents OnDateSelected="AddToList" />
                    <DateInput runat="server" CssClass="HiddenTextBox" />
                </telerik:RadDatePicker>
            </td>
        </tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Business Hours</div></td></tr>    
        <tr>
            <td class="col1">Use Account Business Hours:</td>
            <td><asp:RadioButton ID="rbBusinessHours" runat="server" Text="" GroupName="BusinessHours" OnClick="DisableSlaDaysAndHours()" /></td>
        </tr>
        <tr>
            <td class="col1">Use Custom Business Hours:</td>
            <td><asp:RadioButton ID="rbCustomBusinessHours" runat="server" Text="" GroupName="BusinessHours" OnClick="DisableSlaDaysAndHours()" /></td>
        </tr>
        <tr>
            <td class="col1">SLA Start:</td>
            <td>
                <telerik:RadTimePicker ID="timeSLAStart" runat="server" Width="90px">
                </telerik:RadTimePicker>
            </td>
        </tr>
        <tr>
            <td class="col1">SLA End:</td>
            <td>
                <telerik:RadTimePicker ID="timeSLAEnd" runat="server" Width="90px">
                </telerik:RadTimePicker>
            </td>
        </tr>
        <tr>
            <td class="col1">Time Zone:</td>
            <td>
                <telerik:RadComboBox ID="cbTimeZones" runat="server" Width="200px"></telerik:RadComboBox>
            </td>
        </tr>
            
        <tr>
            <td class="col1">SLA Days:</td>
            <td colspan="4">
                <asp:CheckBox ID="cbSLAMonday" runat="server" Text="Monday" />
                <asp:CheckBox ID="cbSLATuesday" runat="server" Text="Tuesday" />
                <asp:CheckBox ID="cbSLAWednesday" runat="server" Text="Wednesday" />
                <asp:CheckBox ID="cbSLAThursday" runat="server" Text="Thursday" />
            </td>
        </tr>
        <tr>
            <td class="col1"></td>
            <td>
                <asp:CheckBox ID="cbSLAFriday" runat="server" Text="Friday" />
                <asp:CheckBox ID="cbSLASaturday" runat="server" Text="Saturday" />
                <asp:CheckBox ID="cbSLASunday" runat="server" Text="Sunday" />
            </td>
        </tr>

        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Violation Times</div></td></tr>    
        
        <tr><td class="col1">Time Since Initial Response:</td><td>
          <telerik:RadNumericTextBox ID="numInitial" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbInitialMin" runat="server" Text="Minutes" GroupName="IntialTime" />
            <asp:RadioButton ID="rbInitialHour" runat="server" Text="Hours" GroupName="IntialTime" Checked="true"/>
            <asp:RadioButton ID="rbInitialDay" runat="server" Text="Days" GroupName="IntialTime"/></td></tr>
        <tr><td class="col1">Time Since Last Action:</td><td>
          <telerik:RadNumericTextBox ID="numAction" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbActionMin" runat="server" Text="Minutes" GroupName="LastAction" />
            <asp:RadioButton ID="rbActionHour" runat="server" Text="Hours" GroupName="LastAction" Checked="true"/>
            <asp:RadioButton ID="rbActionDay" runat="server" Text="Days" GroupName="LastAction"/></td></tr>
        <tr><td class="col1">Time Until Closed:</td><td>
          <telerik:RadNumericTextBox ID="numClosed" Width="50px" runat="server" 
            DataType="System.Int32" MinValue="0" Value="0">
            <NumberFormat DecimalDigits="0" />
          </telerik:RadNumericTextBox>
            <asp:RadioButton ID="rbClosedMin" runat="server" Text="Minutes" GroupName="TimeClosed" />
            <asp:RadioButton ID="rbClosedHour" runat="server" Text="Hours" GroupName="TimeClosed" Checked="true"/>
            <asp:RadioButton ID="rbClosedDay" runat="server" Text="Days" GroupName="TimeClosed"/></td></tr>
            
        <tr><td colspan="4"><div style="border-bottom: solid 1px; font-size: 16px; margin-bottom: 7px; padding-top: 6px;">Notification Options</div></td></tr>    
            
      </table>
      
      <table cellpadding="0" cellspacing="5" border="0" width="500px">
      <tr>
      <td><asp:CheckBox ID="cbGroupWarnings" runat="server" Text="Notify Group of Warnings" /></td>
      <td><asp:CheckBox ID="cbUserWarnings" runat="server" Text="Notify User of Warnings" /></td>
      </tr>
      <tr>
      <td><asp:CheckBox ID="cbGroupViolations" runat="server" Text="Notify Group of Violations" /></td>
      <td><asp:CheckBox ID="cbUserViolations" runat="server" Text="Notify User of Violations" /></td>
      </tr>
      </table>
      
    </div>
  </div>

    <script type="text/javascript">
        function AddToList()
        {
            var datePicker = $find("<%=PauseOnDates.ClientID %>");
            var dateText = datePicker.get_dateInput().get_selectedDate().localeFormat(parent.Ts.Utils.getDatePattern());
            var htmlSelect = document.getElementById('<%=daysToPauseList.ClientID%>');
            var option = document.createElement("OPTION");
            option.innerHTML = dateText;
            option.value = dateText;
            htmlSelect.appendChild(option);
            AddToHidden(dateText);
        }

        function AddToHidden(dateText) {
            var daysToPauseHidden = document.getElementById('<%=DaysToPauseHidden.ClientID%>');

            if (daysToPauseHidden.value == null || daysToPauseHidden.value == '') {
                daysToPauseHidden.value = dateText;
            } else {
                daysToPauseHidden.value += "," + dateText;
            }
        }

        function DeleteSelectedItems(event) {
            if (event.keyCode == 46 || event.key == "Delete") {
                var htmlSelect = document.getElementById('<%=daysToPauseList.ClientID%>');

                for (var i = (htmlSelect.options.length - 1) ; i >= 0; i--) {
                    if (htmlSelect.options[i].selected) {
                        RemoveOfHidden(htmlSelect.options[i].value);
                        htmlSelect.options[i] = null;
                    }
                }
            }
        }

        function RemoveOfHidden(dateText) {
            var daysToPauseHidden = document.getElementById('<%=DaysToPauseHidden.ClientID%>');

            if (daysToPauseHidden.value != null && daysToPauseHidden.value !== '') {
                daysToPauseHidden.value = daysToPauseHidden.value.replace(dateText, "");
                daysToPauseHidden.value = daysToPauseHidden.value.replace(",,", ",");
            }
        }

        function DisableSlaDaysAndHours() {
            var useOrgBusinessHours = document.getElementById('<%= rbBusinessHours.ClientID %>');
            var timeSLAStart = $find("<%= timeSLAStart.ClientID %>");
            var timeSLAEnd = $find("<%= timeSLAEnd.ClientID %>");
            timeSLAStart.set_enabled(!useOrgBusinessHours.checked);
            timeSLAEnd.set_enabled(!useOrgBusinessHours.checked);
            var timeZones = $find("<%= cbTimeZones.ClientID %>")

            if (useOrgBusinessHours.checked) {
                timeZones.disable();
            } else {
                timeZones.enable();
            }

            var cbSLASunday = document.getElementById('<%= cbSLASunday.ClientID %>');
            cbSLASunday.disabled = useOrgBusinessHours.checked;
            var cbSLAMonday = document.getElementById('<%= cbSLAMonday.ClientID %>');
            cbSLAMonday.disabled = useOrgBusinessHours.checked;
            var cbSLATuesday = document.getElementById('<%= cbSLATuesday.ClientID %>');
            cbSLATuesday.disabled = useOrgBusinessHours.checked;
            var cbSLAWednesday = document.getElementById('<%= cbSLAWednesday.ClientID %>');
            cbSLAWednesday.disabled = useOrgBusinessHours.checked;
            var cbSLAThursday = document.getElementById('<%= cbSLAThursday.ClientID %>');
            cbSLAThursday.disabled = useOrgBusinessHours.checked;
            var cbSLAFriday = document.getElementById('<%= cbSLAFriday.ClientID %>');
            cbSLAFriday.disabled = useOrgBusinessHours.checked;
            var cbSLASaturday = document.getElementById('<%= cbSLASaturday.ClientID %>');
            cbSLASaturday.disabled = useOrgBusinessHours.checked;
        }
    </script>
</asp:Content>