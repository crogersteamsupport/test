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

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class ProductService : System.Web.Services.WebService
  {

    public ProductService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }

    [WebMethod]
    public ProductVersionsViewItemProxy GetVersion(int versionID)
    {
      ProductVersionsViewItem version = ProductVersionsView.GetProductVersionsViewItem(TSAuthentication.GetLoginUser(), versionID);
      if (version == null || version.OrganizationID != TSAuthentication.OrganizationID) return null;
      return version.GetProxy();
    }

    [WebMethod]
    public ProductVersionsViewItemProxy[] GetVersions(int productID)
    {
      Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
      if (product.OrganizationID != TSAuthentication.OrganizationID) return null;

      ProductVersionsView versions = new ProductVersionsView(product.Collection.LoginUser);
      versions.LoadByProductID(productID);
      return versions.GetProductVersionsViewItemProxies();
    }

    [WebMethod]
    public BasicProduct[] GetProducts()
    {
      Products products = new Products(TSAuthentication.GetLoginUser());
      products.LoadByOrganizationID(TSAuthentication.OrganizationID);

      List<BasicProduct> result = new List<BasicProduct>();
      foreach (Product product in products)
      {
        ProductVersions versions = new ProductVersions(products.LoginUser);
        versions.LoadByProductID(product.ProductID);

        List<BasicVersion> basicVersions = new List<BasicVersion>();
        foreach (ProductVersion version in versions)
        {
          BasicVersion basicVersion = new BasicVersion();
          basicVersion.ProductVersionID = version.ProductVersionID;
          basicVersion.VersionNumber = version.VersionNumber;
          basicVersions.Add(basicVersion);
        }

        BasicProduct basicProduct = new BasicProduct();
        basicProduct.ProductID = product.ProductID;
        basicProduct.Name = product.Name;
        basicProduct.Versions = basicVersions.ToArray();

        result.Add(basicProduct);
      }

      return result.ToArray();
    }

    [WebMethod]
    public ProductProxy GetProduct(int productID)
    {
      Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
      if (product == null || product.OrganizationID != TSAuthentication.OrganizationID) return null;
      return product.GetProxy();
    }

    [WebMethod]
    public AttachmentProxy[] GetAttachments(int versionID)
    {
      Attachments attachments = new Attachments(TSAuthentication.GetLoginUser());
      attachments.LoadByReference(ReferenceType.ProductVersions, versionID);
      return attachments.GetAttachmentProxies();
    }

    [WebMethod]
    public void DeleteAttachment(int attachmentID)
    {
      Attachment attachment = Attachments.GetAttachment(TSAuthentication.GetLoginUser(), attachmentID);
      ProductVersionsViewItem version = ProductVersionsView.GetProductVersionsViewItem(attachment.Collection.LoginUser, attachment.RefID);
      if (version.OrganizationID != TSAuthentication.OrganizationID) return;
      attachment.Delete();
      attachment.Collection.Save();
    }

    [WebMethod]
    public string GetShortNameFromID(int productID)
    {
      Products products = new Products(TSAuthentication.GetLoginUser());
      products.LoadByProductID(productID);

      if (products.IsEmpty) return "N/A";

      if (products[0].Name.Length > 10)
        return products[0].Name.Substring(0, 10).ToString() + "...";
      else
        return products[0].Name.ToString();
    }

    [WebMethod]
    public string GetVersionShortNameFromID(int productVersionID)
    {
      ProductVersions productVersions = new ProductVersions(TSAuthentication.GetLoginUser());
      productVersions.LoadByProductVersionID(productVersionID);

      if (productVersions.IsEmpty) return "N/A";

      if (productVersions[0].VersionNumber.Length > 10)
        return productVersions[0].VersionNumber.Substring(0, 10).ToString() + "...";
      else
        return productVersions[0].VersionNumber.ToString();
    }

    [WebMethod]
    public string UpdateRecentlyViewed(string viewid)
    {
      int refType, refID;

      if (viewid.StartsWith("v"))
        refType = (int)ReferenceType.ProductVersions;
      else
        refType = (int)ReferenceType.Products;

      refID = Convert.ToInt32(viewid.Substring(1));

      RecentlyViewedItem recent = (new RecentlyViewedItems(TSAuthentication.GetLoginUser()).AddNewRecentlyViewedItem());


      recent.RefID = refID;
      recent.RefType = refType;
      recent.DateViewed = DateTime.UtcNow;
      recent.UserID = TSAuthentication.GetLoginUser().UserID;
      recent.BaseCollection.Save();

      return GetRecentlyViewed();
    }

    [WebMethod]
    public string GetRecentlyViewed()
    {
      StringBuilder builder = new StringBuilder();
      RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
      recent.LoadRecentForProductsPage(TSAuthentication.GetLoginUser().UserID);

      builder.Append(@"<ul class=""recent-list"">");
      foreach (RecentlyViewedItem item in recent)
      {
        builder.Append(CreateRecentlyViewed(item));
      }
      builder.Append("</ul>");
      return builder.ToString();
    }

    public string CreateRecentlyViewed(RecentlyViewedItem recent)
    {
      string recentHTML;
      if (recent.RefType == (int)ReferenceType.ProductVersions)
      {
        ProductVersions pv = new ProductVersions(TSAuthentication.GetLoginUser());
        pv.LoadByProductVersionID(recent.RefID);
        recentHTML = @" 
          <li>
            <div class=""recent-info"">
              <h4><a class=""productversionlink"" data-productversionid=""{0}"" href=""""><i class=""fa fa-clock-o color-orange""></i>{1}</a></h4>
            </div>
          </li>";
        return string.Format(recentHTML, pv[0].ProductVersionID, pv[0].VersionNumber);
      }
      else
      {
        Products p = new Products(TSAuthentication.GetLoginUser());
        p.LoadByProductID(recent.RefID);
        recentHTML = @" 
          <li>
            <div class=""recent-info"">
              <h4><a class=""productlink"" data-productid=""{0}"" href=""""><i class=""fa fa-barcode color-green""></i>{1}</a></h4>
            </div>
          </li>";

        return string.Format(recentHTML, p[0].ProductID, p[0].Name);
      }
    }

    [WebMethod]
    public int SaveProduct(string data)
    {
      NewProductSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductSave>(data);
      }
      catch (Exception e)
      {
        return -1;
      }

      Product product;
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Products products = new Products(loginUser);

      product = products.AddNewProduct();
      product.OrganizationID = loginUser.OrganizationID;
      product.Name = info.Name;
      product.Description = info.Description;
      product.Collection.Save();

      string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, product.Name);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Products, product.ProductID, description);

      foreach (CustomFieldSaveInfo field in info.Fields)
      {
        CustomValue customValue = CustomValues.GetValue(loginUser, field.CustomFieldID, product.ProductID);
        if (field.Value == null)
        {
          customValue.Value = "";
        }
        else
        {
          if (customValue.FieldType == CustomFieldType.DateTime)
          {
            customValue.Value = ((DateTime)field.Value).ToString();
            //DateTime dt;
            //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            //{
            //    customValue.Value = dt.ToUniversalTime().ToString();
            //}
          }
          else
          {
            customValue.Value = field.Value.ToString();
          }

        }

        customValue.Collection.Save();
      }

      return product.ProductID;

    }

    [WebMethod]
    public int SaveProductVersion(string data)
    {
      NewProductVersionSave info;
      try
      {
        info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductVersionSave>(data);
      }
      catch (Exception e)
      {
        return -1;
      }

      ProductVersion productVersion;
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      ProductVersions productVersions = new ProductVersions(loginUser);

      productVersion = productVersions.AddNewProductVersion();
      productVersion.ProductID = info.ProductID;
      productVersion.ProductVersionStatusID = info.ProductVersionStatusID;
      productVersion.VersionNumber = info.VersionNumber;
      productVersion.ReleaseDate = info.ReleaseDate;
      productVersion.IsReleased = info.IsReleased;
      productVersion.Description = info.Description;
      productVersion.Collection.Save();

      string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, productVersion.VersionNumber);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.ProductVersions, productVersion.ProductVersionID, description);

      foreach (CustomFieldSaveInfo field in info.Fields)
      {
        CustomValue customValue = CustomValues.GetValue(loginUser, field.CustomFieldID, productVersion.ProductVersionID);
        if (field.Value == null)
        {
          customValue.Value = "";
        }
        else
        {
          if (customValue.FieldType == CustomFieldType.DateTime)
          {
            customValue.Value = ((DateTime)field.Value).ToString();
            //DateTime dt;
            //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
            //{
            //    customValue.Value = dt.ToUniversalTime().ToString();
            //}
          }
          else
          {
            customValue.Value = field.Value.ToString();
          }

        }

        customValue.Collection.Save();
      }

      return productVersion.ProductVersionID;

    }

    [WebMethod]
    public BasicVersionStatus[] GetProductVersionStatuses()
    {
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(TSAuthentication.GetLoginUser());
      productVersionStatuses.LoadByOrganizationID(TSAuthentication.OrganizationID);

      List<BasicVersionStatus> result = new List<BasicVersionStatus>();
      foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
      {
        BasicVersionStatus basicVersionStatus = new BasicVersionStatus();
        basicVersionStatus.ProductVersionStatusID = productVersionStatus.ProductVersionStatusID;
        basicVersionStatus.Name = productVersionStatus.Name;

        result.Add(basicVersionStatus);
      }

      return result.ToArray();
    }

  }

  [DataContract]
  public class BasicProduct
  {
    [DataMember] public int ProductID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public BasicVersion[] Versions { get; set; }
  }

  [DataContract]
  public class BasicVersion
  {
    [DataMember] public int ProductVersionID { get; set; }
    [DataMember] public string VersionNumber { get; set; }
  }

  [DataContract]
  public class BasicVersionStatus
  {
    [DataMember]
    public int ProductVersionStatusID { get; set; }
    [DataMember]
    public string Name { get; set; }
  }

  public class NewProductSave
  {
      public NewProductSave() { }
      [DataMember] public string Name { get; set; }
      [DataMember] public string Description { get; set; }
      [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
  }

  public class NewProductVersionSave
  {
      public NewProductVersionSave() { }
      [DataMember] public int ProductID { get; set; }
      [DataMember] public int ProductVersionStatusID { get; set; }
      [DataMember] public string VersionNumber { get; set; }
      [DataMember] public DateTime? ReleaseDate { get; set; }
      [DataMember] public bool IsReleased { get; set; }
      [DataMember] public string Description { get; set; }
      [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
  }
}
