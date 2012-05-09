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


  }  
}