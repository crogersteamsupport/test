using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaTrigger : BaseItem
  {
    private SlaTriggers _slaTriggers;
    
    public SlaTrigger(DataRow row, SlaTriggers slaTriggers): base(row, slaTriggers)
    {
      _slaTriggers = slaTriggers;
    }
	
    #region Properties
    
    public SlaTriggers Collection
    {
      get { return _slaTriggers; }
    }
        
    
    
    
    public int SlaTriggerID
    {
      get { return (int)Row["SlaTriggerID"]; }
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
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SlaTriggers : BaseCollection, IEnumerable<SlaTrigger>
  {
    public SlaTriggers(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaTriggers"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SlaTriggerID"; }
    }



    public SlaTrigger this[int index]
    {
      get { return new SlaTrigger(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaTrigger slaTrigger);
    partial void AfterRowInsert(SlaTrigger slaTrigger);
    partial void BeforeRowEdit(SlaTrigger slaTrigger);
    partial void AfterRowEdit(SlaTrigger slaTrigger);
    partial void BeforeRowDelete(int slaTriggerID);
    partial void AfterRowDelete(int slaTriggerID);    

    partial void BeforeDBDelete(int slaTriggerID);
    partial void AfterDBDelete(int slaTriggerID);    

    #endregion

    #region Public Methods

    public SlaTriggerProxy[] GetSlaTriggerProxies()
    {
      List<SlaTriggerProxy> list = new List<SlaTriggerProxy>();

      foreach (SlaTrigger item in this)
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
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteSlaTrigger";
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
		//SqlTransaction transaction = connection.BeginTransaction("SlaTriggersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateSlaTrigger";

		
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertSlaTrigger";

		
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.StoredProcedure;
		deleteCommand.CommandText = "uspGeneratedDeleteSlaTrigger";
		deleteCommand.Parameters.Add("SlaTriggerID", SqlDbType.Int);

		try
		{
		  foreach (SlaTrigger slaTrigger in this)
		  {
			if (slaTrigger.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaTrigger);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaTrigger.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SlaTriggerID"].AutoIncrement = false;
			  Table.Columns["SlaTriggerID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaTrigger.Row["SlaTriggerID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaTrigger);
			}
			else if (slaTrigger.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaTrigger);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaTrigger.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaTrigger);
			}
			else if (slaTrigger.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaTrigger.Row["SlaTriggerID", DataRowVersion.Original];
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

      foreach (SlaTrigger slaTrigger in this)
      {
        if (slaTrigger.Row.Table.Columns.Contains("CreatorID") && (int)slaTrigger.Row["CreatorID"] == 0) slaTrigger.Row["CreatorID"] = LoginUser.UserID;
        if (slaTrigger.Row.Table.Columns.Contains("ModifierID")) slaTrigger.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaTrigger FindBySlaTriggerID(int slaTriggerID)
    {
      foreach (SlaTrigger slaTrigger in this)
      {
        if (slaTrigger.SlaTriggerID == slaTriggerID)
        {
          return slaTrigger;
        }
      }
      return null;
    }

    public virtual SlaTrigger AddNewSlaTrigger()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaTriggers");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaTrigger(row, this);
    }
    
    public virtual void LoadBySlaTriggerID(int slaTriggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectSlaTrigger";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("SlaTriggerID", slaTriggerID);
        Fill(command);
      }
    }
    
    public static SlaTrigger GetSlaTrigger(LoginUser loginUser, int slaTriggerID)
    {
      SlaTriggers slaTriggers = new SlaTriggers(loginUser);
      slaTriggers.LoadBySlaTriggerID(slaTriggerID);
      if (slaTriggers.IsEmpty)
        return null;
      else
        return slaTriggers[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaTrigger> Members

    public IEnumerator<SlaTrigger> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaTrigger(row, this);
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
