using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ProductVersionIndexDataSource : IndexDataSource
  {
    protected ProductVersionIndexDataSource() { }

    public ProductVersionIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding)
      : base(loginUser, maxCount, organizationID, isRebuilding)
    {
      _logs = new Logs("Product Version Indexer DataSource");
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        ProductVersionsViewItem productVersion = ProductVersionsView.GetProductVersionsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing ProductVersionID: " + productVersion.ProductVersionID.ToString());

        _lastItemID = productVersion.ProductVersionID;

        DocText = string.Format("<html><body>{0}</body></html>", HtmlToText.ConvertHtml(productVersion.Description == null ? string.Empty : productVersion.Description));

        DocFields = string.Empty;
        foreach (DataColumn column in productVersion.Collection.Table.Columns)
        {
          object value = productVersion.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          DocFields += column.ColumnName + "\t" + s.Replace("\t", " ") + "\t";
        }

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.ProductVersions, null, productVersion.ProductVersionID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        }

        DocIsFile = false;
        DocName = productVersion.ProductVersionID.ToString();
        DocCreatedDate = productVersion.DateCreatedUtc;
        DocModifiedDate = DateTime.UtcNow;

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ProductVersionIndexDataSource");
        throw;
      }
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
