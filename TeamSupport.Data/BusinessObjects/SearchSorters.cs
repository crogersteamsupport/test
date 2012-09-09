using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SearchSorter
  {
  }
  
  public partial class SearchSorters
  {
    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM dbo.SearchSorters WHERE UserID = @UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "SearchSorters");
      }
    }

    public string ConvertToOrderByClause(ref string tempItemsTableFieldsDefinition, 
                                         ref string tempItemsTableFields, 
                                         ref string selectTicketsFields,
                                         ref string selectWikisFields)
    {
      StringBuilder resultBuilder = new StringBuilder();

      if (this.Count > 0)
      {
        StringBuilder tempItemsTableFieldsDefinitionBuilder = new StringBuilder();
        StringBuilder tempItemsTableFieldsBuilder           = new StringBuilder();
        StringBuilder selectTicketsFieldsBuilder            = new StringBuilder();
        StringBuilder selectWikisFieldsBuilder              = new StringBuilder();

        WikiArticlesView wikiArticleViewFields = new WikiArticlesView(base.LoginUser);
        wikiArticleViewFields.LoadColumnNames();

        resultBuilder.Append(" ORDER BY ");

        ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
        ticketsViewFields.LoadByReportTableID(10);

        for (int i = 0; i < this.Count; i++)
        {
          if (i > 0)
          {
            resultBuilder.Append(", ");
          }

          string fieldName = ticketsViewFields.FindByReportTableFieldID(this[i].FieldID).FieldName;
          resultBuilder.Append("[" + fieldName + "] " + (this[i].Descending ? "DESC" : string.Empty));

          if (!string.Equals(fieldName, "DateModified", StringComparison.OrdinalIgnoreCase))
          {
            string fieldType = ticketsViewFields.FindByReportTableFieldID(this[i].FieldID).DataType;
            if (string.Equals(fieldType, "varchar", StringComparison.OrdinalIgnoreCase))
            {
              fieldType += "(8000)";
            }
            tempItemsTableFieldsDefinitionBuilder.Append(", " + fieldName + " " + fieldType);
            tempItemsTableFieldsBuilder.Append(", " + fieldName);
            selectTicketsFieldsBuilder.Append(", tv." + fieldName);

            string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);

            if (!DataUtils.GetIsColumnInBaseCollection(wikiArticleViewFields, wikiEquivalentFieldName))
            {
              selectWikisFieldsBuilder.Append(", NULL AS " + fieldName);
            }
            else
            {
              selectWikisFieldsBuilder.Append(", wav." + wikiEquivalentFieldName + " AS " + fieldName);
            }
          }
        }

        tempItemsTableFieldsDefinition = tempItemsTableFieldsDefinitionBuilder.ToString();
        tempItemsTableFields           = tempItemsTableFieldsBuilder.ToString();
        selectTicketsFields            = selectTicketsFieldsBuilder.ToString();
        selectWikisFields              = selectWikisFieldsBuilder.ToString();
      }
      else
      {
        resultBuilder.Append(@" 
          ORDER BY 
            relevance DESC
            , DateModified DESC 
        ");
      }

      return resultBuilder.ToString();
    }
  }
  
}
