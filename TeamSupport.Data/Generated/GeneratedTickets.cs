using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Ticket : BaseItem
  {
    private Tickets _tickets;
    
    public Ticket(DataRow row, Tickets tickets): base(row, tickets)
    {
      _tickets = tickets;
    }
	
    #region Properties
    
    public Tickets Collection
    {
      get { return _tickets; }
    }
        
    
    
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
    }
    

    
    public int? ReportedVersionID
    {
      get { return Row["ReportedVersionID"] != DBNull.Value ? (int?)Row["ReportedVersionID"] : null; }
      set { Row["ReportedVersionID"] = CheckNull(value); }
    }
    
    public int? SolvedVersionID
    {
      get { return Row["SolvedVersionID"] != DBNull.Value ? (int?)Row["SolvedVersionID"] : null; }
      set { Row["SolvedVersionID"] = CheckNull(value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckNull(value); }
    }
    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckNull(value); }
    }
    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckNull(value); }
    }
    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckNull(value); }
    }
    
    public int? CloserID
    {
      get { return Row["CloserID"] != DBNull.Value ? (int?)Row["CloserID"] : null; }
      set { Row["CloserID"] = CheckNull(value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckNull(value); }
    }
    
    public string TicketSource
    {
      get { return Row["TicketSource"] != DBNull.Value ? (string)Row["TicketSource"] : null; }
      set { Row["TicketSource"] = CheckNull(value); }
    }
    
    public string PortalEmail
    {
      get { return Row["PortalEmail"] != DBNull.Value ? (string)Row["PortalEmail"] : null; }
      set { Row["PortalEmail"] = CheckNull(value); }
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
    
    public bool IsKnowledgeBase
    {
      get { return (bool)Row["IsKnowledgeBase"]; }
      set { Row["IsKnowledgeBase"] = CheckNull(value); }
    }
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckNull(value); }
    }
    
    public int TicketNumber
    {
      get { return (int)Row["TicketNumber"]; }
      set { Row["TicketNumber"] = CheckNull(value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
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
    
    public int TicketStatusID
    {
      get { return (int)Row["TicketStatusID"]; }
      set { Row["TicketStatusID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateClosed
    {
      get { return Row["DateClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateClosed"]) : null; }
      set { Row["DateClosed"] = CheckNull(value); }
    }
    
    public DateTime? LastViolationTime
    {
      get { return Row["LastViolationTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastViolationTime"]) : null; }
      set { Row["LastViolationTime"] = CheckNull(value); }
    }
    
    public DateTime? LastWarningTime
    {
      get { return Row["LastWarningTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastWarningTime"]) : null; }
      set { Row["LastWarningTime"] = CheckNull(value); }
    }
    
    public DateTime? SlaViolationTimeClosed
    {
      get { return Row["SlaViolationTimeClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationTimeClosed"]) : null; }
      set { Row["SlaViolationTimeClosed"] = CheckNull(value); }
    }
    
    public DateTime? SlaViolationLastAction
    {
      get { return Row["SlaViolationLastAction"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationLastAction"]) : null; }
      set { Row["SlaViolationLastAction"] = CheckNull(value); }
    }
    
    public DateTime? SlaViolationInitialResponse
    {
      get { return Row["SlaViolationInitialResponse"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationInitialResponse"]) : null; }
      set { Row["SlaViolationInitialResponse"] = CheckNull(value); }
    }
    
    public DateTime? SlaWarningTimeClosed
    {
      get { return Row["SlaWarningTimeClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningTimeClosed"]) : null; }
      set { Row["SlaWarningTimeClosed"] = CheckNull(value); }
    }
    
    public DateTime? SlaWarningLastAction
    {
      get { return Row["SlaWarningLastAction"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningLastAction"]) : null; }
      set { Row["SlaWarningLastAction"] = CheckNull(value); }
    }
    
    public DateTime? SlaWarningInitialResponse
    {
      get { return Row["SlaWarningInitialResponse"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningInitialResponse"]) : null; }
      set { Row["SlaWarningInitialResponse"] = CheckNull(value); }
    }
    

    
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

  public partial class Tickets : BaseCollection, IEnumerable<Ticket>
  {
    public Tickets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Tickets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public Ticket this[int index]
    {
      get { return new Ticket(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Ticket ticket);
    partial void AfterRowInsert(Ticket ticket);
    partial void BeforeRowEdit(Ticket ticket);
    partial void AfterRowEdit(Ticket ticket);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketProxy[] GetTicketProxies()
    {
      List<TicketProxy> list = new List<TicketProxy>();

      foreach (Ticket item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
      BeforeDBDelete(ticketID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteTicket";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeRowDelete(ticketID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(ticketID);
      }
      AfterDBDelete(ticketID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateTicket";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SolvedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketStatusID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("CloserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastViolationTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastWarningTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalEmail", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningInitialResponse", SqlDbType.DateTime, 8);
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
		insertCommand.CommandText = "uspGeneratedInsertTicket";

		
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
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalEmail", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastWarningTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastViolationTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CloserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("TicketStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SolvedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportedVersionID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "uspGeneratedDeleteTicket";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (Ticket ticket in this)
		  {
			if (ticket.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticket);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticket.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticket.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticket);
			}
			else if (ticket.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticket);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticket.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticket);
			}
			else if (ticket.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticket.Row["TicketID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketID"].Value = id;
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

      foreach (Ticket ticket in this)
      {
        if (ticket.Row.Table.Columns.Contains("CreatorID") && (int)ticket.Row["CreatorID"] == 0) ticket.Row["CreatorID"] = LoginUser.UserID;
        if (ticket.Row.Table.Columns.Contains("ModifierID")) ticket.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Ticket FindByTicketID(int ticketID)
    {
      foreach (Ticket ticket in this)
      {
        if (ticket.TicketID == ticketID)
        {
          return ticket;
        }
      }
      return null;
    }

    public virtual Ticket AddNewTicket()
    {
      if (Table.Columns.Count < 1) LoadColumns("Tickets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Ticket(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectTicket";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static Ticket GetTicket(LoginUser loginUser, int ticketID)
    {
      Tickets tickets = new Tickets(loginUser);
      tickets.LoadByTicketID(ticketID);
      if (tickets.IsEmpty)
        return null;
      else
        return tickets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Ticket> Members

    public IEnumerator<Ticket> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Ticket(row, this);
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
