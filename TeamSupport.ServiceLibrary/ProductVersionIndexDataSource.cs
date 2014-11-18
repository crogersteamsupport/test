using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ProductVersionIndexDataSource : IndexDataSource
  {
    protected ProductVersionIndexDataSource() { }

    public ProductVersionIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
      : base(loginUser, maxCount, organizationID, isRebuilding, logName)
    {
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        ProductVersionsViewItem productVersion = ProductVersionsView.GetProductVersionsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing ProductVersionID: " + productVersion.ProductVersionID.ToString());

        _lastItemID = productVersion.ProductVersionID;
        UpdatedItems.Add((int)_lastItemID);

        DocText = HtmlToText.ConvertHtml(productVersion.Description == null ? string.Empty : productVersion.Description);

        _docFields.Clear();
        foreach (DataColumn column in productVersion.Collection.Table.Columns)
        {
          object value = productVersion.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          AddDocField(column.ColumnName, s);
        }

        ProductVersionsSearch productVersionsSearch = new ProductVersionsSearch(productVersion);
        Tickets tickets = new Tickets(_loginUser);
        productVersionsSearch.openTicketCount = tickets.GetProductVersionTicketCount(productVersion.ProductVersionID, 0);
        AddDocField("**JSON", JsonConvert.SerializeObject(productVersionsSearch));

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.ProductVersions, null, productVersion.ProductVersionID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          AddDocField(value.Row["Name"].ToString(), s);
        }
        DocFields = _docFields.ToString();
        DocIsFile = false;
        DocName = productVersion.ProductVersionID.ToString();
        DocCreatedDate = productVersion.DateCreatedUtc;
        DocModifiedDate = DateTime.UtcNow;

      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ProductVersionIndexDataSource");
      }
      return true;
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound product versions, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        ProductVersionsView productVersions = new ProductVersionsView(_loginUser);
        productVersions.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (ProductVersionsViewItem productVersion in productVersions)
        {
          _itemIDList.Add(productVersion.ProductVersionID);
        }
        _lastItemID = null;
        _rowIndex = -1;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ProductVersionIndexDataSource Rewind");
        throw;
      }
      return true;
    }
  }
}
