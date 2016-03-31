using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace TeamSupport.Data
{
  public partial class Attachment 
  {
    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }

    public void DeleteFile()
    {
      try { File.Delete(Path); }
      catch (Exception) { }
    
    }
  }

  public partial class Attachments
  {

    public void LoadByAttachmentGUID(Guid attachmentGUID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.* FROM Attachments a WHERE AttachmentGUID = @attachmentGUID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@attachmentGUID", attachmentGUID);
        Fill(command);
      }

    }

    public static Attachment GetAttachment(LoginUser loginUser, Guid attachmentGUID)
    {
      Attachments attachments = new Attachments(loginUser);
      attachments.LoadByAttachmentGUID(attachmentGUID);
      if (attachments.IsEmpty)
        return null;
      else
        return attachments[0];
    }

    public void LoadByActionID(int actionID, string orderBy = "")
    {
      LoadByReference(ReferenceType.Actions, actionID, orderBy);
    }

    public void LoadByWatercoolerID(int WaterCoolerID)
    {
        LoadByReference(ReferenceType.WaterCooler, WaterCoolerID);
    }

    public void LoadByOrganizationID(int organizationID)
    {
      LoadByReference(ReferenceType.Organizations, organizationID);
    }

    public void LoadByReference(ReferenceType refType, int refID, string orderBy = "", bool includeCompanyChildren = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT 
                a.*
                , (u.FirstName + ' ' + u.LastName) AS CreatorName 
            FROM 
                Attachments a 
                LEFT JOIN Users u 
                    ON u.UserID = a.CreatorID 
            WHERE 
                RefType = @RefType
                AND RefID IN
                (
                    SELECT
                        @RefID
                    UNION
                    SELECT
                        CustomerID
                    FROM
                        CustomerRelationships
                    WHERE
                        RelatedCustomerID = @RefID
                        AND @IncludeCompanyChildren = 1
                )";
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@IncludeCompanyChildren", includeCompanyChildren);
        Fill(command);
      }
    }

    public void LoadForJira(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT a.*, (u.FirstName + ' ' + u.LastName) AS CreatorName FROM Attachments a LEFT JOIN Users u ON u.UserID = a.CreatorID WHERE (RefID = @RefID) AND (RefType = 0) AND SentToJira = 0";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", actionID);
        Fill(command);
      }
    }

    public void TempLoadFix()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
select a.* from attachments a 
left join organizations o on o.OrganizationID=a.OrganizationID
where a.RefType=0 
--and o.OrganizationID in (305383, 421709 )
and a.DateCreated > '2012-04-24 13:03:25.980'
and ISNULL(a.path, '') = ''
--and FileType not like '%image%'
order by o.Name, a.DateCreated desc
";

        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static void DeleteAttachmentAndFile(LoginUser loginUser, int attachmentID)
    {
      Attachment attachment = GetAttachment(loginUser, attachmentID);
      if (attachment != null)
      {
        try { File.Delete(attachment.Path); } catch (Exception) {  }
        attachment.Delete();
        attachment.Collection.Save();
      }
    }

    public static string GetAttachmentPath(LoginUser loginUser, ReferenceType refType, int refID)
    {
      string root = AttachmentPath.GetRoot(loginUser, loginUser.OrganizationID);
      string type = null;
      switch (refType)
      {
        case ReferenceType.None:
          break;
        case ReferenceType.Actions: type = "Actions"; break;
        case ReferenceType.ActionTypes:
          break;
        case ReferenceType.Addresses:
          break;
        case ReferenceType.Attachments:
          break;
        case ReferenceType.CustomFields:
          break;
        case ReferenceType.CustomValues:
          break;
        case ReferenceType.Groups:
          break;
        case ReferenceType.GroupUsers:
          break;
        case ReferenceType.OrganizationProducts:
          break;
        case ReferenceType.Organizations: type = "OrganizationAttachments"; break;
        case ReferenceType.OrganizationTickets:
          break;
        case ReferenceType.PhoneNumbers:
          break;
        case ReferenceType.PhoneTypes:
          break;
        case ReferenceType.Products:
          break;
        case ReferenceType.ProductVersions:
          break;
        case ReferenceType.ProductVersionStatuses:
          break;
        case ReferenceType.TechDocs:
          break;
        case ReferenceType.Tickets:
          break;
        case ReferenceType.TicketSeverities:
          break;
        case ReferenceType.TicketStatuses:
          break;
        case ReferenceType.Subscriptions:
          break;
        case ReferenceType.TicketTypes:
          break;
        case ReferenceType.Users:
          break;
        case ReferenceType.ActionLogs:
          break;
        case ReferenceType.BillingInfo:
          break;
        case ReferenceType.ExceptionLogs:
          break;
        case ReferenceType.Invoices:
          break;
        case ReferenceType.SystemSettings:
          break;
        case ReferenceType.TicketNextStatuses:
          break;
        case ReferenceType.UserSettings:
          break;
        case ReferenceType.TicketQueue:
          break;
        case ReferenceType.CreditCards:
          break;
        case ReferenceType.Contacts:
          break;
        case ReferenceType.UserPhoto:
          type = "UserPhoto";
          break;
        default:
          break;
      }

      if (type == null) throw new Exception("Error: Cannot convert reference type to string.");

      return Path.Combine(Path.Combine(root, type), refID.ToString()) + "\\";
    }

    public static string VerifyUniqueFileName(string directory, string fileName)
    {
      string path = Path.Combine(directory, fileName);
      string result = fileName;
      int i = 0;
      while (File.Exists(path))
      {
        i++;
        if (i > 20) break;
        string name = Path.GetFileNameWithoutExtension(fileName);
        string ext = Path.GetExtension(fileName);
        result = name + " (" + i.ToString() + ")" + ext;
        path = Path.Combine(directory, result);
      }

      return result;
    }
    

  }
}
