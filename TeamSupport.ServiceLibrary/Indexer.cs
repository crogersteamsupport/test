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
  [Serializable]
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
          ProcessIndex(org, ReferenceType.Tickets);
          ProcessIndex(org, ReferenceType.Wikis);
          UpdateHealth();
        }

        
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer"); 
      }
      Logs.WriteEvent("Finished Indexing");


    }


    private void ProcessIndex(Organization organization, ReferenceType referenceType)
    {
      string indexPath                            = string.Empty;
      string deletedIndexItemsFileName            = string.Empty;
      string storedFields                         = string.Empty;
      string tableName                            = string.Empty;
      string primaryKeyName                       = string.Empty;

      IndexDataSource indexDataSource = null;

      switch(referenceType)
      {
        case ReferenceType.Tickets:
           indexPath                            = "\\Tickets";
           deletedIndexItemsFileName            = "DeletedTickets.txt";
           storedFields                         = "TicketID OrganizationID TicketNumber Name IsKnowledgeBase Status Severity DateModified DateCreated DateClosed SlaViolationDate SlaWarningDate";
           tableName                            = "Tickets";
           primaryKeyName                       = "TicketID";
           indexDataSource                      = new TicketIndexDataSource(LoginUser, Settings.ReadInt("Max Records", 1000), organization.OrganizationID);
           break;
        case ReferenceType.Wikis:
           indexPath                            = "\\Wikis";
           deletedIndexItemsFileName            = "DeletedWikis.txt";
           storedFields                         = "OrganizationID Creator Modifier";
           tableName                            = "WikiArticles";
           primaryKeyName                       = "ArticleID";
           indexDataSource                      = new WikiIndexDataSource(LoginUser, Settings.ReadInt("Max Records", 1000), organization.OrganizationID);
           break;
        default:
          throw new System.ArgumentException("ReferenceType " + referenceType.ToString() + " is not supported by indexer."); 
      }


      string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + indexPath);
      Logs.WriteEvent("Path: " + path);
      bool isNew = !System.IO.Directory.Exists(path);

      if (isNew) { Directory.CreateDirectory(path); }

      try
      {
        RemoveOldIndexItems(LoginUser, path, organization, referenceType, deletedIndexItemsFileName);
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer.RemoveOldIndexItems - " + referenceType.ToString() + " - " + organization.OrganizationID.ToString()); 
      }

      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;

      using (IndexJob job = new IndexJob())
      {
        job.DataSourceToIndex = indexDataSource;

        job.IndexPath = path;
        job.ActionCreate = isNew;
        job.ActionAdd = true;
        job.CreateRelativePaths = false;
        job.StoredFields = Server.Tokenize(storedFields);

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
          while (job.IsThreadDone(1000, status) == false)
          {
            if (IsStopped) { job.AbortThread(); }
          }
        }
        catch (Exception ex)
        {
          ExceptionLogs.LogException(LoginUser, ex, "Index Job Processor - " + referenceType.ToString() + " - " + organization.OrganizationID.ToString());
          Logs.WriteException(ex);
          throw;
        }

        UpdateItems(indexDataSource, tableName, primaryKeyName);
      }
    }

    private void UpdateItems(IndexDataSource dataSource, string tableName, string primaryKeyName)
    {
      UpdateHealth();

      if (dataSource.UpdatedItems.Count < 1) return;

      string updateSql = "UPDATE " + tableName + " SET NeedsIndexing = 0 WHERE " + primaryKeyName + " IN (" + DataUtils.IntArrayToCommaString(dataSource.UpdatedItems.ToArray()) + ")";
      Logs.WriteEvent(updateSql);
      SqlCommand command = new SqlCommand();
      command.CommandText = updateSql;
      command.CommandType = System.Data.CommandType.Text;

      SqlExecutor.ExecuteNonQuery(LoginUser, command);
      Logs.WriteEvent(tableName + " Indexes Statuses UPdated");
    }

    private void RemoveOldIndexItems(
      LoginUser loginUser
      , string indexPath
      , Organization organization
      , ReferenceType referenceType
      , string deletedIndexItemsFileName
    )
    {
      if (!Directory.Exists(indexPath)) return;
      DeletedIndexItems items = new DeletedIndexItems(loginUser);
      items.LoadByReferenceType(referenceType, organization.OrganizationID);
      if (items.IsEmpty) return;
      if (!Directory.Exists(indexPath)) return;

      StringBuilder builder = new StringBuilder();
      foreach (DeletedIndexItem item in items)
      {
        builder.AppendLine(item.RefID.ToString());
      }

      string fileName = Path.Combine(indexPath, deletedIndexItemsFileName);
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

      UpdateHealth();
      items.DeleteAll();
      items.Save();
      Logs.WriteEvent("Finished Removing Old Indexes - OrgID = " + organization.OrganizationID + " - " + referenceType.ToString());
    }

  }
}
