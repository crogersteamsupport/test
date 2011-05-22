using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;

namespace TeamSupport.ServiceLibrary
{
  public class Indexer : ServiceThread
  {
    public override void Run()
    {
      try
      {
        ProcessTicketIndex();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(Utils.GetLoginUser("Indexer"), ex, "Indexer"); 
      }


    }

    private void ProcessTicketIndex()
    {
      using (IndexJob job = new IndexJob())
      {
        TicketIndexDataSource dataSource = new TicketIndexDataSource();
        dataSource.LoginUser = Utils.GetLoginUser("Ticket Indexer");
        Utils.SetSettingValue("IndexerTicketsLastStart", DateTime.Now.ToString(), Microsoft.Win32.RegistryValueKind.String);
        job.DataSourceToIndex = dataSource;
        string path = Utils.GetSettingString("IndexerTicketsPath", "c:\\Indexes\\Tickets");
        RemoveOldTicketIndexes(dataSource.LoginUser, path);
        bool isNew = !System.IO.Directory.Exists(path);
        job.IndexPath = path;
        job.ActionCreate = isNew;
        job.ActionAdd = true;
        job.CreateRelativePaths = false;
        job.StoredFields = Server.Tokenize("TicketID OrganizationID TicketNumber Name");
        //job.StoredFields = Server.Tokenize(StoredFields.Text);
        job.IndexingFlags =
          // Compress and store the documents in the index (for highlighting hits)
            IndexingFlags.dtsIndexCacheOriginalFile |
          // Compress and store document text in the index (for generating hits-in-context 
          // snippets to include in search results)
            IndexingFlags.dtsIndexCacheText |
          // Prevents fields added with DataSource.DocFields from being included in cached text
            IndexingFlags.dtsIndexCacheTextWithoutFields;

        // Execute the index job
        Utils.SetSettingValue("IndexerTicketsStatus", "Starting Job", Microsoft.Win32.RegistryValueKind.String);
        ExecuteJob(job, "IndexerTicketsStatus");

      }
    
    }

    private void RemoveOldTicketIndexes(LoginUser loginUser, string indexPath)
    {
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
        Utils.SetSettingValue("IndexerTicketsStatus", "Deleting Old Tickets", Microsoft.Win32.RegistryValueKind.String);
        job.Execute();
      }

      items.DeleteAll();
      items.Save();
    }

    private void ExecuteJob(IndexJob job, string statusKey)
    {
      job.ExecuteInThread();
      
      // Monitor the job execution thread as it progresses
      IndexProgressInfo status = new IndexProgressInfo();
      while (job.IsThreadDone(50, status) == false)
      {
        string statusText = "";
        // Set the status text based on the current indexing step
        switch (status.Step)
        {
          case IndexingStep.ixStepBegin:
            statusText = "Opening index";
            break;

          case IndexingStep.ixStepCheckingFiles:
            statusText = "Checking files";
            break;

          case IndexingStep.ixStepCompressing:
            statusText = "Compressing index";
            break;

          case IndexingStep.ixStepCreatingIndex:
            statusText = "Creating index";
            break;

          case IndexingStep.ixStepDone:
            statusText = "Indexing Complete";
            break;
          case IndexingStep.ixStepMerging:
            statusText = "Merging words into index";
            break;

          case IndexingStep.ixStepNone:
            statusText = "";
            break;

          case IndexingStep.ixStepReadingFiles:
            statusText = status.File.Name;
            break;

          case IndexingStep.ixStepStoringWords:
            statusText = status.File.Name + " (storing words)";
            break;

          default:
            statusText = "";
            break;
        }

        if (IsStopped) { job.AbortThread(); }
      }

      Utils.SetSettingValue("IndexerTicketsLastEnd", DateTime.Now.ToString(), Microsoft.Win32.RegistryValueKind.String);
    }

  }
}
