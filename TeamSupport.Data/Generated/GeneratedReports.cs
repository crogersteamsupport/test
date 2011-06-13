using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Report : BaseItem
  {
    private Reports _reports;
    
    public Report(DataRow row, Reports reports): base(row, reports)
    {
      _reports = reports;
    }
	
    #region Properties
    
    public Reports Collection
    {
      get { return _reports; }
    }
        
    
    
    
    public int ReportID
    {
      get { return (int)Row["ReportID"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckNull(value); }
    }
    
    public string Query
    {
      get { return Row["Query"] != DBNull.Value ? (string)Row["Query"] : null; }
      set { Row["Query"] = CheckNull(value); }
    }
    
    public string CustomFieldKeyName
    {
      get { return Row["CustomFieldKeyName"] != DBNull.Value ? (string)Row["CustomFieldKeyName"] : null; }
      set { Row["CustomFieldKeyName"] = CheckNull(value); }
    }
    
    public int? CustomAuxID
    {
      get { return Row["CustomAuxID"] != DBNull.Value ? (int?)Row["CustomAuxID"] : null; }
      set { Row["CustomAuxID"] = CheckNull(value); }
    }
    
    public int? ReportSubcategoryID
    {
      get { return Row["ReportSubcategoryID"] != DBNull.Value ? (int?)Row["ReportSubcategoryID"] : null; }
      set { Row["ReportSubcategoryID"] = CheckNull(value); }
    }
    
    public string QueryObject
    {
      get { return Row["QueryObject"] != DBNull.Value ? (string)Row["QueryObject"] : null; }
      set { Row["QueryObject"] = CheckNull(value); }
    }
    
    public string ExternalURL
    {
      get { return Row["ExternalURL"] != DBNull.Value ? (string)Row["ExternalURL"] : null; }
      set { Row["ExternalURL"] = CheckNull(value); }
    }
    
    public string LastSqlExecuted
    {
      get { return Row["LastSqlExecuted"] != DBNull.Value ? (string)Row["LastSqlExecuted"] : null; }
      set { Row["LastSqlExecuted"] = CheckNull(value); }
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
    
    public ReferenceType CustomRefType
    {
      get { return (ReferenceType)Row["CustomRefType"]; }
      set { Row["CustomRefType"] = CheckNull(value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    
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

  public partial class Reports : BaseCollection, IEnumerable<Report>
  {
    public Reports(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Reports"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportID"; }
    }



    public Report this[int index]
    {
      get { return new Report(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Report report);
    partial void AfterRowInsert(Report report);
    partial void BeforeRowEdit(Report report);
    partial void AfterRowEdit(Report report);
    partial void BeforeRowDelete(int reportID);
    partial void AfterRowDelete(int reportID);    

    partial void BeforeDBDelete(int reportID);
    partial void AfterDBDelete(int reportID);    

    #endregion

    #region Public Methods

    public ReportProxy[] GetReportProxies()
    {
      List<ReportProxy> list = new List<ReportProxy>();

      foreach (Report item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportID)
    {
      BeforeDBDelete(reportID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Reports] WHERE ([ReportID] = @ReportID);";
        deleteCommand.Parameters.Add("ReportID", SqlDbType.Int);
        deleteCommand.Parameters["ReportID"].Value = reportID;

        BeforeRowDelete(reportID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(reportID);
      }
      AfterDBDelete(reportID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Reports] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Description] = @Description,    [Query] = @Query,    [CustomFieldKeyName] = @CustomFieldKeyName,    [CustomRefType] = @CustomRefType,    [CustomAuxID] = @CustomAuxID,    [ReportSubcategoryID] = @ReportSubcategoryID,    [QueryObject] = @QueryObject,    [ExternalURL] = @ExternalURL,    [LastSqlExecuted] = @LastSqlExecuted,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([ReportID] = @ReportID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Query", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldKeyName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomRefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomAuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("QueryObject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExternalURL", SqlDbType.VarChar, 3000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastSqlExecuted", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Reports] (    [OrganizationID],    [Name],    [Description],    [Query],    [CustomFieldKeyName],    [CustomRefType],    [CustomAuxID],    [ReportSubcategoryID],    [QueryObject],    [ExternalURL],    [LastSqlExecuted],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @Name, @Description, @Query, @CustomFieldKeyName, @CustomRefType, @CustomAuxID, @ReportSubcategoryID, @QueryObject, @ExternalURL, @LastSqlExecuted, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("LastSqlExecuted", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExternalURL", SqlDbType.VarChar, 3000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("QueryObject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomAuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomRefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomFieldKeyName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Query", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 100);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Reports] WHERE ([ReportID] = @ReportID);";
		deleteCommand.Parameters.Add("ReportID", SqlDbType.Int);

		try
		{
		  foreach (Report report in this)
		  {
			if (report.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(report);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = report.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportID"].AutoIncrement = false;
			  Table.Columns["ReportID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				report.Row["ReportID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(report);
			}
			else if (report.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(report);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = report.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(report);
			}
			else if (report.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)report.Row["ReportID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportID"].Value = id;
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

      foreach (Report report in this)
      {
        if (report.Row.Table.Columns.Contains("CreatorID") && (int)report.Row["CreatorID"] == 0) report.Row["CreatorID"] = LoginUser.UserID;
        if (report.Row.Table.Columns.Contains("ModifierID")) report.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Report FindByReportID(int reportID)
    {
      foreach (Report report in this)
      {
        if (report.ReportID == reportID)
        {
          return report;
        }
      }
      return null;
    }

    public virtual Report AddNewReport()
    {
      if (Table.Columns.Count < 1) LoadColumns("Reports");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Report(row, this);
    }
    
    public virtual void LoadByReportID(int reportID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportID], [OrganizationID], [Name], [Description], [Query], [CustomFieldKeyName], [CustomRefType], [CustomAuxID], [ReportSubcategoryID], [QueryObject], [ExternalURL], [LastSqlExecuted], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[Reports] WHERE ([ReportID] = @ReportID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportID", reportID);
        Fill(command);
      }
    }
    
    public static Report GetReport(LoginUser loginUser, int reportID)
    {
      Reports reports = new Reports(loginUser);
      reports.LoadByReportID(reportID);
      if (reports.IsEmpty)
        return null;
      else
        return reports[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Report> Members

    public IEnumerator<Report> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Report(row, this);
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
