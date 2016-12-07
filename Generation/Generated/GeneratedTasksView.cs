using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TasksViewItem : BaseItem
  {
    private TasksView _tasksView;
    
    public TasksViewItem(DataRow row, TasksView tasksView): base(row, tasksView)
    {
      _tasksView = tasksView;
    }
	
    #region Properties
    
    public TasksView Collection
    {
      get { return _tasksView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    public string Creator
    {
      get { return Row["Creator"] != DBNull.Value ? (string)Row["Creator"] : null; }
    }
    
    
    

    
    public string TaskName
    {
      get { return Row["TaskName"] != DBNull.Value ? (string)Row["TaskName"] : null; }
      set { Row["TaskName"] = CheckValue("TaskName", value); }
    }
    
    public int? TaskParentID
    {
      get { return Row["TaskParentID"] != DBNull.Value ? (int?)Row["TaskParentID"] : null; }
      set { Row["TaskParentID"] = CheckValue("TaskParentID", value); }
    }
    
    public string TaskParentName
    {
      get { return Row["TaskParentName"] != DBNull.Value ? (string)Row["TaskParentName"] : null; }
      set { Row["TaskParentName"] = CheckValue("TaskParentName", value); }
    }
    

    
    public bool TaskIsComplete
    {
      get { return (bool)Row["TaskIsComplete"]; }
      set { Row["TaskIsComplete"] = CheckValue("TaskIsComplete", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public bool HasEmailSent
    {
      get { return (bool)Row["HasEmailSent"]; }
      set { Row["HasEmailSent"] = CheckValue("HasEmailSent", value); }
    }
    
    public bool IsDismissed
    {
      get { return (bool)Row["IsDismissed"]; }
      set { Row["IsDismissed"] = CheckValue("IsDismissed", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int ReminderID
    {
      get { return (int)Row["ReminderID"]; }
      set { Row["ReminderID"] = CheckValue("ReminderID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? TaskDueDate
    {
      get { return Row["TaskDueDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["TaskDueDate"]) : null; }
      set { Row["TaskDueDate"] = CheckValue("TaskDueDate", value); }
    }

    public DateTime? TaskDueDateUtc
    {
      get { return Row["TaskDueDate"] != DBNull.Value ? (DateTime?)Row["TaskDueDate"] : null; }
    }
    
    public DateTime? TaskDateCompleted
    {
      get { return Row["TaskDateCompleted"] != DBNull.Value ? DateToLocal((DateTime?)Row["TaskDateCompleted"]) : null; }
      set { Row["TaskDateCompleted"] = CheckValue("TaskDateCompleted", value); }
    }

    public DateTime? TaskDateCompletedUtc
    {
      get { return Row["TaskDateCompleted"] != DBNull.Value ? (DateTime?)Row["TaskDateCompleted"] : null; }
    }
    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    
    public DateTime DueDate
    {
      get { return DateToLocal((DateTime)Row["DueDate"]); }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime DueDateUtc
    {
      get { return (DateTime)Row["DueDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class TasksView : BaseCollection, IEnumerable<TasksViewItem>
  {
    public TasksView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TasksView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }



    public TasksViewItem this[int index]
    {
      get { return new TasksViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TasksViewItem tasksViewItem);
    partial void AfterRowInsert(TasksViewItem tasksViewItem);
    partial void BeforeRowEdit(TasksViewItem tasksViewItem);
    partial void AfterRowEdit(TasksViewItem tasksViewItem);
    partial void BeforeRowDelete(int );
    partial void AfterRowDelete(int );    

    partial void BeforeDBDelete(int );
    partial void AfterDBDelete(int );    

    #endregion

    #region Public Methods

    public TasksViewItemProxy[] GetTasksViewItemProxies()
    {
      List<TasksViewItemProxy> list = new List<TasksViewItemProxy>();

      foreach (TasksViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int )
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TasksView] WH);";
        deleteCommand.Parameters.Add("", SqlDbType.Int);
        deleteCommand.Parameters[""].Value = ;

        BeforeDBDelete();
        BeforeRowDelete();
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete();
        AfterDBDelete();
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TasksViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TasksView] SET     [ReminderID] = @ReminderID,    [OrganizationID] = @OrganizationID,    [RefType] = @RefType,    [RefID] = @RefID,    [Description] = @Description,    [DueDate] = @DueDate,    [UserID] = @UserID,    [IsDismissed] = @IsDismissed,    [HasEmailSent] = @HasEmailSent,    [TaskName] = @TaskName,    [TaskDueDate] = @TaskDueDate,    [TaskIsComplete] = @TaskIsComplete,    [TaskDateCompleted] = @TaskDateCompleted,    [TaskParentID] = @TaskParentID,    [TaskParentName] = @TaskParentName,    [UserName] = @UserName,    [Creator] = @Creator  WH);";

		
		tempParameter = updateCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HasEmailSent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskDueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskIsComplete", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskDateCompleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskParentName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Creator", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TasksView] (    [ReminderID],    [OrganizationID],    [RefType],    [RefID],    [Description],    [DueDate],    [UserID],    [IsDismissed],    [HasEmailSent],    [CreatorID],    [DateCreated],    [TaskName],    [TaskDueDate],    [TaskIsComplete],    [TaskDateCompleted],    [TaskParentID],    [TaskParentName],    [UserName],    [Creator]) VALUES ( @ReminderID, @OrganizationID, @RefType, @RefID, @Description, @DueDate, @UserID, @IsDismissed, @HasEmailSent, @CreatorID, @DateCreated, @TaskName, @TaskDueDate, @TaskIsComplete, @TaskDateCompleted, @TaskParentID, @TaskParentName, @UserName, @Creator); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Creator", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskParentName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskDateCompleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskIsComplete", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskDueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HasEmailSent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TasksView] WH);";
		deleteCommand.Parameters.Add("", SqlDbType.Int);

		try
		{
		  foreach (TasksViewItem tasksViewItem in this)
		  {
			if (tasksViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(tasksViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = tasksViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns[""].AutoIncrement = false;
			  Table.Columns[""].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				tasksViewItem.Row[""] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(tasksViewItem);
			}
			else if (tasksViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(tasksViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = tasksViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(tasksViewItem);
			}
			else if (tasksViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)tasksViewItem.Row["", DataRowVersion.Original];
			  deleteCommand.Parameters[""].Value = id;
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

      foreach (TasksViewItem tasksViewItem in this)
      {
        if (tasksViewItem.Row.Table.Columns.Contains("CreatorID") && (int)tasksViewItem.Row["CreatorID"] == 0) tasksViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (tasksViewItem.Row.Table.Columns.Contains("ModifierID")) tasksViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TasksViewItem FindBy(int )
    {
      foreach (TasksViewItem tasksViewItem in this)
      {
        if (tasksViewItem. == )
        {
          return tasksViewItem;
        }
      }
      return null;
    }

    public virtual TasksViewItem AddNewTasksViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TasksView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TasksViewItem(row, this);
    }
    
    public virtual void LoadBy(int )
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReminderID], [OrganizationID], [RefType], [RefID], [Description], [DueDate], [UserID], [IsDismissed], [HasEmailSent], [CreatorID], [DateCreated], [TaskName], [TaskDueDate], [TaskIsComplete], [TaskDateCompleted], [TaskParentID], [TaskParentName], [UserName], [Creator] FROM [dbo].[TasksView] WH);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("", );
        Fill(command);
      }
    }
    
    public static TasksViewItem GetTasksViewItem(LoginUser loginUser, int )
    {
      TasksView tasksView = new TasksView(loginUser);
      tasksView.LoadBy();
      if (tasksView.IsEmpty)
        return null;
      else
        return tasksView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TasksViewItem> Members

    public IEnumerator<TasksViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TasksViewItem(row, this);
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
