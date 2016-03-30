using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CRMLinkError
  {
  }
  
  public partial class CRMLinkErrors
  {
		public void LoadByTicketID(int ticketId, string crmType, bool? isCleared)
		{
			using (SqlCommand command = new SqlCommand())
			{
				string sql = @"SELECT *
										FROM
											CRMLinkErrors
										WHERE
											ObjectID = @ObjectId
											AND CRMType = @crmType
											{0}
										ORDER BY DateModified DESC";
				sql = string.Format(sql, isCleared != null ? "AND isCleared = @isCleared" : string.Empty);
				command.CommandText = sql;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ObjectId", ticketId);
				command.Parameters.AddWithValue("@crmType", crmType);

				if (isCleared != null)
				{
					command.Parameters.AddWithValue("@isCleared", (bool)isCleared);
				}

				Fill(command);
			}
		}

		public void LoadByOperation(int organizationID, string CRMType, string orientation, string objectType, bool? isCleared = null)
		{
			using (SqlCommand command = new SqlCommand())
			{
				string sql = @"SELECT * 
				FROM 
					CRMLinkErrors
				WHERE 
					OrganizationID = @OrganizationID
					AND CRMType = @CRMType
					AND Orientation = @Orientation
					AND ObjectType = @ObjectType
					{0}";

				sql = string.Format(sql, isCleared != null ? "AND IsCleared = @isCleared" : string.Empty);
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				command.Parameters.AddWithValue("@CRMType", CRMType);
				command.Parameters.AddWithValue("@Orientation", orientation);
				command.Parameters.AddWithValue("@ObjectType", objectType);

				if (isCleared != null)
				{
					command.Parameters.AddWithValue("@isCleared", (bool)isCleared);
				}

				command.CommandText = sql;

				Fill(command, "CRMLinkErrors");
			}
		}

		public void LoadByOperationAndObjectId(int organizationID, string CRMType, string orientation, string objectType, string objectId)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT * 
				FROM 
					CRMLinkErrors
				WHERE 
					OrganizationID = @OrganizationID
					AND CRMType = @CRMType
					AND Orientation = @Orientation
					AND ObjectType = @ObjectType
					AND ObjectID = @ObjectId";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				command.Parameters.AddWithValue("@CRMType", CRMType);
				command.Parameters.AddWithValue("@Orientation", orientation);
				command.Parameters.AddWithValue("@ObjectType", objectType);
				command.Parameters.AddWithValue("@ObjectId", objectId);

				Fill(command, "CRMLinkErrors");
			}
		}

		public void LoadByOperationAndObjectIds(int organizationID, string CRMType, string orientation, string objectType, List<string> objectIds, bool? isCleared = null)
		{
			string sql = @"SELECT * 
			FROM 
				CRMLinkErrors
			WHERE 
				OrganizationID = @OrganizationID
				AND CRMType = @CRMType
				AND Orientation = @Orientation
				AND ObjectType = @ObjectType
				{0}
				{1}";

			sql = string.Format(sql, objectIds != null && objectIds.Any() && objectIds.Count > 0 ? string.Format(" AND ObjectID IN ({0}) ", string.Join(", ", objectIds.Select(p => string.Format("'{0}'", p)).ToArray())) : "",
										isCleared != null ? " AND IsCleared = @isCleared " : string.Empty);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = sql;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				command.Parameters.AddWithValue("@CRMType", CRMType);
				command.Parameters.AddWithValue("@Orientation", orientation);
				command.Parameters.AddWithValue("@ObjectType", objectType);

				if (isCleared != null)
				{
					command.Parameters.AddWithValue("@isCleared", (bool)isCleared);
				}

				Fill(command, "CRMLinkErrors");
			}
		}

		public CRMLinkError FindByObjectIDAndFieldName(string objectID, string fieldName)
		{
			foreach (CRMLinkError item in this)
			{
				if (item.ObjectID == objectID && item.ObjectFieldName == fieldName)
				{
					return item;
				}
			}
			return null;
		}

		public CRMLinkError FindUnclearedByObjectIDAndFieldName(string objectID, string fieldName)
		{
			//If more than one (shouldn't happen) then 'clear' the older and leave just one, the more recent.
			if (this.Where(p => p.ObjectID == objectID && p.ObjectFieldName == fieldName && !p.IsCleared).Any()
				&& this.Where(p => p.ObjectID == objectID && p.ObjectFieldName == fieldName && !p.IsCleared).Count() > 1)
			{
				int latestCrmLinkErrorId = this.Where(p => p.ObjectID == objectID && p.ObjectFieldName == fieldName && !p.IsCleared).OrderByDescending(o => o.DateCreated).Select(p => p.CRMLinkErrorID).FirstOrDefault();

				foreach (var crmLinkError in this.Where(p => p.ObjectID == objectID && p.ObjectFieldName == fieldName && !p.IsCleared && p.CRMLinkErrorID != latestCrmLinkErrorId))
                {
					crmLinkError.IsCleared = true;
					crmLinkError.DateModified = DateTime.UtcNow;
					crmLinkError.Collection.Save();
                }
			}

			CRMLinkError item = this.Where(p => p.ObjectID == objectID && p.ObjectFieldName == fieldName && !p.IsCleared).OrderByDescending(o => o.DateCreated).FirstOrDefault();
			return item;
		}

		/// <summary>
		/// Used to clear all required field specific Errors not cleared yet, by TicketId. That are not required anymore in Jira.
		/// </summary>
		/// <param name="objectID">TicketId</param>
		/// <param name="requiredFieldsErrorsToClear">List of fields required in Jira</param>
		public void ClearRequiredFieldErrors(string objectID, List<string> requiredFieldsErrorsToClear)
		{
			List<CRMLinkError> fieldErrors = this.Where(p => p.ObjectID == objectID
														&& p.Orientation.ToLower() == "out"
														&& !p.IsCleared
														&& !string.IsNullOrEmpty(p.ObjectFieldName)
														&& !requiredFieldsErrorsToClear.Contains(p.ObjectFieldName)).ToList();

			foreach (CRMLinkError fieldError in fieldErrors)
			{
				if (fieldError != null)
				{
					fieldError.IsCleared = true;
					fieldError.DateModified = DateTime.UtcNow;
					fieldError.Collection.Save();
				}
			}
		}

		public List<ActionLogProxy> TranslateToActionLog()
		{
			List<ActionLogProxy> actionLogProxyList = new List<ActionLogProxy>();
			List<CRMLinkErrorProxy> crmLinkErrorProxy = GetCRMLinkErrorProxies().ToList();

			foreach (CRMLinkErrorProxy crmLinkError in GetCRMLinkErrorProxies().ToList())
			{
				ActionLogProxy actionLogProxy = new ActionLogProxy();
				actionLogProxy.ModifierID = (int)SystemUser.CRM;
				actionLogProxy.CreatorID = (int)SystemUser.CRM;
				actionLogProxy.Description = string.Format("{0}{1}",
															crmLinkError.ErrorMessage,
															crmLinkError.Orientation.ToLower() == "in" ? " (" + crmLinkError.ErrorCount + ")" : "");
				actionLogProxy.ActionLogType = crmLinkError.OperationType == "create" ? ActionLogType.Insert : ActionLogType.Update;
				actionLogProxy.RefType = crmLinkError.ObjectType == "ticket" ? ReferenceType.Tickets : crmLinkError.ObjectType == "action" ? ReferenceType.Actions : crmLinkError.ObjectType == "attachment" ? ReferenceType.Attachments : ReferenceType.Tickets;
				actionLogProxy.OrganizationID = crmLinkError.OrganizationID;
				actionLogProxy.ActionLogID = crmLinkError.CRMLinkErrorID;
				actionLogProxy.CreatorName = crmLinkError.CRMType + " Integration";
				actionLogProxy.DateCreated = DateTime.SpecifyKind(crmLinkError.DateCreated, DateTimeKind.Utc);
				actionLogProxy.DateModified = DateTime.SpecifyKind(crmLinkError.DateModified, DateTimeKind.Utc);

				int objectId = 0;
				bool isNumber = int.TryParse(crmLinkError.ObjectID, out objectId);
				actionLogProxy.RefID = objectId;

				actionLogProxyList.Add(actionLogProxy);
			}
			

			return actionLogProxyList;
        }
	}
  
}
