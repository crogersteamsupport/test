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
    public AssetProxy GetAsset(int assetID)
    {
      Asset asset = Assets.GetAsset(TSAuthentication.GetLoginUser(), assetID);
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
      asset.SerialNumber =        info.SerialNumber;
      //asset.WarrantyExpiration = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), (DateTime?)info.WarrantyExpiration);
      asset.WarrantyExpiration = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), info.WarrantyExpiration);
      asset.Notes = info.Notes;
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
  }

  public class NewAssetSave
  {
    public NewAssetSave() { }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int ProductID { get; set; }
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