using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkTableItem : BaseItem
  {
    private CRMLinkTable _cRMLinkTable;
    
    public CRMLinkTableItem(DataRow row, CRMLinkTable cRMLinkTable): base(row, cRMLinkTable)
    {
      _cRMLinkTable = cRMLinkTable;
    }
	
    #region Properties
    
    public CRMLinkTable Collection
    {
      get { return _cRMLinkTable; }
    }
        
    
    
    
    public int CRMLinkID
    {
      get { return (int)Row["CRMLinkID"]; }
    }
    

    
    public string Username
    {
      get { return Row["Username"] != DBNull.Value ? (string)Row["Username"] : null; }
      set { Row["Username"] = CheckNull(value); }
    }
    
    public string Password
    {
      get { return Row["Password"] != DBNull.Value ? (string)Row["Password"] : null; }
      set { Row["Password"] = CheckNull(value); }
    }
    
    public string SecurityToken
    {
      get { return Row["SecurityToken"] != DBNull.Value ? (string)Row["SecurityToken"] : null; }
      set { Row["SecurityToken"] = CheckNull(value); }
    }
    
    public string TypeFieldMatch
    {
      get { return Row["TypeFieldMatch"] != DBNull.Value ? (string)Row["TypeFieldMatch"] : null; }
      set { Row["TypeFieldMatch"] = CheckNull(value); }
    }
    

    
    public bool SendWelcomeEmail
    {
      get { return (bool)Row["SendWelcomeEmail"]; }
      set { Row["SendWelcomeEmail"] = CheckNull(value); }
    }
    
    public bool AllowPortalAccess
    {
      get { return (bool)Row["AllowPortalAccess"]; }
      set { Row["AllowPortalAccess"] = CheckNull(value); }
    }
    
    public int LastTicketID
    {
      get { return (int)Row["LastTicketID"]; }
      set { Row["LastTicketID"] = CheckNull(value); }
    }
    
    public bool SendBackTicketData
    {
      get { return (bool)Row["SendBackTicketData"]; }
      set { Row["SendBackTicketData"] = CheckNull(value); }
    }
    
    public string CRMType
    {
      get { return (string)Row["CRMType"]; }
      set { Row["CRMType"] = CheckNull(value); }
    }
    
    public bool Active
    {
      get { return (bool)Row["Active"]; }
      set { Row["Active"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastLink
    {
      get { return Row["LastLink"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastLink"]) : null; }
      set { Row["LastLink"] = CheckNull(value); }
    }
    

    
    public DateTime LastProcessed
    {
      get { return DateToLocal((DateTime)Row["LastProcessed"]); }
      set { Row["LastProcessed"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class CRMLinkTable : BaseCollection, IEnumerable<CRMLinkTableItem>
  {
    public CRMLinkTable(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkTable"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMLinkID"; }
    }



    public CRMLinkTableItem this[int index]
    {
      get { return new CRMLinkTableItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkTableItem cRMLinkTableItem);
    partial void AfterRowInsert(CRMLinkTableItem cRMLinkTableItem);
    partial void BeforeRowEdit(CRMLinkTableItem cRMLinkTableItem);
    partial void AfterRowEdit(CRMLinkTableItem cRMLinkTableItem);
    partial void BeforeRowDelete(int cRMLinkID);
    partial void AfterRowDelete(int cRMLinkID);    

    partial void BeforeDBDelete(int cRMLinkID);
    partial void AfterDBDelete(int cRMLinkID);    

    #endregion

    #region Public Methods

    public CRMLinkTableItemProxy[] GetCRMLinkTableItemProxies()
    {
      List<CRMLinkTableItemProxy> list = new List<CRMLinkTableItemProxy>();

      foreach (CRMLinkTableItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMLinkID)
    {
      BeforeDBDelete(cRMLinkID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
        deleteCommand.Parameters.Add("CRMLinkID", SqlDbType.Int);
        deleteCommand.Parameters["CRMLinkID"].Value = cRMLinkID;

        BeforeRowDelete(cRMLinkID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(cRMLinkID);
      }
      AfterDBDelete(cRMLinkID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkTableSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkTable] SET     [OrganizationID] = @OrganizationID,    [Active] = @Active,    [CRMType] = @CRMType,    [Username] = @Username,    [Password] = @Password,    [SecurityToken] = @SecurityToken,    [TypeFieldMatch] = @TypeFieldMatch,    [LastLink] = @LastLink,    [SendBackTicketData] = @SendBackTicketData,    [LastProcessed] = @LastProcessed,    [LastTicketID] = @LastTicketID,    [AllowPortalAccess] = @AllowPortalAccess,    [SendWelcomeEmail] = @SendWelcomeEmail  WHERE ([CRMLinkID] = @CRMLinkID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Username", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Password", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SecurityToken", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TypeFieldMatch", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastLink", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SendBackTicketData", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastProcessed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastTicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AllowPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SendWelcomeEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkTable] (    [OrganizationID],    [Active],    [CRMType],    [Username],    [Password],    [SecurityToken],    [TypeFieldMatch],    [LastLink],    [SendBackTicketData],    [LastProcessed],    [LastTicketID],    [AllowPortalAccess],    [SendWelcomeEmail]) VALUES ( @OrganizationID, @Active, @CRMType, @Username, @Password, @SecurityToken, @TypeFieldMatch, @LastLink, @SendBackTicketData, @LastProcessed, @LastTicketID, @AllowPortalAccess, @SendWelcomeEmail); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SendWelcomeEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AllowPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastTicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastProcessed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SendBackTicketData", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastLink", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TypeFieldMatch", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SecurityToken", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Password", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Username", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
		deleteCommand.Parameters.Add("CRMLinkID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkTableItem cRMLinkTableItem in this)
		  {
			if (cRMLinkTableItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkTableItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkTableItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMLinkID"].AutoIncrement = false;
			  Table.Columns["CRMLinkID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkTableItem.Row["CRMLinkID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkTableItem);
			}
			else if (cRMLinkTableItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkTableItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkTableItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkTableItem);
			}
			else if (cRMLinkTableItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkTableItem.Row["CRMLinkID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMLinkID"].Value = id;
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

      foreach (CRMLinkTableItem cRMLinkTableItem in this)
      {
        if (cRMLinkTableItem.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkTableItem.Row["CreatorID"] == 0) cRMLinkTableItem.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkTableItem.Row.Table.Columns.Contains("ModifierID")) cRMLinkTableItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkTableItem FindByCRMLinkID(int cRMLinkID)
    {
      foreach (CRMLinkTableItem cRMLinkTableItem in this)
      {
        if (cRMLinkTableItem.CRMLinkID == cRMLinkID)
        {
          return cRMLinkTableItem;
        }
      }
      return null;
    }

    public virtual CRMLinkTableItem AddNewCRMLinkTableItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkTable");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkTableItem(row, this);
    }
    
    public virtual void LoadByCRMLinkID(int cRMLinkID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMLinkID], [OrganizationID], [Active], [CRMType], [Username], [Password], [SecurityToken], [TypeFieldMatch], [LastLink], [SendBackTicketData], [LastProcessed], [LastTicketID], [AllowPortalAccess], [SendWelcomeEmail] FROM [dbo].[CRMLinkTable] WHERE ([CRMLinkID] = @CRMLinkID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMLinkID", cRMLinkID);
        Fill(command);
      }
    }
    
    public static CRMLinkTableItem GetCRMLinkTableItem(LoginUser loginUser, int cRMLinkID)
    {
      CRMLinkTable cRMLinkTable = new CRMLinkTable(loginUser);
      cRMLinkTable.LoadByCRMLinkID(cRMLinkID);
      if (cRMLinkTable.IsEmpty)
        return null;
      else
        return cRMLinkTable[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkTableItem> Members

    public IEnumerator<CRMLinkTableItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkTableItem(row, this);
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
