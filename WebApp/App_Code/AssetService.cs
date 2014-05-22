using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;
using System.Net;
using System.IO;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class AssetService : System.Web.Services.WebService
  {

    public AssetService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public AutocompleteItem[] FindAsset(string searchTerm)
    {
      Assets assets = new Assets(TSAuthentication.GetLoginUser());
      assets.LoadByLikeNameOrSerial(TSAuthentication.OrganizationID, searchTerm, 25);

      List<AutocompleteItem> list = new List<AutocompleteItem>();
      foreach (Asset asset in assets)
      {
        if (!string.IsNullOrEmpty(asset.SerialNumber)) list.Add(new AutocompleteItem(string.Format("{0} ({1})", asset.Name, asset.SerialNumber) , asset.AssetID.ToString()));
        else list.Add(new AutocompleteItem(asset.Name, asset.AssetID.ToString()));
      }

      return list.ToArray();
    }

    [WebMethod]
    public AssetsViewItemProxy GetAsset(int assetID)
    {
      AssetsViewItem asset = AssetsView.GetAssetsViewItem(TSAuthentication.GetLoginUser(), assetID);
      if (asset.OrganizationID != TSAuthentication.OrganizationID) return null;
      return asset.GetProxy();
    }

    [WebMethod]
    public int SaveAsset(string data)
    {
      NewAssetSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewAssetSave>(data);
      }
      catch (Exception e)
      {
        return -1;
      }

      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Assets assets = new Assets(loginUser);
      Asset asset = assets.AddNewAsset();

      asset.OrganizationID =      TSAuthentication.OrganizationID;
      asset.Name =                info.Name;
      asset.ProductID =           info.ProductID;
      asset.ProductVersionID =    info.ProductVersionID;
      asset.SerialNumber =        info.SerialNumber;
      asset.WarrantyExpiration =  DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), info.WarrantyExpiration);
      asset.Notes =               info.Notes;
      asset.Location =            "2";

      asset.DateCreated =   DateTime.UtcNow;
      asset.DateModified =  DateTime.UtcNow;
      asset.CreatorID =     loginUser.UserID;
      asset.ModifierID =    loginUser.UserID;

      asset.Collection.Save();

      string description = String.Format("{0} created asset {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, GetAssetReference(asset));
      ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Assets, asset.AssetID, description);

      //foreach (CustomFieldSaveInfo field in info.Fields)
      //{
      //  CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organization.OrganizationID);
      //  if (field.Value == null)
      //  {
      //    customValue.Value = "";
      //  }
      //  else
      //  {
      //    if (customValue.FieldType == CustomFieldType.DateTime)
      //    {
      //      customValue.Value = ((DateTime)field.Value).ToString();
      //      //DateTime dt;
      //      //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
      //      //{
      //      //    customValue.Value = dt.ToUniversalTime().ToString();
      //      //}
      //    }
      //    else
      //    {
      //      customValue.Value = field.Value.ToString();
      //    }

      //  }

      //  customValue.Collection.Save();
      //}

      AssetHistory history = new AssetHistory(loginUser);
      AssetHistoryItem historyItem = history.AddNewAssetHistoryItem();

      historyItem.OrganizationID =    loginUser.OrganizationID;
      historyItem.Actor =             loginUser.UserID;
      historyItem.AssetID =           asset.AssetID;
      historyItem.ActionTime =        DateTime.UtcNow;
      historyItem.ActionDescription = "Asset created.";
      historyItem.ShippedFrom =       0;
      historyItem.ShippedTo =         0;
      historyItem.TrackingNumber =    string.Empty;
      historyItem.ShippingMethod =    string.Empty;
      historyItem.ReferenceNum =      string.Empty;
      historyItem.Comments =          string.Empty;

      history.Save();

      return asset.AssetID;

    }

    private string GetAssetReference(Asset asset)
    {
      string result = "with AssetID: " +asset.AssetID.ToString();

      if (!String.IsNullOrEmpty(asset.Name))
      {
        result = asset.Name;
      }
      else if (!String.IsNullOrEmpty(asset.SerialNumber))
      {
        result = "with Serial Number: " + asset.SerialNumber;
      }

      return result;
    }

    [WebMethod]
    public string GetShortNameFromID(int assetID)
    {
      Assets assets = new Assets(TSAuthentication.GetLoginUser());
      assets.LoadByAssetID(assetID);

      if (assets.IsEmpty) return "N/A";

      string result = assets[0].AssetID.ToString();

      if (!String.IsNullOrEmpty(assets[0].Name))
      {
        if (assets[0].Name.Length > 10)
          result = assets[0].Name.Substring(0, 10).ToString() + "...";
        else
          result = assets[0].Name.ToString();
      }
      else if (!String.IsNullOrEmpty(assets[0].SerialNumber))
      {
        if (assets[0].SerialNumber.Length > 10)
          result = assets[0].SerialNumber.Substring(0, 10).ToString() + "...";
        else
          result = assets[0].SerialNumber.ToString();
      }

      return result;
    }

    [WebMethod]
    public AssetHistoryViewItemProxy[] LoadHistory(int assetID, int start)
    {
      AssetHistoryView history = new AssetHistoryView(TSAuthentication.GetLoginUser());
      history.LoadByAssetIDLimit(assetID, start);

      return history.GetAssetHistoryViewItemProxies();
    }
  }

  public class NewAssetSave
  {
    public NewAssetSave() { }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int ProductID { get; set; }
    [DataMember]
    public int? ProductVersionID { get; set; }
    [DataMember]
    public string SerialNumber { get; set; }
    [DataMember]
    public DateTime? WarrantyExpiration { get; set; }
    [DataMember]
    public string Notes { get; set; }
    [DataMember]
    public List<CustomFieldSaveInfo> Fields { get; set; }
  }  
}