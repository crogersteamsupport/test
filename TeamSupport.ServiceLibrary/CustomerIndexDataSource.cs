using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class CustomerIndexDataSource : IndexDataSource
  {
    protected CustomerIndexDataSource() { }

    public CustomerIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding)
      : base(loginUser, maxCount, organizationID, isRebuilding)
    {
      _logs = new Logs("Customer Indexer DataSource");
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        OrganizationsViewItem organization = OrganizationsView.GetOrganizationsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing OrganizationID: " + organization.OrganizationID.ToString());

        _lastItemID = organization.OrganizationID;

        StringBuilder builder = new StringBuilder();
        PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
        phoneNumbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);
        foreach (PhoneNumber number in phoneNumbers)
        {
          builder.AppendLine(number.FormattedNumber + number.Extension == "" ? "" : " Ext: " + number.Extension);
        }

        Addresses addresses = new Addresses(_loginUser);
        addresses.LoadByID(organization.OrganizationID, ReferenceType.Organizations);
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

        List<string> columnsToIndex = new List<string>();
        columnsToIndex.Add("Name");
        columnsToIndex.Add("Description");
        columnsToIndex.Add("Website");
        columnsToIndex.Add("PrimaryContact");

        DocFields = string.Empty;
        foreach (DataColumn column in organization.Collection.Table.Columns)
        {
          if (columnsToIndex.Contains(column.ColumnName))
          {
            object value = organization.Row[column];
            string s = value == null || value == DBNull.Value ? "" : value.ToString();
            DocFields += column.ColumnName + "\t" + s.Replace("\t", " ") + "\t";
          }
        }

        //CustomValues customValues = new CustomValues(_loginUser);
        //customValues.LoadByReferenceType(_organizationID, ReferenceType.Organizations, organization.OrganizationID);

        //foreach (CustomValue value in customValues)
        //{
        //  object o = value.Row["CustomValue"];
        //  string s = o == null || o == DBNull.Value ? "" : o.ToString();
        //  DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        //}

        DocIsFile = false;
        DocName = organization.OrganizationID.ToString();
        DocDisplayName = organization.Name;
        DocCreatedDate = (DateTime)organization.Row["DateCreated"];
        DocModifiedDate = (DateTime)organization.Row["DateModified"];

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "CustomerIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound customers, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        OrganizationsView organizations = new OrganizationsView(_loginUser);
        organizations.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (OrganizationsViewItem organization in organizations)
        {
          _itemIDList.Add(organization.OrganizationID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "CustomerIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }
}
