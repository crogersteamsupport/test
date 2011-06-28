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
    private int _count = 0;
    private Dictionary<string, string> _fieldAliases;

    private int _maxCount = 1000;
    public int MaxCount
    {
      get { return _maxCount; }
      set { _maxCount = value; }
    }

    private Dictionary<int, int> _updatedTickets = null;
    public Dictionary<int, int> UpdatedTickets
    {
      get { return _updatedTickets; }
      set { _updatedTickets = value; }
    }

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
      _updatedTickets = new Dictionary<int, int>();
      _fieldAliases = new Dictionary<string, string>();
    }


    override public bool GetNextDoc()
    {
      try
      {
        if (_reader == null) { Rewind(); }

        if (_lastTicketID != null)
        {
          _updatedTickets.Add((int)_lastTicketID, DocId);
        }

        if (_count >= _maxCount) return false;
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

        //DocModifiedDate = (DateTime)_reader["DateModified"];
        DocModifiedDate = DateTime.UtcNow;
        DocCreatedDate = (DateTime)_reader["DateCreated"];
        DocIsFile = false;
        DocText = string.Format("<html>{0}</html>", _reader["IndexText"].ToString());
        //Logs.Log(_loginUser, "Indexer", "Index Text", DocText, null, ReferenceType.Tickets, (int)_reader["TicketID"]);
        DocFields = "";
        DocName = _reader["TicketID"].ToString();
        DocDisplayName = string.Format("{0}: {1}", _reader["TicketNumber"].ToString(), _reader["Name"].ToString());
        _lastTicketID = (int)_reader["TicketID"];
        for (int i = 0; i < _reader.FieldCount; i++)
        {
          string name = _reader.GetName(i);
          /*if (_fieldAliases.ContainsKey(name))
          {
            name = _fieldAliases[name];
          }*/
          if (name.ToLower() == "indextext") continue;
          object value = _reader.GetValue(i);
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          DocFields += name + "\t" + s.Replace("\t", " ") + "\t";
        }

        _count++;
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
      _fieldAliases.Clear();
      ReportTableFields fields = new ReportTableFields(LoginUser);
      fields.LoadByReportTableID(10);
      foreach (ReportTableField field in fields)
      {
        _fieldAliases.Add(field.FieldName, field.Alias);
      }

      string sql =
      @"SELECT 
      ISNULL(
      (
      ISNULL(
      (
        SELECT CAST(cv.CustomValue + ' ' AS VARCHAR(MAX)) FROM CustomValues cv LEFT JOIN CustomFields cf ON cf.CustomFieldID = cv.CustomFieldID 
        WHERE cf.RefType=17 AND cv.RefID=tv.TicketID    
        FOR XML PATH('')
      ), '') + ' ' +
      (
        SELECT CAST(ISNULL(a.Description, '') + ' ' + ISNULL(a.Name, '') + ' ' + ISNULL(a.CreatorName, '') + ' ' AS VARCHAR(MAX))
        FROM ActionsView a
        WHERE a.TicketID = tv.TicketID
        FOR XML PATH('')
      )

      ), '') AS IndexText,
      tv.*
      FROM TicketsView tv WITH(NOLOCK)
      WHERE tv.NeedsIndexing = 1
      ORDER BY DateModified 
      ";
      SqlConnection connection = new SqlConnection(LoginUser.ConnectionString);
      connection.Open();
      SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
      SqlCommand command = new SqlCommand(sql, connection, transaction);
      command.CommandType = CommandType.Text;
      _reader = command.ExecuteReader(CommandBehavior.CloseConnection);
      _lastTicketID = null;
      _count = 0;
      return true;
    }
  }
}
