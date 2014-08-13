using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class WikiIndexDataSource : IndexDataSource
  {
    protected WikiIndexDataSource() {}

    public WikiIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
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

        WikiArticlesViewItem wiki = WikiArticlesView.GetWikiArticlesViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing Wiki ArticleID: " + wiki.ArticleID.ToString());

        _lastItemID = wiki.ArticleID;
        UpdatedItems.Add((int)_lastItemID);

        DocText = HtmlToText.ConvertHtml(wiki.Body == null ? string.Empty : wiki.Body);

        _docFields.Clear();
        foreach (DataColumn column in wiki.Collection.Table.Columns)
        {
          object value = wiki.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          AddDocField(column.ColumnName, s);
        }
        DocFields = _docFields.ToString();

        DocId           = wiki.ArticleID;
        DocIsFile       = false;
        DocName         = wiki.ArticleID.ToString();
        DocDisplayName  = wiki.ArticleName;
        DocCreatedDate  = wiki.CreatedDate;
        DocModifiedDate = DateTime.UtcNow;

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "WikiIndexDataSource");
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
      _logs.WriteEvent("Rewound wikis, OrgID: " + _organizationID.ToString());
      _itemIDList = new List<int>();
      WikiArticlesView wikis = new WikiArticlesView(_loginUser);
      wikis.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach(WikiArticlesViewItem wiki in wikis)
      {
        _itemIDList.Add(wiki.ArticleID);
      }
      _lastItemID = null;
      _rowIndex = -1;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "WikiIndexDataSource.Rewind");
        throw;
      }

      return true;
    }
  }
}
