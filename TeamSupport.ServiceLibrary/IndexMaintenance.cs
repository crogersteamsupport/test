using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;

namespace TeamSupport.ServiceLibrary
{
  public class IndexMaintenance : ServiceThread
  {
    private Logs logs;

    public override void Run()
    {
      logs = new Logs(LoginUser, ServiceName, "Tickets");
      try
      {
        string ticketPath = Settings.ReadString("Tickets Index Path", "c:\\Indexes\\Tickets");
        CompressIndex(LoginUser, "Tickets", ticketPath);
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "IndexMaintenance"); 
      }

      logs.Log("Finished Compressing Indexes");
    }

    public override string ServiceName
    {
      get { return "IndexMaintenance"; }
    }

    private void CompressIndex(LoginUser loginUser, string indexName, string indexPath)
    {
      if (!Directory.Exists(indexPath))
      {
        ExceptionLogs.AddLog(LoginUser, "Invalid Path", indexPath + " does not exist", "Index Compressor ["+indexName+"]", "", "", "");
        return;
      }

      if (!Settings.ReadBool("Compress Indexes - " + indexName, false)) return;

      if (!Settings.ReadBool("Force Compress - " + indexName, false))
      {
        DateTime last = DateTime.Parse(Settings.ReadString("Last Compressed " + indexName, DateTime.Now.AddYears(-1).ToString()));
        if (last.Subtract(DateTime.Now).TotalHours < 12) return;
        if (DateTime.Now.Hour > 2) return;
      }
      else
      {
        logs.Log("Forced Compress");
        Settings.WriteBool("Force Compress - " + indexName, false);
      }

      logs.Log("Starting Compression [" + indexPath + "]");

      using (IndexJob job = new IndexJob())
      {
        job.IndexPath = indexPath;
        job.ActionCreate = false;
        job.ActionAdd = false;
        job.ActionRemoveListed = false;
        job.ActionCompress = true;
        job.CreateRelativePaths = false;
        job.IndexingFlags = 
            IndexingFlags.dtsIndexKeepExistingDocIds | 
            IndexingFlags.dtsIndexCacheOriginalFile |
            IndexingFlags.dtsIndexCacheText |
            IndexingFlags.dtsIndexCacheTextWithoutFields;

        Settings.WriteString("Compress Status - " + indexName, "Compressing [" + indexPath + "]");

        job.ExecuteInThread();

        bool flag = false;
        DateTime start = DateTime.Now;
        IndexProgressInfo status = new IndexProgressInfo();
        while (job.IsThreadDone(1000, status) == false)
        {
          if (IsStopped || !Settings.ReadBool("Compress Indexes - " + indexName, false)) 
          {
            Settings.WriteString("Compress Status - " + indexName, "Aborted");
            flag = true;
            job.AbortThread(); 
          }
        }
        Settings.WriteBool("Force Compress - " + indexName, false);

        if (!flag)
        {
          Settings.WriteString("Compress Status - " + indexName, "Success");
          Settings.WriteString("Last Compressed - " + indexName, DateTime.Now.ToString());
          Settings.WriteInt("Last Compress Time - " + indexName, (int)DateTime.Now.Subtract(start).TotalSeconds);
        }
        logs.Log("Finished Compressing " + indexName);

      }
    }

  }
}
