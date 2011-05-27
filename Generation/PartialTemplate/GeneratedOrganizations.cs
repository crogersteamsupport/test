using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Organization : BaseItem
  {
    private Organizations _organizations;
    
    public Organization(DataRow row, Organizations organizations): base(row, organizations)
    {
      _organizations = organizations;
    }
	
    #region Properties
    
    public Organizations Collection
    {
      get { return _organizations; }
    }
        
    
    
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckNull(value); }
    }
    
    public string Website
    {
      get { return Row["Website"] != DBNull.Value ? (string)Row["Website"] : null; }
      set { Row["Website"] = CheckNull(value); }
    }
    
    public string WhereHeard
    {
      get { return Row["WhereHeard"] != DBNull.Value ? (string)Row["WhereHeard"] : null; }
      set { Row["WhereHeard"] = CheckNull(value); }
    }
    
    public string PromoCode
    {
      get { return Row["PromoCode"] != DBNull.Value ? (string)Row["PromoCode"] : null; }
      set { Row["PromoCode"] = CheckNull(value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckNull(value); }
    }
    
    public string TimeZoneID
    {
      get { return Row["TimeZoneID"] != DBNull.Value ? (string)Row["TimeZoneID"] : null; }
      set { Row["TimeZoneID"] = CheckNull(value); }
    }
    
    public string InActiveReason
    {
      get { return Row["InActiveReason"] != DBNull.Value ? (string)Row["InActiveReason"] : null; }
      set { Row["InActiveReason"] = CheckNull(value); }
    }
    
    public int? PrimaryUserID
    {
      get { return Row["PrimaryUserID"] != DBNull.Value ? (int?)Row["PrimaryUserID"] : null; }
      set { Row["PrimaryUserID"] = CheckNull(value); }
    }
    
    public int? DefaultPortalGroupID
    {
      get { return Row["DefaultPortalGroupID"] != DBNull.Value ? (int?)Row["DefaultPortalGroupID"] : null; }
      set { Row["DefaultPortalGroupID"] = CheckNull(value); }
    }
    
    public int? DefaultSupportGroupID
    {
      get { return Row["DefaultSupportGroupID"] != DBNull.Value ? (int?)Row["DefaultSupportGroupID"] : null; }
      set { Row["DefaultSupportGroupID"] = CheckNull(value); }
    }
    
    public int? DefaultSupportUserID
    {
      get { return Row["DefaultSupportUserID"] != DBNull.Value ? (int?)Row["DefaultSupportUserID"] : null; }
      set { Row["DefaultSupportUserID"] = CheckNull(value); }
    }
    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckNull(value); }
    }
    
    public string CRMLinkID
    {
      get { return Row["CRMLinkID"] != DBNull.Value ? (string)Row["CRMLinkID"] : null; }
      set { Row["CRMLinkID"] = CheckNull(value); }
    }
    
    public bool? RequireKnownUserForNewEmail
    {
      get { return Row["RequireKnownUserForNewEmail"] != DBNull.Value ? (bool?)Row["RequireKnownUserForNewEmail"] : null; }
      set { Row["RequireKnownUserForNewEmail"] = CheckNull(value); }
    }
    
    public string EmailDelimiter
    {
      get { return Row["EmailDelimiter"] != DBNull.Value ? (string)Row["EmailDelimiter"] : null; }
      set { Row["EmailDelimiter"] = CheckNull(value); }
    }
    
    public string OrganizationReplyToAddress
    {
      get { return Row["OrganizationReplyToAddress"] != DBNull.Value ? (string)Row["OrganizationReplyToAddress"] : null; }
      set { Row["OrganizationReplyToAddress"] = CheckNull(value); }
    }
    
    public string CompanyDomains
    {
      get { return Row["CompanyDomains"] != DBNull.Value ? (string)Row["CompanyDomains"] : null; }
      set { Row["CompanyDomains"] = CheckNull(value); }
    }
    
    public int? DefaultWikiArticleID
    {
      get { return Row["DefaultWikiArticleID"] != DBNull.Value ? (int?)Row["DefaultWikiArticleID"] : null; }
      set { Row["DefaultWikiArticleID"] = CheckNull(value); }
    }
    
    public int? SlaLevelID
    {
      get { return Row["SlaLevelID"] != DBNull.Value ? (int?)Row["SlaLevelID"] : null; }
      set { Row["SlaLevelID"] = CheckNull(value); }
    }
    
    public int? InternalSlaLevelID
    {
      get { return Row["InternalSlaLevelID"] != DBNull.Value ? (int?)Row["InternalSlaLevelID"] : null; }
      set { Row["InternalSlaLevelID"] = CheckNull(value); }
    }
    
    public bool? UseEuropeDate
    {
      get { return Row["UseEuropeDate"] != DBNull.Value ? (bool?)Row["UseEuropeDate"] : null; }
      set { Row["UseEuropeDate"] = CheckNull(value); }
    }
    
    public bool? MatchEmailSubject
    {
      get { return Row["MatchEmailSubject"] != DBNull.Value ? (bool?)Row["MatchEmailSubject"] : null; }
      set { Row["MatchEmailSubject"] = CheckNull(value); }
    }
    
    public string PrimaryInterest
    {
      get { return Row["PrimaryInterest"] != DBNull.Value ? (string)Row["PrimaryInterest"] : null; }
      set { Row["PrimaryInterest"] = CheckNull(value); }
    }
    
    public string PotentialSeats
    {
      get { return Row["PotentialSeats"] != DBNull.Value ? (string)Row["PotentialSeats"] : null; }
      set { Row["PotentialSeats"] = CheckNull(value); }
    }
    
    public string EvalProcess
    {
      get { return Row["EvalProcess"] != DBNull.Value ? (string)Row["EvalProcess"] : null; }
      set { Row["EvalProcess"] = CheckNull(value); }
    }
    
    public bool? AddAdditionalContacts
    {
      get { return Row["AddAdditionalContacts"] != DBNull.Value ? (bool?)Row["AddAdditionalContacts"] : null; }
      set { Row["AddAdditionalContacts"] = CheckNull(value); }
    }
    
    public bool? ChangeStatusIfClosed
    {
      get { return Row["ChangeStatusIfClosed"] != DBNull.Value ? (bool?)Row["ChangeStatusIfClosed"] : null; }
      set { Row["ChangeStatusIfClosed"] = CheckNull(value); }
    }
    

    
    public bool IsPublicArticles
    {
      get { return (bool)Row["IsPublicArticles"]; }
      set { Row["IsPublicArticles"] = CheckNull(value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckNull(value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckNull(value); }
    }
    
    public bool TimedActionsRequired
    {
      get { return (bool)Row["TimedActionsRequired"]; }
      set { Row["TimedActionsRequired"] = CheckNull(value); }
    }
    
    public string CultureName
    {
      get { return (string)Row["CultureName"]; }
      set { Row["CultureName"] = CheckNull(value); }
    }
    
    public int BusinessDays
    {
      get { return (int)Row["BusinessDays"]; }
      set { Row["BusinessDays"] = CheckNull(value); }
    }
    
    public bool ShowWiki
    {
      get { return (bool)Row["ShowWiki"]; }
      set { Row["ShowWiki"] = CheckNull(value); }
    }
    
    public bool AdminOnlyReports
    {
      get { return (bool)Row["AdminOnlyReports"]; }
      set { Row["AdminOnlyReports"] = CheckNull(value); }
    }
    
    public bool AdminOnlyCustomers
    {
      get { return (bool)Row["AdminOnlyCustomers"]; }
      set { Row["AdminOnlyCustomers"] = CheckNull(value); }
    }
    
    public bool RequireNewKeyword
    {
      get { return (bool)Row["RequireNewKeyword"]; }
      set { Row["RequireNewKeyword"] = CheckNull(value); }
    }
    
    public int APIRequestLimit
    {
      get { return (int)Row["APIRequestLimit"]; }
      set { Row["APIRequestLimit"] = CheckNull(value); }
    }
    
    public Guid PortalGuid
    {
      get { return (Guid)Row["PortalGuid"]; }
      set { Row["PortalGuid"] = CheckNull(value); }
    }
    
    public Guid ChatID
    {
      get { return (Guid)Row["ChatID"]; }
      set { Row["ChatID"] = CheckNull(value); }
    }
    
    public Guid SystemEmailID
    {
      get { return (Guid)Row["SystemEmailID"]; }
      set { Row["SystemEmailID"] = CheckNull(value); }
    }
    
    public Guid WebServiceID
    {
      get { return (Guid)Row["WebServiceID"]; }
      set { Row["WebServiceID"] = CheckNull(value); }
    }
    
    public ProductType ProductType
    {
      get { return (ProductType)Row["ProductType"]; }
      set { Row["ProductType"] = CheckNull(value); }
    }
    
    public bool IsBasicPortal
    {
      get { return (bool)Row["IsBasicPortal"]; }
      set { Row["IsBasicPortal"] = CheckNull(value); }
    }
    
    public bool IsAdvancedPortal
    {
      get { return (bool)Row["IsAdvancedPortal"]; }
      set { Row["IsAdvancedPortal"] = CheckNull(value); }
    }
    
    public bool HasPortalAccess
    {
      get { return (bool)Row["HasPortalAccess"]; }
      set { Row["HasPortalAccess"] = CheckNull(value); }
    }
    
    public bool IsInventoryEnabled
    {
      get { return (bool)Row["IsInventoryEnabled"]; }
      set { Row["IsInventoryEnabled"] = CheckNull(value); }
    }
    
    public bool IsApiEnabled
    {
      get { return (bool)Row["IsApiEnabled"]; }
      set { Row["IsApiEnabled"] = CheckNull(value); }
    }
    
    public bool IsApiActive
    {
      get { return (bool)Row["IsApiActive"]; }
      set { Row["IsApiActive"] = CheckNull(value); }
    }
    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckNull(value); }
    }
    
    public int ExtraStorageUnits
    {
      get { return (int)Row["ExtraStorageUnits"]; }
      set { Row["ExtraStorageUnits"] = CheckNull(value); }
    }
    
    public int ChatSeats
    {
      get { return (int)Row["ChatSeats"]; }
      set { Row["ChatSeats"] = CheckNull(value); }
    }
    
    public int PortalSeats
    {
      get { return (int)Row["PortalSeats"]; }
      set { Row["PortalSeats"] = CheckNull(value); }
    }
    
    public int UserSeats
    {
      get { return (int)Row["UserSeats"]; }
      set { Row["UserSeats"] = CheckNull(value); }
    }
    
    public bool IsCustomerFree
    {
      get { return (bool)Row["IsCustomerFree"]; }
      set { Row["IsCustomerFree"] = CheckNull(value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? SAExpirationDate
    {
      get { return Row["SAExpirationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["SAExpirationDate"]) : null; }
      set { Row["SAExpirationDate"] = CheckNull(value); }
    }
    

    
    public DateTime BusinessDayEnd
    {
      get { return DateToLocal((DateTime)Row["BusinessDayEnd"]); }
      set { Row["BusinessDayEnd"] = CheckNull(value); }
    }
    
    public DateTime BusinessDayStart
    {
      get { return DateToLocal((DateTime)Row["BusinessDayStart"]); }
      set { Row["BusinessDayStart"] = CheckNull(value); }
    }
    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckNull(value); }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class Organizations : BaseCollection, IEnumerable<Organization>
  {
    public Organizations(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Organizations"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public Organization this[int index]
    {
      get { return new Organization(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Organization organization);
    partial void AfterRowInsert(Organization organization);
    partial void BeforeRowEdit(Organization organization);
    partial void AfterRowEdit(Organization organization);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public OrganizationProxy[] GetOrganizationProxies()
    {
      List<OrganizationProxy> list = new List<OrganizationProxy>();

      foreach (Organization item in this)
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
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteOrganization";
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
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateOrganization";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WhereHeard", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PromoCode", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCustomerFree", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChatSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsApiActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsApiEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsInventoryEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeZoneID", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsAdvancedPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsBasicPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultPortalGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("WebServiceID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SystemEmailID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChatID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("APIRequestLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequireNewKeyword", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequireKnownUserForNewEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailDelimiter", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationReplyToAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AdminOnlyCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AdminOnlyReports", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShowWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("InternalSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("BusinessDays", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("BusinessDayStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("BusinessDayEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseEuropeDate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CultureName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimedActionsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MatchEmailSubject", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryInterest", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PotentialSeats", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EvalProcess", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AddAdditionalContacts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChangeStatusIfClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPublicArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertOrganization";

		
		tempParameter = insertCommand.Parameters.Add("IsPublicArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChangeStatusIfClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AddAdditionalContacts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EvalProcess", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PotentialSeats", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryInterest", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MatchEmailSubject", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimedActionsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CultureName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseEuropeDate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BusinessDayEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("BusinessDayStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("BusinessDays", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("InternalSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShowWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AdminOnlyReports", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AdminOnlyCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationReplyToAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailDelimiter", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequireKnownUserForNewEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequireNewKeyword", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("APIRequestLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChatID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SystemEmailID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("WebServiceID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultPortalGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsBasicPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsAdvancedPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeZoneID", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsInventoryEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsApiEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsApiActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChatSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsCustomerFree", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PromoCode", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("WhereHeard", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.StoredProcedure;
		deleteCommand.CommandText = "uspGeneratedDeleteOrganization";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (Organization organization in this)
		  {
			if (organization.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organization);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organization.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organization.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organization);
			}
			else if (organization.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organization);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organization.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organization);
			}
			else if (organization.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organization.Row["OrganizationID", DataRowVersion.Original];
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

      foreach (Organization organization in this)
      {
        if (organization.Row.Table.Columns.Contains("CreatorID") && (int)organization.Row["CreatorID"] == 0) organization.Row["CreatorID"] = LoginUser.UserID;
        if (organization.Row.Table.Columns.Contains("ModifierID")) organization.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Organization FindByOrganizationID(int organizationID)
    {
      foreach (Organization organization in this)
      {
        if (organization.OrganizationID == organizationID)
        {
          return organization;
        }
      }
      return null;
    }

    public virtual Organization AddNewOrganization()
    {
      if (Table.Columns.Count < 1) LoadColumns("Organizations");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Organization(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectOrganization";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static Organization GetOrganization(LoginUser loginUser, int organizationID)
    {
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByOrganizationID(organizationID);
      if (organizations.IsEmpty)
        return null;
      else
        return organizations[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Organization> Members

    public IEnumerator<Organization> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Organization(row, this);
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
