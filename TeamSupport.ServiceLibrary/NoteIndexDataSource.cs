using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class NoteIndexDataSource : IndexDataSource
  {
    protected NoteIndexDataSource() { }

    public NoteIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding) : base(loginUser, maxCount, organizationID, isRebuilding)
    {
      _logs = new Logs("Note Indexer DataSource");
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        NotesViewItem note = NotesView.GetNotesViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing NoteID: " + note.NoteID.ToString());

        _lastItemID = note.NoteID;

        DocText = string.Format("<html><body>{0}</body></html>", HtmlToText.ConvertHtml(note.Description));

        DocFields = string.Empty;
        foreach (DataColumn column in note.Collection.Table.Columns)
        {
          object value = note.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          DocFields += column.ColumnName + "\t" + s.Replace("\t", " ") + "\t";
        }

        DocIsFile = false;
        DocName = note.NoteID.ToString();
        DocCreatedDate = note.DateCreatedUtc;
        DocModifiedDate = DateTime.UtcNow;

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "NoteIndexDataSource");
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound notes, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        NotesView notes = new NotesView(_loginUser);
        notes.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (NotesViewItem note in notes)
        {
          _itemIDList.Add(note.NoteID);
        }
        _lastItemID = null;
        _rowIndex = -1;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "NoteIndexDataSource Rewind");
        throw;
      }
      return true;
    }
  }
}
