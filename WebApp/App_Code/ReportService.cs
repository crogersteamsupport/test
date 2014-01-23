using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ReportService : System.Web.Services.WebService
    {

      public ReportService() { }

      [WebMethod]
      public ReportColumn[] GetReportColumnNames(int reportID)
      {
        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
        return report.GetTabularColumns();
      }

      
      [WebMethod]
      public GridResult GetReportData(int reportID, int from, int to, string sortField, bool isDesc)
      {
        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);

        if (report.ReportDefType == ReportType.Custom)
        {
          CustomReport customReport = JsonConvert.DeserializeObject<CustomReport>(report.ReportDef);
          if (customReport.UsePaging)
          {
            try
            {
              return GetReportDataPage(report, from, to, sortField, isDesc);
            }
            catch (Exception ex)
            {
              customReport.UsePaging = false;
              report.ReportDef = JsonConvert.SerializeObject(customReport);
              report.Collection.Save();

              try
              {
                return GetReportDataAll(report, sortField, isDesc);
              }
              catch (Exception ex2)
              {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex2, "Custom Report");
                throw;
              }
            }
          }
          else
          {
            return GetReportDataAll(report, sortField, isDesc);
          }
        
        }
        else
        {
          return GetReportDataPage(report, from, to, sortField, isDesc);
        }

      }

      private GridResult GetReportDataPage(Report report, int from, int to, string sortField, bool isDesc)
      {
        from++;
        to++;
        
        SqlCommand command = new SqlCommand();
        string query = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
SELECT  *, (SELECT MAX(RowNum) FROM r) AS 'TotalRows' FROM r
WHERE RowNum BETWEEN @From AND @To";

        if (string.IsNullOrWhiteSpace(sortField))
        {
          ReportColumn[] cols = GetReportColumnNames(report.ReportID);
          sortField = cols[0].Name;
          isDesc = false;
        }
        command.Parameters.AddWithValue("@From", from);
        command.Parameters.AddWithValue("@To", to);
        report.GetCommand(command);
        command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC");

        DataTable table = new DataTable();
        using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
        {
          connection.Open();
          command.Connection = connection;
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            try
            {
              adapter.Fill(table);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Report Data");
              throw;
            }
          }
          connection.Close();
        }

        GridResult result = new GridResult();
        result.From = --from;
        result.To = --to;
        result.Data = JsonConvert.SerializeObject(table);
        return result;
      }

      private GridResult GetReportDataAll(Report report, string sortField, bool isDesc)
      {
        SqlCommand command = new SqlCommand();
        
        report.GetCommand(command);

        if (command.CommandText.ToLower().IndexOf(" order by ") < 0 && !string.IsNullOrWhiteSpace(sortField))
        {
          command.CommandText = command.CommandText + " ORDER BY " + sortField + (isDesc ? "DESC" : "ASC");        
        }

        DataTable table = new DataTable();
        using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
        {
          connection.Open();
          command.Connection = connection;
          using (SqlDataAdapter adapter = new SqlDataAdapter(command))
          {
            try
            {
              adapter.Fill(table);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Report Data");
              throw;
            }
          }
          connection.Close();
        }

        GridResult result = new GridResult();
        result.From = 0;
        result.To = table.Rows.Count-1;
        result.Data = JsonConvert.SerializeObject(table);
        return result;      
      
      }

      [WebMethod]
      public ReportItem[] GetReports()
      {
        List<ReportItem> result = new List<ReportItem>();
        Reports reports = new Reports(TSAuthentication.GetLoginUser());
        reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
        foreach (Report report in reports)
        {
          result.Add(new ReportItem(report, false));
        }

        return result.ToArray();
      }

      [WebMethod]
      public ReportItem GetReport(int reportID)
      {
        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID, TSAuthentication.UserID);
        report.MigrateToNewReport();
        return new ReportItem(report, true);
      }

      [WebMethod]
      public int[] DeleteReports(string reportIDs)
      {
        List<int> result = new List<int>();
        int[] ids = JsonConvert.DeserializeObject<int[]>(reportIDs);
        for (int i = 0; i < ids.Length; i++)
        {
          int reportID = ids[i];// int.Parse(reportIDs[i]);
          Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
          if (report.OrganizationID == null && TSAuthentication.IsSystemAdmin)
          {
            Reports.HideStockReport(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, reportID);
            result.Add(reportID);
          }
          else if (report.OrganizationID == TSAuthentication.OrganizationID && (TSAuthentication.UserID == report.CreatorID || TSAuthentication.IsSystemAdmin))
          {
            report.Delete();
            report.Collection.Save();
            result.Add(reportID);
          }
        }

        return result.ToArray();
      
      }

      [WebMethod]
      public ReportItem CloneReport(int reportID)
      {
        Reports reports = new Reports(TSAuthentication.GetLoginUser());
        reports.LoadAll(TSAuthentication.OrganizationID);

        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);

        int i = 0;
        string reportName = report.Name + " Clone ({0:D})";

        while (true)
        {
          i++;
          if (reports.FindByName(string.Format(reportName, i)) == null) break;
        }

        int newID = report.CloneReport(string.Format(reportName, i));

        Report result = Reports.GetReport(TSAuthentication.GetLoginUser(), newID, TSAuthentication.UserID);
        return new ReportItem(result, false);
      }

      [WebMethod]
      public void SetFavorite(int reportID, bool value)
      {
        Reports.SetIsFavorite(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID,  value);
      }

      [WebMethod]
      public void SetHidden(int reportID, bool value)
      {
        Reports.SetIsHiddenFromUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID, value);
      }

      [WebMethod]
      public ReportCategoryItem[] GetCategories()
      {
        List<ReportCategoryItem> result = new List<ReportCategoryItem>();
        ReportTables tables = new ReportTables(TSAuthentication.GetLoginUser());
        tables.LoadCategories();

        foreach (ReportTable table in tables)
        {
          ReportCategoryItem item = new ReportCategoryItem();
          item.Name = table.Alias;
          item.ReportTableID = table.ReportTableID;
          ReportSubcategories cats = new ReportSubcategories(TSAuthentication.GetLoginUser());
          cats.LoadByReportTableID(table.ReportTableID);

          List<ReportSubcateoryItem> subs = new List<ReportSubcateoryItem>();
          foreach (ReportSubcategory cat in cats)
          {
            ReportSubcateoryItem sub = new ReportSubcateoryItem();
            sub.SubCatID = cat.ReportSubcategoryID;
            sub.Name = cat.ReportTableID != null ? cat.Row["Alias"].ToString() : "None";
            subs.Add(sub);
          }
          item.Subcategories = subs.ToArray();
          result.Add(item);
        }
        return result.ToArray();
      }

      [WebMethod]
      public ReportFieldItem[] GetFields(int reportSubCatID)
      {
        LoginUser loginUser = TSAuthentication.GetLoginUser();
        List<ReportFieldItem> result = new List<ReportFieldItem>();
        TicketTypes ticketTypes = new TicketTypes(loginUser);
        ticketTypes.LoadAllPositions(loginUser.OrganizationID);

        ReportSubcategory subCat = ReportSubcategories.GetReportSubcategory(loginUser, reportSubCatID);
        ReportTable primaryTable = ReportTables.GetReportTable(loginUser, subCat.ReportCategoryTableID);

        ReportTableFields reportTableFields = new ReportTableFields(loginUser);
        reportTableFields.LoadByReportTableID(subCat.ReportCategoryTableID);
        foreach (ReportTableField reportTableField in reportTableFields)
	      {
          result.Add(new ReportFieldItem(primaryTable.Alias, true, reportTableField));
	      }

        if (primaryTable.CustomFieldRefType != ReferenceType.None)
        {
          CustomFields customFields = new CustomFields(loginUser);
          customFields.LoadByReferenceType(loginUser.OrganizationID, primaryTable.CustomFieldRefType);

          foreach (CustomField customField in customFields)
          {
            if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
            {
              TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
              if (ticketType != null) result.Add(new ReportFieldItem(primaryTable.Alias, true, customField, ticketType.Name));
            }
            else
            {
              result.Add(new ReportFieldItem(primaryTable.Alias, true, customField, ""));
            }

          }
        }

        if (subCat.ReportTableID != null)
        {
          ReportTable subTable = ReportTables.GetReportTable(loginUser, (int)subCat.ReportTableID);
          reportTableFields = new ReportTableFields(loginUser);
          reportTableFields.LoadByReportTableID((int)subCat.ReportTableID);
          foreach (ReportTableField reportTableField in reportTableFields)
          {
            result.Add(new ReportFieldItem(subTable.Alias, false, reportTableField));
          }

          
          if (subTable.CustomFieldRefType != ReferenceType.None)
          {
            CustomFields customFields = new CustomFields(loginUser);
            customFields.LoadByReferenceType(loginUser.OrganizationID, subTable.CustomFieldRefType);

            foreach (CustomField customField in customFields)
            {
              if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
              {
                TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                if (ticketType != null) result.Add(new ReportFieldItem(subTable.Alias, false, customField, ticketType.Name));
              }
              else
              {
                result.Add(new ReportFieldItem(subTable.Alias, false, customField, ""));
              }
            }
          }
        }
        return result.ToArray();
      }

      [WebMethod]
      public int? SaveReport(int? reportID, string name, string description, int reportType, string data)
      {
        Report report = null;
        if (reportID == null)
        {
          report = (new Reports(TSAuthentication.GetLoginUser())).AddNewReport();
        }
        else
        {
          report = Reports.GetReport(TSAuthentication.GetLoginUser(), (int)reportID);
          if (!TSAuthentication.IsSystemAdmin && report.CreatorID != TSAuthentication.UserID) return null;
        }

        report.Name = name;
        report.Description = description;
        report.ReportDef = data;
        report.OrganizationID = TSAuthentication.OrganizationID;

        switch (reportType)
        {
          case 0: report.ReportDefType = ReportType.Table; break;
          case 1: report.ReportDefType = ReportType.Chart; break;
          case 2: report.ReportDefType = ReportType.External; break;
          case 4: report.ReportDefType = ReportType.Summary; break;
          default:
            break;
        }

        report.Collection.Save();
        return report.ReportID;
      }

      [WebMethod]
      public void SaveReportDef(int reportID, string data)
      {
        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
        if (!TSAuthentication.IsSystemAdmin && report.CreatorID != TSAuthentication.UserID) return;
        report.ReportDef = data;
        report.Collection.Save();
      }

      [WebMethod]
      public void SaveUserSettings(int reportID, string data)
      {
        Reports.SetUserSettings(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID, data);
      }

      [WebMethod]
      public ReportFolderProxy[] GetFolders()
      {
        ReportFolders folders = new ReportFolders(TSAuthentication.GetLoginUser());
        folders.LoadAll(TSAuthentication.OrganizationID);
        return folders.GetReportFolderProxies();
      }

      [WebMethod]
      public ReportFolderProxy SaveFolder(int? folderID, string name)
      {
        ReportFolder folder = null;
        if (folderID == null)
        { 
          folder = (new ReportFolders(TSAuthentication.GetLoginUser())).AddNewReportFolder();
          folder.OrganizationID = TSAuthentication.OrganizationID;
          folder.CreatorID = TSAuthentication.UserID;
        }
        else
        {
          folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), (int)folderID);
        }
        folder.Name = name.Trim();
        folder.Collection.Save();
        return folder.GetProxy();
      }

      [WebMethod]
      public bool DeleteFolder(int folderID)
      {
        ReportFolder folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), (int)folderID);
        if (!TSAuthentication.IsSystemAdmin && folder.CreatorID != TSAuthentication.UserID) return false;

        Reports.UnassignFolder(TSAuthentication.GetLoginUser(), folderID);
        folder.Delete();
        folder.Collection.Save();
        return true;
      }

      [WebMethod]
      public void MoveReports(string reportIDs, int folderID)
      {
        ReportFolder folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), folderID);
        if (folder.OrganizationID != TSAuthentication.OrganizationID) return;
        int[] ids = JsonConvert.DeserializeObject<int[]>(reportIDs);
        for (int i = 0; i < ids.Length; i++)
        {
          int reportID = ids[i];
          Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
          if (report.OrganizationID == TSAuthentication.OrganizationID)
          {
            Reports.AssignFolder(TSAuthentication.GetLoginUser(), folderID, TSAuthentication.OrganizationID, reportID);
          }
        }
      }


      [DataContract]
      public class ReportFieldItem
      {
        public ReportFieldItem() { }
        public ReportFieldItem(string table, bool isPrimary, ReportTableField field)
        {
          this.IsPrimary = isPrimary;
          this.ID = field.ReportTableFieldID;
          this.Name = field.Alias;
          this.LookupTableID = field.LookupTableID;
          this.IsCustom = false;
          this.Table = table;

          switch (field.DataType)
          {
            case "datetime": this.DataType = "datetime"; break;
            case "bit": this.DataType = "bool"; break;
            case "float": this.DataType = "number"; break;
            default: this.DataType = "text"; break;
          }

        }

        public ReportFieldItem(string table, bool isPrimary, CustomField field, string AuxName)
        {
          this.IsPrimary = isPrimary;
          this.ID = field.CustomFieldID;
          this.Name = field.Name;
          this.ListValues = string.IsNullOrWhiteSpace(field.ListValues) ? null : field.ListValues.Split('|');
          this.IsCustom = true;
          this.AuxName = AuxName;
          this.Table = table;
          switch (field.FieldType)
          {
            case CustomFieldType.DateTime: this.DataType = "datetime"; break;
            case CustomFieldType.Boolean: this.DataType = "bool"; break;
            case CustomFieldType.Number: this.DataType = "number"; break;
            default: this.DataType = "text"; break;
          }

        }

        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Table { get; set; }
        [DataMember]
        public string AuxName { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public int? LookupTableID { get; set; }
        [DataMember]
        public string[] ListValues { get; set; }
        [DataMember]
        public bool IsCustom { get; set; }
        [DataMember]
        public bool IsPrimary { get; set; }

      }

      [DataContract]
      public class ReportCategoryItem
      { 
        [DataMember] public int ReportTableID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public ReportSubcateoryItem[] Subcategories { get; set; }
      }

      [DataContract]
      public class ReportSubcateoryItem
      {
        [DataMember] public string Name { get; set; }
        [DataMember] public int SubCatID { get; set; }
      }

      [DataContract]
      public class ReportItem
      {
 
        public ReportItem(Report report, bool indcludeDef)
        {
          this.ReportID = report.ReportID;
          this.OrganizationID = report.OrganizationID;
          this.Name = report.Name;
          this.Description = report.Description;
          this.IsFavorite = (report.Row.Table.Columns.IndexOf("IsFavorite") < 0 || report.Row["IsFavorite"] == DBNull.Value ? false : (bool)report.Row["IsFavorite"]);
          this.IsHidden = (report.Row.Table.Columns.IndexOf("IsHidden") < 0 || report.Row["IsHidden"] == DBNull.Value ? false : (bool)report.Row["IsHidden"]);
          this.UserSettings = (report.Row.Table.Columns.IndexOf("Settings") < 0 || report.Row["Settings"] == DBNull.Value ? "" : (string)report.Row["Settings"]);
          this.LastModified = report.DateModifiedUtc;
          this.LastViewed = (report.Row.Table.Columns.IndexOf("LastViewed") < 0 || report.Row["LastViewed"] == DBNull.Value ? null : (DateTime?)report.Row["LastViewed"]);
          this.Creator = (report.Row.Table.Columns.IndexOf("Creator") < 0 || report.Row["Creator"] == DBNull.Value ? "" : (string)report.Row["Creator"]);
          this.Modifier = (report.Row.Table.Columns.IndexOf("Modifier") < 0 || report.Row["Modifier"] == DBNull.Value ? "" : (string)report.Row["Modifier"]);
          this.FolderID = (report.Row.Table.Columns.IndexOf("FolderID") < 0 || report.Row["FolderID"] == DBNull.Value ? null : (int?)report.Row["FolderID"]);
          if ((int)report.ReportDefType < 0)
          {
            if (!string.IsNullOrWhiteSpace(report.ExternalURL))
            {
              this.ReportType = ReportType.External;
            }
            else if (!string.IsNullOrWhiteSpace(report.Query))
            {
              this.ReportType = ReportType.Custom;
            }
            else if ((int)report.ReportType == 3)
            {
              this.ReportType = ReportType.Chart;
            }
            else
            {
              this.ReportType = ReportType.Table;
            }
          }
          else
          {
            this.ReportType = report.ReportDefType;
          }

          if (indcludeDef) this.ReportDef = report.ReportDef;
          
          this.CreatorID = report.CreatorID;
        }

        [DataMember] public int ReportID { get; set; }
        [DataMember] public int? OrganizationID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public ReportType ReportType { get; set; }
        [DataMember] public string ReportDef { get; set; }
        [DataMember] public string UserSettings { get; set; }
        [DataMember] public bool IsFavorite { get; set; }
        [DataMember] public bool IsHidden { get; set; }
        [DataMember] public int CreatorID { get; set; }
        [DataMember] public string Creator { get; set; }
        [DataMember] public string Modifier { get; set; }
        [DataMember] public DateTime? LastViewed { get; set; }
        [DataMember] public DateTime LastModified { get; set; }
        [DataMember] public int? FolderID { get; set; }
      }

      [DataContract]
      public class GridResult
      {
        public GridResult() { }
        [DataMember] public int From { get; set; }
        [DataMember] public int To { get; set; }
        [DataMember] public string Data { get; set; }
      }


      
    }
}