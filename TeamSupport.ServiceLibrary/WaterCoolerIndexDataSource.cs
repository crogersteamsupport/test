using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class WaterCoolerIndexDataSource : IndexDataSource
  {
    protected WaterCoolerIndexDataSource() { }

    public WaterCoolerIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
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

        WaterCoolerViewItem waterCooler = WaterCoolerView.GetWaterCoolerViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing WaterCoolerMessageID: " + waterCooler.MessageID.ToString());

        _lastItemID = waterCooler.MessageID;
        UpdatedItems.Add((int)_lastItemID);

        DocText = string.Format("<html><body>{0}</body></html>", HtmlToText.ConvertHtml(waterCooler.Message));

        _docFields.Clear();
        foreach (DataColumn column in waterCooler.Collection.Table.Columns)
        {
          object value = waterCooler.Row[column];
          string s = value == null || value == DBNull.Value ? "" : value.ToString();
          AddDocField(column.ColumnName, s);
        }

        DocFields = _docFields.ToString();
        DocIsFile = false;
        DocName = waterCooler.MessageID.ToString();
        DocCreatedDate = waterCooler.TimeStampUtc;
        DocModifiedDate = DateTime.UtcNow;

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "WaterCoolerIndexDataSource");
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound water cooler, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        WaterCoolerView waterCoolerMessages = new WaterCoolerView(_loginUser);
        waterCoolerMessages.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (WaterCoolerViewItem waterCoolerMessage in waterCoolerMessages)
        {
          _itemIDList.Add(waterCoolerMessage.MessageID);
        }
        _lastItemID = null;
        _rowIndex = -1;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "WaterCoolerIndexDataSource Rewind OrgID: " + _organizationID.ToString());
        throw;
      }
      return true;
    }
  }
}
