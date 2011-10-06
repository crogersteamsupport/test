using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PortalOption : BaseItem
  {
    private PortalOptions _portalOptions;
    
    public PortalOption(DataRow row, PortalOptions portalOptions): base(row, portalOptions)
    {
      _portalOptions = portalOptions;
    }
	
    #region Properties
    
    public PortalOptions Collection
    {
      get { return _portalOptions; }
    }
        
    
    
    

    
    public string PortalHTMLHeader
    {
      get { return Row["PortalHTMLHeader"] != DBNull.Value ? (string)Row["PortalHTMLHeader"] : null; }
      set { Row["PortalHTMLHeader"] = CheckNull(value); }
    }
    
    public string PortalHTMLFooter
    {
      get { return Row["PortalHTMLFooter"] != DBNull.Value ? (string)Row["PortalHTMLFooter"] : null; }
      set { Row["PortalHTMLFooter"] = CheckNull(value); }
    }
    
    public bool? UseRecaptcha
    {
      get { return Row["UseRecaptcha"] != DBNull.Value ? (bool?)Row["UseRecaptcha"] : null; }
      set { Row["UseRecaptcha"] = CheckNull(value); }
    }
    
    public string FontFamily
    {
      get { return Row["FontFamily"] != DBNull.Value ? (string)Row["FontFamily"] : null; }
      set { Row["FontFamily"] = CheckNull(value); }
    }
    
    public string FontColor
    {
      get { return Row["FontColor"] != DBNull.Value ? (string)Row["FontColor"] : null; }
      set { Row["FontColor"] = CheckNull(value); }
    }
    
    public string PageBackgroundColor
    {
      get { return Row["PageBackgroundColor"] != DBNull.Value ? (string)Row["PageBackgroundColor"] : null; }
      set { Row["PageBackgroundColor"] = CheckNull(value); }
    }
    
    public bool? UseCompanyInBasic
    {
      get { return Row["UseCompanyInBasic"] != DBNull.Value ? (bool?)Row["UseCompanyInBasic"] : null; }
      set { Row["UseCompanyInBasic"] = CheckNull(value); }
    }
    
    public bool? CompanyRequiredInBasic
    {
      get { return Row["CompanyRequiredInBasic"] != DBNull.Value ? (bool?)Row["CompanyRequiredInBasic"] : null; }
      set { Row["CompanyRequiredInBasic"] = CheckNull(value); }
    }
    
    public bool? HideUserAssignedTo
    {
      get { return Row["HideUserAssignedTo"] != DBNull.Value ? (bool?)Row["HideUserAssignedTo"] : null; }
      set { Row["HideUserAssignedTo"] = CheckNull(value); }
    }
    
    public bool? HideGroupAssignedTo
    {
      get { return Row["HideGroupAssignedTo"] != DBNull.Value ? (bool?)Row["HideGroupAssignedTo"] : null; }
      set { Row["HideGroupAssignedTo"] = CheckNull(value); }
    }
    
    public int? BasicPortalColumnWidth
    {
      get { return Row["BasicPortalColumnWidth"] != DBNull.Value ? (int?)Row["BasicPortalColumnWidth"] : null; }
      set { Row["BasicPortalColumnWidth"] = CheckNull(value); }
    }
    
    public bool? DisplayGroups
    {
      get { return Row["DisplayGroups"] != DBNull.Value ? (bool?)Row["DisplayGroups"] : null; }
      set { Row["DisplayGroups"] = CheckNull(value); }
    }
    
    public string PortalName
    {
      get { return Row["PortalName"] != DBNull.Value ? (string)Row["PortalName"] : null; }
      set { Row["PortalName"] = CheckNull(value); }
    }
    
    public bool? KBAccess
    {
      get { return Row["KBAccess"] != DBNull.Value ? (bool?)Row["KBAccess"] : null; }
      set { Row["KBAccess"] = CheckNull(value); }
    }
    
    public bool? DisplayProducts
    {
      get { return Row["DisplayProducts"] != DBNull.Value ? (bool?)Row["DisplayProducts"] : null; }
      set { Row["DisplayProducts"] = CheckNull(value); }
    }
    
    public bool? HonorSupportExpiration
    {
      get { return Row["HonorSupportExpiration"] != DBNull.Value ? (bool?)Row["HonorSupportExpiration"] : null; }
      set { Row["HonorSupportExpiration"] = CheckNull(value); }
    }
    
    public bool? HideCloseButton
    {
      get { return Row["HideCloseButton"] != DBNull.Value ? (bool?)Row["HideCloseButton"] : null; }
      set { Row["HideCloseButton"] = CheckNull(value); }
    }
    
    public string Theme
    {
      get { return Row["Theme"] != DBNull.Value ? (string)Row["Theme"] : null; }
      set { Row["Theme"] = CheckNull(value); }
    }
    
    public int? AdvPortalWidth
    {
      get { return Row["AdvPortalWidth"] != DBNull.Value ? (int?)Row["AdvPortalWidth"] : null; }
      set { Row["AdvPortalWidth"] = CheckNull(value); }
    }
    
    public string BasicPortalDirections
    {
      get { return Row["BasicPortalDirections"] != DBNull.Value ? (string)Row["BasicPortalDirections"] : null; }
      set { Row["BasicPortalDirections"] = CheckNull(value); }
    }
    
    public bool? DeflectionEnabled
    {
      get { return Row["DeflectionEnabled"] != DBNull.Value ? (bool?)Row["DeflectionEnabled"] : null; }
      set { Row["DeflectionEnabled"] = CheckNull(value); }
    }
    
    public bool? DisplayForum
    {
      get { return Row["DisplayForum"] != DBNull.Value ? (bool?)Row["DisplayForum"] : null; }
      set { Row["DisplayForum"] = CheckNull(value); }
    }
    

    
    public bool DisplayAdvKB
    {
      get { return (bool)Row["DisplayAdvKB"]; }
      set { Row["DisplayAdvKB"] = CheckNull(value); }
    }
    
    public bool DisplayAdvProducts
    {
      get { return (bool)Row["DisplayAdvProducts"]; }
      set { Row["DisplayAdvProducts"] = CheckNull(value); }
    }
    
    public bool DisplayPortalPhone
    {
      get { return (bool)Row["DisplayPortalPhone"]; }
      set { Row["DisplayPortalPhone"] = CheckNull(value); }
    }
    
    public bool DisplayFooter
    {
      get { return (bool)Row["DisplayFooter"]; }
      set { Row["DisplayFooter"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class PortalOptions : BaseCollection, IEnumerable<PortalOption>
  {
    public PortalOptions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PortalOptions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public PortalOption this[int index]
    {
      get { return new PortalOption(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PortalOption portalOption);
    partial void AfterRowInsert(PortalOption portalOption);
    partial void BeforeRowEdit(PortalOption portalOption);
    partial void AfterRowEdit(PortalOption portalOption);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public PortalOptionProxy[] GetPortalOptionProxies()
    {
      List<PortalOptionProxy> list = new List<PortalOptionProxy>();

      foreach (PortalOption item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
      BeforeDBDelete(organizationID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeRowDelete(organizationID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(organizationID);
      }
      AfterDBDelete(organizationID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PortalOptionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PortalOptions] SET     [PortalHTMLHeader] = @PortalHTMLHeader,    [PortalHTMLFooter] = @PortalHTMLFooter,    [UseRecaptcha] = @UseRecaptcha,    [FontFamily] = @FontFamily,    [FontColor] = @FontColor,    [PageBackgroundColor] = @PageBackgroundColor,    [UseCompanyInBasic] = @UseCompanyInBasic,    [CompanyRequiredInBasic] = @CompanyRequiredInBasic,    [HideUserAssignedTo] = @HideUserAssignedTo,    [HideGroupAssignedTo] = @HideGroupAssignedTo,    [BasicPortalColumnWidth] = @BasicPortalColumnWidth,    [DisplayGroups] = @DisplayGroups,    [PortalName] = @PortalName,    [KBAccess] = @KBAccess,    [DisplayProducts] = @DisplayProducts,    [HonorSupportExpiration] = @HonorSupportExpiration,    [HideCloseButton] = @HideCloseButton,    [Theme] = @Theme,    [AdvPortalWidth] = @AdvPortalWidth,    [BasicPortalDirections] = @BasicPortalDirections,    [DeflectionEnabled] = @DeflectionEnabled,    [DisplayForum] = @DisplayForum,    [DisplayFooter] = @DisplayFooter,    [DisplayPortalPhone] = @DisplayPortalPhone,    [DisplayAdvProducts] = @DisplayAdvProducts,    [DisplayAdvKB] = @DisplayAdvKB  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalHTMLHeader", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalHTMLFooter", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseRecaptcha", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PageBackgroundColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseCompanyInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompanyRequiredInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideUserAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideGroupAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("BasicPortalColumnWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayGroups", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KBAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HonorSupportExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HideCloseButton", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Theme", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AdvPortalWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("BasicPortalDirections", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DeflectionEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayForum", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayFooter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayPortalPhone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayAdvProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayAdvKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PortalOptions] (    [OrganizationID],    [PortalHTMLHeader],    [PortalHTMLFooter],    [UseRecaptcha],    [FontFamily],    [FontColor],    [PageBackgroundColor],    [UseCompanyInBasic],    [CompanyRequiredInBasic],    [HideUserAssignedTo],    [HideGroupAssignedTo],    [BasicPortalColumnWidth],    [DisplayGroups],    [PortalName],    [KBAccess],    [DisplayProducts],    [HonorSupportExpiration],    [HideCloseButton],    [Theme],    [AdvPortalWidth],    [BasicPortalDirections],    [DeflectionEnabled],    [DisplayForum],    [DisplayFooter],    [DisplayPortalPhone],    [DisplayAdvProducts],    [DisplayAdvKB]) VALUES ( @OrganizationID, @PortalHTMLHeader, @PortalHTMLFooter, @UseRecaptcha, @FontFamily, @FontColor, @PageBackgroundColor, @UseCompanyInBasic, @CompanyRequiredInBasic, @HideUserAssignedTo, @HideGroupAssignedTo, @BasicPortalColumnWidth, @DisplayGroups, @PortalName, @KBAccess, @DisplayProducts, @HonorSupportExpiration, @HideCloseButton, @Theme, @AdvPortalWidth, @BasicPortalDirections, @DeflectionEnabled, @DisplayForum, @DisplayFooter, @DisplayPortalPhone, @DisplayAdvProducts, @DisplayAdvKB); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DisplayAdvKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayAdvProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayPortalPhone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayFooter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayForum", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DeflectionEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BasicPortalDirections", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AdvPortalWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Theme", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideCloseButton", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HonorSupportExpiration", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("KBAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayGroups", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BasicPortalColumnWidth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideGroupAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HideUserAssignedTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyRequiredInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseCompanyInBasic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PageBackgroundColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseRecaptcha", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalHTMLFooter", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalHTMLHeader", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (PortalOption portalOption in this)
		  {
			if (portalOption.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(portalOption);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = portalOption.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				portalOption.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(portalOption);
			}
			else if (portalOption.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(portalOption);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = portalOption.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(portalOption);
			}
			else if (portalOption.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)portalOption.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
			  BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
			  AfterRowDelete(id);
			}
		  }
		  //transaction.Commit();
		}
		catch (Exception)
		{
		  //transaction.Rollback();
		  throw;
		}
		Table.AcceptChanges();
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public void BulkSave()
    {

      foreach (PortalOption portalOption in this)
      {
        if (portalOption.Row.Table.Columns.Contains("CreatorID") && (int)portalOption.Row["CreatorID"] == 0) portalOption.Row["CreatorID"] = LoginUser.UserID;
        if (portalOption.Row.Table.Columns.Contains("ModifierID")) portalOption.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PortalOption FindByOrganizationID(int organizationID)
    {
      foreach (PortalOption portalOption in this)
      {
        if (portalOption.OrganizationID == organizationID)
        {
          return portalOption;
        }
      }
      return null;
    }

    public virtual PortalOption AddNewPortalOption()
    {
      if (Table.Columns.Count < 1) LoadColumns("PortalOptions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PortalOption(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [PortalHTMLHeader], [PortalHTMLFooter], [UseRecaptcha], [FontFamily], [FontColor], [PageBackgroundColor], [UseCompanyInBasic], [CompanyRequiredInBasic], [HideUserAssignedTo], [HideGroupAssignedTo], [BasicPortalColumnWidth], [DisplayGroups], [PortalName], [KBAccess], [DisplayProducts], [HonorSupportExpiration], [HideCloseButton], [Theme], [AdvPortalWidth], [BasicPortalDirections], [DeflectionEnabled], [DisplayForum], [DisplayFooter], [DisplayPortalPhone], [DisplayAdvProducts], [DisplayAdvKB] FROM [dbo].[PortalOptions] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static PortalOption GetPortalOption(LoginUser loginUser, int organizationID)
    {
      PortalOptions portalOptions = new PortalOptions(loginUser);
      portalOptions.LoadByOrganizationID(organizationID);
      if (portalOptions.IsEmpty)
        return null;
      else
        return portalOptions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PortalOption> Members

    public IEnumerator<PortalOption> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PortalOption(row, this);
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
