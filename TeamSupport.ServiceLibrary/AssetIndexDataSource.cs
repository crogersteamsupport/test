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
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        AssetsViewItem asset = AssetsView.GetAssetsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing AssetID: " + asset.AssetID.ToString());

        _lastItemID = asset.AssetID;

        StringBuilder builder = new StringBuilder();
        builder.AppendLine(asset.CreatorName
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

        DocFields = string.Empty;

        DocFields += "AssetID\t" + asset.AssetID.ToString() + "\t";
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

        DocFields += "Location\t" + assetLocationString.ToString() + "\t";

        if (string.IsNullOrWhiteSpace(asset.Name))
        {
          if (string.IsNullOrWhiteSpace(asset.SerialNumber))
          {
            DocFields += "Name\t" + asset.AssetID.ToString() + "\t";
            DocDisplayName = asset.AssetID.ToString();
          }
          else
          {
            DocFields += "Name\t" + asset.SerialNumber + "\t";
            DocDisplayName = asset.SerialNumber;
          }
        }
        else
        {
          DocFields += "Name\t" + asset.Name + "\t";
          DocDisplayName = asset.Name;
        }

        InventorySearchAsset assetItem = new InventorySearchAsset(asset);
        DocFields += "**JSON\t" + JsonConvert.SerializeObject(assetItem) + "\t";

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Assets, asset.AssetID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        }

        DocIsFile = false;
        DocName = asset.AssetID.ToString();
        DocCreatedDate = (DateTime)asset.Row["DateCreated"];
        DocModifiedDate = (DateTime)asset.Row["DateModified"];

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
