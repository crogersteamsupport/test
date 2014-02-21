using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ContactsViewItem
  {
  }
  
  public partial class ContactsView
  {
    public void LoadByParentOrganizationID(int organizationID, string orderBy = "LastName, FirstName", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        command.CommandText = "SELECT " + limit + " * FROM ContactsView WHERE OrganizationParentID = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void LoadByOrganizationID(int organizationID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ContactsView WHERE OrganizationID = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
    public void LoadByTicketID(int ticketID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public ContactsViewItem FindBySalesForceID(string salesForceID, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.SalesForceID != null && contactsViewItem.SalesForceID.Trim().ToLower() == salesForceID.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public ContactsViewItem FindByEmail(string email, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.Email.Trim().ToLower() == email.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public void LoadByTicketIDOrderedByDateCreated(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadSentToSalesForce(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.SentToSalesForce = 1 AND ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE cv.NeedsIndexing = 1
        AND cv.MarkDeleted = 0
        AND o.ParentID = @OrganizationID
        ORDER BY cv.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE o.ParentID = @OrganizationID
        AND cv.MarkDeleted = 0
        ORDER BY cv.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }

  public class CustomerSearchContact
  {
    public CustomerSearchContact() { }
    public CustomerSearchContact(ContactsViewItem item)
    {
      userID = item.UserID;
      organizationID = item.OrganizationID;
      email = item.Email;
      title = item.Title;
      organization = item.Organization;
      fName = item.FirstName;
      lName = item.LastName;
      isPortal = item.IsPortalUser;
    }

    public int userID { get; set; }
    public int organizationID { get; set; }
    public string email { get; set; }
    public string title { get; set; }
    public string organization { get; set; }
    public string fName { get; set; }
    public string lName { get; set; }
    public bool isPortal { get; set; }
    public int openTicketCount { get; set; }
    public CustomerSearchPhone[] phones { get; set; }
  }
  
}
