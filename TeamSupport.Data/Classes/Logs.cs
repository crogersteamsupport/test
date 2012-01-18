using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  public class Logs
  {

    string _category = null;

    public string Category
    {
      get
      {
        lock (this)
        {
          return _category;
        } 
      }
    }

    public Logs() { }

    public Logs(string category)
    {
      this._category = category;
    }

    public void WriteException(Exception ex)
    {
      WriteException(ex, null);
    }

    public void WriteException(Exception ex, DataRow row)
    {
      WriteEvent("EXCEPTION:");
      WriteEvent("Message: ");
      WriteEvent(ex.Message);
      WriteEvent("Stack Trace: ");
      WriteEvent(ex.StackTrace);

      if (row != null) { WriteData(row); }
    }

    public void WriteData(DataRow row)
    {
      WriteEvent("Data Row:");
      WriteEvent(DataUtils.DataRowToString(row));
    }

    public void WriteEvent(string message)
    {
      lock (this)
      {
        string name = Path.ChangeExtension(System.AppDomain.CurrentDomain.FriendlyName, "");

        if (!string.IsNullOrEmpty(Category)) { name = name + " [" + Category + "]"; }

        string logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");

        if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

        string path = Path.Combine(logPath,
            string.Format("{0} {1:yyyy-MM-dd}.txt", name, DateTime.Now));

        message = string.Format("[{0:hh:mm:ss tt}] {1}", DateTime.Now, message);

        //TextWriter writer = TextWriter.Synchronized(!File.Exists(path) ? File.CreateText(path) : File.AppendText(path));

        using (StreamWriter writer = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path))
        {
          writer.WriteLine(message);
        }

      }
    }

  }
}
