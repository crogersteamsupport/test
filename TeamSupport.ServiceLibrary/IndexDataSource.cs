using System;
using System.Collections.Generic;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class IndexDataSource : dtSearch.Engine.DataSource
  {
    protected   int       _organizationID;
    protected   LoginUser _loginUser = null;
    protected   Logs      _logs;

    protected   int       _maxCount;

    protected   List<int> _itemIDList   = null;
    protected   List<int> _updatedItems = null;
    protected   int       _rowIndex     = 0;
    protected   int?      _lastItemID   = null;
    protected bool _isRebuilding = false;

    public    List<int> UpdatedItems
    {
      get { lock (this) { return _updatedItems; } }
    }
    
    protected IndexDataSource() { }

    public IndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
    {
      _organizationID = organizationID;
      _isRebuilding = isRebuilding;
      _loginUser      = new LoginUser(loginUser.ConnectionString, loginUser.UserID, loginUser.OrganizationID, null);
      _logs = new Logs(logName);

      _maxCount = maxCount;

      _updatedItems = new List<int>();

      DocIsFile       = false;
      DocName         = "";
      DocDisplayName  = "";
      DocText         = "";
      DocFields       = "";
      DocCreatedDate  = System.DateTime.UtcNow;
      DocModifiedDate = System.DateTime.UtcNow;
    }

    override public bool GetNextDoc()
    {
      return false;
    }

    override public bool Rewind()
    {
      return false;
    }
  }
}

