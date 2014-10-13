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
using System.Globalization;

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

    [WebMethod]
    public string LoadChartData(int productID, bool open)
    {

      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

      TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
      ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

      int total = 0;
      StringBuilder chartString = new StringBuilder("");

      foreach (TicketType ticketType in ticketTypes)
      {
        int count;
        if (open)
          count = Tickets.GetProductOpenTicketCount(TSAuthentication.GetLoginUser(), productID, ticketType.TicketTypeID);
        else
          count = Tickets.GetProductClosedTicketCount(TSAuthentication.GetLoginUser(), productID, ticketType.TicketTypeID);
        total += count;

        if (count > 0)
          chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
        //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
      }
      if (chartString.ToString().EndsWith(","))
      {
        chartString.Remove(chartString.Length - 1, 1);
      }

      return chartString.ToString();
    }

    [WebMethod]
    public string LoadVersionChartData(int productVersionID, bool open)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByOrganizationID(loginUser.OrganizationID);

      TicketTypes ticketTypes = new TicketTypes(loginUser);
      ticketTypes.LoadByOrganizationID(loginUser.OrganizationID, organizations[0].ProductType);

      int total = 0;
      StringBuilder chartString = new StringBuilder("");

      foreach (TicketType ticketType in ticketTypes)
      {
        int count;
        if (open)
          count = Tickets.GetProductVersionOpenTicketCount(loginUser, productVersionID, ticketType.TicketTypeID);
        else
          count = Tickets.GetProductVersionClosedTicketCount(loginUser, productVersionID, ticketType.TicketTypeID);
        total += count;

        if (count > 0)
          chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
        //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
      }
      if (chartString.ToString().EndsWith(","))
      {
        chartString.Remove(chartString.Length - 1, 1);
      }

      return chartString.ToString();
    }

    [WebMethod]
    public string GetProductTickets(int productID, int closed)
    {
      TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

      return tickets.GetProductTicketCount(productID, closed).ToString();
    }

    [WebMethod]
    public string GetProductVersionTickets(int productVersionID, int closed)
    {
      TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

      return tickets.GetProductVersionTicketCount(productVersionID, closed).ToString();
    }

    [WebMethod]
    public ActionLogProxy[] LoadHistory(int productID, int start)
    {
      ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
      actionLogs.LoadByProductIDLimit(productID, start);

      return actionLogs.GetActionLogProxies();
    }

    [WebMethod]
    public ActionLogProxy[] LoadVersionHistory(int productVersionID, int start)
    {
      ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
      actionLogs.LoadByProductVersionIDLimit(productVersionID, start);

      return actionLogs.GetActionLogProxies();
    }

    [WebMethod]
    public string SetName(int productID, string value)
    {
      Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
      p.Name = value;
      p.Collection.Save();
      string description = String.Format("{0} set product name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
      ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Products, productID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string SetVersionNumber(int productVersionID, string value)
    {
      ProductVersion pv = ProductVersions.GetProductVersion(TSAuthentication.GetLoginUser(), productVersionID);
      pv.VersionNumber = value;
      pv.Collection.Save();
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      string description = String.Format("{0} set product version number to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string SetDescription(int productID, string value)
    {
      Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
      p.Description = value;
      p.Collection.Save();
      string description = String.Format("{0} set product description to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
      ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Products, productID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string SetVersionDescription(int productVersionID, string value)
    {
      ProductVersion pv = ProductVersions.GetProductVersion(TSAuthentication.GetLoginUser(), productVersionID);
      pv.Description = value;
      pv.Collection.Save();
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      string description = String.Format("{0} set product version description to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, value);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.ProductVersions, productVersionID, description);
      return value != "" ? value : "Empty";
    }

    [WebMethod]
    public string LoadVersions(int productID)
    {
      StringBuilder htmlresults = new StringBuilder("");
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      ProductVersionsView productVersions = new ProductVersionsView(loginUser);
      productVersions.LoadByProductID(productID);

      foreach (ProductVersionsViewItem productVersion in productVersions)
      {
        htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <span class='pull-right'>{0}</span>
                            <a href='#' id='{1}' class='productversionlink'>
                              <h4 class='list-group-item-heading'>{2}</h4>
                            </a>
                            <div class='row'>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{3}</p>
                                    {4}
                                </div>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{5} Open Tickets</p>
                                    <p class='list-group-item-text'>{6} Closed Tickets</p>                            
                                </div>
                            </div>
                            </div>"

            , productVersion.IsReleased ? "Released" : "Not released"
            , productVersion.ProductVersionID
            , productVersion.VersionNumber
            , productVersion.VersionStatus
            , (productVersion.IsReleased && productVersion.ReleaseDate != null) ? "Released on " + DataUtils.DateToLocal(loginUser, (((DateTime)productVersion.ReleaseDateUtc))).ToString(GetDateFormatNormal()) : ""
            , GetProductVersionTickets(productVersion.ProductVersionID, 0)
            , GetProductVersionTickets(productVersion.ProductVersionID, 1));

      }

      return htmlresults.ToString();
    }

    [WebMethod]
    public ProductCustomOrganization[] LoadCustomers(int productID)
    {
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      OrganizationProductsView organizationProducts = new OrganizationProductsView(loginUser);
      organizationProducts.LoadByProductID(productID);
      List<ProductCustomOrganization> list = new List<ProductCustomOrganization>();
      CustomFields fields = new CustomFields(loginUser);
      fields.LoadByReferenceType(loginUser.OrganizationID, ReferenceType.OrganizationProducts);


      foreach (DataRow row in organizationProducts.Table.Rows)
      {
        ProductCustomOrganization test = new ProductCustomOrganization();
        test.Customer = row["OrganizationName"].ToString();
        test.VersionNumber = row["VersionNumber"].ToString();
        test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(loginUser, (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
        test.VersionStatus = row["VersionStatus"].ToString();
        test.IsReleased = row["IsReleased"].ToString();
        test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
        test.OrganizationProductID = (int)row["OrganizationProductID"];
        test.CustomFields = new List<string>();
        foreach (CustomField field in fields)
        {
          CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
          test.CustomFields.Add(customValue.Value);
        }


        list.Add(test);
      }


      return list.ToArray();
    }

    [WebMethod]
    public ProductCustomOrganization LoadCustomer(int organizationProductID)
    {
      OrganizationProductsViewItem organizationProduct = (OrganizationProductsViewItem)OrganizationProductsView.GetOrganizationProductsViewItem(TSAuthentication.GetLoginUser(), organizationProductID);
      ProductCustomOrganization productCustomer = new ProductCustomOrganization();

      productCustomer.Customer = organizationProduct.OrganizationName;
      productCustomer.VersionNumber = organizationProduct.ProductVersionID.HasValue ? organizationProduct.ProductVersionID.ToString() : "-1";

      if (organizationProduct.SupportExpiration.HasValue)
        productCustomer.SupportExpiration = ((DateTime)organizationProduct.SupportExpiration).ToString(GetDateFormatNormal());
      else
        productCustomer.SupportExpiration = "";

      productCustomer.VersionStatus = "";
      productCustomer.IsReleased = "";
      productCustomer.ReleaseDate = null;
      productCustomer.OrganizationProductID = organizationProduct.OrganizationProductID;
      productCustomer.OrganizationID = organizationProduct.OrganizationID;

      return productCustomer;
    }

    public string GetDateFormatNormal()
    {
      CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
      return us.DateTimeFormat.ShortDatePattern;
    }

    [WebMethod]
    public string LoadAssets(int productID)
    {
      StringBuilder htmlresults = new StringBuilder("");
      AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
      assets.LoadByProductID(productID);

      StringBuilder productVersionNumberDisplayName;
      StringBuilder serialNumberDisplayValue;
      StringBuilder warrantyExpirationDisplayValue;

      foreach (AssetsViewItem asset in assets)
      {
        productVersionNumberDisplayName = new StringBuilder();
        serialNumberDisplayValue = new StringBuilder();
        warrantyExpirationDisplayValue = new StringBuilder();

        if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
        {
          productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
        }

        if (string.IsNullOrEmpty(asset.SerialNumber))
        {
          serialNumberDisplayValue.Append("Empty");
        }
        else
        {
          serialNumberDisplayValue.Append(asset.SerialNumber);
        }

        if (asset.WarrantyExpiration == null)
        {
          warrantyExpirationDisplayValue.Append("Empty");
        }
        else
        {
          warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
        }

        htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>
                            </div>"

            , asset.AssetID
            , asset.DisplayName
            , asset.ProductName
            , productVersionNumberDisplayName
            , serialNumberDisplayValue
            , warrantyExpirationDisplayValue);
      }

      return htmlresults.ToString();
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

  public class ProductCustomOrganization
  {
    [DataMember]
    public string Customer { get; set; }
    [DataMember]
    public string VersionNumber { get; set; }
    [DataMember]
    public string SupportExpiration { get; set; }
    [DataMember]
    public string VersionStatus { get; set; }
    [DataMember]
    public string IsReleased { get; set; }
    [DataMember]
    public string ReleaseDate { get; set; }
    [DataMember]
    public int ProductID { get; set; }
    [DataMember]
    public int OrganizationID { get; set; }
    [DataMember]
    public int OrganizationProductID { get; set; }
    [DataMember]
    public List<string> CustomFields { get; set; }

  }

}
