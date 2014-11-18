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
  class ProductIndexDataSource : IndexDataSource
  {
    protected ProductIndexDataSource() { }
    public ProductIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
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

        Product product = Products.GetProduct(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing ProductID: " + product.ProductID.ToString());

        _lastItemID = product.ProductID;
        UpdatedItems.Add((int)_lastItemID);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine(product.Description
        + " " + product.Name
        + " " + product.DateCreated
        + " " + product.DateModified);

        DocText = builder.ToString();

        _docFields.Clear();
        AddDocField("ProductID", product.ProductID);
        AddDocField("Name", product.Name);
        DocDisplayName = product.Name;

        ProductSearch productItem = new ProductSearch(product);
        Tickets tickets = new Tickets(_loginUser);
        productItem.openTicketCount = tickets.GetProductTicketCount(product.ProductID, 0);
        AddDocField("**JSON", JsonConvert.SerializeObject(productItem));

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Products, product.ProductID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          AddDocField(value.Row["Name"].ToString(), s);
        }
        DocFields = _docFields.ToString();
        DocIsFile = false;
        DocName = product.ProductID.ToString();
        try
        {
          DocCreatedDate = (DateTime)product.Row["DateCreated"];
          DocModifiedDate = (DateTime)product.Row["DateModified"];
        }
        catch (Exception)
        {

        }

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ProductIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound products, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        Products products = new Products(_loginUser);
        products.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (Product product in products)
        {
          _itemIDList.Add(product.ProductID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ProductIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }
}
