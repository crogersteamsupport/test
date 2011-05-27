using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  public class Settings
  {
    private LoginUser _loginUser;
    private int _serviceID;

    public Settings(LoginUser loginUser, string serviceName)
    {
      _loginUser = loginUser;
      _serviceID = Services.GetService(loginUser, serviceName).ServiceID;
    }

    public string ReadString(string key) { return ReadString(key, ""); }

    public string ReadString(string key, string defaultValue)
    {
      return ServiceSettings.GetServiceSetting(_loginUser, _serviceID, key, defaultValue).SettingValue;
    }


    public int ReadInt(string key) { return ReadInt(key, 0); }
    public int ReadInt(string key, int defaultValue)
    {
      string value = ReadString(key, defaultValue.ToString());
      return int.Parse(value);
    }

    public bool ReadBool(string key) { return ReadBool(key, false); }
    public bool ReadBool(string key, bool defaultValue)
    {
      string value = ReadString(key, defaultValue.ToString());
      try
      {
        return bool.Parse(value);

      }
      catch (Exception)
      {
        return defaultValue;
      }
    }

    public void WriteInt(string key, int value) { WriteString(key, value.ToString()); }
    public void WriteBool(string key, bool value) { WriteString(key, value.ToString()); }
    public void WriteString(string key, string value) 
    {
      ServiceSetting setting = ServiceSettings.GetServiceSetting(_loginUser, _serviceID, key, "");
      setting.SettingValue = value;
      setting.Collection.Save();
    }

  }
}
