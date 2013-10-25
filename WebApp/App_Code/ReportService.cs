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

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ReportService : System.Web.Services.WebService
    {

      public ReportService() { }

      [WebMethod]
      public ReportItem[] GetReports()
      {
        List<ReportItem> result = new List<ReportItem>();
        Reports reports = new Reports(TSAuthentication.GetLoginUser());
        reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
        foreach (Report report in reports)
        {
          result.Add(new ReportItem(report));
        }

        return result.ToArray();
      }

      [WebMethod]
      public void DeleteReport(int reportID)
      {
        Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
        if (report.OrganizationID == null)
        {
          Reports.HideStockReport(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, reportID);
        }
        else if (report.OrganizationID == TSAuthentication.OrganizationID && (TSAuthentication.UserID == report.CreatorID || TSAuthentication.IsSystemAdmin))
        {
          report.Delete();
          report.Collection.Save();
        }
      
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

        Report result = Reports.GetReport(TSAuthentication.GetLoginUser(), newID);
        return new ReportItem(result);
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
            if (cat.ReportTableID != null)
            {
              ReportSubcateoryItem sub = new ReportSubcateoryItem();
              sub.SubCatID = cat.ReportSubcategoryID;
              sub.Name = cat.Row["Alias"].ToString();
              subs.Add(sub);
            }
          }
          item.Subcategories = subs.ToArray();
          result.Add(item);
        }
        return result.ToArray();
      }

      [WebMethod]
      public ReportFieldItem[] GetFields(int reportTableID, int reportSubCatID)
      {
        LoginUser loginUser = TSAuthentication.GetLoginUser();
        List<ReportFieldItem> result = new List<ReportFieldItem>();

        TicketTypes ticketTypes = new TicketTypes(loginUser);
        ticketTypes.LoadAllPositions(loginUser.OrganizationID);

        ReportSubcategory subCat = reportSubCatID < 0 ? null : ReportSubcategories.GetReportSubcategory(loginUser, reportSubCatID);

        ReportTableFields reportTableFields = new ReportTableFields(loginUser);
        reportTableFields.LoadByReportTableID(reportTableID);
        foreach (ReportTableField reportTableField in reportTableFields)
	      {
          result.Add(new ReportFieldItem(true, reportTableField));
	      }

        ReportTable table = (ReportTable)ReportTables.GetReportTable(loginUser, reportTableID);
        if (table.CustomFieldRefType != ReferenceType.None)
        {
          CustomFields customFields = new CustomFields(loginUser);
          customFields.LoadByReferenceType(loginUser.OrganizationID, table.CustomFieldRefType);

          foreach (CustomField customField in customFields)
          {
            if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
            {
              TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
              if (ticketType != null) result.Add(new ReportFieldItem(true, customField, ticketType.Name));
            }
            else
            {
              result.Add(new ReportFieldItem(true, customField, ""));
            }

          }
        }

        if (subCat != null)
        {
          reportTableFields = new ReportTableFields(loginUser);
          reportTableFields.LoadByReportTableID((int)subCat.ReportTableID);
          foreach (ReportTableField reportTableField in reportTableFields)
          {
            result.Add(new ReportFieldItem(false, reportTableField));
          }

          table = (ReportTable)ReportTables.GetReportTable(loginUser, (int)subCat.ReportTableID);
          if (table.CustomFieldRefType != ReferenceType.None)
          {
            CustomFields customFields = new CustomFields(loginUser);
            customFields.LoadByReferenceType(loginUser.OrganizationID, table.CustomFieldRefType);

            foreach (CustomField customField in customFields)
            {
              if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
              {
                TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                if (ticketType != null) result.Add(new ReportFieldItem(false, customField, ticketType.Name));
              }
              else
              {
                result.Add(new ReportFieldItem(false, customField, ""));
              }
            }
          }
        }
        return result.ToArray();
      }

      [DataContract]
      public class ReportFieldItem
      {
        public ReportFieldItem() { }
        public ReportFieldItem(bool isPrimary, ReportTableField field)
        {
          this.IsPrimary = IsPrimary;
          this.ID = field.ReportTableFieldID;
          //this.DataType = field.DataType;
          this.Name = field.Alias;
          this.LookupTableID = field.LookupTableID;
          this.IsCustom = false;

          switch (field.DataType)
          {
            case "datetime": this.DataType = "datetime"; break;
            case "bit": this.DataType = "bool"; break;
            case "float": this.DataType = "number"; break;
            default: this.DataType = "text"; break;
          }

        }

        public ReportFieldItem(bool isPrimary, CustomField field, string AuxName)
        {
          this.IsPrimary = IsPrimary;
          this.ID = field.CustomFieldID;
          this.Name = field.Name;
          this.ListValues = string.IsNullOrWhiteSpace(field.ListValues) ? null : field.ListValues.Split('|');
          this.IsCustom = true;
          this.AuxName = AuxName;
          switch (field.FieldType)
          {
            case CustomFieldType.DateTime: this.DataType = "datetime"; break;
            case CustomFieldType.Boolean: this.DataType = "bool"; break;
            case CustomFieldType.Number: this.DataType = "number"; break;
            default: this.DataType = "text"; break;
          }
        
        }
        
        [DataMember] public int ID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string AuxName { get; set; }
        [DataMember] public string DataType { get; set; }
        [DataMember] public int? LookupTableID { get; set; }
        [DataMember] public string[] ListValues { get; set; }
        [DataMember] public bool IsCustom { get; set; }
        [DataMember] public bool IsPrimary { get; set; }

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
        public ReportItem(Report report)
        {
          this.ReportID = report.ReportID;
          this.OrganizationID = report.OrganizationID;
          this.Name = report.Name;
          this.Description = report.Description;
          this.IsFavorite = (report.Row.Table.Columns.IndexOf("IsFavorite") < 0 || report.Row["IsFavorite"] == DBNull.Value ? false : (bool)report.Row["IsFavorite"]);
          this.IsHidden = (report.Row.Table.Columns.IndexOf("IsHidden") < 0 || report.Row["IsHidden"] == DBNull.Value ? false : (bool)report.Row["IsHidden"]);
          this.ReportType = report.ReportType;
          this.CreatorID = report.CreatorID;
        }

        [DataMember] public int ReportID { get; set; }
        [DataMember] public int? OrganizationID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public ReportType ReportType { get; set; }
        [DataMember] public bool IsFavorite { get; set; }
        [DataMember] public bool IsHidden { get; set; }
        [DataMember] public int CreatorID { get; set; }

  

      }
    }
}