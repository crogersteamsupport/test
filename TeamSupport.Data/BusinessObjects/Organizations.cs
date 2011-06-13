using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  public partial class Organization 
  {
    public OrganizationsViewItem GetOrganizationView()
    {
      return OrganizationsView.GetOrganizationsViewItem(BaseCollection.LoginUser, OrganizationID);
    }

    public string GetReplyToAddress()
    {
      if (string.IsNullOrEmpty(OrganizationReplyToAddress))
        return SystemEmailID.ToString() + "@teamsupport.com";
      return OrganizationReplyToAddress;
    }

    public bool IsDayInBusinessHours(DayOfWeek dayOfWeek)
    {
      if (BusinessDays == null) return false;
      return ((int)BusinessDays & (int)Math.Pow(2, (int)dayOfWeek)) > 0;
    }

    public string BusinessDaysText
    {
      get 
      {
        StringBuilder days = new StringBuilder();
        foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
        {
          if (IsDayInBusinessHours(dayOfWeek))
          {
            if (days.Length > 0) days.Append(", ");
            days.Append(Enum.GetName(typeof(DayOfWeek), dayOfWeek));
          }
        }
        return days.ToString();
      } 
    }

    public void ClearBusinessDays()
    {
      BusinessDays = 0;
    }

    public void AddBusinessDay(DayOfWeek dayOfWeek)
    { 
      BusinessDays = BusinessDays | (int)Math.Pow(2, (int)dayOfWeek);
    
    }
  }

  public partial class Organizations 
  {
    
    public static void CreateStandardData(LoginUser loginUser, Organization organization, bool createTypes, bool createWorkflow)
    {
    
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByOrganizationID(loginUser.OrganizationID);
      if (organizations.IsEmpty || (organizations[0].ParentID != 1 && organizations[0].ParentID != null)) return;
      
      TicketTypes typesTest = new TicketTypes(loginUser);
      typesTest.LoadByOrganizationID(organization.OrganizationID, organization.ProductType);
      if (!typesTest.IsEmpty) return;

      if (createTypes)
      {
        #region Product Version Statuses
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(loginUser);
      ProductVersionStatus productVersionStatus;
      
      productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
      productVersionStatus.Name = "Alpha";
      productVersionStatus.Description = "In house release";
      productVersionStatus.IsDiscontinued = false;
      productVersionStatus.IsShipping = false;
      productVersionStatus.OrganizationID = organization.OrganizationID;
      productVersionStatus.Position = 0;

      productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
      productVersionStatus.Name = "Beta";
      productVersionStatus.Description = "External test release";
      productVersionStatus.IsDiscontinued = false;
      productVersionStatus.IsShipping = false;
      productVersionStatus.OrganizationID = organization.OrganizationID;
      productVersionStatus.Position = 1;

      productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
      productVersionStatus.Name = "Released";
      productVersionStatus.Description = "Live customer use";
      productVersionStatus.IsDiscontinued = false;
      productVersionStatus.IsShipping = true;
      productVersionStatus.OrganizationID = organization.OrganizationID;
      productVersionStatus.Position = 2;

      productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
      productVersionStatus.Name = "Discontinued";
      productVersionStatus.Description = "";
      productVersionStatus.IsDiscontinued = true;
      productVersionStatus.IsShipping = false;
      productVersionStatus.OrganizationID = organization.OrganizationID;
      productVersionStatus.Position = 3;
      productVersionStatuses.Save();
      #endregion
      }

      if (createTypes)
      {
        #region Action Types
      ActionTypes actionTypes = new ActionTypes(loginUser);
      ActionType actionType;

      actionType = actionTypes.AddNewActionType();
      actionType.Name = "Comment";
      actionType.Description = "Add comments for a ticket.";
      actionType.OrganizationID = organization.OrganizationID;
      actionType.Position = 0;
      actionType.IsTimed = true;

      actionTypes.Save();

      #endregion
      }

      if (createTypes)
      {
        #region Phone Types
      PhoneTypes phoneTypes = new PhoneTypes(loginUser);
      PhoneType phoneType;

      phoneType = phoneTypes.AddNewPhoneType();
      phoneType.Name = "Work";
      phoneType.Description = "Work";
      phoneType.Position = 0;
      phoneType.OrganizationID = organization.OrganizationID;

      phoneType = phoneTypes.AddNewPhoneType();
      phoneType.Name = "Mobile";
      phoneType.Description = "Mobile";
      phoneType.Position = 1;
      phoneType.OrganizationID = organization.OrganizationID;

      phoneType = phoneTypes.AddNewPhoneType();
      phoneType.Name = "Home";
      phoneType.Description = "Home";
      phoneType.Position = 2;
      phoneType.OrganizationID = organization.OrganizationID;

      phoneType = phoneTypes.AddNewPhoneType();
      phoneType.Name = "Fax";
      phoneType.Description = "Fax";
      phoneType.Position = 3;
      phoneType.OrganizationID = organization.OrganizationID;

      phoneTypes.Save();
      #endregion
      }

      TicketTypes ticketTypes = new TicketTypes(loginUser);
      TicketStatuses ticketStatuses;
      TicketNextStatuses ticketNextStatuses;
      
      TicketType ticketType;
      TicketStatus ticketStatus;

      #region Support
      ticketStatuses = new TicketStatuses(loginUser);

      ticketType = ticketTypes.AddNewTicketType();
      ticketType.Name = "Support";
      ticketType.Description = "Support";
      ticketType.OrganizationID = organization.OrganizationID;
      ticketType.IconUrl = "Images/TicketTypes/Phone.gif";
      ticketType.Position = 0;
      ticketTypes.Save();

      if (createTypes)
      {

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name= "New";
        ticketStatus.Description = "New";
        ticketStatus.Position = 0;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Under Review";
        ticketStatus.Description = "Under Review";
        ticketStatus.Position = 1;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Need More Info";
        ticketStatus.Description = "Need More Info";
        ticketStatus.Position = 2;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Waiting on Customer";
        ticketStatus.Description = "Waiting on Customer";
        ticketStatus.Position = 3;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Customer Responded";
        ticketStatus.Description = "Customer Responded";
        ticketStatus.IsEmailResponse = true;
        ticketStatus.Position = 4;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false; 
        
        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "On Hold";
        ticketStatus.Description = "On Hold";
        ticketStatus.Position = 5;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Inform Customer";
        ticketStatus.Description = "Inform Customer";
        ticketStatus.Position = 6;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Closed";
        ticketStatus.Description = "Closed";
        ticketStatus.Position = 7;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = true;

        ticketStatuses.Save();
        
        if (createWorkflow)
        {
          ticketNextStatuses = new TicketNextStatuses(loginUser);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);

          //ticketNextStatuses.Save();
        }
      }

      #endregion

      #region Defects
      ticketStatuses = new TicketStatuses(loginUser);

      ticketType = ticketTypes.AddNewTicketType();
      ticketType.Name = "Defects";
      ticketType.Description = "Defects";
      ticketType.OrganizationID = organization.OrganizationID;
      ticketType.IconUrl = "Images/TicketTypes/Bugs.png";
      ticketType.Position = 1;
      ticketTypes.Save();

      if (createTypes)
      {

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "New";
        ticketStatus.Description = "New";
        ticketStatus.Position = 0;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Under Review";
        ticketStatus.Description = "Under Review";
        ticketStatus.Position = 1;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Need More Info";
        ticketStatus.Description = "Need More Info";
        ticketStatus.Position = 2;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Waiting on Customer";
        ticketStatus.Description = "Waiting on Customer";
        ticketStatus.Position = 3;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Customer Responded";
        ticketStatus.Description = "Customer Responded";
        ticketStatus.IsEmailResponse = true;
        ticketStatus.Position = 4;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "On Hold";
        ticketStatus.Description = "On Hold";
        ticketStatus.Position = 5;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;


        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "In Engineering";
        ticketStatus.Description = "In Engineering";
        ticketStatus.Position = 6;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "In QA";
        ticketStatus.Description = "In QA";
        ticketStatus.Position = 7;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Inform Customer";
        ticketStatus.Description = "Inform Customer";
        ticketStatus.Position = 8;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Closed";
        ticketStatus.Description = "Closed";
        ticketStatus.Position = 9;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = true;

        ticketStatuses.Save();

        if (createWorkflow)
        {

          ticketNextStatuses = new TicketNextStatuses(loginUser);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[9], 8);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[9], 8);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[9], 8);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[9], 8);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[8], 8);

          //ticketNextStatuses.Save();
        }
      }

      #endregion

      #region Features
      ticketStatuses = new TicketStatuses(loginUser);

      ticketType = ticketTypes.AddNewTicketType();
      ticketType.Name = "Features";
      ticketType.Description = "Features";
      ticketType.OrganizationID = organization.OrganizationID;
      ticketType.IconUrl = "Images/TicketTypes/Features.png";
      ticketType.Position = 2;
      ticketTypes.Save();

      if (createTypes)
      {
        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "New";
        ticketStatus.Description = "New";
        ticketStatus.Position = 0;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Waiting Approval";
        ticketStatus.Description = "Waiting Approval";
        ticketStatus.Position = 1;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Need More Info";
        ticketStatus.Description = "Need More Info";
        ticketStatus.Position = 2;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Waiting on Customer";
        ticketStatus.Description = "Waiting on Customer";
        ticketStatus.Position = 3;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Customer Responded";
        ticketStatus.Description = "Customer Responded";
        ticketStatus.IsEmailResponse = true;
        ticketStatus.Position = 4;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "On Hold";
        ticketStatus.Description = "On Hold";
        ticketStatus.Position = 5;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;


        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Approved";
        ticketStatus.Description = "Approved";
        ticketStatus.Position = 6;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Rejected";
        ticketStatus.Description = "Rejected";
        ticketStatus.Position = 7;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "In Engineering";
        ticketStatus.Description = "In Engineering";
        ticketStatus.Position = 8;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "In QA";
        ticketStatus.Description = "In QA";
        ticketStatus.Position = 9;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Inform Customer";
        ticketStatus.Description = "Inform Customer";
        ticketStatus.Position = 10;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Closed";
        ticketStatus.Description = "Closed";
        ticketStatus.Position = 11;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = true;


        ticketStatuses.Save();
        if (createWorkflow)
        {
          ticketNextStatuses = new TicketNextStatuses(loginUser);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[11], 10);


          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[8], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[9], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[8], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[10], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[8], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[9], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[11], 10);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[1], 1);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[2], 2);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[3], 3);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[4], 4);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[5], 5);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[6], 6);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[7], 7);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[8], 8);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[9], 9);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[10], 10);
          //ticketNextStatuses.Save();
        }
      }
      #endregion

      #region Tasks
      ticketStatuses = new TicketStatuses(loginUser);

      ticketType = ticketTypes.AddNewTicketType();
      ticketType.Name = "Tasks";
      ticketType.Description = "Tasks";
      ticketType.OrganizationID = organization.OrganizationID;
      ticketType.IconUrl = "Images/TicketTypes/Tasks.png";
      ticketType.Position = 3;

      ticketTypes.Save();
      if (createTypes)
      {

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "New";
        ticketStatus.Description = "New";
        ticketStatus.Position = 0;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "In Progress";
        ticketStatus.Description = "In Progress";
        ticketStatus.Position = 1;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = false;

        ticketStatus = ticketStatuses.AddNewTicketStatus();
        ticketStatus.Name = "Closed";
        ticketStatus.Description = "Closed";
        ticketStatus.Position = 2;
        ticketStatus.OrganizationID = organization.OrganizationID;
        ticketStatus.TicketTypeID = ticketType.TicketTypeID;
        ticketStatus.IsClosed = true;

        ticketStatuses.Save();

        if (createWorkflow)
        {
          ticketNextStatuses = new TicketNextStatuses(loginUser);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);

          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
          //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);

          //ticketNextStatuses.Save();
        }
      }
      #endregion

      
 
      if (createTypes)
      {

        #region Ticket Severities
      TicketSeverities ticketSeverities = new TicketSeverities(loginUser);
      TicketSeverity ticketSeverity;

      ticketSeverity = ticketSeverities.AddNewTicketSeverity();
      ticketSeverity.Name = "Immediate Attention";
      ticketSeverity.Description = "Immediate Attention";
      ticketSeverity.Position = 0;
      ticketSeverity.OrganizationID = organization.OrganizationID;

      ticketSeverity = ticketSeverities.AddNewTicketSeverity();
      ticketSeverity.Name = "High";
      ticketSeverity.Description = "High";
      ticketSeverity.Position = 1;
      ticketSeverity.OrganizationID = organization.OrganizationID;

      ticketSeverity = ticketSeverities.AddNewTicketSeverity();
      ticketSeverity.Name = "Medium";
      ticketSeverity.Description = "Medium";
      ticketSeverity.Position = 2;
      ticketSeverity.OrganizationID = organization.OrganizationID;

      ticketSeverity = ticketSeverities.AddNewTicketSeverity();
      ticketSeverity.Name = "Low";
      ticketSeverity.Description = "Low";
      ticketSeverity.Position = 3;
      ticketSeverity.OrganizationID = organization.OrganizationID;

      ticketSeverities.Save();
      #endregion
      }

      Groups groups = new Groups(loginUser);
      Group group = groups.AddNewGroup();
      group.Description = "Default Support Group";
      group.Name = "Support";
      group.OrganizationID = organization.OrganizationID;
      group.Collection.Save();

      groups.AddGroupUser(loginUser.UserID, group.GroupID);
      organization.DefaultPortalGroupID = group.GroupID;
      organization.Collection.Save();
      
    
    }

    partial void BeforeRowDelete(int organizationID)
    {
      Organization organization = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
      string description = "Deleted organization '" + organization.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
    }

    partial void BeforeRowEdit(Organization organization)
    {
      string description;

      Organization oldOrganization = (Organization)Organizations.GetOrganization(LoginUser, organization.OrganizationID);

      if (oldOrganization.Description != organization.Description)
      {
        description = "Changed description for organization '" + organization.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }

      if (oldOrganization.HasPortalAccess != organization.HasPortalAccess)
      {
        description = "Changed portal access rights for organization '" + organization.Name + "' to '" + organization.HasPortalAccess.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }

      if (oldOrganization.InActiveReason != organization.InActiveReason)
      {
        description = "Changed inactive reason for organization '" + organization.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }

      if (oldOrganization.IsActive != organization.IsActive)
      {
        description = "Changed active status for organization '" + organization.Name + "' to '" + organization.IsActive.ToString() + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }

      if (oldOrganization.Name != organization.Name)
      {
        description = "Changed organization name from '" + oldOrganization.Name + "' to '" + organization.Name + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }

      if (oldOrganization.PrimaryUserID != organization.PrimaryUserID)
      {
        string name1 = oldOrganization.PrimaryUserID == null ? "Unassigned" : Users.GetUserFullName(LoginUser, (int)oldOrganization.PrimaryUserID);
        string name2 = organization.PrimaryUserID == null ? "Unassigned" : Users.GetUserFullName(LoginUser, (int)organization.PrimaryUserID);

        description = "Changed primary contact for organization '" + organization.Name + "' from '" + name1 + "' to '" + name2 + "'";
        ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
      }
    }

    partial void AfterRowInsert(Organization organization)
    {
      string description = "Created organization '" + organization.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organization.OrganizationID, description);
    }

    public void LoadByEmail(string email)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT o.* FROM Organizations o WHERE ((o.ParentID = 1) OR (o.ParentID is null)) AND EXISTS(SELECT * FROM Users u WHERE (u.MarkDeleted = 0) AND (u.Email = @Email) AND u.OrganizationID = o.OrganizationID) ORDER BY o.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@Email", email);
        Fill(command, "Organizations,Users");
      }
    }

    public void LoadTeamSupport()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Organizations WHERE ParentID is null";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }


    public void LoadByOrganizationName(string name)
    {
      LoadByOrganizationName(name, 1);
    }

    public void LoadByOrganizationName(string name, int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Organizations WHERE Name = @Name AND ParentID = @ParentID ORDER BY Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

    public void LoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive)
    {
      LoadByLikeOrganizationName(parentID, name, loadOnlyActive, int.MaxValue);
    }

    public void LoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive, int maxRows)
    {
      using (SqlCommand command = new SqlCommand())
      {
        if (name.Trim() == "")
          command.CommandText = "SELECT TOP (@MaxRows) * FROM Organizations WHERE (ParentID = @ParentID) AND (@ActiveOnly = 0 OR IsActive = 1) ORDER BY Name";
        else
          command.CommandText = "SELECT TOP (@MaxRows) * FROM Organizations WHERE ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) AND (ParentID = @ParentID) AND (@ActiveOnly = 0 OR IsActive = 1) ORDER BY Name";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ParentID", parentID);
        command.Parameters.AddWithValue("@MaxRows", maxRows);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByParentID(int parentID, bool loadOnlyActive)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Organizations WHERE (ParentID = @ParentID) AND (@ActiveOnly = 0 OR IsActive = 1) ORDER BY Name ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByCRMLinkID(int crmlinkID) {
        using (SqlCommand command = new SqlCommand()) {
            command.CommandText = "SELECT * FROM Organizations WHERE CRMLinkID = @CrmlinkID";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@CrmlinkID", crmlinkID);
            Fill(command);
        }
    }

    public void LoadNotTicketCustomers(int parentID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Organizations o WHERE ((o.ParentID = @ParentID) OR (o.OrganizationID = @ParentID)) AND (o.IsActive = 1) AND NOT EXISTS(SELECT * FROM OrganizationTickets ot WHERE o.OrganizationID = ot.OrganizationID AND ot.TicketID = @TicketID) ORDER BY o.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }
      
    public void LoadByMostActive(int parentID, DateTime beginDate, int top)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP " + top.ToString() + @" o.*, 
                                  (
                                    SELECT COUNT(*) FROM Tickets t 
                                    INNER JOIN OrganizationTickets ot 
	                                  ON ot.TicketID = t.TicketID 
                                    WHERE (ot.OrganizationID = o.OrganizationID) 
	                                  AND (t.DateModified >= @DateModified)
                                  ) AS TicketCount
                                FROM Organizations o
                                WHERE o.ParentID = @ParentID
                                AND o.IsActive = 1
                                ORDER BY TicketCount DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", parentID);
        command.Parameters.AddWithValue("@DateModified", beginDate);
        Fill(command);
      }
    }

    /// <summary>
    /// Loads ALL the organizations associated with a ticket
    /// </summary>
    /// <param name="ticketID"></param>

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT o.* FROM Organizations o LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID WHERE ot.TicketID = @TicketID AND o.IsActive = 1 ORDER BY o.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Organizations,OrganizationTickets");
      }
    }

    /// <summary>
    /// Loads ONLY the organizations associated with a ticket, but not already associated with a specific contact
    /// </summary>
    /// <param name="ticketID"></param>
    public void LoadByNotContactTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT o.* FROM Organizations o 
LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID 
WHERE ot.TicketID = @TicketID 
AND ot.OrganizationID NOT IN (SELECT u.OrganizationID FROM UserTickets ut LEFT JOIN Users u ON u.UserID = ut.UserID WHERE TicketID = @TicketID AND u.MarkDeleted=0)
AND o.IsActive = 1 ORDER BY o.Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Organizations,OrganizationTickets");
      }
    }

    public void LoadByWebServiceID(Guid webServiceID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT o.* FROM Organizations o WHERE WebServiceID = @WebServiceID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@WebServiceID", webServiceID);
        Fill(command, "Organizations");
      }
    
    }

    public void LoadByChatID(Guid chatID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT o.* FROM Organizations o WHERE ChatID = @ChatID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatID", chatID);
        Fill(command, "Organizations");
      }

    }

    public static int GetChatCount(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Users u WHERE (u.IsActive = 1) AND (u.MarkDeleted = 0) AND (u.OrganizationID = @OrganizationID) AND (u.IsChatUser = 1)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        Organizations organizations = new Organizations(loginUser);
        return (int)organizations.ExecuteScalar(command, "Users");
      }
    }

    public static int GetUserCount(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Users u WHERE (u.IsActive = 1) AND (u.MarkDeleted = 0) AND (u.OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        Organizations organizations = new Organizations(loginUser);
        return (int)organizations.ExecuteScalar(command, "Users");
      }
    }
    
    public static int GetPortalCount(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT COUNT(*) FROM Organizations o
                                WHERE (o.IsActive = 1) AND (o.ParentID = @ParentID) AND (HasPortalAccess = 1)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ParentID", organizationID);

        Organizations organizations = new Organizations(loginUser);
        return (int)organizations.ExecuteScalar(command, "Organizations");
      }
    }    
    
    public static int GetStorageUsed(LoginUser loginUser, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT SUM(a.FileSize) FROM Attachments a WHERE (a.OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        Organizations organizations = new Organizations(loginUser);
        object o = organizations.ExecuteScalar(command, "Attachments");
        if (o == DBNull.Value)
        {
          return 0;
        }
        else
        {
          long value = (long)o;
          return (int)(value / 1024 / 1024);
        }
        
        
      }
    }

    public static int GetExtraStorageAllowed(LoginUser loginUser, int organizationID)
    {
      Organization organization = (Organization)Organizations.GetOrganization(loginUser, organizationID);
      if (organization == null) return 0;
      return organization.ExtraStorageUnits * 500;
    }

    public static int GetBaseStorageAllowed(LoginUser loginUser, int organizationID)
    {

      Organization organization = Organizations.GetOrganization(loginUser, organizationID);
      if (organization == null) return 0;
      int result = 0;


      switch (organization.ProductType)
      {
        case ProductType.Express:
          result = organization.UserSeats * 75;
          break;
        case ProductType.HelpDesk:
        case ProductType.BugTracking:
          result = organization.UserSeats * 150;
          break;
        case ProductType.Enterprise:
          result = organization.UserSeats * 250;
          break;
        default:
          break;
      } 
    
      return result;
    }

    public static int GetTotalStorageAllowed(LoginUser loginUser, int organizationID)
    {
      return GetExtraStorageAllowed(loginUser, organizationID) + GetBaseStorageAllowed(loginUser, organizationID);
    }

    public static void DeleteOrganizationAndAllReleatedData(LoginUser loginUser, int organizationID)
    {
      Invoices invoices = new Invoices(loginUser);
      invoices.LoadByOrganizationID(organizationID);

      if (invoices.IsEmpty) DeleteOrganizations(loginUser, organizationID);

    }

    private static void DeleteOrganizations(LoginUser loginUser, int organizationID)
    {
      User user = (User)Users.GetUser(loginUser, loginUser.UserID);
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByParentID(organizationID, false);
      foreach (Organization organization in organizations)
      {
        DeleteOrganizationAndAllReleatedData(loginUser, organization.OrganizationID);
      }
      if (user != null)
        ActionLogs.AddActionLog(loginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, "Organization '" + ((Organization)Organizations.GetOrganization(loginUser, organizationID)).Name + "' was deleted by '" + user.FirstLastName + "'");
      DeleteOrganizationItems(loginUser, organizationID);

      Organization org = (Organization)Organizations.GetOrganization(loginUser, organizationID);
      org.Delete();
      org.Collection.Save();
    }

    private static void DeleteOrganizationItems(LoginUser loginUser, int organizationID)
    {
      using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand command = connection.CreateCommand();
        command.Connection = connection;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);

        command.CommandText = "DELETE FROM OrganizationProducts WHERE OrganizationID = @OrganizationID";
        command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM OrganizationTickets WHERE OrganizationID = @OrganizationID";
        command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM Tickets WHERE OrganizationID = @OrganizationID";
        command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM BillingInfo WHERE OrganizationID = @OrganizationID";
        command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM CreditCards WHERE OrganizationID = @OrganizationID";
        command.ExecuteNonQuery();

      }

      DeleteAttachments(loginUser, organizationID);
    }

    private static void DeleteAttachments(LoginUser loginUser, int organizationID)
    {
      string path = AttachmentPath.GetRoot(loginUser, organizationID);
      if (Directory.Exists(path))
      {
        try
        {
          Directory.Delete(path, true);
        }
        catch (Exception)
        {
        }
        
      }
    }

    public Organization FindByImportID(string importID)
    {
      importID = importID.ToLower().Trim();
      foreach (Organization organization in this)
      {
        if ((organization.ImportID != null && organization.ImportID.Trim().ToLower() == importID) || organization.Name.ToLower().Trim() == importID)
        {
          return organization;
        }
      }
      return null;
    }

    public Organization FindByName(string name)
    {
      foreach (Organization organization in this)
      {
        if (organization.Name.ToLower().Trim() == name.ToLower().Trim())
        {
          return organization;
        }
      }
      return null;
    }


  }
}
