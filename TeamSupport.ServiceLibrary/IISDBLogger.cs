using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;

namespace TeamSupport.ServiceLibrary
{
  public class IisDbLogger : ServiceThread
  {
/*    public string LastLogFileName {
      get
      {
        return Utils.GetSettingString("IisLastLogFileName");
      }
      set
      {
        Utils.SetSettingValue("IisLastLogFileName", value, Microsoft.Win32.RegistryValueKind.String);
      }
    }

    private string _logPath;
    private LoginUser _iisLoginUser;
    private LoginUser _loginUser;
    private string _filePrefix;
    private string[] _uriFilters = null;
    */
    public IisDbLogger() { }

    public override string ServiceName
    {
      get { return "iis"; }
    }

    public override void Run()
    {
      //ProcessLogs();
    }
    /*
    public bool ProcessLogs()
    {
      _loginUser = Utils.GetLoginUser("IIS Logger");

      Utils.SetSettingValue("IssLogStatus", "Processing", Microsoft.Win32.RegistryValueKind.String);
      string filters = Utils.GetSettingString("IisLogUriExlude", "");
      if (filters != "")
      {
        _uriFilters = filters.ToLower().Split(';');
      }


      _logPath = Utils.GetSettingString("IisLogPath");
      _iisLoginUser = new LoginUser(Utils.GetSettingString("IisConnectionString", "Data Source=localhost;Initial Catalog=IisLogs;Persist Security Info=True;User ID=sa;Password=muroc"), -1, -1, null);
      _filePrefix = Utils.GetSettingString("IisLogFilePrefix", "u_extend");
      string fileName = "";
      try 
	    {	        
        if (LastLogFileName == "")
        {
          fileName = GetFirstFileName();
          try
          {
            if (fileName == "" || !File.Exists(Path.Combine(_logPath, fileName))) throw new Exception();
          }
          catch (Exception)
          {
            Utils.SetSettingValue("IssLogStatus", "File not found. ('" + fileName + "')", Microsoft.Win32.RegistryValueKind.String);
            return false;
          }
        }
        else
        {
          fileName = GetNextFileName(LastLogFileName);
        }

        string theFile = Path.Combine(_logPath, fileName);
        if (ProcessLog(theFile))
        {
          Utils.SetSettingValue("IssLogStatus", "Success ('" + fileName + "')", Microsoft.Win32.RegistryValueKind.String);
          LastLogFileName = fileName;
          File.Delete(theFile);
          fileName = GetNextFileName(fileName);
          return true;
        }
      }
	    catch (Exception ex)
	    {
        Utils.SetSettingValue("IssLogStatus", "Error ('" + fileName + "')", Microsoft.Win32.RegistryValueKind.String);
        Utils.SetSettingValue("IssLogLastError", ex.Message, Microsoft.Win32.RegistryValueKind.String);
        Utils.LogException(_loginUser, ex, "IisLogs", "FileName: " + fileName);
	    }

      return false;
    }

    public bool ProcessLog(string fileName)
    {
      int counter = 0;
        try
        {
          List<string> fields = new List<string>();
          IisLogs iisLogs = new IisLogs(_iisLoginUser);

          using (StreamReader reader = new StreamReader(fileName))
          {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
              if (!ProcessLine(line, fields, iisLogs, fileName)) continue;
              counter++;

              if (counter % 5000 == 0)
              {
                iisLogs.BulkSave();
                iisLogs.Table.Clear();
                iisLogs = new IisLogs(_iisLoginUser);
              }
              System.Threading.Thread.Sleep(0);

            }
            iisLogs.BulkSave();
            reader.Close();
          }
          return true;
        }
        catch (Exception ex)
        {
          Utils.SetSettingValue("IssLogStatus", "File not ready. (" + fileName + ") -> " + ex.Message, Microsoft.Win32.RegistryValueKind.String);
        }
        return false;

      
    }

    public bool ProcessLine(string line, List<string> fields, IisLogs iisLogs, string fileName)
    {
      if (line.Length < 1) return false;

      if (line[0] == '#')
      {
        if (line.IndexOf("#Fields:") == 0) LoadFields(line, fields);
        return false;
      }
      else
	    {
        return ProcessDataLine(line, fields, iisLogs, fileName);
	    }
    }

    public void LoadFields(string line, List<string> fields)
    {
      fields.Clear();
      line = line.Substring(9, line.Length - 9);
      fields.AddRange(line.Split(' '));
      for (int i = 0; i < fields.Count; i++)
      {
        fields[i] = fields[i].Replace("-", "_").Replace("(", "_").Replace(")", "_");
      }
    }

    public bool ProcessDataLine(string line, List<string> fields, IisLogs iisLogs, string fileName)
    { 
      string[] data = line.Split(' ');
      if (fields.Count != data.Length) throw new Exception("Fields and Data do not match");
      
      string uri = data[fields.IndexOf("cs_uri_stem")].ToLower();
      if (_uriFilters != null && _uriFilters.Contains(uri)) return false;

      IisLog iisLog = iisLogs.AddNewIisLog();
      iisLog.FileName = fileName;
      iisLog.DateLogged = DateTime.Parse(data[fields.IndexOf("date")] + " " + data[fields.IndexOf("time")]);

      foreach (string s in fields)
      {
        if (iisLog.Row.Table.Columns.Contains(s))
        {
          iisLog.Row[s] = data[fields.IndexOf(s)];
        }
      }
      return true;
    }

    public string GetFirstFileName()
    {
      if (!Directory.Exists(_logPath)) return "";
      string[] files = Directory.GetFiles(_logPath, _filePrefix +  "*.log");
      Array.Sort(files);
      if (files.Length < 1) return "";
      return Path.GetFileName(files[0]);
    }

    public string GetNextFileName(string fileName)
    {
      int current = FileNameToNumber(fileName);
      if (!Directory.Exists(_logPath)) return "";
      string[] files = Directory.GetFiles(_logPath, _filePrefix + "*.log");
      Array.Sort(files);
      if (files.Length < 1) return "";

      foreach (string s in files)
      {
        int number = FileNameToNumber(s);
        if (number > current) return Path.GetFileName(s);
      }

      return "";
    }

    public string NumberToFileName(int number)
    {
      return string.Format("{0}{1}.log", _filePrefix, number.ToString());
    }

    public int FileNameToNumber(string fileName)
    {
      try
      {
        return int.Parse(Regex.Replace(Path.GetFileName(fileName), @"[^\d]", ""));
      }
      catch (Exception)
      {
        Utils.SetSettingValue("IssLogLastError", "Invalid Filename", Microsoft.Win32.RegistryValueKind.String);
        return -1;
      }
    }

    public string IncrementFileName(string filename)
    {
      return NumberToFileName(FileNameToNumber(filename) + 1);
    }

    */
  }
}
