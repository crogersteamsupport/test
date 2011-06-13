using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaTriggersViewItem : BaseItem
  {
    private SlaTriggersView _slaTriggersView;
    
    public SlaTriggersViewItem(DataRow row, SlaTriggersView slaTriggersView): base(row, slaTriggersView)
    {
      _slaTriggersView = slaTriggersView;
    }
	
    #region Properties
    
    public SlaTriggersView Collection
    {
      get { return _slaTriggersView; }
    }
        
    
    
    

    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckNull(value); }
    }
    
    public string TicketType
    {
      get { return (string)Row["TicketType"]; }
      set { Row["TicketType"] = CheckNull(value); }
    }
    
    public string Severity
    {
      get { return (string)Row["Severity"]; }
      set { Row["Severity"] = CheckNull(value); }
    }
    
    public string LevelName
    {
      get { return (string)Row["LevelName"]; }
      set { Row["LevelName"] = CheckNull(value); }
    }
    
    public bool UseBusinessHours
    {
      get { return (bool)Row["UseBusinessHours"]; }
      set { Row["UseBusinessHours"] = CheckNull(value); }
    }
    
    public int WarningTime
    {
      get { return (int)Row["WarningTime"]; }
      set { Row["WarningTime"] = CheckNull(value); }
    }
    
    public bool NotifyGroupOnViolation
    {
      get { return (bool)Row["NotifyGroupOnViolation"]; }
      set { Row["NotifyGroupOnViolation"] = CheckNull(value); }
    }
    
    public bool NotifyUserOnViolation
    {
      get { return (bool)Row["NotifyUserOnViolation"]; }
      set { Row["NotifyUserOnViolation"] = CheckNull(value); }
    }
    
    public bool NotifyGroupOnWarning
    {
      get { return (bool)Row["NotifyGroupOnWarning"]; }
      set { Row["NotifyGroupOnWarning"] = CheckNull(value); }
    }
    
    public bool NotifyUserOnWarning
    {
      get { return (bool)Row["NotifyUserOnWarning"]; }
      set { Row["NotifyUserOnWarning"] = CheckNull(value); }
    }
    
    public int TimeToClose
    {
      get { return (int)Row["TimeToClose"]; }
      set { Row["TimeToClose"] = CheckNull(value); }
    }
    
    public int TimeLastAction
    {
      get { return (int)Row["TimeLastAction"]; }
      set { Row["TimeLastAction"] = CheckNull(value); }
    }
    
    public int TimeInitialResponse
    {
      get { return (int)Row["TimeInitialResponse"]; }
      set { Row["TimeInitialResponse"] = CheckNull(value); }
    }
    
    public int TicketSeverityID
    {
      get { return (int)Row["TicketSeverityID"]; }
      set { Row["TicketSeverityID"] = CheckNull(value); }
    }
    
    public int TicketTypeID
    {
      get { return (int)Row["TicketTypeID"]; }
      set { Row["TicketTypeID"] = CheckNull(value); }
    }
    
    public int SlaLevelID
    {
      get { return (int)Row["SlaLevelID"]; }
      set { Row["SlaLevelID"] = CheckNull(value); }
    }
    
    public int SlaTriggerID
    {
      get { return (int)Row["SlaTriggerID"]; }
      set { Row["SlaTriggerID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SlaTriggersView : BaseCollection, IEnumerable<SlaTriggersViewItem>
  {
    public SlaTriggersView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaTriggersView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SlaTriggerID"; }
    }



    public SlaTriggersViewItem this[int index]
    {
      get { return new SlaTriggersViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaTriggersViewItem slaTriggersViewItem);
    partial void AfterRowInsert(SlaTriggersViewItem slaTriggersViewItem);
    partial void BeforeRowEdit(SlaTriggersViewItem slaTriggersViewItem);
    partial void AfterRowEdit(SlaTriggersViewItem slaTriggersViewItem);
    partial void BeforeRowDelete(int slaTriggerID);
    partial void AfterRowDelete(int slaTriggerID);    

    partial void BeforeDBDelete(int slaTriggerID);
    partial void AfterDBDelete(int slaTriggerID);    

    #endregion

    #region Public Methods

    public SlaTriggersViewItemProxy[] GetSlaTriggersViewItemProxies()
    {
      List<SlaTriggersViewItemProxy> list = new List<SlaTriggersViewItemProxy>();

      foreach (SlaTriggersViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int slaTriggerID)
    {
      BeforeDBDelete(slaTriggerID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaTriggersView] WHERE ([SlaTriggerID] = @SlaTriggerID);";
        deleteCommand.Parameters.Add("SlaTriggerID", SqlDbType.Int);
        deleteCommand.Parameters["SlaTriggerID"].Value = slaTriggerID;

        BeforeRowDelete(slaTriggerID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(slaTriggerID);
      }
      AfterDBDelete(slaTriggerID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaTriggersViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaTriggersView] SET     [SlaLevelID] = @SlaLevelID,    [TicketTypeID] = @TicketTypeID,    [TicketSeverityID] = @TicketSeverityID,    [TimeInitialResponse] = @TimeInitialResponse,    [TimeLastAction] = @TimeLastAction,    [TimeToClose] = @TimeToClose,    [NotifyUserOnWarning] = @NotifyUserOnWarning,    [NotifyGroupOnWarning] = @NotifyGroupOnWarning,    [NotifyUserOnViolation] = @NotifyUserOnViolation,    [NotifyGroupOnViolation] = @NotifyGroupOnViolation,    [WarningTime] = @WarningTime,    [UseBusinessHours] = @UseBusinessHours,    [LevelName] = @LevelName,    [Severity] = @Severity,    [TicketType] = @TicketType,    [Position] = @Position,    [OrganizationID] = @OrganizationID  WHERE ([SlaTriggerID] = @SlaTriggerID);";

		
		tempParameter = updateCommand.Parameters.Add("SlaTriggerID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeToClose", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NotifyUserOnWarning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NotifyGroupOnWarning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NotifyUserOnViolation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NotifyGroupOnViolation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarningTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseBusinessHours", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LevelName", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Severity", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaTriggersView] (    [SlaTriggerID],    [SlaLevelID],    [TicketTypeID],    [TicketSeverityID],    [TimeInitialResponse],    [TimeLastAction],    [TimeToClose],    [NotifyUserOnWarning],    [NotifyGroupOnWarning],    [NotifyUserOnViolation],    [NotifyGroupOnViolation],    [WarningTime],    [UseBusinessHours],    [LevelName],    [Severity],    [TicketType],    [Position],    [OrganizationID]) VALUES ( @SlaTriggerID, @SlaLevelID, @TicketTypeID, @TicketSeverityID, @TimeInitialResponse, @TimeLastAction, @TimeToClose, @NotifyUserOnWarning, @NotifyGroupOnWarning, @NotifyUserOnViolation, @NotifyGroupOnViolation, @WarningTime, @UseBusinessHours, @LevelName, @Severity, @TicketType, @Position, @OrganizationID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Severity", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LevelName", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseBusinessHours", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("WarningTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NotifyGroupOnViolation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NotifyUserOnViolation", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NotifyGroupOnWarning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NotifyUserOnWarning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeToClose", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("SlaTriggerID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaTriggersView] WHERE ([SlaTriggerID] = @SlaTriggerID);";
		deleteCommand.Parameters.Add("SlaTriggerID", SqlDbType.Int);

		try
		{
		  foreach (SlaTriggersViewItem slaTriggersViewItem in this)
		  {
			if (slaTriggersViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaTriggersViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaTriggersViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SlaTriggerID"].AutoIncrement = false;
			  Table.Columns["SlaTriggerID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaTriggersViewItem.Row["SlaTriggerID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaTriggersViewItem);
			}
			else if (slaTriggersViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaTriggersViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaTriggersViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaTriggersViewItem);
			}
			else if (slaTriggersViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaTriggersViewItem.Row["SlaTriggerID", DataRowVersion.Original];
			  deleteCommand.Parameters["SlaTriggerID"].Value = id;
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

      foreach (SlaTriggersViewItem slaTriggersViewItem in this)
      {
        if (slaTriggersViewItem.Row.Table.Columns.Contains("CreatorID") && (int)slaTriggersViewItem.Row["CreatorID"] == 0) slaTriggersViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (slaTriggersViewItem.Row.Table.Columns.Contains("ModifierID")) slaTriggersViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaTriggersViewItem FindBySlaTriggerID(int slaTriggerID)
    {
      foreach (SlaTriggersViewItem slaTriggersViewItem in this)
      {
        if (slaTriggersViewItem.SlaTriggerID == slaTriggerID)
        {
          return slaTriggersViewItem;
        }
      }
      return null;
    }

    public virtual SlaTriggersViewItem AddNewSlaTriggersViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaTriggersView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaTriggersViewItem(row, this);
    }
    
    public virtual void LoadBySlaTriggerID(int slaTriggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SlaTriggerID], [SlaLevelID], [TicketTypeID], [TicketSeverityID], [TimeInitialResponse], [TimeLastAction], [TimeToClose], [NotifyUserOnWarning], [NotifyGroupOnWarning], [NotifyUserOnViolation], [NotifyGroupOnViolation], [WarningTime], [UseBusinessHours], [LevelName], [Severity], [TicketType], [Position], [OrganizationID] FROM [dbo].[SlaTriggersView] WHERE ([SlaTriggerID] = @SlaTriggerID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SlaTriggerID", slaTriggerID);
        Fill(command);
      }
    }
    
    public static SlaTriggersViewItem GetSlaTriggersViewItem(LoginUser loginUser, int slaTriggerID)
    {
      SlaTriggersView slaTriggersView = new SlaTriggersView(loginUser);
      slaTriggersView.LoadBySlaTriggerID(slaTriggerID);
      if (slaTriggersView.IsEmpty)
        return null;
      else
        return slaTriggersView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaTriggersViewItem> Members

    public IEnumerator<SlaTriggersViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaTriggersViewItem(row, this);
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
