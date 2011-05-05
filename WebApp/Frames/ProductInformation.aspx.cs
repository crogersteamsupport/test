using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Data;

public partial class Frames_ProductInformation : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    int productID;

    try
    {
      productID = int.Parse(Request["ProductID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
      
    }

    lblProperties.Visible = true;

    Product product = (Product)Products.GetProduct(UserSession.LoginUser, productID);
    lblProperties.Visible = product == null;

    if (product == null) return;

    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    table.Rows.Add(new string[] { "Name:", product.Name });
    table.Rows.Add(new string[] { "Description:", product.Description });
    table.Rows.Add(new string[] { "Product ID:", product.ProductID.ToString()});

    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.Products);

    foreach (CustomField field in fields)
    {
      CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, productID);
      table.Rows.Add(new string[] { field.Name + ":", value.Value });
    }

    rptProperties.DataSource = table;
    rptProperties.DataBind();

  }


}
