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
  public class Indexer : ServiceThread
  {
    public override void Run()
    {
      Logs.WriteEvent("Started Indexing");
      try
      {

        Organizations orgs = new Organizations(LoginUser);
        orgs.LoadByNeedsIndexing();
        int cnt = 0;
        foreach (Organization org in orgs)
        {
          cnt++;
          Logs.WriteEvent(string.Format("Started Indexing for org: {0}, [{1}/{2}]",org.OrganizationID.ToString(), cnt.ToString(), orgs.Count.ToString()));
          ProcessTicketIndex(org);
        }

        
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer"); 
      }
      Logs.WriteEvent("Finished Indexing");


    }

    public override string ServiceName
    {
      get { return "Indexer"; }
    }

    private void ProcessTicketIndex(Organization organization)
    {
      string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + "\\Tickets");
      Logs.WriteEvent("Path: " + path);
      bool isNew = !System.IO.Directory.Exists(path);

      if (isNew) { Directory.CreateDirectory(path); }


      try
      {
        RemoveOldTicketIndexes(LoginUser, path, organization);
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer.RemoveOldTicketIndexes - " + organization.OrganizationID.ToString()); 
      }

      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (IndexJob job = new IndexJob())
      {
        TicketIndexDataSource dataSource = new TicketIndexDataSource();
        dataSource.LoginUser = LoginUser;
        dataSource.Logs = Logs;
        dataSource.MaxCount = Settings.ReadInt("Max Records", 1000);
        dataSource.OrganizationID = organization.OrganizationID;
        job.DataSourceToIndex = dataSource;
        
        job.IndexPath = path;
        job.ActionCreate = isNew;
        job.ActionAdd = true;
        job.CreateRelativePaths = false;
        job.StoredFields = Server.Tokenize("TicketID OrganizationID TicketNumber Name");
        job.IndexingFlags =
            IndexingFlags.dtsAlwaysAdd |
            IndexingFlags.dtsIndexCacheOriginalFile |
            IndexingFlags.dtsIndexCacheText |
            IndexingFlags.dtsIndexCacheTextWithoutFields;

        try
        {
          job.ExecuteInThread();

          // Monitor the job execution thread as it progresses
          IndexProgressInfo status = new IndexProgressInfo();
          while (job.IsThreadDone(500, status) == false)
          {
            if (IsStopped) { job.AbortThread(); }
          }
        }
        catch (Exception ex)
        {
          ExceptionLogs.LogException(LoginUser, ex, "Index Job Processor - " + organization.OrganizationID.ToString());
          Logs.WriteException(ex);
          throw;
        }
        
        UpdateTickets(dataSource);
      }
    }

    private void UpdateTickets(TicketIndexDataSource dataSource)
    {
      if (dataSource.UpdatedTickets.Count < 1) return;

      /*
      StringBuilder builder = new StringBuilder();
      foreach (KeyValuePair<int, int> item in dataSource.UpdatedTickets)
      {
        string sql = string.Format("UPDATE Tickets SET NeedsIndexing = 0, DocID = {1} WHERE TicketID = {0};", item.Key.ToString(), item.Value.ToString());
        builder.AppendLine(sql);
      }
      */

      string updateSql = "UPDATE Tickets SET NeedsIndexing = 0 WHERE TicketID IN (" + DataUtils.IntArrayToCommaString(dataSource.UpdatedTickets.ToArray()) + ")";
      Logs.WriteEvent(updateSql);
      SqlCommand command = new SqlCommand();
      command.CommandText = updateSql;
      command.CommandType = System.Data.CommandType.Text;

      SqlExecutor.ExecuteNonQuery(dataSource.LoginUser, command);
      Logs.WriteEvent("Ticket Indexes Statuses UPdated");
    }

    private void RemoveOldTicketIndexes(LoginUser loginUser, string indexPath, Organization organization)
    {
      if (!Directory.Exists(indexPath)) return;
      DeletedIndexItems items = new DeletedIndexItems(loginUser);
      items.LoadByReferenceType(ReferenceType.Tickets, organization.OrganizationID);
      if (items.IsEmpty) return;
      if (!Directory.Exists(indexPath)) return;

      StringBuilder builder = new StringBuilder();
      foreach (DeletedIndexItem item in items)
      {
        builder.AppendLine(item.RefID.ToString());
      }

      string fileName = Path.Combine(indexPath, "DeletedTickets.txt");
      if (File.Exists(fileName)) File.Delete(fileName);
      using (StreamWriter writer = new StreamWriter(fileName))
      {
        writer.Write(builder.ToString());
      }


      using (IndexJob job = new IndexJob())
      {
        job.IndexPath = indexPath;
        job.ActionCreate = false;
        job.ActionAdd = false;
        job.ActionRemoveListed = true;
        job.ToRemoveListName = fileName;
        job.CreateRelativePaths = false;
        job.Execute();
      }

      items.DeleteAll();
      items.Save();
      Logs.WriteEvent("Finished Removing Old Ticket Indexes - OrgID = " + organization.OrganizationID);
    }
    /*
    private void CompressTicketIndexes(LoginUser loginUser, string indexPath)
    {
      if (!Settings.ReadBool("Compress Indexes", false)) return;

      if (!Settings.ReadBool("Force Compress", false))
      {
        DateTime last = DateTime.Parse(Settings.ReadString("Last Compressed", DateTime.Now.AddYears(-1).ToString()));
        if (last.Subtract(DateTime.Now).TotalHours < 12) return;
        if (DateTime.Now.Hour > 2) return;
      }
      else
      {
        Logs.WriteEvent("Forced Compress");
        Settings.WriteBool("Force Compress", false);
      }



      Logs.WriteEvent("Starting Compression Job");

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

        Settings.WriteString("Compress Status", "Compressing");

        job.ExecuteInThread();

        bool flag = false;
        DateTime start = DateTime.Now;
        IndexProgressInfo status = new IndexProgressInfo();
        while (job.IsThreadDone(1000, status) == false)
        {
          if (IsStopped || !Settings.ReadBool("Compress Indexes", false)) 
          {
            Settings.WriteString("Compress Status", "Aborted");
            flag = true;
            job.AbortThread(); 
          }
        }

        if (!flag)
        {
          Settings.WriteString("Compress Status", "Success");
          Settings.WriteString("Last Compressed", DateTime.Now.ToString());
          Settings.WriteInt("Last Compress Time", (int)DateTime.Now.Subtract(start).TotalSeconds);
        }
        Logs.WriteEvent("Finished Compressing Ticket Indexes");
        Settings.WriteBool("Force Compress", false);

      }
    }
    */
  }
}
