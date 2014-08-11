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
  class AssetIndexDataSource : IndexDataSource
  {
    protected AssetIndexDataSource() { }
    public AssetIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
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

        AssetsViewItem asset = AssetsView.GetAssetsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing AssetID: " + asset.AssetID.ToString());

        _lastItemID = asset.AssetID;
        UpdatedItems.Add((int)_lastItemID);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine(asset.CreatorName
        + " " + asset.DisplayName
        + " " + asset.DateCreated
        + " " + asset.DateModified
        + " " + asset.ModifierName
        + " " + asset.Notes
        + " " + asset.ProductName
        + " " + asset.ProductVersionNumber
        + " " + asset.SerialNumber
        + " " + asset.WarrantyExpiration);

        AssetHistoryView assetHistoryView = new AssetHistoryView(_loginUser);
        assetHistoryView.LoadByAssetID(asset.AssetID);
        foreach (AssetHistoryViewItem assetHistoryViewItem in assetHistoryView)
        {
          builder.AppendLine(assetHistoryViewItem.ActionTime
          + " " + assetHistoryViewItem.ActionDescription
          + " " + assetHistoryViewItem.NameAssignedFrom
          + " " + assetHistoryViewItem.NameAssignedTo
          + " " + assetHistoryViewItem.TrackingNumber
          + " " + assetHistoryViewItem.ShippingMethod
          + " " + assetHistoryViewItem.ReferenceNum
          + " " + assetHistoryViewItem.Comments
          + " " + assetHistoryViewItem.ActorName
          + " " + assetHistoryViewItem.DateModified
          + " " + assetHistoryViewItem.ModifierName
          + " " + assetHistoryViewItem.DateCreated);
        }

        DocText = string.Format("<html>{1} {0}</html>", "CUSTOM FIELDS", builder.ToString());

        StringBuilder assetLocationString = new StringBuilder();
        switch (asset.Location)
        {
          case "1":
            assetLocationString.Append("Assigned");
            break;
          case "2":
            assetLocationString.Append("Warehouse");
            break;
          case "3":
            assetLocationString.Append("Junkyard");
            break;
        }
        _docFields.Clear();
        AddDocField("AssetID", asset.AssetID);
        AddDocField("Location", assetLocationString.ToString());
        AddDocField("SerialNumber", asset.SerialNumber);

        if (string.IsNullOrWhiteSpace(asset.Name))
        {
          if (string.IsNullOrWhiteSpace(asset.SerialNumber))
          {
            AddDocField("Name", asset.AssetID);
            DocDisplayName = asset.AssetID.ToString();
          }
          else
          {
            AddDocField("Name", asset.SerialNumber);
            DocDisplayName = asset.SerialNumber;
          }
        }
        else
        {
          AddDocField("Name", asset.Name);
          DocDisplayName = asset.Name;
        }

        InventorySearchAsset assetItem = new InventorySearchAsset(asset);
        AddDocField("**JSON", JsonConvert.SerializeObject(assetItem));

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Assets, asset.AssetID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          AddDocField(value.Row["Name"].ToString(), s);
        }
        DocFields = _docFields.ToString();
        DocIsFile = false;
        DocName = asset.AssetID.ToString();
        try
        {
          DocCreatedDate = (DateTime)asset.Row["DateCreated"];
          DocModifiedDate = (DateTime)asset.Row["DateModified"];
        }
        catch (Exception)
        {

        }

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "AssetIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound assets, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        Assets assets = new Assets(_loginUser);
        assets.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (Asset asset in assets)
        {
          _itemIDList.Add(asset.AssetID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "AssetIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }



}
