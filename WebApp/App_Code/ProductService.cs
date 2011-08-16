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

}
