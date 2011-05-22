using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace TeamSupport.ServiceLibrary
{
  class TicketIndexDataSource : dtSearch.Engine.DataSource
  {
    private int? _lastTicketID = null;
    private SqlDataReader _reader = null;

    private LoginUser _loginUser = null;
    public LoginUser LoginUser
    {
      get { return _loginUser; }
      set { _loginUser = value; }
    }

    public TicketIndexDataSource()
    {
      DocName = "";
      DocDisplayName = "";
      DocModifiedDate = System.DateTime.UtcNow;
      DocCreatedDate = System.DateTime.UtcNow;
      DocText = "";
      DocFields = "";
      DocIsFile = false;
    }


    override public bool GetNextDoc()
    {
      try
      {
        if (_reader == null) { Rewind(); }

        if (_lastTicketID != null)
        {
          Ticket ticket = Tickets.GetTicket(LoginUser, (int)_lastTicketID);
          if (ticket != null)
          {
            ticket.DocID = DocId;
            ticket.NeedsIndexing = false;
            ticket.Collection.Save();
          }
        }

        if (!_reader.HasRows || !_reader.Read())
        {
          try
          {
            if (!_reader.IsClosed) _reader.Close();
            return false;
          }
          catch (Exception)
          {
          }
        }

        DocModifiedDate = System.DateTime.UtcNow;
        DocCreatedDate = System.DateTime.UtcNow;
        DocIsFile = false;
        DocText = _reader.GetString(4);
        DocFields = "";
        DocName = _reader.GetInt32(0).ToString();
        DocDisplayName = string.Format("{0}: {1}", _reader.GetInt32(1).ToString(), _reader.GetString(2));
        _lastTicketID = _reader.GetInt32(0);
        for (int i = 0; i < 4; i++)
        {
          object value = _reader.GetValue(i);
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          DocFields += _reader.GetName(i) + "\t" + s.Replace("\t", " ") + "\t";
        }

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "TicketIndexDataSource");
        throw;
      }
    }

    override public bool Rewind()
    {
      string sql =
      @"SELECT 
      t.TicketID,
      t.TicketNumber,
      t.Name,
      t.OrganizationID,
      ISNULL(
      (
      t.Name + ' ' +

      ISNULL(
      (
        SELECT CAST(cv.CustomValue + ' ' AS VARCHAR(MAX)) FROM CustomValues cv LEFT JOIN CustomFields cf ON cf.CustomFieldID = cv.CustomFieldID 
        WHERE cf.RefType=17 AND cv.RefID=t.TicketID    
        FOR XML PATH('')
      ), '') + ' ' +
      (
        SELECT CAST(a.Description + ' ' + a.Name + ' ' AS VARCHAR(MAX))
        FROM Actions a
        WHERE a.TicketID = t.TicketID
        FOR XML PATH('')
      )

      ), '') AS IndexText
      FROM Tickets t WITH(NOLOCK)
      WHERE t.NeedsIndexing = 1
      ORDER BY DateModified 
      ";
      SqlConnection connection = new SqlConnection(LoginUser.ConnectionString);
      connection.Open();
      SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
      SqlCommand command = new SqlCommand(sql, connection, transaction);
      command.CommandType = CommandType.Text;
      _reader = command.ExecuteReader(CommandBehavior.CloseConnection);
      _lastTicketID = null;
      return true;
    }
  }
}
