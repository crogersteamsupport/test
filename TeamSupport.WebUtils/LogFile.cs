using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace TeamSupport.WebUtils
{
  public class LogFile
  {
    private static void Add(string path, string text)
    { 
      using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
      {
        try
        {
          writer.WriteLine(text);
          writer.Close();
        }
        catch
        {
        }
      }
    }

    public static void Add(string text)
    {
      //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
      string path = HttpContext.Current.Server.MapPath("");
      path = Path.Combine(path, "Logs");
      path = Path.Combine(path, "LogFile.txt");
      Add(path, text);
    }

    public static void LogException(Exception ex)
    {

      StringBuilder builder = new StringBuilder();
      try
      {
        builder.AppendLine("Exception occurred at " + string.Format(Constants.FORMAT_DATETIME_FullDateTime));
        builder.AppendLine(ex.Message + System.Environment.NewLine + ex.StackTrace);

        
        Exception exInner = ex.InnerException;
        while (exInner != null)
        {
          builder.AppendLine(exInner.Message + System.Environment.NewLine + exInner.StackTrace);
          if (ex.InnerException != null)
          {
            exInner = exInner.InnerException;
          }
          else
          {
            exInner = null;
          }
        }

        builder.AppendLine("-----------------------------------------------" + System.Environment.NewLine);
        Add(builder.ToString());
      }
      catch 
      { 
      }
    }
  }
}


