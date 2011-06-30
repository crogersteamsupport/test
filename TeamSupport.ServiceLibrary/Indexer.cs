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
    private Logs logs;

    public override void Run()
    {
      logs = new Logs(LoginUser, ServiceName, "Tickets");
      try
      {
        ProcessTicketIndex();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Indexer"); 
      }


    }

    public override string ServiceName
    {
      get { return "Indexer"; }
    }

    private void ProcessTicketIndex()
    {
      logs.Log("Starting Ticket Index");
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (IndexJob job = new IndexJob())
      {
        TicketIndexDataSource dataSource = new TicketIndexDataSource();
        dataSource.LoginUser = LoginUser;
        dataSource.MaxCount = Settings.ReadInt("Max Records", 1000);
        job.DataSourceToIndex = dataSource;
        
        string path = Settings.ReadString("Tickets Index Path", "c:\\Indexes\\Tickets");
        RemoveOldTicketIndexes(dataSource.LoginUser, path);
        bool isNew = !System.IO.Directory.Exists(path);
        job.IndexPath = path;
        job.ActionCreate = isNew;
        job.ActionAdd = true;
        job.CreateRelativePaths = false;
        job.StoredFields = Server.Tokenize("TicketID OrganizationID TicketNumber Name");
        job.IndexingFlags =
            IndexingFlags.dtsIndexCacheOriginalFile |
            IndexingFlags.dtsIndexCacheText |
            IndexingFlags.dtsIndexCacheTextWithoutFields;
        ExecuteJob(job, "IndexerTicketsStatus");
        UpdateTickets(dataSource);
      }
    }

    private void UpdateTickets(TicketIndexDataSource dataSource)
    {
      logs.Log("Started Updating Ticket Indexes Statuses");

      if (dataSource.UpdatedTickets.Count < 1) return;

      StringBuilder builder = new StringBuilder();
      foreach (KeyValuePair<int, int> item in dataSource.UpdatedTickets)
      {
        string sql = string.Format("UPDATE Tickets SET NeedsIndexing = 0, DocID = {1} WHERE TicketID = {0};", item.Key.ToString(), item.Value.ToString());
        builder.AppendLine(sql);
      }

      SqlCommand command = new SqlCommand();
      command.CommandText = builder.ToString();
      command.CommandType = System.Data.CommandType.Text;

      SqlExecutor.ExecuteNonQuery(dataSource.LoginUser, command);
      logs.Log("Finished Updating Ticket Indexes Statuses");
    }

    private void RemoveOldTicketIndexes(LoginUser loginUser, string indexPath)
    {
      logs.Log("Started Removing Old Ticket Indexes");
      if (!Directory.Exists(indexPath)) return;
      DeletedIndexItems items = new DeletedIndexItems(loginUser);
      items.LoadByReferenceType(ReferenceType.Tickets);
      if (items.IsEmpty) return;
      if (!Directory.Exists(indexPath)) return;

      StringBuilder builder = new StringBuilder();
      foreach (DeletedIndexItem item in items)
      {
        builder.AppendLine(item.RefID.ToString());
      }

      string fileName = Path.Combine(indexPath, "DeletedTicketIDs.txt");
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
      logs.Log("Finished Removing Old Ticket Indexes");
    }

    private void ExecuteJob(IndexJob job, string statusKey)
    {
      job.ExecuteInThread();
      
      // Monitor the job execution thread as it progresses
      IndexProgressInfo status = new IndexProgressInfo();
      while (job.IsThreadDone(500, status) == false)
      {
        if (IsStopped) { job.AbortThread(); }
      }
      logs.Log("Finished Ticket Index");

    }

  }
}
