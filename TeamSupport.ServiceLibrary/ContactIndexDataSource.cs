using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ContactIndexDataSource : IndexDataSource
  {
    protected ContactIndexDataSource() { }

    public ContactIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding)
      : base(loginUser, maxCount, organizationID, isRebuilding)
    {
      _logs = new Logs("Contact Indexer DataSource");
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        ContactsViewItem contact = ContactsView.GetContactsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing UserID: " + contact.UserID.ToString());

        _lastItemID = contact.UserID;

        List<PhoneItem> phones = new List<PhoneItem>();
        StringBuilder builder = new StringBuilder();
        PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
        phoneNumbers.LoadByID(contact.UserID, ReferenceType.Contacts);
        foreach (PhoneNumber number in phoneNumbers)
        {
          phones.Add(new PhoneItem(number));
          builder.AppendLine(Regex.Replace(number.Number, "[^0-9]", ""));
        }

        Addresses addresses = new Addresses(_loginUser);
        addresses.LoadByID(contact.UserID, ReferenceType.Contacts);
        foreach (Address address in addresses)
        {
          builder.AppendLine(address.Description
          + " " + address.Addr1
          + " " + address.Addr2
          + " " + address.Addr3
          + " " + address.City
          + " " + address.State
          + " " + address.Zip
          + " " + address.Country);
        }

        DocText = string.Format("<html>{1} {0}</html>", "CUSTOM FIELDS", builder.ToString());

        DocFields = string.Empty;
        if (string.IsNullOrWhiteSpace(contact.LastName))
        {
          DocFields += "Name\t" + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim()) + "\t";
          DocDisplayName = string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim();
        }
        else
        {
          DocFields += "Name\t" + contact.LastName.Trim() + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : ", " + contact.FirstName.Trim()) + "\t";
          DocDisplayName = contact.LastName.Trim() + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : ", " + contact.FirstName.Trim());
        }

        DocFields += "Organization\t" + (string.IsNullOrWhiteSpace(contact.Organization) ? "" : contact.Organization.Trim()) + "\t";
        DocFields += "Email\t" + (string.IsNullOrWhiteSpace(contact.Email) ? "" : contact.Email.Trim()) + "\t";
        ContactItem contactItem = new ContactItem(contact);
        contactItem.phones = phones.ToArray();
        TicketsView tickets = new TicketsView(_loginUser);
        contactItem.openTicketCount = tickets.GetUserTicketCount(contact.UserID, 0);
        
        DocFields += "**JSON\t" + JsonConvert.SerializeObject(contactItem) + "\t";

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Contacts, contact.UserID);
        
        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        }

        DocIsFile = false;
        DocName = contact.UserID.ToString();
        DocCreatedDate = (DateTime)contact.Row["DateCreated"];
        DocModifiedDate = (DateTime)contact.Row["DateModified"];

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ContactIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound users, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        ContactsView contacts = new ContactsView(_loginUser);
        contacts.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (ContactsViewItem contact in contacts)
        {
          _itemIDList.Add(contact.UserID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ContactIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }

  public class ContactItem {
    public ContactItem() { }
    public ContactItem(ContactsViewItem item)
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
    public PhoneItem[] phones { get; set; }
  }


}
