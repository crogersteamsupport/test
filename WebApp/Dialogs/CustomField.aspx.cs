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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Text;

public partial class Dialogs_CustomField : BaseDialogPage
{
  private int _customFieldID = -1;
  private int _auxID = -1;
  private ReferenceType _refType = ReferenceType.None;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["CustomFieldID"] != null)
    {
      _customFieldID = int.Parse(Request["CustomFieldID"]);
    }
    else
    {
      _refType = (ReferenceType)int.Parse(Request["RefType"]);
      if (Request["AuxID"] != null)
      {
        _auxID = int.Parse(Request["AuxID"]);
      }
      
    }


    if (!IsPostBack)
    {
      LoadFieldTypes();
      if (_customFieldID > -1) LoadCustomField(_customFieldID);
    }

    _manager.AjaxSettings.AddAjaxSetting(comboFieldType, divMain);

    pnlPickList.Visible = GetSelectedFieldType() == CustomFieldType.PickList;
    cbIsRequired.Visible = GetSelectedFieldType() != CustomFieldType.Boolean;

  }

  private CustomFieldType GetSelectedFieldType()
  {
    return (CustomFieldType)(int.Parse(comboFieldType.SelectedValue));
  }

  private void LoadFieldTypes()
  {
    comboFieldType.Items.Clear();
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Text), ((int)CustomFieldType.Text).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Number), ((int)CustomFieldType.Number).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.PickList), ((int)CustomFieldType.PickList).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.DateTime), ((int)CustomFieldType.DateTime).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Boolean), ((int)CustomFieldType.Boolean).ToString()));
  }

  private void LoadCustomField(int customFieldID)
  {
    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByCustomFieldID(customFieldID);
    if (fields.IsEmpty) return;

    if (fields[0].OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    textDescription.Text = fields[0].Description;
    textName.Text = fields[0].Name;
    textApiFieldName.Text = fields[0].ApiFieldName;
    textList.Text = fields[0].ListValues.Replace("|", "\n");
    comboFieldType.SelectedValue = ((int)fields[0].FieldType).ToString();
    cbIsVisibleOnPortal.Checked = fields[0].IsVisibleOnPortal == null ? false : (bool)fields[0].IsVisibleOnPortal;
    cbIsRequired.Checked = fields[0].IsRequired;
    cbFirstSelect.Checked = fields[0].IsFirstIndexSelect;
  }

  public override bool Save()
  {
    if (textName.Text.Trim() == "")
    {
      _manager.Alert("Please give the custom field a name.");
      return false;
    }

    string apiFieldName = GetApiFieldName();

    if (apiFieldName == "")
    {
      _manager.Alert("Please give the custom field an API field name.");
      return false;
    }

    if (FieldExists(apiFieldName)) return false;




    CustomFields fields = new CustomFields(UserSession.LoginUser);
    CustomField field;

    if (_customFieldID < 0)
    {
      field = fields.AddNewCustomField();
      field.OrganizationID = UserSession.LoginUser.OrganizationID;
      field.RefType = _refType;
      field.AuxID = _auxID;
      field.Position = fields.GetMaxPosition(UserSession.LoginUser.OrganizationID, _refType, _auxID) + 1;
    }
    else
    {
      fields.LoadByCustomFieldID(_customFieldID);
      if (fields.IsEmpty) return false;
      field = fields[0];
    }

    field.Description = textDescription.Text;
    field.Name = textName.Text;
    field.ApiFieldName = apiFieldName;
    field.FieldType = GetSelectedFieldType();
    field.IsVisibleOnPortal = cbIsVisibleOnPortal.Checked;
    field.IsFirstIndexSelect = cbFirstSelect.Checked;
    field.IsRequired = cbIsRequired.Checked;

    string list = textList.Text.Replace("\n", "|");
    if (list != "")
    {
      string[] items = list.Split('|');

      CommaDelimitedStringCollection collection = new CommaDelimitedStringCollection();

      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < items.Length; i++)
			{
    		 builder.Append(items[i].Trim());
         if (i < items.Length-1) builder.Append("|");
			}

      field.ListValues = builder.ToString();
    }
    else
    {
      field.ListValues = list;
    }
    field.Collection.Save();

    return true;
  }

  private string GetApiFieldName()
  {
    string name = textApiFieldName.Text.Trim();
    if (name == "") 
    {
      name = textName.Text;
    }
    return CustomFields.GenerateApiFieldName(name);
  }

  private bool FieldExists(string apiFieldName)
  {
    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

    foreach (CustomField field in fields)
    {
      if (field.ApiFieldName.Trim().ToLower() == apiFieldName.Trim().ToLower())
      {
        _manager.Alert("Please choose a unique API field name for this custom field.");
        return true;
      }

      if (field.Name.Trim().ToLower() == textName.Text.Trim().ToLower())
      {
        _manager.Alert("Please choose a unique name for this custom field.");
        return true;
      }
    }
    return false;
  }

}
