using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Asset : BaseItem
  {
    private Assets _assets;
    
    public Asset(DataRow row, Assets assets): base(row, assets)
    {
      _assets = assets;
    }
	
    #region Properties
    
    public Assets Collection
    {
      get { return _assets; }
    }
        
    
    
    
    public int AssetID
    {
      get { return (int)Row["AssetID"]; }
    }
    

    
    public string SerialNumber
    {
      get { return Row["SerialNumber"] != DBNull.Value ? (string)Row["SerialNumber"] : null; }
      set { Row["SerialNumber"] = CheckNull(value); }
    }
    
    public string Name
    {
      get { return Row["Name"] != DBNull.Value ? (string)Row["Name"] : null; }
      set { Row["Name"] = CheckNull(value); }
    }
    
    public string Location
    {
      get { return Row["Location"] != DBNull.Value ? (string)Row["Location"] : null; }
      set { Row["Location"] = CheckNull(value); }
    }
    
    public string Notes
    {
      get { return Row["Notes"] != DBNull.Value ? (string)Row["Notes"] : null; }
      set { Row["Notes"] = CheckNull(value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckNull(value); }
    }
    
    public int? AssignedTo
    {
      get { return Row["AssignedTo"] != DBNull.Value ? (int?)Row["AssignedTo"] : null; }
      set { Row["AssignedTo"] = CheckNull(value); }
    }
    
    public int? CreatorID
    {
      get { return Row["CreatorID"] != DBNull.Value ? (int?)Row["CreatorID"] : null; }
      set { Row["CreatorID"] = CheckNull(value); }
    }
    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckNull(value); }
    }
    
    public int? SubPartOf
    {
      get { return Row["SubPartOf"] != DBNull.Value ? (int?)Row["SubPartOf"] : null; }
      set { Row["SubPartOf"] = CheckNull(value); }
    }
    
    public string Status
    {
      get { return Row["Status"] != DBNull.Value ? (string)Row["Status"] : null; }
      set { Row["Status"] = CheckNull(value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckNull(value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? WarrantyExpiration
    {
      get { return Row["WarrantyExpiration"] != DBNull.Value ? DateToLocal((DateTime?)Row["WarrantyExpiration"]) : null; }
      set { Row["WarrantyExpiration"] = CheckNull(value); }
    }

    public DateTime? WarrantyExpirationUtc
    {
      get { return Row["WarrantyExpiration"] != DBNull.Value ? (DateTime?)Row["WarrantyExpiration"] : null; }
    }
    
    public DateTime? DateCreated
    {
      get { return Row["DateCreated"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateCreated"]) : null; }
      set { Row["DateCreated"] = CheckNull(value); }
    }

    public DateTime? DateCreatedUtc
    {
      get { return Row["DateCreated"] != DBNull.Value ? (DateTime?)Row["DateCreated"] : null; }
    }
    
    public DateTime? DateModified
    {
      get { return Row["DateModified"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModified"]) : null; }
      set { Row["DateModified"] = CheckNull(value); }
    }

    public DateTime? DateModifiedUtc
    {
      get { return Row["DateModified"] != DBNull.Value ? (DateTime?)Row["DateModified"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class Assets : BaseCollection, IEnumerable<Asset>
  {
    public Assets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Assets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AssetID"; }
    }



    public Asset this[int index]
    {
      get { return new Asset(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Asset asset);
    partial void AfterRowInsert(Asset asset);
    partial void BeforeRowEdit(Asset asset);
    partial void AfterRowEdit(Asset asset);
    partial void BeforeRowDelete(int assetID);
    partial void AfterRowDelete(int assetID);    

    partial void BeforeDBDelete(int assetID);
    partial void AfterDBDelete(int assetID);    

    #endregion

    #region Public Methods

    public AssetProxy[] GetAssetProxies()
    {
      List<AssetProxy> list = new List<AssetProxy>();

      foreach (Asset item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int assetID)
    {
      BeforeDBDelete(assetID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Assets] WHERE ([AssetID] = @AssetID);";
        deleteCommand.Parameters.Add("AssetID", SqlDbType.Int);
        deleteCommand.Parameters["AssetID"].Value = assetID;

        BeforeRowDelete(assetID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(assetID);
      }
      AfterDBDelete(assetID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Assets] SET     [OrganizationID] = @OrganizationID,    [SerialNumber] = @SerialNumber,    [Name] = @Name,    [Location] = @Location,    [Notes] = @Notes,    [ProductID] = @ProductID,    [WarrantyExpiration] = @WarrantyExpiration,    [AssignedTo] = @AssignedTo,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [SubPartOf] = @SubPartOf,    [Status] = @Status,    [ImportID] = @ImportID  WHERE ([AssetID] = @AssetID);";

		
		tempParameter = updateCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SerialNumber", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Location", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Notes", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarrantyExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("AssignedTo", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SubPartOf", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Assets] (    [OrganizationID],    [SerialNumber],    [Name],    [Location],    [Notes],    [ProductID],    [WarrantyExpiration],    [AssignedTo],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [SubPartOf],    [Status],    [ImportID]) VALUES ( @OrganizationID, @SerialNumber, @Name, @Location, @Notes, @ProductID, @WarrantyExpiration, @AssignedTo, @DateCreated, @DateModified, @CreatorID, @ModifierID, @SubPartOf, @Status, @ImportID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SubPartOf", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = insertCommand.Parameters.Add("AssignedTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("WarrantyExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Notes", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Location", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SerialNumber", SqlDbType.VarChar, 500);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Assets] WHERE ([AssetID] = @AssetID);";
		deleteCommand.Parameters.Add("AssetID", SqlDbType.Int);

		try
		{
		  foreach (Asset asset in this)
		  {
			if (asset.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(asset);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = asset.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AssetID"].AutoIncrement = false;
			  Table.Columns["AssetID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				asset.Row["AssetID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(asset);
			}
			else if (asset.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(asset);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = asset.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(asset);
			}
			else if (asset.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)asset.Row["AssetID", DataRowVersion.Original];
			  deleteCommand.Parameters["AssetID"].Value = id;
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

      foreach (Asset asset in this)
      {
        if (asset.Row.Table.Columns.Contains("CreatorID") && (int)asset.Row["CreatorID"] == 0) asset.Row["CreatorID"] = LoginUser.UserID;
        if (asset.Row.Table.Columns.Contains("ModifierID")) asset.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Asset FindByAssetID(int assetID)
    {
      foreach (Asset asset in this)
      {
        if (asset.AssetID == assetID)
        {
          return asset;
        }
      }
      return null;
    }

    public virtual Asset AddNewAsset()
    {
      if (Table.Columns.Count < 1) LoadColumns("Assets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Asset(row, this);
    }
    
    public virtual void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AssetID], [OrganizationID], [SerialNumber], [Name], [Location], [Notes], [ProductID], [WarrantyExpiration], [AssignedTo], [DateCreated], [DateModified], [CreatorID], [ModifierID], [SubPartOf], [Status], [ImportID] FROM [dbo].[Assets] WHERE ([AssetID] = @AssetID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AssetID", assetID);
        Fill(command);
      }
    }
    
    public static Asset GetAsset(LoginUser loginUser, int assetID)
    {
      Assets assets = new Assets(loginUser);
      assets.LoadByAssetID(assetID);
      if (assets.IsEmpty)
        return null;
      else
        return assets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Asset> Members

    public IEnumerator<Asset> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Asset(row, this);
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
