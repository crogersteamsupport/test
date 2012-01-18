using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
  public class ReminderProcessor : ServiceThread
  {
    private Logs logs;

    public override void Run()
    {
      try
      {
        Reminders reminders = new Reminders(LoginUser);
        reminders.LoadForEmail();
        foreach (Reminder reminder in reminders)
        {
          ProcessReminder(reminder);
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "ReminderProcessor"); 
      }
    }

    public override string ServiceName
    {
      get { return "ReminderProcessor"; }
    }

    private void ProcessReminder(Reminder reminder)
    { 
      MailMessage message;
      UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, reminder.UserID);

      switch (reminder.RefType)
      {
        case ReferenceType.Organizations:
          OrganizationsViewItem org = OrganizationsView.GetOrganizationsViewItem(LoginUser, reminder.RefID);
          if (org == null) return;
          message = EmailTemplates.GetReminderCustomerEmail(LoginUser, reminder, user, org);
          break;
        case ReferenceType.Tickets:
          TicketsViewItem ticket = TicketsView.GetTicketsViewItem(LoginUser, reminder.RefID);
          if (ticket == null) return;
          message = EmailTemplates.GetReminderTicketEmail(LoginUser, reminder, user, ticket);
          break;
        case ReferenceType.Contacts:
          ContactsViewItem contact = ContactsView.GetContactsViewItem(LoginUser, reminder.RefID);
          if (contact == null) return;
          message = EmailTemplates.GetReminderContactEmail(LoginUser, reminder, user, contact);
          break;
        default:
          message = null;
          break;
      }
      
      if (message == null) return;

      message.To.Add(new MailAddress(user.Email, user.FirstName + ' ' + user.LastName ));

      Emails.AddEmail(LoginUser, reminder.OrganizationID, null, message.Subject, message);
      reminder.HasEmailSent = true;
      reminder.Collection.Save();
    }


  }
}
