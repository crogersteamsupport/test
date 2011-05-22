using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class BillingInfoItem : BaseItem
  {
    private BillingInfo _billingInfo;
    
    public BillingInfoItem(DataRow row, BillingInfo billingInfo): base(row, billingInfo)
    {
      _billingInfo = billingInfo;
    }
	
    #region Properties
    
    public BillingInfo Collection
    {
      get { return _billingInfo; }
    }
        
    
    
    

    
    public int? CreditCardID
    {
      get { return Row["CreditCardID"] != DBNull.Value ? (int?)Row["CreditCardID"] : null; }
      set { Row["CreditCardID"] = CheckNull(value); }
    }
    
    public int? AddressID
    {
      get { return Row["AddressID"] != DBNull.Value ? (int?)Row["AddressID"] : null; }
      set { Row["AddressID"] = CheckNull(value); }
    }
    
    public double? UserPrice
    {
      get { return Row["UserPrice"] != DBNull.Value ? (double?)Row["UserPrice"] : null; }
      set { Row["UserPrice"] = CheckNull(value); }
    }
    
    public double? PortalPrice
    {
      get { return Row["PortalPrice"] != DBNull.Value ? (double?)Row["PortalPrice"] : null; }
      set { Row["PortalPrice"] = CheckNull(value); }
    }
    
    public double? BasicPortalPrice
    {
      get { return Row["BasicPortalPrice"] != DBNull.Value ? (double?)Row["BasicPortalPrice"] : null; }
      set { Row["BasicPortalPrice"] = CheckNull(value); }
    }
    
    public double? ChatPrice
    {
      get { return Row["ChatPrice"] != DBNull.Value ? (double?)Row["ChatPrice"] : null; }
      set { Row["ChatPrice"] = CheckNull(value); }
    }
    
    public double? StoragePrice
    {
      get { return Row["StoragePrice"] != DBNull.Value ? (double?)Row["StoragePrice"] : null; }
      set { Row["StoragePrice"] = CheckNull(value); }
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
    
    public bool IsAutomatic
    {
      get { return (bool)Row["IsAutomatic"]; }
      set { Row["IsAutomatic"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckNull(value); }
    }
    
    public DateTime NextInvoiceDate
    {
      get { return DateToLocal((DateTime)Row["NextInvoiceDate"]); }
      set { Row["NextInvoiceDate"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class BillingInfo : BaseCollection, IEnumerable<BillingInfoItem>
  {
    public BillingInfo(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "BillingInfo"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public BillingInfoItem this[int index]
    {
      get { return new BillingInfoItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(BillingInfoItem billingInfoItem);
    partial void AfterRowInsert(BillingInfoItem billingInfoItem);
    partial void BeforeRowEdit(BillingInfoItem billingInfoItem);
    partial void AfterRowEdit(BillingInfoItem billingInfoItem);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public BillingInfoItemProxy[] GetBillingInfoItemProxies()
    {
      List<BillingInfoItemProxy> list = new List<BillingInfoItemProxy>();

      foreach (BillingInfoItem item in this)
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
        deleteCommand.CommandText = "uspGeneratedDeleteBillingInfoItem";
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
		//SqlTransaction transaction = connection.BeginTransaction("BillingInfoSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateBillingInfoItem";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreditCardID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AddressID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsAutomatic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("BasicPortalPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChatPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("StoragePrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("NextInvoiceDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertBillingInfoItem";

		
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
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("NextInvoiceDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("StoragePrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChatPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("BasicPortalPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserPrice", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsAutomatic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AddressID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreditCardID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandType = CommandType.StoredProcedure;
		deleteCommand.CommandText = "uspGeneratedDeleteBillingInfoItem";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (BillingInfoItem billingInfoItem in this)
		  {
			if (billingInfoItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(billingInfoItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = billingInfoItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				billingInfoItem.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(billingInfoItem);
			}
			else if (billingInfoItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(billingInfoItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = billingInfoItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(billingInfoItem);
			}
			else if (billingInfoItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)billingInfoItem.Row["OrganizationID", DataRowVersion.Original];
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

      foreach (BillingInfoItem billingInfoItem in this)
      {
        if (billingInfoItem.Row.Table.Columns.Contains("CreatorID") && (int)billingInfoItem.Row["CreatorID"] == 0) billingInfoItem.Row["CreatorID"] = LoginUser.UserID;
        if (billingInfoItem.Row.Table.Columns.Contains("ModifierID")) billingInfoItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public BillingInfoItem FindByOrganizationID(int organizationID)
    {
      foreach (BillingInfoItem billingInfoItem in this)
      {
        if (billingInfoItem.OrganizationID == organizationID)
        {
          return billingInfoItem;
        }
      }
      return null;
    }

    public virtual BillingInfoItem AddNewBillingInfoItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("BillingInfo");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new BillingInfoItem(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectBillingInfoItem";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static BillingInfoItem GetBillingInfoItem(LoginUser loginUser, int organizationID)
    {
      BillingInfo billingInfo = new BillingInfo(loginUser);
      billingInfo.LoadByOrganizationID(organizationID);
      if (billingInfo.IsEmpty)
        return null;
      else
        return billingInfo[0];
    }
    
    
    

    #endregion

    #region IEnumerable<BillingInfoItem> Members

    public IEnumerator<BillingInfoItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new BillingInfoItem(row, this);
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
