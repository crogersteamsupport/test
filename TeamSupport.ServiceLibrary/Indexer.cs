using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class Indexer : ServiceThread
  {
    public override void Run()
    {
      try
      {
        bool isRebuilder = ConfigurationManager.AppSettings["RebuilderMode"] != null && ConfigurationManager.AppSettings["RebuilderMode"] == "1";
        Organizations orgs = new Organizations(LoginUser);
        if (!isRebuilder)
        {
          orgs.LoadByNeedsIndexing();
        }
        else 
        {
          int daysSinceLastRebuild = ConfigurationManager.AppSettings["DaysSinceLastRebuild"] == null ? 14 : int.Parse(ConfigurationManager.AppSettings["DaysSinceLastRebuild"]);
          int minutesSinceLastActive = ConfigurationManager.AppSettings["MinutesSinceLastActive"] == null ? 30 : int.Parse(ConfigurationManager.AppSettings["MinutesSinceLastActive"]);
          orgs.LoadByNeedsIndexRebuilt(minutesSinceLastActive, daysSinceLastRebuild);
          Logs.WriteEvent(string.Format("Running in rebuilding mode.  Days since last rebuild: {0}, Minutes Since Last Active: {1}", daysSinceLastRebuild.ToString(), minutesSinceLastActive.ToString()));
        }
        int cnt = 0;
        foreach (Organization org in orgs)
        {
          UpdateHealth();
          Logs.WriteLine();
          Logs.WriteLine();
          Logs.WriteEvent(string.Format("Started Indexing for org: {0}, [{1}/{2}]", org.OrganizationID.ToString(), cnt.ToString(), orgs.Count.ToString()));

          try
          {
            if (!isRebuilder)
            {
              if (IsLocked(org.OrganizationID))
              {
                Logs.WriteEvent("Skipped because the index was locked.");
                continue;
              }
              else
              {
                LockIndex(org.OrganizationID, true);
              }
            }
            else
            {
              org.IsRebuildingIndex = true;
              org.Collection.Save();
              Logs.WriteEvent("IsRebuildingIndex set to TRUE");
            }

            cnt++;
            ProcessIndex(org, ReferenceType.Tickets, isRebuilder);
            ProcessIndex(org, ReferenceType.Wikis, isRebuilder);
            ProcessIndex(org, ReferenceType.Notes, isRebuilder);
            ProcessIndex(org, ReferenceType.ProductVersions, isRebuilder);
            ProcessIndex(org, ReferenceType.WaterCooler, isRebuilder);
            ProcessIndex(org, ReferenceType.Organizations, isRebuilder);
            ProcessIndex(org, ReferenceType.Contacts, isRebuilder);
            if (isRebuilder)
            {
              org.LastIndexRebuilt = DateTime.UtcNow;
              Logs.WriteEvent("LastIndexRebuilt Updated");
              org.Collection.Save();
            }
          }
          finally
          {
            if (isRebuilder)
            {
              org.IsRebuildingIndex = false;
              org.Collection.Save();
              Logs.WriteEvent("IsRebuildingIndex set to FALSE");
            }
            else
            {
              LockIndex(org.OrganizationID, false);
            }
          }
        }

        
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer"); 
      }
    }

    public override void Start()
    {
      SqlExecutor.ExecuteNonQuery(LoginUser, "UPDATE Organizations SET IsIndexLocked = 0 WHERE IsIndexLocked=1");
      base.Start();

    }

    private void ProcessIndex(Organization organization, ReferenceType referenceType, bool isRebuilder)
    {
      string indexPath                            = string.Empty;
      string deletedIndexItemsFileName            = string.Empty;
      string storedFields                         = string.Empty;
      string tableName                            = string.Empty;
      string primaryKeyName                       = string.Empty;

      IndexDataSource indexDataSource = null;
      int maxRecords = Settings.ReadInt("Max Records", 1000);
      

      switch(referenceType)
      {
        case ReferenceType.Tickets:
           indexPath                            = "\\Tickets";
           deletedIndexItemsFileName            = "DeletedTickets.txt";
           storedFields                         = "TicketID OrganizationID TicketNumber Name IsKnowledgeBase Status Severity DateModified DateCreated DateClosed SlaViolationDate SlaWarningDate";
           tableName                            = "Tickets";
           primaryKeyName                       = "TicketID";
           indexDataSource                      = new TicketIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.Wikis:
           indexPath                            = "\\Wikis";
           deletedIndexItemsFileName            = "DeletedWikis.txt";
           storedFields                         = "OrganizationID Creator Modifier";
           tableName                            = "WikiArticles";
           primaryKeyName                       = "ArticleID";
           indexDataSource = new WikiIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.Notes:
           indexPath                            = "\\Notes";
           deletedIndexItemsFileName            = "DeletedNotes.txt";
           tableName                            = "Notes";
           primaryKeyName                       = "NoteID";
           indexDataSource = new NoteIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.ProductVersions:
           indexPath                            = "\\ProductVersions";
           deletedIndexItemsFileName            = "DeletedProductVersions.txt";
           tableName                            = "ProductVersions";
           primaryKeyName                       = "ProductVersionID";
           indexDataSource = new ProductVersionIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.WaterCooler:
           indexPath                            = "\\WaterCooler";
           deletedIndexItemsFileName            = "DeletedWaterCoolerMessages.txt";
           tableName                            = "WatercoolerMsg";
           primaryKeyName                       = "MessageID";
           indexDataSource = new WaterCoolerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.Organizations:
           indexPath                            = "\\Customers";
           deletedIndexItemsFileName            = "DeletedCustomers.txt";
           storedFields                         = "Name JSON";
           tableName                            = "Organizations";
           primaryKeyName                       = "OrganizationID";
           indexDataSource = new CustomerIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        case ReferenceType.Contacts:
           indexPath                            = "\\Contacts";
           deletedIndexItemsFileName            = "DeletedContacts.txt";
           storedFields                         = "Name JSON";
           tableName                            = "Users";
           primaryKeyName                       = "UserID";
           indexDataSource = new ContactIndexDataSource(LoginUser, maxRecords, organization.OrganizationID, isRebuilder);
           break;
        default:
          throw new System.ArgumentException("ReferenceType " + referenceType.ToString() + " is not supported by indexer."); 
      }

      string mainIndexPath = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + indexPath);
      if (isRebuilder) indexPath = "\\Rebuild" + indexPath;
      string path = Path.Combine(Settings.ReadString("Tickets Index Path", "c:\\Indexes"), organization.OrganizationID.ToString() + indexPath);
      Logs.WriteEvent("Path: " + path);

      bool isNew = !System.IO.Directory.Exists(path);

      if (isNew) { 
        Directory.CreateDirectory(path);
        Logs.WriteEvent("Createing path: " + path);
      }

      if (isRebuilder) DeleteIndex(path);

      try
      {
        if (!isRebuilder && !organization.IsRebuildingIndex) RemoveOldIndexItems(LoginUser, path, organization, referenceType, deletedIndexItemsFileName);
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Indexer.RemoveOldIndexItems - " + referenceType.ToString() + " - " + organization.OrganizationID.ToString()); 
      }

      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      Logs.WriteEvent("Processing " + tableName);
      using (IndexJob job = new IndexJob())
      {
        job.DataSourceToIndex = indexDataSource;

        job.IndexPath = path;
        job.ActionCreate = isNew || isRebuilder;
        job.ActionAdd = true;
        job.CreateRelativePaths = false;
        job.StoredFields = Server.Tokenize(storedFields);
        //string tempPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TempIndexFiles" + indexPath);
        //if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
        //job.TempFileDir = tempPath;

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

        if (!isRebuilder && !organization.IsRebuildingIndex)
        {
          UpdateItems(indexDataSource, tableName, primaryKeyName);
        }
        else
        {
          MoveRebuiltIndex(organization.OrganizationID, mainIndexPath, path);
        }

        
      }
    }

    private void DeleteIndex(string path)
    {
      Logs.WriteEvent("Deleting Index Files: " + path);

      while (true)
      {
        int count = 0;
        try
        {
          count++;
          DirectoryInfo dir = new DirectoryInfo(path);
          if (!dir.Exists) return;

          foreach (FileInfo file in dir.GetFiles())
          {
            file.Delete();
          }

          break;
        }
        catch (Exception ex)
        {
          Logs.WriteException(ex);
          if (count > 3)
          {
            Logs.WriteEvent("Too many exceptions deleting old index. exiting...");
            throw;
          }
        }
        
      }

    }

    private void MoveRebuiltIndex(int organizationID, string indexPath, string rebuiltPath)
    {
      int count = 0;
      while (IsLocked(organizationID))
      {
        Logs.WriteEvent("Index Locked, trying again...");
        count++;
        System.Threading.Thread.Sleep(5000);
        if (count > 10)
        {
          Exception ex = new Exception("Could not obain index lock to move rebuilt index");
          ExceptionLogs.LogException(LoginUser, ex, "Index Rebuilder");
          Logs.WriteException(ex);
          throw ex;
        }
      }
      LockIndex(organizationID, true);
      try
      {
        try
        {
          DeleteIndex(indexPath);

            if (Directory.Exists(indexPath))
            {
              Logs.WriteEvent("Deleting: " + indexPath);
              Directory.Delete(indexPath, true);
            }

            Logs.WriteEvent(string.Format("Moving files from {0} to {1}", rebuiltPath, indexPath));
            Directory.Move(rebuiltPath, indexPath);
        }
        finally
        {
          LockIndex(organizationID, false);
        }
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        throw;
      }
    }

    private void LockIndex(int organizationID, bool value)
    {
      Logs.WriteEvent(string.Format("Setting Index Lock, OrganizationID: {0:D}  Value: {1}", organizationID, value.ToString()));
      SqlCommand command = new SqlCommand();
      command.CommandText = "UPDATE Organizations SET IsIndexLocked = @value WHERE OrganizationID = @OrganizationID";
      command.Parameters.AddWithValue("value", value);
      command.Parameters.AddWithValue("OrganizationID", organizationID);
      SqlExecutor.ExecuteNonQuery(LoginUser, command);
    }

    private bool IsLocked(int organizationID)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = "SELECT IsIndexLocked FROM Organizations WHERE OrganizationID = @OrganizationID";
      command.Parameters.AddWithValue("OrganizationID", organizationID);
      object o = SqlExecutor.ExecuteScalar(LoginUser, command);
      return o != null && (bool)o;
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
      Logs.WriteEvent("Removing deleted items:  " + referenceType.ToString());
      if (!Directory.Exists(indexPath))
      {
        Logs.WriteEvent("Path does not exist:  " + indexPath);
        return;
      }
      DeletedIndexItems items = new DeletedIndexItems(loginUser);
      items.LoadByReferenceType(referenceType, organization.OrganizationID);
      if (items.IsEmpty)
      {
        Logs.WriteEvent("No Items to delete");
        return;
      }

      
      StringBuilder builder = new StringBuilder();
      foreach (DeletedIndexItem item in items)
      {
        builder.AppendLine(item.RefID.ToString());
      }

      string fileName = Path.Combine(indexPath, deletedIndexItemsFileName);
      if (File.Exists(fileName)) File.Delete(fileName);
      using (StreamWriter writer = new StreamWriter(fileName))
      {
        Logs.WriteEvent("Adding IDs to delete file: " + builder.ToString());
        writer.Write(builder.ToString());
      }


      Logs.WriteEvent("Deleting Items");
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

      Logs.WriteEvent("Items deleted");
      UpdateHealth();
      items.DeleteAll();
      items.Save();
      Logs.WriteEvent("Finished Removing Old Indexes - OrgID = " + organization.OrganizationID + " - " + referenceType.ToString());
    }

  }


}
