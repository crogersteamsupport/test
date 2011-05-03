using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;

namespace TeamSupport.WebUtils
{
  [Serializable]
  public class TabInfo
  {
    private string _clientID;
    public string ClientID
    {
      get { return _clientID; }
      set { _clientID = value; }
    }
    
    private int _auxID = -1;
    public int AuxID
    {
      get { return _auxID; }
    }

    private string _controlName;
    public string ControlName
    {
      get { return _controlName; }
    }

    public string ControlID
    {
      get
      {
        if (_auxID < 0)
          return _controlName.Split('.')[0].Replace("/", "").Replace("~", "") + "_" + ClientID;
        else
          return _controlName.Split('.')[0].Replace("/", "").Replace("~", "") + "_" + _auxID.ToString() + ClientID;
      }
    }

    private int _tabType;
    public int TabType
    {
      get { return _tabType; }
    }

    public TabInfo(string clientID, int tabType, string controlName) 
    {
      _controlName = controlName;
      _tabType = tabType;
    }
    
    public TabInfo(string clientID, int tabType, string controlName, int TicketTypeID)
    {
      _controlName = controlName;
      _tabType = tabType;
      _auxID = TicketTypeID;
    }

    public static string Serialize(TabInfo tabInfo)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize(writer, tabInfo);
      return writer.ToString();
    }

    public static TabInfo Deserialize(string value)
    {
      if (value == string.Empty) return null;

      try
      {
        LosFormatter formatter = new LosFormatter();
        return formatter.Deserialize(value) as TabInfo;
      }
      catch
      {
        return null;
      }

    }
  }
}
