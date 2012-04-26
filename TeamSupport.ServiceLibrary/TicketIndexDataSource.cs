using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using HtmlAgilityPack;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class TicketIndexDataSource : dtSearch.Engine.DataSource
  {
    private int? _lastTicketID = null;
    protected List<int> _ticketIDList = null;
    private int _rowIndex = 0;
    //private Dictionary<string, string> _fieldAliases;
    private LoginUser _loginUser = null;
    private int _organizationID;
    private int _maxCount;
    protected Logs _logs;
    private List<int> _updatedTickets = null;
    public List<int> UpdatedTickets
    {
      get { lock (this) { return _updatedTickets; } }
    }


    protected TicketIndexDataSource() { }

    public TicketIndexDataSource(LoginUser loginUser, int maxCount, int organizationID)
    {
      DocName = "";
      DocDisplayName = "";
      DocModifiedDate = System.DateTime.UtcNow;
      DocCreatedDate = System.DateTime.UtcNow;
      DocText = "";
      DocFields = "";
      DocIsFile = false;
      _maxCount = maxCount;
      _loginUser = new LoginUser(loginUser.ConnectionString, loginUser.UserID, loginUser.OrganizationID, null);
      _updatedTickets = new List<int>();
      _organizationID = organizationID;
      _logs = new Logs("Ticket Indexer DataSource");
      //_fieldAliases = new Dictionary<string, string>();
    }


    override public bool GetNextDoc()
    {
      try
      {
        if (_ticketIDList == null) { Rewind(); }
        if (_lastTicketID != null) { UpdatedTickets.Add((int)_lastTicketID); }
        _rowIndex++;
        if (_ticketIDList.Count <= _rowIndex) { return false; }
        TicketsViewItem ticket = TicketsView.GetTicketsViewItem(_loginUser, _ticketIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing TicketID: " + ticket.TicketID.ToString());
        
        //DocModifiedDate = (DateTime)_reader["DateModified"];
        DocModifiedDate = DateTime.UtcNow;
        DocCreatedDate = (DateTime) ticket.Row["DateCreated"];
        DocIsFile = false;

        StringBuilder actionsBuilder = new StringBuilder();
        Actions actions = new Actions(_loginUser);
        actions.LoadByTicketID(ticket.TicketID);
        foreach (TeamSupport.Data.Action action in actions)
        {
          string actionText = action.Description;
          try
          {
            //actionText = HtmlUtility.TidyHtml(actionText);
          }
          catch (Exception)
          {
          }
          
          try
          {
            actionText = HtmlToText.ConvertHtml(actionText);
          }
          catch (Exception)
          {
          }
          actionsBuilder.AppendLine(actionText);
        }
        DocText = string.Format("<html>{1} {0}</html>", "CUSTOM FIELDS", actionsBuilder.ToString());


        DocFields = "";
        DocName = ticket.TicketID.ToString();
        DocDisplayName = string.Format("{0}: {1}", ticket.TicketNumber.ToString(), ticket.Name);
        _lastTicketID = ticket.TicketID;

        foreach (DataColumn column in ticket.Collection.Table.Columns)
	      {
          object value = ticket.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          DocFields += column.ColumnName + "\t" + s.Replace("\t", " ") + "\t";

	      }

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        }
        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "TicketIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    

    override public bool Rewind()
    {
      try
      {
        //Logs.WriteEvent("Rewinding Tickets Source");
        /*_fieldAliases.Clear();
        ReportTableFields fields = new ReportTableFields(LoginUser);
        fields.LoadByReportTableID(10);
        foreach (ReportTableField field in fields)
        {
          _fieldAliases.Add(field.FieldName, field.Alias);
        }
         */
        _logs.WriteEvent("Rewound, OrgID: " + _organizationID.ToString());
        _ticketIDList = new List<int>();
        TicketsView tickets = new TicketsView(_loginUser);
        tickets.LoadForIndexing(_organizationID, _maxCount);
        foreach (TicketsViewItem ticket in tickets)
        {
          _ticketIDList.Add(ticket.TicketID);
        }
        _lastTicketID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "TicketIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }
}

