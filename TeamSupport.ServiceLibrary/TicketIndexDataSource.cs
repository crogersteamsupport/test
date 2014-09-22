using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class TicketIndexDataSource : IndexDataSource
  {
    protected TicketIndexDataSource() { }

    public TicketIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
      : base(loginUser, maxCount, organizationID, isRebuilding, logName)
    {
    }


    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        TicketsViewItem ticket = TicketsView.GetTicketsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing TicketID: " + ticket.TicketID.ToString());

        _lastItemID = ticket.TicketID;
        UpdatedItems.Add((int)_lastItemID);

        StringBuilder actionsBuilder = new StringBuilder();
        Actions actions = new Actions(_loginUser);
        actions.LoadByTicketID(ticket.TicketID);
        foreach (TeamSupport.Data.Action action in actions)
        {
          string actionText = action.Description;

          try
          {
            actionText = HtmlToText.ConvertHtml(actionText);
          }
          catch (Exception)
          {
          }

          actionsBuilder.AppendLine(actionText);
        }

        DocText = actionsBuilder.ToString();

        _docFields.Clear();
        foreach (DataColumn column in ticket.Collection.Table.Columns)
	      {
          object value = ticket.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          AddDocField(column.ColumnName, s);
        }

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID);

        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          AddDocField(value.Row["Name"].ToString(), s);
        }
        DocFields = _docFields.ToString();

        DocIsFile       = false;
        DocName         = ticket.TicketID.ToString();
        DocDisplayName  = string.Format("{0}: {1}", ticket.TicketNumber.ToString(), ticket.Name);
        DocCreatedDate  = (DateTime)ticket.Row["DateCreated"];
        DocModifiedDate = (DateTime)ticket.Row["DateModified"];

      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "TicketIndexDataSource");
      }
      return true;
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
        _logs.WriteEvent("Rewound tickets, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        TicketsView tickets = new TicketsView(_loginUser);
        tickets.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (TicketsViewItem ticket in tickets)
        {
          _itemIDList.Add(ticket.TicketID);
        }
        _lastItemID = null;
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