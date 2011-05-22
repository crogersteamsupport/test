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
}