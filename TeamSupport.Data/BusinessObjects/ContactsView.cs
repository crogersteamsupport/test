using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  public partial class ContactsViewItem
  {
  }
  
  public partial class ContactsView
  {
    public void LoadByParentOrganizationID(int organizationID, string orderBy = "LastName, FirstName", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        command.CommandText = "SELECT " + limit + " * FROM ContactsView WHERE OrganizationParentID = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

	public void LoadOneByParentOrganizationID(int organizationParentId)
	{
		using (SqlCommand command = new SqlCommand())
		{
			command.CommandText = "SELECT TOP 1 * FROM ContactsView WHERE (organizationParentId = @OrganizationParentId)";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@OrganizationParentId", organizationParentId);

			Fill(command);
		}
	}

	public void LoadByParentOrganizationID(int organizationParentId, NameValueCollection filters, string orderBy = "LastName, FirstName", int? limitNumber = null)
	{
		//Get the column names, this row will be deleted before getting the actual data
		this.LoadOneByParentOrganizationID(organizationParentId);

		using (SqlCommand command = new SqlCommand())
		{
			string limit = string.Empty;

			if (limitNumber != null)
			{
				limit = "TOP " + limitNumber.ToString();
			}

			string sql = BuildLoadByParentOrganizationIdSql(limit, organizationParentId, orderBy, filters, command.Parameters);
			sql = InjectCustomFields(sql, "UserID", ReferenceType.Contacts);
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
			command.Parameters.AddWithValue("@OrganizationParentId", organizationParentId);
			this.DeleteAll();

			Fill(command);
		}
	}

    public void LoadByOrganizationID(int organizationID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ContactsView WHERE OrganizationID = @OrganizationID AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

	/// <summary>
	/// Build the sql statement including its filters to avoid using the .NET filtering. This improves performance greatly.
	/// </summary>
	/// <param name="limit">Return the specified number of rows. E.g. "TOP 1"</param>
	/// <param name="parentID">Organization Id.</param>
	/// <param name="orderBy">Fields in sql format to sort the results by. E.g. "LastName, FirstName"</param>
	/// <param name="filters">Filters to be applied. Specified in the URL request.</param>
	/// <param name="filterParameters">SqlParamenterCollection for the input parameters of the sql query.</param>
	/// <returns>A string with the full sql statement.</returns>
	public string BuildLoadByParentOrganizationIdSql(string limit, int organizationParentId, string orderBy, NameValueCollection filters, SqlParameterCollection filterParameters)
	{
		StringBuilder result = new StringBuilder();

		result.Append("SELECT " + limit + " * ");
		result.Append("FROM ContactsView ");
		result.Append("WHERE OrganizationParentID = @OrganizationParentId AND (MarkDeleted = 0) " + BuildWhereClausesFromFilters(organizationParentId, filters, ref filterParameters) + " ");
		result.Append("ORDER BY " + orderBy);

		return result.ToString();
	}

	private string BuildWhereClausesFromFilters(int organizationParentId, NameValueCollection filters, ref SqlParameterCollection filterParameters)
	{
		StringBuilder result = new StringBuilder();

		CustomFields customFields = new CustomFields(this.LoginUser);
		customFields.LoadByReferenceType(organizationParentId, ReferenceType.Contacts);

		StringBuilder filterFieldName;
		StringBuilder filterOperator;
		List<string> filterValues;
		CustomField customField = null;

		foreach (string key in filters)
		{
			var value = filters[key];

			if (!string.IsNullOrEmpty(key))
			{
				filterFieldName = new StringBuilder();
				filterOperator	= new StringBuilder();
				filterValues	= new List<string>();

				filterFieldName = GetFilterFieldName(key, filters.GetValues(key), customFields, ref filterOperator, ref filterValues, ref customField);

				if (filterFieldName.Length > 0)
				{
					result.Append(" AND ");

					if (customField == null)
					{
						if (filterValues.Count > 1)
							result.Append("(");

						if (filterValues[0] == null)
						{
							string notEmptyOperator = filterOperator.ToString().ToLower() == "is not" ? "<>" : "=";
							result.Append("(");
							result.Append(filterFieldName + " " + filterOperator + " NULL");
							result.Append(" OR ");
							result.Append(filterFieldName + " " + notEmptyOperator + " ''");
							result.Append(")");
						}
						else
						{
							result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName);
							filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);
						}
						

						if (filterValues.Count > 1)
						{
							for (int j = 1; j < filterValues.Count; j++)
							{
								result.Append(" OR ");

								if (filterValues[j] == null)
								{
									string notEmptyOperator = filterOperator.ToString().ToLower() == "is not" ? "<>" : "=";
									result.Append("(");
									result.Append(filterFieldName + " " + filterOperator + " NULL");
									result.Append(" OR ");
									result.Append(filterFieldName + " " + notEmptyOperator + " ''");
									result.Append(")");
								}
								else
								{
									result.Append(filterFieldName + " " + filterOperator + " @" + filterFieldName + j.ToString());
									filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);
								}
							}

							result.Append(")");
						}
					}
					else
					{
						result.Append("OrganizationID IN (SELECT RefID FROM CustomValues WHERE CustomFieldID = ");
						result.Append(customField.CustomFieldID.ToString());
						result.Append(" AND ");
						if (filterValues.Count > 1) result.Append("(");
						result.Append("CustomValue " + filterOperator + " @" + filterFieldName);
						filterParameters.AddWithValue("@" + filterFieldName, filterValues[0]);

						if (filterValues.Count > 1)
						{
							for (int j = 1; j < filterValues.Count; j++)
							{
								result.Append(" OR ");
								result.Append("CustomValue " + filterOperator + " @" + filterFieldName + j.ToString());
								filterParameters.AddWithValue("@" + filterFieldName + j.ToString(), filterValues[j]);
							}
							result.Append(")");
						}
					}
				}
			}
		}

		return result.ToString();
	}

	private StringBuilder GetFilterFieldName(string rawFieldName, string[] rawValues, CustomFields customFields, ref StringBuilder filterOperator, ref List<string> filterValues, ref CustomField customField)
	{
		StringBuilder result = new StringBuilder();
		string	rawOperator = "=";

		if (rawFieldName.Contains('['))
		{
			int index = rawFieldName.IndexOf('[');
			result.Append(rawFieldName.Substring(0, index));
			rawOperator = Regex.Match(rawFieldName, @"\[([^]]*)\]").Groups[1].Value;
		}
		else
		{
			result.Append(rawFieldName);
		}

		Type filterFieldDataType = GetFilterFieldDataType(result.ToString(), customFields, ref customField);

		if (filterFieldDataType != null)
		{
			filterOperator.Append(GetSqlOperator(filterFieldDataType, rawOperator, rawValues, ref filterValues));

			if (filterOperator.Length == 0)
			{
				result.Clear();
			}
		}
		else
		{
			result.Clear();
		}

		return result;
	}

	private Type GetFilterFieldDataType(string fieldName, CustomFields customFields, ref CustomField customField)
	{
		Type fieldDataType = null;
		string field = FieldMap.GetPrivateField(fieldName);

		if (!string.IsNullOrEmpty(field))
		{
			BaseItem baseItem = new BaseItem(Table.Rows[0], this);
			object fieldObject = baseItem.Row[field];

			if (fieldObject != null)
			{
				customField = null;
				fieldDataType = baseItem.Row.Table.Columns[field].DataType;
			}
		}
		else
		{
			customField = customFields.FindByApiFieldName(fieldName);

			if (customField != null)
			{
				switch (customField.FieldType)
				{
					case CustomFieldType.Boolean:
						Boolean boolean = true;
						fieldDataType = boolean.GetType();
						break;
					case CustomFieldType.Date:
					case CustomFieldType.DateTime:
					case CustomFieldType.Time:
						DateTime dateTime = DateTime.Now;
						fieldDataType = dateTime.GetType();
						break;
					case CustomFieldType.Number:
						int integer = 0;
						fieldDataType = integer.GetType();
						break;
					case CustomFieldType.PickList:
					case CustomFieldType.Text:
						string text = string.Empty;
						fieldDataType = text.GetType();
						break;
					default:
						fieldDataType = null;
						break;
				}
			}
		}

		return fieldDataType;
	}

	private string GetSqlOperator(Type filterFieldDataType, string rawOperator, string[] rawValues, ref List<string> filterValues)
	{
		StringBuilder result = new StringBuilder();

		for (int i = 0; i < rawValues.Length; i++)
		{
			if (rawValues[i].ToLower() == "[null]")
			{
				if (i == 0)
				{
					if (rawOperator.ToLower() == "not")
					{
						result.Append("IS NOT");
					}
					else
					{
						result.Append("IS");
					}
				}

				filterValues.Add(null);
			}
			else
			{
				if (filterFieldDataType == typeof(System.DateTime))
				{
					//format needs to be: yyyymmddhhmmss
					if (rawValues[i].Length == "yyyymmddhhmmss".Length)
					{
						StringBuilder filterValue = new StringBuilder();
						//sql default datetime format "yyyy-mm-dd hh:mm:ss"
						//yyyy
						filterValue.Append(rawValues[i].Substring(0, 4));
						filterValue.Append("-");
						//mm
						filterValue.Append(rawValues[i].Substring(4, 2));
						filterValue.Append("-");
						//dd
						filterValue.Append(rawValues[i].Substring(6, 2));
						filterValue.Append(" ");
						//hh
						filterValue.Append(rawValues[i].Substring(8, 2));
						filterValue.Append(":");
						//mm
						filterValue.Append(rawValues[i].Substring(10, 2));
						filterValue.Append(":");
						//ss
						filterValue.Append(rawValues[i].Substring(12, 2));

						filterValues.Add(filterValue.ToString());

						if (i == 0)
						{
							if (rawOperator == "lt")
							{
								result.Append("<");
							}
							else
							{
								result.Append(">");
							}
						}
					}
				}
				else if (filterFieldDataType == typeof(System.Boolean))
				{
					if (rawValues[i].ToLower().IndexOf("t") > -1 || rawValues[i].ToLower().IndexOf("1") > -1 || rawValues[i].ToLower().IndexOf("y") > -1)
					{
						filterValues.Add("1");
					}
					if (i == 0)
					{
						result.Append("=");
					}
				}
				else if (filterFieldDataType == typeof(System.Double))
				{
					double d = double.Parse(rawValues[i]);
					filterValues.Add(d.ToString());

					if (i == 0)
					{
						switch (rawOperator)
						{
							case "lt": result.Append("<"); break;
							case "lte": result.Append("<="); break;
							case "gt": result.Append(">"); break;
							case "gte": result.Append(">="); break;
							case "not": result.Append("<>"); break;
							default: result.Append("="); break;
						}
					}
				}
				else if (filterFieldDataType == typeof(System.Int32))
				{
					int j = int.Parse(rawValues[i]);
					filterValues.Add(j.ToString());

					if (i == 0)
					{
						switch (rawOperator)
						{
							case "lt": result.Append("<"); break;
							case "lte": result.Append("<="); break;
							case "gt": result.Append(">"); break;
							case "gte": result.Append(">="); break;
							case "not": result.Append("<>"); break;
							default: result.Append("="); break;
						}
					}
				}
				else
				{
					switch (rawOperator)
					{
						case "contains":
							if (i == 0) result.Append("LIKE");
							filterValues.Add("%" + rawValues[i] + "%");
							break;
						case "not":
							if (i == 0) result.Append("<>");
							filterValues.Add(rawValues[i]);
							break;
						default:
							if (i == 0) result.Append("=");
							filterValues.Add(rawValues[i]);
							break;
					}
				}
			}
		}

		return result.ToString();
	}

    public void LoadByTicketID(int ticketID, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public ContactsViewItem FindBySalesForceID(string salesForceID, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.SalesForceID != null && contactsViewItem.SalesForceID.Trim().ToLower() == salesForceID.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public ContactsViewItem FindByEmail(string email, string organizationSalesForceID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        string contactsViewItemOrganizationSalesForceID = Organizations.GetOrganization(this.LoginUser, contactsViewItem.OrganizationID).CRMLinkID;
        if (contactsViewItemOrganizationSalesForceID != null)
        {
          if (contactsViewItem.Email.Trim().ToLower() == email.Trim().ToLower())
          {
            if (contactsViewItemOrganizationSalesForceID == organizationSalesForceID)
            {
              return contactsViewItem;
            }
          }
        }
      }
      return null;
    }

    public void LoadByTicketIDOrderedByDateCreated(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadSentToSalesForce(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT cv.* FROM ContactsView cv LEFT JOIN UserTickets ut ON ut.UserID = cv.UserID WHERE ut.SentToSalesForce = 1 AND ut.TicketID = @TicketID AND (cv.MarkDeleted = 0) ORDER BY ut.DateCreated ";
        command.CommandText = InjectCustomFields(command.CommandText, "cv.UserID", ReferenceType.Contacts);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE cv.NeedsIndexing = 1
        AND cv.MarkDeleted = 0
        AND o.ParentID = @OrganizationID
        ORDER BY cv.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
        SELECT cv.UserID
        FROM ContactsView cv WITH(NOLOCK)
        JOIN Organizations o WITH(NOLOCK)
          ON cv.OrganizationID = o.OrganizationID
        WHERE o.ParentID = @OrganizationID
        AND cv.MarkDeleted = 0
        ORDER BY cv.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }

  public class CustomerSearchContact
  {
    public CustomerSearchContact() { }
    public CustomerSearchContact(ContactsViewItem item)
    {
      userID = item.UserID;
      organizationID = item.OrganizationID;
      email = item.Email;
      title = item.Title;
      organization = item.Organization;
      fName = item.FirstName;
      lName = item.LastName;
      isPortal = item.IsPortalUser;
    }

    public int userID { get; set; }
    public int organizationID { get; set; }
    public string email { get; set; }
    public string title { get; set; }
    public string organization { get; set; }
    public string fName { get; set; }
    public string lName { get; set; }
    public bool isPortal { get; set; }
    public int openTicketCount { get; set; }
    public CustomerSearchPhone[] phones { get; set; }
  }
  
}
