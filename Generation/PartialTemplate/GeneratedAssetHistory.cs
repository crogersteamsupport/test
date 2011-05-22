using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AssetHistoryItem : BaseItem
  {
    private AssetHistory _assetHistory;
    
    public AssetHistoryItem(DataRow row, AssetHistory assetHistory): base(row, assetHistory)
    {
      _assetHistory = assetHistory;
    }
	
    #region Properties
    
    public AssetHistory Collection
    {
      get { return _assetHistory; }
    }
        
    
    
    
    public int HistoryID
    {
      get { return (int)Row["HistoryID"]; }
    }
    

    
    public string ActionDescription
    {
      get { return Row["ActionDescription"] != DBNull.Value ? (string)Row["ActionDescription"] : null; }
      set { Row["ActionDescription"] = CheckNull(value); }
    }
    
    public int? ShippedFrom
    {
      get { return Row["ShippedFrom"] != DBNull.Value ? (int?)Row["ShippedFrom"] : null; }
      set { Row["ShippedFrom"] = CheckNull(value); }
    }
    
    public int? ShippedTo
    {
      get { return Row["ShippedTo"] != DBNull.Value ? (int?)Row["ShippedTo"] : null; }
      set { Row["ShippedTo"] = CheckNull(value); }
    }
    
    public string TrackingNumber
    {
      get { return Row["TrackingNumber"] != DBNull.Value ? (string)Row["TrackingNumber"] : null; }
      set { Row["TrackingNumber"] = CheckNull(value); }
    }
    
    public string ShippingMethod
    {
      get { return Row["ShippingMethod"] != DBNull.Value ? (string)Row["ShippingMethod"] : null; }
      set { Row["ShippingMethod"] = CheckNull(value); }
    }
    
    public string ReferenceNum
    {
      get { return Row["ReferenceNum"] != DBNull.Value ? (string)Row["ReferenceNum"] : null; }
      set { Row["ReferenceNum"] = CheckNull(value); }
    }
    
    public string Comments
    {
      get { return Row["Comments"] != DBNull.Value ? (string)Row["Comments"] : null; }
      set { Row["Comments"] = CheckNull(value); }
    }
    
    public int? Actor
    {
      get { return Row["Actor"] != DBNull.Value ? (int?)Row["Actor"] : null; }
      set { Row["Actor"] = CheckNull(value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    
    public int AssetID
    {
      get { return (int)Row["AssetID"]; }
      set { Row["AssetID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? ActionTime
    {
      get { return Row["ActionTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["ActionTime"]) : null; }
      set { Row["ActionTime"] = CheckNull(value); }
    }
    
    public DateTime? DateCreated
    {
      get { return Row["DateCreated"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateCreated"]) : null; }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    

    

    #endregion
    
    
  }

  public partial class AssetHistory : BaseCollection, IEnumerable<AssetHistoryItem>
  {
    public AssetHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AssetHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "HistoryID"; }
    }



    public AssetHistoryItem this[int index]
    {
      get { return new AssetHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AssetHistoryItem assetHistoryItem);
    partial void AfterRowInsert(AssetHistoryItem assetHistoryItem);
    partial void BeforeRowEdit(AssetHistoryItem assetHistoryItem);
    partial void AfterRowEdit(AssetHistoryItem assetHistoryItem);
    partial void BeforeRowDelete(int historyID);
    partial void AfterRowDelete(int historyID);    

    partial void BeforeDBDelete(int historyID);
    partial void AfterDBDelete(int historyID);    

    #endregion

    #region Public Methods

    public AssetHistoryItemProxy[] GetAssetHistoryItemProxies()
    {
      List<AssetHistoryItemProxy> list = new List<AssetHistoryItemProxy>();

      foreach (AssetHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int historyID)
    {
      BeforeDBDelete(historyID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteAssetHistoryItem";
        deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);
        deleteCommand.Parameters["HistoryID"].Value = historyID;

        BeforeRowDelete(historyID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(historyID);
      }
      AfterDBDelete(historyID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateAssetHistoryItem";

		
		tempParameter = updateCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
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
		
		tempParameter = updateCommand.Parameters.Add("ActionTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionDescription", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippedFrom", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippedTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TrackingNumber", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippingMethod", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReferenceNum", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comments", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Actor", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertAssetHistoryItem";

		
		tempParameter = insertCommand.Parameters.Add("Actor", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("Comments", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReferenceNum", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippingMethod", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TrackingNumber", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippedTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippedFrom", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionDescription", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "uspGeneratedDeleteAssetHistoryItem";
		deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);

		try
		{
		  foreach (AssetHistoryItem assetHistoryItem in this)
		  {
			if (assetHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(assetHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = assetHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["HistoryID"].AutoIncrement = false;
			  Table.Columns["HistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				assetHistoryItem.Row["HistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(assetHistoryItem);
			}
			else if (assetHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(assetHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = assetHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(assetHistoryItem);
			}
			else if (assetHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)assetHistoryItem.Row["HistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["HistoryID"].Value = id;
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

      foreach (AssetHistoryItem assetHistoryItem in this)
      {
        if (assetHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)assetHistoryItem.Row["CreatorID"] == 0) assetHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (assetHistoryItem.Row.Table.Columns.Contains("ModifierID")) assetHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AssetHistoryItem FindByHistoryID(int historyID)
    {
      foreach (AssetHistoryItem assetHistoryItem in this)
      {
        if (assetHistoryItem.HistoryID == historyID)
        {
          return assetHistoryItem;
        }
      }
      return null;
    }

    public virtual AssetHistoryItem AddNewAssetHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("AssetHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AssetHistoryItem(row, this);
    }
    
    public virtual void LoadByHistoryID(int historyID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectAssetHistoryItem";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("HistoryID", historyID);
        Fill(command);
      }
    }
    
    public static AssetHistoryItem GetAssetHistoryItem(LoginUser loginUser, int historyID)
    {
      AssetHistory assetHistory = new AssetHistory(loginUser);
      assetHistory.LoadByHistoryID(historyID);
      if (assetHistory.IsEmpty)
        return null;
      else
        return assetHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AssetHistoryItem> Members

    public IEnumerator<AssetHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AssetHistoryItem(row, this);
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
