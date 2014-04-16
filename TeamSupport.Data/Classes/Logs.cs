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
    static readonly object _syncObject = new object();
    string _category = null;
    string _logPath;
    string _name;

    public string Category
    {
      get
      {
        lock (_syncObject)
        {
          return _category;
        } 
      }
    }

    public Logs() 
    { 
      InitializeLog();
    }

    public Logs(string category)
    {
      this._category = category;
      InitializeLog();
    }

    private void InitializeLog()
    {
      _name = Path.ChangeExtension(System.AppDomain.CurrentDomain.FriendlyName, "");
      if (!string.IsNullOrEmpty(Category)) { _name = _name + " [" + Category + "]"; }
      _logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");
      if (!Directory.Exists(_logPath)) Directory.CreateDirectory(_logPath);
    }

    public void WriteException(Exception ex, DataRow row = null)
    {
      WriteEvent("EXCEPTION:");
      WriteEvent("Message: ");
      WriteEvent(ex.Message);
      WriteEvent("Stack Trace: ");
      WriteEvent(ex.StackTrace);
      
      if (row != null) { WriteData(row); }

      if (ex.InnerException != null) WriteException(ex.InnerException);
    }

    public void WriteData(DataRow row)
    {
      WriteEvent("Data Row:");
      WriteEvent(DataUtils.DataRowToString(row));
    }

    public void WriteEvent(string message, bool showDateTime)
    {
      lock (_syncObject)
      {

        if (showDateTime) message = string.Format("[{0:hh:mm:ss tt}] {1}", DateTime.Now, message);
        //TextWriter _writer = TextWriter.Synchronized(!File.Exists(_logPath) ? File.CreateText(_logPath) : File.AppendText(_logPath));
        
        string path = Path.Combine(_logPath, string.Format("{0} {1:yyyy-MM-dd}.txt", _name, DateTime.Now));

        bool doesFileExist = File.Exists(path);
        using (StreamWriter writer = !doesFileExist ? File.CreateText(path) : File.AppendText(path))
        {
          writer.WriteLine(message);
        }

        // Clean up old log files after creating a new on.
        if (!doesFileExist)
        {
          string[] files = Directory.GetFiles(_logPath, "*.txt");

          foreach (string file in files)
          {
            if (File.GetLastWriteTime(file) < DateTime.Now.AddDays(-7)) File.Delete(file);
          }
        
        }
      }
    }

    public void WriteEventFormat(string formatString, bool showDateTime, params object[] args)
    {
      WriteEvent(string.Format(formatString, args), showDateTime);
    }

    public void WriteEventFormat(string formatString, params object[] args)
    {
      WriteEventFormat(formatString, true, args);
    }

    public void WriteParam(string paramName, string value)
    {
      WriteEventFormat("{0}: {1}", paramName, value);
    }
    public void WriteEvent(string message)
    {
      WriteEvent(message, true);
    }

    public void WriteLine()
    {
      WriteEvent(System.Environment.NewLine, false);
    }

    public void WriteHeader(string text)
    {
      StringBuilder builder = new StringBuilder("********************************************");
      for (int i = 0; i < text.Length+2; i++)
			{
			  builder.Append("*");
			}

      WriteLine();
      WriteEvent(builder.ToString(), false);
      WriteEvent(string.Format("********************** {0} **********************", text), false);
      WriteEvent(builder.ToString(), false);
      WriteLine();


    }

   
  }
}


/*
 * 
 * class ThreadSafe
{
    static readonly object _lock = new object();

    public static void Test()
    {
        lock (_lock)
        {
            // critical section
        }
    }
}
 * 
 * private static readonly object _syncObject = new object();

public static void Log(string logMessage, TextWriter w)    {
   // only one thread can own this lock, so other threads
   // entering this method will wait here until lock is
   // available.
   lock(_syncObject) {
      w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
          DateTime.Now.ToLongDateString());
      w.WriteLine("  :");
      w.WriteLine("  :{0}", logMessage);
      w.WriteLine("-------------------------------");
      // Update the underlying file.
      w.Flush();
   }
}
 * 
 * 
 */


