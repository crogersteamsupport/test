using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Not : BaseItem
  {
    private Notes _notes;
    
    public Not(DataRow row, Notes notes): base(row, notes)
    {
      _notes = notes;
    }
	
    #region Properties
    
    public Notes Collection
    {
      get { return _notes; }
    }
        
    
    
    
    public int NoteID
    {
      get { return (int)Row["NoteID"]; }
    }
    

    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }
    

    
    public bool IsAlert
    {
      get { return (bool)Row["IsAlert"]; }
      set { Row["IsAlert"] = CheckValue("IsAlert", value); }
    }
    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Title
    {
      get { return (string)Row["Title"]; }
      set { Row["Title"] = CheckValue("Title", value); }
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
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
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
    

    #endregion
    
    
  }

  public partial class Notes : BaseCollection, IEnumerable<Not>
  {
    public Notes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Notes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "NoteID"; }
    }



    public Not this[int index]
    {
      get { return new Not(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Not not);
    partial void AfterRowInsert(Not not);
    partial void BeforeRowEdit(Not not);
    partial void AfterRowEdit(Not not);
    partial void BeforeRowDelete(int noteID);
    partial void AfterRowDelete(int noteID);    

    partial void BeforeDBDelete(int noteID);
    partial void AfterDBDelete(int noteID);    

    #endregion

    #region Public Methods

    public NotProxy[] GetNotProxies()
    {
      List<NotProxy> list = new List<NotProxy>();

      foreach (Not item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int noteID)
    {
      BeforeDBDelete(noteID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Notes] WHERE ([NoteID] = @NoteID);";
        deleteCommand.Parameters.Add("NoteID", SqlDbType.Int);
        deleteCommand.Parameters["NoteID"].Value = noteID;

        BeforeRowDelete(noteID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(noteID);
      }
      AfterDBDelete(noteID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("NotesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Notes] SET     [RefType] = @RefType,    [RefID] = @RefID,    [Title] = @Title,    [Description] = @Description,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [NeedsIndexing] = @NeedsIndexing,    [IsAlert] = @IsAlert,    [ImportFileID] = @ImportFileID  WHERE ([NoteID] = @NoteID);";

		
		tempParameter = updateCommand.Parameters.Add("NoteID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Title", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
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
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsAlert", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Notes] (    [RefType],    [RefID],    [Title],    [Description],    [CreatorID],    [ModifierID],    [DateCreated],    [DateModified],    [NeedsIndexing],    [IsAlert],    [ImportFileID]) VALUES ( @RefType, @RefID, @Title, @Description, @CreatorID, @ModifierID, @DateCreated, @DateModified, @NeedsIndexing, @IsAlert, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsAlert", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Title", SqlDbType.VarChar, 1000);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Notes] WHERE ([NoteID] = @NoteID);";
		deleteCommand.Parameters.Add("NoteID", SqlDbType.Int);

		try
		{
		  foreach (Not not in this)
		  {
			if (not.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(not);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = not.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["NoteID"].AutoIncrement = false;
			  Table.Columns["NoteID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				not.Row["NoteID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(not);
			}
			else if (not.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(not);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = not.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(not);
			}
			else if (not.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)not.Row["NoteID", DataRowVersion.Original];
			  deleteCommand.Parameters["NoteID"].Value = id;
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

      foreach (Not not in this)
      {
        if (not.Row.Table.Columns.Contains("CreatorID") && (int)not.Row["CreatorID"] == 0) not.Row["CreatorID"] = LoginUser.UserID;
        if (not.Row.Table.Columns.Contains("ModifierID")) not.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Not FindByNoteID(int noteID)
    {
      foreach (Not not in this)
      {
        if (not.NoteID == noteID)
        {
          return not;
        }
      }
      return null;
    }

    public virtual Not AddNewNot()
    {
      if (Table.Columns.Count < 1) LoadColumns("Notes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Not(row, this);
    }
    
    public virtual void LoadByNoteID(int noteID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [NoteID], [RefType], [RefID], [Title], [Description], [CreatorID], [ModifierID], [DateCreated], [DateModified], [NeedsIndexing], [IsAlert], [ImportFileID] FROM [dbo].[Notes] WHERE ([NoteID] = @NoteID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("NoteID", noteID);
        Fill(command);
      }
    }
    
    public static Not GetNot(LoginUser loginUser, int noteID)
    {
      Notes notes = new Notes(loginUser);
      notes.LoadByNoteID(noteID);
      if (notes.IsEmpty)
        return null;
      else
        return notes[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Not> Members

    public IEnumerator<Not> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Not(row, this);
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
