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
      public ReportColumn[] GetReportColumns(int reportID)
      {
        return Reports.GetReportColumns(TSAuthentication.GetLoginUser(), reportID);
      }

      [WebMethod]
      public GridResult GetReportData(int reportID, int from, int to, string sortField, bool isDesc)
      {
        return Reports.GetReportData(TSAuthentication.GetLoginUser(), reportID, from, to, sortField, isDesc);
      }

      [WebMethod]
      public string GetSummary(string data)
      {
        DataTable table = Reports.GetSummaryData(TSAuthentication.GetLoginUser(), JsonConvert.DeserializeObject<SummaryReport>(data));


        if (table.Columns.Count == 2)
        {
          Object[] result = new Object[table.Rows.Count];
          for (int i = 0; i < table.Rows.Count; i++)
			    {
            DataRow row = table.Rows[i];
            result[i] = new Object[] { (row[0] == DBNull.Value ? "" : row[0].ToString()), (row[1] == DBNull.Value ? 0 : row[1]) };
			    }
          return JsonConvert.SerializeObject(result);
        }
        else if (table.Columns.Count == 3)
        {
          SortedDictionary<string, List<Object[]>> dic = new SortedDictionary<string, List<Object[]>>();
         
          for (int i = 0; i < table.Rows.Count; i++)
          {
            DataRow row = table.Rows[i];
            string series = row[0] == DBNull.Value ? "" : row[0].ToString();
            

            Object[] point = new Object[] { (row[1] == DBNull.Value ? "" : row[1].ToString()), (row[2] == DBNull.Value ? 0 : row[2]) };
            if (dic.ContainsKey(series))
            {
              dic[series].Add(point);
            }
            else
	          {
              List<Object[]> list = new List<object[]>();
              list.Add(point);
              dic.Add(series, list);
	          }
          }
          List<ChartSeries> result = new List<ChartSeries>();

          foreach (KeyValuePair<string, List<object[]>> entry in dic)
          {
            ChartSeries item = new ChartSeries();
            item.SeriesName = entry.Key;
            item.Value = entry.Value.ToArray();
            result.Add(item);
          }
          return JsonConvert.SerializeObject(result);
        }

        return "";
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
      public ReportItem SaveReport(int? reportID, string name, int reportType, string data)
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
        return new ReportItem(report, true);
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
          Reports.AssignFolder(TSAuthentication.GetLoginUser(), folderID, TSAuthentication.OrganizationID, ids[i]);
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
            case "int": this.DataType = "number"; break;
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
      public class ChartSeries
      {
        [DataMember] public string SeriesName { get; set; }
        [DataMember] public Object[] Value { get; set; }
      }




      
    }
}