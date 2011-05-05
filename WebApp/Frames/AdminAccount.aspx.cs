using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Text;


public partial class Frames_AdminAccount : BaseFramePage
{
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!UserSession.CurrentUser.IsFinanceAdmin)
    {
      Response.Write("");
      Response.End();
      return;
    }


    btnInfo.OnClientClick = "ShowDialog(top.GetAccountDialog(" + UserSession.LoginUser.OrganizationID.ToString() + ")); return false;";

    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(UserSession.LoginUser.OrganizationID, ReferenceType.BillingInfo);

    if (addresses.IsEmpty)
    {
      btnAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + UserSession.LoginUser.OrganizationID.ToString() + ", 24)); return false;";
    }
    else
    {
      btnAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + addresses[0].AddressID.ToString() + ")); return false;";
    }

    LoadDetails();


  }

  private void LoadDetails()
  {
    VerifyBillingInfo();
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    StringBuilder builder = new StringBuilder();

    if (organization != null)
    {

      builder.Append("<tr><td style=\"width: 200px;\"><strong>Product Type: </strong></td><td>");
      builder.Append(DataUtils.ProductTypeString(organization.ProductType) + "</td></tr>");
      //builder.Append("<tr><td style=\"width: 200px;\"><strong>Organization ID: </strong></td><td>");
      //builder.Append(organization.OrganizationID.ToString() + "</td></tr>");
      //builder.Append("<tr><td><strong>Is Customer Free: </strong></td><td>");
      //builder.Append(organization.IsCustomerFree.ToString() + "</td></tr>");
      //builder.Append("<tr><td><strong>Heard about us from: </strong></td><td>");
      //builder.Append(organization.WhereHeard + "</td></tr>");
      //builder.Append("<tr><td><strong>Is Active: </strong></td><td>");
      //builder.Append(organization.IsActive.ToString() + "</td></tr>");
      //builder.Append("<tr><td><strong>Inactive Reason: </strong></td><td>");
      //builder.Append(organization.InActiveReason + "</td></tr>");
      //builder.Append("<tr><td style=\"width: 200px;\"><strong>Automatic Payment: </strong></td><td>");
      //builder.Append(item.IsAutomatic.ToString() + "</td></tr>");

      //builder.Append("<tr><td><strong>User Seats: </strong></td><td>");
      //builder.Append(organization.UserSeats.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Users Used: </strong></td><td>");
      builder.Append(Organizations.GetUserCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID).ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>User Price: </strong></td><td>$");
      if (item.UserPrice == null)
      {
        switch (organization.ProductType)
        {
          case ProductType.Express: builder.Append("10.00"); break;
          case ProductType.HelpDesk: builder.Append("15.00"); break;
          case ProductType.BugTracking: builder.Append("15.00"); break;
          case ProductType.Enterprise: builder.Append("25.00"); break;
          default:
            break;
        }
      }
      else
      {
        builder.Append(item.UserPrice.ToString());
      }
      builder.Append(" / User</td></tr>");

      builder.Append("<tr><td><strong>Advanced Portal: </strong></td><td>");
      builder.Append(organization.IsAdvancedPortal ? "Enabled" : "Disabled" + "</td></tr>");
//      builder.Append("<tr><td><strong>Advanced Portal Seats: </strong></td><td>");
//      builder.Append(organization.PortalSeats.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Advanced Portals Used: </strong></td><td>");
      builder.Append(Organizations.GetPortalCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID).ToString() + "</td></tr>");

      builder.Append("<tr><td><strong>Advanced Portal Price: </strong></td><td>$");
      if (item.PortalPrice == null)
        builder.Append("2.00");
      else
        builder.Append(item.PortalPrice.ToString());
      builder.Append(" / Customer</td></tr>");

      builder.Append("<tr><td><strong>Basic Portal: </strong></td><td>");
      builder.Append(organization.IsBasicPortal ? "Enabled" : "Disabled" + "</td></tr>");

      builder.Append("<tr><td><strong>Basic Portal Price: </strong></td><td>");
      if (item.BasicPortalPrice == null)
        builder.Append("FREE</td></tr>");
      else
        builder.Append("$" + item.BasicPortalPrice.ToString() + " / Month</td></tr>");
      
      
      builder.Append("<tr><td><strong>Extra Storage Units: </strong></td><td>");
      builder.Append(organization.ExtraStorageUnits.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Storage Used: </strong></td><td>");
      builder.Append(Organizations.GetStorageUsed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID).ToString() + "MB</td></tr>");
      builder.Append("<tr><td><strong>Storage Price: </strong></td><td>$");
      if (item.StoragePrice == null)
        builder.Append("25.00");
      else
        builder.Append(item.StoragePrice.ToString());

      builder.Append(" / Month / 5 GB</td></tr>");

      builder.Append("<tr><td><strong>Extra Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetExtraStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) + "MB</td></tr>");
      builder.Append("<tr><td><strong>Base Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetBaseStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) + "MB</td></tr>");
      builder.Append("<tr><td><strong>Total Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetTotalStorageAllowed(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) + "MB</td></tr>");





    /*  if (item.NextInvoiceDate == null)
      {
        builder.Append("<tr><td><strong>Next Invoice Date: </strong></td><td>");
        builder.Append("None</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Next Invoice Date: </strong></td><td>");
        builder.Append(item.NextInvoiceDate.ToString() + "</td></tr>");
      }
      */
      
      
   /*   builder.Append("<tr><td><strong>Web Service ID: </strong></td><td>");
      builder.Append(organization.WebServiceID + "</td></tr>");

      string email = organization.SystemEmailID + "@teamsupport.com";
      builder.Append("<tr><td><strong>System Email: </strong></td><td>");
      builder.Append("<a href=\"mailto:" + email + "\">" + email + "</a>" + "</td></tr>");
      string portalLink = "http://portal.teamsupport.com?OrganizationID=" + organization.OrganizationID.ToString();
      portalLink = @"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>";
      builder.Append("<tr><td><strong>Portal Link: </strong></td><td>");
      builder.Append(portalLink + "</td></tr>");
      */
      builder.Append("<tr><td><strong>Total Bill: </strong></td><td>");
      double total = 0;
      total = (item.UserPrice == null ? GetPricePerUser(organization.ProductType) : (double)item.UserPrice) * Organizations.GetUserCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
      if (total < 0) total = 0;
      double portals = ((organization.PortalSeats - 10) * (item.PortalPrice == null ? 2.0 : (double)item.PortalPrice));
      if (portals < 0) portals = 0;
      total = total + portals;
      total = total + (organization.ExtraStorageUnits * (item.StoragePrice == null ? 3.0 : (double)item.StoragePrice));
      //if (organization.IsBasicPortal) total = total + 75;

      builder.Append("$" + total.ToString("#0.00") + "</td></tr>");
    }

    LoadAddress();
    if (item.CreditCardID != null) LoadCredit((int)item.CreditCardID);

    litAccount.Text = builder.ToString();
    
    
    
  }

  private double GetPricePerUser(ProductType type)
  {
    double result = 0.0;
    switch (type)
    {
      case ProductType.Express: result = 10.00; break;
      case ProductType.HelpDesk: result = 15.00; break;
      case ProductType.BugTracking: result = 15.00; break;
      case ProductType.Enterprise: result = 25.00; break;
      default:
        break;
    }
    return result;

  }

  private void  LoadAddress()
  {
    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(UserSession.LoginUser.OrganizationID, ReferenceType.BillingInfo);


    StringBuilder builder = new StringBuilder();

    if (!addresses.IsEmpty)
    {
      Address address = addresses[0];

      builder.Append(address.Addr1 + "<br />");
      if (address.Addr2.Trim() != "")
      {
        builder.Append(address.Addr2 + "<br />");
        if (address.Addr3.Trim() != "") builder.Append(address.Addr3 + "<br />");
      }
      
      builder.Append(address.City + ", " + address.State + " &nbsp&nbsp " + address.Zip + "<br />");
      builder.Append(address.Country);
    }
    else
    {
      builder.Append("No address on file.");
    }
    
    litAddress.Text = builder.ToString();
  }
  private void LoadCredit(int creditID)
  {
    CreditCard creditCard = (CreditCard) CreditCards.GetCreditCard(UserSession.LoginUser, creditID);
    StringBuilder builder = new StringBuilder();

    builder.Append("<tr><td style=\"width: 200px;\"><strong>Name: </strong></td><td>");
    builder.Append(creditCard.NameOnCard + "</td></tr>");
    builder.Append("<tr><td><strong>Number: </strong></td><td>");
    builder.Append(creditCard.DisplayNumber + "</td></tr>");
    builder.Append("<tr><td><strong>Type: </strong></td><td>");
    builder.Append(DataUtils.CreditCardTypeString(creditCard.CreditCardType) + "</td></tr>");
    builder.Append("<tr><td><strong>Security Code: </strong></td><td>");
    builder.Append(creditCard.SecurityCode + "</td></tr>");
    builder.Append("<tr><td><strong>Expiration Date: </strong></td><td>");
    builder.Append(creditCard.ExpirationDate.ToString() + "</td></tr>");
    
    litCredit.Text = builder.ToString();
  
  }
  private void VerifyBillingInfo()
  {
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    if (item == null)
    {
      BillingInfo info = new BillingInfo(UserSession.LoginUser);
      item = info.AddNewBillingInfoItem();
      item.IsAutomatic = true;
      item.NextInvoiceDate = DateTime.UtcNow;
      item.OrganizationID = UserSession.LoginUser.OrganizationID;
      item.Collection.Save();
    }
  }

}
