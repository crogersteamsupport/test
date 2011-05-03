using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailPost : BaseItem
  {
    private EmailPosts _emailPosts;
    
    public EmailPost(DataRow row, EmailPosts emailPosts): base(row, emailPosts)
    {
      _emailPosts = emailPosts;
    }
	
    #region Properties
    
    public EmailPosts Collection
    {
      get { return _emailPosts; }
    }
        
    
    
    
    public int EmailPostID
    {
      get { return (int)Row["EmailPostID"]; }
    }
    

    
    public string Param1
    {
      get { return Row["Param1"] != DBNull.Value ? (string)Row["Param1"] : null; }
      set { Row["Param1"] = CheckNull(value); }
    }
    
    public string Param2
    {
      get { return Row["Param2"] != DBNull.Value ? (string)Row["Param2"] : null; }
      set { Row["Param2"] = CheckNull(value); }
    }
    
    public string Param3
    {
      get { return Row["Param3"] != DBNull.Value ? (string)Row["Param3"] : null; }
      set { Row["Param3"] = CheckNull(value); }
    }
    
    public string Param4
    {
      get { return Row["Param4"] != DBNull.Value ? (string)Row["Param4"] : null; }
      set { Row["Param4"] = CheckNull(value); }
    }
    
    public string Param5
    {
      get { return Row["Param5"] != DBNull.Value ? (string)Row["Param5"] : null; }
      set { Row["Param5"] = CheckNull(value); }
    }
    
    public string Param6
    {
      get { return Row["Param6"] != DBNull.Value ? (string)Row["Param6"] : null; }
      set { Row["Param6"] = CheckNull(value); }
    }
    
    public string Param7
    {
      get { return Row["Param7"] != DBNull.Value ? (string)Row["Param7"] : null; }
      set { Row["Param7"] = CheckNull(value); }
    }
    
    public string Param8
    {
      get { return Row["Param8"] != DBNull.Value ? (string)Row["Param8"] : null; }
      set { Row["Param8"] = CheckNull(value); }
    }
    
    public string Param9
    {
      get { return Row["Param9"] != DBNull.Value ? (string)Row["Param9"] : null; }
      set { Row["Param9"] = CheckNull(value); }
    }
    
    public string Param10
    {
      get { return Row["Param10"] != DBNull.Value ? (string)Row["Param10"] : null; }
      set { Row["Param10"] = CheckNull(value); }
    }
    
    public string Text1
    {
      get { return Row["Text1"] != DBNull.Value ? (string)Row["Text1"] : null; }
      set { Row["Text1"] = CheckNull(value); }
    }
    
    public string Text2
    {
      get { return Row["Text2"] != DBNull.Value ? (string)Row["Text2"] : null; }
      set { Row["Text2"] = CheckNull(value); }
    }
    
    public string Text3
    {
      get { return Row["Text3"] != DBNull.Value ? (string)Row["Text3"] : null; }
      set { Row["Text3"] = CheckNull(value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckNull(value); }
    }
    
    public int HoldTime
    {
      get { return (int)Row["HoldTime"]; }
      set { Row["HoldTime"] = CheckNull(value); }
    }
    
    public EmailPostType EmailPostType
    {
      get { return (EmailPostType)Row["EmailPostType"]; }
      set { Row["EmailPostType"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class EmailPosts : BaseCollection, IEnumerable<EmailPost>
  {
    public EmailPosts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EmailPosts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailPostID"; }
    }



    public EmailPost this[int index]
    {
      get { return new EmailPost(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailPost emailPost);
    partial void AfterRowInsert(EmailPost emailPost);
    partial void BeforeRowEdit(EmailPost emailPost);
    partial void AfterRowEdit(EmailPost emailPost);
    partial void BeforeRowDelete(int emailPostID);
    partial void AfterRowDelete(int emailPostID);    

    partial void BeforeDBDelete(int emailPostID);
    partial void AfterDBDelete(int emailPostID);    

    #endregion

    #region Public Methods

    public EmailPostProxy[] GetEmailPostProxies()
    {
      List<EmailPostProxy> list = new List<EmailPostProxy>();

      foreach (EmailPost item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailPostID)
    {
      BeforeDBDelete(emailPostID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteEmailPost";
        deleteCommand.Parameters.Add("EmailPostID", SqlDbType.Int);
        deleteCommand.Parameters["EmailPostID"].Value = emailPostID;

        BeforeRowDelete(emailPostID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(emailPostID);
      }
      AfterDBDelete(emailPostID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailPostsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateEmailPost";

		
		tempParameter = updateCommand.Parameters.Add("EmailPostID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailPostType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param1", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param2", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param3", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param4", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param5", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param6", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param7", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param8", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param9", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param10", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text1", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text2", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text3", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertEmailPost";

		
		tempParameter = insertCommand.Parameters.Add("Text3", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Text2", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Text1", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param10", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param9", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param8", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param7", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param6", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param5", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param4", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param3", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param2", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param1", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailPostType", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "uspGeneratedDeleteEmailPost";
		deleteCommand.Parameters.Add("EmailPostID", SqlDbType.Int);

		try
		{
		  foreach (EmailPost emailPost in this)
		  {
			if (emailPost.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailPost);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailPost.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailPostID"].AutoIncrement = false;
			  Table.Columns["EmailPostID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailPost.Row["EmailPostID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailPost);
			}
			else if (emailPost.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailPost);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailPost.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailPost);
			}
			else if (emailPost.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailPost.Row["EmailPostID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailPostID"].Value = id;
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

      foreach (EmailPost emailPost in this)
      {
        if (emailPost.Row.Table.Columns.Contains("CreatorID") && (int)emailPost.Row["CreatorID"] == 0) emailPost.Row["CreatorID"] = LoginUser.UserID;
        if (emailPost.Row.Table.Columns.Contains("ModifierID")) emailPost.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailPost FindByEmailPostID(int emailPostID)
    {
      foreach (EmailPost emailPost in this)
      {
        if (emailPost.EmailPostID == emailPostID)
        {
          return emailPost;
        }
      }
      return null;
    }

    public virtual EmailPost AddNewEmailPost()
    {
      if (Table.Columns.Count < 1) LoadColumns("EmailPosts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailPost(row, this);
    }
    
    public virtual void LoadByEmailPostID(int emailPostID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectEmailPost";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("EmailPostID", emailPostID);
        Fill(command);
      }
    }
    
    public static EmailPost GetEmailPost(LoginUser loginUser, int emailPostID)
    {
      EmailPosts emailPosts = new EmailPosts(loginUser);
      emailPosts.LoadByEmailPostID(emailPostID);
      if (emailPosts.IsEmpty)
        return null;
      else
        return emailPosts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailPost> Members

    public IEnumerator<EmailPost> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailPost(row, this);
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
