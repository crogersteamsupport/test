using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using TeamSupport.Data;
using System.Data;

namespace TeamSupport.ServiceLibrary
{
  public class Utils
  {
    public static string GetSettingString(string keyName) { return GetSettingString(keyName, ""); }
    public static string GetSettingString(string keyName, string defaultValue) { return (string)GetSetting(keyName, defaultValue, RegistryValueKind.String); }
    public static int GetSettingInt(string keyName, int defaultValue) { return (int)GetSetting(keyName, defaultValue, RegistryValueKind.DWord); }
    public static int GetSettingInt(string keyName) { return (int)GetSetting(keyName, -1, RegistryValueKind.DWord); }

    public static void SetSettingValue(string keyName, object value, RegistryValueKind kind)
    {
      RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\TeamSupport\Service");
      key.SetValue(keyName, value, kind);
    }

    public static object GetSetting(string keyName, object defaultValue, RegistryValueKind kind)
    {
      RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\TeamSupport\Service");
      object o = key.GetValue(keyName);
      if (o != null)
      {
        return o;
      }
      else
      {
        key.SetValue(keyName, defaultValue, kind);
        return defaultValue;
      }
    }

    public static void LogException(LoginUser loginUser, Exception exception, string module, string info)
    {
      try
      {
        ExceptionLogs logs = new ExceptionLogs(loginUser);
        ExceptionLog log = logs.AddNewExceptionLog();

        log.Browser = "";
        log.ExceptionName = exception.ToString();
        log.Message = exception.Message;
        log.PageInfo = info;
        log.StackTrace = exception.StackTrace;
        log.URL = "TeamSupport Service - " + module;
        logs.Save();
      }
      catch (Exception)
      {

      }
    }

    public static void LogException(LoginUser loginUser, Exception exception, string module, DataRow row)
    {
      StringBuilder builder = new StringBuilder();
      foreach (DataColumn column in row.Table.Columns)
      {
        builder.Append("[" + column.ColumnName + "] = '" + row[column].ToString() + "';  ");
      }
      LogException(loginUser, exception, module, "Data Row:  " + builder.ToString());
    }

    public static LoginUser GetLoginUser(string appName)
    {
      string constring = GetSettingString("ConnectionString", "Application Name=TeamSupport Service: APPNAME" + appName + ";Data Source=localhost;Initial Catalog=TeamSupport;Persist Security Info=True;User ID=sa;Password=muroc").Replace("APPNAME", appName);
      return new LoginUser(constring, -1, -1, null);
    }


  }
}
